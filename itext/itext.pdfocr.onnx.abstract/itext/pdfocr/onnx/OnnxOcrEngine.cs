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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions.Data;
using iText.Commons.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Logs;
using iText.Pdfocr.Onnx.Detection;
using iText.Pdfocr.Onnx.Exceptions;
using iText.Pdfocr.Onnx.Orientation;
using iText.Pdfocr.Onnx.Recognition;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnx {
    /// <summary>
    /// <see cref="iText.Pdfocr.IOcrEngine"/>
    /// implementation, based on OnnxTR/DocTR machine learning OCR projects.
    /// </summary>
    /// <remarks>
    /// <see cref="iText.Pdfocr.IOcrEngine"/>
    /// implementation, based on OnnxTR/DocTR machine learning OCR projects.
    /// <para />
    /// NOTE:
    /// <see cref="OnnxOcrEngine"/>
    /// instance shall be closed after all usages to avoid native allocations leak.
    /// </remarks>
    public class OnnxOcrEngine : IOcrEngine, IDisposable, IProductAware {
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
        private readonly OnnxEngineProperties properties;

        /// <summary>The logger.</summary>
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Pdfocr.Onnx.OnnxOcrEngine)
            );

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
        public OnnxOcrEngine(IDetectionPredictor detectionPredictor, IOrientationPredictor orientationPredictor, IRecognitionPredictor
             recognitionPredictor)
            : this(detectionPredictor, orientationPredictor, recognitionPredictor, new OnnxEngineProperties()) {
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
        public OnnxOcrEngine(IDetectionPredictor detectionPredictor, IOrientationPredictor orientationPredictor, IRecognitionPredictor
             recognitionPredictor, OnnxEngineProperties properties) {
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
        public OnnxOcrEngine(IDetectionPredictor detectionPredictor, IRecognitionPredictor recognitionPredictor)
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
            return DoImageOcr(input, new OcrProcessContext(new OnnxEventHelper()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input, OcrProcessContext ocrProcessContext
            ) {
            return DoImageOcr(JavaCollectionsUtil.SingletonList(input), ocrProcessContext);
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(IList<FileInfo> inputs) {
            return DoImageOcr(inputs, new OcrProcessContext(new OnnxEventHelper()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(IList<FileInfo> inputs, OcrProcessContext ocrProcessContext
            ) {
            return DoImageOcrInternal(PdfOcrFileUtil.ConvertToInputStreams(inputs), ocrProcessContext);
        }

        /// <summary><inheritDoc/></summary>
        public virtual void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile) {
            CreateTxtFile(inputImages, txtFile, new OcrProcessContext(new OnnxEventHelper()));
        }

        /// <summary><inheritDoc/></summary>
        public virtual void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile, OcrProcessContext ocrProcessContext
            ) {
            CreateTxtFileInternal(PdfOcrFileUtil.ConvertToInputStreams(inputImages), PdfOcrFileUtil.ConvertToOutputStream
                (txtFile), ocrProcessContext);
        }

        /// <summary><inheritDoc/></summary>
        public virtual bool IsTaggingSupported() {
            return false;
        }

        /// <summary><inheritDoc/></summary>
        public virtual PdfOcrMetaInfoContainer GetMetaInfoContainer() {
            return new PdfOcrMetaInfoContainer(new OnnxMetaInfo());
        }

        /// <summary><inheritDoc/></summary>
        public virtual ProductData GetProductData() {
            return null;
        }

        /// <summary>
        /// Reads data from the provided input image stream and returns retrieved data
        /// in the format described below.
        /// </summary>
        /// <param name="input">
        /// input stream
        /// <see cref="System.IO.Stream"/>
        /// </param>
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
        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(Stream input) {
            return DoImageOcr(input, new OcrProcessContext(new OnnxEventHelper()));
        }

        /// <summary>
        /// Reads data from the provided input image stream and returns retrieved data
        /// in the format described below.
        /// </summary>
        /// <param name="input">
        /// input image
        /// <see cref="System.IO.Stream"/>
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
        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(Stream input, OcrProcessContext ocrProcessContext
            ) {
            return DoImageOcrInternal(JavaCollectionsUtil.SingletonList(input), ocrProcessContext);
        }

        /// <summary>
        /// Performs OCR using provided
        /// <see cref="iText.Pdfocr.IOcrEngine"/>
        /// for the given
        /// input image and saves result into provided
        /// <see cref="System.IO.FileStream"/>
        /// with UTF-8 encoding.
        /// </summary>
        /// <remarks>
        /// Performs OCR using provided
        /// <see cref="iText.Pdfocr.IOcrEngine"/>
        /// for the given
        /// input image and saves result into provided
        /// <see cref="System.IO.FileStream"/>
        /// with UTF-8 encoding.
        /// Note that a human reading order is not guaranteed
        /// due to possible specifics of input images (multi column layout, tables etc)
        /// </remarks>
        /// <param name="inputImage">
        /// image
        /// <see cref="System.IO.Stream"/>
        /// </param>
        /// <param name="outputStream">
        /// output stream
        /// <see cref="System.IO.FileStream"/>
        /// </param>
        public virtual void CreateTxtFile(Stream inputImage, FileStream outputStream) {
            CreateTxtFileInternal(JavaCollectionsUtil.SingletonList(inputImage), outputStream, new OcrProcessContext(new 
                OnnxEventHelper()));
        }

        private IDictionary<int, IList<TextInfo>> DoImageOcrInternal(IList<Stream> inputs, OcrProcessContext ocrProcessContext
            ) {
            IDictionary<int, IList<TextInfo>> result = DoOnnxOcr(inputs, ocrProcessContext);
            if (iText.Pdfocr.Onnx.Text.TextPositioning.BY_WORDS.Equals(properties.GetTextPositioning())) {
                PdfOcrTextBuilder.SortTextInfosByLines(result);
            }
            else {
                if (iText.Pdfocr.Onnx.Text.TextPositioning.BY_LINES.Equals(properties.GetTextPositioning())) {
                    PdfOcrTextBuilder.CollectWordsIntoLines(result);
                }
                else {
                    // Use TextPositioning.BY_WORDS_AND_LINES by default.
                    PdfOcrTextBuilder.GenerifyWordBBoxesByLine(result);
                }
            }
            return result;
        }

        private void CreateTxtFileInternal(IList<Stream> inputImages, Stream outputStream, OcrProcessContext ocrProcessContext
            ) {
            LOGGER.LogInformation(MessageFormatUtil.Format(PdfOcrLogMessageConstant.START_OCR_FOR_IMAGES, inputImages.
                Count));
            AbstractPdfOcrEventHelper storedEventHelper;
            if (ocrProcessContext.GetOcrEventHelper() == null) {
                storedEventHelper = new OnnxEventHelper();
            }
            else {
                storedEventHelper = ocrProcessContext.GetOcrEventHelper();
            }
            try {
                // save confirm events from doImageOcr, to send them only after successful writing to the file
                OnnxFileResultEventHelper fileResultEventHelper = new OnnxFileResultEventHelper(storedEventHelper);
                ocrProcessContext.SetOcrEventHelper(fileResultEventHelper);
                IDictionary<int, IList<TextInfo>> outputMap = DoOnnxOcr(inputImages, ocrProcessContext);
                String content = PdfOcrTextBuilder.BuildText(outputMap);
                PdfOcrFileUtil.WriteToStream(outputStream, content);
                fileResultEventHelper.RegisterAllSavedEvents();
            }
            finally {
                ocrProcessContext.SetOcrEventHelper(storedEventHelper);
            }
        }

//\cond DO_NOT_DOCUMENT
        internal static IList<IronSoftware.Drawing.AnyBitmap> GetImages(MemoryStream input) {
            try {
                if (TiffImageUtil.IsTiffImage(input)) {
                    IList<IronSoftware.Drawing.AnyBitmap> images = TiffImageUtil.GetAllImages(input);
                    if (images.IsEmpty()) {
                        throw new PdfOcrInputException(PdfOcrOnnxExceptionMessageConstant.FAILED_TO_READ_IMAGE);
                    }
                    return images;
                }
                else {
                    IronSoftware.Drawing.AnyBitmap image = IronSoftware.Drawing.AnyBitmap.FromStream(input);
                    if (image == null) {
                        throw new PdfOcrInputException(PdfOcrOnnxExceptionMessageConstant.FAILED_TO_READ_IMAGE);
                    }
                    return JavaCollectionsUtil.SingletonList(image);
                }
            }
            catch (Exception e) {
                throw new PdfOcrInputException(PdfOcrOnnxExceptionMessageConstant.FAILED_TO_READ_IMAGE, e);
            }
        }
//\endcond

        /// <summary>
        /// Reads raw data from the provided input image files and returns retrieved data
        /// in the format described below.
        /// </summary>
        /// <param name="inputStreams">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of input image files
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
        private IDictionary<int, IList<TextInfo>> DoOnnxOcr(IList<Stream> inputStreams, OcrProcessContext ocrProcessContext
            ) {
            IList<IronSoftware.Drawing.AnyBitmap> images = new List<IronSoftware.Drawing.AnyBitmap>();
            foreach (Stream stream in inputStreams) {
                images.AddAll(GetImages(ByteArrayStreamUtil.CreateByteArrayInputStream(stream)));
            }
            OnnxProcessor onnxProcessor = new OnnxProcessor(detectionPredictor, orientationPredictor, recognitionPredictor
                );
            return onnxProcessor.DoOcr(images, ocrProcessContext);
        }

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}
