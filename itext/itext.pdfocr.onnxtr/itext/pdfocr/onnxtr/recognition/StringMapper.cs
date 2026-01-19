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
using System.Collections.Generic;
using System.Globalization;
using iText.Commons.Utils;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr.Recognition {
    /// <summary>Look-up table for mapping text recognition model results to strings.</summary>
    /// <remarks>
    /// Look-up table for mapping text recognition model results to strings.
    /// <para />
    /// If you only need to map indices to single UTF-16 code units, then consider
    /// using
    /// <see cref="Vocabulary"/>
    /// instead, as it is much more memory efficient.
    /// </remarks>
    public class StringMapper : IOutputLabelMapper<String> {
        private readonly String[] lookUpTable;

        /// <summary>Creates a new string mapper based on a look-up string.</summary>
        /// <remarks>
        /// Creates a new string mapper based on a look-up string. Each code point
        /// is mapped to an index.
        /// </remarks>
        /// <param name="lookUpString">look-up string, that will be used to build a look-up table</param>
        public StringMapper(String lookUpString) {
            Objects.RequireNonNull(lookUpString);
            List<string> characters = new List<string>();
            TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(lookUpString);
            while (enumerator.MoveNext()) {
                characters.Add(enumerator.GetTextElement());
            }
            this.lookUpTable = characters.ToArray();
        }

        /// <summary>Creates a new string mapper based on a look-up table.</summary>
        /// <param name="lookUpTable">look-up table to be used in the string mapper</param>
        public StringMapper(String[] lookUpTable) {
            Objects.RequireNonNull(lookUpTable);
            this.lookUpTable = (String[])lookUpTable.Clone();
        }

        /// <summary>Returns the size of the string mapper.</summary>
        /// <returns>the size of the string mapper</returns>
        public virtual int Size() {
            return lookUpTable.Length;
        }

        /// <summary>
        /// Returns character, which is mapped to the specified index in the lookup
        /// string.
        /// </summary>
        /// <param name="index">index to map</param>
        /// <returns>mapped character</returns>
        public virtual String Map(int index) {
            return lookUpTable[index];
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(lookUpTable);
        }

        public override bool Equals(Object o) {
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnxtr.Recognition.StringMapper that = (iText.Pdfocr.Onnxtr.Recognition.StringMapper)o;
            return Objects.DeepEquals(lookUpTable, that.lookUpTable);
        }

        public override String ToString() {
            return JavaUtil.ArraysToString(lookUpTable);
        }
    }
}
