/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
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
        // TODO DEVSIX-9193: mark this on breaking changes page. Make this interface better.
        //  There are two problems. First, we get only one image per call in {@link #doImageOcr}. But the text detector
        //  can batch multiple images and for on them at once, which is a performance improvement, at least on GPU.
        //  Second problem is that it forces all OCR engines to reimplement image reading code. Image reading should happen on
        //  a layer higher, so that the code is common. This should also be a performance improvement, since images get read
        //  again anyway to create the final PDF. Check com.itextpdf.pdfocr.onnxtr.OnnxTrOcrEngine.getImages
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
        /// Reads data from the provided input image file and returns retrieved data
        /// in the format described below.
        /// </summary>
        /// <param name="input">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="ocrProcessContext">ocr processing context</param>
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
        IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input, OcrProcessContext ocrProcessContext);

        /// <summary>
        /// Performs OCR using provided
        /// <see cref="IOcrEngine"/>
        /// for the given list of
        /// input images and saves output to a text file using provided path.
        /// </summary>
        /// <remarks>
        /// Performs OCR using provided
        /// <see cref="IOcrEngine"/>
        /// for the given list of
        /// input images and saves output to a text file using provided path.
        /// Note that a human reading order is not guaranteed
        /// due to possible specifics of input images (multi column layout, tables etc)
        /// </remarks>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="txtFile">file to be created</param>
        void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile);

        /// <summary>
        /// Performs OCR using provided
        /// <see cref="IOcrEngine"/>
        /// for the given list of
        /// input images and saves output to a text file using provided path.
        /// </summary>
        /// <remarks>
        /// Performs OCR using provided
        /// <see cref="IOcrEngine"/>
        /// for the given list of
        /// input images and saves output to a text file using provided path.
        /// Note that a human reading order is not guaranteed
        /// due to possible specifics of input images (multi column layout, tables etc)
        /// </remarks>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="txtFile">file to be created</param>
        /// <param name="ocrProcessContext">ocr processing context</param>
        void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile, OcrProcessContext ocrProcessContext);

        /// <summary>Checks whether tagging is supported by the OCR engine.</summary>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if tagging is supported by the engine,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        bool IsTaggingSupported();
    }
}
