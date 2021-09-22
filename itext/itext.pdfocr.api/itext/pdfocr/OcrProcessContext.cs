namespace iText.Pdfocr {
    /// <summary>Class for storing ocr processing context.</summary>
    public class OcrProcessContext {
        private readonly AbstractPdfOcrEventHelper ocrEventHelper;

        /// <summary>Creates an instance of ocr process context</summary>
        /// <param name="eventHelper">helper class for working with events</param>
        public OcrProcessContext(AbstractPdfOcrEventHelper eventHelper) {
            this.ocrEventHelper = eventHelper;
        }

        /// <summary>Returns helper for working with events.</summary>
        /// <returns>
        /// an instance of
        /// <see cref="AbstractPdfOcrEventHelper"/>
        /// </returns>
        public virtual AbstractPdfOcrEventHelper GetOcrEventHelper() {
            return ocrEventHelper;
        }
    }
}
