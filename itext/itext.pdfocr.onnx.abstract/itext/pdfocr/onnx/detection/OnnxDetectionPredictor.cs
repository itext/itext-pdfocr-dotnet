/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Pdfocr.Onnx;
using iText.Pdfocr.Onnx.Util;

namespace iText.Pdfocr.Onnx.Detection {
    /// <summary>
    /// A text detection predictor implementation, which is using ONNX Runtime and
    /// its ML models to find, where text is located on an image.
    /// </summary>
    public class OnnxDetectionPredictor : AbstractOnnxPredictor<IronSoftware.Drawing.AnyBitmap, IList<iText.Kernel.Geom.Point
        []>>, IDetectionPredictor {
        /// <summary>The expected output shape (BCHW).</summary>
        /// <remarks>
        /// The expected output shape (BCHW).
        /// <para />
        /// Batch size is dynamic, as usual, so -1 there.
        /// <para />
        /// For channels, ideally, there is just one "monochrome" image, but some
        /// models put multiple different metrics in one output (ex. EasyOCR
        /// returns 2), so we will assume dynamic size here as well.
        /// <para />
        /// As for height and width, while in OnnxTR the dimensions are static and
        /// are equal to the input image dimensions, this is not the case
        /// everywhere. For example, in EasyOCR output is quarter of the input
        /// resolution, but still static. On the other hand, in PaddleOCR, input
        /// and output resolutions are the same, but they are dynamic. So we cannot
        /// statically check this here without knowing the exact dimensions of the
        /// input.
        /// <para />
        /// Overall, this means, that the dimension checks for the output of the
        /// models are useless here, except for checking, that there are 4
        /// dimensions...
        /// </remarks>
        private static readonly long[] EXPECTED_OUTPUT_SHAPE = new long[] { -1, -1, -1, -1 };

        /// <summary>Configuration properties of the predictor.</summary>
        private readonly OnnxDetectionPredictorProperties properties;

        /// <summary>Creates a text detection predictor with the specified properties.</summary>
        /// <param name="properties">properties of the predictor</param>
        public OnnxDetectionPredictor(OnnxDetectionPredictorProperties properties)
            : base(properties, EXPECTED_OUTPUT_SHAPE) {
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
        /// <para />
        /// These models output boxes of words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new predictor with the DBNet model loaded</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor DbNet(String modelPath) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.DbNet(modelPath
                ));
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
        /// <para />
        /// These models output boxes of words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor with the DBNet model loaded</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor DbNet(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.DbNet(modelPath
                , ortSessionOptionsCreator));
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
        /// <para />
        /// These models output boxes of words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new predictor with the FAST model loaded</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor Fast(String modelPath) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.Fast(modelPath
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
        /// <para />
        /// These models output boxes of words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor with the FAST model loaded</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor Fast(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.Fast(modelPath
                , ortSessionOptionsCreator));
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
        /// <para />
        /// These models output boxes of words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new predictor with the LinkNet model loaded</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor LinkNet(String modelPath) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.LinkNet(modelPath
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
        /// <para />
        /// These models output boxes of words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor with the LinkNet model loaded</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor LinkNet(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.LinkNet(modelPath
                , ortSessionOptionsCreator));
        }

        /// <summary>
        /// Creates a new text detection predictor using an existing pre-trained
        /// PaddleOCR model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection predictor using an existing pre-trained
        /// PaddleOCR model, stored on disk.
        /// <para />
        /// Only models in the ONNX format are supported. Since, by default,
        /// PaddleOCR does not provide models in the ONNX format, you might need to
        /// do a model conversion yourself. Check out
        /// <a href="https://www.paddleocr.ai/latest/en/version3.x/deployment/obtaining_onnx_models.html">this page</a>
        /// for information on how to do that.
        /// <para />
        /// This method expects the directory to contain two files:
        /// <list type="bullet">
        /// <item><description>
        /// <c>inference.onnx</c>
        /// - the inference model in the ONNX format
        /// </description></item>
        /// <item><description>
        /// <c>inference.yml</c>
        /// - the configuration file for the model in YAML
        /// </description></item>
        /// </list>
        /// <para />
        /// This method can be used to load the following PaddleOCR models:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv5_server_det_infer.tar">
        /// PP-OCRv5_server_det
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv5_mobile_det_infer.tar">
        /// PP-OCRv5_mobile_det
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv4_server_det_infer.tar">
        /// PP-OCRv4_server_det
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv4_mobile_det_infer.tar">
        /// PP-OCRv4_mobile_det
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models output boxes of text lines. Make sure you choose a
        /// recognition model that can handle spaces.
        /// </remarks>
        /// <param name="modelDirPath">
        /// path to the directory with the model and its
        /// configuration file
        /// </param>
        /// <returns>a new predictor with the PaddleOCR model loaded</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor PaddleOcr(String modelDirPath) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.PaddleOcr(modelDirPath
                ));
        }

        /// <summary>
        /// Creates a new text detection predictor using an existing pre-trained
        /// PaddleOCR model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection predictor using an existing pre-trained
        /// PaddleOCR model, stored on disk.
        /// <para />
        /// Only models in the ONNX format are supported. Since, by default,
        /// PaddleOCR does not provide models in the ONNX format, you might need to
        /// do a model conversion yourself. Check out
        /// <a href="https://www.paddleocr.ai/latest/en/version3.x/deployment/obtaining_onnx_models.html">this page</a>
        /// for information on how to do that.
        /// <para />
        /// This method expects the directory to contain two files:
        /// <list type="bullet">
        /// <item><description>
        /// <c>inference.onnx</c>
        /// - the inference model in the ONNX format
        /// </description></item>
        /// <item><description>
        /// <c>inference.yml</c>
        /// - the configuration file for the model in YAML
        /// </description></item>
        /// </list>
        /// <para />
        /// This method can be used to load the following PaddleOCR models:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv5_server_det_infer.tar">
        /// PP-OCRv5_server_det
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv5_mobile_det_infer.tar">
        /// PP-OCRv5_mobile_det
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv4_server_det_infer.tar">
        /// PP-OCRv4_server_det
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv4_mobile_det_infer.tar">
        /// PP-OCRv4_mobile_det
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models output boxes of text lines. Make sure you choose a
        /// recognition model that can handle spaces.
        /// </remarks>
        /// <param name="modelDirPath">
        /// path to the directory with the model and its
        /// configuration file
        /// </param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor with the PaddleOCR model loaded</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor PaddleOcr(String modelDirPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.PaddleOcr(modelDirPath
                , ortSessionOptionsCreator));
        }

        /// <summary>
        /// Creates a new text detection predictor using an existing pre-trained
        /// PaddleOCR model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection predictor using an existing pre-trained
        /// PaddleOCR model, stored on disk.
        /// <para />
        /// Only models in the ONNX format are supported. Since, by default,
        /// PaddleOCR does not provide models in the ONNX format, you might need to
        /// do a model conversion yourself. Check out
        /// <a href="https://www.paddleocr.ai/latest/en/version3.x/deployment/obtaining_onnx_models.html">this page</a>
        /// for information on how to do that.
        /// <para />
        /// This method can be used to load the following PaddleOCR models:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv5_server_det_infer.tar">
        /// PP-OCRv5_server_det
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv5_mobile_det_infer.tar">
        /// PP-OCRv5_mobile_det
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv4_server_det_infer.tar">
        /// PP-OCRv4_server_det
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv4_mobile_det_infer.tar">
        /// PP-OCRv4_mobile_det
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models output boxes of text lines. Make sure you choose a
        /// recognition model that can handle spaces.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model in the ONNX format</param>
        /// <param name="configPath">path to the configuration file for the model</param>
        /// <returns>a new predictor with the PaddleOCR model loaded</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor PaddleOcr(String modelPath, String configPath
            ) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.PaddleOcr(modelPath
                , configPath));
        }

        /// <summary>
        /// Creates a new text detection predictor using an existing pre-trained
        /// PaddleOCR model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection predictor using an existing pre-trained
        /// PaddleOCR model, stored on disk.
        /// <para />
        /// Only models in the ONNX format are supported. Since, by default,
        /// PaddleOCR does not provide models in the ONNX format, you might need to
        /// do a model conversion yourself. Check out
        /// <a href="https://www.paddleocr.ai/latest/en/version3.x/deployment/obtaining_onnx_models.html">this page</a>
        /// for information on how to do that.
        /// <para />
        /// This method can be used to load the following PaddleOCR models:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv5_server_det_infer.tar">
        /// PP-OCRv5_server_det
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv5_mobile_det_infer.tar">
        /// PP-OCRv5_mobile_det
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv4_server_det_infer.tar">
        /// PP-OCRv4_server_det
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/pp-ocrv4_mobile_det_infer.tar">
        /// PP-OCRv4_mobile_det
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models output boxes of text lines. Make sure you choose a
        /// recognition model that can handle spaces.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model in the ONNX format</param>
        /// <param name="configPath">path to the configuration file for the model</param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor with the PaddleOCR model loaded</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor PaddleOcr(String modelPath, String configPath
            , IOrtSessionOptionsCreator ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.PaddleOcr(modelPath
                , configPath, ortSessionOptionsCreator));
        }

        /// <summary>
        /// Creates a new text detection predictor using an existing pre-trained
        /// EasyOCR CRAFT model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection predictor using an existing pre-trained
        /// EasyOCR CRAFT model, stored on disk.
        /// <para />
        /// Only models in the ONNX format are supported. Since, by default,
        /// EasyOCR does not provide models in the ONNX format, you might need to
        /// do a model conversion yourself.
        /// <para />
        /// This can be used to load the following models from EasyOCR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/jaidedai/easyocr/releases/download/pre-v1.1.6/craft_mlt_25k.zip">
        /// CRAFT
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models output boxes of text lines. Make sure you choose a
        /// recognition model that can handle spaces.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new predictor with the EasyOCR CRAFT model loaded</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor EasyOcr(String modelPath) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.EasyOcr(modelPath
                ));
        }

        /// <summary>
        /// Creates a new text detection predictor using an existing pre-trained
        /// EasyOCR CRAFT model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection predictor using an existing pre-trained
        /// EasyOCR CRAFT model, stored on disk.
        /// <para />
        /// Only models in the ONNX format are supported. Since, by default,
        /// EasyOCR does not provide models in the ONNX format, you might need to
        /// do a model conversion yourself.
        /// <para />
        /// This can be used to load the following models from EasyOCR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/jaidedai/easyocr/releases/download/pre-v1.1.6/craft_mlt_25k.zip">
        /// CRAFT
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models output boxes of text lines. Make sure you choose a
        /// recognition model that can handle spaces.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor with the EasyOCR CRAFT model loaded</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor EasyOcr(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictor(OnnxDetectionPredictorProperties.EasyOcr(modelPath
                , ortSessionOptionsCreator));
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
            int batchWidth = outputBatch.GetDimension(3);
            int batchHeight = outputBatch.GetDimension(2);
            bool usedSymmetricPadding = properties.GetInputProperties().UseSymmetricPad();
            IDetectionPostProcessor postProcessor = properties.GetPostProcessor();
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
                ConvertToAbsoluteInputBoxes(image, textBoxes, batchWidth, batchHeight, usedSymmetricPadding);
                batchTextBoxes.Add(textBoxes);
            }
            return batchTextBoxes;
        }

        private static void ConvertToAbsoluteInputBoxes(IronSoftware.Drawing.AnyBitmap image, IList<iText.Kernel.Geom.Point
            []> boxes, int batchWidth, int batchHeight, bool usedSymmetricPadding) {
            int sourceWidth = BufferedImageUtil.GetWidth(image);
            int sourceHeight = BufferedImageUtil.GetHeight(image);
            double widthRatio = (double)batchWidth / sourceWidth;
            double heightRatio = (double)batchHeight / sourceHeight;
            double widthScale;
            double heightScale;
            // We preserve ratio, when resizing input
            if (heightRatio > widthRatio) {
                heightScale = batchHeight / (double)MathematicUtil.Round(sourceHeight * widthRatio);
                widthScale = 1;
            }
            else {
                widthScale = batchWidth / (double)MathematicUtil.Round(sourceWidth * heightRatio);
                heightScale = 1;
            }
            Action<iText.Kernel.Geom.Point> updater;
            if (usedSymmetricPadding) {
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
    }
}
