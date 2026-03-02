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
using System.IO;
using iText.Commons.Utils;
using iText.Pdfocr.Onnx;
using iText.Pdfocr.Onnx.Conf.Paddle.Model;
using iText.Pdfocr.Onnx.Conf.Paddle.Parser;
using iText.Pdfocr.Onnx.Exceptions;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnx.Detection {
    /// <summary>Properties for configuring text detection ONNX models.</summary>
    /// <remarks>
    /// Properties for configuring text detection ONNX models.
    /// <para />
    /// It contains a path to the model, model input properties and a model
    /// output post-processor.
    /// </remarks>
    public class OnnxDetectionPredictorProperties : AbstractOnnxPredictorProperties {
        private static readonly OnnxInputProperties DEFAULT_INPUT_PROPERTIES = new OnnxInputProperties(new ImageResizeOptions
            (ImageChannelConfiguration.RGB, 1024, 1024, PaddingStrategy.SYMMETRIC_BLACK), new float[] { 0.798F, 0.785F
            , 0.772F }, new float[] { 0.264F, 0.2749F, 0.287F });

        private static readonly IDetectionPostProcessor DEFAULT_POST_PROCESSOR = new OnnxDetectionPostProcessor();

        /*
        * By default, DBNet has different thresholds for binarization and for
        * discarding results.
        */
        private static readonly IDetectionPostProcessor DB_NET_POST_PROCESSOR = new OnnxDetectionPostProcessor(0.3F
            , 0.1F);

        private const int PADDLE_LIMIT_SIDE_LEN = 64;

        private const int PADDLE_MAX_SIDE_LIMIT = 4000;

        private const int PADDLE_SIDE_MULTIPLE = 32;

        private const int PADDLE_BATCH_SIZE = 1;

        private static readonly OnnxInputProperties EASY_OCR_INPUT_PROPERTIES = new OnnxInputProperties(/* * This will work a bit differently to what is done in EasyOCR. 
            * They first scale the image and then put it on top of a 32-multiple * black background. So in their case there will be padding on both bottom 
            * and right, where the image is padded to 32 chunks. * * In our case the image is scaled to the "multiple" canvas, so there 
            * will be padding only on one side. * * Shouldn't, really, matter that much. */ new ImageResizeOptions
            (ImageChannelConfiguration.RGB, 32, 32, 2560, 2560, 32, 32, PaddingStrategy.BOTTOM_RIGHT_BLACK), new float
            [] { 0.485F, 0.456F, 0.406F }, new float[] { 0.229F, 0.224F, 0.225F });

        private static readonly EasyOcrDetectionPostProcessor EASY_OCR_POST_PROCESSOR = new EasyOcrDetectionPostProcessor
            ();

        /// <summary>Post-processor of the outputs of the ONNX model.</summary>
        /// <remarks>
        /// Post-processor of the outputs of the ONNX model. Converts the mask-like
        /// output of the model to rotated text rectangles.
        /// </remarks>
        private readonly IDetectionPostProcessor postProcessor;

        /// <summary>Creates new text detection predictor properties.</summary>
        /// <param name="modelPath">path to the ONNX model to load</param>
        /// <param name="inputProperties">ONNX model input properties</param>
        /// <param name="postProcessor">ONNX model output post-processor</param>
        public OnnxDetectionPredictorProperties(String modelPath, OnnxInputProperties inputProperties, IDetectionPostProcessor
             postProcessor)
            : this(modelPath, inputProperties, postProcessor, DEFAULT_ORT_SESSION_CREATOR) {
        }

        /// <summary>Creates new text detection predictor properties.</summary>
        /// <param name="modelPath">path to the ONNX model to load</param>
        /// <param name="inputProperties">ONNX model input properties</param>
        /// <param name="postProcessor">ONNX model output post-processor</param>
        /// <param name="ortSessionOptionsCreator">ONNX runtime session options creator</param>
        public OnnxDetectionPredictorProperties(String modelPath, OnnxInputProperties inputProperties, IDetectionPostProcessor
             postProcessor, IOrtSessionOptionsCreator ortSessionOptionsCreator)
            : base(modelPath, inputProperties, ortSessionOptionsCreator) {
            this.postProcessor = Objects.RequireNonNull(postProcessor);
        }

        /// <summary>
        /// Creates a new text detection properties object for existing pre-trained
        /// DBNet models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection properties object for existing pre-trained
        /// DBNet models, stored on disk.
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
        /// <returns>a new text detection properties object for a DBNet model</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties DbNet(String modelPath) {
            return DbNet(modelPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new text detection properties object for existing pre-trained
        /// DBNet models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection properties object for existing pre-trained
        /// DBNet models, stored on disk.
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
        /// <returns>a new text detection properties object for a DBNet model</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties DbNet(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , DB_NET_POST_PROCESSOR, ortSessionOptionsCreator);
        }

        /// <summary>
        /// Creates a new text detection properties object for existing pre-trained
        /// FAST models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection properties object for existing pre-trained
        /// FAST models, stored on disk. This is the default text detection model in
        /// OnnxTR.
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
        /// <returns>a new text detection properties object for a FAST model</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties Fast(String modelPath) {
            return Fast(modelPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new text detection properties object for existing pre-trained
        /// FAST models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection properties object for existing pre-trained
        /// FAST models, stored on disk. This is the default text detection model in
        /// OnnxTR.
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
        /// <returns>a new text detection properties object for a FAST model</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties Fast(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , DEFAULT_POST_PROCESSOR, ortSessionOptionsCreator);
        }

        /// <summary>Creates a new text detection properties object for existing pre-trained LinkNet models, stored on disk.
        ///     </summary>
        /// <remarks>
        /// Creates a new text detection properties object for existing pre-trained LinkNet models, stored on disk.
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
        /// <returns>a new text detection properties object for a LinkNet model</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties LinkNet(String modelPath) {
            return LinkNet(modelPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>Creates a new text detection properties object for existing pre-trained LinkNet models, stored on disk.
        ///     </summary>
        /// <remarks>
        /// Creates a new text detection properties object for existing pre-trained LinkNet models, stored on disk.
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
        /// <returns>a new text detection properties object for a LinkNet model</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties LinkNet(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , DEFAULT_POST_PROCESSOR, ortSessionOptionsCreator);
        }

        /// <summary>
        /// Creates a new text detection properties object for existing pre-trained
        /// PaddleOCR models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection properties object for existing pre-trained
        /// PaddleOCR models, stored on disk.
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
        /// <returns>a new text detection properties object for a PaddleOCR model</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties PaddleOcr(String modelDirPath) {
            return PaddleOcr(modelDirPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new text detection properties object for existing pre-trained
        /// PaddleOCR models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection properties object for existing pre-trained
        /// PaddleOCR models, stored on disk.
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
        /// <returns>a new text detection properties object for a PaddleOCR model</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties PaddleOcr(String modelDirPath, 
            IOrtSessionOptionsCreator ortSessionOptionsCreator) {
            return PaddleOcr(modelDirPath + "/inference.onnx", modelDirPath + "/inference.yml", ortSessionOptionsCreator
                );
        }

        /// <summary>
        /// Creates a new text detection properties object for existing pre-trained
        /// PaddleOCR models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection properties object for existing pre-trained
        /// PaddleOCR models, stored on disk.
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
        /// <returns>a new text detection properties object for a PaddleOCR model</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties PaddleOcr(String modelPath, String
             configPath) {
            return PaddleOcr(modelPath, configPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new text detection properties object for existing pre-trained
        /// PaddleOCR models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection properties object for existing pre-trained
        /// PaddleOCR models, stored on disk.
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
        /// <returns>a new text detection properties object for a PaddleOCR model</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties PaddleOcr(String modelPath, String
             configPath, IOrtSessionOptionsCreator ortSessionOptionsCreator) {
            InferenceConfig config;
            using (Stream @is = iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine(configPath))
                ) {
                config = InferenceConfigParser.Parse(@is);
            }
            OnnxInputProperties inputProperties = CreatePaddleInputProperties(config);
            PaddleOcrDetectionPostProcessor postProcessor = CreatePaddlePostProcessor(config);
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties(modelPath, inputProperties, postProcessor
                , ortSessionOptionsCreator);
        }

        /// <summary>
        /// Creates a new text detection properties object for an existing
        /// pre-trained EasyOCR CRAFT model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection properties object for an existing
        /// pre-trained EasyOCR CRAFT model, stored on disk.
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
        /// <returns>a new text detection properties object for an EasyOCR CRAFT model</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties EasyOcr(String modelPath) {
            return EasyOcr(modelPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new text detection properties object for an existing
        /// pre-trained EasyOCR CRAFT model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text detection properties object for an existing
        /// pre-trained EasyOCR CRAFT model, stored on disk.
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
        /// <returns>a new text detection properties object for an EasyOCR CRAFT model</returns>
        public static iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties EasyOcr(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties(modelPath, EASY_OCR_INPUT_PROPERTIES
                , EASY_OCR_POST_PROCESSOR, ortSessionOptionsCreator);
        }

        /// <summary>Returns the ONNX model output post-processor.</summary>
        /// <returns>the ONNX model output post-processor</returns>
        public virtual IDetectionPostProcessor GetPostProcessor() {
            return postProcessor;
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || this.GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties that = (iText.Pdfocr.Onnx.Detection.OnnxDetectionPredictorProperties
                )o;
            return Object.Equals(modelPath, that.modelPath) && Object.Equals(inputProperties, that.inputProperties) &&
                 Object.Equals(postProcessor, that.postProcessor) && Object.Equals(ortSessionOptionsCreator, that.ortSessionOptionsCreator
                );
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)modelPath, inputProperties, postProcessor, ortSessionOptionsCreator
                );
        }

        /// <summary><inheritDoc/></summary>
        public override String ToString() {
            return "OnnxDetectionPredictorProperties{" + "modelPath='" + modelPath + '\'' + ", inputProperties=" + inputProperties
                 + ", postProcessor=" + postProcessor + '}';
        }

        private static OnnxInputProperties CreatePaddleInputProperties(InferenceConfig config) {
            TransformOp[] ops = config.GetPreProcess().GetTransformOps();
            DecodeImage decode = GetPaddleOp<DecodeImage>(ops, DecodeImage.WRAPPING_KEY);
            if (decode.GetChannelFirst()) {
                throw PaddleOcrInitException.ChannelFirstIsNotSupported();
            }
            ImageChannelConfiguration channelConfig = MapImgMode(decode.GetImgMode());
            NormalizeImage normalize = GetPaddleOp<NormalizeImage>(ops, NormalizeImage.WRAPPING_KEY);
            float[] mean = normalize.GetMean();
            if (mean.Length != channelConfig.GetChannelCount()) {
                throw PaddleOcrInitException.UnexpectedMeanChannelCount(mean.Length);
            }
            float[] std = normalize.GetStd();
            if (std.Length != channelConfig.GetChannelCount()) {
                throw PaddleOcrInitException.UnexpectedStdChannelCount(std.Length);
            }
            DetResizeForTest resize = GetPaddleOp<DetResizeForTest>(ops, DetResizeForTest.WRAPPING_KEY);
            if (resize.GetImageShape() != null) {
                throw PaddleOcrInitException.ImageShapeIsNotSupported();
            }
            /*
            * From looking at the logic within PaddleOCR, it seems like there are
            * very few ways for the configuration file to, actually, affect the
            * resizing operation. The majority of the parameters come from a
            * global OCR config file, which is static. So the only things you are
            * getting from the model config file here is the channel
            * configuration. It can also be affected, if an `image_shape` key is
            * present, but we didn't add support for that anyway.
            */
            ImageResizeOptions resizeOpts = new ImageResizeOptions(channelConfig, PADDLE_LIMIT_SIDE_LEN, PADDLE_LIMIT_SIDE_LEN
                , PADDLE_MAX_SIDE_LIMIT, PADDLE_MAX_SIDE_LIMIT, PADDLE_SIDE_MULTIPLE, PADDLE_SIDE_MULTIPLE, PaddingStrategy
                .BOTTOM_RIGHT_BLACK);
            return new OnnxInputProperties(resizeOpts, mean, std, PADDLE_BATCH_SIZE);
        }

        private static T GetPaddleOp<T>(TransformOp[] ops, String name) {
            System.Type cls = typeof(T);
            for (int i = 0; i < ops.Length; ++i) {
                TransformOp op = ops[i];
                if (cls.IsInstanceOfType(op)) {
                    return (T)op;
                }
            }
            throw PaddleOcrInitException.PreProcessorOperationMissing(name);
        }

        private static ImageChannelConfiguration MapImgMode(ImgMode im) {
            switch (im) {
                case ImgMode.GRAY: {
                    return ImageChannelConfiguration.GRAYSCALE;
                }

                case ImgMode.RGB: {
                    return ImageChannelConfiguration.RGB;
                }

                case ImgMode.BGR: {
                    return ImageChannelConfiguration.BGR;
                }
            }
            // Should not get here
            throw new InvalidOperationException(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_CHANNEL_CONFIGURATION);
        }

        private static PaddleOcrDetectionPostProcessor CreatePaddlePostProcessor(InferenceConfig config) {
            PostProcess postProcess = config.GetPostProcess();
            if (!(postProcess is DbPostProcess)) {
                throw PaddleOcrInitException.UnexpectedPostProcessorType(postProcess.GetName());
            }
            DbPostProcess db = (DbPostProcess)postProcess;
            if (db.GetUseDilation()) {
                throw PaddleOcrInitException.UseDilationIsNotSupported();
            }
            if (db.GetScoreMode() != ScoreMode.FAST) {
                throw PaddleOcrInitException.ScoreModeIsNotSupported();
            }
            if (db.GetBoxType() != BoxType.QUAD) {
                throw PaddleOcrInitException.BoxTypeIsNotSupported();
            }
            return new PaddleOcrDetectionPostProcessor(db.GetThresh(), db.GetBoxThresh(), db.GetUnclipRatio(), db.GetMaxCandidates
                ());
        }
    }
}
