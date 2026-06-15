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

namespace iText.Pdfocr.Onnx.Recognition {
    /// <summary>Properties for configuring text recognition ONNX models.</summary>
    /// <remarks>
    /// Properties for configuring text recognition ONNX models.
    /// <para />
    /// It contains a path to the model, model input properties and a model output post-processor.
    /// </remarks>
    public class OnnxRecognitionPredictorProperties : AbstractOnnxPredictorProperties {
        private static readonly OnnxInputProperties DEFAULT_INPUT_PROPERTIES = new OnnxInputProperties(new ImageResizeOptions
            (ImageChannelConfiguration.RGB, 128, 32, PaddingStrategy.BOTTOM_RIGHT_BLACK), new float[] { 0.694F, 0.695F
            , 0.693F }, new float[] { 0.299F, 0.296F, 0.301F }, 64);

        private const int PADDLE_MAX_WIDTH = 3200;

        private static readonly float[] PADDLE_MEAN = new float[] { 0.5F, 0.5F, 0.5F };

        private static readonly float[] PADDLE_STD = new float[] { 0.5F, 0.5F, 0.5F };

        private const int PADDLE_BATCH_SIZE = 6;

        private static readonly OnnxInputProperties EASY_OCR_INPUT_PROPERTIES = new OnnxInputProperties(new ImageResizeOptions
            (ImageChannelConfiguration.GRAYSCALE, 1, 64, 
                // There is, actually, no width limit here for EasyOCR, so just setting
                
                // something big, but reasonable here...
                4096, 64, PaddingStrategy.BOTTOM_RIGHT_EDGE), new float[] { 0.5F }, new float[] { 0.5F }, 
                // In the CPU case just having 1 should be faster
                1);

        /// <summary>Post-processor of the outputs of the ONNX model.</summary>
        /// <remarks>
        /// Post-processor of the outputs of the ONNX model. Converts the  output of
        /// the model to a text string.
        /// </remarks>
        private readonly IRecognitionPostProcessor postProcessor;

        /// <summary>
        /// Defines, whether input images to the recognition model should be split
        /// into smaller ones with better aspect ratios.
        /// </summary>
        /// <remarks>
        /// Defines, whether input images to the recognition model should be split
        /// into smaller ones with better aspect ratios. Usually should be false
        /// for models, which operates on lines, as merging of the text back could
        /// cause errors.
        /// </remarks>
        private readonly bool splitImages;

        /// <summary>Creates new text recognition predictor properties.</summary>
        /// <param name="modelPath">path to the ONNX model to load</param>
        /// <param name="inputProperties">ONNX model input properties</param>
        /// <param name="postProcessor">ONNX model output post-processor</param>
        /// <param name="splitImages">
        /// whether input images to the ML model should be split
        /// into smaller ones with better aspect ratios
        /// </param>
        public OnnxRecognitionPredictorProperties(String modelPath, OnnxInputProperties inputProperties, IRecognitionPostProcessor
             postProcessor, bool splitImages)
            : this(modelPath, inputProperties, postProcessor, splitImages, DEFAULT_ORT_SESSION_CREATOR) {
        }

        /// <summary>Creates new text recognition predictor properties.</summary>
        /// <param name="modelPath">path to the ONNX model to load</param>
        /// <param name="inputProperties">ONNX model input properties</param>
        /// <param name="postProcessor">ONNX model output post-processor</param>
        /// <param name="splitImages">
        /// whether input images to the ML model should be split
        /// into smaller ones with better aspect ratios
        /// </param>
        /// <param name="ortSessionOptionsCreator">ONNX runtime session options creator</param>
        public OnnxRecognitionPredictorProperties(String modelPath, OnnxInputProperties inputProperties, IRecognitionPostProcessor
             postProcessor, bool splitImages, IOrtSessionOptionsCreator ortSessionOptionsCreator)
            : base(modelPath, inputProperties, ortSessionOptionsCreator) {
            this.postProcessor = Objects.RequireNonNull(postProcessor);
            this.splitImages = splitImages;
        }

        /// <summary>Creates new text recognition predictor properties.</summary>
        /// <remarks>
        /// Creates new text recognition predictor properties.
        /// <para />
        /// Images will be split before passing them to the ML model.
        /// </remarks>
        /// <param name="modelPath">path to the ONNX model to load</param>
        /// <param name="inputProperties">ONNX model input properties</param>
        /// <param name="postProcessor">ONNX model output post-processor</param>
        public OnnxRecognitionPredictorProperties(String modelPath, OnnxInputProperties inputProperties, IRecognitionPostProcessor
             postProcessor)
            : this(modelPath, inputProperties, postProcessor, true) {
        }

        /// <summary>Creates new text recognition predictor properties.</summary>
        /// <param name="modelPath">path to the ONNX model to load</param>
        /// <param name="inputProperties">ONNX model input properties</param>
        /// <param name="postProcessor">ONNX model output post-processor</param>
        /// <param name="ortSessionOptionsCreator">ONNX runtime session options creator</param>
        public OnnxRecognitionPredictorProperties(String modelPath, OnnxInputProperties inputProperties, IRecognitionPostProcessor
             postProcessor, IOrtSessionOptionsCreator ortSessionOptionsCreator)
            : this(modelPath, inputProperties, postProcessor, true, ortSessionOptionsCreator) {
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing pre-trained
        /// CRNN models with a VGG-16 backbone, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing pre-trained
        /// CRNN models with a VGG-16 backbone, stored on disk. This is the default
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
        /// <returns>a new text recognition properties object for a CRNN model with a VGG-16 backbone</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties CrnnVgg16(String modelPath) {
            return CrnnVgg16(modelPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing pre-trained
        /// CRNN models with a VGG-16 backbone, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing pre-trained
        /// CRNN models with a VGG-16 backbone, stored on disk. This is the default
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
        /// <returns>a new text recognition properties object for a CRNN model with a VGG-16 backbone</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties CrnnVgg16(String modelPath, 
            IOrtSessionOptionsCreator ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , new CrnnPostProcessor(Vocabulary.LEGACY_FRENCH), ortSessionOptionsCreator);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing pre-trained
        /// CRNN models with a MobileNet V3 backbone, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing pre-trained
        /// CRNN models with a MobileNet V3 backbone, stored on disk.
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
        /// <returns>a new text recognition properties object for a CRNN model with a MobileNet V3 backbone</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties CrnnMobileNetV3(String modelPath
            ) {
            return CrnnMobileNetV3(modelPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing pre-trained
        /// CRNN models with a MobileNet V3 backbone, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing pre-trained
        /// CRNN models with a MobileNet V3 backbone, stored on disk.
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
        /// <returns>a new text recognition properties object for a CRNN model with a MobileNet V3 backbone</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties CrnnMobileNetV3(String modelPath
            , IOrtSessionOptionsCreator ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , new CrnnPostProcessor(Vocabulary.FRENCH), ortSessionOptionsCreator);
        }

        /// <summary>Creates a new text recognition properties object for existing pre-trained MASTER models, stored on disk.
        ///     </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing pre-trained MASTER models, stored on disk.
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
        /// <returns>a new text recognition properties object for a MASTER model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties Master(String modelPath) {
            return Master(modelPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>Creates a new text recognition properties object for existing pre-trained MASTER models, stored on disk.
        ///     </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing pre-trained MASTER models, stored on disk.
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
        /// <returns>a new text recognition properties object for a MASTER model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties Master(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , 
                        // Additional "<sos>" and "<pad>" tokens
                        new EndOfStringPostProcessor(Vocabulary.FRENCH, 2), ortSessionOptionsCreator);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing pre-trained
        /// PARSeq models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing pre-trained
        /// PARSeq models, stored on disk.
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
        /// <returns>a new text recognition properties object for a PARSeq model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties ParSeq(String modelPath) {
            return ParSeq(modelPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing pre-trained
        /// PARSeq models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing pre-trained
        /// PARSeq models, stored on disk.
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
        /// <returns>a new text recognition properties object for a PARSeq model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties ParSeq(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties.ParSeq(modelPath, Vocabulary.FRENCH
                , 0, ortSessionOptionsCreator);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing pre-trained
        /// PARSeq models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing pre-trained
        /// PARSeq models, stored on disk.
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
        /// <returns>a new text recognition properties object for a PARSeq model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties ParSeq(String modelPath, Vocabulary
             vocabulary, int additionalTokens) {
            return ParSeq(modelPath, vocabulary, additionalTokens, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing pre-trained
        /// PARSeq models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing pre-trained
        /// PARSeq models, stored on disk.
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
        /// <returns>a new text recognition properties object for a PARSeq model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties ParSeq(String modelPath, Vocabulary
             vocabulary, int additionalTokens, IOrtSessionOptionsCreator ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , new EndOfStringPostProcessor(vocabulary, additionalTokens), ortSessionOptionsCreator);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing pre-trained
        /// SAR models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing pre-trained
        /// SAR models, stored on disk.
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
        /// <returns>a new text recognition properties object for a SAR model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties Sar(String modelPath) {
            return Sar(modelPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing pre-trained
        /// SAR models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing pre-trained
        /// SAR models, stored on disk.
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
        /// <returns>a new text recognition properties object for a SAR model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties Sar(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , new EndOfStringPostProcessor(Vocabulary.FRENCH, 0), ortSessionOptionsCreator);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing pre-trained
        /// ViTSTR models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing pre-trained
        /// ViTSTR models, stored on disk.
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
        /// <returns>a new text recognition properties object for a ViTSTR model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties ViTstr(String modelPath) {
            return ViTstr(modelPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing pre-trained
        /// ViTSTR models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing pre-trained
        /// ViTSTR models, stored on disk.
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
        /// <returns>a new text recognition properties object for a ViTSTR model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties ViTstr(String modelPath, IOrtSessionOptionsCreator
             ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , new EndOfStringPostProcessor(Vocabulary.FRENCH, 0), ortSessionOptionsCreator);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing
        /// pre-trained PaddleOCR models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing
        /// pre-trained PaddleOCR models, stored on disk.
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
        /// <returns>a new text recognition properties object for a PaddleOCR model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties PaddleOcr(String modelDirPath
            ) {
            return PaddleOcr(modelDirPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing
        /// pre-trained PaddleOCR models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing
        /// pre-trained PaddleOCR models, stored on disk.
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
        /// <param name="ortSessionOptionsCreator">ONNX runtime session options creator</param>
        /// <returns>a new text recognition properties object for a PaddleOCR model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties PaddleOcr(String modelDirPath
            , IOrtSessionOptionsCreator ortSessionOptionsCreator) {
            return PaddleOcr(modelDirPath + "/inference.onnx", modelDirPath + "/inference.yml", ortSessionOptionsCreator
                );
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing
        /// pre-trained PaddleOCR models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing
        /// pre-trained PaddleOCR models, stored on disk.
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
        /// <param name="modelPath">path to the pre-trained model in the ONNX format</param>
        /// <param name="configPath">path to the configuration file for the model</param>
        /// <returns>a new text recognition properties object for a PaddleOCR model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties PaddleOcr(String modelPath, 
            String configPath) {
            return PaddleOcr(modelPath, configPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing
        /// pre-trained PaddleOCR models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing
        /// pre-trained PaddleOCR models, stored on disk.
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
        /// <param name="modelPath">path to the pre-trained model in the ONNX format</param>
        /// <param name="configPath">path to the configuration file for the model</param>
        /// <param name="ortSessionOptionsCreator">ONNX runtime session options creator</param>
        /// <returns>a new text recognition properties object for a PaddleOCR model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties PaddleOcr(String modelPath, 
            String configPath, IOrtSessionOptionsCreator ortSessionOptionsCreator) {
            InferenceConfig config;
            using (Stream @is = iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine(configPath))
                ) {
                config = InferenceConfigParser.Parse(@is);
            }
            OnnxInputProperties inputProperties = CreatePaddleInputProperties(config);
            CtcLabelPostProcessor postProcessor = CreatePaddlePostProcessor(config);
            // Splitting the images makes the results worse, as the model is
            // designed to handle long line, also it seems like the split/merge
            // algorithm is not handling whitespaces properly
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties(modelPath, inputProperties, postProcessor
                , false, ortSessionOptionsCreator);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing
        /// pre-trained EasyOCR models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing
        /// pre-trained EasyOCR models, stored on disk.
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
        /// <returns>a new text recognition properties object for a EasyOCR model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties EasyOcr(String modelPath, EasyOcrMapper
             labelMapper) {
            return EasyOcr(modelPath, labelMapper, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new text recognition properties object for existing
        /// pre-trained EasyOCR models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new text recognition properties object for existing
        /// pre-trained EasyOCR models, stored on disk.
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
        /// <param name="ortSessionOptionsCreator">ONNX runtime session options creator</param>
        /// <returns>a new text recognition properties object for a EasyOCR model</returns>
        public static iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties EasyOcr(String modelPath, EasyOcrMapper
             labelMapper, IOrtSessionOptionsCreator ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties(modelPath, EASY_OCR_INPUT_PROPERTIES
                , new CtcLabelPostProcessor(labelMapper), false, ortSessionOptionsCreator);
        }

        /// <summary>Returns the ONNX model output post-processor.</summary>
        /// <returns>the ONNX model output post-processor</returns>
        public virtual IRecognitionPostProcessor GetPostProcessor() {
            return postProcessor;
        }

        /// <summary>Returns whether input images should be split.</summary>
        /// <returns>whether input images should be split</returns>
        public virtual bool ShouldSplitImages() {
            return splitImages;
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || this.GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties that = (iText.Pdfocr.Onnx.Recognition.OnnxRecognitionPredictorProperties
                )o;
            return splitImages == that.splitImages && Object.Equals(modelPath, that.modelPath) && Object.Equals(inputProperties
                , that.inputProperties) && Object.Equals(postProcessor, that.postProcessor) && Object.Equals(ortSessionOptionsCreator
                , that.ortSessionOptionsCreator);
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)modelPath, inputProperties, postProcessor, splitImages, ortSessionOptionsCreator
                );
        }

        /// <summary><inheritDoc/></summary>
        public override String ToString() {
            return "OnnxRecognitionPredictorProperties{" + "modelPath='" + modelPath + '\'' + ", inputProperties=" + inputProperties
                 + ", postProcessor=" + postProcessor + ", splitImages=" + splitImages + '}';
        }

        private static OnnxInputProperties CreatePaddleInputProperties(InferenceConfig config) {
            TransformOp[] ops = config.GetPreProcess().GetTransformOps();
            DecodeImage decode = GetPaddleOp<DecodeImage>(ops, DecodeImage.WRAPPING_KEY);
            if (decode.GetChannelFirst()) {
                throw PaddleOcrInitException.ChannelFirstIsNotSupported();
            }
            ImageChannelConfiguration channelConfig = MapImgMode(decode.GetImgMode());
            RecResizeImg resize = GetPaddleOp<RecResizeImg>(ops, RecResizeImg.WRAPPING_KEY);
            int[] inputShape = resize.GetImageShape();
            int height = inputShape[1];
            int minWidth = inputShape[2];
            ImageResizeOptions resizeOpts = new ImageResizeOptions(channelConfig, minWidth, height, PADDLE_MAX_WIDTH, 
                height, PaddingStrategy.BOTTOM_RIGHT_GRAY);
            return new OnnxInputProperties(resizeOpts, PADDLE_MEAN, PADDLE_STD, PADDLE_BATCH_SIZE);
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
            throw new InvalidOperationException(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_CHANNEL_CONFIGURATION);
        }

        private static CtcLabelPostProcessor CreatePaddlePostProcessor(InferenceConfig config) {
            PostProcess postProcess = config.GetPostProcess();
            if (!(postProcess is CtcLabelDecode)) {
                throw PaddleOcrInitException.UnexpectedPostProcessorType(postProcess.GetName());
            }
            CtcLabelDecode ctc = (CtcLabelDecode)postProcess;
            /*
            * In PaddleOCR there is a space character mapping, but it is not
            * included in the config file. It is a parameter in the post
            * processor, which is always true. For simplicity, we will just
            * modify the vocab here.
            */
            String[] lookUpTable = Add(ctc.GetCharacterDict(), " ");
            return new CtcLabelPostProcessor(new StringMapper(lookUpTable));
        }

        private static String[] Add(String[] arr, String elem) {
            String[] newArr = JavaUtil.ArraysCopyOf(arr, arr.Length + 1);
            newArr[arr.Length] = elem;
            return newArr;
        }
    }
}
