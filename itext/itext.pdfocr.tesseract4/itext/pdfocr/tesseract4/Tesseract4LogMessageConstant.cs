using System;
using iText.Pdfocr;

namespace iText.Pdfocr.Tesseract4 {
    public class Tesseract4LogMessageConstant : LogMessageConstant {
        public const String TesseractFailed = "Tesseract failed: {0}";

        public const String CannotReadFile = "Cannot read file {0}: {1}";

        public const String CannotOcrInputFile = "Cannot ocr input file: {1}";

        public const String CannotUseUserWords = "Cannot use custom user words: {0}";

        public const String CannotRetrievePagesFromImage = "Cannot get pages from image {0}: {1}";

        public const String PageNumberIsIncorrect = "Provided number of page ({0}) is incorrect for {1}";

        public const String CannotDeleteFile = "File {0} cannot be deleted: {1}";

        public const String CannotProcessImage = "Cannot process " + "image: {0}";

        /*
        INFO messages
        */
        public const String CreatedTemporaryFile = "Created temp file {0}";

        public const String CannotConvertImageToGrayscale = "Cannot convert to gray image with depth {0}";

        public const String CannotBinarizeImage = "Cannot binarize image with depth {0}";

        public const String CannotCreateBufferedImage = "Cannot create a buffered image from the input image: {0}";
    }
}
