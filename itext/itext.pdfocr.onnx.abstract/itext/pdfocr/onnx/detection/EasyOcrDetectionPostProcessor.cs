/*
Copyright (c) 2020 JaidedAI Authors.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System;
using System.Collections.Generic;
using iText.Pdfocr.Onnx;
using iText.Pdfocr.Onnx.Detection.Score;
using iText.Pdfocr.Onnx.Merging;

namespace iText.Pdfocr.Onnx.Detection {
    /// <summary>
    /// Implementation of a text detection predictor post-processor, used for
    /// EasyOCR model outputs.
    /// </summary>
    public class EasyOcrDetectionPostProcessor : BasicDetectionPostProcessor {
        /// <summary>Threshold for binarization of the text score array.</summary>
        private const float TEXT_BINARIZATION_THRESHOLD = 0.4F;

        /// <summary>Threshold for binarization of the link score array.</summary>
        private const float LINK_BINARIZATION_THRESHOLD = 0.4F;

        /// <summary>Creates a new post-processor.</summary>
        /// <param name="scoreThreshold">
        /// score threshold for a detected box. If score is lower than this value,
        /// the box gets discarded
        /// </param>
        public EasyOcrDetectionPostProcessor(float scoreThreshold)
            : base(1.0F, scoreThreshold, int.MaxValue) {
        }

        /// <summary>Creates a new post-processor with the default parameters.</summary>
        public EasyOcrDetectionPostProcessor()
            : this(0.7F) {
        }

        /// <summary><inheritDoc/></summary>
        public override IList<iText.Kernel.Geom.Point[]> Process(IronSoftware.Drawing.AnyBitmap input, FloatBufferMdArray
             output) {
            IList<iText.Kernel.Geom.Point[]> result = base.Process(input, output);
            return ApplyTextBoxMerger(result);
        }

        /// <summary>
        /// The text detection model from EasyOCR, for the most part, returns
        /// words or small groups of words.
        /// </summary>
        /// <remarks>
        /// The text detection model from EasyOCR, for the most part, returns
        /// words or small groups of words. Since the EasyOCR text recognition
        /// models expect lines as input, we need to merge the boxes.
        /// </remarks>
        /// <param name="detectedTextBoxes">
        /// list of rotated text boxes, provided by the
        /// text detection routine
        /// </param>
        /// <returns>a new list with merged text boxes</returns>
        protected internal virtual IList<iText.Kernel.Geom.Point[]> ApplyTextBoxMerger(IList<iText.Kernel.Geom.Point
            []> detectedTextBoxes) {
            return new EasyOcrTextBoxMerger().Process(detectedTextBoxes);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override FloatBufferMdArray GetMaskSourceArray(FloatBufferMdArray output) {
            /*
            * Mask for finding contours is based on there being either text or
            * link data. So we are creating a new buffer, where they are
            * combined.
            */
            int height = output.GetDimension(1);
            int width = output.GetDimension(2);
            int size = height * width;
            float[] textScoreBuffer = new float[size];
            output.GetSubArray(0).GetData().Get(textScoreBuffer);
            float[] linkScoreBuffer = new float[size];
            output.GetSubArray(1).GetData().Get(linkScoreBuffer);
            FloatBufferWrapper maskSourceBuffer = FloatBufferWrapper.Allocate(height * width);
            float[] mask = new float[size];
            for (int i = 0; i < size; ++i) {
                float text = textScoreBuffer[i] >= TEXT_BINARIZATION_THRESHOLD ? 1.0F : 0.0F;
                float link = linkScoreBuffer[i] >= LINK_BINARIZATION_THRESHOLD ? 1.0F : 0.0F;
                mask[i] = text + link;
            }
            maskSourceBuffer.Put(mask, 0, mask.Length);
            maskSourceBuffer.Rewind();
            return new FloatBufferMdArray(maskSourceBuffer, new long[] { height, width });
        }

        /// <summary><inheritDoc/></summary>
        protected internal override IScoreCalculator CreateScoreCalculator() {
            return new MaxScoreCalculator();
        }

        /// <summary><inheritDoc/></summary>
        protected internal override double CalcTextBoxEnlargement(double width, double height) {
            /*
            * In EasyOCR it is calculated as `2 * sqrt(area / max(w, h))`. But in
            * their case they use the area of the contour. Since we are not
            * calculating that, but are working on a box instead, it got
            * simplified further.
            */
            return 2.0 * Math.Sqrt(Math.Min(width, height));
        }
    }
}
