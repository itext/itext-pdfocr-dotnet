using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common.Logging;
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Ocr;
using iText.Test.Attributes;

namespace iText.Ocr.General {
    public abstract class BasicTesseractIntegrationTest : AbstractIntegrationTest {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Ocr.General.BasicTesseractIntegrationTest
            ));

        internal TesseractReader tesseractReader;

        [NUnit.Framework.SetUp]
        public virtual void InitTessDataPath() {
            tesseractReader.SetPathToTessData(GetTessDataDirectory());
            tesseractReader.SetLanguages(new List<String>());
        }

        public BasicTesseractIntegrationTest(AbstractIntegrationTest.ReaderType type) {
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.Test]
        public virtual void TestFontColorInMultiPagePdf() {
            String testName = "testFontColorInMultiPagePdf";
            String path = testImagesDirectory + "multipage.tiff";
            String pdfPath = testImagesDirectory + testName + ".pdf";
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
        public virtual void TestKeepOriginalSizeScaleMode() {
            String filePath = testImagesDirectory + "numbers_01.jpg";
            FileInfo file = new FileInfo(filePath);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader);
            PdfDocument doc = pdfRenderer.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter());
            NUnit.Framework.Assert.IsNotNull(doc);
            ImageData imageData = ImageDataFactory.Create(file.FullName);
            float imageWidth = GetPoints(imageData.GetWidth());
            float imageHeight = GetPoints(imageData.GetHeight());
            float realWidth = doc.GetFirstPage().GetPageSize().GetWidth();
            float realHeight = doc.GetFirstPage().GetPageSize().GetHeight();
            NUnit.Framework.Assert.AreEqual(imageWidth, realWidth, delta);
            NUnit.Framework.Assert.AreEqual(imageHeight, realHeight, delta);
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestScaleWidthMode() {
            String testName = "testScaleWidthMode";
            String srcPath = testImagesDirectory + "numbers_01.jpg";
            String pdfPath = testImagesDirectory + testName + ".pdf";
            FileInfo file = new FileInfo(srcPath);
            float pageWidthPt = 400f;
            float pageHeightPt = 400f;
            Rectangle pageSize = new Rectangle(pageWidthPt, pageHeightPt);
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetScaleMode(ScaleMode.SCALE_WIDTH);
            properties.SetPageSize(pageSize);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, properties);
            PdfDocument doc = pdfRenderer.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter(pdfPath
                ));
            doc.Close();
            Rectangle rect = GetImageBBoxRectangleFromPdf(pdfPath);
            ImageData originalImageData = ImageDataFactory.Create(file.FullName);
            // page size should be equal to the result image size
            // result image height should be equal to the value that
            // was set as page height result image width should be scaled
            // proportionally according to the provided image height
            // and original image size
            NUnit.Framework.Assert.AreEqual(pageHeightPt, rect.GetHeight(), delta);
            NUnit.Framework.Assert.AreEqual(originalImageData.GetWidth() / originalImageData.GetHeight(), rect.GetWidth
                () / rect.GetHeight(), delta);
        }

        [NUnit.Framework.Test]
        public virtual void TestScaleHeightMode() {
            String testName = "testScaleHeightMode";
            String srcPath = testImagesDirectory + "numbers_01.jpg";
            String pdfPath = testImagesDirectory + testName + ".pdf";
            FileInfo file = new FileInfo(srcPath);
            float pageWidthPt = 400f;
            float pageHeightPt = 400f;
            Rectangle pageSize = new Rectangle(pageWidthPt, pageHeightPt);
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetScaleMode(ScaleMode.SCALE_HEIGHT);
            properties.SetPageSize(pageSize);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, properties);
            PdfDocument doc = pdfRenderer.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter(pdfPath
                ));
            doc.Close();
            Rectangle rect = GetImageBBoxRectangleFromPdf(pdfPath);
            ImageData originalImageData = ImageDataFactory.Create(file.FullName);
            NUnit.Framework.Assert.AreEqual(pageWidthPt, rect.GetWidth(), delta);
            NUnit.Framework.Assert.AreEqual(originalImageData.GetWidth() / originalImageData.GetHeight(), rect.GetWidth
                () / rect.GetHeight(), delta);
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
        public virtual void TestFontColor() {
            String testName = "testFontColor";
            String path = testImagesDirectory + "numbers_01.jpg";
            String pdfPath = testImagesDirectory + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetTextLayerName("Text1");
            Color color = DeviceCmyk.CYAN;
            properties.SetTextColor(color);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, properties);
            PdfDocument doc = pdfRenderer.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter(pdfPath
                ));
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            AbstractIntegrationTest.ExtractionStrategy strategy = new AbstractIntegrationTest.ExtractionStrategy("Text1"
                );
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetFirstPage());
            pdfDocument.Close();
            Color fillColor = strategy.GetFillColor();
            NUnit.Framework.Assert.AreEqual(color, fillColor);
        }

        [NUnit.Framework.Test]
        public virtual void TestImageWithoutText() {
            String testName = "testImageWithoutText";
            String filePath = testImagesDirectory + "pantone_blue.jpg";
            String pdfPath = testImagesDirectory + testName + ".pdf";
            FileInfo file = new FileInfo(filePath);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader);
            PdfDocument doc = pdfRenderer.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), new PdfWriter(pdfPath
                ));
            NUnit.Framework.Assert.IsNotNull(doc);
            ImageData imageData = ImageDataFactory.Create(file.FullName);
            PageSize defaultPageSize = PageSize.A4;
            iText.Layout.Element.Image resultImage = GetImageFromPdf(tesseractReader, file, ScaleMode.SCALE_TO_FIT, defaultPageSize
                );
            // TODO
            /*if (imageData != null) {
            float imageWidth = getPoints(imageData.getWidth());
            float imageHeight = getPoints(imageData.getHeight());
            float realImageWidth = resultImage.getImageWidth();
            float realImageHeight = resultImage.getImageHeight();
            
            float realWidth = doc.getFirstPage().getPageSize().getWidth();
            float realHeight = doc.getFirstPage().getPageSize().getHeight();
            
            Assert.assertEquals(imageWidth / imageHeight,
            realImageWidth / realImageHeight, delta);
            Assert.assertEquals(defaultPageSize.getHeight(), realHeight, delta);
            Assert.assertEquals(defaultPageSize.getWidth(), realWidth, delta);
            }*/
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            AbstractIntegrationTest.ExtractionStrategy strategy = new AbstractIntegrationTest.ExtractionStrategy("Text Layer"
                );
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetFirstPage());
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual("", strategy.GetResultantText());
        }

        [LogMessage(LogMessageConstant.CannotReadInputImage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestInputInvalidImage() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file1 = new FileInfo(testImagesDirectory + "example.txt");
                FileInfo file2 = new FileInfo(testImagesDirectory + "example_05_corrupted.bmp");
                FileInfo file3 = new FileInfo(testImagesDirectory + "numbers_02.jpg");
                tesseractReader.SetPathToTessData(GetTessDataDirectory());
                PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader);
                pdfRenderer.CreatePdf(JavaUtil.ArraysAsList(file3, file1, file2, file3), GetPdfWriter());
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(OcrException.IncorrectInputImageFormat, "txt")))
;
        }

        [LogMessage(OcrException.CannotFindPathToTessDataDirectory, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestNullPathToTessData() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                tesseractReader.SetPathToTessData(null);
                GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("eng"));
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>().With.Message.EqualTo(OcrException.CannotFindPathToTessDataDirectory))
;
        }

        [LogMessage(OcrException.IncorrectLanguage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestPathToTessDataWithoutData() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                tesseractReader.SetPathToTessData("test/");
                GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("eng"));
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(OcrException.IncorrectLanguage, "eng.traineddata", "test/")))
;
        }

        [LogMessage(OcrException.CannotFindPathToTessDataDirectory, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestIncorrectPathToTessData3() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                tesseractReader.SetPathToTessData("");
                GetTextFromPdf(tesseractReader, file);
                NUnit.Framework.Assert.AreEqual("", tesseractReader.GetPathToTessData());
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>().With.Message.EqualTo(OcrException.CannotFindPathToTessDataDirectory))
;
        }

        [NUnit.Framework.Test]
        public virtual void TestSimpleTextOutput() {
            String imgPath = testImagesDirectory + "numbers_01.jpg";
            String expectedOutput = "619121";
            String result = GetRecognizedTextFromTextFile(tesseractReader, imgPath);
            NUnit.Framework.Assert.IsTrue(result.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void TestTxtStringOutput() {
            FileInfo file = new FileInfo(testImagesDirectory + "multipage.tiff");
            IList<String> expectedOutput = JavaUtil.ArraysAsList("Multipage\nTIFF\nExample\nPage 1", "Multipage\nTIFF\nExample\nPage 2"
                , "Multipage\nTIFF\nExample\nPage 4", "Multipage\nTIFF\nExample\nPage 5", "Multipage\nTIFF\nExample\nPage 6"
                , "Multipage\nTIFF\nExample\nPage /", "Multipage\nTIFF\nExample\nPage 8", "Multipage\nTIFF\nExample\nPage 9"
                );
            String result = tesseractReader.DoImageOcr(file, IOcrReader.OutputFormat.TXT);
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
            String result = tesseractReader.DoImageOcr(file, IOcrReader.OutputFormat.HOCR);
            foreach (String line in expectedOutput) {
                NUnit.Framework.Assert.IsTrue(iText.IO.Util.StringUtil.ReplaceAll(result, "\r", "").Contains(line));
            }
        }

        [LogMessage(LogMessageConstant.CannotReadProvidedFont, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestInvalidFont() {
            String testName = "testImageWithoutText";
            String expectedOutput = "619121";
            String path = testImagesDirectory + "numbers_01.jpg";
            String pdfPath = testImagesDirectory + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetFontPath("font.ttf");
            properties.SetScaleMode(ScaleMode.SCALE_TO_FIT);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, properties);
            PdfDocument doc = pdfRenderer.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter(pdfPath
                ));
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            String result = GetTextFromPdfLayer(pdfPath, "Text Layer", 1);
            NUnit.Framework.Assert.AreEqual(expectedOutput, result);
            NUnit.Framework.Assert.AreEqual(ScaleMode.SCALE_TO_FIT, pdfRenderer.GetOcrPdfCreatorProperties().GetScaleMode
                ());
        }

        [LogMessage(OcrException.IncorrectLanguage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestIncorrectLanguage() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("spa_new"));
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(OcrException.IncorrectLanguage, "spa_new.traineddata", langTessDataDirectory)))
;
        }

        [LogMessage(OcrException.IncorrectLanguage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestListOfLanguagesWithOneIncorrectLanguage() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("spa", "spa_new", "spa_old"));
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(OcrException.IncorrectLanguage, "spa_new.traineddata", langTessDataDirectory)))
;
        }

        [LogMessage(OcrException.IncorrectLanguage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestIncorrectScriptsName() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                tesseractReader.SetPathToTessData(scriptTessDataDirectory);
                GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("English"));
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(OcrException.IncorrectLanguage, "English.traineddata", scriptTessDataDirectory)))
;
        }

        [LogMessage(OcrException.IncorrectLanguage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestListOfScriptsWithOneIncorrect() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
                tesseractReader.SetPathToTessData(scriptTessDataDirectory);
                GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("Georgian", "Japanese", "English"));
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(OcrException.IncorrectLanguage, "English.traineddata", scriptTessDataDirectory)))
;
        }

        [LogMessage(LogMessageConstant.CannotReadInputImage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestCorruptedImage() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "corrupted.jpg");
                String realOutput = GetTextFromPdf(tesseractReader, file);
                NUnit.Framework.Assert.IsNotNull(realOutput);
                NUnit.Framework.Assert.AreEqual("", realOutput);
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>())
;
        }

        [LogMessage(LogMessageConstant.CannotReadInputImage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestCorruptedImageWithoutExtesion() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "corrupted");
                String realOutput = GetTextFromPdf(tesseractReader, file);
                NUnit.Framework.Assert.IsNotNull(realOutput);
                NUnit.Framework.Assert.AreEqual("", realOutput);
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>())
;
        }

        /// <summary>Parse text from image and compare with expected.</summary>
        private void TestImageOcrText(TesseractReader tesseractReader, String path, String expectedOutput) {
            FileInfo ex1 = new FileInfo(path);
            String realOutputHocr = GetTextUsingTesseractFromImage(tesseractReader, ex1);
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        /// <summary>Parse text from given image using tesseract.</summary>
        private String GetTextUsingTesseractFromImage(IOcrReader tesseractReader, FileInfo file) {
            int page = 1;
            IDictionary<int, IList<TextInfo>> data = tesseractReader.DoImageOcr(file);
            IList<TextInfo> pageText = data.Get(page);
            if (pageText == null || pageText.Count == 0) {
                pageText = new List<TextInfo>();
                TextInfo textInfo = new TextInfo();
                textInfo.SetCoordinates(JavaUtil.ArraysAsList(0f, 0f, 0f, 0f));
                textInfo.SetText("");
                pageText.Add(textInfo);
            }
            NUnit.Framework.Assert.AreEqual(4, pageText[0].GetCoordinates().Count);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TextInfo text in pageText) {
                stringBuilder.Append(text.GetText());
                stringBuilder.Append(" ");
            }
            return stringBuilder.ToString().Trim();
        }
    }
}
