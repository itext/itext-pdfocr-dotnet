using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Layer;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4;

namespace iText.Pdfocr.Pdflayers {
    public abstract class PdfLayersIntegrationTest : AbstractIntegrationTest {
        internal AbstractTesseract4OcrEngine tesseractReader;

        public PdfLayersIntegrationTest(AbstractIntegrationTest.ReaderType type) {
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPdfLayersFromMultiPageTiff() {
            String testName = "testTextFromPdfLayersFromMultiPageTiff";
            bool preprocess = tesseractReader.GetTesseract4OcrEngineProperties().IsPreprocessingImages();
            String path = testImagesDirectory + "multipage.tiff";
            String pdfPath = GetTargetDirectory() + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages
                (false));
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader);
            PdfDocument doc = ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter(
                pdfPath));
            NUnit.Framework.Assert.IsNotNull(doc);
            int numOfPages = doc.GetNumberOfPages();
            IList<PdfLayer> layers = doc.GetCatalog().GetOCProperties(true).GetLayers();
            NUnit.Framework.Assert.AreEqual(numOfPages * 2, layers.Count);
            NUnit.Framework.Assert.AreEqual("Image Layer", layers[2].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.AreEqual("Text Layer", layers[3].GetPdfObject().Get(PdfName.Name).ToString());
            doc.Close();
            // Text layer should contain all text
            // Image layer shouldn't contain any text
            String expectedOutput = "Multipage\nTIFF\nExample\nPage 5";
            NUnit.Framework.Assert.AreEqual(expectedOutput, GetTextFromPdfLayer(pdfPath, "Text Layer", 5));
            NUnit.Framework.Assert.AreEqual("", GetTextFromPdfLayer(pdfPath, "Image Layer", 5));
            NUnit.Framework.Assert.IsFalse(tesseractReader.GetTesseract4OcrEngineProperties().IsPreprocessingImages());
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages
                (preprocess));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPdfLayersFromMultiPagePdf() {
            String testName = "testTextFromPdfLayersFromMultiPagePdf";
            String pdfPath = GetTargetDirectory() + testName + ".pdf";
            IList<FileInfo> files = JavaUtil.ArraysAsList(new FileInfo(testImagesDirectory + "german_01.jpg"), new FileInfo
                (testImagesDirectory + "noisy_01.png"), new FileInfo(testImagesDirectory + "numbers_01.jpg"), new FileInfo
                (testImagesDirectory + "example_04.png"));
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetImageLayerName("image");
            properties.SetTextLayerName("text");
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader, properties);
            PdfDocument doc = ocrPdfCreator.CreatePdf(files, GetPdfWriter(pdfPath));
            NUnit.Framework.Assert.IsNotNull(doc);
            int numOfPages = doc.GetNumberOfPages();
            NUnit.Framework.Assert.AreEqual(numOfPages, files.Count);
            IList<PdfLayer> layers = doc.GetCatalog().GetOCProperties(true).GetLayers();
            NUnit.Framework.Assert.AreEqual(numOfPages * 2, layers.Count);
            NUnit.Framework.Assert.AreEqual("image", layers[2].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.AreEqual("text", layers[3].GetPdfObject().Get(PdfName.Name).ToString());
            doc.Close();
            // Text layer should contain all text
            // Image layer shouldn't contain any text
            String expectedOutput = "619121";
            NUnit.Framework.Assert.AreEqual(expectedOutput, GetTextFromPdfLayer(pdfPath, "text", 3));
            NUnit.Framework.Assert.AreEqual("", GetTextFromPdfLayer(pdfPath, "image", 3));
        }
    }
}
