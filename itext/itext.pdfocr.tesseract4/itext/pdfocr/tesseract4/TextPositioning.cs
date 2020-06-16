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
namespace iText.Pdfocr.Tesseract4 {
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
}
