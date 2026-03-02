/*
Copyright (C) 2021-2024, Mindee | Felix Dittrich.

This program is licensed under the Apache License 2.0.
See <https://opensource.org/licenses/Apache-2.0> for full license details.
*/
using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;
using iText.Pdfocr.Onnx.Util;

namespace iText.Pdfocr.Onnx.Detection {
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
    public class OnnxDetectionPostProcessor : BasicDetectionPostProcessor {
        /// <summary>Cached 3x3 kernel, which is used in morphological operations.</summary>
        private static readonly Mat OPENING_KERNEL = new Mat(3, 3, MatType.CV_8U, new Scalar(1.0, 1.0, 1.0, 1.0));

        /// <summary>Creates a new post-processor.</summary>
        /// <param name="binarizationThreshold">
        /// threshold value used, when binarizing a monochromatic image. If pixel value is
        /// greater or equal to the threshold, it is mapped to 1, otherwise it is mapped to 0
        /// </param>
        /// <param name="scoreThreshold">
        /// score threshold for a detected box. If score is lower than this value,
        /// the box gets discarded
        /// </param>
        public OnnxDetectionPostProcessor(float binarizationThreshold, float scoreThreshold)
            : base(AdaptBinarizationThreshold(binarizationThreshold), scoreThreshold, int.MaxValue) {
        }

        /// <summary>Creates a new post-processor with the default threshold values.</summary>
        public OnnxDetectionPostProcessor()
            : this(0.1F, 0.1F) {
        }

        /// <summary><inheritDoc/></summary>
        protected internal override VectorOfMat FindTextContours(Mat mask) {
            // OnnxTR runs an opening operation over the mask before searching for
            // contours
            Cv2.MorphologyEx(mask, mask, OpenCvSharp.MorphTypes.Open, OPENING_KERNEL);
            return base.FindTextContours(mask);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override float MapPredToSample(float pred) {
            // OnnxTR uses `expit` to normalize prediction values, as it is not
            // done in the models themselves
            return MathUtil.Expit(pred);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override double CalcTextBoxEnlargement(double width, double height) {
            double area = (width + 1.0) * (height + 1.0);
            double length = 2.0 * (width + height + 1.0);
            // Minimized from `2 * area * unclipRatio / length` with unclipRatio as 1.5
            return 3.0 * area / length;
        }

        private static float AdaptBinarizationThreshold(float value) {
            // We will actually use `logit` value of the OnnxTR threshold, this
            // way we won't need to run `expit` over the whole output buffer
            // beforehand for binarization
            return (float)MathUtil.Logit(MathUtil.Clamp(value, 0.0, 1.0));
        }
    }
}
