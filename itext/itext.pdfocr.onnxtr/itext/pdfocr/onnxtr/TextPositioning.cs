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
namespace iText.Pdfocr.Onnxtr {
    /// <summary>Enumeration of the possible types of text positioning.</summary>
    /// <remarks>
    /// Enumeration of the possible types of text positioning.
    /// It is used to combine the
    /// <see cref="OnnxTrOcrEngine"/>
    /// image OCR result text and group it by lines or by words.
    /// </remarks>
    public enum TextPositioning {
        /// <summary>Text will be grouped by lines.</summary>
        /// <remarks>
        /// Text will be grouped by lines.
        /// (default value)
        /// </remarks>
        BY_LINES,
        /// <summary>Text will be grouped by words.</summary>
        BY_WORDS
    }
}
