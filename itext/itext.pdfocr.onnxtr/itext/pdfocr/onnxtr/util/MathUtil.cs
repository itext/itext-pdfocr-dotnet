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
using System;

namespace iText.Pdfocr.Onnxtr.Util {
    /// <summary>Additional math functions.</summary>
    public sealed class MathUtil {
        private MathUtil() {
        }

        public static int Argmax(float[] values) {
            if (values.Length == 0) {
                throw new ArgumentException("values should be a non-empty array");
            }
            float resultValue = float.NegativeInfinity;
            int resultIndex = 0;
            for (int i = 0; i < values.Length; ++i) {
                if (values[i] > resultValue) {
                    resultValue = values[i];
                    resultIndex = i;
                }
            }
            return resultIndex;
        }

        public static float Expit(float x) {
            return (float)(1 / (1 + Math.Exp(-x)));
        }

        public static float EuclideanModulo(float x, float y) {
            float remainder = x % y;
            if (remainder < 0) {
                return remainder + Math.Abs(y);
            }
            return remainder;
        }

        public static double Clamp(double value, double min, double max) {
            if (max < min) {
                throw new ArgumentException("max should not be less than min");
            }
            return Math.Min(max, Math.Max(value, min));
        }
    }
}
