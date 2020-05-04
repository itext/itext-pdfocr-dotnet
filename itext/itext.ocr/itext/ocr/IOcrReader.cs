using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace iText.Ocr {
    /// <summary>Interface for OcrReader classes.</summary>
    /// <remarks>
    /// Interface for OcrReader classes.
    /// IOcrReader interface provides possibility to perform OCR actions,
    /// read data from input files and return contained text in the described format
    /// </remarks>
    public abstract class IOcrReader {
        /// <summary>Enum describing possible types of text positioning.</summary>
        /// <remarks>
        /// Enum describing possible types of text positioning.
        /// <see cref="BY_LINES"/>
        /// <see cref="BY_WORDS"/>
        /// </remarks>
        public enum TextPositioning {
            /// <summary>BY_LINES (default value).</summary>
            /// <remarks>
            /// BY_LINES (default value).
            /// text will be located by lines retrieved from hocr file
            /// </remarks>
            BY_LINES,
            /// <summary>BY_WORDS.</summary>
            /// <remarks>
            /// BY_WORDS.
            /// text will be located by words retrieved from hocr file
            /// </remarks>
            BY_WORDS
        }

        /// <summary>Enum describing available output formats.</summary>
        /// <remarks>
        /// Enum describing available output formats.
        /// <see cref="TXT"/>
        /// <see cref="HOCR"/>
        /// </remarks>
        public enum OutputFormat {
            /// <summary>HOCR.</summary>
            /// <remarks>
            /// HOCR.
            /// Reader will produce XHTML output compliant
            /// with the hOCR specification.
            /// Output will be parsed and represented as
            /// <see cref="IList{E}"/>
            /// </remarks>
            HOCR,
            /// <summary>TXT.</summary>
            /// <remarks>
            /// TXT.
            /// Reader will produce plain txt file
            /// </remarks>
            TXT
        }

        /// <summary>
        /// Reads data from the provided input image file and returns retrieved data
        /// in the following format:
        /// Map<Integer, List&lt;textinfo>&gt;:
        /// key: number of the page,
        /// value: list of
        /// <see cref="TextInfo"/>
        /// elements where
        /// each
        /// <see cref="TextInfo"/>
        /// element contains a word or a line
        /// and its 4 coordinates(bbox).
        /// </summary>
        /// <remarks>
        /// Reads data from the provided input image file and returns retrieved data
        /// in the following format:
        /// Map<Integer, List&lt;textinfo>&gt;:
        /// key: number of the page,
        /// value: list of
        /// <see cref="TextInfo"/>
        /// elements where
        /// each
        /// <see cref="TextInfo"/>
        /// element contains a word or a line
        /// and its 4 coordinates(bbox).
        /// (There will be parsed result in hOCR format produced by reader)
        /// </remarks>
        /// <param name="input">input file</param>
        /// <returns>
        /// Map&lt;Integer, List
        /// <see cref="TextInfo"/>
        /// &gt;&gt;
        /// </returns>
        public abstract IDictionary<int, IList<TextInfo>> ReadDataFromInput(FileInfo input);

        /// <summary>
        /// Reads data from the provided input image file and returns retrieved data
        /// as string.
        /// </summary>
        /// <param name="input">
        /// 
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="outputFormat">
        /// 
        /// <see cref="OutputFormat"/>
        /// </param>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// </returns>
        public abstract String ReadDataFromInput(FileInfo input, IOcrReader.OutputFormat outputFormat);
    }

    public static class IOcrReaderConstants {
    }
}
