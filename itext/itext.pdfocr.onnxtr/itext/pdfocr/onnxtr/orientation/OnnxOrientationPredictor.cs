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
using iText.Pdfocr;
using iText.Pdfocr.Onnxtr;
using iText.Pdfocr.Onnxtr.Util;

namespace iText.Pdfocr.Onnxtr.Orientation {
    /// <summary>
    /// A crop orientation predictor implementation, which is using ONNX Runtime and
    /// its ML models to figure out, how text is oriented in a cropped image of text.
    /// </summary>
    public class OnnxOrientationPredictor : AbstractOnnxPredictor<System.Drawing.Bitmap, TextOrientation>, IOrientationPredictor {
        /// <summary>Configuration properties of the predictor.</summary>
        private readonly OnnxOrientationPredictorProperties properties;

        /// <summary>Creates a crop orientation predictor with the specified properties.</summary>
        /// <param name="properties">Properties of the predictor.</param>
        public OnnxOrientationPredictor(OnnxOrientationPredictorProperties properties)
            : base(properties.GetModelPath(), properties.GetInputProperties(), GetExpectedOutputShape(properties)) {
            this.properties = properties;
        }

        /// <summary>
        /// Creates a new crop orientation predictor using an existing pre-trained
        /// MobileNetV3 model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new crop orientation predictor using an existing pre-trained
        /// MobileNetV3 model, stored on disk. This is the only crop orientation
        /// model architecture available in OnnxTR.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/mobilenet_v3_small_crop_orientation-5620cf7e.onnx">
        /// mobilenet_v3_small_crop_orientation
        /// </a>
        /// 
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/mobilenet_v3_small_crop_orientation_static_8_bit-4cfaa621.onnx">
        /// mobilenet_v3_small_crop_orientation (8-bit quantized)
        /// </a>
        /// 
        /// </description></item>
        /// </list>
        /// 
        /// </remarks>
        /// <param name="modelPath">Path to the pre-trained model.</param>
        /// <returns>A new predictor with the MobileNetV3 model loaded.</returns>
        public static iText.Pdfocr.Onnxtr.Orientation.OnnxOrientationPredictor MobileNetV3(String modelPath) {
            return new iText.Pdfocr.Onnxtr.Orientation.OnnxOrientationPredictor(OnnxOrientationPredictorProperties.MobileNetV3
                (modelPath));
        }

        /// <summary>Returns the crop orientation predictor properties.</summary>
        /// <returns>The crop orientation predictor properties.</returns>
        public virtual OnnxOrientationPredictorProperties GetProperties() {
            return properties;
        }

        protected internal override FloatBufferMdArray ToInputBuffer(IList<System.Drawing.Bitmap> batch) {
            // Just your regular BCHW input
            return BufferedImageUtil.ToBchwInput(batch, properties.GetInputProperties());
        }

        protected internal override IList<TextOrientation> FromOutputBuffer(IList<System.Drawing.Bitmap> inputBatch
            , FloatBufferMdArray outputBatch) {
            // Just extracting the highest scoring "orientation class" for each image via argmax
            IList<TextOrientation> orientations = new List<TextOrientation>(outputBatch.GetDimension(0));
            float[] values = new float[outputBatch.GetDimension(1)];
            FloatBuffer outputBuffer = outputBatch.GetData();
            while (outputBuffer.HasRemaining()) {
                outputBuffer.Get(values);
                int label = MathUtil.Argmax(values);
                orientations.Add(properties.GetOutputMapper().Map(label));
            }
            return orientations;
        }

        private static long[] GetExpectedOutputShape(OnnxOrientationPredictorProperties properties) {
            // Dynamic batch size
            long BATCH_SIZE = -1;
            long classCount = properties.GetOutputMapper().Size();
            return new long[] { BATCH_SIZE, classCount };
        }
    }
}
