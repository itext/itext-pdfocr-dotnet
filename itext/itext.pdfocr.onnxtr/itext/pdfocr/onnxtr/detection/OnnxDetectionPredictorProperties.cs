/*
Copyright (C) 2021-2024, Mindee | Felix Dittrich.

This program is licensed under the Apache License 2.0.
See <https://opensource.org/licenses/Apache-2.0> for full license details.
*/
using System;
using iText.Commons.Utils;
using iText.Pdfocr.Onnxtr;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr.Detection {
    /// <summary>Properties for configuring text detection ONNX models.</summary>
    /// <remarks>
    /// Properties for configuring text detection ONNX models.
    /// <para />
    /// It contains a path to the model, model input properties and a model
    /// output post-processor.
    /// </remarks>
    public class OnnxDetectionPredictorProperties {
        private static readonly OnnxInputProperties DEFAULT_INPUT_PROPERTIES = new OnnxInputProperties(new float[]
             { 0.798F, 0.785F, 0.772F }, new float[] { 0.264F, 0.2749F, 0.287F }, new long[] { 2, 3, 1024, 1024 }, 
            true);

        private static readonly IDetectionPostProcessor DEFAULT_POST_PROCESSOR = new OnnxDetectionPostProcessor();

        /*
        * By default, DBNet has different thresholds for binarization and for
        * discarding results.
        */
        private static readonly IDetectionPostProcessor DB_NET_POST_PROCESSOR = new OnnxDetectionPostProcessor(0.3F
            , 0.1F);

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
        /// Post-processor of the outputs of the ONNX model. Converts the mask-like
        /// output of the model to rotated text rectangles.
        /// </remarks>
        private readonly IDetectionPostProcessor postProcessor;

        /// <summary>Creates new text detection predictor properties.</summary>
        /// <param name="modelPath">path to the ONNX model to load</param>
        /// <param name="inputProperties">ONNX model input properties</param>
        /// <param name="postProcessor">ONNX model output post-processor</param>
        public OnnxDetectionPredictorProperties(String modelPath, OnnxInputProperties inputProperties, IDetectionPostProcessor
             postProcessor) {
            this.modelPath = Objects.RequireNonNull(modelPath);
            this.inputProperties = Objects.RequireNonNull(inputProperties);
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
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new text detection properties object for a DBNet model</returns>
        public static iText.Pdfocr.Onnxtr.Detection.OnnxDetectionPredictorProperties DbNet(String modelPath) {
            return new iText.Pdfocr.Onnxtr.Detection.OnnxDetectionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , DB_NET_POST_PROCESSOR);
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
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new text detection properties object for a FAST model</returns>
        public static iText.Pdfocr.Onnxtr.Detection.OnnxDetectionPredictorProperties Fast(String modelPath) {
            return new iText.Pdfocr.Onnxtr.Detection.OnnxDetectionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , DEFAULT_POST_PROCESSOR);
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
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new text detection properties object for a LinkNet model</returns>
        public static iText.Pdfocr.Onnxtr.Detection.OnnxDetectionPredictorProperties LinkNet(String modelPath) {
            return new iText.Pdfocr.Onnxtr.Detection.OnnxDetectionPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , DEFAULT_POST_PROCESSOR);
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
        public virtual IDetectionPostProcessor GetPostProcessor() {
            return postProcessor;
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnxtr.Detection.OnnxDetectionPredictorProperties that = (iText.Pdfocr.Onnxtr.Detection.OnnxDetectionPredictorProperties
                )o;
            return Object.Equals(modelPath, that.modelPath) && Object.Equals(inputProperties, that.inputProperties) &&
                 Object.Equals(postProcessor, that.postProcessor);
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)modelPath, inputProperties, postProcessor);
        }

        public override String ToString() {
            return "OnnxDetectionPredictorProperties{" + "modelPath='" + modelPath + '\'' + ", inputProperties=" + inputProperties
                 + ", postProcessor=" + postProcessor + '}';
        }
    }
}
