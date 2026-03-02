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
    /// <summary>
    /// Score calculator, which calculates the mean values over the observed
    /// samples.
    /// </summary>
    /// <remarks>
    /// Score calculator, which calculates the mean values over the observed
    /// samples. If no samples are observed, 0 is returned.
    /// </remarks>
    public class MeanScoreCalculator : IScoreCalculator {
        /// <summary>Value to return, when no samples are observed.</summary>
        private const float EMPTY_VALUE = 0.0F;

        /// <summary>Sum of the observed samples.</summary>
        private double sum;

        /// <summary>Counter of observed samples.</summary>
        private long n;

        /// <summary>Creates a new score calculator.</summary>
        public MeanScoreCalculator() {
            this.sum = 0.0;
            this.n = 0;
        }

        /// <summary><inheritDoc/></summary>
        public virtual void Observe(float sample) {
            sum += sample;
            ++n;
        }

        /// <summary><inheritDoc/></summary>
        public virtual float Calculate() {
            if (n == 0) {
                return EMPTY_VALUE;
            }
            return (float)(sum / n);
        }
    }
}
