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
using iText.Pdfocr.Onnx;
using iText.Pdfocr.Onnx.Util;

namespace iText.Pdfocr.Onnx.Recognition {
    /// <summary>
    /// A text recognition predictor implementation, which is using ONNX Runtime and
    /// its ML models to recognize text characters on an image.
    /// </summary>
    public class OnnxRecognitionPredictor : AbstractOnnxPredictor<IronSoftware.Drawing.AnyBitmap, String>, IRecognitionPredictor {
        /// <summary>Configuration properties of the predictor.</summary>
        private readonly OnnxRecognitionPredictorProperties properties;

        /// <summary>Creates a text recognition predictor with the specified properties.</summary>
        /// <param name="properties">properties of the predictor</param>
        public OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties properties)
            : base(properties, GetExpectedOutputShape(properties)) {
            this.properties = properties;
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// CRNN model with a VGG-16 backbone, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// CRNN model with a VGG-16 backbone, stored on disk. This is the default
        /// text recognition model in OnnxTR.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/crnn_vgg16_bn-662979cc.onnx">
        /// crnn_vgg16_bn
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/crnn_vgg16_bn_static_8_bit-bce050c7.onnx">
        /// crnn_vgg16_bn (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models cannot handle spaces. Make sure you choose a detection
        /// model that outputs words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new predictor object with the CRNN model loaded with a VGG-16 backbone</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor CrnnVgg16(String modelPath) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.CrnnVgg16
                (modelPath));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// CRNN model with a VGG-16 backbone, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// CRNN model with a VGG-16 backbone, stored on disk. This is the default
        /// text recognition model in OnnxTR.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/crnn_vgg16_bn-662979cc.onnx">
        /// crnn_vgg16_bn
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/crnn_vgg16_bn_static_8_bit-bce050c7.onnx">
        /// crnn_vgg16_bn (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models cannot handle spaces. Make sure you choose a detection
        /// model that outputs words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor object with the CRNN model loaded with a VGG-16 backbone</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor CrnnVgg16(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.CrnnVgg16
                (modelPath, ortSessionOptionsCreator));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// CRNN model with a MobileNet V3 backbone, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// CRNN model with a MobileNet V3 backbone, stored on disk.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/crnn_mobilenet_v3_large-d42e8185.onnx">
        /// crnn_mobilenet_v3_large
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/crnn_mobilenet_v3_large_static_8_bit-459e856d.onnx">
        /// crnn_mobilenet_v3_large (8-bit quantized)
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/crnn_mobilenet_v3_small-bded4d49.onnx">
        /// crnn_mobilenet_v3_small
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/crnn_mobilenet_v3_small_static_8_bit-4949006f.onnx">
        /// crnn_mobilenet_v3_small (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models cannot handle spaces. Make sure you choose a detection
        /// model that outputs words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new predictor object with the CRNN model loaded with a MobileNet V3 backbone</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor CrnnMobileNetV3(String modelPath) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.CrnnMobileNetV3
                (modelPath));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// CRNN model with a MobileNet V3 backbone, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// CRNN model with a MobileNet V3 backbone, stored on disk.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/crnn_mobilenet_v3_large-d42e8185.onnx">
        /// crnn_mobilenet_v3_large
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/crnn_mobilenet_v3_large_static_8_bit-459e856d.onnx">
        /// crnn_mobilenet_v3_large (8-bit quantized)
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/crnn_mobilenet_v3_small-bded4d49.onnx">
        /// crnn_mobilenet_v3_small
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/crnn_mobilenet_v3_small_static_8_bit-4949006f.onnx">
        /// crnn_mobilenet_v3_small (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models cannot handle spaces. Make sure you choose a detection
        /// model that outputs words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor object with the CRNN model loaded with a MobileNet V3 backbone</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor CrnnMobileNetV3(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.CrnnMobileNetV3
                (modelPath, ortSessionOptionsCreator));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// MASTER model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// MASTER model, stored on disk.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/master-b1287fcd.onnx">
        /// MASTER
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/master_dynamic_8_bit-d8bd8206.onnx">
        /// MASTER (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models cannot handle spaces. Make sure you choose a detection
        /// model that outputs words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new predictor object with the MASTER model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor Master(String modelPath) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.Master
                (modelPath));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// MASTER model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// MASTER model, stored on disk.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/master-b1287fcd.onnx">
        /// MASTER
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/master_dynamic_8_bit-d8bd8206.onnx">
        /// MASTER (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models cannot handle spaces. Make sure you choose a detection
        /// model that outputs words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor object with the MASTER model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor Master(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.Master
                (modelPath, ortSessionOptionsCreator));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// PARSeq model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// PARSeq model, stored on disk.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/parseq-00b40714.onnx">
        /// parseq
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/parseq_dynamic_8_bit-5b04d9f7.onnx">
        /// parseq (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models cannot handle spaces. Make sure you choose a detection
        /// model that outputs words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new predictor object with the PARSeq model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor ParSeq(String modelPath) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.ParSeq
                (modelPath));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// PARSeq model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// PARSeq model, stored on disk.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/parseq-00b40714.onnx">
        /// parseq
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/parseq_dynamic_8_bit-5b04d9f7.onnx">
        /// parseq (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models cannot handle spaces. Make sure you choose a detection
        /// model that outputs words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor object with the PARSeq model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor ParSeq(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.ParSeq
                (modelPath, ortSessionOptionsCreator));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// PARSeq model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// PARSeq model, stored on disk.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/parseq-00b40714.onnx">
        /// parseq
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/parseq_dynamic_8_bit-5b04d9f7.onnx">
        /// parseq (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models cannot handle spaces. Make sure you choose a detection
        /// model that outputs words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <param name="vocabulary">vocabulary used for the model output (without special tokens)</param>
        /// <param name="additionalTokens">amount of additional tokens in the total vocabulary after the end-of-string token
        ///     </param>
        /// <returns>a new predictor object with the PARSeq model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor ParSeq(String modelPath, Vocabulary vocabulary
            , int additionalTokens) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.ParSeq
                (modelPath, vocabulary, additionalTokens));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// PARSeq model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// PARSeq model, stored on disk.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/parseq-00b40714.onnx">
        /// parseq
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/parseq_dynamic_8_bit-5b04d9f7.onnx">
        /// parseq (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models cannot handle spaces. Make sure you choose a detection
        /// model that outputs words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <param name="vocabulary">vocabulary used for the model output (without special tokens)</param>
        /// <param name="additionalTokens">amount of additional tokens in the total vocabulary after the end-of-string token
        ///     </param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor object with the PARSeq model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor ParSeq(String modelPath, Vocabulary vocabulary
            , int additionalTokens, IOrtSessionOptionsCreator ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.ParSeq
                (modelPath, vocabulary, additionalTokens, ortSessionOptionsCreator));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// SAR model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// SAR model, stored on disk.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/sar_resnet31-395f8005.onnx">
        /// sar_resnet31
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/sar_resnet31_static_8_bit-c07316bc.onnx">
        /// sar_resnet31 (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models cannot handle spaces. Make sure you choose a detection
        /// model that outputs words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new predictor object with the SAR model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor Sar(String modelPath) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.Sar(modelPath
                ));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// SAR model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// SAR model, stored on disk.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/sar_resnet31-395f8005.onnx">
        /// sar_resnet31
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/sar_resnet31_static_8_bit-c07316bc.onnx">
        /// sar_resnet31 (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models cannot handle spaces. Make sure you choose a detection
        /// model that outputs words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor object with the SAR model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor Sar(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.Sar(modelPath
                , ortSessionOptionsCreator));
        }

        /// <summary>Creates a new text recognition predictor using an existing pre-trained ViTSTR model, stored on disk.
        ///     </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained ViTSTR model, stored on disk.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/vitstr_base-ff62f5be.onnx">
        /// vitstr_base
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/vitstr_base_dynamic_8_bit-976c7cd6.onnx">
        /// vitstr_base (8-bit quantized)
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/vitstr_small-3ff9c500.onnx">
        /// vitstr_small
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/vitstr_small_dynamic_8_bit-bec6c796.onnx">
        /// vitstr_small (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models cannot handle spaces. Make sure you choose a detection
        /// model that outputs words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new predictor object with the ViTSTR model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor ViTstr(String modelPath) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.ViTstr
                (modelPath));
        }

        /// <summary>Creates a new text recognition predictor using an existing pre-trained ViTSTR model, stored on disk.
        ///     </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained ViTSTR model, stored on disk.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/vitstr_base-ff62f5be.onnx">
        /// vitstr_base
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/vitstr_base_dynamic_8_bit-976c7cd6.onnx">
        /// vitstr_base (8-bit quantized)
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/vitstr_small-3ff9c500.onnx">
        /// vitstr_small
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/vitstr_small_dynamic_8_bit-bec6c796.onnx">
        /// vitstr_small (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models cannot handle spaces. Make sure you choose a detection
        /// model that outputs words.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor object with the ViTSTR model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor ViTstr(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.ViTstr
                (modelPath, ortSessionOptionsCreator));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// PaddleOCR model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
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
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv5_server_rec_infer">
        /// PP-OCRv5_server_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv5_mobile_rec_infer">
        /// PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv4_server_rec_doc_infer">
        /// PP-OCRv4_server_rec_doc
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv4_mobile_rec_infer">
        /// PP-OCRv4_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv4_server_rec_infer">
        /// PP-OCRv4_server_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv3_mobile_rec_infer">
        /// PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ch_svtrv2_rec_infer">
        /// ch_SVTRv2_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ch_repsvtr_rec_infer">
        /// ch_RepSVTR_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-en_pp-ocrv5_mobile_rec_infer">
        /// en_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-en_pp-ocrv4_mobile_rec_infer">
        /// en_PP-OCRv4_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-en_pp-ocrv3_mobile_rec_infer">
        /// en_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-korean_pp-ocrv5_mobile_rec_infer">
        /// korean_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-latin_pp-ocrv5_mobile_rec_infer">
        /// latin_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-eslav_pp-ocrv5_mobile_rec_infer">
        /// eslav_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-th_pp-ocrv5_mobile_rec_infer">
        /// th_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-el_pp-ocrv5_mobile_rec_infer">
        /// el_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-arabic_pp-ocrv5_mobile_rec_infer">
        /// arabic_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-cyrillic_pp-ocrv5_mobile_rec_infer">
        /// cyrillic_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-devanagari_pp-ocrv5_mobile_rec_infer">
        /// devanagari_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-te_pp-ocrv5_mobile_rec_infer">
        /// te_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ta_pp-ocrv5_mobile_rec_infer">
        /// ta_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-korean_pp-ocrv3_mobile_rec_infer">
        /// korean_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-japan_pp-ocrv3_mobile_rec_infer">
        /// japan_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-chinese_cht_pp-ocrv3_mobile_rec_infer">
        /// chinese_cht_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-te_pp-ocrv3_mobile_rec_infer">
        /// te_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ka_pp-ocrv3_mobile_rec_infer">
        /// ka_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ta_pp-ocrv3_mobile_rec_infer">
        /// ta_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-latin_pp-ocrv3_mobile_rec_infer">
        /// latin_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-arabic_pp-ocrv3_mobile_rec_infer">
        /// arabic_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-cyrillic_pp-ocrv3_mobile_rec_infer">
        /// cyrillic_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-devanagari_pp-ocrv3_mobile_rec_infer">
        /// devanagari_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models can handle spaces.
        /// </remarks>
        /// <param name="modelDirPath">
        /// path to the directory with the model and its
        /// configuration file
        /// </param>
        /// <returns>a new predictor object with the PaddleOCR model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor PaddleOcr(String modelDirPath) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.PaddleOcr
                (modelDirPath));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// PaddleOCR model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
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
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv5_server_rec_infer">
        /// PP-OCRv5_server_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv5_mobile_rec_infer">
        /// PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv4_server_rec_doc_infer.tar">
        /// PP-OCRv4_server_rec_doc
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv4_mobile_rec_infer">
        /// PP-OCRv4_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv4_server_rec_infer">
        /// PP-OCRv4_server_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv3_mobile_rec_infer">
        /// PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ch_svtrv2_rec_infer">
        /// ch_SVTRv2_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ch_repsvtr_rec_infer">
        /// ch_RepSVTR_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-en_pp-ocrv5_mobile_rec_infer">
        /// en_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-en_pp-ocrv4_mobile_rec_infer">
        /// en_PP-OCRv4_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-en_pp-ocrv3_mobile_rec_infer">
        /// en_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-korean_pp-ocrv5_mobile_rec_infer">
        /// korean_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-latin_pp-ocrv5_mobile_rec_infer">
        /// latin_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-eslav_pp-ocrv5_mobile_rec_infer">
        /// eslav_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-th_pp-ocrv5_mobile_rec_infer">
        /// th_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-el_pp-ocrv5_mobile_rec_infer">
        /// el_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-arabic_pp-ocrv5_mobile_rec_infer">
        /// arabic_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-cyrillic_pp-ocrv5_mobile_rec_infer">
        /// cyrillic_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-devanagari_pp-ocrv5_mobile_rec_infer">
        /// devanagari_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-te_pp-ocrv5_mobile_rec_infer">
        /// te_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ta_pp-ocrv5_mobile_rec_infer">
        /// ta_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-korean_pp-ocrv3_mobile_rec_infer">
        /// korean_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-japan_pp-ocrv3_mobile_rec_infer">
        /// japan_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-chinese_cht_pp-ocrv3_mobile_rec_infer">
        /// chinese_cht_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-te_pp-ocrv3_mobile_rec_infer">
        /// te_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ka_pp-ocrv3_mobile_rec_infer">
        /// ka_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ta_pp-ocrv3_mobile_rec_infer">
        /// ta_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-latin_pp-ocrv3_mobile_rec_infer">
        /// latin_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-arabic_pp-ocrv3_mobile_rec_infer">
        /// arabic_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-cyrillic_pp-ocrv3_mobile_rec_infer">
        /// cyrillic_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-devanagari_pp-ocrv3_mobile_rec_infer">
        /// devanagari_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models can handle spaces.
        /// </remarks>
        /// <param name="modelDirPath">
        /// path to the directory with the model and its
        /// configuration file
        /// </param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor object with the PaddleOCR model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor PaddleOcr(String modelDirPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.PaddleOcr
                (modelDirPath, ortSessionOptionsCreator));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// PaddleOCR model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
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
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv5_server_rec_infer">
        /// PP-OCRv5_server_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv5_mobile_rec_infer">
        /// PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv4_server_rec_doc_infer.tar">
        /// PP-OCRv4_server_rec_doc
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv4_mobile_rec_infer">
        /// PP-OCRv4_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv4_server_rec_infer">
        /// PP-OCRv4_server_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv3_mobile_rec_infer">
        /// PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ch_svtrv2_rec_infer">
        /// ch_SVTRv2_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ch_repsvtr_rec_infer">
        /// ch_RepSVTR_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-en_pp-ocrv5_mobile_rec_infer">
        /// en_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-en_pp-ocrv4_mobile_rec_infer">
        /// en_PP-OCRv4_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-en_pp-ocrv3_mobile_rec_infer">
        /// en_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-korean_pp-ocrv5_mobile_rec_infer">
        /// korean_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-latin_pp-ocrv5_mobile_rec_infer">
        /// latin_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-eslav_pp-ocrv5_mobile_rec_infer">
        /// eslav_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-th_pp-ocrv5_mobile_rec_infer">
        /// th_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-el_pp-ocrv5_mobile_rec_infer">
        /// el_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-arabic_pp-ocrv5_mobile_rec_infer">
        /// arabic_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-cyrillic_pp-ocrv5_mobile_rec_infer">
        /// cyrillic_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-devanagari_pp-ocrv5_mobile_rec_infer">
        /// devanagari_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-te_pp-ocrv5_mobile_rec_infer">
        /// te_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ta_pp-ocrv5_mobile_rec_infer">
        /// ta_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-korean_pp-ocrv3_mobile_rec_infer">
        /// korean_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-japan_pp-ocrv3_mobile_rec_infer">
        /// japan_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-chinese_cht_pp-ocrv3_mobile_rec_infer">
        /// chinese_cht_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-te_pp-ocrv3_mobile_rec_infer">
        /// te_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ka_pp-ocrv3_mobile_rec_infer">
        /// ka_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ta_pp-ocrv3_mobile_rec_infer">
        /// ta_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-latin_pp-ocrv3_mobile_rec_infer">
        /// latin_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-arabic_pp-ocrv3_mobile_rec_infer">
        /// arabic_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-cyrillic_pp-ocrv3_mobile_rec_infer">
        /// cyrillic_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-devanagari_pp-ocrv3_mobile_rec_infer">
        /// devanagari_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models can handle spaces.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model in the ONNX format</param>
        /// <param name="configPath">path to the configuration file for the model</param>
        /// <returns>a new predictor object with the PaddleOCR model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor PaddleOcr(String modelPath, String configPath
            ) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.PaddleOcr
                (modelPath, configPath));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// PaddleOCR model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
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
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv5_server_rec_infer">
        /// PP-OCRv5_server_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv5_mobile_rec_infer">
        /// PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv4_server_rec_doc_infer.tar">
        /// PP-OCRv4_server_rec_doc
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv4_mobile_rec_infer">
        /// PP-OCRv4_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv4_server_rec_infer">
        /// PP-OCRv4_server_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-pp-ocrv3_mobile_rec_infer">
        /// PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ch_svtrv2_rec_infer">
        /// ch_SVTRv2_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ch_repsvtr_rec_infer">
        /// ch_RepSVTR_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-en_pp-ocrv5_mobile_rec_infer">
        /// en_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-en_pp-ocrv4_mobile_rec_infer">
        /// en_PP-OCRv4_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-en_pp-ocrv3_mobile_rec_infer">
        /// en_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-korean_pp-ocrv5_mobile_rec_infer">
        /// korean_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-latin_pp-ocrv5_mobile_rec_infer">
        /// latin_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-eslav_pp-ocrv5_mobile_rec_infer">
        /// eslav_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-th_pp-ocrv5_mobile_rec_infer">
        /// th_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-el_pp-ocrv5_mobile_rec_infer">
        /// el_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-arabic_pp-ocrv5_mobile_rec_infer">
        /// arabic_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-cyrillic_pp-ocrv5_mobile_rec_infer">
        /// cyrillic_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-devanagari_pp-ocrv5_mobile_rec_infer">
        /// devanagari_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-te_pp-ocrv5_mobile_rec_infer">
        /// te_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ta_pp-ocrv5_mobile_rec_infer">
        /// ta_PP-OCRv5_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-korean_pp-ocrv3_mobile_rec_infer">
        /// korean_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-japan_pp-ocrv3_mobile_rec_infer">
        /// japan_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-chinese_cht_pp-ocrv3_mobile_rec_infer">
        /// chinese_cht_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-te_pp-ocrv3_mobile_rec_infer">
        /// te_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ka_pp-ocrv3_mobile_rec_infer">
        /// ka_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-ta_pp-ocrv3_mobile_rec_infer">
        /// ta_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-latin_pp-ocrv3_mobile_rec_infer">
        /// latin_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-arabic_pp-ocrv3_mobile_rec_infer">
        /// arabic_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-cyrillic_pp-ocrv3_mobile_rec_infer">
        /// cyrillic_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-devanagari_pp-ocrv3_mobile_rec_infer">
        /// devanagari_PP-OCRv3_mobile_rec
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models can handle spaces.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model in the ONNX format</param>
        /// <param name="configPath">path to the configuration file for the model</param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor object with the PaddleOCR model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor PaddleOcr(String modelPath, String configPath
            , IOrtSessionOptionsCreator ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.PaddleOcr
                (modelPath, configPath, ortSessionOptionsCreator));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// EasyOCR model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// EasyOCR model, stored on disk.
        /// <para />
        /// Only models in the ONNX format are supported. Since, by default,
        /// EasyOCR does not provide models in the ONNX format, you might need to
        /// do a model conversion yourself.
        /// <para />
        /// This method can be used to load the following EasyOCR models:
        /// <list type="bullet">
        /// <item><description>
        /// <a href=https://huggingface.co/itextresearch/itext-easyocr-english_g2">
        /// english_g2
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-latin_g2">
        /// latin_g2
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-zh_sim_g2">
        /// zh_sim_g2
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-japanese_g2">
        /// japanese_g2
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-korean_g2">
        /// korean_g2
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-telugu">
        /// telugu_g2
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-kannada">
        /// kannada_g2
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-latin">
        /// latin_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-chinese_sim">
        /// zh_sim_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-chinese">
        /// zh_tra_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-japanese">
        /// japanese_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-korean">
        /// korean_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-thai">
        /// thai_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-devanagari">
        /// devanagari_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-cyrillic">
        /// cyrillic_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-arabic">
        /// arabic_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-bengali">
        /// bengali_g1
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models can handle spaces.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model in the ONNX format</param>
        /// <param name="labelMapper">label mapper to use for the model</param>
        /// <returns>a new predictor object with the EasyOCR model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor EasyOcr(String modelPath, EasyOcrMapper
             labelMapper) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.EasyOcr
                (modelPath, labelMapper));
        }

        /// <summary>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// EasyOCR model, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition predictor using an existing pre-trained
        /// EasyOCR model, stored on disk.
        /// <para />
        /// Only models in the ONNX format are supported. Since, by default,
        /// EasyOCR does not provide models in the ONNX format, you might need to
        /// do a model conversion yourself.
        /// <para />
        /// This method can be used to load the following EasyOCR models:
        /// <list type="bullet">
        /// <item><description>
        /// <a href=https://huggingface.co/itextresearch/itext-easyocr-english_g2">
        /// english_g2
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-latin_g2">
        /// latin_g2
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-zh_sim_g2">
        /// zh_sim_g2
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-japanese_g2">
        /// japanese_g2
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-korean_g2">
        /// korean_g2
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-telugu">
        /// telugu_g2
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-kannada">
        /// kannada_g2
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-latin">
        /// latin_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-chinese_sim">
        /// zh_sim_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-chinese">
        /// zh_tra_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-japanese">
        /// japanese_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-korean">
        /// korean_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-thai">
        /// thai_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-devanagari">
        /// devanagari_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-cyrillic">
        /// cyrillic_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-arabic">
        /// arabic_g1
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://huggingface.co/itextresearch/itext-easyocr-bengali">
        /// bengali_g1
        /// </a>
        /// </description></item>
        /// </list>
        /// <para />
        /// These models can handle spaces.
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model in the ONNX format</param>
        /// <param name="labelMapper">label mapper to use for the model</param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new predictor object with the EasyOCR model loaded</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor EasyOcr(String modelPath, EasyOcrMapper
             labelMapper, IOrtSessionOptionsCreator ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictor(OnnxRecognitionPredictorProperties.EasyOcr
                (modelPath, labelMapper, ortSessionOptionsCreator));
        }

        /// <summary>Returns the text recognition predictor properties.</summary>
        /// <returns>the text recognition predictor properties</returns>
        public virtual OnnxRecognitionPredictorProperties GetProperties() {
            return properties;
        }

        /// <summary><inheritDoc/></summary>
        public override IEnumerator<String> Predict(IEnumerator<IronSoftware.Drawing.AnyBitmap> inputs) {
            if (!properties.ShouldSplitImages()) {
                return base.Predict(inputs);
            }
            // Additional pre- and post-processing, if we are splitting images
            TextBoxSplitter textBoxSplitter = new TextBoxSplitter();
            IEnumerator<IronSoftware.Drawing.AnyBitmap> splitInputs = textBoxSplitter.MapInputs(inputs);
            IEnumerator<String> outputs = base.Predict(splitInputs);
            return textBoxSplitter.MapOutputs(outputs);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override FloatBufferMdArray ToInputBuffer(IList<IronSoftware.Drawing.AnyBitmap> batch) {
            // Just your regular BCHW input
            return BufferedImageUtil.ToBchwInput(batch, properties.GetInputProperties());
        }

        /// <summary><inheritDoc/></summary>
        protected internal override IList<String> FromOutputBuffer(IList<IronSoftware.Drawing.AnyBitmap> inputBatch
            , FloatBufferMdArray outputBatch) {
            int batchSize = outputBatch.GetDimension(0);
            IList<String> words = new List<String>(batchSize);
            for (int i = 0; i < batchSize; ++i) {
                words.Add(properties.GetPostProcessor().Process(outputBatch.GetSubArray(i)));
            }
            return words;
        }

        private static long[] GetExpectedOutputShape(OnnxRecognitionPredictorProperties properties) {
            // Dynamic batch size
            long BATCH_SIZE = -1;
            // Token count is, usually, not dynamic in the model, but we don't
            // really care about it, as it is just a loop boundary in the algorithm
            long TOKEN_COUNT = -1;
            long classCount = properties.GetPostProcessor().LabelDimension();
            return new long[] { BATCH_SIZE, TOKEN_COUNT, classCount };
        }
    }
}
