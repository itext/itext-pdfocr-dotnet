/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Pdfocr.Onnxtr;
using iText.Pdfocr.Onnxtr.Util;

namespace iText.Pdfocr.Onnxtr.Detection {
    /// <summary>
    /// A text detection predictor implementation, which is using ONNX Runtime and
    /// its ML models to find, where text is located on an image.
    /// </summary>
    public class OnnxDetectionPredictor : AbstractOnnxPredictor<IronSoftware.Drawing.AnyBitmap, IList<iText.Kernel.Geom.Point
        []>>, IDetectionPredictor {
        /// <summary>Configuration properties of the predictor.</summary>
        private readonly OnnxDetectionPredictorProperties properties;

        /// <summary>Creates a text detection predictor with the specified properties.</summary>
        /// <param name="properties">properties of the predictor</param>
        public OnnxDetectionPredictor(OnnxDetectionPredictorProperties properties)
            : base(properties.GetModelPath(), properties.GetInputProperties(), GetExpectedOutputShape(properties)) {
            this.properties = properties;
        }

        /// <summary>Creates a new text detection predictor using an existing pre-trained DBNet model, stored on disk.
        ///     </summary>
        /// <remarks>
        /// Creates a new text detection predictor using an existing pre-trained DBNet model, stored on disk.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/db_resnet50-69ba0015.onnx">
        /// db_resnet50
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/db_resnet50_static_8_bit-09a6104f.onnx">
        /// db_resnet50 (8-bit quantized)
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/db_resnet34-b4873198.onnx">
        /// db_resnet34
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/db_resnet34_static_8_bit-027e2c7f.onnx">
        /// db_resnet34 (8-bit quantized)
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.2.0/db_mobilenet_v3_large-4987e7bd.onnx">
        /// db_mobilenet_v3_large
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.2.0/db_mobilenet_v3_large_static_8_bit-535a6f25.onnx">
        /// db_mobilenet_v3_large (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new predictor with the DBNet model loaded</returns>
        public static iText.Pdfocr.Onnxtr.Detection.OnnxDetectionPredictor DbNet(String modelPath) {
            return new iText.Pdfocr.Onnxtr.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.DbNet(modelPath
                ));
        }

        /// <summary>Creates a new text detection predictor using an existing pre-trained FAST model, stored on disk.</summary>
        /// <remarks>
        /// Creates a new text detection predictor using an existing pre-trained FAST model, stored on disk.
        /// This is the default text detection model in OnnxTR.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/rep_fast_base-1b89ebf9.onnx">
        /// fast_base
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/rep_fast_small-10428b70.onnx">
        /// fast_small
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/rep_fast_tiny-28867779.onnx">
        /// fast_tiny
        /// </a>
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new predictor with the FAST model loaded</returns>
        public static iText.Pdfocr.Onnxtr.Detection.OnnxDetectionPredictor Fast(String modelPath) {
            return new iText.Pdfocr.Onnxtr.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.Fast(modelPath
                ));
        }

        /// <summary>Creates a new text detection predictor using an existing pre-trained LinkNet model, stored on disk.
        ///     </summary>
        /// <remarks>
        /// Creates a new text detection predictor using an existing pre-trained LinkNet model, stored on disk.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/linknet_resnet50-15d8c4ec.onnx">
        /// linknet_resnet50
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/linknet_resnet50_static_8_bit-65d6b0b8.onnx">
        /// linknet_resnet50 (8-bit quantized)
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/linknet_resnet34-93e39a39.onnx">
        /// linknet_resnet34
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/linknet_resnet34_static_8_bit-2824329d.onnx">
        /// linknet_resnet34 (8-bit quantized)
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/linknet_resnet18-e0e0b9dc.onnx">
        /// linknet_resnet18
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/linknet_resnet18_static_8_bit-3b3a37dd.onnx">
        /// linknet_resnet18 (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new predictor with the LinkNet model loaded</returns>
        public static iText.Pdfocr.Onnxtr.Detection.OnnxDetectionPredictor LinkNet(String modelPath) {
            return new iText.Pdfocr.Onnxtr.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.LinkNet(modelPath
                ));
        }

        /// <summary>Returns the text detection predictor properties.</summary>
        /// <returns>the text detection predictor properties</returns>
        public virtual OnnxDetectionPredictorProperties GetProperties() {
            return properties;
        }

        /// <summary><inheritDoc/></summary>
        protected internal override FloatBufferMdArray ToInputBuffer(IList<IronSoftware.Drawing.AnyBitmap> batch) {
            // Just your regular BCHW input
            return BufferedImageUtil.ToBchwInput(batch, properties.GetInputProperties());
        }

        /// <summary><inheritDoc/></summary>
        protected internal override IList<IList<iText.Kernel.Geom.Point[]>> FromOutputBuffer(IList<IronSoftware.Drawing.AnyBitmap
            > inputBatch, FloatBufferMdArray outputBatch) {
            IDetectionPostProcessor postProcessor = properties.GetPostProcessor();
            // Normalizing pixel values via a sigmoid expit function
            float[] outputBuffer = outputBatch.GetData();
            int offset = outputBatch.GetArrayOffset();
            for (int i = offset; i < offset + outputBatch.GetArraySize(); ++i) {
                outputBuffer[i] = MathUtil.Expit(outputBuffer[i]);
            }
            IList<IList<iText.Kernel.Geom.Point[]>> batchTextBoxes = new List<IList<iText.Kernel.Geom.Point[]>>(inputBatch
                .Count);
            for (int i = 0; i < inputBatch.Count; ++i) {
                IronSoftware.Drawing.AnyBitmap image = inputBatch[i];
                IList<iText.Kernel.Geom.Point[]> textBoxes = postProcessor.Process(image, outputBatch.GetSubArray(i));
                /*
                * Post-processor returns points with relative floating-point
                * coordinates in the [0, 1] range. We need to convert these to
                * absolute coordinates in the input image. This means, that we need
                * to revert resizing/padding changes as well.
                */
                ConvertToAbsoluteInputBoxes(image, textBoxes, properties.GetInputProperties());
                batchTextBoxes.Add(textBoxes);
            }
            return batchTextBoxes;
        }

        private static void ConvertToAbsoluteInputBoxes(IronSoftware.Drawing.AnyBitmap image, IList<iText.Kernel.Geom.Point
            []> boxes, OnnxInputProperties properties) {
            int sourceWidth = BufferedImageUtil.GetWidth(image);
            int sourceHeight = BufferedImageUtil.GetHeight(image);
            float targetWidth = properties.GetWidth();
            float targetHeight = properties.GetHeight();
            float widthRatio = targetWidth / sourceWidth;
            float heightRatio = targetHeight / sourceHeight;
            float widthScale;
            float heightScale;
            // We preserve ratio, when resizing input
            if (heightRatio > widthRatio) {
                heightScale = targetHeight / (float)MathematicUtil.Round(sourceHeight * widthRatio);
                widthScale = 1;
            }
            else {
                widthScale = targetWidth / (float)MathematicUtil.Round(sourceWidth * heightRatio);
                heightScale = 1;
            }
            Action<iText.Kernel.Geom.Point> updater;
            if (properties.UseSymmetricPad()) {
                updater = (p) => p.SetLocation(MathUtil.Clamp(sourceWidth * (0.5 + (p.GetX() - 0.5) * widthScale), 0, sourceWidth
                    ), MathUtil.Clamp(sourceHeight * (0.5 + (p.GetY() - 0.5) * heightScale), 0, sourceHeight));
            }
            else {
                updater = (p) => p.SetLocation(MathUtil.Clamp(sourceWidth * (p.GetX() * widthScale), 0, sourceWidth), MathUtil
                    .Clamp(sourceHeight * (p.GetY() * heightScale), 0, sourceHeight));
            }
            foreach (iText.Kernel.Geom.Point[] box in boxes) {
                foreach (iText.Kernel.Geom.Point p in box) {
                    updater(p);
                }
            }
        }

        private static long[] GetExpectedOutputShape(OnnxDetectionPredictorProperties properties) {
            OnnxInputProperties inputProperties = properties.GetInputProperties();
            // Dynamic batch size
            long BATCH_SIZE = -1;
            // Output is "monochrome"
            long CHANNEL_COUNT = 1;
            // Output retains the "image" dimension from the input
            long height = inputProperties.GetHeight();
            long width = inputProperties.GetWidth();
            return new long[] { BATCH_SIZE, CHANNEL_COUNT, height, width };
        }
    }
}
