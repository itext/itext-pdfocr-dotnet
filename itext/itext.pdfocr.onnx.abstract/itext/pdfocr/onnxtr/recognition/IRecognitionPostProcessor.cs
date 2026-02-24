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
using System;
using iText.Pdfocr.Onnxtr;

namespace iText.Pdfocr.Onnxtr.Recognition {
    /// <summary>
    /// Interface for post-processors, which convert raw output of an ML model and
    /// returns recognized characters as a string.
    /// </summary>
    public interface IRecognitionPostProcessor {
        /// <summary>Process ML model output and return recognized characters as string.</summary>
        /// <param name="output">raw output of the ML model</param>
        /// <returns>recognized characters as string</returns>
        String Process(FloatBufferMdArray output);

        /// <summary>Returns the size of the output character label vector.</summary>
        /// <remarks>
        /// Returns the size of the output character label vector. I.e. how many
        /// distinct tokens/characters the model recognizes.
        /// </remarks>
        /// <returns>the size of the output character label vector</returns>
        int LabelDimension();
    }
}
