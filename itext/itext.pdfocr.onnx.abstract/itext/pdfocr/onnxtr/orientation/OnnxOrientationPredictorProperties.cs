/*
Copyright (C) 2021-2024, Mindee | Felix Dittrich.

This program is licensed under the Apache License 2.0.
See <https://opensource.org/licenses/Apache-2.0> for full license details.
*/
using System;
using iText.Commons.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Onnxtr;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr.Orientation {
    /// <summary>Properties for configuring crop orientation ONNX models.</summary>
    /// <remarks>
    /// Properties for configuring crop orientation ONNX models.
    /// <para />
    /// It contains a path to the model, model input properties and a model output mapper.
    /// </remarks>
    public class OnnxOrientationPredictorProperties : AbstractOnnxPredictorProperties {
        private static readonly OnnxInputProperties DEFAULT_INPUT_PROPERTIES = new OnnxInputProperties(new ImageResizeOptions
            (ImageChannelConfiguration.RGB, 256, 256, PaddingStrategy.SYMMETRIC_BLACK), new float[] { 0.694F, 0.695F
            , 0.693F }, new float[] { 0.299F, 0.296F, 0.301F }, 64);

        private static readonly DefaultOrientationMapper DEFAULT_OUTPUT_MAPPER = new DefaultOrientationMapper();

        /// <summary>Properties of the outputs of the ONNX model.</summary>
        /// <remarks>Properties of the outputs of the ONNX model. Used for validation and post-processing.</remarks>
        private readonly IOutputLabelMapper<TextOrientation> outputMapper;

        /// <summary>Creates new crop orientation predictor properties.</summary>
        /// <param name="modelPath">path to the ONNX model to load</param>
        /// <param name="inputProperties">ONNX model input properties</param>
        /// <param name="outputMapper">ONNX model output mapper</param>
        public OnnxOrientationPredictorProperties(String modelPath, OnnxInputProperties inputProperties, IOutputLabelMapper
            <TextOrientation> outputMapper)
            : this(modelPath, inputProperties, outputMapper, DEFAULT_ORT_SESSION_CREATOR) {
        }

        /// <summary>Creates new crop orientation predictor properties.</summary>
        /// <param name="modelPath">path to the ONNX model to load</param>
        /// <param name="inputProperties">ONNX model input properties</param>
        /// <param name="outputMapper">ONNX model output mapper</param>
        /// <param name="ortSessionOptionsCreator">ONNX runtime session options creator</param>
        public OnnxOrientationPredictorProperties(String modelPath, OnnxInputProperties inputProperties, IOutputLabelMapper
            <TextOrientation> outputMapper, IOrtSessionOptionsCreator ortSessionOptionsCreator)
            : base(modelPath, inputProperties, ortSessionOptionsCreator) {
            this.outputMapper = Objects.RequireNonNull(outputMapper);
        }

        /// <summary>
        /// Creates a new crop orientation properties object for existing pre-trained
        /// MobileNetV3 models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new crop orientation properties object for existing pre-trained
        /// MobileNetV3 models, stored on disk. This is the only crop orientation
        /// model architecture available in OnnxTR.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/mobilenet_v3_small_crop_orientation-5620cf7e.onnx">
        /// mobilenet_v3_small_crop_orientation
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/mobilenet_v3_small_crop_orientation_static_8_bit-4cfaa621.onnx">
        /// mobilenet_v3_small_crop_orientation (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <returns>a new crop orientation properties object for a MobileNetV3 model</returns>
        public static iText.Pdfocr.Onnxtr.Orientation.OnnxOrientationPredictorProperties MobileNetV3(String modelPath
            ) {
            return MobileNetV3(modelPath, DEFAULT_ORT_SESSION_CREATOR);
        }

        /// <summary>
        /// Creates a new crop orientation properties object for existing pre-trained
        /// MobileNetV3 models, stored on disk.
        /// </summary>
        /// <remarks>
        /// Creates a new crop orientation properties object for existing pre-trained
        /// MobileNetV3 models, stored on disk. This is the only crop orientation
        /// model architecture available in OnnxTR.
        /// <para />
        /// This can be used to load the following models from OnnxTR:
        /// <list type="bullet">
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.0.1/mobilenet_v3_small_crop_orientation-5620cf7e.onnx">
        /// mobilenet_v3_small_crop_orientation
        /// </a>
        /// </description></item>
        /// <item><description>
        /// <a href="https://github.com/felixdittrich92/onnxtr/releases/download/v0.1.2/mobilenet_v3_small_crop_orientation_static_8_bit-4cfaa621.onnx">
        /// mobilenet_v3_small_crop_orientation (8-bit quantized)
        /// </a>
        /// </description></item>
        /// </list>
        /// </remarks>
        /// <param name="modelPath">path to the pre-trained model</param>
        /// <param name="ortSessionOptionsCreator">the ONNX runtime session options creator</param>
        /// <returns>a new crop orientation properties object for a MobileNetV3 model</returns>
        public static iText.Pdfocr.Onnxtr.Orientation.OnnxOrientationPredictorProperties MobileNetV3(String modelPath
            , IOrtSessionOptionsCreator ortSessionOptionsCreator) {
            return new iText.Pdfocr.Onnxtr.Orientation.OnnxOrientationPredictorProperties(modelPath, DEFAULT_INPUT_PROPERTIES
                , DEFAULT_OUTPUT_MAPPER, ortSessionOptionsCreator);
        }

        /// <summary>Returns the ONNX model output mapper.</summary>
        /// <returns>the ONNX model output mapper</returns>
        public virtual IOutputLabelMapper<TextOrientation> GetOutputMapper() {
            return outputMapper;
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)modelPath, inputProperties, outputMapper, ortSessionOptionsCreator);
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnxtr.Orientation.OnnxOrientationPredictorProperties that = (iText.Pdfocr.Onnxtr.Orientation.OnnxOrientationPredictorProperties
                )o;
            return Object.Equals(modelPath, that.modelPath) && Object.Equals(inputProperties, that.inputProperties) &&
                 Object.Equals(outputMapper, that.outputMapper) && Object.Equals(ortSessionOptionsCreator, that.ortSessionOptionsCreator
                );
        }

        /// <summary><inheritDoc/></summary>
        public override String ToString() {
            return "OnnxOrientationPredictorProperties{" + "modelPath='" + modelPath + '\'' + ", inputProperties=" + inputProperties
                 + ", outputMapper=" + outputMapper + '}';
        }
    }
}
