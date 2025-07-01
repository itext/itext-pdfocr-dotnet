/*
Copyright (C) 2021-2024, Mindee | Felix Dittrich.

This program is licensed under the Apache License 2.0.
See <https://opensource.org/licenses/Apache-2.0> for full license details.
*/
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Pdfocr.Onnxtr.Util;
using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;

namespace iText.Pdfocr.Onnxtr.Detection {
    /// <summary>
    /// Implementation of a text detection predictor post-processor, used for OnnxTR
    /// model outputs.
    /// </summary>
    /// <remarks>
    /// Implementation of a text detection predictor post-processor, used for OnnxTR
    /// model outputs.
    /// <para />
    /// Current implementation works somewhat like this:
    /// <list type="number">
    /// <item><description>Model output is binarized and then cleaned-up via erosion and dilation.
    /// </description></item>
    /// <item><description>Large-enough contours from the image in the previous step are found.
    /// </description></item>
    /// <item><description>Contours with less certainty score are discarded.
    /// </description></item>
    /// <item><description>Remaining contours are wrapped into boxes with relative [0, 1] coordinates.
    /// </description></item>
    /// </list>
    /// </remarks>
    public class OnnxDetectionPostProcessor : IDetectionPostProcessor {
        /// <summary>
        /// Coefficient used to scale, how much a box is enlarged from the ones
        /// found in a model output.
        /// </summary>
        /// <remarks>
        /// Coefficient used to scale, how much a box is enlarged from the ones
        /// found in a model output. The higher the value, the bigger the enlargement
        /// is.
        /// </remarks>
        private const float UNCLIP_RATIO = 1.5F;

        /// <summary>Cached 3x3 kernel, which is used in morphological operations.</summary>
        private static readonly Mat OPENING_KERNEL = new Mat(3, 3, MatType.CV_8UC1, new Scalar(1));

        /// <summary>Threshold value used, when binarizing a monochromatic image.</summary>
        /// <remarks>
        /// Threshold value used, when binarizing a monochromatic image. If pixel
        /// value is greater or equal to the threshold, it is mapped to 1, otherwise
        /// it is mapped to 0.
        /// </remarks>
        private readonly float binarizationThreshold;

        /// <summary>Score threshold for a detected box.</summary>
        /// <remarks>
        /// Score threshold for a detected box. If score is lower than this value,
        /// the box gets discarded.
        /// </remarks>
        private readonly float scoreThreshold;

        /// <summary>Creates a new post-processor.</summary>
        /// <param name="binarizationThreshold">
        /// threshold value used, when binarizing a monochromatic image. If pixel value is
        /// greater or equal to the threshold, it is mapped to 1, otherwise it is mapped to 0
        /// </param>
        /// <param name="scoreThreshold">
        /// score threshold for a detected box. If score is lower than this value,
        /// the box gets discarded
        /// </param>
        public OnnxDetectionPostProcessor(float binarizationThreshold, float scoreThreshold) {
            this.binarizationThreshold = binarizationThreshold;
            this.scoreThreshold = scoreThreshold;
        }

        /// <summary>Creates a new post-processor with the default threshold values.</summary>
        public OnnxDetectionPostProcessor()
            : this(0.1F, 0.1F) {
        }

        public virtual IList<iText.Kernel.Geom.Point[]> Process(IronSoftware.Drawing.AnyBitmap input, 
            FloatBufferMdArray output) {
            int height = output.GetDimension(1);
            int width = output.GetDimension(2);
            IList<iText.Kernel.Geom.Point[]> boxes = new List<iText.Kernel.Geom.Point[]>();
            // TODO DEVSIX-9153: Ideally we would want to either cache the score mask (as model
            //       dimensions won't change) or use a smaller mask with only the
            //       contour. Though based on profiling, it doesn't look like it is
            //       that bad, when it is only once per input image.
            using (Mat scoreMask = new Mat(height, width, MatType.CV_8UC1, new Scalar(0))) {
                using (VectorOfMat contours = FindTextContours(output, binarizationThreshold)) {
                    long contourCount = contours.Size;
                    for (long contourIdx = 0; contourIdx < contourCount; ++contourIdx) {
                        using (Mat contour = contours.ToArray()[contourIdx]) {
                            Rect contourBox = Cv2.BoundingRect(contour);
                            // Skip, if contour is too small
                            if (contourBox.Width < 2 || contourBox.Height < 2) {
                                continue;
                            }
                            float score = GetPredictionScore(scoreMask, output, contour, contourBox);
                            if (score < scoreThreshold) {
                                continue;
                            }
                            boxes.Add(CalculateTextBox(contour, width, height));
                        }
                    }
                }
            }
            return boxes;
        }

        private static VectorOfMat FindTextContours(FloatBufferMdArray chwMdArray, float binarizationThreshold) {
            using (Mat binaryImage = BinarizeImage(chwMdArray, binarizationThreshold)) {
                Cv2.MorphologyEx(binaryImage, binaryImage, MorphTypes.Open, OPENING_KERNEL);
                Mat[] contours;
                Mat hierarchy = new Mat();
                Cv2.FindContours(
                    binaryImage, 
                    out contours,
                    hierarchy,
                    RetrievalModes.External, 
                    ContourApproximationModes.ApproxSimple);
                return new VectorOfMat(contours);
            }
        }

        private static Mat BinarizeImage(FloatBufferMdArray chwMdArray, float binarizationThreshold) {
            System.Diagnostics.Debug.Assert(chwMdArray.GetDimensionCount() == 3 && chwMdArray.GetDimension(0) == 1);
            FloatBufferMdArray hwMdArray = chwMdArray.GetSubArray(0);
            int height = hwMdArray.GetDimension(0);
            int width = hwMdArray.GetDimension(1);
            Mat binaryImage = new Mat(height, width, MatType.CV_8UC1);
            for (int y = 0; y < height; y++) {
                FloatBufferMdArray predictionsRow = hwMdArray.GetSubArray(y);
                for (int x = 0; x < width; ++x) {
                    float prediction = predictionsRow.GetScalar(x);
                    binaryImage.Set<byte>(y, x, prediction >= binarizationThreshold ? (byte)1 : (byte)0);
                }
            }

            return binaryImage;
        }

        private static float GetPredictionScore(Mat scoreMask, FloatBufferMdArray predictions, Mat contour, Rect contourBox
            ) {
            /*
            * Algorithm here is pretty simple. We go over all the points, painted
            * by the contour shape, and calculate the mean prediction score
            * value over the original normalized output array.
            */
            FloatBufferMdArray hwMdArray = predictions.GetSubArray(0);
            int height = hwMdArray.GetDimension(0);
            int width = hwMdArray.GetDimension(1);
            double sum = 0;
            long nonZeroCount = 0;
            
            Cv2.FillPoly(scoreMask, contour, Scalar.White);

            int yBegin = Math.Max(0, contourBox.Y);
            int yEnd = Math.Min(height, contourBox.Y + contourBox.Height);
            int xBegin = Math.Max(0, contourBox.X);
            int xEnd = Math.Min(width, contourBox.X + contourBox.Width);

            for (int y = yBegin; y < yEnd; y++) {
                var predictionsRow = hwMdArray.GetSubArray(y);
                for (int x = xBegin; x < xEnd; x++) {
                    if (scoreMask.At<byte>(y, x) != 255) {
                        continue;
                    }

                    float prediction = predictionsRow.GetScalar(x);
                    if (prediction > 0) {
                        sum += prediction;
                        nonZeroCount++;
                    }

                    scoreMask.Set<byte>(y, x, 0);
                }
            }

            // Should not happen
            if (nonZeroCount == 0) {
                return 0;
            }
            return (float)(sum / nonZeroCount);
        }

        private static Point2f[] GetPaddedBox(Mat points) {
            RotatedRect rect = OpenCvUtil.NormalizeRotatedRect(Cv2.MinAreaRect(points));
            Size2f rectSize = rect.Size;
            float rectWidth = rectSize.Width;
            float rectHeight = rectSize.Height;
            float area = (rectWidth + 1) * (rectHeight + 1);
            float length = 2 * (rectWidth + rectHeight + 1);
            float expandAmount = 2 * (area * UNCLIP_RATIO / length);
            rectSize.Width = (float)MathematicUtil.Round(rectWidth + expandAmount);
            rectSize.Height = (float)MathematicUtil.Round(rectHeight + expandAmount);
            rect.Size = rectSize;
            return rect.Points();
        }

        private static iText.Kernel.Geom.Point[] CalculateTextBox(Mat points, int width, int height) {
            Point2f[] cvBox = GetPaddedBox(points);
            iText.Kernel.Geom.Point[] textBox = new iText.Kernel.Geom.Point[4];
            for (int i = 0; i < 4; ++i) {
                Point2f cvPoint = cvBox[i];
                textBox[i] = new iText.Kernel.Geom.Point(MathUtil.Clamp((double)cvPoint.X / width, 0, 1), MathUtil.Clamp
                    ((double)cvPoint.Y / height, 0, 1));
            }
            return textBox;
        }
    }
}
