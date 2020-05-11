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

        internal String parameter;

        [NUnit.Framework.SetUp]
        public virtual void InitTessDataPath() {
            tesseractReader.SetPathToTessData(GetTessDataDirectory());
            tesseractReader.SetLanguages(new List<String>());
        }

        public BasicTesseractIntegrationTest(String type) {
            parameter = type;
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.Test]
        public virtual void TestFontColorInMultiPagePdf() {
            String path = testImagesDirectory + "multipage.tiff";
            String pdfPath = testImagesDirectory + Guid.NewGuid().ToString() + ".pdf";
            FileInfo file = new FileInfo(path);
            try {
                IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                    ));
                pdfRenderer.SetTextLayerName("Text1");
                Color color = DeviceCmyk.MAGENTA;
                pdfRenderer.SetTextColor(color);
                PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter(pdfPath));
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
            finally {
                DeleteFile(pdfPath);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestKeepOriginalSizeScaleMode() {
            String filePath = testImagesDirectory + "numbers_01.jpg";
            FileInfo file = new FileInfo(filePath);
            IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                ));
            pdfRenderer.SetScaleMode(IPdfRenderer.ScaleMode.KEEP_ORIGINAL_SIZE);
            PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter());
            NUnit.Framework.Assert.IsNotNull(doc);
            ImageData imageData = null;
            try {
                imageData = ImageDataFactory.Create(file.FullName);
            }
            catch (UriFormatException e) {
                LOGGER.Error(e.Message);
            }
            if (imageData != null) {
                float imageWidth = UtilService.GetPoints(imageData.GetWidth());
                float imageHeight = UtilService.GetPoints(imageData.GetHeight());
                float realWidth = doc.GetFirstPage().GetPageSize().GetWidth();
                float realHeight = doc.GetFirstPage().GetPageSize().GetHeight();
                NUnit.Framework.Assert.AreEqual(imageWidth, realWidth, delta);
                NUnit.Framework.Assert.AreEqual(imageHeight, realHeight, delta);
                NUnit.Framework.Assert.AreEqual(IPdfRenderer.ScaleMode.KEEP_ORIGINAL_SIZE, pdfRenderer.GetScaleMode());
            }
            if (!doc.IsClosed()) {
                doc.Close();
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestScaleWidthMode() {
            String filePath = testImagesDirectory + "numbers_01.jpg";
            FileInfo file = new FileInfo(filePath);
            ImageData originalImageData = null;
            try {
                originalImageData = ImageDataFactory.Create(file.FullName);
            }
            catch (UriFormatException e) {
                LOGGER.Error(e.Message);
            }
            float pageWidthPt = 500f;
            float pageHeightPt = 500f;
            Rectangle pageSize = new Rectangle(pageWidthPt, pageHeightPt);
            // page size should be equal to the result image size
            // result image height should be equal to the value that
            // was set as page height result image width should be scaled
            // proportionally according to the provided image height
            // and original image size
            if (originalImageData != null) {
                float resultPageWidth = pageSize.GetWidth();
                float resultPageHeight = pageSize.GetHeight();
                NUnit.Framework.Assert.AreEqual(resultPageWidth, pageWidthPt, delta);
                NUnit.Framework.Assert.AreEqual(resultPageHeight, pageHeightPt, delta);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestScaleHeightMode() {
            String filePath = testImagesDirectory + "numbers_01.jpg";
            FileInfo file = new FileInfo(filePath);
            ImageData originalImageData = null;
            try {
                originalImageData = ImageDataFactory.Create(file.FullName);
            }
            catch (UriFormatException e) {
                LOGGER.Error(e.Message);
            }
            float pageWidthPt = 500f;
            float pageHeightPt = 500f;
            Rectangle pageSize = new Rectangle(pageWidthPt, pageHeightPt);
            iText.Layout.Element.Image resultImage = GetImageFromPdf(tesseractReader, file, IPdfRenderer.ScaleMode.SCALE_HEIGHT
                , pageSize);
            if (originalImageData != null) {
                float resultPageWidth = pageSize.GetWidth();
                float resultPageHeight = pageSize.GetHeight();
                NUnit.Framework.Assert.AreEqual(resultPageWidth, pageWidthPt, delta);
                NUnit.Framework.Assert.AreEqual(resultPageHeight, pageHeightPt, delta);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestScaleToFitMode() {
            String filePath = testImagesDirectory + "numbers_01.jpg";
            FileInfo file = new FileInfo(filePath);
            IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                ));
            PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter());
            NUnit.Framework.Assert.IsNotNull(doc);
            float realPageWidth = doc.GetFirstPage().GetPageSize().GetWidth();
            float realPageHeight = doc.GetFirstPage().GetPageSize().GetHeight();
            NUnit.Framework.Assert.AreEqual(PageSize.A4.GetWidth(), realPageWidth, delta);
            NUnit.Framework.Assert.AreEqual(PageSize.A4.GetHeight(), realPageHeight, delta);
            if (!doc.IsClosed()) {
                doc.Close();
            }
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
            String path = testImagesDirectory + "numbers_01.jpg";
            String pdfPath = testImagesDirectory + Guid.NewGuid().ToString() + ".pdf";
            FileInfo file = new FileInfo(path);
            IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                ));
            pdfRenderer.SetTextLayerName("Text1");
            Color color = DeviceCmyk.CYAN;
            pdfRenderer.SetTextColor(color);
            PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter(pdfPath));
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            AbstractIntegrationTest.ExtractionStrategy strategy = new AbstractIntegrationTest.ExtractionStrategy("Text1"
                );
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetFirstPage());
            try {
                Color fillColor = strategy.GetFillColor();
                NUnit.Framework.Assert.AreEqual(color, fillColor);
            }
            finally {
                pdfDocument.Close();
                DeleteFile(pdfPath);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestImageWithoutText() {
            String filePath = testImagesDirectory + "pantone_blue.jpg";
            String pdfPath = testImagesDirectory + Guid.NewGuid().ToString() + ".pdf";
            FileInfo file = new FileInfo(filePath);
            IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                ));
            PdfDocument doc = pdfRenderer.DoPdfOcr(new PdfWriter(pdfPath));
            NUnit.Framework.Assert.IsNotNull(doc);
            ImageData imageData = null;
            try {
                imageData = ImageDataFactory.Create(file.FullName);
            }
            catch (UriFormatException e) {
                LOGGER.Error(e.Message);
            }
            PageSize defaultPageSize = PageSize.A4;
            iText.Layout.Element.Image resultImage = GetImageFromPdf(tesseractReader, file, IPdfRenderer.ScaleMode.SCALE_TO_FIT
                , defaultPageSize);
            if (imageData != null) {
                float imageWidth = UtilService.GetPoints(imageData.GetWidth());
                float imageHeight = UtilService.GetPoints(imageData.GetHeight());
                float realImageWidth = resultImage.GetImageWidth();
                float realImageHeight = resultImage.GetImageHeight();
                float realWidth = doc.GetFirstPage().GetPageSize().GetWidth();
                float realHeight = doc.GetFirstPage().GetPageSize().GetHeight();
                NUnit.Framework.Assert.AreEqual(imageWidth / imageHeight, realImageWidth / realImageHeight, delta);
                NUnit.Framework.Assert.AreEqual(defaultPageSize.GetHeight(), realHeight, delta);
                NUnit.Framework.Assert.AreEqual(defaultPageSize.GetWidth(), realWidth, delta);
            }
            if (!doc.IsClosed()) {
                doc.Close();
            }
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            AbstractIntegrationTest.ExtractionStrategy strategy = new AbstractIntegrationTest.ExtractionStrategy("Text Layer"
                );
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetFirstPage());
            NUnit.Framework.Assert.AreEqual("", strategy.GetResultantText());
            pdfDocument.Close();
            DeleteFile(pdfPath);
        }

        [LogMessage(LogMessageConstant.CannotReadInputImage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestInputInvalidImage() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file1 = new FileInfo(testImagesDirectory + "example.txt");
                FileInfo file2 = new FileInfo(testImagesDirectory + "example_05_corrupted.bmp");
                FileInfo file3 = new FileInfo(testImagesDirectory + "numbers_02.jpg");
                tesseractReader.SetPathToTessData(GetTessDataDirectory());
                IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaUtil.ArraysAsList(file3, file1, file2, file3
                    ));
                pdfRenderer.DoPdfOcr(GetPdfWriter());
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
            String result = tesseractReader.ReadDataFromInput(file, IOcrReader.OutputFormat.TXT);
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
            String result = tesseractReader.ReadDataFromInput(file, IOcrReader.OutputFormat.HOCR);
            foreach (String line in expectedOutput) {
                NUnit.Framework.Assert.IsTrue(iText.IO.Util.StringUtil.ReplaceAll(result, "\r", "").Contains(line));
            }
        }

        [LogMessage(LogMessageConstant.CannotReadProvidedFont, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestInvalidFont() {
            String path = testImagesDirectory + "numbers_01.jpg";
            String expectedOutput = "619121";
            String pdfPath = testImagesDirectory + Guid.NewGuid().ToString() + ".pdf";
            FileInfo file = new FileInfo(path);
            IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                ));
            pdfRenderer.SetFontPath("font.ttf");
            PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter(pdfPath));
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            String result = GetTextFromPdfLayer(pdfPath, "Text Layer", 1);
            NUnit.Framework.Assert.AreEqual(expectedOutput, result);
            DeleteFile(pdfPath);
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
        /// <param name="path"/>
        /// <param name="expectedOutput"/>
        private void TestImageOcrText(TesseractReader tesseractReader, String path, String expectedOutput) {
            FileInfo ex1 = new FileInfo(path);
            String realOutputHocr = GetTextUsingTesseractFromImage(tesseractReader, ex1);
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        /// <summary>Parse text from given image using tesseract.</summary>
        /// <param name="tesseractReader"/>
        /// <param name="file"/>
        /// <returns/>
        private String GetTextUsingTesseractFromImage(IOcrReader tesseractReader, FileInfo file) {
            int page = 1;
            IDictionary<int, IList<TextInfo>> data = tesseractReader.ReadDataFromInput(file);
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
