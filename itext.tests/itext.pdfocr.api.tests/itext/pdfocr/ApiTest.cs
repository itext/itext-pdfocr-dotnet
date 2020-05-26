using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Util;
using iText.Pdfocr.Helpers;

namespace iText.Pdfocr {
    public class ApiTest {
        [NUnit.Framework.Test]
        public virtual void TestTextInfo() {
            String path = PdfHelper.GetDefaultImagePath();
            IDictionary<int, IList<TextInfo>> result = new CustomOcrEngine().DoImageOcr(new FileInfo(path));
            NUnit.Framework.Assert.AreEqual(1, result.Count);
            TextInfo textInfo = new TextInfo();
            textInfo.SetText("text");
            textInfo.SetBbox(JavaUtil.ArraysAsList(204.0f, 158.0f, 742.0f, 294.0f));
            int page = 2;
            result.Put(page, JavaCollectionsUtil.SingletonList<TextInfo>(textInfo));
            NUnit.Framework.Assert.AreEqual(2, result.Count);
            NUnit.Framework.Assert.AreEqual(textInfo.GetText(), result.Get(page)[0].GetText());
            NUnit.Framework.Assert.AreEqual(textInfo.GetBbox().Count, result.Get(page)[0].GetBbox().Count);
        }
    }
}
