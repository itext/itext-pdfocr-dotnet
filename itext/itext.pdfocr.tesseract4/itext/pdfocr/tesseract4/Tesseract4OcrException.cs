using System;
using iText.Pdfocr;

namespace iText.Pdfocr.Tesseract4 {
    public class Tesseract4OcrException : OcrException {
        public const String IncorrectInputImageFormat = "{0} format is not supported.";

        public const String IncorrectLanguage = "{0} does not exist in {1}";

        public const String LanguageIsNotInTheList = "Provided list of languages doesn't contain {0} language";

        public const String CannotReadProvidedImage = "Cannot read input image {0}";

        public const String TesseractFailed = "Tesseract failed. " + "Please check provided parameters";

        public const String CannotFindPathToTesseractExecutable = "Cannot find path to tesseract executable.";

        public const String CannotFindPathToTessDataDirectory = "Cannot find path to tess data directory";

        /// <summary>Creates a new TesseractException.</summary>
        /// <param name="msg">the detail message.</param>
        /// <param name="e">
        /// the cause
        /// (which is saved for later retrieval
        /// by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public Tesseract4OcrException(String msg, Exception e)
            : base(msg, e) {
        }

        /// <summary>Creates a new TesseractException.</summary>
        /// <param name="msg">the detail message.</param>
        public Tesseract4OcrException(String msg)
            : base(msg) {
        }
    }
}
