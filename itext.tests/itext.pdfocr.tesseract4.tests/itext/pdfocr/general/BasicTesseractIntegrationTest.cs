using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common.Logging;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4;
using iText.Test.Attributes;

namespace iText.Pdfocr.General {
    public abstract class BasicTesseractIntegrationTest : AbstractIntegrationTest {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Pdfocr.General.BasicTesseractIntegrationTest
            ));

        internal Tesseract4OcrEngine tesseractReader;

        public BasicTesseractIntegrationTest(AbstractIntegrationTest.ReaderType type) {
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.Test]
        public virtual void TestFontColorInMultiPagePdf() {
            String testName = "testFontColorInMultiPagePdf";
            String path = testImagesDirectory + "multipage.tiff";
            String pdfPath = GetTargetDirectory() + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
            ocrPdfCreatorProperties.SetTextLayerName("Text1");
            Color color = DeviceCmyk.MAGENTA;
            ocrPdfCreatorProperties.SetTextColor(color);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, ocrPdfCreatorProperties);
            PdfDocument doc = pdfRenderer.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter(pdfPath
                ));
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            AbstractIntegrationTest.ExtractionStrategy strategy = new AbstractIntegrationTest.ExtractionStrategy("Text1"
                );
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetPage(1));
            Color fillColor = strategy.GetFillColor();
            NUnit.Framework.Assert.AreEqual(fillColor, color);
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestNoisyImage() {
            String path = testImagesDirectory + "noisy_01.png";
            String expectedOutput1 = "Noisyimage to test Tesseract OCR";
            String expectedOutput2 = "Noisy image to test Tesseract OCR";
            String realOutputHocr = GetTextUsingTesseractFromImage(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Equals(expectedOutput1) || realOutputHocr.Equals(expectedOutput2
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestPantoneImage() {
            String filePath = testImagesDirectory + "pantone_blue.jpg";
            String expected = "";
            String realOutputHocr = GetTextUsingTesseractFromImage(tesseractReader, new FileInfo(filePath));
            NUnit.Framework.Assert.AreEqual(expected, realOutputHocr);
        }

        [NUnit.Framework.Test]
        public virtual void TestDifferentTextStyles() {
            String path = testImagesDirectory + "example_04.png";
            String expectedOutput = "How about a bigger font?";
            TestImageOcrText(tesseractReader, path, expectedOutput);
        }

        [NUnit.Framework.Test]
        public virtual void TestImageWithoutText() {
            String testName = "testImageWithoutText";
            String filePath = testImagesDirectory + "pantone_blue.jpg";
            String pdfPath = GetTargetDirectory() + testName + ".pdf";
            FileInfo file = new FileInfo(filePath);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader);
            pdfRenderer.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), new PdfWriter(pdfPath)).Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            AbstractIntegrationTest.ExtractionStrategy strategy = new AbstractIntegrationTest.ExtractionStrategy("Text Layer"
                );
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetFirstPage());
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual("", strategy.GetResultantText());
        }

        [LogMessage(Tesseract4LogMessageConstant.CannotReadInputImage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestInputInvalidImage() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file1 = new FileInfo(testImagesDirectory + "example.txt");
                FileInfo file2 = new FileInfo(testImagesDirectory + "example_05_corrupted.bmp");
                FileInfo file3 = new FileInfo(testImagesDirectory + "numbers_02.jpg");
                tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPathToTessData
                    (GetTessDataDirectory()));
                PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader);
                pdfRenderer.CreatePdf(JavaUtil.ArraysAsList(file3, file1, file2, file3), GetPdfWriter());
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(Tesseract4OcrException.IncorrectInputImageFormat, "txt")))
;
        }

        [LogMessage(Tesseract4OcrException.CannotFindPathToTessDataDirectory, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestNullPathToTessData() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPathToTessData
                    (null));
                GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("eng"));
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(Tesseract4OcrException.CannotFindPathToTessDataDirectory))
;
        }

        [LogMessage(Tesseract4OcrException.IncorrectLanguage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestPathToTessDataWithoutData() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPathToTessData
                    ("test/"));
                GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("eng"));
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(Tesseract4OcrException.IncorrectLanguage, "eng.traineddata", "test/")))
;
        }

        [LogMessage(Tesseract4OcrException.CannotFindPathToTessDataDirectory, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestIncorrectPathToTessData3() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPathToTessData
                    (""));
                GetTextFromPdf(tesseractReader, file);
                NUnit.Framework.Assert.AreEqual("", tesseractReader.GetTesseract4OcrEngineProperties().GetPathToTessData()
                    );
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(Tesseract4OcrException.CannotFindPathToTessDataDirectory))
;
        }

        [NUnit.Framework.Test]
        public virtual void TestTxtStringOutput() {
            FileInfo file = new FileInfo(testImagesDirectory + "multipage.tiff");
            IList<String> expectedOutput = JavaUtil.ArraysAsList("Multipage\nTIFF\nExample\nPage 1", "Multipage\nTIFF\nExample\nPage 2"
                , "Multipage\nTIFF\nExample\nPage 4", "Multipage\nTIFF\nExample\nPage 5", "Multipage\nTIFF\nExample\nPage 6"
                , "Multipage\nTIFF\nExample\nPage /", "Multipage\nTIFF\nExample\nPage 8", "Multipage\nTIFF\nExample\nPage 9"
                );
            String result = tesseractReader.DoImageOcr(file, OutputFormat.TXT);
            foreach (String line in expectedOutput) {
                NUnit.Framework.Assert.IsTrue(iText.IO.Util.StringUtil.ReplaceAll(result, "\r", "").Contains(line));
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestHocrStringOutput() {
            FileInfo file = new FileInfo(testImagesDirectory + "multipage.tiff");
            IList<String> expectedOutput = JavaUtil.ArraysAsList("Multipage\nTIFF\nExample\nPage 1", "Multipage\nTIFF\nExample\nPage 2"
                , "Multipage\nTIFF\nExample\nPage 4", "Multipage\nTIFF\nExample\nPage 5", "Multipage\nTIFF\nExample\nPage 6"
                , "Multipage\nTIFF\nExample\nPage /", "Multipage\nTIFF\nExample\nPage 8", "Multipage\nTIFF\nExample\nPage 9"
                );
            String result = tesseractReader.DoImageOcr(file, OutputFormat.HOCR);
            foreach (String line in expectedOutput) {
                NUnit.Framework.Assert.IsTrue(iText.IO.Util.StringUtil.ReplaceAll(result, "\r", "").Contains(line));
            }
        }

        [LogMessage(Tesseract4OcrException.IncorrectLanguage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestIncorrectLanguage() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("spa_new"));
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(Tesseract4OcrException.IncorrectLanguage, "spa_new.traineddata", langTessDataDirectory)))
;
        }

        [LogMessage(Tesseract4OcrException.IncorrectLanguage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestListOfLanguagesWithOneIncorrectLanguage() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("spa", "spa_new", "spa_old"));
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(Tesseract4OcrException.IncorrectLanguage, "spa_new.traineddata", langTessDataDirectory)))
;
        }

        [LogMessage(Tesseract4OcrException.IncorrectLanguage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestIncorrectScriptsName() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPathToTessData
                    (scriptTessDataDirectory));
                GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("English"));
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(Tesseract4OcrException.IncorrectLanguage, "English.traineddata", scriptTessDataDirectory)))
;
        }

        [LogMessage(Tesseract4OcrException.IncorrectLanguage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestListOfScriptsWithOneIncorrect() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPathToTessData
                    (scriptTessDataDirectory));
                GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("Georgian", "Japanese", "English"));
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(Tesseract4OcrException.IncorrectLanguage, "English.traineddata", scriptTessDataDirectory)))
;
        }

        /// <summary>Parse text from image and compare with expected.</summary>
        private void TestImageOcrText(Tesseract4OcrEngine tesseractReader, String path, String expectedOutput) {
            FileInfo ex1 = new FileInfo(path);
            String realOutputHocr = GetTextUsingTesseractFromImage(tesseractReader, ex1);
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        /// <summary>Parse text from given image using tesseract.</summary>
        private String GetTextUsingTesseractFromImage(IOcrEngine tesseractReader, FileInfo file) {
            int page = 1;
            IDictionary<int, IList<TextInfo>> data = tesseractReader.DoImageOcr(file);
            IList<TextInfo> pageText = data.Get(page);
            if (pageText == null || pageText.Count == 0) {
                pageText = new List<TextInfo>();
                TextInfo textInfo = new TextInfo();
                textInfo.SetBbox(JavaUtil.ArraysAsList(0f, 0f, 0f, 0f));
                textInfo.SetText("");
                pageText.Add(textInfo);
            }
            NUnit.Framework.Assert.AreEqual(4, pageText[0].GetBbox().Count);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TextInfo text in pageText) {
                stringBuilder.Append(text.GetText());
                stringBuilder.Append(" ");
            }
            return stringBuilder.ToString().Trim();
        }
    }
}
