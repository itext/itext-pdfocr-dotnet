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
using iText.Commons.Actions.Data;

namespace iText.Pdfocr.Tesseract4.Actions.Data {
    /// <summary>
    /// Stores an instance of
    /// <see cref="iText.Commons.Actions.Data.ProductData"/>
    /// related to iText pdfOcr Tesseract4 module.
    /// </summary>
    public class PdfOcrTesseract4ProductData {
        private const String PDF_OCR_TESSERACT4_PRODUCT_NAME = "pdfOcr-tesseract4";

        private const String PDF_OCR_TESSERACT4_PUBLIC_PRODUCT_NAME = "pdfOCR-Tesseract4";

        private const String PDF_OCR_VERSION = "4.0.3-SNAPSHOT";

        private const int PDF_OCR_COPYRIGHT_SINCE = 2000;

        private const int PDF_OCR_COPYRIGHT_TO = 2025;

        private static readonly ProductData PDF_OCR_PRODUCT_DATA = new ProductData(PDF_OCR_TESSERACT4_PUBLIC_PRODUCT_NAME
            , PDF_OCR_TESSERACT4_PRODUCT_NAME, PDF_OCR_VERSION, PDF_OCR_COPYRIGHT_SINCE, PDF_OCR_COPYRIGHT_TO);

        /// <summary>
        /// Getter for an instance of
        /// <see cref="iText.Commons.Actions.Data.ProductData"/>
        /// related to iText pdfOcr Tesseract4 module.
        /// </summary>
        /// <returns>iText pdfOcr Tesseract4 product description</returns>
        public static ProductData GetInstance() {
            return PDF_OCR_PRODUCT_DATA;
        }
    }
}
