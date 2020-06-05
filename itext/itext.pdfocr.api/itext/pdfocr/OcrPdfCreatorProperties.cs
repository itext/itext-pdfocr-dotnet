using System;
using iText.Kernel.Colors;
using iText.Kernel.Geom;

namespace iText.Pdfocr {
    /// <summary>
    /// Properties that will be used by the
    /// <see cref="OcrPdfCreator"/>.
    /// </summary>
    public class OcrPdfCreatorProperties {
        /// <summary>Path to the default font.</summary>
        private const String DEFAULT_FONT_PATH = "iText.Pdfocr.Api.font.LiberationSans-Regular.ttf";

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
        /// by default. But this value will be used
        /// only if
        /// <see cref="pageSize"/>
        /// is not null;
        /// </remarks>
        private ScaleMode scaleMode = ScaleMode.SCALE_TO_FIT;

        /// <summary>Size of the PDF document pages.</summary>
        /// <remarks>
        /// Size of the PDF document pages.
        /// <c>null</c> by default.
        /// If this parameter is null, size of the page will be equal to the
        /// input image size. If this parameter is not null, input image will be
        /// scaled according to the selected
        /// <see cref="ScaleMode"/>.
        /// </remarks>
        private Rectangle pageSize = null;

        /// <summary>Name of the image layer.</summary>
        /// <remarks>
        /// Name of the image layer.
        /// <c>null</c> by default.
        /// If this parameter is null then image is placed directly in canvas instead of layer.
        /// If value of imageLayerName is equal to value of textLayerName then image and text placed in one layer.
        /// </remarks>
        private String imageLayerName = null;

        /// <summary>Name of the text layer.</summary>
        /// <remarks>
        /// Name of the text layer.
        /// <c>null</c> by default.
        /// If this parameter is null then text is placed directly in canvas instead of layer.
        /// If value of textLayerName is equal to value of imageLayerName then text and image placed in one layer.
        /// </remarks>
        private String textLayerName = null;

        /// <summary>PDF Language.</summary>
        /// <remarks>
        /// PDF Language.
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
        /// Creates a new
        /// <see cref="OcrPdfCreatorProperties"/>
        /// instance.
        /// </summary>
        public OcrPdfCreatorProperties() {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="OcrPdfCreatorProperties"/>
        /// instance
        /// based on another
        /// <see cref="OcrPdfCreatorProperties"/>
        /// instance (copy
        /// constructor).
        /// </summary>
        /// <param name="other">
        /// the other
        /// <see cref="OcrPdfCreatorProperties"/>
        /// instance
        /// </param>
        public OcrPdfCreatorProperties(iText.Pdfocr.OcrPdfCreatorProperties other) {
            this.scaleMode = other.scaleMode;
            this.pageSize = other.pageSize;
            this.imageLayerName = other.imageLayerName;
            this.textLayerName = other.textLayerName;
            this.textColor = other.textColor;
            this.pdfLang = other.pdfLang;
            this.title = other.title;
            this.fontPath = other.fontPath;
        }

        /// <summary>Gets text color in output PDF document.</summary>
        /// <returns>
        /// set text
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// </returns>
        public Color GetTextColor() {
            return textColor;
        }

        /// <summary>Sets text color in output PDF document.</summary>
        /// <param name="textColor">
        /// required text
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// </param>
        /// <returns>
        /// the
        /// <see cref="OcrPdfCreatorProperties"/>
        /// instance
        /// </returns>
        public iText.Pdfocr.OcrPdfCreatorProperties SetTextColor(Color textColor) {
            this.textColor = textColor;
            return this;
        }

        /// <summary>
        /// Gets scale mode for input images using available options from
        /// <see cref="ScaleMode"/>
        /// enumeration.
        /// </summary>
        /// <returns>
        /// selected
        /// <see cref="ScaleMode"/>
        /// </returns>
        public ScaleMode GetScaleMode() {
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
        /// <returns>
        /// the
        /// <see cref="OcrPdfCreatorProperties"/>
        /// instance
        /// </returns>
        public iText.Pdfocr.OcrPdfCreatorProperties SetScaleMode(ScaleMode scaleMode) {
            this.scaleMode = scaleMode;
            return this;
        }

        /// <summary>Gets required size for output PDF document.</summary>
        /// <remarks>
        /// Gets required size for output PDF document. Real size of the page will
        /// be calculated according to the selected
        /// <see cref="ScaleMode"/>
        /// </remarks>
        /// <returns>
        /// required page size as
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// </returns>
        public Rectangle GetPageSize() {
            return pageSize;
        }

        /// <summary>Sets required size for output PDF document.</summary>
        /// <remarks>
        /// Sets required size for output PDF document. Real size of the page will
        /// be calculated according to the selected
        /// <see cref="ScaleMode"/>.
        /// </remarks>
        /// <param name="pageSize">
        /// requested page
        /// size as
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// </param>
        /// <returns>
        /// the
        /// <see cref="OcrPdfCreatorProperties"/>
        /// instance
        /// </returns>
        public iText.Pdfocr.OcrPdfCreatorProperties SetPageSize(Rectangle pageSize) {
            this.pageSize = pageSize;
            return this;
        }

        /// <summary>Gets name of image layer.</summary>
        /// <returns>
        /// image layer's name as
        /// <see cref="System.String"/>
        /// if it was
        /// manually set, otherwise - <c>null</c>
        /// </returns>
        public String GetImageLayerName() {
            return imageLayerName;
        }

        /// <summary>Sets name for the image layer.</summary>
        /// <remarks>
        /// Sets name for the image layer.
        /// <c>null</c> by default.
        /// If null then image is placed directly in canvas instead of layer.
        /// If image layer name is equal to text layer name then text and image placed in one layer.
        /// </remarks>
        /// <param name="layerName">
        /// name of the image layer
        /// as
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// the
        /// <see cref="OcrPdfCreatorProperties"/>
        /// instance
        /// </returns>
        public iText.Pdfocr.OcrPdfCreatorProperties SetImageLayerName(String layerName) {
            imageLayerName = layerName;
            return this;
        }

        /// <summary>Gets name of text layer.</summary>
        /// <returns>
        /// text layer's name as
        /// <see cref="System.String"/>
        /// if it was
        /// manually set, otherwise - <c>null</c>
        /// </returns>
        public String GetTextLayerName() {
            return textLayerName;
        }

        /// <summary>Sets name for the text layer.</summary>
        /// <remarks>
        /// Sets name for the text layer.
        /// <c>null</c> by default.
        /// If null then text is placed directly in canvas instead of layer.
        /// If text layer name is equal to image layer name then text and image placed in one layer.
        /// </remarks>
        /// <param name="layerName">
        /// of the text layer as
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// the
        /// <see cref="OcrPdfCreatorProperties"/>
        /// instance
        /// </returns>
        public iText.Pdfocr.OcrPdfCreatorProperties SetTextLayerName(String layerName) {
            textLayerName = layerName;
            return this;
        }

        /// <summary>Gets PDF language.</summary>
        /// <returns>
        /// PDF document language as
        /// <see cref="System.String"/>
        /// </returns>
        public String GetPdfLang() {
            return pdfLang;
        }

        /// <summary>Specify PDF natural language, and optionally locale.</summary>
        /// <remarks>
        /// Specify PDF natural language, and optionally locale.
        /// For the content usage dictionary use
        /// <see cref="iText.Kernel.Pdf.PdfName.Language"/>
        /// </remarks>
        /// <param name="language">
        /// PDF document language as
        /// <see cref="System.String"/>
        /// ,
        /// e.g. "en-US", etc.
        /// </param>
        /// <returns>
        /// the
        /// <see cref="OcrPdfCreatorProperties"/>
        /// instance
        /// </returns>
        public iText.Pdfocr.OcrPdfCreatorProperties SetPdfLang(String language) {
            pdfLang = language;
            return this;
        }

        /// <summary>Gets PDF document title.</summary>
        /// <returns>
        /// PDF title as
        /// <see cref="System.String"/>
        /// </returns>
        public String GetTitle() {
            return title;
        }

        /// <summary>Sets PDF document title.</summary>
        /// <param name="title">
        /// PDF title as
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// the
        /// <see cref="OcrPdfCreatorProperties"/>
        /// instance
        /// </returns>
        public iText.Pdfocr.OcrPdfCreatorProperties SetTitle(String title) {
            this.title = title;
            return this;
        }

        /// <summary>Returns path to the font to be used in PDF document.</summary>
        /// <returns>path to the required font</returns>
        public virtual String GetFontPath() {
            return fontPath;
        }

        /// <summary>Sets path to the font to be used in PDF document.</summary>
        /// <param name="path">path to the required font</param>
        /// <returns>
        /// the
        /// <see cref="OcrPdfCreatorProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Pdfocr.OcrPdfCreatorProperties SetFontPath(String path) {
            fontPath = path;
            return this;
        }

        /// <summary>Gets path to the default font.</summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// path to default font
        /// </returns>
        public virtual String GetDefaultFontName() {
            return iText.Pdfocr.OcrPdfCreatorProperties.DEFAULT_FONT_PATH;
        }
    }
}
