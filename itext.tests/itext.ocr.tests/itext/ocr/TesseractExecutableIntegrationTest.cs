using System.IO;
using iText.IO.Util;
using iText.Test.Attributes;

namespace iText.Ocr {
    public class TesseractExecutableIntegrationTest : AbstractIntegrationTest {
        [LogMessage(OcrException.CannotFindPathToTesseractExecutable, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestNullPathToTesseractExecutable() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                TesseractExecutableReader tesseractExecutableReader = new TesseractExecutableReader(null);
                tesseractExecutableReader.SetPathToExecutable(null);
                GetTextFromPdf(tesseractExecutableReader, file);
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>().With.Message.EqualTo(OcrException.CannotFindPathToTesseractExecutable))
;
        }

        [LogMessage(OcrException.CannotFindPathToTesseractExecutable, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestEmptyPathToTesseractExecutable() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                GetTextFromPdf(new TesseractExecutableReader("", ""), file);
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>().With.Message.EqualTo(OcrException.CannotFindPathToTesseractExecutable))
;
        }

        [LogMessage(LogMessageConstant.TesseractFailed, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestCLTesseractWithWrongCommand() {
            NUnit.Framework.Assert.That(() =>  {
                TesseractUtil.RunCommand(JavaUtil.ArraysAsList("tesseract", "random.jpg"), false);
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>())
;
        }

        [LogMessage(LogMessageConstant.TesseractFailed, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestCLTesseractWithNullCommand() {
            NUnit.Framework.Assert.That(() =>  {
                TesseractUtil.RunCommand(null, false);
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>())
;
        }
    }
}
