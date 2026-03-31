/*
Copyright (C) 2021-2024, Mindee | Felix Dittrich.

This program is licensed under the Apache License 2.0.
See <https://opensource.org/licenses/Apache-2.0> for full license details.
*/
using System.Text;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnx.Recognition {
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
    public class CrnnPostProcessor : BasicLabelPostProcessor {
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

        /// <summary><inheritDoc/></summary>
        protected internal override void AppendLabel(StringBuilder output, int labelIndex) {
            // Last letter is <blank>
            if (labelIndex < vocabulary.Size()) {
                output.Append(vocabulary.Map(labelIndex));
            }
        }

        /// <summary><inheritDoc/></summary>
        public override int LabelDimension() {
            // +1 is "<blank>" token at the end
            return vocabulary.Size() + 1;
        }
    }
}
