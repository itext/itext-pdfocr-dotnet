/*
Copyright (C) 2021-2024, Mindee | Felix Dittrich.

This program is licensed under the Apache License 2.0.
See <https://opensource.org/licenses/Apache-2.0> for full license details.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.Pdfocr;
using iText.Pdfocr.Onnxtr.Actions.Events;
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Orientation;
using iText.Pdfocr.Onnxtr.Recognition;
using iText.Pdfocr.Onnxtr.Util;

namespace iText.Pdfocr.Onnxtr {
//\cond DO_NOT_DOCUMENT
    /// <summary>Class containing OCRing methods adapted from https://github.com/felixdittrich92/OnnxTR.</summary>
    internal class OnnxTrProcessor {
        /// <summary>Image pixel to PDF point ratio.</summary>
        private const float PX_TO_PT = 0.75F;

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

        /// <summary>Text detector.</summary>
        /// <remarks>Text detector. For an input image it outputs a list of text boxes.</remarks>
        private readonly IDetectionPredictor detectionPredictor;

        /// <summary>Text orientation predictor.</summary>
        /// <remarks>
        /// Text orientation predictor. For an input image, which is a tight crop of text, it outputs its orientation
        /// in 90 degrees steps. Can be null.
        /// </remarks>
        private readonly IOrientationPredictor orientationPredictor;

        /// <summary>Text recognizer.</summary>
        /// <remarks>Text recognizer. For an input image, which is a tight crop of text, it outputs the displayed string.
        ///     </remarks>
        private readonly IRecognitionPredictor recognitionPredictor;

//\cond DO_NOT_DOCUMENT
        internal OnnxTrProcessor(IDetectionPredictor detectionPredictor, IOrientationPredictor orientationPredictor, IRecognitionPredictor
             recognitionPredictor) {
            this.detectionPredictor = detectionPredictor;
            this.orientationPredictor = orientationPredictor;
            this.recognitionPredictor = recognitionPredictor;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual IDictionary<int, IList<TextInfo>> DoOcr(IList<System.Drawing.Bitmap> images, OcrProcessContext ocrProcessContext) {
            IDictionary<int, IList<TextInfo>> result = new Dictionary<int, IList<TextInfo>>(images.Count);
            int imageIndex = 0;
            IEnumerator<IList<iText.Kernel.Geom.Point[]>> textBoxGenerator = detectionPredictor.Predict(images);
            while (textBoxGenerator.MoveNext()) {
                AbstractPdfOcrEventHelper eventHelper = ocrProcessContext.GetOcrEventHelper() == null ?
                    new OnnxTrEventHelper() : ocrProcessContext.GetOcrEventHelper();
                // usage event
                PdfOcrOnnxTrProductEvent ocrEvent = PdfOcrOnnxTrProductEvent.CreateProcessImageOnnxTrEvent(
                    eventHelper.GetSequenceId(), null, eventHelper.GetConfirmationType());
                eventHelper.OnEvent(ocrEvent);
                
                /*
                * TODO DEVSIX-9153: Potential performance improvement (at least for GPU).
                *
                * There is a potential for performance improvements here. Currently, this mirrors the
                * behavior in OnnxTR/DocTR, where inputs for orientation and recognition models are
                * aggregated per input image.
                *
                * But, most of the time, this will not be enough to saturate the batch size fully.
                * Ideally, we should process all text boxes together, regardless of the origin image,
                * and then separate the results afterwards.
                */
                System.Drawing.Bitmap image = images[imageIndex];
                IList<iText.Kernel.Geom.Point[]> textBoxes = textBoxGenerator.Current;
                IList<System.Drawing.Bitmap> textImages = BufferedImageUtil.ExtractBoxes(image, textBoxes);
                IList<TextOrientation> textOrientations = null;
                if (orientationPredictor != null) {
                    textOrientations = ToList(orientationPredictor.Predict(textImages));
                    CorrectOrientations(textImages, textOrientations);
                }
                IList<String> textString = RecognizeText(textImages);
                IList<TextInfo> textInfos = new List<TextInfo>(textBoxes.Count);
                for (int i = 0; i < textBoxes.Count; ++i) {
                    TextOrientation textOrientation = TextOrientation.HORIZONTAL;
                    if (textOrientations != null) {
                        textOrientation = textOrientations[i];
                    }
                    textInfos.Add(new TextInfo(textString[i], ToPdfRectangle(textBoxes[i], image.Height), textOrientation));
                }
                result.Put(imageIndex + 1, textInfos);
                ++imageIndex;
                
                // here can be statistics event sending

                // confirm on_demand event
                if (ocrEvent.GetConfirmationType() == EventConfirmationType.ON_DEMAND) {
                    eventHelper.OnEvent(new ConfirmEvent(ocrEvent));
                }
            }
            return result;
        }
//\endcond

        /// <summary>Splits text images to smaller images with better aspect ratios.</summary>
        /// <param name="images">text images to split</param>
        /// <returns>a list with image splits together with a map to restore them back</returns>
        private static OnnxTrProcessor.SplitResult SplitTextImages(IList<System.Drawing.Bitmap> images) {
            OnnxTrProcessor.SplitResult result = new OnnxTrProcessor.SplitResult(images.Count);
            for (int i = 0; i < images.Count; ++i) {
                System.Drawing.Bitmap image = images[i];
                int width = image.Width;
                int height = image.Height;
                float aspectRatio = (float)width / height;
                if (aspectRatio < SPLIT_CROPS_MAX_RATIO) {
                    result.splitImages.Add(image);
                    result.restoreMap[i] = 1;
                    continue;
                }
                // For some reason here is truncation in OnnxTR...
                int splitCount = (int)Math.Ceiling(aspectRatio / SPLIT_CROPS_TARGET_RATIO);
                float rawSplitWidth = (float)width / splitCount;
                float targetSplitHalfWidth = (SPLIT_CROPS_DILATION_FACTOR * rawSplitWidth) / 2;
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
                    result.splitImages.Add(image.Clone(new System.Drawing.Rectangle(minX, 0, currentSplitWidth, height), image.PixelFormat));
                }
                result.restoreMap[i] = nonEmptySplitCount;
            }
            return result;
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

        /// <summary>Runs text recognition on the provided text images.</summary>
        /// <param name="textImages">images with text to recognize</param>
        /// <returns>list of strings, recognized in the images</returns>
        private IList<String> RecognizeText(IList<System.Drawing.Bitmap> textImages) {
            // For better recognition results we want to split text images to have better aspect ratios
            OnnxTrProcessor.SplitResult split = SplitTextImages(textImages);
            IEnumerator<String> recognitionIterator = recognitionPredictor.Predict(split.splitImages);
            // And now we merge results back
            IList<String> textStrings = new List<String>(split.restoreMap.Length);
            for (int j = 0; j < split.restoreMap.Length; ++j) {
                int stringPartsLeft = split.restoreMap[j];
                String testString;
                if (stringPartsLeft == 1 && recognitionIterator.MoveNext()) {
                    testString = recognitionIterator.Current;
                }
                else {
                    StringBuilder sb = new StringBuilder();
                    while (stringPartsLeft > 0 && recognitionIterator.MoveNext()) {
                        MergeStrings(sb, recognitionIterator.Current);
                        --stringPartsLeft;
                    }
                    testString = sb.ToString();
                }
                textStrings.Add(testString);
            }
            return textStrings;
        }

        /// <summary>
        /// Rotates all images in the text image list, so that they are upright, based on the found text
        /// orientation information.
        /// </summary>
        /// <param name="textImages">text images to rotate</param>
        /// <param name="textOrientations">orientations of text images. Should be the same size as textImages</param>
        private static void CorrectOrientations(IList<System.Drawing.Bitmap> textImages, IList<TextOrientation> textOrientations
            ) {
            System.Diagnostics.Debug.Assert(textImages.Count == textOrientations.Count);
            for (int i = 0; i < textImages.Count; ++i) {
                textImages[i] = BufferedImageUtil.Rotate(textImages[i], textOrientations[i]);
            }
        }

        /// <summary>Convert a text polygon to a bounding box in PDF points.</summary>
        /// <param name="polygon">polygon to convert</param>
        /// <param name="imageHeight">height of the image (to change the y origin)</param>
        /// <returns>a bounding box in PDF points</returns>
        private static Rectangle ToPdfRectangle(iText.Kernel.Geom.Point[] polygon, int imageHeight) {
            float minX = (float)polygon[0].GetX();
            float maxX = minX;
            float minY = (float)polygon[0].GetY();
            float maxY = minY;
            for (int i = 1; i < polygon.Length; ++i) {
                float x = (float)polygon[i].GetX();
                if (x < minX) {
                    minX = x;
                }
                else {
                    if (x > maxX) {
                        maxX = x;
                    }
                }
                float y = (float)polygon[i].GetY();
                if (y < minY) {
                    minY = y;
                }
                else {
                    if (y > maxY) {
                        maxY = y;
                    }
                }
            }
            return new Rectangle(PX_TO_PT * minX, PX_TO_PT * (imageHeight - maxY), PX_TO_PT * (maxX - minX), PX_TO_PT 
                * (maxY - minY));
        }

        private static IList<E> ToList<E>(IEnumerator<E> iterator) {
            var list = new List<E>();
            while (iterator.MoveNext()) {
                list.Add(iterator.Current);
            }
            return list;
        }

        /// <summary>Contains results of a text image split.</summary>
        public class SplitResult {
            /// <summary>List of sub-images, that the original images were split into.</summary>
            public readonly IList<System.Drawing.Bitmap> splitImages;

            /// <summary>A map of splits.</summary>
            /// <remarks>
            /// A map of splits. Array length is equal to the original image count. Each element defines
            /// how many sub-images were generated from each original image.
            /// </remarks>
            public readonly int[] restoreMap;

            /// <summary>
            /// Creates new
            /// <see cref="SplitResult"/>
            /// instance.
            /// </summary>
            /// <param name="capacity">capacity of the list of sub-images</param>
            public SplitResult(int capacity) {
                this.splitImages = new List<System.Drawing.Bitmap>(capacity);
                this.restoreMap = new int[capacity];
            }
        }
    }
//\endcond
}
