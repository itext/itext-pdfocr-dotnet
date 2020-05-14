using iText.Kernel.Geom;

namespace iText.Ocr {
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
        /// Height will be equal to the page width that was set using
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
        /// Size of every page of the output PDF document will match the
        /// values set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method.
        /// </summary>
        SCALE_TO_FIT
    }
}
