using System;
using iText.Pdfocr;

namespace iText.Pdfocr.Tesseract4 {
    public class Tesseract4OcrException : OcrException {
        public const String TESSERACT_LIB_NOT_INSTALLED_WIN = "Tesseract failed. "
             + "Please ensure you have at least Visual C++ 2015 Redistributable installed";

        public const String INCORRECT_INPUT_IMAGE_FORMAT = "{0} format is not supported.";

        public const String INCORRECT_LANGUAGE = "{0} does not exist in {1}";

        public const String LANGUAGE_IS_NOT_IN_THE_LIST = "Provided list of languages doesn't contain {0} language";

        public const String CANNOT_READ_PROVIDED_IMAGE = "Cannot read input image {0}";

        public const String TESSERACT_FAILED = "Tesseract failed. " + "Please check provided parameters";

        public const String TESSERACT_LIB_NOT_INSTALLED = "Tesseract failed. " + "Please ensure you have tesseract library installed";

        public const String TESSERACT_NOT_FOUND = "Tesseract failed. " + "Please check that tesseract is installed and provided path to "
             + "tesseract executable directory is correct";

        public const String CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE = "Cannot find path to tesseract executable.";

        public const String PATH_TO_TESS_DATA_DIRECTORY_IS_INVALID = "Provided path to tess data directory does not exist or it is "
             + "an invalid directory";

        public const String PATH_TO_TESS_DATA_IS_NOT_SET = "Path to tess data directory cannot be null and must be set "
             + "to a valid directory";

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
