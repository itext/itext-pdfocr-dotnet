using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Ocr {
    /// <summary>
    /// <see cref="IPdfRenderer"/>
    /// interface is used for instantiating
    /// new
    /// <see cref="PdfRenderer"/>
    /// objects.
    /// </summary>
    /// <remarks>
    /// <see cref="IPdfRenderer"/>
    /// interface is used for instantiating
    /// new
    /// <see cref="PdfRenderer"/>
    /// objects.
    /// <see cref="IPdfRenderer"/>
    /// provides possibilities to set list of input images
    /// to be used for OCR, to set scaling mode for images, to set color of text in
    /// output PDF document, to set fixed size of the PDF document and
    /// to perform OCR using given images and to return
    /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
    /// as result.
    /// </remarks>
    public abstract class IPdfRenderer {
        /// <summary>Gets list of provided input images for OCR.</summary>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of given input images
        /// </returns>
        public abstract IList<FileInfo> GetInputImages();

        /// <summary>Sets list of input images for OCR.</summary>
        /// <param name="images">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of input images
        /// </param>
        public abstract void SetInputImages(IList<FileInfo> images);

        /// <summary>Gets text color in output PDF document.</summary>
        /// <returns>
        /// set text
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// </returns>
        public abstract Color GetTextColor();

        /// <summary>Sets text color in output PDF document.</summary>
        /// <param name="color">
        /// required text
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// </param>
        public abstract void SetTextColor(Color color);

        /// <summary>Gets scale mode for input images.</summary>
        /// <returns>
        /// selected
        /// <see cref="ScaleMode"/>
        /// </returns>
        public abstract IPdfRenderer.ScaleMode GetScaleMode();

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
        public abstract void SetScaleMode(IPdfRenderer.ScaleMode scaleMode);

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
        public abstract Rectangle GetPageSize();

        /// <summary>Sets required size for output PDF document.</summary>
        /// <remarks>
        /// Sets required size for output PDF document. Real size of the page will
        /// be calculated according to the selected
        /// <see cref="ScaleMode"/>.
        /// </remarks>
        /// <param name="pageSize">
        /// required page
        /// size as
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// </param>
        public abstract void SetPageSize(Rectangle pageSize);

        /// <summary>Gets name of image layer.</summary>
        /// <returns>
        /// image layer's name as
        /// <see cref="System.String"/>
        /// if it was
        /// manually set, otherwise - the default name ("Image layer")
        /// </returns>
        public abstract String GetImageLayerName();

        /// <summary>Sets name for the image layer.</summary>
        /// <remarks>
        /// Sets name for the image layer.
        /// (by default its name is "Image layer")
        /// </remarks>
        /// <param name="layerName">
        /// name of the image layer
        /// as
        /// <see cref="System.String"/>
        /// </param>
        public abstract void SetImageLayerName(String layerName);

        /// <summary>Gets name of text layer.</summary>
        /// <returns>
        /// text layer's name as
        /// <see cref="System.String"/>
        /// if it was
        /// manually set, otherwise - the default name ("Text layer")
        /// </returns>
        public abstract String GetTextLayerName();

        /// <summary>Sets name for the text layer.</summary>
        /// <remarks>
        /// Sets name for the text layer.
        /// (by default it is "Text layer")
        /// </remarks>
        /// <param name="layerName">
        /// of the text layer as
        /// <see cref="System.String"/>
        /// </param>
        public abstract void SetTextLayerName(String layerName);

        /// <summary>Gets pdf language.</summary>
        /// <returns>
        /// pdf document language as
        /// <see cref="System.String"/>
        /// </returns>
        public abstract String GetPdfLang();

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
        public abstract void SetPdfLang(String language);

        /// <summary>Gets pdf document title.</summary>
        /// <returns>
        /// pdf title as
        /// <see cref="System.String"/>
        /// </returns>
        public abstract String GetTitle();

        /// <summary>Sets pdf document title.</summary>
        /// <param name="title">
        /// pdf title as
        /// <see cref="System.String"/>
        /// </param>
        public abstract void SetTitle(String title);

        /// <summary>Returns path to font to be used in pdf document.</summary>
        /// <returns>path to the required font</returns>
        public abstract String GetFontPath();

        /// <summary>Sets path to font to be used in pdf document.</summary>
        /// <param name="path">path to the required font</param>
        public abstract void SetFontPath(String path);

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
        public abstract PdfDocument DoPdfOcr(PdfWriter pdfWriter, PdfOutputIntent pdfOutputIntent);

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
        public abstract PdfDocument DoPdfOcr(PdfWriter pdfWriter);

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
        public abstract void DoPdfOcr(String path);

        /// <summary>Enumeration of the possible scale modes for input images.</summary>
        public enum ScaleMode {
            /// <summary>
            /// Size of every page of the output PDF document will match the size
            /// of the corresponding input image.
            /// </summary>
            /// <remarks>
            /// Size of every page of the output PDF document will match the size
            /// of the corresponding input image.
            /// (default value)
            /// </remarks>
            KEEP_ORIGINAL_SIZE,
            /// <summary>
            /// Only width of the image will be proportionally scaled to fit
            /// required size that is set using
            /// <see cref="IPdfRenderer.SetPageSize(Rectangle)"/>
            /// method.
            /// </summary>
            SCALE_WIDTH,
            /// <summary>
            /// Only height of the image will be proportionally scaled to fit
            /// required size that is set using
            /// <see cref="IPdfRenderer.SetPageSize(Rectangle)"/>
            /// method.
            /// </summary>
            SCALE_HEIGHT,
            /// <summary>
            /// Size of every page of the output PDF document will match the
            /// values set using
            /// <see cref="IPdfRenderer.SetPageSize(Rectangle)"/>
            /// method.
            /// </summary>
            SCALE_TO_FIT
        }
    }

    public static class IPdfRendererConstants {
    }
}
