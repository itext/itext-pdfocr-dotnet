using System;
using System.Text;
using iText.Pdfocr.Onnxtr;
using iText.Pdfocr.Onnxtr.Util;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr.Recognition {
    /// <summary>
    /// Implementation of a text recognition predictor post-processor, used for
    /// OnnxTR CRNN model outputs.
    /// </summary>
    /// <remarks>
    /// Implementation of a text recognition predictor post-processor, used for
    /// OnnxTR CRNN model outputs.
    /// <para />
    /// Notably it does not have end-of-string tokens. Only token, besides the
    /// vocabulary one, is blank, which is just skipped or used as a char separator.
    /// Multiple of the same label in a row is aggregated into one.
    /// </remarks>
    public class CrnnPostProcessor : IRecognitionPostProcessor {
        /// <summary>Vocabulary used for the model output (without special tokens).</summary>
        private readonly Vocabulary vocabulary;

        /// <summary>Creates a new post-processor.</summary>
        /// <param name="vocabulary">vocabulary used for the model output (without special tokens)</param>
        public CrnnPostProcessor(Vocabulary vocabulary) {
            this.vocabulary = Objects.RequireNonNull(vocabulary);
        }

        /// <summary>Creates a new post-processor with the default vocabulary.</summary>
        public CrnnPostProcessor() {
            this.vocabulary = Vocabulary.FRENCH;
        }

        public virtual String Process(FloatBufferMdArray output) {
            int maxWordLength = output.GetDimension(0);
            StringBuilder wordBuilder = new StringBuilder(maxWordLength);
            float[] values = new float[LabelDimension()];
            float[] outputBuffer = output.GetData();
            int prevLetterIndex = -1;
            int arrayOffset = output.GetArrayOffset();
            for (int i = arrayOffset; i < arrayOffset + output.GetArraySize(); i += values.Length) {
                Array.Copy(outputBuffer, i, values, 0, values.Length);
                int letterIndex = MathUtil.Argmax(values);
                // Last letter is <blank>
                if (prevLetterIndex != letterIndex && letterIndex < vocabulary.Size()) {
                    wordBuilder.Append(vocabulary.Map(letterIndex));
                }
                prevLetterIndex = letterIndex;
            }
            return wordBuilder.ToString();
        }

        public virtual int LabelDimension() {
            // +1 is "<blank>" token
            return vocabulary.Size() + 1;
        }
    }
}
