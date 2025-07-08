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
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Pdfocr;

namespace iText.Pdfocr.Onnxtr {
    public class OnnxTestUtils {
        public static ExtractionStrategy ExtractTextFromLayer(PdfDocument pdfDocument, int pageNr, String layerName
            ) {
            ExtractionStrategy strategy = new ExtractionStrategy(layerName);
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetPage(pageNr));
            return strategy;
        }

        protected internal static String GetTextFromImage(FileInfo imageFile, OnnxTrOcrEngine ocrEngine) {
            IDictionary<int, IList<TextInfo>> integerListMap = ocrEngine.DoImageOcr(imageFile);
            return GetStringFromListMap(integerListMap);
        }

        private static String GetStringFromListMap(IDictionary<int, IList<TextInfo>> listMap) {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<int, IList<TextInfo>> entry in listMap) {
                foreach (TextInfo textInfo in entry.Value) {
                    if (textInfo.GetText() != null) {
                        stringBuilder.Append(textInfo.GetText()).Append("\n");
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
}
