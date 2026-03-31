/*
Copyright (C) 2021-2024, Mindee | Felix Dittrich.

This program is licensed under the Apache License 2.0.
See <https://opensource.org/licenses/Apache-2.0> for full license details.
*/
using System;
using System.Collections.Generic;
using iText.Commons.Actions.Confirmations;
using iText.Pdfocr;
using iText.Pdfocr.Onnx.Actions.Events;
using iText.Pdfocr.Onnx.Detection;
using iText.Pdfocr.Onnx.Orientation;
using iText.Pdfocr.Onnx.Recognition;
using iText.Pdfocr.Onnx.Util;

namespace iText.Pdfocr.Onnx {
//\cond DO_NOT_DOCUMENT
    /// <summary>Class containing OCRing methods adapted from <a href="https://github.com/felixdittrich92/onnxtr">OnnxTR</a>.
    ///     </summary>
    internal class OnnxProcessor {
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
        internal OnnxProcessor(IDetectionPredictor detectionPredictor, IOrientationPredictor orientationPredictor, 
            IRecognitionPredictor recognitionPredictor) {
            this.detectionPredictor = detectionPredictor;
            this.orientationPredictor = orientationPredictor;
            this.recognitionPredictor = recognitionPredictor;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual IDictionary<int, IList<TextInfo>> DoOcr(IList<IronSoftware.Drawing.AnyBitmap> images, OcrProcessContext
             ocrProcessContext) {
            IDictionary<int, IList<TextInfo>> result = new Dictionary<int, IList<TextInfo>>(images.Count);
            int imageIndex = 0;
            IEnumerator<IList<iText.Kernel.Geom.Point[]>> textBoxGenerator = detectionPredictor.Predict(images);
            while (textBoxGenerator.MoveNext()) {
                AbstractPdfOcrEventHelper eventHelper = ocrProcessContext.GetOcrEventHelper() == null ? new OnnxEventHelper
                    () : ocrProcessContext.GetOcrEventHelper();
                // Usage event.
                PdfOcrOnnxProductEvent @event = PdfOcrOnnxProductEvent.CreateProcessImageOnnxEvent(eventHelper.GetSequenceId
                    (), null, eventHelper.GetConfirmationType());
                eventHelper.OnEvent(@event);
                /*
                * Currently, inputs for orientation and recognition models are aggregated per input image.
                * Most of the time, this is enough to saturate the batch size fully for real use cases
                * (for example, 64 words for DocTR or 6 lines for PaddleOcr).
                * If we process all text boxes together, regardless of the origin image, and then separate
                * the results afterward, the performance improvement is not noticeable.
                */
                IronSoftware.Drawing.AnyBitmap image = images[imageIndex];
                IList<iText.Kernel.Geom.Point[]> textBoxes = textBoxGenerator.Current;
                IList<IronSoftware.Drawing.AnyBitmap> textImages = BufferedImageUtil.ExtractBoxes(image, textBoxes);
                IList<TextOrientation> textOrientations = null;
                if (orientationPredictor != null) {
                    textOrientations = ToList(orientationPredictor.Predict(textImages));
                    CorrectOrientations(textImages, textOrientations);
                }
                IList<String> textString = ToList(recognitionPredictor.Predict(textImages));
                IList<TextInfo> textInfos = new List<TextInfo>(textBoxes.Count);
                int imageHeight = BufferedImageUtil.GetHeight(image);
                for (int i = 0; i < textBoxes.Count; ++i) {
                    TextOrientation textOrientation = TextOrientation.HORIZONTAL;
                    if (textOrientations != null) {
                        textOrientation = textOrientations[i];
                    }
                    iText.Kernel.Geom.Point[] textPoints = GetTextPoints(textBoxes[i], textOrientation);
                    textInfos.Add(new TextInfo().SetText(textString[i]).SetPixelTextPoints(textPoints, imageHeight));
                }
                result.Put(imageIndex + 1, textInfos);
                ++imageIndex;
                // Here can be statistics event sending.
                // Confirm on_demand event.
                if (@event.GetConfirmationType() == EventConfirmationType.ON_DEMAND) {
                    eventHelper.OnEvent(new ConfirmEvent(@event));
                }
            }
            return result;
        }
//\endcond

        /// <summary>
        /// Rotates all images in the text image list, so that they are upright, based on the found text
        /// orientation information.
        /// </summary>
        /// <param name="textImages">text images to rotate</param>
        /// <param name="textOrientations">orientations of text images. Should be the same size as textImages</param>
        private static void CorrectOrientations(IList<IronSoftware.Drawing.AnyBitmap> textImages, IList<TextOrientation
            > textOrientations) {
            System.Diagnostics.Debug.Assert(textImages.Count == textOrientations.Count);
            for (int i = 0; i < textImages.Count; ++i) {
                textImages[i] = BufferedImageUtil.Rotate(textImages[i], textOrientations[i]);
            }
        }

        /// <summary>Reorders textBox points to be in lower-left based order relative to text.</summary>
        /// <param name="textBox">
        /// arbitrarily rotated quadrilateral representing text bounding points with the next order
        /// relative to x and y axes directions: 0 - lower-left, 1 - upper-left, 2 - upper-right, 3 - lower-right point
        /// </param>
        /// <param name="textOrientation">
        /// 
        /// <see cref="iText.Pdfocr.TextOrientation"/>
        /// to determine same points order, but relative to text itself. So
        /// for example for 90 degrees rotated text initial lower-left point will be upper-left relative to text
        /// </param>
        /// <returns>
        /// array of 4
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// s describing text bbox (0 - lower-left, 1 - upper-left,
        /// 2 - upper-right, 3 - lower-right point relative to text)
        /// </returns>
        private static iText.Kernel.Geom.Point[] GetTextPoints(iText.Kernel.Geom.Point[] textBox, TextOrientation 
            textOrientation) {
            iText.Kernel.Geom.Point[] rotatedTextBox;
            switch (textOrientation) {
                case TextOrientation.HORIZONTAL_ROTATED_90: {
                    rotatedTextBox = new iText.Kernel.Geom.Point[] { textBox[3], textBox[0], textBox[1], textBox[2] };
                    break;
                }

                case TextOrientation.HORIZONTAL_ROTATED_180: {
                    rotatedTextBox = new iText.Kernel.Geom.Point[] { textBox[2], textBox[3], textBox[0], textBox[1] };
                    break;
                }

                case TextOrientation.HORIZONTAL_ROTATED_270: {
                    rotatedTextBox = new iText.Kernel.Geom.Point[] { textBox[1], textBox[2], textBox[3], textBox[0] };
                    break;
                }

                case TextOrientation.HORIZONTAL:
                default: {
                    rotatedTextBox = textBox;
                    break;
                }
            }
            return rotatedTextBox;
        }

        private static IList<E> ToList<E>(IEnumerator<E> iterator) {
            IList<E> list = new List<E>();
            iterator.ForEachRemaining(list);
            return list;
        }
    }
//\endcond
}
