using System;

namespace iText.Ocr {
    public class LogMessageConstant {
        public const String CannotReadInputImage = "Cannot read input image {0}";

        public const String TesseractFailed = "Tesseract failed: {0}";

        public const String CannotReadProvidedFont = "Cannot read given font or it was not provided: {0}";

        public const String CannotReadDefaultFont = "Cannot default read font: {0}";

        public const String CannotWriteToFile = "Cannot write to file {0}: {1}";

        public const String CannotReadFile = "Cannot read file {0}: {1}";

        public const String CannotOcrInputFile = "Cannot ocr input file: {1}";

        public const String CannotAddDataToPdfDocument = "Cannot add data to pdf document: {1}";

        public const String CannotUseUserWords = "Cannot use custom user words: {0}";

        public const String CannotRetrievePagesFromImage = "Cannot get pages from image {0}: {1}";

        public const String PageNumberIsIncorrect = "Provided number of page ({0}) is incorrect for {1}";

        public const String CannotDeleteFile = "File {0} cannot be deleted: {1}";

        public const String CannotProcessImage = "Cannot process " + "image: {0}";

        /*
        INFO messages
        */
        public const String StartOcrForImages = "Starting ocr for {0} image(s)";

        public const String NumberOfPagesInImage = "Image {0} contains {1} page(s)";

        public const String AttemptToConvertToPng = "Trying to convert {0} to png: {1}";

        public const String ReadingImageAsPix = "Trying to read image {0} as pix: {1}";

        public const String CreatedTemporaryFile = "Created temp file {0}";

        public const String CannotConvertImageToGrayscale = "Cannot convert to gray image with depth {0}";

        public const String CannotBinarizeImage = "Cannot binarize image with depth {0}";

        public const String CannotCreateBufferedImage = "Cannot create a buffered image from the input image: {0}";
    }
}
