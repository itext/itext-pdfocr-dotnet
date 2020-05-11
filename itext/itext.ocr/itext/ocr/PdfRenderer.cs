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
    /// <summary>
    /// The default implementation of
    /// <see cref="IPdfRenderer"/>.
    /// </summary>
    /// <remarks>
    /// The default implementation of
    /// <see cref="IPdfRenderer"/>.
    /// <see cref="IPdfRenderer"/>
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
    public class PdfRenderer : IPdfRenderer {
        /// <summary>Supported image formats.</summary>
        private static readonly ICollection<String> SUPPORTED_IMAGE_FORMATS = JavaCollectionsUtil.UnmodifiableSet(
            new HashSet<String>(JavaUtil.ArraysAsList("bmp", "png", "pnm", "pgm", "ppm", "pbm", "tiff", "tif", "jpeg"
            , "jpg", "jpe", "jfif")));

        /// <summary>The logger.</summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Ocr.PdfRenderer));

        /// <summary>Path to default font file.</summary>
        /// <remarks>
        /// Path to default font file.
        /// "LiberationSans-Regular" by default.
        /// </remarks>
        private const String DEFAULT_FONT_NAME = "LiberationSans-Regular.ttf";

        /// <summary>List of Files with input images.</summary>
        private IList<FileInfo> inputImages = JavaCollectionsUtil.EmptyList<FileInfo>();

        /// <summary>Color of the text in the output PDF document.</summary>
        /// <remarks>
        /// Color of the text in the output PDF document.
        /// Text will be transparent by default.
        /// </remarks>
        private Color textColor = null;

        /// <summary>Scale mode for input images.</summary>
        /// <remarks>
        /// Scale mode for input images.
        /// <see cref="ScaleMode.SCALE_TO_FIT"/>
        /// by default.
        /// </remarks>
        private IPdfRenderer.ScaleMode scaleMode = IPdfRenderer.ScaleMode.SCALE_TO_FIT;

        /// <summary>Size of the PDF document pages.</summary>
        /// <remarks>
        /// Size of the PDF document pages.
        /// "A4" by default.
        /// This parameter is taken into account only if
        /// <see cref="ScaleMode"/>
        /// is
        /// <see cref="ScaleMode.SCALE_WIDTH"/>
        /// ,
        /// <see cref="ScaleMode.SCALE_HEIGHT"/>
        /// or
        /// <see cref="ScaleMode.SCALE_TO_FIT"/>
        /// </remarks>
        private Rectangle pageSize = new Rectangle(PageSize.A4.GetX(), PageSize.A4.GetY(), PageSize.A4.GetWidth(), 
            PageSize.A4.GetHeight());

        /// <summary>Name of the image layer.</summary>
        /// <remarks>
        /// Name of the image layer.
        /// "Image Layer" by default.
        /// </remarks>
        private String imageLayerName = "Image Layer";

        /// <summary>Name of the text layer.</summary>
        /// <remarks>
        /// Name of the text layer.
        /// "Text Layer" by default.
        /// </remarks>
        private String textLayerName = "Text Layer";

        /// <summary>Pdf Language.</summary>
        /// <remarks>
        /// Pdf Language.
        /// "en-US" by default.
        /// </remarks>
        private String pdfLang = "en-US";

        /// <summary>Title of the created document.</summary>
        /// <remarks>
        /// Title of the created document.
        /// It is empty by default.
        /// </remarks>
        private String title = "";

        /// <summary>Path to the used font.</summary>
        /// <remarks>
        /// Path to the used font.
        /// It should be set explicitly or the default font will be used.
        /// </remarks>
        private String fontPath;

        /// <summary>
        /// Selected
        /// <see cref="IOcrReader"/>.
        /// </summary>
        private IOcrReader ocrReader;

        /// <summary>
        /// Creates a new
        /// <see cref="PdfRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="reader">
        /// 
        /// <see cref="IOcrReader"/>
        /// selected OCR Reader
        /// </param>
        public PdfRenderer(IOcrReader reader) {
            ocrReader = reader;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="reader">
        /// selected OCR Reader
        /// <see cref="IOcrReader"/>
        /// </param>
        /// <param name="images">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        public PdfRenderer(IOcrReader reader, IList<FileInfo> images) {
            ocrReader = reader;
            inputImages = JavaCollectionsUtil.UnmodifiableList<FileInfo>(images);
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="reader">
        /// selected OCR Reader
        /// <see cref="IOcrReader"/>
        /// </param>
        /// <param name="images">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="mode">
        /// 
        /// <see cref="ScaleMode"/>
        /// </param>
        public PdfRenderer(IOcrReader reader, IList<FileInfo> images, IPdfRenderer.ScaleMode mode) {
            ocrReader = reader;
            inputImages = JavaCollectionsUtil.UnmodifiableList<FileInfo>(images);
            scaleMode = mode;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="reader">
        /// selected OCR Reader
        /// <see cref="IOcrReader"/>
        /// </param>
        /// <param name="images">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="color">
        /// selected text
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// </param>
        public PdfRenderer(IOcrReader reader, IList<FileInfo> images, Color color) {
            ocrReader = reader;
            inputImages = JavaCollectionsUtil.UnmodifiableList<FileInfo>(images);
            textColor = color;
            scaleMode = IPdfRenderer.ScaleMode.KEEP_ORIGINAL_SIZE;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfRenderer"/>
        /// instance.
        /// </summary>
        /// <param name="reader">
        /// selected OCR Reader
        /// <see cref="IOcrReader"/>
        /// </param>
        /// <param name="images">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="color">
        /// selected text
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// </param>
        /// <param name="mode">
        /// 
        /// <see cref="ScaleMode"/>
        /// for input images
        /// and pdf pages
        /// </param>
        public PdfRenderer(IOcrReader reader, IList<FileInfo> images, Color color, IPdfRenderer.ScaleMode mode) {
            ocrReader = reader;
            textColor = color;
            inputImages = JavaCollectionsUtil.UnmodifiableList<FileInfo>(images);
            scaleMode = mode;
        }

        /// <summary>Gets list of provided input images for OCR.</summary>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </returns>
        public sealed override IList<FileInfo> GetInputImages() {
            return new List<FileInfo>(inputImages);
        }

        /// <summary>Sets list of input images for OCR.</summary>
        /// <param name="images">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        public override void SetInputImages(IList<FileInfo> images) {
            inputImages = JavaCollectionsUtil.UnmodifiableList<FileInfo>(images);
        }

        /// <summary>Gets text color in output PDF document.</summary>
        /// <returns>
        /// set text
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// </returns>
        public sealed override Color GetTextColor() {
            return textColor;
        }

        /// <summary>Sets text color in output PDF document.</summary>
        /// <param name="textColor">
        /// required text
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// </param>
        public sealed override void SetTextColor(Color textColor) {
            this.textColor = textColor;
        }

        /// <summary>Gets scale mode for input images.</summary>
        /// <returns>
        /// selected
        /// <see cref="ScaleMode"/>
        /// </returns>
        public sealed override IPdfRenderer.ScaleMode GetScaleMode() {
            return scaleMode;
        }

        /// <summary>
        /// Sets scale mode for input images using available options
        /// from
        /// <see cref="ScaleMode"/>
        /// enumeration.
        /// </summary>
        /// <param name="scaleMode">
        /// selected
        /// <see cref="ScaleMode"/>
        /// </param>
        public sealed override void SetScaleMode(IPdfRenderer.ScaleMode scaleMode) {
            this.scaleMode = scaleMode;
        }

        /// <summary>Gets required size for output PDF document.</summary>
        /// <remarks>
        /// Gets required size for output PDF document. Real size of the page will
        /// be calculates according to the selected
        /// <see cref="ScaleMode"/>
        /// </remarks>
        /// <returns>
        /// required page size as
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// </returns>
        public sealed override Rectangle GetPageSize() {
            return pageSize;
        }

        /// <summary>Sets required size for output PDF document.</summary>
        /// <remarks>
        /// Sets required size for output PDF document. Real size of the page will be
        /// calculates according to the selected
        /// <see cref="ScaleMode"/>.
        /// </remarks>
        /// <param name="pageSize">
        /// required page
        /// size as
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// </param>
        public sealed override void SetPageSize(Rectangle pageSize) {
            this.pageSize = pageSize;
        }

        /// <summary>Gets name of image layer.</summary>
        /// <returns>
        /// image layer's name as
        /// <see cref="System.String"/>
        /// if it was
        /// manually set, otherwise - the default name ("Image layer")
        /// </returns>
        public sealed override String GetImageLayerName() {
            return imageLayerName;
        }

        /// <summary>Sets name for the image layer.</summary>
        /// <remarks>
        /// Sets name for the image layer.
        /// "Image layer" by default.
        /// </remarks>
        /// <param name="layerName">
        /// name of the image layer
        /// as
        /// <see cref="System.String"/>
        /// </param>
        public sealed override void SetImageLayerName(String layerName) {
            imageLayerName = layerName;
        }

        /// <summary>Gets name of text layer.</summary>
        /// <returns>
        /// text layer's name as
        /// <see cref="System.String"/>
        /// if it was
        /// manually set, otherwise - the default name ("Text layer")
        /// </returns>
        public sealed override String GetTextLayerName() {
            return textLayerName;
        }

        /// <summary>Sets name for the text layer.</summary>
        /// <remarks>
        /// Sets name for the text layer.
        /// "Text layer" by default.
        /// </remarks>
        /// <param name="layerName">
        /// of the text layer as
        /// <see cref="System.String"/>
        /// </param>
        public sealed override void SetTextLayerName(String layerName) {
            textLayerName = layerName;
        }

        /// <summary>Gets pdf language.</summary>
        /// <returns>
        /// pdf document language as
        /// <see cref="System.String"/>
        /// </returns>
        public sealed override String GetPdfLang() {
            return pdfLang;
        }

        /// <summary>Specify pdf natural language, and optionally locale.</summary>
        /// <remarks>
        /// Specify pdf natural language, and optionally locale.
        /// For the content usage dictionary use
        /// <see cref="iText.Kernel.Pdf.PdfName.Language"/>
        /// </remarks>
        /// <param name="language">
        /// pdf document language as
        /// <see cref="System.String"/>
        /// ,
        /// e.g. "en-US", etc.
        /// </param>
        public sealed override void SetPdfLang(String language) {
            pdfLang = language;
        }

        /// <summary>Gets pdf document title.</summary>
        /// <returns>
        /// pdf title as
        /// <see cref="System.String"/>
        /// </returns>
        public sealed override String GetTitle() {
            return title;
        }

        /// <summary>Sets pdf document title.</summary>
        /// <param name="title">
        /// pdf title as
        /// <see cref="System.String"/>
        /// </param>
        public sealed override void SetTitle(String title) {
            this.title = title;
        }

        /// <summary>Returns path to font to be used in pdf document.</summary>
        /// <returns>path to the required font</returns>
        public override String GetFontPath() {
            return fontPath;
        }

        /// <summary>Sets path to font to be used in pdf document.</summary>
        /// <param name="path">path to the required font</param>
        public override void SetFontPath(String path) {
            fontPath = path;
        }

        /// <summary>
        /// Performs OCR with set parameters and create pdf using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>.
        /// </summary>
        /// <remarks>
        /// Performs OCR with set parameters and create pdf using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>
        /// and
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>.
        /// PDF/A-3u document will be created if
        /// provided
        /// <see cref="iText.Kernel.Pdf.PdfOutputIntent"/>
        /// is not null.
        /// </remarks>
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
        public sealed override PdfDocument DoPdfOcr(PdfWriter pdfWriter, PdfOutputIntent pdfOutputIntent) {
            LOGGER.Info(MessageFormatUtil.Format(LogMessageConstant.StartOcrForImages, GetInputImages().Count));
            // map contains:
            // keys: image files
            // values:
            // map pageNumber -> retrieved text data(text and its coordinates)
            IDictionary<FileInfo, IDictionary<int, IList<TextInfo>>> imagesTextData = new LinkedDictionary<FileInfo, IDictionary
                <int, IList<TextInfo>>>();
            foreach (FileInfo inputImage in GetInputImages()) {
                imagesTextData.Put(inputImage, DoOcrForImage(inputImage));
            }
            // create PdfDocument
            return CreatePdfDocument(pdfWriter, pdfOutputIntent, imagesTextData);
        }

        /// <summary>
        /// Performs OCR using provided
        /// <see cref="iText.Kernel.Pdf.PdfWriter"/>.
        /// </summary>
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
        public sealed override PdfDocument DoPdfOcr(PdfWriter pdfWriter) {
            return DoPdfOcr(pdfWriter, null);
        }

        /// <summary>
        /// Performs OCR for the given list of input images and saves output to a
        /// text file using provided path.
        /// </summary>
        /// <param name="path">
        /// path as
        /// <see cref="System.String"/>
        /// to file to be
        /// created
        /// </param>
        public override void DoPdfOcr(String path) {
            LOGGER.Info(MessageFormatUtil.Format(LogMessageConstant.StartOcrForImages, GetInputImages().Count));
            StringBuilder content = new StringBuilder();
            foreach (FileInfo inputImage in GetInputImages()) {
                content.Append(DoOcrForImage(inputImage, IOcrReader.OutputFormat.TXT));
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
        public byte[] GetFont() {
            if (fontPath != null && !String.IsNullOrEmpty(fontPath)) {
                try {
                    return System.IO.File.ReadAllBytes(System.IO.Path.Combine(fontPath));
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
        public byte[] GetDefaultFont() {
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
        internal virtual void AddToCanvas(PdfDocument pdfDocument, PdfFont font, Rectangle imageSize, IList<TextInfo
            > pageText, ImageData imageData) {
            Rectangle rectangleSize = GetScaleMode() == IPdfRenderer.ScaleMode.KEEP_ORIGINAL_SIZE ? imageSize : GetPageSize
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

        /// <summary>
        /// Reads data from input image using selected output format if provided
        /// image has valid extension.
        /// </summary>
        /// <param name="inputImage">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="outputFormat">
        /// 
        /// <see cref="OutputFormat"/>
        /// for the result
        /// returned by
        /// <see cref="IOcrReader"/>
        /// </param>
        /// <returns>
        /// result text data as
        /// <see cref="System.String"/>
        /// </returns>
        private String DoOcrForImage(FileInfo inputImage, IOcrReader.OutputFormat outputFormat) {
            String data = null;
            if (IsValidImageFormat(inputImage)) {
                data = ocrReader.ReadDataFromInput(inputImage, outputFormat);
            }
            return data;
        }

        /// <summary>
        /// Reads data from input image using @link IOcrReader.OutputFormat#HOCR}
        /// output format if provided image has valid extension.
        /// </summary>
        /// <param name="inputImage">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <returns>
        /// result text data as
        /// <see cref="System.String"/>
        /// </returns>
        private IDictionary<int, IList<TextInfo>> DoOcrForImage(FileInfo inputImage) {
            IDictionary<int, IList<TextInfo>> data = new LinkedDictionary<int, IList<TextInfo>>();
            if (IsValidImageFormat(inputImage)) {
                data = ocrReader.ReadDataFromInput(inputImage);
            }
            return data;
        }

        /// <summary>Validates input image format.</summary>
        /// <remarks>
        /// Validates input image format.
        /// Allowed image formats are listed
        /// in
        /// <see cref="SUPPORTED_IMAGE_FORMATS"/>
        /// </remarks>
        /// <param name="image">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <returns>true if image extension is valid, false - if not</returns>
        private bool IsValidImageFormat(FileInfo image) {
            bool isValid = false;
            String extension = "incorrect extension";
            int index = image.FullName.LastIndexOf('.');
            if (index > 0) {
                extension = new String(image.FullName.ToCharArray(), index + 1, image.FullName.Length - index - 1);
                foreach (String format in SUPPORTED_IMAGE_FORMATS) {
                    if (format.Equals(extension.ToLowerInvariant())) {
                        isValid = true;
                        break;
                    }
                }
            }
            if (!isValid) {
                LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CannotReadInputImage, image.FullName));
                throw new OcrException(OcrException.IncorrectInputImageFormat).SetMessageParams(extension);
            }
            return isValid;
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
                            Rectangle imageSize = UtilService.CalculateImageSize(imageData, GetScaleMode(), GetPageSize());
                            AddToCanvas(pdfDocument, font, imageSize, imageTextData.Get(page + 1), imageData);
                        }
                    }
                    else {
                        ImageData imageData = imageDataList[0];
                        Rectangle imageSize = UtilService.CalculateImageSize(imageData, GetScaleMode(), GetPageSize());
                        AddToCanvas(pdfDocument, font, imageSize, new List<TextInfo>(), imageData);
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
                if (GetScaleMode() == IPdfRenderer.ScaleMode.KEEP_ORIGINAL_SIZE) {
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
                pdfCanvas.ShowText("").EndText();
            }
            else {
                IList<float> imageCoordinates = CalculateImageCoordinates(GetPageSize(), imageSize, GetScaleMode());
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
                        float fontSize = CalculateFontSize(line, font, bboxHeightPt);
                        float lineWidth = font.GetWidth(line, fontSize);
                        float deltaX = UtilService.GetPoints(left);
                        float deltaY = imageSize.GetHeight() - UtilService.GetPoints(bottom);
                        float descent = font.GetDescent(line, fontSize);
                        iText.Layout.Canvas canvas = new iText.Layout.Canvas(pdfCanvas, pageMediaBox);
                        iText.Layout.Element.Text text = new iText.Layout.Element.Text(line).SetHorizontalScaling(bboxWidthPt / lineWidth
                            ).SetBaseDirection(BaseDirection.LEFT_TO_RIGHT);
                        Paragraph paragraph = new Paragraph(text).SetMargin(0).SetMultipliedLeading(1);
                        paragraph.SetFont(font).SetFontSize(fontSize);
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

        /// <summary>
        /// Calculates font size according to given bbox height and selected font
        /// parameters.
        /// </summary>
        /// <param name="line">text line</param>
        /// <param name="font">font for the placed text (could be custom or default)</param>
        /// <param name="bboxHeightPt">height of bbox calculated by OCR Reader</param>
        /// <returns>font size</returns>
        private float CalculateFontSize(String line, PdfFont font, float bboxHeightPt) {
            float fontSize = bboxHeightPt;
            bool textScaled = false;
            float realTextSize;
            while (!textScaled) {
                float ascent = font.GetAscent(line, fontSize);
                float descent = font.GetDescent(line, fontSize);
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

        /// <summary>Calculates image coordinates on the page.</summary>
        /// <param name="size">size of the page</param>
        /// <param name="imageSize">size of the image</param>
        /// <param name="pageScaleMode">
        /// selected
        /// <see cref="ScaleMode"/>
        /// </param>
        /// <returns>list of two elements (coordinates): first - x, second - y.</returns>
        private IList<float> CalculateImageCoordinates(Rectangle size, Rectangle imageSize, IPdfRenderer.ScaleMode
             pageScaleMode) {
            float x = 0;
            float y = 0;
            if (pageScaleMode != IPdfRenderer.ScaleMode.KEEP_ORIGINAL_SIZE) {
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
