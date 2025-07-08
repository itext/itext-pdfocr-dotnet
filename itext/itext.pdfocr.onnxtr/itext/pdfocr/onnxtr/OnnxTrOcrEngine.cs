using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iText.Commons.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Orientation;
using iText.Pdfocr.Onnxtr.Recognition;
using iText.Pdfocr.Onnxtr.Util;
using iText.Pdfocr.Util;
using Point = iText.Kernel.Geom.Point;
using Rectangle = iText.Kernel.Geom.Rectangle;
using iText.Pdfocr.Util;
using iText.Pdfocr.Onnxtr.Exceptions;

namespace iText.Pdfocr.Onnxtr {
    /// <summary>
    /// <see cref="iText.Pdfocr.IOcrEngine"/>
    /// implementation, based on OnnxTR/DocTR machine learning OCR projects.
    /// </summary>
    /// <remarks>
    /// <see cref="iText.Pdfocr.IOcrEngine"/>
    /// implementation, based on OnnxTR/DocTR machine learning OCR projects.
    /// <para />
    /// NOTE:
    /// <see cref="OnnxTrOcrEngine"/>
    /// instance shall be closed after all usages to avoid native allocations leak.
    /// </remarks>
    public class OnnxTrOcrEngine : IOcrEngine, IDisposable {
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

        /// <summary>Create a new OCR engine with the provided predictors.</summary>
        /// <param name="detectionPredictor">text detector. For an input image it outputs a list of text boxes</param>
        /// <param name="orientationPredictor">
        /// text orientation predictor. For an input image, which is a tight  crop of text,
        /// it outputs its orientation in 90 degrees steps. Can be null, in that case all text
        /// is assumed to be upright
        /// </param>
        /// <param name="recognitionPredictor">
        /// text recognizer. For an input image, which is a tight crop of text, it outputs the
        /// displayed string
        /// </param>
        public OnnxTrOcrEngine(IDetectionPredictor detectionPredictor, IOrientationPredictor orientationPredictor, 
            IRecognitionPredictor recognitionPredictor) {
            this.detectionPredictor = detectionPredictor;
            this.orientationPredictor = orientationPredictor;
            this.recognitionPredictor = recognitionPredictor;
        }

        /// <summary>Create a new OCR engine with the provided predictors, without text orientation prediction.</summary>
        /// <param name="detectionPredictor">text detector. For an input image it outputs a list of text boxes</param>
        /// <param name="recognitionPredictor">
        /// text recognizer. For an input image, which is a tight crop of text,
        /// it outputs the displayed string
        /// </param>
        public OnnxTrOcrEngine(IDetectionPredictor detectionPredictor, IRecognitionPredictor recognitionPredictor)
            : this(detectionPredictor, null, recognitionPredictor) {
        }

        public virtual void Close() {
            detectionPredictor.Dispose();
            if (orientationPredictor != null) {
                orientationPredictor.Dispose();
            }
            recognitionPredictor.Dispose();
        }

        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input) {
            return DoImageOcr(input, null);
        }

        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input, OcrProcessContext ocrProcessContext
            ) {
            IList<System.Drawing.Bitmap> images = GetImages(input);
            IDictionary<int, IList<TextInfo>> result = new Dictionary<int, IList<TextInfo>>(images.Count);
            int imageIndex = 0;
            IEnumerator<IList<Point[]>> textBoxGenerator = detectionPredictor.Predict(images);
            while (textBoxGenerator.MoveNext()) {
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
                IList<Point[]> textBoxes = textBoxGenerator.Current;
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
                    textInfos.Add(new TextInfo(textString[i], /* * FIXME DEVSIX-9154: Why not return rectangles in image pixels?.. 
                        * * Seems odd, that an OCR engine should be concerned by PDF specific. It * would make sense for an engine to return results, which could be directly 
                        * applied to images inputs instead... */ ToPdfRectangle(textBoxes[i], image.Height), textOrientation
                        ));
                }
                result.Put(imageIndex + 1, textInfos);
                ++imageIndex;
            }
            return result;
        }

        public virtual void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile) {
            CreateTxtFile(inputImages, txtFile, null);
        }

        public virtual void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile, OcrProcessContext ocrProcessContext
            ) {
            StringBuilder content = new StringBuilder();
            foreach (FileInfo inputImage in inputImages) {
                IDictionary<int, IList<TextInfo>> outputMap = DoImageOcr(inputImage, ocrProcessContext);
                content.Append(PdfOcrTextBuilder.BuildText(outputMap));
            }
            WriteToTextFile(txtFile.FullName, content.ToString());
        }

        public virtual bool IsTaggingSupported() {
            return false;
        }

        /// <summary>
        /// Writes provided
        /// <see cref="System.String"/>
        /// to text file using provided path.
        /// </summary>
        /// <param name="path">
        /// path as
        /// <see cref="System.String"/>
        /// to file to be created
        /// </param>
        /// <param name="data">
        /// text data in required format as
        /// <see cref="System.String"/>
        /// </param>
        private static void WriteToTextFile(String path, String data) {
            try {
                using (TextWriter writer = new StreamWriter(FileUtil.GetFileOutputStream(path), System.Text.Encoding.UTF8)
                    ) {
                    writer.Write(data);
                }
            }
            catch (System.IO.IOException e) {
                throw new PdfOcrException(MessageFormatUtil.Format(PdfOcrExceptionMessageConstant.CANNOT_WRITE_TO_FILE, path
                    , e.Message), e);
            }
        }

        /// <summary>Runs text recognition on the provided text images.</summary>
        /// <param name="textImages">images with text to recognize</param>
        /// <returns>list of strings, recognized in the images</returns>
        private IList<String> RecognizeText(IList<System.Drawing.Bitmap> textImages) {
            // For better recognition results we want to split text images to have better aspect ratios
            OnnxTrOcrEngine.SplitResult split = SplitTextImages(textImages);
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

        /// <summary>Splits text images to smaller images with better aspect ratios.</summary>
        /// <param name="images">text images to split</param>
        /// <returns>a list with image splits together with a map to restore them back</returns>
        private static OnnxTrOcrEngine.SplitResult SplitTextImages(IList<System.Drawing.Bitmap> images) {
            OnnxTrOcrEngine.SplitResult result = new OnnxTrOcrEngine.SplitResult(images.Count);
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

        private static int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
                return string.IsNullOrEmpty(t) ? 0 : t.Length;
            if (string.IsNullOrEmpty(t))
                return s.Length;

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; i++)
                d[i, 0] = i;
            for (int j = 0; j <= m; j++)
                d[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = s[i - 1] == t[j - 1] ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1,
                            d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
        
        /// <summary>Merges strings, collected from splits of text images.</summary>
        /// <remarks>
        /// Merges strings, collected from splits of text images.
        /// <para />
        /// TODO DEVSIX-9153 This code is pretty much 1-to-1 to what is in OnnxTR. Logic is not that trivial.
        /// </remarks>
        /// <param name="collector">string builder collector, which contains the current left part of the string</param>
        /// <param name="nextString">next string to add to the collector</param>
        private static void MergeStrings(StringBuilder collector, String nextString) {
            // Comments are also pretty much copies from OnnxTR...
            int commonLength = Math.Min(collector.Length, nextString.Length);
            double[] scores = new double[commonLength];
            for (int i = 0; i < commonLength; ++i) {
                // TODO DEVSIX-9153: org.apache.commons.commons-text is used only for this, but
                //        since Levenshtein distance is relatively trivial, might be
                //        better to just reimplement it
                scores[i] = LevenshteinDistance(collector.ToString().Substring(collector.Length - i - 1), 
                    nextString.Substring(0, i + 1)) / (i + 1.0);
            }
            int index = 0;
            // Comparing floats to 0 is fine here, as it only happens, when the
            // integer nominator (i.e. Levenshtein distance) was 0
            if (commonLength > 1 && scores[0] == 0 && scores[1] == 0) {
                // Edge case (split in the middle of char repetitions): if it starts with 2 or more 0
                // Compute n_overlap (number of overlapping chars, geometrically determined)
                int overlap = (int)MathematicUtil.Round(nextString.Length * (iText.Pdfocr.Onnxtr.OnnxTrOcrEngine.SPLIT_CROPS_DILATION_FACTOR
                                                                             - 1) / iText.Pdfocr.Onnxtr.OnnxTrOcrEngine.SPLIT_CROPS_DILATION_FACTOR);
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

        /// <summary>Convert a text polygon to a bounding box in PDF points.</summary>
        /// <param name="polygon">polygon to convert</param>
        /// <param name="imageHeight">height of the image (to change the y origin)</param>
        /// <returns>a bounding box in PDF points</returns>
        private static Rectangle ToPdfRectangle(Point[] polygon, int imageHeight) {
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

        private static IList<System.Drawing.Bitmap> GetImages(FileInfo input) {
            try {
                if (TiffImageUtil.IsTiffImage(input)) {
                    IList<System.Drawing.Bitmap> images = TiffImageUtil.GetAllImages(input);
                    if (images.Count == 0) {
                        throw new PdfOcrInputException(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_READ_IMAGE);
                    }
                    return images;
                }
                else {
                    System.Drawing.Bitmap image = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(input.FullName);
                    if (image == null) {
                        throw new PdfOcrInputException(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_READ_IMAGE);
                    }
                    return JavaCollectionsUtil.SingletonList(image);
                }
            }
            catch (System.Exception e) {
                throw new PdfOcrInputException(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_READ_IMAGE, e);
            }
        }

        private static IList<E> ToList<E>(IEnumerator<E> iterator) {
            var list = new List<E>();
            while (iterator.MoveNext()) {
                list.Add(iterator.Current);
            }
            return list;
        }

        /// <summary>Contains results of a text image split.</summary>
        private class SplitResult {
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

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}
