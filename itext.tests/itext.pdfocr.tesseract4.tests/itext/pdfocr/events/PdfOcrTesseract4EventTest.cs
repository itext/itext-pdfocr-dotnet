/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

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
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4.Events;

namespace iText.Pdfocr.Events {
    public class PdfOcrTesseract4EventTest : IntegrationTestHelper {
        private const String PDF_OCR_TESSERACT4_ORIGIN_ID = "iText.Pdfocr.Tesseract4";

        [NUnit.Framework.Test]
        public virtual void TestEventTypes() {
            String[] expectedTypes = new String[] { "pdfOcr-tesseract4-image-ocr", "pdfOcr-tesseract4-image-to-pdf", "pdfOcr-tesseract4-image-to-pdfa"
                 };
            PdfOcrTesseract4Event[] testedEvents = new PdfOcrTesseract4Event[] { PdfOcrTesseract4Event.TESSERACT4_IMAGE_OCR
                , PdfOcrTesseract4Event.TESSERACT4_IMAGE_TO_PDF, PdfOcrTesseract4Event.TESSERACT4_IMAGE_TO_PDFA };
            for (int i = 0; i < testedEvents.Length; i++) {
                NUnit.Framework.Assert.AreEqual(expectedTypes[i], testedEvents[i].GetEventType());
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestOriginId() {
            String expected = PDF_OCR_TESSERACT4_ORIGIN_ID;
            PdfOcrTesseract4Event[] testedEvents = new PdfOcrTesseract4Event[] { PdfOcrTesseract4Event.TESSERACT4_IMAGE_TO_PDF
                , PdfOcrTesseract4Event.TESSERACT4_IMAGE_TO_PDF, PdfOcrTesseract4Event.TESSERACT4_IMAGE_TO_PDFA };
            foreach (PdfOcrTesseract4Event @event in testedEvents) {
                NUnit.Framework.Assert.AreEqual(expected, @event.GetOriginId());
            }
        }
    }
}
