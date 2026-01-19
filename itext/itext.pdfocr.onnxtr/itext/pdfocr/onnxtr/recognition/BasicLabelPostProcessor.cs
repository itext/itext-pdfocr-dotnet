/*
Copyright (C) 2021-2024, Mindee | Felix Dittrich.

This program is licensed under the Apache License 2.0.
See <https://opensource.org/licenses/Apache-2.0> for full license details.
*/
using System;
using System.Text;
using iText.Pdfocr.Onnxtr;
using iText.Pdfocr.Onnxtr.Util;

namespace iText.Pdfocr.Onnxtr.Recognition {
    /// <summary>Abstract Implementation of a basic text recognition predictor post-processor.</summary>
    /// <remarks>
    /// Abstract Implementation of a basic text recognition predictor post-processor. It contains logic, which is common
    /// between OnnxTR, EasyOCR and PaddleOCR label mappers:
    /// <list type="bullet">
    /// <item><description>It receives a two-dimensional array with a (maxStringLength, labelDimension) shape.
    /// </description></item>
    /// <item><description>Label with the highest value in the array is picked as the recognized one.
    /// </description></item>
    /// <item><description>There are labels, which should not be added to the string, and should just be treated as separators.
    /// </description></item>
    /// <item><description>If the same label appears multiple times in a row in a string, it is returned only once.
    /// </description></item>
    /// </list>
    /// </remarks>
    public abstract class BasicLabelPostProcessor : IRecognitionPostProcessor {
        /// <summary><inheritDoc/></summary>
        public virtual String Process(FloatBufferMdArray output) {
            int maxStringLength = output.GetDimension(0);
            int labelStride = output.GetDimension(1);
            StringBuilder stringBuilder = new StringBuilder(maxStringLength);
            float[] values = new float[Math.Min(LabelDimension(), labelStride)];
            float[] outputBuffer = output.GetData();
            int prevLabelIndex = -1;
            int arrayOffset = output.GetArrayOffset();
            for (int i = arrayOffset; i < arrayOffset + output.GetArraySize(); i += labelStride) {
                Array.Copy(outputBuffer, i, values, 0, values.Length);
                int labelIndex = MathUtil.Argmax(values);
                if (prevLabelIndex != labelIndex) {
                    AppendLabel(stringBuilder, labelIndex);
                }
                prevLabelIndex = labelIndex;
            }
            return stringBuilder.ToString();
        }

        /// <summary>Adds label to the string output, based on the label's index.</summary>
        /// <remarks>
        /// Adds label to the string output, based on the label's index. Can be a
        /// noop, if label index should be ignored.
        /// </remarks>
        /// <param name="output">string builder to append the label to</param>
        /// <param name="labelIndex">
        /// index of the label to append, guaranteed to be in the
        /// [0; labelDimension()) range.
        /// </param>
        protected internal abstract void AppendLabel(StringBuilder output, int labelIndex);

        public abstract int LabelDimension();
    }
}
