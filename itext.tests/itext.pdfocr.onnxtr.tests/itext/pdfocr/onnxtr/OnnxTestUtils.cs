using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Pdfocr;

namespace iText.Pdfocr.Onnxtr {
    public class OnnxTestUtils {
        public static String GetTextFromLayer(PdfDocument pdfDocument, int pageNr, String layerName) {
            ExtractionStrategy extractionStrategy = ExtractTextFromLayer(pdfDocument, pageNr, layerName);
            return extractionStrategy.GetResultantText();
        }

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
