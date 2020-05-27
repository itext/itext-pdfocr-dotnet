using System;
using System.IO;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4;
using iText.Test.Attributes;

namespace iText.Pdfocr.Imageformats {
    public abstract class ImageFormatIntegrationTest : AbstractIntegrationTest {
        internal AbstractTesseract4OcrEngine tesseractReader;

        public ImageFormatIntegrationTest(AbstractIntegrationTest.ReaderType type) {
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.Test]
        public virtual void TestBMPText() {
            String path = testImagesDirectory + "example_01.BMP";
            String expectedOutput = "This is a test message for OCR Scanner Test";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path), JavaCollectionsUtil.SingletonList
                <String>("eng"));
            realOutputHocr = iText.IO.Util.StringUtil.ReplaceAll(realOutputHocr, "[\n]", " ");
            realOutputHocr = iText.IO.Util.StringUtil.ReplaceAll(realOutputHocr, "[‘]", "");
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains((expectedOutput)));
        }

        [NUnit.Framework.Test]
        public virtual void TestBMPText02() {
            String path = testImagesDirectory + "englishText.bmp";
            String expectedOutput = "This is a test message for OCR Scanner Test BMPTest";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path), JavaCollectionsUtil.SingletonList
                <String>("eng"));
            realOutputHocr = iText.IO.Util.StringUtil.ReplaceAll(realOutputHocr, "[\n]", " ");
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains((expectedOutput)));
        }

        [NUnit.Framework.Test]
        public virtual void CompareJFIF() {
            String testName = "compareJFIF";
            String filename = "example_02";
            String expectedPdfPath = testDocumentsDirectory + filename + ".pdf";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + ".pdf";
            DoOcrAndSavePdfToPath(tesseractReader, testImagesDirectory + filename + ".JFIF", resultPdfPath, null, DeviceCmyk
                .MAGENTA);
            new CompareTool().CompareByContent(expectedPdfPath, resultPdfPath, testDocumentsDirectory, "diff_");
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromJPG() {
            String path = testImagesDirectory + "numbers_02.jpg";
            String expectedOutput = "0123456789";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromJPE() {
            String path = testImagesDirectory + "numbers_01.jpe";
            String expectedOutput = "619121";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromTIF() {
            String path = testImagesDirectory + "numbers_01.tif";
            String expectedOutput = "619121";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void TestBigTiffWithoutPreprocessing() {
            String path = testImagesDirectory + "example_03_10MB.tiff";
            String expectedOutput = "Image File Format";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages
                (false));
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path), JavaCollectionsUtil.SingletonList
                <String>("eng"));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void TestInputMultipagesTIFFWithPreprocessing() {
            String path = testImagesDirectory + "multipage.tiff";
            String expectedOutput = "Multipage\nTIFF\nExample\nPage 5";
            FileInfo file = new FileInfo(path);
            String realOutputHocr = GetTextFromPdf(tesseractReader, file, 5, JavaCollectionsUtil.SingletonList<String>
                ("eng"));
            NUnit.Framework.Assert.IsNotNull(realOutputHocr);
            NUnit.Framework.Assert.AreEqual(expectedOutput, realOutputHocr);
        }

        [NUnit.Framework.Test]
        public virtual void TestInputMultipagesTIFFWithoutPreprocessing() {
            String path = testImagesDirectory + "multipage.tiff";
            String expectedOutput = "Multipage\nTIFF\nExample\nPage 3";
            FileInfo file = new FileInfo(path);
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages
                (false));
            String realOutputHocr = GetTextFromPdf(tesseractReader, file, 3, JavaCollectionsUtil.SingletonList<String>
                ("eng"));
            NUnit.Framework.Assert.IsNotNull(realOutputHocr);
            NUnit.Framework.Assert.AreEqual(expectedOutput, realOutputHocr);
        }

        [LogMessage(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestInputWrongFormat() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "example.txt");
                GetTextFromPdf(tesseractReader, file);
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(Tesseract4OcrException.INCORRECT_INPUT_IMAGE_FORMAT, "txt")))
;
        }

        [NUnit.Framework.Test]
        public virtual void CompareNumbersJPG() {
            String testName = "compareNumbersJPG";
            String filename = "numbers_01";
            String expectedPdfPath = testDocumentsDirectory + filename + ".pdf";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + ".pdf";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetTextPositioning
                (TextPositioning.BY_WORDS));
            DoOcrAndSavePdfToPath(tesseractReader, testImagesDirectory + filename + ".jpg", resultPdfPath);
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetTextPositioning
                (TextPositioning.BY_LINES));
            new CompareTool().CompareByContent(expectedPdfPath, resultPdfPath, testDocumentsDirectory, "diff_");
        }
    }
}
