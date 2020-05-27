using System;
using System.Collections.Generic;
using System.IO;
using Common.Logging;
using iText.IO.Font;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Pdfa;

namespace iText.Pdfocr {
    /// <summary>
    /// <see cref="OcrPdfCreator"/>
    /// is the class that creates Pdf documents containing input
    /// images and text that was recognized using provided
    /// <see cref="IOcrEngine"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="OcrPdfCreator"/>
    /// is the class that creates Pdf documents containing input
    /// images and text that was recognized using provided
    /// <see cref="IOcrEngine"/>.
    /// <see cref="OcrPdfCreator"/>
    /// provides possibilities to set list of input images to
    /// be used for OCR, to set scaling mode for images, to set color of text in
    /// output PDF document, to set fixed size of the PDF document's page and to
    /// perform OCR using given images and to return
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// as result.
    /// OCR is based on the provided
    /// <see cref="IOcrEngine"/>
    /// (e.g. tesseract reader). This parameter is obligatory and it should be
    /// provided in constructor
    /// or using setter.
    /// </remarks>
    public class OcrPdfCreator {
        /// <summary>The logger.</summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Pdfocr.OcrPdfCreator));

        /// <summary>
        /// Selected
        /// <see cref="IOcrEngine"/>.
        /// </summary>
        private IOcrEngine ocrEngine;

        /// <summary>Set of properties.</summary>
        private OcrPdfCreatorProperties ocrPdfCreatorProperties;

        /// <summary>
        /// Creates a new
        /// <see cref="OcrPdfCreator"/>
        /// instance.
        /// </summary>
        /// <param name="ocrEngine">
        /// 
        /// <see cref="IOcrEngine"/>
        /// selected OCR Reader
        /// </param>
        public OcrPdfCreator(IOcrEngine ocrEngine)
            : this(ocrEngine, new OcrPdfCreatorProperties()) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="OcrPdfCreator"/>
        /// instance.
        /// </summary>
        /// <param name="ocrEngine">
        /// selected OCR Reader
        /// <see cref="IOcrEngine"/>
        /// </param>
        /// <param name="ocrPdfCreatorProperties">
        /// set of properties for
        /// <see cref="OcrPdfCreator"/>
        /// </param>
        public OcrPdfCreator(IOcrEngine ocrEngine, OcrPdfCreatorProperties ocrPdfCreatorProperties) {
            SetOcrEngine(ocrEngine);
            SetOcrPdfCreatorProperties(ocrPdfCreatorProperties);
        }

        /// <summary>
        /// Gets properties for
        /// <see cref="OcrPdfCreator"/>.
        /// </summary>
        /// <returns>
        /// set properties
        /// <see cref="OcrPdfCreatorProperties"/>
        /// </returns>
        public OcrPdfCreatorProperties GetOcrPdfCreatorProperties() {
            return ocrPdfCreatorProperties;
        }

        /// <summary>
        /// Sets properties for
        /// <see cref="OcrPdfCreator"/>.
        /// </summary>
        /// <param name="ocrPdfCreatorProperties">
        /// set of properties
        /// <see cref="OcrPdfCreatorProperties"/>
        /// for
        /// <see cref="OcrPdfCreator"/>
        /// </param>
        public void SetOcrPdfCreatorProperties(OcrPdfCreatorProperties ocrPdfCreatorProperties) {
            this.ocrPdfCreatorProperties = ocrPdfCreatorProperties;
        }

        /// <summary>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates pdf using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>.
        /// </summary>
        /// <remarks>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates pdf using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>.
        /// PDF/A-3u document will be created if
        /// provided
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// is not null.
        /// </remarks>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="pdfWriter">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object
        /// to write final pdf document to
        /// </param>
        /// <param name="pdfOutputIntent">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// for PDF/A-3u document
        /// </param>
        /// <returns>
        /// result PDF/A-3u
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object
        /// </returns>
        public PdfDocument CreatePdfA(IList<FileInfo> inputImages, PdfWriter pdfWriter, PdfOutputIntent pdfOutputIntent
            ) {
            LOGGER.Info(MessageFormatUtil.Format(PdfOcrLogMessageConstant.START_OCR_FOR_IMAGES, inputImages.Count));
            // map contains:
            // keys: image files
            // values:
            // map pageNumber -> retrieved text data(text and its coordinates)
            IDictionary<FileInfo, IDictionary<int, IList<TextInfo>>> imagesTextData = new LinkedDictionary<FileInfo, IDictionary
                <int, IList<TextInfo>>>();
            foreach (FileInfo inputImage in inputImages) {
                imagesTextData.Put(inputImage, ocrEngine.DoImageOcr(inputImage));
            }
            // create PdfDocument
            return CreatePdfDocument(pdfWriter, pdfOutputIntent, imagesTextData);
        }

        /// <summary>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrEngine"/>
        /// and
        /// creates pdf using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>.
        /// </summary>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="pdfWriter">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object
        /// to write final pdf document to
        /// </param>
        /// <returns>
        /// result
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object
        /// </returns>
        public PdfDocument CreatePdf(IList<FileInfo> inputImages, PdfWriter pdfWriter) {
            return CreatePdfA(inputImages, pdfWriter, null);
        }

        /// <summary>
        /// Gets used
        /// <see cref="IOcrEngine"/>.
        /// </summary>
        /// <remarks>
        /// Gets used
        /// <see cref="IOcrEngine"/>.
        /// Returns
        /// <see cref="IOcrEngine"/>
        /// reader object to perform OCR.
        /// </remarks>
        /// <returns>
        /// selected
        /// <see cref="IOcrEngine"/>
        /// instance
        /// </returns>
        public IOcrEngine GetOcrEngine() {
            return ocrEngine;
        }

        /// <summary>
        /// Sets
        /// <see cref="IOcrEngine"/>
        /// reader object to perform OCR.
        /// </summary>
        /// <param name="reader">
        /// selected
        /// <see cref="IOcrEngine"/>
        /// instance
        /// </param>
        public void SetOcrEngine(IOcrEngine reader) {
            ocrEngine = reader;
        }

        /// <summary>Gets font as a byte array using provided fontp ath or the default one.</summary>
        /// <returns>selected font as byte[]</returns>
        private byte[] GetFont() {
            if (ocrPdfCreatorProperties.GetFontPath() != null && !String.IsNullOrEmpty(ocrPdfCreatorProperties.GetFontPath
                ())) {
                try {
                    return System.IO.File.ReadAllBytes(System.IO.Path.Combine(ocrPdfCreatorProperties.GetFontPath()));
                }
                catch (Exception e) {
                    LOGGER.Error(MessageFormatUtil.Format(PdfOcrLogMessageConstant.CANNOT_READ_PROVIDED_FONT, e.Message));
                    return GetDefaultFont();
                }
            }
            else {
                return GetDefaultFont();
            }
        }

        /// <summary>Gets default font as a byte array.</summary>
        /// <returns>default font as byte[]</returns>
        private byte[] GetDefaultFont() {
            try {
                using (Stream stream = ResourceUtil.GetResourceStream(GetOcrPdfCreatorProperties().GetDefaultFontName())) {
                    return StreamUtil.InputStreamToArray(stream);
                }
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(MessageFormatUtil.Format(PdfOcrLogMessageConstant.CANNOT_READ_DEFAULT_FONT, e.Message));
                return new byte[0];
            }
        }

        /// <summary>Adds image (or its one page) and text that was found there to canvas.</summary>
        /// <param name="pdfDocument">
        /// result
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// </param>
        /// <param name="font">font for the placed text (could be custom or default)</param>
        /// <param name="imageSize">
        /// size of the image according to the selected
        /// <see cref="ScaleMode"/>
        /// </param>
        /// <param name="pageText">text that was found on this image (or on this page)</param>
        /// <param name="imageData">
        /// input image if it is a single page or its one page if
        /// this is a multi-page image
        /// </param>
        private void AddToCanvas(PdfDocument pdfDocument, PdfFont font, Rectangle imageSize, IList<TextInfo> pageText
            , ImageData imageData) {
            Rectangle rectangleSize = ocrPdfCreatorProperties.GetPageSize() == null ? imageSize : ocrPdfCreatorProperties
                .GetPageSize();
            PageSize size = new PageSize(rectangleSize);
            PdfPage pdfPage = pdfDocument.AddNewPage(size);
            PdfCanvas canvas = new PdfCanvas(pdfPage);
            PdfLayer imageLayer = new PdfLayer(ocrPdfCreatorProperties.GetImageLayerName(), pdfDocument);
            PdfLayer textLayer = new PdfLayer(ocrPdfCreatorProperties.GetTextLayerName(), pdfDocument);
            canvas.BeginLayer(imageLayer);
            AddImageToCanvas(imageData, imageSize, canvas);
            canvas.EndLayer();
            // how much the original image size changed
            float multiplier = imageData == null ? 1 : imageSize.GetWidth() / PdfCreatorUtil.GetPoints(imageData.GetWidth
                ());
            canvas.BeginLayer(textLayer);
            AddTextToCanvas(imageSize, pageText, canvas, font, multiplier, pdfPage.GetMediaBox());
            canvas.EndLayer();
        }

        /// <summary>
        /// Creates a new pdf document using provided properties, adds images with
        /// recognized text.
        /// </summary>
        /// <param name="pdfWriter">
        /// the
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// object
        /// to write final pdf document to
        /// </param>
        /// <param name="pdfOutputIntent">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// for PDF/A-3u document
        /// </param>
        /// <param name="imagesTextData">
        /// Map<File, Map&lt;Integer, List&lt;textinfo>&gt;&gt; -
        /// map that contains input image files as keys,
        /// and as value: map pageNumber -&gt; text for the page
        /// </param>
        /// <returns>
        /// result
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object
        /// </returns>
        private PdfDocument CreatePdfDocument(PdfWriter pdfWriter, PdfOutputIntent pdfOutputIntent, IDictionary<FileInfo
            , IDictionary<int, IList<TextInfo>>> imagesTextData) {
            PdfDocument pdfDocument;
            if (pdfOutputIntent != null) {
                pdfDocument = new PdfADocument(pdfWriter, PdfAConformanceLevel.PDF_A_3U, pdfOutputIntent);
            }
            else {
                pdfDocument = new PdfDocument(pdfWriter);
            }
            // add metadata
            pdfDocument.GetCatalog().SetLang(new PdfString(ocrPdfCreatorProperties.GetPdfLang()));
            pdfDocument.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
            PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
            info.SetTitle(ocrPdfCreatorProperties.GetTitle());
            // create PdfFont
            PdfFont defaultFont = null;
            try {
                defaultFont = PdfFontFactory.CreateFont(GetFont(), PdfEncodings.IDENTITY_H, true);
            }
            catch (Exception e) {
                LOGGER.Error(MessageFormatUtil.Format(PdfOcrLogMessageConstant.CANNOT_READ_PROVIDED_FONT, e.Message));
                try {
                    defaultFont = PdfFontFactory.CreateFont(GetDefaultFont(), PdfEncodings.IDENTITY_H, true);
                }
                catch (Exception ex) {
                    LOGGER.Error(MessageFormatUtil.Format(PdfOcrLogMessageConstant.CANNOT_READ_DEFAULT_FONT, ex.Message));
                    throw new OcrException(OcrException.CANNOT_READ_FONT);
                }
            }
            AddDataToPdfDocument(imagesTextData, pdfDocument, defaultFont);
            return pdfDocument;
        }

        /// <summary>Places provided images and recognized text to the result PDF document.</summary>
        /// <param name="imagesTextData">
        /// Map<File, Map&lt;Integer, List&lt;textinfo>&gt;&gt; -
        /// map that contains input image
        /// files as keys, and as value:
        /// map pageNumber -&gt; text for the page
        /// </param>
        /// <param name="pdfDocument">
        /// result
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// </param>
        /// <param name="font">font for the placed text (could be custom or default)</param>
        private void AddDataToPdfDocument(IDictionary<FileInfo, IDictionary<int, IList<TextInfo>>> imagesTextData, 
            PdfDocument pdfDocument, PdfFont font) {
            foreach (KeyValuePair<FileInfo, IDictionary<int, IList<TextInfo>>> entry in imagesTextData) {
                try {
                    FileInfo inputImage = entry.Key;
                    IList<ImageData> imageDataList = PdfCreatorUtil.GetImageData(inputImage);
                    LOGGER.Info(MessageFormatUtil.Format(PdfOcrLogMessageConstant.NUMBER_OF_PAGES_IN_IMAGE, inputImage.ToString
                        (), imageDataList.Count));
                    IDictionary<int, IList<TextInfo>> imageTextData = entry.Value;
                    if (imageTextData.Keys.Count > 0) {
                        for (int page = 0; page < imageDataList.Count; ++page) {
                            ImageData imageData = imageDataList[page];
                            Rectangle imageSize = PdfCreatorUtil.CalculateImageSize(imageData, ocrPdfCreatorProperties.GetScaleMode(), 
                                ocrPdfCreatorProperties.GetPageSize());
                            if (imageTextData.ContainsKey(page + 1)) {
                                AddToCanvas(pdfDocument, font, imageSize, imageTextData.Get(page + 1), imageData);
                            }
                        }
                    }
                }
                catch (System.IO.IOException e) {
                    LOGGER.Error(MessageFormatUtil.Format(PdfOcrLogMessageConstant.CANNOT_ADD_DATA_TO_PDF_DOCUMENT, e.Message)
                        );
                }
            }
        }

        /// <summary>Places given image to canvas to background to a separate layer.</summary>
        /// <param name="imageData">
        /// input image as
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="imageSize">
        /// size of the image according to the selected
        /// <see cref="ScaleMode"/>
        /// </param>
        /// <param name="pdfCanvas">canvas to place the image</param>
        private void AddImageToCanvas(ImageData imageData, Rectangle imageSize, PdfCanvas pdfCanvas) {
            if (imageData != null) {
                if (ocrPdfCreatorProperties.GetPageSize() == null) {
                    pdfCanvas.AddImage(imageData, imageSize, false);
                }
                else {
                    IList<float> coordinates = PdfCreatorUtil.CalculateImageCoordinates(ocrPdfCreatorProperties.GetPageSize(), 
                        imageSize);
                    Rectangle rect = new Rectangle(coordinates[0], coordinates[1], imageSize.GetWidth(), imageSize.GetHeight()
                        );
                    pdfCanvas.AddImage(imageData, rect, false);
                }
            }
        }

        /// <summary>Places retrieved text to canvas to a separate layer.</summary>
        /// <param name="imageSize">
        /// size of the image according to the selected
        /// <see cref="ScaleMode"/>
        /// </param>
        /// <param name="pageText">text that was found on this image (or on this page)</param>
        /// <param name="pdfCanvas">canvas to place the text</param>
        /// <param name="font">font for the placed text (could be custom or default)</param>
        /// <param name="multiplier">coefficient to adjust text placing on canvas</param>
        /// <param name="pageMediaBox">page parameters</param>
        private void AddTextToCanvas(Rectangle imageSize, IList<TextInfo> pageText, PdfCanvas pdfCanvas, PdfFont font
            , float multiplier, Rectangle pageMediaBox) {
            if (pageText == null || pageText.Count == 0) {
                pdfCanvas.BeginText().SetFontAndSize(font, 1);
            }
            else {
                IList<float> imageCoordinates = PdfCreatorUtil.CalculateImageCoordinates(ocrPdfCreatorProperties.GetPageSize
                    (), imageSize);
                float x = imageCoordinates[0];
                float y = imageCoordinates[1];
                foreach (TextInfo item in pageText) {
                    String line = item.GetText();
                    IList<float> coordinates = item.GetBbox();
                    float left = coordinates[0] * multiplier;
                    float right = (coordinates[2] + 1) * multiplier - 1;
                    float top = coordinates[1] * multiplier;
                    float bottom = (coordinates[3] + 1) * multiplier - 1;
                    float bboxWidthPt = PdfCreatorUtil.GetPoints(right - left);
                    float bboxHeightPt = PdfCreatorUtil.GetPoints(bottom - top);
                    if (!String.IsNullOrEmpty(line) && bboxHeightPt > 0 && bboxWidthPt > 0) {
                        // Scale the text width to fit the OCR bbox
                        float fontSize = PdfCreatorUtil.CalculateFontSize(new Document(pdfCanvas.GetDocument()), line, font, bboxHeightPt
                            , bboxWidthPt);
                        float lineWidth = font.GetWidth(line, fontSize);
                        float deltaX = PdfCreatorUtil.GetPoints(left);
                        float deltaY = imageSize.GetHeight() - PdfCreatorUtil.GetPoints(bottom);
                        float descent = font.GetDescent(line, fontSize);
                        iText.Layout.Canvas canvas = new iText.Layout.Canvas(pdfCanvas, pageMediaBox);
                        Text text = new Text(line).SetHorizontalScaling(bboxWidthPt / lineWidth);
                        Paragraph paragraph = new Paragraph(text).SetMargin(0).SetMultipliedLeading(1.2f);
                        paragraph.SetFont(font).SetFontSize(fontSize);
                        paragraph.SetWidth(bboxWidthPt * 1.5f);
                        if (ocrPdfCreatorProperties.GetTextColor() != null) {
                            paragraph.SetFontColor(ocrPdfCreatorProperties.GetTextColor());
                        }
                        else {
                            paragraph.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.INVISIBLE);
                        }
                        canvas.ShowTextAligned(paragraph, deltaX + x, deltaY + y, TextAlignment.LEFT);
                        canvas.Close();
                    }
                }
            }
        }
    }
}
