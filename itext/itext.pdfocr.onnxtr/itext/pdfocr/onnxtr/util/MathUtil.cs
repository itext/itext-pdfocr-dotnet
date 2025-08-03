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
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr.Util {
    /// <summary>Additional math functions.</summary>
    public sealed class MathUtil {
        private MathUtil() {
        }

        /// <summary>Returns the index of the maximum value in the given array.</summary>
        /// <param name="values">the array of float values (must not be null or empty)</param>
        /// <returns>the index of the maximum value in the array</returns>
        public static int Argmax(float[] values) {
            Objects.RequireNonNull(values);
            if (values.Length == 0) {
                throw new ArgumentException(PdfOcrOnnxTrExceptionMessageConstant.VALUES_SHOULD_BE_A_NON_EMPTY_ARRAY);
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

        /// <summary>Calculates the Levenshtein distance between two input strings.</summary>
        /// <param name="source">the original string to be transformed</param>
        /// <param name="target">the target string to transform into</param>
        /// <returns>
        /// the minimum number of single-character edits required
        /// to convert the source string into the target string
        /// </returns>
        public static int CalculateLevenshteinDistance(String source, String target) {
            if (source == null || String.IsNullOrEmpty(source)) {
                return target == null || String.IsNullOrEmpty(target) ? 0 : target.Length;
            }
            if (target == null || String.IsNullOrEmpty(target)) {
                return source.Length;
            }
            char[] sourceChars = source.ToCharArray();
            char[] targetChars = target.ToCharArray();
            int[] previousRow = new int[targetChars.Length + 1];
            for (int i = 0; i <= targetChars.Length; ++i) {
                previousRow[i] = i;
            }
            for (int i = 1; i <= sourceChars.Length; ++i) {
                int[] currentRow = new int[targetChars.Length + 1];
                currentRow[0] = i;
                for (int j = 1; j <= targetChars.Length; ++j) {
                    int costDelete = previousRow[j] + 1;
                    int costInsert = currentRow[j - 1] + 1;
                    int costReplace = previousRow[j - 1] + (sourceChars[i - 1] == targetChars[j - 1] ? 0 : 1);
                    currentRow[j] = Math.Min(Math.Min(costDelete, costInsert), costReplace);
                }
                previousRow = currentRow;
            }
            return previousRow[targetChars.Length];
        }

        /// <summary>Computes the sigmoid function, also known as the logistic function, for the given input.</summary>
        /// <param name="x">the input value</param>
        /// <returns>the sigmoid of the input value</returns>
        public static float Expit(float x) {
            return (float)(1 / (1 + Math.Exp(-x)));
        }

        /// <summary>
        /// Computes the Euclidean modulo (non-negative remainder) of
        /// <paramref name="x"/>
        /// modulo
        /// <paramref name="y"/>.
        /// </summary>
        /// <param name="x">the dividend</param>
        /// <param name="y">the divisor (must not be zero)</param>
        /// <returns>
        /// the non-negative remainder of
        /// <paramref name="x"/>
        /// modulo
        /// <paramref name="y"/>
        /// </returns>
        public static float EuclideanModulo(float x, float y) {
            float remainder = x % y;
            if (remainder < 0) {
                return remainder + Math.Abs(y);
            }
            return remainder;
        }

        /// <summary>Clamps a value between a specified minimum and maximum range.</summary>
        /// <param name="value">the value to clamp</param>
        /// <param name="min">the minimum allowed value</param>
        /// <param name="max">the maximum allowed value</param>
        /// <returns>
        /// 
        /// <paramref name="value"/>
        /// if it falls within the range; otherwise, the nearest bound (min or max)
        /// </returns>
        public static double Clamp(double value, double min, double max) {
            if (max < min) {
                throw new ArgumentException(PdfOcrOnnxTrExceptionMessageConstant.MAX_SHOULD_NOT_BE_LESS_THAN_MIN);
            }
            return Math.Min(max, Math.Max(value, min));
        }
    }
}
