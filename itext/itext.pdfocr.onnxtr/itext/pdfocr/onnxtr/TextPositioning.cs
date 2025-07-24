namespace iText.Pdfocr.Onnxtr {
    /// <summary>Enumeration of the possible types of text positioning.</summary>
    /// <remarks>
    /// Enumeration of the possible types of text positioning.
    /// It is used to combine the
    /// <see cref="OnnxTrOcrEngine"/>
    /// image OCR result text and group it by lines or by words.
    /// </remarks>
    public enum TextPositioning {
        /// <summary>Text will be grouped by lines.</summary>
        /// <remarks>
        /// Text will be grouped by lines.
        /// (default value)
        /// </remarks>
        BY_LINES,
        /// <summary>Text will be grouped by words.</summary>
        BY_WORDS
    }
}
