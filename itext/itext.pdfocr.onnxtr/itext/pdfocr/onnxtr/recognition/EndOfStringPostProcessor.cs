/*
Copyright (C) 2021-2024, Mindee | Felix Dittrich.

This program is licensed under the Apache License 2.0.
See <https://opensource.org/licenses/Apache-2.0> for full license details.
*/
using System;
using System.Text;
using iText.Pdfocr.Onnxtr;
using iText.Pdfocr.Onnxtr.Util;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr.Recognition {
    /// <summary>Implementation of a text recognition predictor post-processor, used for OnnxTR non-CRNN model outputs.
    ///     </summary>
    /// <remarks>
    /// Implementation of a text recognition predictor post-processor, used for OnnxTR non-CRNN model outputs.
    /// <para />
    /// This assumes there is an end-of-string token just after the vocabulary. You can specify additional tokens afterward,
    /// but they are not used in the processing. No same character aggregation is done. Output is read till an end-of-string
    /// token in encountered.
    /// </remarks>
    public class EndOfStringPostProcessor : IRecognitionPostProcessor {
        /// <summary>Vocabulary used for the model output (without special tokens).</summary>
        private readonly Vocabulary vocabulary;

        /// <summary>
        /// Amount of additional tokens in the total vocabulary after the
        /// end-of-string token.
        /// </summary>
        private readonly int additionalTokens;

        /// <summary>Creates a new post-processor.</summary>
        /// <param name="vocabulary">vocabulary used for the model output (without special tokens)</param>
        /// <param name="additionalTokens">amount of additional tokens in the total vocabulary after the end-of-string token
        ///     </param>
        public EndOfStringPostProcessor(Vocabulary vocabulary, int additionalTokens) {
            this.vocabulary = Objects.RequireNonNull(vocabulary);
            this.additionalTokens = additionalTokens;
        }

        /// <summary>Creates a new post-processor without any additional tokens.</summary>
        /// <param name="vocabulary">vocabulary used for the model output (without special tokens)</param>
        public EndOfStringPostProcessor(Vocabulary vocabulary)
            : this(vocabulary, 0) {
        }

        /// <summary>Creates a new post-processor with the default vocabulary.</summary>
        public EndOfStringPostProcessor()
            : this(Vocabulary.FRENCH, 0) {
        }

        /// <summary><inheritDoc/></summary>
        public virtual String Process(FloatBufferMdArray output) {
            int maxWordLength = output.GetDimension(0);
            StringBuilder wordBuilder = new StringBuilder(maxWordLength);
            float[] values = new float[LabelDimension()];
            float[] outputBuffer = output.GetData();
            int arrayOffset = output.GetArrayOffset();
            for (int i = arrayOffset; i < arrayOffset + output.GetArraySize(); i += values.Length) {
                Array.Copy(outputBuffer, i, values, 0, values.Length);
                int letterIndex = MathUtil.Argmax(values);
                if (letterIndex < vocabulary.Size()) {
                    wordBuilder.Append(vocabulary.Map(letterIndex));
                }
                else {
                    if (letterIndex == vocabulary.Size()) {
                        // If found end-of-sentence "<eos>" token
                        break;
                    }
                }
            }
            return wordBuilder.ToString();
        }

        /// <summary><inheritDoc/></summary>
        public virtual int LabelDimension() {
            // +1 is for "<eos>" token itself
            return vocabulary.Size() + 1 + additionalTokens;
        }
    }
}
