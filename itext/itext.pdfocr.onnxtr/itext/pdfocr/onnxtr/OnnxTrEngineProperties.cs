namespace iText.Pdfocr.Onnxtr {
    /// <summary>
    /// Properties that are used by the
    /// <see cref="OnnxTrOcrEngine"/>.
    /// </summary>
    public class OnnxTrEngineProperties {
        /// <summary>
        /// Creates a new
        /// <see cref="OnnxTrEngineProperties"/>
        /// instance.
        /// </summary>
        public OnnxTrEngineProperties() {
        }

        /// <summary>Defines the way text is retrieved and grouped from onnxtr engine output.</summary>
        /// <remarks>
        /// Defines the way text is retrieved and grouped from onnxtr engine output.
        /// It changes the way text is selected in the result pdf document.
        /// Does not affect the result of
        /// <see cref="iText.Pdfocr.IOcrEngine.CreateTxtFile(System.Collections.Generic.IList{E}, System.IO.FileInfo)"
        ///     />.
        /// </remarks>
        private TextPositioning textPositioning;

        /// <summary>
        /// Defines the way text is retrieved from ocr engine output using
        /// <see cref="TextPositioning"/>.
        /// </summary>
        /// <returns>the way text is retrieved</returns>
        public virtual TextPositioning GetTextPositioning() {
            return textPositioning;
        }

        /// <summary>
        /// Defines the way text is retrieved from ocr engine output
        /// using
        /// <see cref="TextPositioning"/>.
        /// </summary>
        /// <param name="textPositioning">the way text is retrieved</param>
        /// <returns>
        /// the
        /// <see cref="OnnxTrEngineProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Pdfocr.Onnxtr.OnnxTrEngineProperties SetTextPositioning(TextPositioning textPositioning
            ) {
            this.textPositioning = textPositioning;
            return this;
        }
    }
}
