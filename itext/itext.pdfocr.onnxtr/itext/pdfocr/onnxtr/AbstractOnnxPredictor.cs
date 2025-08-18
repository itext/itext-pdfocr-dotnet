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
using System.IO;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using iText.Commons.Utils;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Pdfocr.Onnxtr.Util;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr {
    /// <summary>Abstract predictor, based on models running over ONNX runtime.</summary>
    /// <typeparam name="T">predictor input type</typeparam>
    /// <typeparam name="R">predictor output type</typeparam>
    public abstract class AbstractOnnxPredictor<T, R> : IPredictor<T, R> {
        /// <summary>Model input properties.</summary>
        private readonly OnnxInputProperties inputProperties;

        /// <summary>ONNX runtime session options.</summary>
        /// <remarks>
        /// ONNX runtime session options.
        /// <see cref="AI.Onnxruntime.OrtSession"/>
        /// does not take ownership of the options, when you pass them.
        /// It uses the options during initialization but does not manage their lifetime afterward. So storing to dispose
        /// it after session disposal.
        /// </remarks>
        private readonly SessionOptions sessionOptions;

        /// <summary>ONNX runtime session.</summary>
        /// <remarks>ONNX runtime session. Contains the machine learning model.</remarks>
        private readonly InferenceSession session;

        /// <summary>Key for the singular input of a model.</summary>
        private readonly String inputName;

        /// <summary>Close status of the predictor.</summary>
        private bool closed = false;

        static AbstractOnnxPredictor() {
            try {
                // OnnxRuntime initialization is called under the hood.
                new SessionOptions().Close();
            }
            catch (Exception e) {
                DependencyLoadChecker.ProcessException(e);
                throw;
            }
        }

        /// <summary>Creates a new abstract predictor.</summary>
        /// <remarks>
        /// Creates a new abstract predictor.
        /// <para />
        /// If the specified model does not match input and output properties, it will throw an exception.
        /// </remarks>
        /// <param name="modelPath">path to the ONNX runtime model to load</param>
        /// <param name="inputProperties">expected input properties of a model</param>
        /// <param name="outputShape">
        /// expected shape of the output. -1 entries mean that the dimension can be
        /// of any size (ex. batch size)
        /// </param>
        protected internal AbstractOnnxPredictor(String modelPath, OnnxInputProperties inputProperties, long[] outputShape
            ) {
            this.inputProperties = Objects.RequireNonNull(inputProperties);
            try {
                this.sessionOptions = CreateDefaultSessionOptions();
            }
            catch (OnnxRuntimeException e) {
                throw new PdfOcrException(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_INIT_SESSION_OPTIONS, e);
            }
            try {
                this.session = new InferenceSession(File.ReadAllBytes(modelPath), sessionOptions);
            }
            catch (Exception e) {
                this.sessionOptions.Close();
                throw new PdfOcrException(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_INIT_ONNX_RUNTIME_SESSION, e);
            }
            try {
                this.inputName = ValidateModel(this.session, inputProperties, outputShape);
            }
            catch (Exception e) {
                PdfOcrException userException = new PdfOcrException(
                    PdfOcrOnnxTrExceptionMessageConstant.MODEL_DID_NOT_PASS_VALIDATION, e);
                try {
                    this.session.Dispose();
                }
                catch (OnnxRuntimeException closeException) {
                    userException = new PdfOcrException(
                        PdfOcrOnnxTrExceptionMessageConstant.MODEL_DID_NOT_PASS_VALIDATION + " " + e.Message, 
                        closeException);
                }
                this.sessionOptions.Close();
                throw userException;
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual IEnumerator<R> Predict(IEnumerator<T> inputs) {
            return new BatchProcessingGenerator<T, R>(Batching.Wrap(inputs, inputProperties.GetBatchSize()),
                new BatchProcessor(this));
        }

        /// <summary><inheritDoc/></summary>
        public IEnumerator<R> Predict(IEnumerable<T> inputs) {
            return Predict(inputs.GetEnumerator());
        }

        private sealed class BatchProcessor : IBatchProcessor<T, R> {
            private AbstractOnnxPredictor<T, R> predictor;
            public BatchProcessor(AbstractOnnxPredictor<T, R> predictor) {
                this.predictor = predictor;
            }

            public IList<R> ProcessBatch(IList<T> batch) {
                try {
                    DenseTensor<float> inputTensor = CreateTensor(predictor.ToInputBuffer(batch));
                    NamedOnnxValue namedOnnxValue = NamedOnnxValue.CreateFromTensor(predictor.inputName, inputTensor);
                    using (IDisposableReadOnlyCollection<DisposableNamedOnnxValue> outputTensor = predictor.session.Run(new[] { namedOnnxValue })) {
                        return predictor.FromOutputBuffer(batch, ParseModelOutput(outputTensor));
                    }
                }
                catch (OnnxRuntimeException e) {
                    throw new PdfOcrException(PdfOcrOnnxTrExceptionMessageConstant.ONNX_RUNTIME_OPERATION_FAILED, e);
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Close() {
            if (closed) {
                return;
            }
            try {
                session.Dispose();
                sessionOptions.Close();
            }
            catch (OnnxRuntimeException e) {
                throw new PdfOcrException(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_CLOSE_ONNX_RUNTIME_SESSION, e);
            }
            closed = true;
        }

        /// <summary>Converts predictor inputs to an ONNX runtime model batched input MD-array buffer.</summary>
        /// <param name="batch">batch of raw predictor inputs</param>
        /// <returns>batched model input MD-array buffer</returns>
        protected internal abstract FloatBufferMdArray ToInputBuffer(IList<T> batch);

        /// <summary>Converts ONNX runtime model batched output MD-array buffer to a list of predictor outputs.</summary>
        /// <param name="inputBatch">list of raw predictor inputs, matching the output</param>
        /// <param name="outputBatch">batched model output MD-array buffer</param>
        /// <returns>a list of predictor output</returns>
        protected internal abstract IList<R> FromOutputBuffer(IList<T> inputBatch, FloatBufferMdArray outputBatch);

        private static SessionOptions CreateDefaultSessionOptions() {
            SessionOptions ortOptions = new SessionOptions();
            try {
                ortOptions.AppendExecutionProvider_CPU();
#if USE_CUDA
                ortOptions.AppendExecutionProvider_CUDA(0);
#endif
                ortOptions.ExecutionMode = ExecutionMode.ORT_SEQUENTIAL;
                ortOptions.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;
                ortOptions.IntraOpNumThreads = -1;
                ortOptions.InterOpNumThreads = -1;
                return ortOptions;
            }
            catch (Exception e) {
                ortOptions.Close();
                throw;
            }
        }

        private static DenseTensor<float> CreateTensor(FloatBufferMdArray batch) {
            float[] floatData = batch.GetData();
            long[] shape = batch.GetShape();
            int[] intShape = shape.Select(s => (int)s).ToArray();

            return new DenseTensor<float>(floatData, intShape);
        }

        /// <summary>Validates model, loaded in session, against expected inputs and outputs.</summary>
        /// <remarks>
        /// Validates model, loaded in session, against expected inputs and outputs.
        /// If model is invalid, then an exception is thrown.
        /// </remarks>
        /// <param name="session">
        /// current
        /// <see cref="AI.Onnxruntime.OrtSession"/>
        /// with the loaded ONNX runtime model
        /// </param>
        /// <param name="properties">
        /// 
        /// <see cref="OnnxInputProperties"/>
        /// properties of the input of an ONNX model which expects an RGB image
        /// </param>
        /// <param name="outputShape">expected shape of the output. -1 entries mean that the dimension can be of any size
        ///     </param>
        /// <returns>input info</returns>
        private static String ValidateModel(InferenceSession session, OnnxInputProperties properties, long[] outputShape
            ) {
            String inputName = ValidateModelInput(session, properties);
            ValidateModelOutput(session, outputShape);
            return inputName;
        }

        private static String ValidateModelInput(InferenceSession session, OnnxInputProperties properties) {
            IEnumerable<NodeMetadata> inputInfo = session.InputMetadata.Values;
            if (inputInfo.Count() != 1) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_INPUT_SIZE
                    , inputInfo.Count()));
            }
            NodeMetadata inputNodeInfo = inputInfo.First();
            long[] inputShape = Array.ConvertAll(inputNodeInfo.Dimensions, item => (long)item);
            if (IsShapeIncompatible(properties.GetShape(), inputShape)) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_INPUT_SHAPE
                    , JavaUtil.ArraysToString(properties.GetShape()), JavaUtil.ArraysToString(inputShape)));
            }
            return session.InputNames.First();
        }

        private static void ValidateModelOutput(InferenceSession session, long[] expectedOutputShape) {
            IEnumerable<NodeMetadata> outputInfo = session.OutputMetadata.Values;
            if (outputInfo.Count() != 1) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_OUTPUT_SIZE
                    , outputInfo.Count()));
            }
            NodeMetadata outputNodeInfo = outputInfo.First();
            
            int[] actualOutputShape = outputNodeInfo.Dimensions;
            if (IsShapeIncompatible(expectedOutputShape, Array.ConvertAll(actualOutputShape, item => (long)item))) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_OUTPUT_SHAPE
                    , JavaUtil.ArraysToString(expectedOutputShape), JavaUtil.ArraysToString(actualOutputShape)));
            }
        }

        /// <summary>Wraps a model output into an MD-array.</summary>
        /// <param name="result">model output</param>
        /// <returns>MD-array wrapper</returns>
        private static FloatBufferMdArray ParseModelOutput(IDisposableReadOnlyCollection<DisposableNamedOnnxValue> result) {
            DisposableNamedOnnxValue output = result[0];
            Tensor<float> outputInfo = output.AsTensor<float>();
            ReadOnlySpan<int> outputShape = outputInfo.Dimensions;
            float[] outputBuffer = outputInfo.ToArray();
            return new FloatBufferMdArray(outputBuffer, Array.ConvertAll(outputShape.ToArray(), item => (long)item));
        }

        /// <summary>Returns whether two shapes are compatible.</summary>
        /// <remarks>Returns whether two shapes are compatible. I.e. have the same size and dimensions (except for -1 wildcards).
        ///     </remarks>
        /// <param name="expectedShape">expected shape</param>
        /// <param name="actualShape">actual model shape</param>
        /// <returns>whether shapes are compatible</returns>
        private static bool IsShapeIncompatible(long[] expectedShape, long[] actualShape) {
            if (actualShape.Length != expectedShape.Length) {
                return true;
            }
            for (int i = 0; i < actualShape.Length; ++i) {
                // -1 is flexible, so can be skipped
                if (actualShape[i] != expectedShape[i] && actualShape[i] != -1 && expectedShape[i] != -1) {
                    return true;
                }
            }
            return false;
        }

        public void Dispose() {
            Close();
        }
    }
}
