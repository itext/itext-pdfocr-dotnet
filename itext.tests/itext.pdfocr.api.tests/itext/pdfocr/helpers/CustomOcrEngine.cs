using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Util;
using iText.Pdfocr;

namespace iText.Pdfocr.Helpers {
    public class CustomOcrEngine : IOcrEngine {
        private OcrEngineProperties ocrEngineProperties;

        public CustomOcrEngine() {
        }

        public CustomOcrEngine(OcrEngineProperties ocrEngineProperties) {
            this.ocrEngineProperties = new OcrEngineProperties(ocrEngineProperties);
        }

        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input) {
            IDictionary<int, IList<TextInfo>> result = new Dictionary<int, IList<TextInfo>>();
            String text = PdfHelper.DEFAULT_TEXT;
            if (input.FullName.Contains(PdfHelper.THAI_IMAGE_NAME)) {
                text = PdfHelper.THAI_TEXT;
            }
            TextInfo textInfo = new TextInfo(text, JavaUtil.ArraysAsList(204.0f, 158.0f, 742.0f, 294.0f));
            result.Put(1, JavaCollectionsUtil.SingletonList<TextInfo>(textInfo));
            return result;
        }

        public virtual void CreateTxt(IList<FileInfo> inputImages, FileInfo txtFile) {
        }

        public virtual OcrEngineProperties GetOcrEngineProperties() {
            return ocrEngineProperties;
        }
    }
}
