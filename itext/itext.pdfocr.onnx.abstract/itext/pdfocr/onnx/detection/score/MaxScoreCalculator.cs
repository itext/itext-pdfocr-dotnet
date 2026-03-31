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
namespace iText.Pdfocr.Onnx.Detection.Score {
    /// <summary>Score calculator, which returns the biggest observed sample.</summary>
    /// <remarks>
    /// Score calculator, which returns the biggest observed sample. If no samples
    /// are observed, -inf is returned.
    /// </remarks>
    public class MaxScoreCalculator : IScoreCalculator {
        /// <summary>Biggest observed sample.</summary>
        private float maxSample;

        /// <summary>Creates a new score calculator.</summary>
        public MaxScoreCalculator() {
            maxSample = float.NegativeInfinity;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Observe(float sample) {
            if (sample > maxSample) {
                maxSample = sample;
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual float Calculate() {
            return maxSample;
        }
    }
}
