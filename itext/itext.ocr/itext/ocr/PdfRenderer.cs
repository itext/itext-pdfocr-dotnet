using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common.Logging;
using iText.IO.Font;
using iText.IO.Image;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using iText.Pdfa;

namespace iText.Ocr {
    /// <summary>
    /// <see cref="PdfRenderer"/>
    /// is the class that creates Pdf documents containing input
    /// images and text that was recognized using provided
    /// <see cref="IOcrReader"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="PdfRenderer"/>
    /// is the class that creates Pdf documents containing input
    /// images and text that was recognized using provided
    /// <see cref="IOcrReader"/>.
    /// <see cref="PdfRenderer"/>
    /// provides possibilities to set list of input images to
    /// be used for OCR, to set scaling mode for images, to set color of text in
    /// output PDF document, to set fixed size of the PDF document's page and to
    /// perform OCR using given images and to return
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// as result.
    /// PDFRenderer's OCR is based on the provided
    /// <see cref="IOcrReader"/>
    /// (e.g. tesseract reader). This parameter is obligatory and it should be
    /// provided in constructor
    /// or using setter.
    /// </remarks>
    public class PdfRenderer {
        /// <summary>The logger.</summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Ocr.PdfRenderer));

        /// <summary>Path to default font file.</summary>
        /// <remarks>
        /// Path to default font file.
        /// "LiberationSans-Regular" by default.
        /// </remarks>
        private const String DEFAULT_FONT_NAME = "LiberationSans-Regular.ttf";

        /// <summary>
        /// Selected
        /// <see cref="IOcrReader"/>.
        /// </summary>
        private IOcrReader ocrReader;

        /// <summary>Set of properties.</summary>
        private OcrPdfCreatorProperties ocrPdfCreatorProperties;

        /// <summary>
        /// Creates a new
        /// <see cref="PdfRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="ocrReader">
        /// 
        /// <see cref="IOcrReader"/>
        /// selected OCR Reader
        /// </param>
        public PdfRenderer(IOcrReader ocrReader) {
            SetOcrReader(ocrReader);
            SetOcrPdfCreatorProperties(new OcrPdfCreatorProperties());
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="ocrReader">
        /// selected OCR Reader
        /// <see cref="IOcrReader"/>
        /// </param>
        /// <param name="ocrPdfCreatorProperties">
        /// set of properties for
        /// <see cref="PdfRenderer"/>
        /// </param>
        public PdfRenderer(IOcrReader ocrReader, OcrPdfCreatorProperties ocrPdfCreatorProperties) {
            SetOcrReader(ocrReader);
            SetOcrPdfCreatorProperties(ocrPdfCreatorProperties);
        }

        /// <summary>
        /// Gets properties for
        /// <see cref="PdfRenderer"/>.
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
        /// <see cref="PdfRenderer"/>.
        /// </summary>
        /// <param name="ocrPdfCreatorProperties">
        /// set of properties
        /// <see cref="OcrPdfCreatorProperties"/>
        /// for
        /// <see cref="PdfRenderer"/>
        /// </param>
        public void SetOcrPdfCreatorProperties(OcrPdfCreatorProperties ocrPdfCreatorProperties) {
            this.ocrPdfCreatorProperties = ocrPdfCreatorProperties;
        }

        /// <summary>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrReader"/>
        /// and
        /// creates pdf using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>.
        /// </summary>
        /// <remarks>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrReader"/>
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
            LOGGER.Info(MessageFormatUtil.Format(LogMessageConstant.StartOcrForImages, inputImages.Count));
            // map contains:
            // keys: image files
            // values:
            // map pageNumber -> retrieved text data(text and its coordinates)
            IDictionary<FileInfo, IDictionary<int, IList<TextInfo>>> imagesTextData = new LinkedDictionary<FileInfo, IDictionary
                <int, IList<TextInfo>>>();
            foreach (FileInfo inputImage in inputImages) {
                imagesTextData.Put(inputImage, ocrReader.DoImageOcr(inputImage));
            }
            // create PdfDocument
            return CreatePdfDocument(pdfWriter, pdfOutputIntent, imagesTextData);
        }

        /// <summary>
        /// Performs OCR with set parameters using provided
        /// <see cref="IOcrReader"/>
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
        /// Performs OCR using provided
        /// <see cref="IOcrReader"/>
        /// for the given list of
        /// input images and saves output to a text file using provided path.
        /// </summary>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="path">
        /// path as
        /// <see cref="System.String"/>
        /// to file to be
        /// created
        /// </param>
        public virtual void CreateTxt(IList<FileInfo> inputImages, String path) {
            LOGGER.Info(MessageFormatUtil.Format(LogMessageConstant.StartOcrForImages, inputImages.Count));
            StringBuilder content = new StringBuilder();
            foreach (FileInfo inputImage in inputImages) {
                content.Append(ocrReader.DoImageOcr(inputImage, IOcrReader.OutputFormat.TXT));
            }
            // write to file
            WriteToTextFile(path, content.ToString());
        }

        /// <summary>Gets path to the default font.</summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// path to default font
        /// </returns>
        public virtual String GetDefaultFontName() {
            return TesseractUtil.FONT_RESOURCE_PATH + DEFAULT_FONT_NAME;
        }

        /// <summary>
        /// Gets used
        /// <see cref="IOcrReader"/>.
        /// </summary>
        /// <remarks>
        /// Gets used
        /// <see cref="IOcrReader"/>.
        /// Returns
        /// <see cref="IOcrReader"/>
        /// reader object to perform OCR.
        /// </remarks>
        /// <returns>
        /// selected
        /// <see cref="IOcrReader"/>
        /// instance
        /// </returns>
        public IOcrReader GetOcrReader() {
            return ocrReader;
        }

        /// <summary>
        /// Sets
        /// <see cref="IOcrReader"/>
        /// reader object to perform OCR.
        /// </summary>
        /// <param name="reader">
        /// selected
        /// <see cref="IOcrReader"/>
        /// instance
        /// </param>
        public void SetOcrReader(IOcrReader reader) {
            ocrReader = reader;
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
                    LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CannotReadProvidedFont, e.Message));
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
                using (Stream stream = ResourceUtil.GetResourceStream(GetDefaultFontName())) {
                    return StreamUtil.InputStreamToArray(stream);
                }
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CannotReadDefaultFont, e.Message));
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
            float multiplier = imageData == null ? 1 : imageSize.GetWidth() / UtilService.GetPoints(imageData.GetWidth
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
                LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CannotReadProvidedFont, e.Message));
                try {
                    defaultFont = PdfFontFactory.CreateFont(GetDefaultFont(), PdfEncodings.IDENTITY_H, true);
                }
                catch (Exception ex) {
                    LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CannotReadDefaultFont, ex.Message));
                    throw new OcrException(OcrException.CannotReadFont);
                }
            }
            AddDataToPdfDocument(imagesTextData, pdfDocument, defaultFont);
            return pdfDocument;
        }

        /// <summary>
        /// Writes provided
        /// <see cref="System.String"/>
        /// to text file using
        /// provided path.
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
        private void WriteToTextFile(String path, String data) {
            try {
                using (TextWriter writer = new StreamWriter(new FileStream(path, FileMode.Create), System.Text.Encoding.UTF8
                    )) {
                    writer.Write(data);
                }
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CannotWriteToFile, path, e.Message));
            }
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
                    IList<ImageData> imageDataList = GetImageData(inputImage);
                    LOGGER.Info(MessageFormatUtil.Format(LogMessageConstant.NumberOfPagesInImage, inputImage.ToString(), imageDataList
                        .Count));
                    IDictionary<int, IList<TextInfo>> imageTextData = entry.Value;
                    if (imageTextData.Keys.Count > 0) {
                        for (int page = 0; page < imageDataList.Count; ++page) {
                            ImageData imageData = imageDataList[page];
                            Rectangle imageSize = UtilService.CalculateImageSize(imageData, ocrPdfCreatorProperties.GetScaleMode(), ocrPdfCreatorProperties
                                .GetPageSize());
                            if (imageTextData.ContainsKey(page + 1)) {
                                AddToCanvas(pdfDocument, font, imageSize, imageTextData.Get(page + 1), imageData);
                            }
                        }
                    }
                }
                catch (System.IO.IOException e) {
                    LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CannotAddDataToPdfDocument, e.Message));
                }
            }
        }

        /// <summary>
        /// Retrieves
        /// <see cref="iText.IO.Image.ImageData"/>
        /// from the
        /// input
        /// <see cref="System.IO.FileInfo"/>.
        /// </summary>
        /// <param name="inputImage">
        /// input image as
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <returns>
        /// list of
        /// <see cref="iText.IO.Image.ImageData"/>
        /// objects
        /// (more than one element in the list if it is a multipage tiff)
        /// </returns>
        private IList<ImageData> GetImageData(FileInfo inputImage) {
            IList<ImageData> images = new List<ImageData>();
            String ext = "";
            int index = inputImage.FullName.LastIndexOf('.');
            if (index > 0) {
                ext = new String(inputImage.FullName.ToCharArray(), index + 1, inputImage.FullName.Length - index - 1);
                if ("tiff".Equals(ext.ToLowerInvariant()) || "tif".Equals(ext.ToLowerInvariant())) {
                    int tiffPages = ImageUtil.GetNumberOfPageTiff(inputImage);
                    for (int page = 0; page < tiffPages; page++) {
                        byte[] bytes = System.IO.File.ReadAllBytes(inputImage.FullName);
                        ImageData imageData = ImageDataFactory.CreateTiff(bytes, true, page + 1, true);
                        images.Add(imageData);
                    }
                }
                else {
                    try {
                        ImageData imageData = ImageDataFactory.Create(inputImage.FullName);
                        images.Add(imageData);
                    }
                    catch (iText.IO.IOException e) {
                        LOGGER.Info(MessageFormatUtil.Format(LogMessageConstant.AttemptToConvertToPng, inputImage.FullName, e.Message
                            ));
                        try {
                            System.Drawing.Bitmap bufferedImage = null;
                            try {
                                bufferedImage = ImageUtil.ReadImageFromFile(inputImage);
                            }
                            catch (Exception ex) {
                                LOGGER.Info(MessageFormatUtil.Format(LogMessageConstant.ReadingImageAsPix, inputImage.FullName, ex.Message
                                    ));
                                bufferedImage = ImageUtil.ReadAsPixAndConvertToBufferedImage(inputImage);
                            }
                            ByteArrayOutputStream baos = new ByteArrayOutputStream();
                            bufferedImage.Save(baos, TesseractUtil.GetPngImageFormat());
                            ImageData imageData = ImageDataFactory.Create(baos.ToArray());
                            images.Add(imageData);
                        }
                        catch (Exception ex) {
                            LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CannotReadInputImage, ex.Message));
                            throw new OcrException(OcrException.CannotReadInputImage);
                        }
                    }
                }
            }
            return images;
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
                    IList<float> coordinates = CalculateImageCoordinates(ocrPdfCreatorProperties.GetPageSize(), imageSize);
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
                IList<float> imageCoordinates = CalculateImageCoordinates(ocrPdfCreatorProperties.GetPageSize(), imageSize
                    );
                float x = imageCoordinates[0];
                float y = imageCoordinates[1];
                foreach (TextInfo item in pageText) {
                    String line = item.GetText();
                    IList<float> coordinates = item.GetCoordinates();
                    float left = coordinates[0] * multiplier;
                    float right = (coordinates[2] + 1) * multiplier - 1;
                    float top = coordinates[1] * multiplier;
                    float bottom = (coordinates[3] + 1) * multiplier - 1;
                    float bboxWidthPt = UtilService.GetPoints(right - left);
                    float bboxHeightPt = UtilService.GetPoints(bottom - top);
                    if (!String.IsNullOrEmpty(line) && bboxHeightPt > 0 && bboxWidthPt > 0) {
                        // Scale the text width to fit the OCR bbox
                        float fontSize = CalculateFontSize(new Document(pdfCanvas.GetDocument()), line, font, bboxHeightPt, bboxWidthPt
                            );
                        float lineWidth = font.GetWidth(line, fontSize);
                        float deltaX = UtilService.GetPoints(left);
                        float deltaY = imageSize.GetHeight() - UtilService.GetPoints(bottom);
                        float descent = font.GetDescent(line, fontSize);
                        iText.Layout.Canvas canvas = new iText.Layout.Canvas(pdfCanvas, pageMediaBox);
                        iText.Layout.Element.Text text = new iText.Layout.Element.Text(line).SetHorizontalScaling(bboxWidthPt / lineWidth
                            );
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

        /// <summary>
        /// Calculates font size according to given bbox height, width and selected
        /// font.
        /// </summary>
        /// <param name="document">
        /// pdf document as a
        /// <see cref="iText.Layout.Document"/>
        /// object
        /// </param>
        /// <param name="line">text line</param>
        /// <param name="font">font for the placed text (could be custom or default)</param>
        /// <param name="bboxHeightPt">height of bbox calculated by OCR Reader</param>
        /// <param name="bboxWidthPt">width of bbox calculated by OCR Reader</param>
        /// <returns>font size</returns>
        private float CalculateFontSize(Document document, String line, PdfFont font, float bboxHeightPt, float bboxWidthPt
            ) {
            Rectangle bbox = new Rectangle(bboxWidthPt * 1.5f, bboxHeightPt * 1.5f);
            Paragraph paragraph = new Paragraph(line);
            paragraph.SetWidth(bboxWidthPt);
            paragraph.SetFont(font);
            // setting minimum and maximum (approx.) values for font size
            float fontSize = 1;
            float maxFontSize = bboxHeightPt * 2;
            while (Math.Abs(fontSize - maxFontSize) > 1e-1) {
                float curFontSize = (fontSize + maxFontSize) / 2;
                paragraph.SetFontSize(curFontSize);
                IRenderer renderer = paragraph.CreateRendererSubTree().SetParent(document.GetRenderer());
                LayoutContext context = new LayoutContext(new LayoutArea(1, bbox));
                if (renderer.Layout(context).GetStatus() == LayoutResult.FULL) {
                    fontSize = curFontSize;
                }
                else {
                    maxFontSize = curFontSize;
                }
            }
            return fontSize;
        }

        /// <summary>Calculates image coordinates on the page.</summary>
        /// <param name="size">size of the page</param>
        /// <param name="imageSize">size of the image</param>
        /// <returns>list of two elements (coordinates): first - x, second - y.</returns>
        private IList<float> CalculateImageCoordinates(Rectangle size, Rectangle imageSize) {
            float x = 0;
            float y = 0;
            if (ocrPdfCreatorProperties.GetPageSize() != null) {
                if (imageSize.GetHeight() < size.GetHeight()) {
                    y = (size.GetHeight() - imageSize.GetHeight()) / 2;
                }
                if (imageSize.GetWidth() < size.GetWidth()) {
                    x = (size.GetWidth() - imageSize.GetWidth()) / 2;
                }
            }
            return JavaUtil.ArraysAsList(x, y);
        }
    }
}
