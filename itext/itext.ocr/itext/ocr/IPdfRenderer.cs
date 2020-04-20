using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;

namespace iText.Ocr {
    /// <summary>Interface for PDF Renderer classes.</summary>
    /// <remarks>
    /// Interface for PDF Renderer classes.
    /// <para />
    /// The IPdfRenderer provides possibilities to set list of input images
    /// to be used for OCR, to set scaling mode for images, color of text in output
    /// PDF document,  set fixed size of the PDF document
    /// and to perform OCR using given images and return PDFDocument as result
    /// </remarks>
    public abstract class IPdfRenderer {
        public enum ScaleMode {
            /// <summary>keepOriginalSize (default value).</summary>
            /// <remarks>
            /// keepOriginalSize (default value).
            /// <para />
            /// the size of every page of
            /// the output PDF document will match the size of the
            /// corresponding input image
            /// </remarks>
            keepOriginalSize,
            /// <summary>scaleWidth.</summary>
            /// <remarks>
            /// scaleWidth.
            /// <para />
            /// Only width of the image will be proportionally scaled
            /// </remarks>
            scaleWidth,
            /// <summary>scaleHeight.</summary>
            /// <remarks>
            /// scaleHeight.
            /// <para />
            /// Only height of the image will be proportionally scaled
            /// </remarks>
            scaleHeight,
            /// <summary>scaleToFit.</summary>
            /// <remarks>
            /// scaleToFit.
            /// <para />
            /// the size of every page of the output PDF document
            /// will match the values set using "setPdfSize()" method
            /// </remarks>
            scaleToFit
        }

        /// <summary>Supported image formats.</summary>
        public enum ImgFormat {
            /// <summary>BMP file format.</summary>
            bmp,
            /// <summary>PNG file format.</summary>
            png,
            /// <summary>PNM file format.</summary>
            pnm,
            /// <summary>PGM file format.</summary>
            pgm,
            /// <summary>PPM file format.</summary>
            ppm,
            /// <summary>PBM file format.</summary>
            pbm,
            /// <summary>TIFF file format.</summary>
            tiff,
            /// <summary>TIF file format.</summary>
            tif,
            /// <summary>JPEG file format.</summary>
            jpeg,
            /// <summary>JPG file format.</summary>
            jpg,
            /// <summary>JPE file format.</summary>
            jpe,
            /// <summary>JFIF file format.</summary>
            jfif
        }

        /// <summary>Set list of input images for OCR.</summary>
        /// <param name="images">
        /// a
        /// <see cref="System.Collections.IList{E}"/>
        /// object.
        /// </param>
        public abstract void SetInputImages(IList<FileInfo> images);

        /// <summary>Get list of provided input images for OCR.</summary>
        /// <returns>
        /// a
        /// <see cref="System.Collections.IList{E}"/>
        /// object.
        /// </returns>
        public abstract IList<FileInfo> GetInputImages();

        /// <summary>Set text color (should be CMYK) in output PDF document.</summary>
        /// <param name="newColor">
        /// a
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// object.
        /// </param>
        public abstract void SetTextColor(Color newColor);

        /// <summary>Get text color in output PDF document.</summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Colors.Color"/>
        /// object.
        /// </returns>
        public abstract Color GetTextColor();

        /// <summary>
        /// Set scale mode for input images using available options
        /// from ScaleMode enum.
        /// </summary>
        /// <param name="mode">
        /// a
        /// <see cref="ScaleMode"/>
        /// object.
        /// </param>
        public abstract void SetScaleMode(IPdfRenderer.ScaleMode mode);

        /// <summary>Get scale mode for input images.</summary>
        /// <returns>
        /// a
        /// <see cref="ScaleMode"/>
        /// object.
        /// </returns>
        public abstract IPdfRenderer.ScaleMode GetScaleMode();

        /// <summary>Set fixed size for output PDF document.</summary>
        /// <remarks>
        /// Set fixed size for output PDF document.
        /// (this parameter is used only is ScaleMode is set as "fitToSize")
        /// </remarks>
        /// <param name="pdfSize">
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object.
        /// </param>
        public abstract void SetPageSize(Rectangle pdfSize);

        /// <summary>Get size for output document.</summary>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// object.
        /// </returns>
        public abstract Rectangle GetPageSize();

        /// <summary>Set name for the image layer.</summary>
        /// <remarks>
        /// Set name for the image layer.
        /// (of by default it is "Image layer")
        /// </remarks>
        /// <param name="name">layer's name</param>
        public abstract void SetImageLayerName(String name);

        /// <summary>Get name of image layer.</summary>
        /// <returns>
        /// layer's name that was manually set or
        /// the default one (="Image layer")
        /// </returns>
        public abstract String GetImageLayerName();

        /// <summary>Set name for the text layer.</summary>
        /// <remarks>
        /// Set name for the text layer.
        /// (of by default it is "Text layer")
        /// </remarks>
        /// <param name="name">layer's name</param>
        public abstract void SetTextLayerName(String name);

        /// <returns>
        /// layer's name that was manually set or
        /// the default one (="Text layer")
        /// </returns>
        public abstract String GetTextLayerName();

        /// <summary>Specify pdf natural language, and optionally locale.</summary>
        /// <param name="lang">String</param>
        public abstract void SetPdfLang(String lang);

        /// <returns>pdf document lang</returns>
        public abstract String GetPdfLang();

        /// <summary>Set pdf document title.</summary>
        /// <param name="name">String</param>
        public abstract void SetTitle(String name);

        /// <returns>pdf document title</returns>
        public abstract String GetTitle();

        /// <summary>Set path to font to be used in pdf document.</summary>
        /// <param name="name">String</param>
        public abstract void SetFontPath(String name);

        /// <returns>path to font</returns>
        public abstract String GetFontPath();

        /// <summary>Perform OCR using provided pdfWriter and pdfOutputIntent.</summary>
        /// <remarks>
        /// Perform OCR using provided pdfWriter and pdfOutputIntent.
        /// PDF/A-3u document will be created if pdfOutputIntent is not null
        /// </remarks>
        /// <param name="pdfWriter">PdfWriter</param>
        /// <param name="pdfOutputIntent">PdfOutputIntent</param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object - PDF/A-3u document
        /// </returns>
        public abstract PdfDocument DoPdfOcr(PdfWriter pdfWriter, PdfOutputIntent pdfOutputIntent);

        /// <summary>Perform OCR using provided pdfWriter.</summary>
        /// <param name="pdfWriter">provided pdfWriter</param>
        /// <returns>
        /// a
        /// <see cref="iText.Kernel.Pdf.PdfDocument"/>
        /// object
        /// </returns>
        public abstract PdfDocument DoPdfOcr(PdfWriter pdfWriter);

        /// <summary>Perform OCR using provided pdfWriter.</summary>
        /// <param name="path">path to text file to be created</param>
        public abstract void DoPdfOcr(String path);
    }

    public static class IPdfRendererConstants {
    }
}
