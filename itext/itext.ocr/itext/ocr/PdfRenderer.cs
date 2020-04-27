using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common.Logging;
using iText.IO.Font;
using iText.IO.Image;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Pdfa;

namespace iText.Ocr {
    /// <summary>PDF Renderer class.</summary>
    /// <remarks>
    /// PDF Renderer class.
    /// <para />
    /// The IPdfRenderer provides possibilities to set list of input images
    /// to be used for OCR, to set scaling mode for images, color of text in
    /// the output PDF document, set fixed size of the PDF document
    /// and to perform OCR using given images and return PDFDocument as result
    /// <para />
    /// PDFRenderer's ocr is based on the provided IOcrReader (e.g. tesseract).
    /// This parameter is obligatory and it should be provided in constructor
    /// or using setter
    /// </remarks>
    public class PdfRenderer : IPdfRenderer {
        /// <summary>Logger.</summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Ocr.PdfRenderer));

        /// <summary>Path to default font file (LiberationSans-Regular).</summary>
        private String defaultFontName = "LiberationSans-Regular.ttf";

        /// <summary>List of Files with input images.</summary>
        private IList<FileInfo> inputImages = JavaCollectionsUtil.EmptyList<FileInfo>();

        /// <summary>CMYK color of the text in the output PDF document.</summary>
        /// <remarks>
        /// CMYK color of the text in the output PDF document.
        /// Text will be transparent by default
        /// </remarks>
        private Color textColor = null;

        /// <summary>Scale mode for input images: "scaleToFit" by default.</summary>
        private IPdfRenderer.ScaleMode scaleMode = IPdfRenderer.ScaleMode.scaleToFit;

        /// <summary>Size of the PDF document pages: "A4" by default.</summary>
        /// <remarks>
        /// Size of the PDF document pages: "A4" by default.
        /// This parameter is taken into account only if "scaleMode" is scaleWidth,
        /// scaleHeight or scaleToFit
        /// </remarks>
        private Rectangle pageSize = new Rectangle(PageSize.A4.GetX(), PageSize.A4.GetY(), PageSize.A4.GetWidth(), 
            PageSize.A4.GetHeight());

        /// <summary>Name of the image layer.</summary>
        private String imageLayerName = "Image Layer";

        /// <summary>Name of the text layer.</summary>
        private String textLayerName = "Text Layer";

        /// <summary>Pdf Language (default: "en-US").</summary>
        private String pdfLang = "en-US";

        /// <summary>Title of the created document (empty by default).</summary>
        private String title = "";

        /// <summary>Path to font.</summary>
        /// <remarks>
        /// Path to font.
        /// (should be set explicitly or default font will be used)
        /// </remarks>
        private String fontPath;

        /// <summary>
        /// Parameter describing selectedOCR reader
        /// that corresponds IOcrReader interface.
        /// </summary>
        private IOcrReader ocrReader;

        /// <summary>PdfRenderer constructor with IOcrReader.</summary>
        /// <param name="reader">IOcrReader</param>
        public PdfRenderer(IOcrReader reader) {
            ocrReader = reader;
        }

        /// <summary>PdfRenderer constructor with IOcrReader and list of input files.</summary>
        /// <param name="reader">IOcrReader</param>
        /// <param name="images">images</param>
        public PdfRenderer(IOcrReader reader, IList<FileInfo> images) {
            ocrReader = reader;
            inputImages = JavaCollectionsUtil.UnmodifiableList<FileInfo>(images);
        }

        /// <summary>
        /// PdfRenderer constructor with IOcrReader, list of input files
        /// and scale mode.
        /// </summary>
        /// <param name="reader">IOcrReader</param>
        /// <param name="images">List<file></param>
        /// <param name="mode">ScaleMode</param>
        public PdfRenderer(IOcrReader reader, IList<FileInfo> images, IPdfRenderer.ScaleMode mode) {
            ocrReader = reader;
            inputImages = JavaCollectionsUtil.UnmodifiableList<FileInfo>(images);
            scaleMode = mode;
        }

        /// <summary>
        /// PdfRenderer constructor with IOcrReader, list of input files
        /// and text color.
        /// </summary>
        /// <param name="reader">IOcrReader</param>
        /// <param name="images">List<file></param>
        /// <param name="newColor">Color</param>
        public PdfRenderer(IOcrReader reader, IList<FileInfo> images, Color newColor) {
            ocrReader = reader;
            inputImages = JavaCollectionsUtil.UnmodifiableList<FileInfo>(images);
            textColor = newColor;
            scaleMode = IPdfRenderer.ScaleMode.keepOriginalSize;
        }

        /// <summary>
        /// PdfRenderer constructor with IOcrReader, list of input files,
        /// text color and scale mode.
        /// </summary>
        /// <param name="reader">IOcrReader</param>
        /// <param name="images">List<file></param>
        /// <param name="newColor">Color</param>
        /// <param name="mode">ScaleMode</param>
        public PdfRenderer(IOcrReader reader, IList<FileInfo> images, Color newColor, IPdfRenderer.ScaleMode mode) {
            ocrReader = reader;
            textColor = newColor;
            inputImages = JavaCollectionsUtil.UnmodifiableList<FileInfo>(images);
            scaleMode = mode;
        }

        /// <summary>Set list of input images for OCR.</summary>
        /// <param name="images">List<file></param>
        public override void SetInputImages(IList<FileInfo> images) {
            inputImages = JavaCollectionsUtil.UnmodifiableList<FileInfo>(images);
        }

        /// <summary>Get list of provided input images for OCR.</summary>
        /// <returns>List<file></returns>
        public sealed override IList<FileInfo> GetInputImages() {
            return new List<FileInfo>(inputImages);
        }

        /// <summary>Set text color (should be CMYK) in output PDF document.</summary>
        /// <param name="newColor">CMYK Color</param>
        public sealed override void SetTextColor(Color newColor) {
            textColor = newColor;
        }

        /// <summary>Get text color in output PDF document.</summary>
        /// <returns>Color</returns>
        public sealed override Color GetTextColor() {
            return textColor;
        }

        /// <summary>
        /// Set scale mode for input images using available options
        /// from ScaleMode enum.
        /// </summary>
        /// <param name="mode">ScaleMode</param>
        public sealed override void SetScaleMode(IPdfRenderer.ScaleMode mode) {
            scaleMode = mode;
        }

        /// <summary>Get scale mode for input images.</summary>
        /// <returns>ScaleMode</returns>
        public sealed override IPdfRenderer.ScaleMode GetScaleMode() {
            return scaleMode;
        }

        /// <summary>Set fixed size for output PDF document.</summary>
        /// <remarks>
        /// Set fixed size for output PDF document.
        /// (this parameter is used only is ScaleMode is set as "fitToSize")
        /// </remarks>
        /// <param name="size">pageSize</param>
        public sealed override void SetPageSize(Rectangle size) {
            pageSize = size;
        }

        /// <summary>Get size for output document.</summary>
        /// <returns>Rectangle</returns>
        public sealed override Rectangle GetPageSize() {
            return pageSize;
        }

        /// <summary>Set name for the image layer.</summary>
        /// <remarks>
        /// Set name for the image layer.
        /// (of by default it is "Image layer")
        /// </remarks>
        /// <param name="name">layer's name</param>
        public sealed override void SetImageLayerName(String name) {
            imageLayerName = name;
        }

        /// <summary>Get name of image layer.</summary>
        /// <returns>
        /// layer's name that was manually set
        /// or the default one (="Image layer")
        /// </returns>
        public sealed override String GetImageLayerName() {
            return imageLayerName;
        }

        /// <summary>Set name for the text layer.</summary>
        /// <remarks>
        /// Set name for the text layer.
        /// (of by default it is "Text layer")
        /// </remarks>
        /// <param name="name">layer's name</param>
        public sealed override void SetTextLayerName(String name) {
            textLayerName = name;
        }

        /// <returns>
        /// layer's name that was manually set
        /// or the default one (="Text layer")
        /// </returns>
        public sealed override String GetTextLayerName() {
            return textLayerName;
        }

        /// <summary>Specify pdf natural language, and optionally locale.</summary>
        /// <param name="lang">String</param>
        public sealed override void SetPdfLang(String lang) {
            pdfLang = lang;
        }

        /// <returns>pdf document lang</returns>
        public sealed override String GetPdfLang() {
            return pdfLang;
        }

        /// <summary>Set pdf document title.</summary>
        /// <param name="name">String</param>
        public sealed override void SetTitle(String name) {
            title = name;
        }

        /// <returns>pdf document title</returns>
        public sealed override String GetTitle() {
            return title;
        }

        /// <summary>Set path to font to be used in pdf document.</summary>
        /// <remarks>
        /// Set path to font to be used in pdf document.
        /// (if it isn't set default font will be used)
        /// </remarks>
        /// <param name="path">Path</param>
        public override void SetFontPath(String path) {
            fontPath = path;
        }

        /// <returns>Font path that was set or default font path</returns>
        public override String GetFontPath() {
            return fontPath;
        }

        /// <returns>path to default font</returns>
        public virtual String GetDefaultFontName() {
            return TesseractUtil.FONT_RESOURCE_PATH + defaultFontName;
        }

        /// <summary>Set IOcrReader reader (e.g. TesseractReader object).</summary>
        /// <param name="reader">IOcrReader</param>
        public void SetOcrReader(IOcrReader reader) {
            ocrReader = reader;
        }

        /// <summary>Get used ocr reader.</summary>
        /// <returns>IOcrReader</returns>
        public IOcrReader GetOcrReader() {
            return ocrReader;
        }

        /// <summary>Get byte array sing provided font path or the default one.</summary>
        /// <returns>byte[]</returns>
        public byte[] GetFont() {
            if (fontPath != null && !String.IsNullOrEmpty(fontPath)) {
                try {
                    return System.IO.File.ReadAllBytes(System.IO.Path.Combine(fontPath));
                }
                catch (Exception e) {
                    LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CANNOT_READ_PROVIDED_FONT, e.Message));
                    return GetDefaultFont();
                }
            }
            else {
                return GetDefaultFont();
            }
        }

        /// <summary>Get byte array using default font path.</summary>
        /// <returns>byte[]</returns>
        public byte[] GetDefaultFont() {
            try {
                using (Stream stream = ResourceUtil.GetResourceStream(GetDefaultFontName())) {
                    return StreamUtil.InputStreamToArray(stream);
                }
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CANNOT_READ_DEFAULT_FONT, e.Message));
                return new byte[0];
            }
        }

        /// <summary>
        /// Perform OCR for the given list of input images and
        /// save output to a text file with provided path.
        /// </summary>
        /// <param name="absolutePath">String</param>
        public override void DoPdfOcr(String absolutePath) {
            LOGGER.Info(MessageFormatUtil.Format(LogMessageConstant.START_OCR_FOR_IMAGES, GetInputImages().Count));
            StringBuilder content = new StringBuilder();
            foreach (FileInfo inputImage in GetInputImages()) {
                content.Append(DoOCRForImages(inputImage, IOcrReader.OutputFormat.txt));
            }
            // write to file
            WriteToTextFile(absolutePath, content.ToString());
        }

        /// <summary>Perform OCR for the given list of input images using provided pdfWriter.</summary>
        /// <param name="pdfWriter">PdfWriter</param>
        /// <returns>PdfDocument</returns>
        public sealed override PdfDocument DoPdfOcr(PdfWriter pdfWriter) {
            return DoPdfOcr(pdfWriter, null);
        }

        /// <summary>Perform OCR for the given list of input images using provided pdfWriter.</summary>
        /// <remarks>
        /// Perform OCR for the given list of input images using provided pdfWriter.
        /// PDF/A-3u document will be created if pdfOutputIntent is not null
        /// </remarks>
        /// <param name="pdfWriter">PdfWriter</param>
        /// <param name="pdfOutputIntent">PdfOutputIntent</param>
        /// <returns>PDF/A-3u document if pdfOutputIntent is not null</returns>
        public sealed override PdfDocument DoPdfOcr(PdfWriter pdfWriter, PdfOutputIntent pdfOutputIntent) {
            LOGGER.Info(MessageFormatUtil.Format(LogMessageConstant.START_OCR_FOR_IMAGES, GetInputImages().Count));
            // map contains:
            // keys: image files
            // values: map pageNumber -> retrieved text data(text and its coordinates)
            IDictionary<FileInfo, IDictionary<int, IList<TextInfo>>> imagesTextData = new LinkedDictionary<FileInfo, IDictionary
                <int, IList<TextInfo>>>();
            foreach (FileInfo inputImage in GetInputImages()) {
                imagesTextData.Put(inputImage, DoOCRForImages(inputImage));
            }
            // create PdfDocument
            return CreatePdfDocument(pdfWriter, pdfOutputIntent, imagesTextData);
        }

        /// <summary>
        /// Create a pdf document using provided properties,
        /// add images with parsed text.
        /// </summary>
        /// <param name="pdfWriter">PdfWriter</param>
        /// <param name="pdfOutputIntent">PdfOutputIntent</param>
        /// <param name="imagesTextData">Map<File, Map&lt;Integer, List&lt;textinfo>&gt;&gt;</param>
        /// <returns>PdfDocument</returns>
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
            pdfDocument.GetCatalog().SetLang(new PdfString(GetPdfLang()));
            pdfDocument.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
            PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
            info.SetTitle(GetTitle());
            // create PdfFont
            PdfFont defaultFont = null;
            try {
                defaultFont = PdfFontFactory.CreateFont(GetFont(), PdfEncodings.IDENTITY_H, true);
            }
            catch (Exception e) {
                LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CANNOT_READ_PROVIDED_FONT, e.Message));
                try {
                    defaultFont = PdfFontFactory.CreateFont(GetDefaultFont(), PdfEncodings.IDENTITY_H, true);
                }
                catch (Exception ex) {
                    LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CANNOT_READ_DEFAULT_FONT, ex.Message));
                    throw new OCRException(OCRException.CANNOT_READ_FONT);
                }
            }
            AddDataToPdfDocument(imagesTextData, pdfDocument, defaultFont);
            return pdfDocument;
        }

        /// <summary>Write parsed data to text file using provided path.</summary>
        /// <param name="path">String</param>
        /// <param name="data">String</param>
        private void WriteToTextFile(String path, String data) {
            try {
                using (TextWriter writer = new StreamWriter(new FileStream(path, FileMode.Create), System.Text.Encoding.UTF8
                    )) {
                    writer.Write(data);
                }
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CANNOT_WRITE_TO_FILE, path, e.Message));
            }
        }

        /// <summary>Validate image format and perform OCR specifying output format.</summary>
        /// <param name="inputImage">File</param>
        /// <param name="outputFormat">OutputFormat</param>
        /// <returns>String</returns>
        private String DoOCRForImages(FileInfo inputImage, IOcrReader.OutputFormat outputFormat) {
            String data = null;
            if (IsValidImageFormat(inputImage)) {
                data = ocrReader.ReadDataFromInput(inputImage, outputFormat);
            }
            return data;
        }

        /// <summary>Validate image format and perform OCR using hOCR output format.</summary>
        /// <param name="inputImage">File</param>
        /// <returns>Map<Integer, List&lt;textinfo>&gt;</returns>
        private IDictionary<int, IList<TextInfo>> DoOCRForImages(FileInfo inputImage) {
            IDictionary<int, IList<TextInfo>> data = new LinkedDictionary<int, IList<TextInfo>>();
            if (IsValidImageFormat(inputImage)) {
                data = ocrReader.ReadDataFromInput(inputImage);
            }
            return data;
        }

        /// <summary>Validate input image format.</summary>
        /// <remarks>
        /// Validate input image format.
        /// allowed image formats are provided in "ImageFormat" format
        /// </remarks>
        /// <param name="image">input file</param>
        /// <returns>true if image format is valid, false - if not</returns>
        private bool IsValidImageFormat(FileInfo image) {
            bool isValid = false;
            String extension = "incorrect extension";
            int index = image.FullName.LastIndexOf('.');
            if (index > 0) {
                extension = new String(image.FullName.ToCharArray(), index + 1, image.FullName.Length - index - 1);
                foreach (IPdfRenderer.ImgFormat imageFormat in System.Enum.GetValues(typeof(IPdfRenderer.ImgFormat))) {
                    if (imageFormat.ToString().Equals(extension.ToLowerInvariant())) {
                        isValid = true;
                        break;
                    }
                }
            }
            if (!isValid) {
                LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CANNOT_READ_INPUT_IMAGE, image.FullName));
                throw new OCRException(OCRException.INCORRECT_INPUT_IMAGE_FORMAT).SetMessageParams(extension);
            }
            return isValid;
        }

        /// <summary>Perform OCR for input image.</summary>
        /// <param name="imagesTextData">
        /// Map<File, Map&lt;Integer, List&lt;textinfo>&gt;&gt; -
        /// map that contains input image
        /// files as keys, and as value:
        /// map pageNumber -&gt; text for the page
        /// </param>
        /// <param name="pdfDocument">output pdf document</param>
        /// <param name="defaultFont">default font</param>
        private void AddDataToPdfDocument(IDictionary<FileInfo, IDictionary<int, IList<TextInfo>>> imagesTextData, 
            PdfDocument pdfDocument, PdfFont defaultFont) {
            foreach (KeyValuePair<FileInfo, IDictionary<int, IList<TextInfo>>> entry in imagesTextData) {
                try {
                    FileInfo inputImage = entry.Key;
                    IList<ImageData> imageDataList = GetImageData(inputImage);
                    LOGGER.Info(MessageFormatUtil.Format(LogMessageConstant.NUMBER_OF_PAGES_IN_IMAGE, inputImage.ToString(), imageDataList
                        .Count));
                    IDictionary<int, IList<TextInfo>> imageTextData = entry.Value;
                    if (imageTextData.Keys.Count > 0) {
                        for (int page = 0; page < imageDataList.Count; ++page) {
                            ImageData imageData = imageDataList[page];
                            Rectangle imageSize = UtilService.CalculateImageSize(imageData, GetScaleMode(), GetPageSize());
                            AddToCanvas(pdfDocument, defaultFont, imageSize, imageTextData.Get(page + 1), imageData);
                        }
                    }
                    else {
                        ImageData imageData = imageDataList[0];
                        Rectangle imageSize = UtilService.CalculateImageSize(imageData, GetScaleMode(), GetPageSize());
                        AddToCanvas(pdfDocument, defaultFont, imageSize, new List<TextInfo>(), imageData);
                    }
                }
                catch (System.IO.IOException e) {
                    LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CANNOT_ADD_DATA_TO_PDF_DOCUMENT, e.Message));
                }
            }
        }

        /// <summary>Add image and text to canvas.</summary>
        /// <param name="pdfDocument">PdfDocument</param>
        /// <param name="defaultFont">PdfFont</param>
        /// <param name="imageSize">PageSize</param>
        /// <param name="pageText">List<textinfo></param>
        /// <param name="imageData">ImageData</param>
        internal virtual void AddToCanvas(PdfDocument pdfDocument, PdfFont defaultFont, Rectangle imageSize, IList
            <TextInfo> pageText, ImageData imageData) {
            Rectangle rectangleSize = GetScaleMode() == IPdfRenderer.ScaleMode.keepOriginalSize ? imageSize : GetPageSize
                ();
            PageSize size = new PageSize(rectangleSize);
            PdfPage pdfPage = pdfDocument.AddNewPage(size);
            PdfCanvas canvas = new PdfCanvas(pdfPage);
            PdfLayer imageLayer = new PdfLayer(GetImageLayerName(), pdfDocument);
            PdfLayer textLayer = new PdfLayer(GetTextLayerName(), pdfDocument);
            canvas.BeginLayer(imageLayer);
            AddImageToCanvas(imageData, imageSize, canvas);
            canvas.EndLayer();
            // how much the original image size changed
            float multiplier = imageData == null ? 1 : imageSize.GetWidth() / UtilService.GetPoints(imageData.GetWidth
                ());
            canvas.BeginLayer(textLayer);
            AddTextToCanvas(imageSize, pageText, canvas, defaultFont, multiplier, pdfPage.GetMediaBox());
            canvas.EndLayer();
        }

        /// <summary>Retrieve image data from the file.</summary>
        /// <param name="inputImage">input file</param>
        /// <returns>list of ImageData objects (in case of multipage tiff)</returns>
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
                        LOGGER.Info(MessageFormatUtil.Format(LogMessageConstant.ATTEMPT_TO_CONVERT_TO_PNG, inputImage.FullName, e.
                            Message));
                        try {
                            System.Drawing.Bitmap bufferedImage = null;
                            try {
                                bufferedImage = ImageUtil.ReadImageFromFile(inputImage);
                            }
                            catch (Exception ex) {
                                LOGGER.Info(MessageFormatUtil.Format(LogMessageConstant.READING_IMAGE_AS_PIX, inputImage.FullName, ex.Message
                                    ));
                                bufferedImage = ImageUtil.ReadAsPixAndConvertToBufferedImage(inputImage);
                            }
                            ByteArrayOutputStream baos = new ByteArrayOutputStream();
                            bufferedImage.Save(baos, TesseractUtil.GetPngImageFormat());
                            ImageData imageData = ImageDataFactory.Create(baos.ToArray());
                            images.Add(imageData);
                        }
                        catch (Exception ex) {
                            LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CANNOT_READ_INPUT_IMAGE, ex.Message));
                            throw new OCRException(OCRException.CANNOT_READ_INPUT_IMAGE);
                        }
                    }
                }
            }
            return images;
        }

        /// <summary>Add image to canvas to background.</summary>
        /// <param name="imageData">imageData</param>
        /// <param name="imageSize">calculated size of the image</param>
        /// <param name="pdfCanvas">pdfCanvas</param>
        private void AddImageToCanvas(ImageData imageData, Rectangle imageSize, PdfCanvas pdfCanvas) {
            if (imageData != null) {
                if (GetScaleMode() == IPdfRenderer.ScaleMode.keepOriginalSize) {
                    pdfCanvas.AddImage(imageData, imageSize, false);
                }
                else {
                    IList<float> coordinates = CalculateImageCoordinates(GetPageSize(), imageSize, GetScaleMode());
                    Rectangle rect = new Rectangle(coordinates[0], coordinates[1], imageSize.GetWidth(), imageSize.GetHeight()
                        );
                    pdfCanvas.AddImage(imageData, rect, false);
                }
            }
        }

        /// <summary>Add retrieved text to canvas.</summary>
        /// <param name="imageSize">calculated image size</param>
        /// <param name="data">List<textinfo></param>
        /// <param name="pdfCanvas">PdfCanvas</param>
        /// <param name="defaultFont">PdfFont</param>
        /// <param name="multiplier">how image was scaled</param>
        /// <param name="pageMediaBox">Rectangle</param>
        private void AddTextToCanvas(Rectangle imageSize, IList<TextInfo> data, PdfCanvas pdfCanvas, PdfFont defaultFont
            , float multiplier, Rectangle pageMediaBox) {
            if (data == null || data.Count == 0) {
                pdfCanvas.BeginText().SetFontAndSize(defaultFont, 1);
                pdfCanvas.ShowText("").EndText();
            }
            else {
                IList<float> imageCoordinates = CalculateImageCoordinates(GetPageSize(), imageSize, GetScaleMode());
                float x = imageCoordinates[0];
                float y = imageCoordinates[1];
                foreach (TextInfo item in data) {
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
                        float fontSize = CalculateFontSize(line, defaultFont, bboxHeightPt);
                        float lineWidth = defaultFont.GetWidth(line, fontSize);
                        float deltaX = UtilService.GetPoints(left);
                        float deltaY = imageSize.GetHeight() - UtilService.GetPoints(bottom);
                        float descent = defaultFont.GetDescent(line, fontSize);
                        iText.Layout.Canvas canvas = new iText.Layout.Canvas(pdfCanvas, pageMediaBox);
                        iText.Layout.Element.Text text = new iText.Layout.Element.Text(line).SetHorizontalScaling(bboxWidthPt / lineWidth
                            ).SetBaseDirection(BaseDirection.LEFT_TO_RIGHT);
                        Paragraph paragraph = new Paragraph(text).SetMargin(0).SetMultipliedLeading(1);
                        paragraph.SetFont(defaultFont).SetFontSize(fontSize);
                        paragraph.SetWidth(bboxWidthPt * 1.5f);
                        if (GetTextColor() != null) {
                            paragraph.SetFontColor(GetTextColor());
                        }
                        else {
                            paragraph.SetOpacity(0.0f);
                        }
                        canvas.ShowTextAligned(paragraph, deltaX + x, deltaY + y + descent, TextAlignment.LEFT);
                        canvas.Close();
                    }
                }
            }
        }

        /// <summary>Calculate appropriate font size to fit bbox's width and height.</summary>
        /// <param name="line">text line</param>
        /// <param name="defaultFont">default font</param>
        /// <param name="bboxHeightPt">bbox height</param>
        /// <returns>float</returns>
        private float CalculateFontSize(String line, PdfFont defaultFont, float bboxHeightPt) {
            float fontSize = bboxHeightPt;
            bool textScaled = false;
            float realTextSize;
            while (!textScaled) {
                float ascent = defaultFont.GetAscent(line, fontSize);
                float descent = defaultFont.GetDescent(line, fontSize);
                realTextSize = ascent - descent;
                if (realTextSize - bboxHeightPt <= 0.5) {
                    textScaled = true;
                }
                else {
                    if (realTextSize - bboxHeightPt < 0.5) {
                        fontSize += 0.5f;
                    }
                    else {
                        fontSize -= 0.5f;
                    }
                }
            }
            return fontSize;
        }

        /// <summary>Calculate image coordinates on the page.</summary>
        /// <param name="size">size of the page</param>
        /// <param name="imageSize">calculates size of the image</param>
        /// <param name="pageScaleMode">page scale mode</param>
        /// <returns>Pair<float, float> containing x and y coordinates</returns>
        private IList<float> CalculateImageCoordinates(Rectangle size, Rectangle imageSize, IPdfRenderer.ScaleMode
             pageScaleMode) {
            float x = 0;
            float y = 0;
            if (pageScaleMode != IPdfRenderer.ScaleMode.keepOriginalSize) {
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
