using System;
using System.IO;
using iText.Pdfocr.Helpers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfocr {
    public class PdfInputImageTest : ExtendedITextTest {
        [LogMessage(LogMessageConstant.CannotReadInputImage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestCorruptedImage() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(PdfHelper.GetImagesTestDirectory() + "corrupted.jpg");
                String realOutput = PdfHelper.GetTextFromPdf(file, "testCorruptedImage");
                NUnit.Framework.Assert.IsNotNull(realOutput);
                NUnit.Framework.Assert.AreEqual("", realOutput);
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>())
;
        }

        [LogMessage(LogMessageConstant.CannotReadInputImage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestCorruptedImageWithoutExtension() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(PdfHelper.GetImagesTestDirectory() + "corrupted");
                String realOutput = PdfHelper.GetTextFromPdf(file, "testCorruptedImageWithoutExtension");
                NUnit.Framework.Assert.IsNotNull(realOutput);
                NUnit.Framework.Assert.AreEqual("", realOutput);
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>())
;
        }
    }
}
