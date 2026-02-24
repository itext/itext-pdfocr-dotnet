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
using System.Collections.Generic;

namespace iText.Pdfocr.Onnxtr.Merging {
    /// <summary>
    /// Interface for a processing class, which handles merging text boxes,
    /// received from a text detection routine.
    /// </summary>
    public interface ITextBoxMerger {
        /// <summary>Merges text boxes based on some set of rules.</summary>
        /// <remarks>
        /// Merges text boxes based on some set of rules. This method should return
        /// a new list and not modify the input.
        /// </remarks>
        /// <param name="detectedTextBoxes">
        /// list of rotated text boxes, provided by the
        /// text detection routine
        /// </param>
        /// <returns>a new list with merged text boxes</returns>
        IList<iText.Kernel.Geom.Point[]> Process(IList<iText.Kernel.Geom.Point[]> detectedTextBoxes);
    }
}
