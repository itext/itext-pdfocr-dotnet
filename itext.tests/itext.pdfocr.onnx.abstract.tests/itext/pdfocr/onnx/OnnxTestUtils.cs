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
using System.IO;
using System.Text;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Onnx.Util;

namespace iText.Pdfocr.Onnx {
    public class OnnxTestUtils {
        public static ExtractionStrategy ExtractTextFromLayer(PdfDocument pdfDocument, int pageNr, String layerName
            ) {
            ExtractionStrategy strategy = new ExtractionStrategy(layerName);
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetPage(pageNr));
            return strategy;
        }

        public static void ComparePdfs(String dest, String cmp, String targetDirectory) {
            String diff = new CompareTool().SetContentStreamFloatTolerance(0.021f).CompareByContent(dest, cmp, targetDirectory
                , "diff_");
            if (diff != null) {
                String[] splitted = iText.Commons.Utils.StringUtil.Split(cmp, "\\.");
                String filename = splitted[splitted.Length - 2];
                String cmp2 = cmp.Replace(filename, filename + "_2");
                if (FileUtil.FileExists(cmp2)) {
                    // Second cmp is required on .NET because of different results on .NET CoreApp and .NET Framework.
                    diff = new CompareTool().SetContentStreamFloatTolerance(0.021f).CompareByContent(dest, cmp2, targetDirectory
                        , "diff_");
                }
            }
            NUnit.Framework.Assert.IsNull(diff);
        }

        protected internal static String GetTextFromImage(FileInfo imageFile, IOcrEngine ocrEngine) {
            IDictionary<int, IList<TextInfo>> integerListMap = ocrEngine.DoImageOcr(imageFile);
            return GetStringFromListMap(integerListMap);
        }

        protected internal static void DoOcrAndCreatePdf(String imagePath, String destPdfPath, IOcrEngine ocrEngine
            ) {
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties().SetTextLayerName("Text1").
                SetTextColor(DeviceCmyk.MAGENTA);
            DoOcrAndCreatePdf(imagePath, destPdfPath, ocrEngine, ocrPdfCreatorProperties);
        }

        protected internal static void DoOcrAndCreatePdf(String imagePath, String destPdfPath, IOcrEngine ocrEngine
            , OcrPdfCreatorProperties ocrPdfCreatorProperties) {
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(ocrEngine, ocrPdfCreatorProperties);
            using (PdfWriter writer = new PdfWriter(destPdfPath)) {
                ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList(new FileInfo(imagePath)), writer).Close();
            }
        }

        protected internal static void ExtractTextAndCompare(String dest, String cmpTxt, String layerName, double 
            expRelDistance) {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, layerName);
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                String outText = extractionStrategy.GetResultantText();
                String cmpText = GetCmpText(cmpTxt);
                double relativeDistance = (double)MathUtil.CalculateLevenshteinDistance(cmpText, outText) / cmpText.Length;
                NUnit.Framework.Assert.IsTrue(relativeDistance < expRelDistance, "Expected: \"" + cmpText + "\", but was: \""
                     + outText + "\"");
            }
        }

        private static String GetCmpText(String txtPath) {
            byte[] bytes = File.ReadAllBytes(System.IO.Path.Combine(txtPath));
            return iText.Commons.Utils.JavaUtil.GetStringForBytes(bytes, System.Text.Encoding.UTF8);
        }

        private static String GetStringFromListMap(IDictionary<int, IList<TextInfo>> listMap) {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<int, IList<TextInfo>> entry in listMap) {
                foreach (TextInfo textInfo in entry.Value) {
                    if (textInfo.GetText() != null) {
                        stringBuilder.Append(textInfo.GetText()).Append('\n');
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
}
