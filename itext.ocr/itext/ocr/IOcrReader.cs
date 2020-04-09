using System;
using System.Collections.Generic;
using System.IO;

namespace iText.Ocr {
    /// <summary>Interface for OcrReader classes.</summary>
    /// <remarks>
    /// Interface for OcrReader classes.
    /// <para />
    /// IOcrReader interface provides possibility to perform OCR actions,
    /// read data from input files and return contained text in the described format
    /// </remarks>
    public abstract class IOcrReader {
        public enum TextPositioning {
            /// <summary>byLines (default value).</summary>
            /// <remarks>
            /// byLines (default value).
            /// <para />
            /// text will be located by lines retrieved from hocr file
            /// </remarks>
            byLines,
            /// <summary>byWords.</summary>
            /// <remarks>
            /// byWords.
            /// <para />
            /// text will be located by words retrieved from hocr file
            /// </remarks>
            byWords
        }

        public enum OutputFormat {
            /// <summary>hocr.</summary>
            /// <remarks>
            /// hocr.
            /// <para />
            /// Reader will produce XHTML output compliant
            /// with the hOCR specification.
            /// Output will be parsed and represented as List<textinfo>
            /// </remarks>
            hocr,
            /// <summary>txt.</summary>
            /// <remarks>
            /// txt.
            /// <para />
            /// Reader will produce plain txt file
            /// </remarks>
            txt
        }

        /// <summary>
        /// Reads data from the provided input image file and returns retrieved data
        /// in the following format:
        /// </summary>
        /// <remarks>
        /// Reads data from the provided input image file and returns retrieved data
        /// in the following format:
        /// <para />
        /// Map<Integer, List&lt;textinfo>&gt;:
        /// key: number of the page,
        /// value: list of TextInfo elements where each list TextInfo element contains word
        /// or line and its 4 coordinates(bbox).
        /// (There will be parsed result in hOCR format produced by reader)
        /// </remarks>
        /// <param name="input">input file</param>
        /// <returns>Map<Integer, List&lt;textinfo>&gt;</returns>
        public abstract IDictionary<int, IList<TextInfo>> ReadDataFromInput(FileInfo input);

        /// <summary>
        /// Reads data from the provided input image file and returns retrieved data
        /// as string.
        /// </summary>
        /// <param name="input">input file</param>
        /// <param name="outputFormat">OutputFormat</param>
        /// <returns>List<textinfo></returns>
        public abstract String ReadDataFromInput(FileInfo input, IOcrReader.OutputFormat outputFormat);

        /// <summary>
        /// Reads data from input stream and returns retrieved data
        /// in the following format:
        /// </summary>
        /// <remarks>
        /// Reads data from input stream and returns retrieved data
        /// in the following format:
        /// <para />
        /// List<textinfo> where each list TextInfo element contains word
        /// or line and its 4 coordinates(bbox).
        /// </remarks>
        /// <param name="is">InputStream</param>
        /// <param name="outputFormat">OutputFormat</param>
        /// <returns>List<textinfo></returns>
        public abstract IList<TextInfo> ReadDataFromInput(Stream @is, IOcrReader.OutputFormat outputFormat);
    }

    public static class IOcrReaderConstants {
    }
}
