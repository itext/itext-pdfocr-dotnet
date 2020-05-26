using System.Collections.Generic;
using System.IO;

namespace iText.Pdfocr {
    /// <summary>
    /// <see cref="IOcrEngine"/>
    /// interface is used for instantiating new OcrReader
    /// objects.
    /// </summary>
    /// <remarks>
    /// <see cref="IOcrEngine"/>
    /// interface is used for instantiating new OcrReader
    /// objects.
    /// <see cref="IOcrEngine"/>
    /// interface provides possibility to perform OCR,
    /// to read data from input files and to return the contained text in the
    /// required format.
    /// </remarks>
    public interface IOcrEngine {
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
        IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input);

        /// <summary>
        /// Performs OCR using provided
        /// <see cref="IOcrEngine"/>
        /// for the given list of
        /// input images and saves output to a text file using provided path.
        /// </summary>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="txtFile">file to be created</param>
        void CreateTxt(IList<FileInfo> inputImages, FileInfo txtFile);
    }
}
