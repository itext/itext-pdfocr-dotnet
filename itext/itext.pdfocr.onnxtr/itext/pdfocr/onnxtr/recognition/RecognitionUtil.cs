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

namespace iText.Pdfocr.Onnxtr.Recognition {
//\cond DO_NOT_DOCUMENT
    /// <summary>Class which contains utility methods for recognition package.</summary>
    public class RecognitionUtil {
        /// <summary>Returns the number of Unicode code points in the specified text range of this String.</summary>
        /// <remarks>
        /// Returns the number of Unicode code points in the specified text range of this String.
        /// The text range begins at the specified beginIndex and extends to the char at index endIndex - 1.
        /// Thus the length (in chars) of the text range is endIndex-beginIndex.
        /// Unpaired surrogates within the text range count as one code point each.
        /// </remarks>
        /// <param name="str">str to search codepoints for.</param>
        /// <param name="beginIndex">– the index to the first char of the text range.</param>
        /// <param name="endIndex">– the index after the last char of the text range.</param>
        /// <returns>the number of Unicode code points in the specified text range</returns>
        internal static int CodePointCount(String str, int beginIndex, int endIndex) {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (beginIndex < 0 || endIndex > str.Length || beginIndex > endIndex)
                throw new ArgumentOutOfRangeException();

            int count = 0;
            for (int i = beginIndex; i < endIndex; i++)
            {
                if (char.IsHighSurrogate(str[i]))
                {
                    if (i + 1 < endIndex && char.IsLowSurrogate(str[i + 1]))
                    {
                        i++; 
                    }
                }
                count++;
            }
            return count;
        }
        
        public static int GetArrayOffset(FloatBufferMdArray array) {
            return 0;
        }
        
        public static int GetArraySize(FloatBufferMdArray array) {
            return array.GetData().Length;
        }
    }
//\endcond
}
