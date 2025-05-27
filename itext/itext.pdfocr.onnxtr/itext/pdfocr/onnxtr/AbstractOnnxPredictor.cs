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
using AI.Onnxruntime;
using iText.Commons.Utils;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Onnxtr.Util;

namespace iText.Pdfocr.Onnxtr {
    /// <summary>Abstract predictor, based on models running over ONNX runtime.</summary>
    /// <typeparam name="T">predictor input type</typeparam>
    /// <typeparam name="R">predictor output type</typeparam>
    public abstract class AbstractOnnxPredictor<T, R> : IPredictor<T, R> {
        /// <summary>Model input properties.</summary>
        private readonly OnnxInputProperties inputProperties;

        /// <summary>ONNX runtime session options.</summary>
        /// <remarks>
        /// ONNX runtime session options. TODO DEVSIX-9154 Not sure, whether
        /// <see cref="AI.Onnxruntime.OrtSession"/>
        /// takes
        /// ownership of options, when you pass them, so it might not be safe to
        /// close the object just after the session creation. So storing to dispose
        /// it after session disposal.
        /// </remarks>
        private readonly OrtSession.SessionOptions sessionOptions;

        /// <summary>ONNX runtime session.</summary>
        /// <remarks>ONNX runtime session. Contains the machine learning model.</remarks>
        private readonly OrtSession session;

        /// <summary>Key for the singular input of a model.</summary>
        private readonly String inputName;

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
            catch (OrtException e) {
                throw new PdfOcrException("Failed to init ONNX Runtime session options", e);
            }
            try {
                this.session = OrtEnvironment.GetEnvironment().CreateSession(modelPath, sessionOptions);
            }
            catch (Exception e) {
                this.sessionOptions.Close();
                throw new PdfOcrException("Failed to init ONNX Runtime session", e);
            }
            try {
                this.inputName = ValidateModel(this.session, inputProperties, outputShape);
            }
            catch (Exception e) {
                PdfOcrException userException = new PdfOcrException("ONNX Runtime model did not pass validation", e);
                try {
                    this.session.Close();
                }
                catch (OrtException closeException) {
                    userException.AddSuppressed(closeException);
                }
                this.sessionOptions.Close();
                throw userException;
            }
        }

        public virtual IEnumerator<R> Predict(IEnumerator<T> inputs) {
            return new BatchProcessingGenerator<T, R>(Batching.Wrap(inputs, inputProperties.GetBatchSize()), (batch) => {
                try {
                    using (OnnxTensor inputTensor = CreateTensor(ToInputBuffer(batch))) {
                        using (OrtSession.Result outputTensor = session.Run(JavaCollectionsUtil.SingletonMap(inputName, inputTensor
                            ))) {
                            return FromOutputBuffer(batch, ParseModelOutput(outputTensor));
                        }
                    }
                }
                catch (OrtException e) {
                    throw new PdfOcrException("ONNX Runtime operation failed", e);
                }
            }
            );
        }

        public virtual void Close() {
            try {
                session.Close();
                sessionOptions.Close();
            }
            catch (OrtException e) {
                throw new PdfOcrException("Failed to close an ONNX Runtime session", e);
            }
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

        private static OrtSession.SessionOptions CreateDefaultSessionOptions() {
            OrtSession.SessionOptions ortOptions = new OrtSession.SessionOptions();
            try {
                ortOptions.AddCPU(true);
                if (OrtEnvironment.GetAvailableProviders().Contains(OrtProvider.CUDA)) {
                    ortOptions.AddCUDA();
                }
                ortOptions.SetExecutionMode(OrtSession.SessionOptions.ExecutionMode.SEQUENTIAL);
                ortOptions.SetOptimizationLevel(OrtSession.SessionOptions.OptLevel.ALL_OPT);
                ortOptions.SetIntraOpNumThreads(-1);
                ortOptions.SetInterOpNumThreads(-1);
                return ortOptions;
            }
            catch (Exception e) {
                ortOptions.Close();
                throw;
            }
        }

        private static OnnxTensor CreateTensor(FloatBufferMdArray batch) {
            return OnnxTensor.CreateTensor(OrtEnvironment.GetEnvironment(), batch.GetData(), batch.GetShape());
        }

        /// <summary>Validates model, loaded in session, against expected inputs and outputs.</summary>
        /// <remarks>
        /// Validates model, loaded in session, against expected inputs and outputs.
        /// If model is invalid, then an exception is thrown.
        /// </remarks>
        /// <param name="session">TODO DEVSIX-9154</param>
        /// <param name="properties">TODO DEVSIX-9154</param>
        /// <param name="outputShape">TODO DEVSIX-9154</param>
        /// <returns>input info</returns>
        private static String ValidateModel(OrtSession session, OnnxInputProperties properties, long[] outputShape
            ) {
            String inputName = ValidateModelInput(session, properties);
            ValidateModelOutput(session, outputShape);
            return inputName;
        }

        private static String ValidateModelInput(OrtSession session, OnnxInputProperties properties) {
            ICollection<NodeInfo> inputInfo = session.GetInputInfo().Values;
            if (inputInfo.Count != 1) {
                throw new ArgumentException("Expected 1 input, but got " + inputInfo.Count + " instead");
            }
            NodeInfo inputNodeInfo = inputInfo.Iterator().Next();
            ValueInfo inputNodeValueInfo = inputNodeInfo.GetInfo();
            if (!(inputNodeValueInfo is TensorInfo)) {
                throw new ArgumentException("Unexpected input type, expected float32 tensor");
            }
            TensorInfo inputTensorInfo = (TensorInfo)inputNodeValueInfo;
            if (inputTensorInfo.type != OnnxJavaType.FLOAT) {
                throw new ArgumentException("Unexpected input type, expected float32 tensor");
            }
            long[] inputShape = inputTensorInfo.GetShape();
            if (IsShapeIncompatible(properties.GetShape(), inputShape)) {
                throw new ArgumentException("Expected " + JavaUtil.ArraysToString(properties.GetShape()) + " input shape, "
                     + "but got " + JavaUtil.ArraysToString(inputShape) + " instead");
            }
            return inputNodeInfo.GetName();
        }

        private static void ValidateModelOutput(OrtSession session, long[] expectedOutputShape) {
            ICollection<NodeInfo> outputInfo = session.GetOutputInfo().Values;
            if (outputInfo.Count != 1) {
                throw new ArgumentException("Expected 1 output, but got " + outputInfo.Count + " instead");
            }
            NodeInfo outputNodeInfo = outputInfo.Iterator().Next();
            ValueInfo outputNodeValueInfo = outputNodeInfo.GetInfo();
            if (!(outputNodeValueInfo is TensorInfo)) {
                throw new ArgumentException("Unexpected output type, expected float32 tensor");
            }
            TensorInfo outputTensorInfo = (TensorInfo)outputNodeValueInfo;
            if (outputTensorInfo.type != OnnxJavaType.FLOAT) {
                throw new ArgumentException("Unexpected output type, expected float32 tensor");
            }
            long[] actualOutputShape = outputTensorInfo.GetShape();
            if (IsShapeIncompatible(expectedOutputShape, actualOutputShape)) {
                throw new ArgumentException("Expected " + JavaUtil.ArraysToString(expectedOutputShape) + " output shape, "
                     + "but got " + JavaUtil.ArraysToString(actualOutputShape) + " instead");
            }
        }

        /// <summary>Wraps a model output into an MD-array.</summary>
        /// <param name="result">model output</param>
        /// <returns>MD-array wrapper</returns>
        private static FloatBufferMdArray ParseModelOutput(OrtSession.Result result) {
            OnnxValue output = result.Get(0);
            TensorInfo outputInfo = (TensorInfo)output.GetInfo();
            long[] outputShape = outputInfo.GetShape();
            FloatBuffer outputBuffer = ((OnnxTensor)output).GetFloatBuffer();
            return new FloatBufferMdArray(outputBuffer, outputShape);
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
    }
}
