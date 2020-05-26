using System;

namespace iText.Pdfocr {
    public class LogMessageConstant {
        public const String CannotReadInputImage = "Cannot read input image {0}";

        public const String CannotReadProvidedFont = "Cannot read given font or it was not provided: {0}";

        public const String CannotReadDefaultFont = "Cannot default read font: {0}";

        public const String CannotWriteToFile = "Cannot write to file {0}: {1}";

        public const String CannotAddDataToPdfDocument = "Cannot add data to pdf document: {1}";

        public const String StartOcrForImages = "Starting ocr for {0} image(s)";

        public const String NumberOfPagesInImage = "Image {0} contains {1} page(s)";

        public const String AttemptToConvertToPng = "Trying to convert {0} to png: {1}";

        public const String ReadingImageAsPix = "Trying to read image {0} as pix: {1}";
    }
}
