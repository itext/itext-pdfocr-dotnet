/*
Copyright (C) 2021-2024, Mindee | Felix Dittrich.

This program is licensed under the Apache License 2.0.
See <https://opensource.org/licenses/Apache-2.0> for full license details.
*/
using System;
using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;
using iText.Commons.Utils;
using iText.Pdfocr.Onnx;
using iText.Pdfocr.Onnx.Detection.Score;
using iText.Pdfocr.Onnx.Util;

namespace iText.Pdfocr.Onnx.Detection {
    /// <summary>
    /// Implementation of a text detection predictor post-processor, which is used
    /// as a basis for creating post-processors for handling OnnxTR, EasyOCR and
    /// PaddleOCR model outputs.
    /// </summary>
    /// <remarks>
    /// Implementation of a text detection predictor post-processor, which is used
    /// as a basis for creating post-processors for handling OnnxTR, EasyOCR and
    /// PaddleOCR model outputs.
    /// <para />
    /// Base implementation works somewhat like this:
    /// <list type="number">
    /// <item><description>Model output is binarized to create a predictions mask.
    /// </description></item>
    /// <item><description>Large-enough contours from the mask in the previous step are found.
    /// </description></item>
    /// <item><description>Contours with less certainty score are discarded.
    /// </description></item>
    /// <item><description>Remaining contours are wrapped into boxes with relative [0, 1] coordinates.
    /// </description></item>
    /// </list>
    /// </remarks>
    public abstract class BasicDetectionPostProcessor : IDetectionPostProcessor {
        /// <summary>Minimum size for the contour dimensions to not be immediately filtered.</summary>
        private const int MIN_CONTOUR_SIZE = 3;

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

        /// <summary>
        /// Maximum amount of text box contours, that will be handled in the post
        /// processor.
        /// </summary>
        private readonly int maxCandidates;

        /// <summary>Creates a new post-processor.</summary>
        /// <param name="binarizationThreshold">
        /// threshold value used, when binarizing a monochromatic image. If pixel value is
        /// greater or equal to the threshold, it is mapped to 1, otherwise it is mapped to 0
        /// </param>
        /// <param name="scoreThreshold">
        /// score threshold for a detected box. If score is lower than this value,
        /// the box gets discarded
        /// </param>
        /// <param name="maxCandidates">maximum amount of text box contours, that will be handled in the post processor
        ///     </param>
        protected internal BasicDetectionPostProcessor(float binarizationThreshold, float scoreThreshold, int maxCandidates
            ) {
            this.binarizationThreshold = binarizationThreshold;
            this.scoreThreshold = scoreThreshold;
            this.maxCandidates = maxCandidates;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IList<iText.Kernel.Geom.Point[]> Process(IronSoftware.Drawing.AnyBitmap input, FloatBufferMdArray
             output) {
            FloatBufferMdArray preds = GetPredsArray(output);
            int height = preds.GetDimension(0);
            int width = preds.GetDimension(1);
            IList<iText.Kernel.Geom.Point[]> boxes = new List<iText.Kernel.Geom.Point[]>();
            using (VectorOfMat contours = FindTextContoursFromOutput(output)) {
                long contourCount = Math.Min(contours.Size, maxCandidates);
                for (long contourIdx = 0; contourIdx < contourCount; ++contourIdx) {
                    using (Mat contour = contours.ToArray()[contourIdx]) {
                        Rect contourBox = Cv2.BoundingRect(contour);
                        if (!IsValidContour(contour, contourBox)) {
                            continue;
                        }
                        float score = CalcPredictionScore(preds, contour, contourBox);
                        if (score < scoreThreshold) {
                            continue;
                        }
                        boxes.Add(CalculateTextBox(contour, width, height));
                    }
                }
            }
            return boxes;
        }

        /// <summary>Returns the preds array from the output buffer.</summary>
        /// <param name="output">output buffer from the model</param>
        /// <returns>the preds array</returns>
        protected internal virtual FloatBufferMdArray GetPredsArray(FloatBufferMdArray output) {
            // By default, we assume that output is just in CHW
            return output.GetSubArray(0);
        }

        /// <summary>
        /// Returns the array to be used, when building a mask for contour
        /// detection.
        /// </summary>
        /// <param name="output">output buffer from the model</param>
        /// <returns>the array to build the mask from</returns>
        protected internal virtual FloatBufferMdArray GetMaskSourceArray(FloatBufferMdArray output) {
            // By default, we want to build it from the preds array
            return GetPredsArray(output);
        }

        /// <summary>Extracts text contours from the provided 0 - 255 mask.</summary>
        /// <param name="mask">mask to find contours in, can be modified, should not be closed</param>
        /// <returns>found text contours</returns>
        protected internal virtual VectorOfMat FindTextContours(Mat mask) {
            Mat[] contours;
            Mat hierarchy = new Mat();
            Cv2.FindContours(
                mask, 
                out contours,
                hierarchy,
                RetrievalModes.External, 
                ContourApproximationModes.ApproxSimple);
            return new VectorOfMat(contours);
        }

        /// <summary>Returns whether the contour is good enough to be a text box.</summary>
        /// <remarks>
        /// Returns whether the contour is good enough to be a text box. Called
        /// before score calculations.
        /// </remarks>
        /// <param name="contour">contour to check</param>
        /// <param name="contourBox">bounding box of the contour to check</param>
        /// <returns>whether the contour is good enough to be a text box</returns>
        protected internal virtual bool IsValidContour(Mat contour, Rect contourBox) {
            // By default, skipping contours that are too small
            return contourBox.Width >= MIN_CONTOUR_SIZE && contourBox.Height >= MIN_CONTOUR_SIZE;
        }

        /// <summary>
        /// Builds and return a mask for calculating prediction score for the
        /// provided contour.
        /// </summary>
        /// <remarks>
        /// Builds and return a mask for calculating prediction score for the
        /// provided contour.
        /// <para />
        /// Mask should adhere to the following requirements:
        /// <list type="bullet">
        /// <item><description>Mask should have the same dimensions as the contour box.
        /// </description></item>
        /// <item><description>Data type should be CV_8U.
        /// </description></item>
        /// <item><description>Pixels, that should be counted towards the score, should have a
        /// non-zero value in the mask.
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="contour">contour to build mask for</param>
        /// <param name="contourBox">bounding box of the contour to build mask for</param>
        /// <returns>the built mask</returns>
        protected internal virtual Mat BuildTextContourPredictionMask(Mat contour, Rect contourBox) {
            int x = contourBox.X;
            int y = contourBox.Y;
            int height = contourBox.Height;
            int width = contourBox.Width;
            Mat mask = new Mat(height, width, MatType.CV_8U, OpenCvSharp.Scalar.Black);
            try {
                OpenCvUtil.FillPolyAtOffset(mask, contour, OpenCvSharp.Scalar.White, -x, -y);
                return mask;
            }
            catch (Exception e) {
                throw;
            }
        }

        /// <summary>
        /// Creates a new score calculator for calculating score over a text
        /// contour.
        /// </summary>
        /// <returns>a new score calculator</returns>
        protected internal virtual IScoreCalculator CreateScoreCalculator() {
            return new MeanScoreCalculator();
        }

        /// <summary>
        /// Calculates the score sample value, based on a prediction value from the
        /// buffer.
        /// </summary>
        /// <param name="pred">prediction value to map</param>
        /// <returns>mapped score</returns>
        protected internal virtual float MapPredToSample(float pred) {
            return pred;
        }

        /// <summary>
        /// Calculates by how much the dimensions of a text box should be enlarged
        /// compared to the ones gotten from the model output.
        /// </summary>
        /// <param name="width">original width of the text box</param>
        /// <param name="height">original height of the text box</param>
        /// <returns>value to enlarge the dimensions by</returns>
        protected internal virtual double CalcTextBoxEnlargement(double width, double height) {
            double area = width * height;
            double length = 2.0 * (width + height);
            return 2.0 * area / length;
        }

        /// <summary>Find contours using the output of the ML model.</summary>
        /// <param name="output">output buffer from the model</param>
        /// <returns>found text contours</returns>
        private VectorOfMat FindTextContoursFromOutput(FloatBufferMdArray output) {
            // Expecting CHW buffer here
            System.Diagnostics.Debug.Assert(output.GetDimensionCount() == 3);
            FloatBufferMdArray maskSource = GetMaskSourceArray(output);
            using (Mat mask = OpenCvUtil.BinarizeMdArray(maskSource, binarizationThreshold)) {
                return FindTextContours(mask);
            }
        }

        /// <summary>Calculates the prediction score for the text contour.</summary>
        /// <param name="preds">original output predictions matrix</param>
        /// <param name="contour">text contour to calculate score for</param>
        /// <param name="contourBox">bounding box of the text contour to calculate score for</param>
        /// <returns>the calculated score</returns>
        private float CalcPredictionScore(FloatBufferMdArray preds, Mat contour, Rect contourBox) {
            /*
            * Algorithm here is pretty simple. We go over all the points, marked
            * by the mask and calculate the mean prediction score value over the
            * original output array.
            */
            IScoreCalculator scoreCalculator = CreateScoreCalculator();
            int contourX = contourBox.X;
            int contourY = contourBox.Y;
            using (Mat mask = BuildTextContourPredictionMask(contour, contourBox)) {
                Mat.Indexer<byte> maskIndexer = mask.GetGenericIndexer<byte>();
                // Making sure we use correct boundaries for preds
                int yEnd = Math.Min(mask.Rows, preds.GetDimension(0) - contourY);
                int xEnd = Math.Min(mask.Cols, preds.GetDimension(1) - contourX);
                for (int y = 0; y < yEnd; ++y) {
                    FloatBufferMdArray predictionsRow = preds.GetSubArray(y + contourY);
                    for (int x = 0; x < xEnd; ++x) {
                        if (maskIndexer[y, x] == 0) {
                            continue;
                        }
                        float sample = MapPredToSample(predictionsRow.GetScalar(x + contourX));
                        scoreCalculator.Observe(sample);
                    }
                }
            }
            return scoreCalculator.Calculate();
        }

        /// <summary>
        /// Returns points of the text box quad, which has been padded using
        /// <c>calcTextBoxEnlargement</c>.
        /// </summary>
        /// <param name="points">points of the text box contour</param>
        /// <returns>the padded text box</returns>
        private Point2f[] GetPaddedBox(Mat points) {
            RotatedRect rect = OpenCvUtil.NormalizedMinAreaRect(points);
            Size2f rectSize = rect.Size;
            double rectWidth = rectSize.Width;
            double rectHeight = rectSize.Height;
            double expandAmount = CalcTextBoxEnlargement(rectWidth, rectHeight);
            rectSize.Width = (float)MathematicUtil.Round(rectWidth + expandAmount);
            rectSize.Height = (float)MathematicUtil.Round(rectHeight + expandAmount);
            rect.Size = rectSize;
            return rect.Points();
        }

        /// <summary>Calculates the 0-1 relative coordinate text box based on the contour.</summary>
        /// <param name="points">text box contour</param>
        /// <param name="width">width of the predictions mask</param>
        /// <param name="height">height of the predictions mask</param>
        /// <returns>the calculated text box</returns>
        private iText.Kernel.Geom.Point[] CalculateTextBox(Mat points, int width, int height) {
            Point2f[] cvBox = GetPaddedBox(points);
            iText.Kernel.Geom.Point[] textBox = new iText.Kernel.Geom.Point[4];
            for (int i = 0; i < 4; ++i) {
                Point2f cvPoint = cvBox[i];
                    // Coordinates are relative on an [0, 1] scale, so that it
                    // is easier to map back to the input image.
                    textBox[i] = new iText.Kernel.Geom.Point(MathUtil.Clamp((double)cvPoint.X / width, 0, 1), 
                        MathUtil.Clamp((double)cvPoint.Y / height, 0, 1));
            }

            return textBox;
        }
    }
}
