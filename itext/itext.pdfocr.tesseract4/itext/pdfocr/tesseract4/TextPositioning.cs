namespace iText.Pdfocr.Tesseract4 {
    /// <summary>Enumeration of the possible types of text positioning.</summary>
    /// <remarks>
    /// Enumeration of the possible types of text positioning.
    /// It is used when there is possibility in selected Reader to process
    /// the text by lines or by words and to return coordinates for the
    /// selected type of item.
    /// For tesseract this value makes sense only if selected
    /// <see cref="OutputFormat"/>
    /// is
    /// <see cref="OutputFormat.HOCR"/>.
    /// </remarks>
    public enum TextPositioning {
        /// <summary>Text will be located by lines retrieved from hocr file.</summary>
        /// <remarks>
        /// Text will be located by lines retrieved from hocr file.
        /// (default value)
        /// </remarks>
        BY_LINES,
        /// <summary>Text will be located by words retrieved from hocr file.</summary>
        BY_WORDS
    }
}
