using System;

namespace iText.Ocr {
    /// <summary>Exception class for custom exceptions.</summary>
    public class OCRException : iText.IO.IOException {
        public const String INCORRECT_INPUT_IMAGE_FORMAT = "{0} format is not supported.";

        public const String INCORRECT_LANGUAGE = "{0} does not exist in {1}";

        public const String LANGUAGE_IS_NOT_IN_THE_LIST = "Provided list of languages doesn't contain {0} language";

        public const String CANNOT_READ_INPUT_IMAGE = "Cannot read input image";

        public const String CANNOT_READ_FONT = "Cannot read font";

        public const String TESSERACT_FAILED = "Tesseract failed. " + "Please check provided parameters";

        public const String TESSERACT_FAILED_WITH_REASON = "Tesseract " + "failed. {0}";

        public const String CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE = "Cannot find path to tesseract executable.";

        public const String CANNOT_FIND_PATH_TO_TESSDATA = "Cannot find path to tess data directory";

        public OCRException(String msg, Exception e)
            : base(msg, e) {
        }

        public OCRException(String msg)
            : base(msg) {
        }
    }
}
