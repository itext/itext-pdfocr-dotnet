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
namespace iText.Pdfocr.Onnxtr.Detection.Score {
    /// <summary>
    /// Interface for abstracting away score calculation over a text contour in the
    /// text detection post-processor.
    /// </summary>
    public interface IScoreCalculator {
        /// <summary>Observe a sample value from the text contour.</summary>
        /// <remarks>
        /// Observe a sample value from the text contour. This modifies the
        /// calculator state.
        /// </remarks>
        /// <param name="sample">sample to observe</param>
        void Observe(float sample);

        /// <summary>Calculate the score based on the observed samples.</summary>
        /// <returns>the calculated score</returns>
        float Calculate();
    }
}
