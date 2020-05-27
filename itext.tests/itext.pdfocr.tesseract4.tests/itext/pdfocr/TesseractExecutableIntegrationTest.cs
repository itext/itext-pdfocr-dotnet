using System.IO;
using iText.Pdfocr.Tesseract4;
using iText.Test.Attributes;

namespace iText.Pdfocr {
    public class TesseractExecutableIntegrationTest : AbstractIntegrationTest {
        [LogMessage(Tesseract4OcrException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestNullPathToTesseractExecutable() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                Tesseract4ExecutableOcrEngine tesseractExecutableReader = new Tesseract4ExecutableOcrEngine(new Tesseract4OcrEngineProperties
                    ());
                tesseractExecutableReader.SetPathToExecutable(null);
                GetTextFromPdf(tesseractExecutableReader, file);
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(Tesseract4OcrException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE))
;
        }

        [LogMessage(Tesseract4OcrException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestEmptyPathToTesseractExecutable() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                GetTextFromPdf(new Tesseract4ExecutableOcrEngine("", new Tesseract4OcrEngineProperties()), file);
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(Tesseract4OcrException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE))
;
        }

        [LogMessage(Tesseract4LogMessageConstant.COMMAND_FAILED, Count = 1)]
        [LogMessage(Tesseract4OcrException.TESSERACT_NOT_FOUND, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestIncorrectPathToTesseractExecutable() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                GetTextFromPdf(new Tesseract4ExecutableOcrEngine("path\\to\\executable\\", new Tesseract4OcrEngineProperties
                    ()), file);
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(Tesseract4OcrException.TESSERACT_NOT_FOUND))
;
        }
    }
}
