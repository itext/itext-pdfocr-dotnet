using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace iText.Ocr {
    /// <summary>
    /// <see cref="IOcrReader"/>
    /// interface is used for instantiating new OcrReader
    /// objects.
    /// </summary>
    /// <remarks>
    /// <see cref="IOcrReader"/>
    /// interface is used for instantiating new OcrReader
    /// objects.
    /// <see cref="IOcrReader"/>
    /// interface provides possibility to perform OCR,
    /// to read data from input files and to return the contained text in the
    /// required format.
    /// </remarks>
    public abstract class IOcrReader {
        /// <summary>
        /// Reads data from the provided input image file and returns retrieved data
        /// in the format described below.
        /// </summary>
        /// <param name="input">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// where key is
        /// <see cref="int?"/>
        /// representing the number of the page and value is
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="TextInfo"/>
        /// elements where each
        /// <see cref="TextInfo"/>
        /// element contains a word or a line and its 4
        /// coordinates(bbox)
        /// </returns>
        public abstract IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input);

        /// <summary>
        /// Reads data from the provided input image file and returns retrieved data
        /// as string.
        /// </summary>
        /// <param name="input">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="outputFormat">
        /// 
        /// <see cref="OutputFormat"/>
        /// for the result returned
        /// by
        /// <see cref="IOcrReader"/>
        /// </param>
        /// <returns>
        /// OCR result as a
        /// <see cref="System.String"/>
        /// that is
        /// returned after processing the given image
        /// </returns>
        public abstract String DoImageOcr(FileInfo input, IOcrReader.OutputFormat outputFormat);

        /// <summary>Enumeration of the possible types of text positioning.</summary>
        /// <remarks>
        /// Enumeration of the possible types of text positioning.
        /// It is used when there is possibility in selected Reader to process
        /// the text by lines or by words and to return coordinates for the
        /// selected type of item.
        /// For tesseract this value makes sense only if selected
        /// <see cref="OutputFormat"/>
        /// is
        /// <see cref="OutputFormat.HOCR"/>.
        /// </remarks>
        public enum TextPositioning {
            /// <summary>Text will be located by lines retrieved from hocr file.</summary>
            /// <remarks>
            /// Text will be located by lines retrieved from hocr file.
            /// (default value)
            /// </remarks>
            BY_LINES,
            /// <summary>Text will be located by words retrieved from hocr file.</summary>
            BY_WORDS
        }

        /// <summary>Enumeration of the available output formats.</summary>
        /// <remarks>
        /// Enumeration of the available output formats.
        /// It is used when there is possibility in selected Reader to process input
        /// file and to return result in the required output format.
        /// </remarks>
        public enum OutputFormat {
            /// <summary>
            /// Reader will produce XHTML output compliant
            /// with the hOCR specification.
            /// </summary>
            /// <remarks>
            /// Reader will produce XHTML output compliant
            /// with the hOCR specification.
            /// Output will be parsed and represented as
            /// <see cref="IList{E}"/>
            /// of
            /// <see cref="TextInfo"/>
            /// objects
            /// </remarks>
            HOCR,
            /// <summary>Reader will produce plain txt file.</summary>
            TXT
        }
    }

    public static class IOcrReaderConstants {
    }
}
