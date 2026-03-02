/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Pdfocr.Onnx;

namespace iText.Pdfocr.Onnx.Text {
    /// <summary>Enumeration of the possible types of text positioning.</summary>
    /// <remarks>
    /// Enumeration of the possible types of text positioning.
    /// It is used to combine the
    /// <see cref="OnnxTrOcrEngine"/>
    /// image OCR result text
    /// and group it by lines, by words or by words and lines.
    /// </remarks>
    public enum TextPositioning {
        /// <summary>Text will be grouped by lines.</summary>
        BY_LINES,
        /// <summary>Text will be grouped by words.</summary>
        BY_WORDS,
        /// <summary>Similar to BY_WORDS mode, but top and bottom of word BBox are inherited from line (default value).
        ///     </summary>
        BY_WORDS_AND_LINES
    }
}
