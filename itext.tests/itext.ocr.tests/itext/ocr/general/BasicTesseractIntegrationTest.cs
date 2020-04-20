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

namespace iText.Ocr.General {
    public abstract class BasicTesseractIntegrationTest : AbstractIntegrationTest {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Ocr.General.BasicTesseractIntegrationTest
            ));

        internal TesseractReader tesseractReader;

        internal String parameter;

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
                tesseractReader.SetPathToTessData(GetTessDataDirectory());
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
            pdfRenderer.SetScaleMode(IPdfRenderer.ScaleMode.keepOriginalSize);
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
                NUnit.Framework.Assert.AreEqual(IPdfRenderer.ScaleMode.keepOriginalSize, pdfRenderer.GetScaleMode());
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
            iText.Layout.Element.Image resultImage = GetImageFromPdf(tesseractReader, file, IPdfRenderer.ScaleMode.scaleWidth
                , pageSize);
            // page size should be equal to the result image size
            // result image height should be equal to the value that
            // was set as page height result image width should be scaled
            // proportionally according to the provided image height
            // and original image size
            if (originalImageData != null) {
                float originalImageHeight = UtilService.GetPoints(originalImageData.GetHeight());
                float originalImageWidth = UtilService.GetPoints(originalImageData.GetWidth());
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
            iText.Layout.Element.Image resultImage = GetImageFromPdf(tesseractReader, file, IPdfRenderer.ScaleMode.scaleHeight
                , pageSize);
            if (originalImageData != null) {
                float originalImageHeight = UtilService.GetPoints(originalImageData.GetHeight());
                float originalImageWidth = UtilService.GetPoints(originalImageData.GetWidth());
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
            iText.Layout.Element.Image resultImage = GetImageFromPdf(tesseractReader, file, IPdfRenderer.ScaleMode.scaleToFit
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

        [NUnit.Framework.Test]
        public virtual void TestInputInvalidImage() {
            FileInfo file1 = new FileInfo(testImagesDirectory + "example.txt");
            FileInfo file2 = new FileInfo(testImagesDirectory + "example_05_corrupted.bmp");
            FileInfo file3 = new FileInfo(testImagesDirectory + "numbers_02.jpg");
            try {
                tesseractReader.SetPathToTessData(GetTessDataDirectory());
                IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaUtil.ArraysAsList(file3, file1, file2, file3
                    ));
                pdfRenderer.DoPdfOcr(GetPdfWriter());
            }
            catch (OCRException e) {
                String expectedMsg = String.Format(OCRException.INCORRECT_INPUT_IMAGE_FORMAT, "txt");
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
            }
            tesseractReader.SetPathToTessData(GetTessDataDirectory());
        }

        [NUnit.Framework.Test]
        public virtual void TestIncorrectPathToTessData() {
            FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
            try {
                tesseractReader.SetPathToTessData(null);
                GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("eng"));
            }
            catch (OCRException e) {
                NUnit.Framework.Assert.AreEqual(OCRException.CANNOT_FIND_PATH_TO_TESSDATA, e.Message);
                tesseractReader.SetPathToTessData(GetTessDataDirectory());
            }
            try {
                tesseractReader.SetPathToTessData("test/");
                GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("eng"));
            }
            catch (OCRException e) {
                String expectedMsg = String.Format(OCRException.INCORRECT_LANGUAGE, "eng.traineddata", "test/");
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
            }
            tesseractReader.SetPathToTessData(GetTessDataDirectory());
            try {
                GetTextFromPdf(tesseractReader, file);
            }
            catch (OCRException e) {
                String expectedMsg = String.Format(OCRException.INCORRECT_LANGUAGE, "eng.traineddata", langTessDataDirectory
                    );
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
            }
            tesseractReader.SetPathToTessData(GetTessDataDirectory());
        }

        [NUnit.Framework.Test]
        public virtual void TestSimpleTextOutput() {
            String imgPath = testImagesDirectory + "numbers_01.jpg";
            String expectedOutput = "619121";
            String result = GetOCRedTextFromTextFile(tesseractReader, imgPath);
            NUnit.Framework.Assert.IsTrue(result.Contains(expectedOutput));
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
            if (pageText.Count > 0) {
                NUnit.Framework.Assert.AreEqual(4, pageText[0].GetCoordinates().Count);
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TextInfo text in pageText) {
                stringBuilder.Append(text.GetText());
                stringBuilder.Append(" ");
            }
            return stringBuilder.ToString().Trim();
        }
    }
}
