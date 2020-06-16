using iText.Kernel.Geom;

namespace iText.Pdfocr {
    /// <summary>Enumeration of the possible scale modes for input images.</summary>
    public enum ScaleMode {
        /// <summary>
        /// Only width of the image will be proportionally scaled to fit
        /// required size that is set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method.
        /// </summary>
        /// <remarks>
        /// Only width of the image will be proportionally scaled to fit
        /// required size that is set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method.
        /// Height will be equal to the page height that was set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method and
        /// width will be proportionally scaled to keep the original aspect ratio.
        /// </remarks>
        SCALE_WIDTH,
        /// <summary>
        /// Only height of the image will be proportionally scaled to fit
        /// required size that is set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method.
        /// </summary>
        /// <remarks>
        /// Only height of the image will be proportionally scaled to fit
        /// required size that is set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method.
        /// Width will be equal to the page width that was set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method and
        /// height will be proportionally scaled to keep the original aspect ratio.
        /// </remarks>
        SCALE_HEIGHT,
        /// <summary>
        /// The image will be scaled to fit within the page width and height dimensions that are set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method.
        /// </summary>
        /// <remarks>
        /// The image will be scaled to fit within the page width and height dimensions that are set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method.
        /// Original aspect ratio of the image stays unchanged.
        /// </remarks>
        SCALE_TO_FIT
    }
}
