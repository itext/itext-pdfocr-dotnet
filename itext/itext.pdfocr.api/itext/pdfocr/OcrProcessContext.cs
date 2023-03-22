/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
namespace iText.Pdfocr {
    /// <summary>Class for storing ocr processing context.</summary>
    public class OcrProcessContext {
        private AbstractPdfOcrEventHelper ocrEventHelper;

        /// <summary>Creates an instance of ocr process context</summary>
        /// <param name="eventHelper">helper class for working with events</param>
        public OcrProcessContext(AbstractPdfOcrEventHelper eventHelper) {
            this.ocrEventHelper = eventHelper;
        }

        /// <summary>Returns helper for working with events.</summary>
        /// <returns>
        /// an instance of
        /// <see cref="AbstractPdfOcrEventHelper"/>
        /// </returns>
        public virtual AbstractPdfOcrEventHelper GetOcrEventHelper() {
            return ocrEventHelper;
        }

        /// <summary>Sets ocr event helper.</summary>
        /// <param name="eventHelper">event helper</param>
        public virtual void SetOcrEventHelper(AbstractPdfOcrEventHelper eventHelper) {
            this.ocrEventHelper = eventHelper;
        }
    }
}
