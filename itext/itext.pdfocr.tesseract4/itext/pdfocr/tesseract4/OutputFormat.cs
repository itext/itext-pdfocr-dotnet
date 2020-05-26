using System.Collections;
using iText.Pdfocr;

namespace iText.Pdfocr.Tesseract4 {
    /// <summary>Enumeration of the available output formats.</summary>
    /// <remarks>
    /// Enumeration of the available output formats.
    /// It is used when there is possibility in selected Reader to process input
    /// file and to return result in the required output format.
    /// </remarks>
    public enum OutputFormat {
        /// <summary>
        /// Reader will produce XHTML output compliant
        /// with the hOCR specification.
        /// </summary>
        /// <remarks>
        /// Reader will produce XHTML output compliant
        /// with the hOCR specification.
        /// Output will be parsed and represented as
        /// <see cref="IList{E}"/>
        /// of
        /// <see cref="TextInfo"/>
        /// objects
        /// </remarks>
        HOCR,
        /// <summary>Reader will produce plain txt file.</summary>
        TXT
    }
}
