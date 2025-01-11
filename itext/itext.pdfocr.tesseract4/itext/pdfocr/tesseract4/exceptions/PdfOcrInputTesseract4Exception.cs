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

namespace iText.Pdfocr.Tesseract4.Exceptions {
    /// <summary>Exception class for Tesseract4 input related exceptions.</summary>
    public class PdfOcrInputTesseract4Exception : PdfOcrTesseract4Exception {
        /// <summary>
        /// Creates a new
        /// <see cref="PdfOcrInputTesseract4Exception"/>.
        /// </summary>
        /// <param name="msg">the detail message.</param>
        /// <param name="e">
        /// the cause
        /// (which is saved for later retrieval
        /// by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public PdfOcrInputTesseract4Exception(String msg, Exception e)
            : base(msg, e) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfOcrInputTesseract4Exception"/>.
        /// </summary>
        /// <param name="msg">the detail message.</param>
        public PdfOcrInputTesseract4Exception(String msg)
            : base(msg) {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="PdfOcrInputTesseract4Exception"/>.
        /// </summary>
        /// <param name="e">
        /// the cause
        /// which is saved for later retrieval
        /// by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public PdfOcrInputTesseract4Exception(Exception e)
            : base(e) {
        }
    }
}
