/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions.Data;
using iText.Commons.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Logs;
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Pdfocr.Onnxtr.Orientation;
using iText.Pdfocr.Onnxtr.Recognition;
using iText.Pdfocr.Util;

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
    public class OnnxTrOcrEngine : IOcrEngine, IDisposable, IProductAware {
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

        /// <summary>Set of properties.</summary>
        private readonly OnnxTrEngineProperties properties;

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
            IRecognitionPredictor recognitionPredictor)
            : this(detectionPredictor, orientationPredictor, recognitionPredictor, new OnnxTrEngineProperties()) {
        }

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
        /// <param name="properties">set of properties</param>
        public OnnxTrOcrEngine(IDetectionPredictor detectionPredictor, IOrientationPredictor orientationPredictor, 
            IRecognitionPredictor recognitionPredictor, OnnxTrEngineProperties properties) {
            this.detectionPredictor = Objects.RequireNonNull(detectionPredictor);
            this.orientationPredictor = orientationPredictor;
            this.recognitionPredictor = Objects.RequireNonNull(recognitionPredictor);
            this.properties = properties;
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

        /// <summary><inheritDoc/></summary>
        public virtual void Close() {
            detectionPredictor.Close();
            if (orientationPredictor != null) {
                orientationPredictor.Close();
            }
            recognitionPredictor.Close();
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input) {
            return DoImageOcr(input, new OcrProcessContext(new OnnxTrEventHelper()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input, OcrProcessContext ocrProcessContext
            ) {
            IDictionary<int, IList<TextInfo>> result = DoOnnxTrOcr(input, ocrProcessContext);
            if (TextPositioning.BY_WORDS.Equals(properties.GetTextPositioning())) {
                PdfOcrTextBuilder.SortTextInfosByLines(result);
            }
            else {
                PdfOcrTextBuilder.GenerifyWordBBoxesByLine(result);
            }
            return result;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile) {
            CreateTxtFile(inputImages, txtFile, new OcrProcessContext(new OnnxTrEventHelper()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile, OcrProcessContext ocrProcessContext
            ) {
            ITextLogManager.GetLogger(GetType()).LogInformation(MessageFormatUtil.Format(PdfOcrLogMessageConstant.START_OCR_FOR_IMAGES
                , inputImages.Count));
            AbstractPdfOcrEventHelper storedEventHelper;
            if (ocrProcessContext.GetOcrEventHelper() == null) {
                storedEventHelper = new OnnxTrEventHelper();
            }
            else {
                storedEventHelper = ocrProcessContext.GetOcrEventHelper();
            }
            try {
                // save confirm events from doImageOcr, to send them only after successful writing to the file
                OnnxTrFileResultEventHelper fileResultEventHelper = new OnnxTrFileResultEventHelper(storedEventHelper);
                ocrProcessContext.SetOcrEventHelper(fileResultEventHelper);
                StringBuilder content = new StringBuilder();
                foreach (FileInfo inputImage in inputImages) {
                    IDictionary<int, IList<TextInfo>> outputMap = DoOnnxTrOcr(inputImage, ocrProcessContext);
                    content.Append(PdfOcrTextBuilder.BuildText(outputMap));
                }
                PdfOcrFileUtil.WriteToTextFile(txtFile.FullName, content.ToString());
                fileResultEventHelper.RegisterAllSavedEvents();
            }
            finally {
                ocrProcessContext.SetOcrEventHelper(storedEventHelper);
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool IsTaggingSupported() {
            return false;
        }

        /// <summary><inheritDoc/></summary>
        public virtual PdfOcrMetaInfoContainer GetMetaInfoContainer() {
            return new PdfOcrMetaInfoContainer(new OnnxTrMetaInfo());
        }

        /// <summary><inheritDoc/></summary>
        public virtual ProductData GetProductData() {
            return null;
        }

//\cond DO_NOT_DOCUMENT
        internal static IList<IronSoftware.Drawing.AnyBitmap> GetImages(FileInfo input) {
            try {
                if (TiffImageUtil.IsTiffImage(input)) {
                    IList<IronSoftware.Drawing.AnyBitmap> images = TiffImageUtil.GetAllImages(input);
                    if (images.IsEmpty()) {
                        throw new PdfOcrInputException(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_READ_IMAGE);
                    }
                    return images;
                }
                else {
                    IronSoftware.Drawing.AnyBitmap image = IronSoftware.Drawing.AnyBitmap.FromFile(input.FullName);
                    if (image == null) {
                        throw new PdfOcrInputException(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_READ_IMAGE);
                    }
                    return JavaCollectionsUtil.SingletonList(image);
                }
            }
            catch (Exception e) {
                throw new PdfOcrInputException(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_READ_IMAGE, e);
            }
        }
//\endcond

        /// <summary>
        /// Reads raw data from the provided input image file and returns retrieved data
        /// in the format described below.
        /// </summary>
        /// <param name="input">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="ocrProcessContext">ocr processing context</param>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// where key is
        /// <see cref="int?"/>
        /// representing the number of the page and value is
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// elements where each
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// element contains a word or a line and its 4
        /// coordinates(bbox)
        /// </returns>
        private IDictionary<int, IList<TextInfo>> DoOnnxTrOcr(FileInfo input, OcrProcessContext ocrProcessContext) {
            IList<IronSoftware.Drawing.AnyBitmap> images = GetImages(input);
            OnnxTrProcessor onnxTrProcessor = new OnnxTrProcessor(detectionPredictor, orientationPredictor, recognitionPredictor
                );
            return onnxTrProcessor.DoOcr(images, ocrProcessContext);
        }

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}
