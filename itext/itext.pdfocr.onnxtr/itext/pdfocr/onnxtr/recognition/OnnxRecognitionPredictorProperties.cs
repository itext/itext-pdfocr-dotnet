/*
Copyright (C) 2021-2024, Mindee | Felix Dittrich.

This program is licensed under the Apache License 2.0.
See <https://opensource.org/licenses/Apache-2.0> for full license details.
*/
using System;
using iText.Commons.Utils;
using iText.Pdfocr.Onnxtr;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr.Recognition {
    /// <summary>Properties for configuring text recognition ONNX models.</summary>
    /// <remarks>
    /// Properties for configuring text recognition ONNX models.
    /// <para />
    /// It contains a path to the model, model input properties and a model output post-processor.
    /// </remarks>
    public class OnnxRecognitionPredictorProperties {
        private static readonly OnnxInputProperties DEFAULT_INPUT_PROPERTIES = new OnnxInputProperties(new float[]
             { 0.694F, 0.695F, 0.693F }, new float[] { 0.299F, 0.296F, 0.301F }, new long[] { 512, 3, 32, 128 }, false
            );

        /// <summary>Path to the ONNX model to load.</summary>
        private readonly String modelPath;

        /// <summary>Properties of the inputs of the ONNX model.</summary>
        /// <remarks>
        /// Properties of the inputs of the ONNX model. Used for validation (both
        /// input and output, since output mask size is the same) and pre-processing.
        /// </remarks>
        private readonly OnnxInputProperties inputProperties;

        /// <summary>Post-processor of the outputs of the ONNX model.</summary>
        /// <remarks>
        /// Post-processor of the outputs of the ONNX model. Converts the  output of
        /// the model to a text string.
        /// </remarks>
        private readonly IRecognitionPostProcessor postProcessor;

        /// <summary>Creates new text recognition predictor properties.</summary>
        /// <param name="modelPath">path to the ONNX model to load</param>
        /// <param name="inputProperties">ONNX model input properties</param>
        /// <param name="postProcessor">ONNX model output post-processor</param>
        public OnnxRecognitionPredictorProperties(String modelPath, OnnxInputProperties inputProperties, IRecognitionPostProcessor
             postProcessor) {
            this.modelPath = Objects.RequireNonNull(modelPath);
            this.inputProperties = Objects.RequireNonNull(inputProperties);
            this.postProcessor = Objects.RequireNonNull(postProcessor);
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
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new text recognition properties object for a CRNN model with a VGG-16 backbone</returns>
        public static iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties CrnnVgg16(String modelPath
            ) {
            return new iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , new CrnnPostProcessor(Vocabulary.LEGACY_FRENCH));
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
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new text recognition properties object for a CRNN model with a MobileNet V3 backbone</returns>
        public static iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties CrnnMobileNetV3(String modelPath
            ) {
            return new iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , new CrnnPostProcessor(Vocabulary.FRENCH));
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
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new text recognition properties object for a MASTER model</returns>
        public static iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties Master(String modelPath) {
            return new iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , 
                        // Additional "<sos>" and "<pad>" tokens
                        new EndOfStringPostProcessor(Vocabulary.FRENCH, 2));
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
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new text recognition properties object for a PARSeq model</returns>
        public static iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties ParSeq(String modelPath) {
            return iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties.ParSeq(modelPath, Vocabulary.LATIN_EXTENDED
                , 0);
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
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <param name="vocabulary">vocabulary used for the model output (without special tokens)</param>
        /// <param name="additionalTokens">amount of additional tokens in the total vocabulary after the end-of-string token
        ///     </param>
        /// <returns>a new text recognition properties object for a PARSeq model</returns>
        public static iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties ParSeq(String modelPath, 
            Vocabulary vocabulary, int additionalTokens) {
            return new iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , new EndOfStringPostProcessor(vocabulary, additionalTokens));
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
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new text recognition properties object for a SAR model</returns>
        public static iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties Sar(String modelPath) {
            return new iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , new EndOfStringPostProcessor(Vocabulary.FRENCH, 0));
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
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new text recognition properties object for a ViTSTR model</returns>
        public static iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties ViTstr(String modelPath) {
            return new iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , new EndOfStringPostProcessor(Vocabulary.FRENCH, 0));
        }

        /// <summary>Returns the path to the ONNX model.</summary>
        /// <returns>the path to the ONNX model</returns>
        public virtual String GetModelPath() {
            return modelPath;
        }

        /// <summary>Returns the ONNX model input properties.</summary>
        /// <returns>the ONNX model input properties</returns>
        public virtual OnnxInputProperties GetInputProperties() {
            return inputProperties;
        }

        /// <summary>Returns the ONNX model output post-processor.</summary>
        /// <returns>the ONNX model output post-processor</returns>
        public virtual IRecognitionPostProcessor GetPostProcessor() {
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
            iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties that = (iText.Pdfocr.Onnxtr.Recognition.OnnxRecognitionPredictorProperties
                )o;
            return Object.Equals(modelPath, that.modelPath) && Object.Equals(inputProperties, that.inputProperties) &&
                 Object.Equals(postProcessor, that.postProcessor);
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)modelPath, inputProperties, postProcessor);
        }

        /// <summary><inheritDoc/></summary>
        public override String ToString() {
            return "OnnxRecognitionPredictorProperties{" + "modelPath='" + modelPath + '\'' + ", inputProperties=" + inputProperties
                 + ", postProcessor=" + postProcessor + '}';
        }
    }
}
