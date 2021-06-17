/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.Kernel.Counter.Event;

namespace iText.Pdfocr.Tesseract4.Events {
    /// <summary>Class for ocr events</summary>
    public class PdfOcrTesseract4Event : IGenericEvent {
        public static readonly iText.Pdfocr.Tesseract4.Events.PdfOcrTesseract4Event TESSERACT4_IMAGE_OCR = new iText.Pdfocr.Tesseract4.Events.PdfOcrTesseract4Event
            ("tesseract4-image-ocr");

        public static readonly iText.Pdfocr.Tesseract4.Events.PdfOcrTesseract4Event TESSERACT4_IMAGE_TO_PDF = new 
            iText.Pdfocr.Tesseract4.Events.PdfOcrTesseract4Event("tesseract4-image-to-pdf");

        public static readonly iText.Pdfocr.Tesseract4.Events.PdfOcrTesseract4Event TESSERACT4_IMAGE_TO_PDFA = new 
            iText.Pdfocr.Tesseract4.Events.PdfOcrTesseract4Event("tesseract4-image-to-pdfa");

        private const String PDF_OCR_TESSERACT4_ORIGIN_ID = "iText.Pdfocr.Tesseract4";

        private readonly String subtype;

        private PdfOcrTesseract4Event(String subtype) {
            this.subtype = subtype;
        }

        public virtual String GetEventType() {
            return "pdfOcr-" + subtype;
        }

        public virtual String GetOriginId() {
            return PDF_OCR_TESSERACT4_ORIGIN_ID;
        }
    }
}
