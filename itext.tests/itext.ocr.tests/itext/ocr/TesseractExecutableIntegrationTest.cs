using System.IO;
using iText.IO.Util;
using iText.Test.Attributes;

namespace iText.Ocr {
    public class TesseractExecutableIntegrationTest : AbstractIntegrationTest {
        [LogMessage(OCRException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestNullPathToTesseractExecutable() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                GetTextFromPdf(new TesseractExecutableReader(null, null), file);
            }
            , NUnit.Framework.Throws.InstanceOf<OCRException>().With.Message.EqualTo(OCRException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE))
;
        }

        [LogMessage(OCRException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestEmptyPathToTesseractExecutable() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                GetTextFromPdf(new TesseractExecutableReader("", ""), file);
            }
            , NUnit.Framework.Throws.InstanceOf<OCRException>().With.Message.EqualTo(OCRException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE))
;
        }

        [LogMessage(LogMessageConstant.TESSERACT_FAILED, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestCLTesseractWithWrongCommand() {
            NUnit.Framework.Assert.That(() =>  {
                TesseractUtil.RunCommand(JavaUtil.ArraysAsList("tesseract", "random.jpg"), false);
            }
            , NUnit.Framework.Throws.InstanceOf<OCRException>())
;
        }

        [LogMessage(LogMessageConstant.TESSERACT_FAILED, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestCLTesseractWithNullCommand() {
            NUnit.Framework.Assert.That(() =>  {
                TesseractUtil.RunCommand(null, false);
            }
            , NUnit.Framework.Throws.InstanceOf<OCRException>())
;
        }
    }
}
