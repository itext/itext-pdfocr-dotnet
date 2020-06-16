/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2020 iText Group NV
    Authors: iText Software.

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
    }
}
