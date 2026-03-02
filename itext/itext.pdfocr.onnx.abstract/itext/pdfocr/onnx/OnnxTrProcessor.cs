/*
Copyright (C) 2021-2024, Mindee | Felix Dittrich.

This program is licensed under the Apache License 2.0.
See <https://opensource.org/licenses/Apache-2.0> for full license details.
*/
using System;
using System.Collections.Generic;
using iText.Commons.Actions.Confirmations;
using iText.Kernel.Geom;
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
    internal class OnnxTrProcessor {
        /// <summary>Image pixel to PDF point ratio.</summary>
        private const float PX_TO_PT = 0.75F;

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
        internal OnnxTrProcessor(IDetectionPredictor detectionPredictor, IOrientationPredictor orientationPredictor
            , IRecognitionPredictor recognitionPredictor) {
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
                AbstractPdfOcrEventHelper eventHelper = ocrProcessContext.GetOcrEventHelper() == null ? new OnnxTrEventHelper
                    () : ocrProcessContext.GetOcrEventHelper();
                // Usage event.
                PdfOcrOnnxTrProductEvent @event = PdfOcrOnnxTrProductEvent.CreateProcessImageOnnxTrEvent(eventHelper.GetSequenceId
                    (), null, eventHelper.GetConfirmationType());
                eventHelper.OnEvent(@event);
                /*
                * Potential performance improvement (at least for GPU).
                *
                * There is a potential for performance improvements here. Currently, this mirrors the
                * behavior in OnnxTR/DocTR, where inputs for orientation and recognition models are
                * aggregated per input image.
                *
                * But, most of the time, this will not be enough to saturate the batch size fully.
                * Ideally, we should process all text boxes together, regardless of the origin image,
                * and then separate the results afterward.
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
                for (int i = 0; i < textBoxes.Count; ++i) {
                    TextOrientation textOrientation = TextOrientation.HORIZONTAL;
                    if (textOrientations != null) {
                        textOrientation = textOrientations[i];
                    }
                    textInfos.Add(new TextInfo(textString[i], ToPdfRectangle(textBoxes[i], BufferedImageUtil.GetHeight(image))
                        , textOrientation));
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
            IList<E> list = new List<E>();
            iterator.ForEachRemaining(list);
            return list;
        }
    }
//\endcond
}
