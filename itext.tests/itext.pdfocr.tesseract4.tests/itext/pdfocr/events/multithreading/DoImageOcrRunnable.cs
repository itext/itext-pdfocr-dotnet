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
using System.IO;
using iText.IO.Util;
using iText.Kernel.Counter.Event;
using iText.Kernel.Pdf;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4;

namespace iText.Pdfocr.Events.Multithreading {
    public class DoImageOcrRunnable : Object {
        private AbstractTesseract4OcrEngine tesseractReader;

        private FileInfo imgFile;

        private FileInfo outputFile;

        private bool createPdf;

        private IMetaInfo metaInfo;

        internal DoImageOcrRunnable(AbstractTesseract4OcrEngine tesseractReader, IMetaInfo metaInfo, FileInfo imgFile
            , FileInfo outputFile, bool createPdf) {
            this.tesseractReader = tesseractReader;
            this.metaInfo = metaInfo;
            this.imgFile = imgFile;
            this.outputFile = outputFile;
            this.createPdf = createPdf;
        }

        public virtual void Run() {
            try {
                tesseractReader.SetThreadLocalMetaInfo(metaInfo);
                if (createPdf) {
                    new OcrPdfCreator(tesseractReader).CreatePdf(JavaUtil.ArraysAsList(imgFile), new PdfWriter(outputFile));
                }
                else {
                    tesseractReader.DoTesseractOcr(imgFile, outputFile, OutputFormat.TXT);
                }
                // for test purposes
                System.Console.Out.WriteLine(imgFile.Name);
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }
    }
}
