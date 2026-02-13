/*
Copyright (C) 2021-2024, Mindee | Felix Dittrich.

This program is licensed under the Apache License 2.0.
See <https://opensource.org/licenses/Apache-2.0> for full license details.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iText.Commons.Utils;
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Pdfocr.Onnxtr.Util;

namespace iText.Pdfocr.Onnxtr.Recognition {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Small utility class, which handles text box splitting for text recognition
    /// models, based on the algorithm from OnnxTR.
    /// </summary>
    /// <remarks>
    /// Small utility class, which handles text box splitting for text recognition
    /// models, based on the algorithm from OnnxTR.
    /// <para />
    /// This is not really useful for other models, like EasyOCR and PaddleOCR
    /// ones, as those have a dynamic input size and can have whatever long lines.
    /// They also work on lines and not words, so they wouldn't mind additional
    /// information in one go.
    /// </remarks>
    internal class TextBoxSplitter {
        /// <summary>Aspect ratio, at which a text box is split for better text recognition.</summary>
        private const float SPLIT_CROPS_MAX_RATIO = 8;

        /// <summary>Target aspect ratio for the text box splits.</summary>
        private const float SPLIT_CROPS_TARGET_RATIO = 6;

        /// <summary>Multiplier, which controls the overlap between splits.</summary>
        /// <remarks>
        /// Multiplier, which controls the overlap between splits. Factor of 1 means, that there will be no overlap.
        /// <para />
        /// This is for cases, when a split happens in the middle of a character. With some overlap, at least one of the
        /// sub-images will contain the character in full.
        /// </remarks>
        private const float SPLIT_CROPS_DILATION_FACTOR = 1.4F;

        /// <summary>State for the current inputs split.</summary>
        /// <remarks>State for the current inputs split. Contains the number of images that were spawned from the original image.
        ///     </remarks>
        private readonly LinkedList<int> mergeQueue = new LinkedList<int>();

        /// <summary>Creates new <see cref="TextBoxSplitter"/> instance.</summary>
        public TextBoxSplitter() {
            // Empty constructor in order for default one to not be removed if another one is added.
        }

        /// <summary>
        /// Wrap the iterator of the text recognition predictor inputs, which
        /// splits the images, if they have too skewed aspect ratio.
        /// </summary>
        /// <param name="inputs">models inputs to split</param>
        /// <returns>iterator, which outputs split input images</returns>
        public virtual IEnumerator<IronSoftware.Drawing.AnyBitmap> MapInputs(IEnumerator<IronSoftware.Drawing.AnyBitmap
            > inputs) {
            if (!mergeQueue.IsEmpty()) {
                throw new InvalidOperationException(PdfOcrOnnxTrExceptionMessageConstant.CANNOT_START_ANOTHER_MAPPING_OPERATION
                    );
            }
            return new _IEnumerator_67(this, inputs);
        }

        private sealed class _IEnumerator_67 : IEnumerator<IronSoftware.Drawing.AnyBitmap> {
            public _IEnumerator_67(TextBoxSplitter _enclosing, IEnumerator<IronSoftware.Drawing.AnyBitmap> inputs) {
                this._enclosing = _enclosing;
                this.inputs = inputs;
                this.buffer = new LinkedList<IronSoftware.Drawing.AnyBitmap>();
            }

            private readonly LinkedList<IronSoftware.Drawing.AnyBitmap> buffer;

            public bool MoveNext() {
                return !this.buffer.IsEmpty() || inputs.MoveNext();
            }

            object IEnumerator.Current => Current;

            public IronSoftware.Drawing.AnyBitmap Current {
                get {
                    // Emptying the buffer first
                    if (!this.buffer.IsEmpty()) {
                        IronSoftware.Drawing.AnyBitmap first = this.buffer.First.Value;
                        this.buffer.RemoveFirst();
                        return first;
                    }
                    IronSoftware.Drawing.AnyBitmap image = inputs.Current;
                    int width = BufferedImageUtil.GetWidth(image);
                    int height = BufferedImageUtil.GetHeight(image);
                    double aspectRatio = (double)width / height;
                    if (aspectRatio < TextBoxSplitter.SPLIT_CROPS_MAX_RATIO) {
                        this._enclosing.mergeQueue.AddLast(1);
                        // No need to touch the buffer here, just return as-is
                        return image;
                    }
                    // For some reason here is truncation in OnnxTR...
                    int splitCount = (int)Math.Ceiling(aspectRatio / TextBoxSplitter.SPLIT_CROPS_TARGET_RATIO);
                    float rawSplitWidth = (float)width / splitCount;
                    float targetSplitHalfWidth = (TextBoxSplitter.SPLIT_CROPS_DILATION_FACTOR * rawSplitWidth) / 2;
                    int nonEmptySplitCount = 0;
                    for (int j = 0; j < splitCount; ++j) {
                        float center = (j + 0.5F) * rawSplitWidth;
                        int minX = Math.Max(0, (int)Math.Floor(center - targetSplitHalfWidth));
                        int maxX = Math.Min(width - 1, (int)Math.Ceiling(center + targetSplitHalfWidth));
                        int currentSplitWidth = maxX - minX;
                        if (currentSplitWidth == 0) {
                            continue;
                        }
                        ++nonEmptySplitCount;
                        this.buffer.AddLast(image.GetSubimage(minX, 0, currentSplitWidth, height));
                    }
                    this._enclosing.mergeQueue.AddLast(nonEmptySplitCount);
                    IronSoftware.Drawing.AnyBitmap result = this.buffer.First.Value;
                    this.buffer.RemoveFirst();
                    return result;
                }
            }

            private readonly TextBoxSplitter _enclosing;

            private readonly IEnumerator<IronSoftware.Drawing.AnyBitmap> inputs;

            public void Reset() {
                inputs?.Reset();
            }

            public void Dispose() {
                inputs?.Dispose();
            }
        }

        /// <summary>
        /// Wrap the iterator of the text recognition predictor outputs, which
        /// merges text back, based on how the input images were split before.
        /// </summary>
        /// <param name="outputs">text outputs to merge</param>
        /// <returns>iterator, which returns merged images</returns>
        public virtual IEnumerator<String> MapOutputs(IEnumerator<String> outputs) {
            return new _IEnumerator_127(this, outputs);
        }

        private sealed class _IEnumerator_127 : IEnumerator<String> {
            public _IEnumerator_127(TextBoxSplitter _enclosing, IEnumerator<String> outputs) {
                this._enclosing = _enclosing;
                this.outputs = outputs;
            }

            public bool MoveNext() {
                return outputs.MoveNext();
            }

            object IEnumerator.Current => Current;

            public String Current {
                get {
                    // Important, that we "touch" the iterator first, as
                    // mergeQueue is lazily populated from it
                    String text = outputs.Current;
                    int stringPartsLeft = this._enclosing.mergeQueue.First();
                    this._enclosing.mergeQueue.RemoveFirst();
                    // Return as-is if no need to merge
                    if (stringPartsLeft == 1) {
                        return text;
                    }
                    // Otherwise doing the merge
                    StringBuilder sb = new StringBuilder(text);
                    while (stringPartsLeft > 1 && outputs.MoveNext()) {
                        TextBoxSplitter.MergeStrings(sb, outputs.Current);
                        --stringPartsLeft;
                    }
                    return sb.ToString();
                }
            }

            private readonly TextBoxSplitter _enclosing;

            private readonly IEnumerator<String> outputs;

            public void Reset() {
                outputs?.Reset();
            }

            public void Dispose() {
                outputs?.Dispose();
            }
        }

        /// <summary>Merges strings, collected from splits of text images.</summary>
        /// <param name="collector">string builder collector, which contains the current left part of the string</param>
        /// <param name="nextString">next string to add to the collector</param>
        private static void MergeStrings(StringBuilder collector, String nextString) {
            // Comments are also pretty much copies from OnnxTR...
            int commonLength = Math.Min(collector.Length, nextString.Length);
            double[] scores = new double[commonLength];
            for (int i = 0; i < commonLength; ++i) {
                scores[i] = MathUtil.CalculateLevenshteinDistance(collector.Substring(collector.Length - i - 1), nextString
                    .JSubstring(0, i + 1)) / (i + 1.0);
            }
            int index = 0;
            // Comparing floats to 0 is fine here, as it only happens, when the
            // integer nominator (i.e. Levenshtein distance) was 0
            if (commonLength > 1 && scores[0] == 0 && scores[1] == 0) {
                // Edge case (split in the middle of char repetitions): if it starts with 2 or more 0
                // Compute n_overlap (number of overlapping chars, geometrically determined)
                int overlap = (int)MathematicUtil.Round(nextString.Length * (SPLIT_CROPS_DILATION_FACTOR - 1) / SPLIT_CROPS_DILATION_FACTOR
                    );
                // Find the number of consecutive zeros in the scores list
                // Impossible to have a zero after a non-zero score in that case
                int zeros = (int)JavaUtil.ArraysToEnumerable(scores).Where((x) => x == 0).Count();
                index = Math.Min(zeros, overlap);
            }
            else {
                // Common case: choose the min score index
                double minScore = 1.0;
                for (int i = 0; i < commonLength; ++i) {
                    if (scores[i] < minScore) {
                        minScore = scores[i];
                        index = i + 1;
                    }
                }
            }
            if (index == 0) {
                collector.Append(nextString);
            }
            else {
                collector.Length = Math.Max(0, collector.Length - 1);
                collector.JAppend(nextString, index - 1, nextString.Length);
            }
        }
    }
//\endcond
}
