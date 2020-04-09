using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Layer;
using iText.Ocr;

namespace iText.Ocr.Pdflayers {
    public abstract class PdfLayersIntegrationTest : AbstractIntegrationTest {
        internal TesseractReader tesseractReader;

        internal String parameter;

        public PdfLayersIntegrationTest(String type) {
            parameter = type;
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfLayersWithDefaultNames() {
            String path = testImagesDirectory + "numbers_01.jpg";
            FileInfo file = new FileInfo(path);
            IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader);
            pdfRenderer.SetInputImages(JavaCollectionsUtil.SingletonList<FileInfo>(file));
            PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter());
            NUnit.Framework.Assert.IsNotNull(doc);
            IList<PdfLayer> layers = doc.GetCatalog().GetOCProperties(true).GetLayers();
            NUnit.Framework.Assert.AreEqual(2, layers.Count);
            NUnit.Framework.Assert.AreEqual("Image Layer", layers[0].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.AreEqual("Text Layer", layers[1].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.AreEqual(1, pdfRenderer.GetInputImages().Count);
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfLayersWithCustomNames() {
            String path = testImagesDirectory + "numbers_01.jpg";
            FileInfo file = new FileInfo(path);
            IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader);
            pdfRenderer.SetInputImages(JavaCollectionsUtil.SingletonList<FileInfo>(file));
            pdfRenderer.SetImageLayerName("name image 1");
            pdfRenderer.SetTextLayerName("name text 1");
            PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter());
            // setting layer's name after ocr was done, name shouldn't change
            pdfRenderer.SetImageLayerName("name image 100500");
            NUnit.Framework.Assert.IsNotNull(doc);
            IList<PdfLayer> layers = doc.GetCatalog().GetOCProperties(true).GetLayers();
            NUnit.Framework.Assert.AreEqual(2, layers.Count);
            NUnit.Framework.Assert.AreEqual("name image 1", layers[0].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.IsTrue(layers[0].IsOn());
            NUnit.Framework.Assert.AreEqual("name text 1", layers[1].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.IsTrue(layers[1].IsOn());
            NUnit.Framework.Assert.AreEqual(1, pdfRenderer.GetInputImages().Count);
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPdfLayers() {
            String path = testImagesDirectory + "numbers_01.jpg";
            String pdfPath = testDocumentsDirectory + System.Guid.NewGuid().ToString() + ".pdf";
            FileInfo file = new FileInfo(path);
            try {
                IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                    ));
                PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter(pdfPath));
                NUnit.Framework.Assert.IsNotNull(doc);
                IList<PdfLayer> layers = doc.GetCatalog().GetOCProperties(true).GetLayers();
                NUnit.Framework.Assert.AreEqual(2, layers.Count);
                NUnit.Framework.Assert.AreEqual("Image Layer", layers[0].GetPdfObject().Get(PdfName.Name).ToString());
                NUnit.Framework.Assert.IsTrue(layers[0].IsOn());
                NUnit.Framework.Assert.AreEqual("Text Layer", layers[1].GetPdfObject().Get(PdfName.Name).ToString());
                NUnit.Framework.Assert.IsTrue(layers[1].IsOn());
                doc.Close();
                // Text layer should contain all text
                // Image layer shouldn't contain any text
                String expectedOutput = "619121";
                NUnit.Framework.Assert.AreEqual(expectedOutput, GetTextFromPdfLayer(pdfPath, "Text Layer", 1));
                NUnit.Framework.Assert.AreEqual("", GetTextFromPdfLayer(pdfPath, "Image Layer", 1));
            }
            finally {
                DeleteFile(pdfPath);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPdfLayersFromMultiPageTiff() {
            bool preprocess = tesseractReader.IsPreprocessingImages();
            String path = testImagesDirectory + "multipage.tiff";
            String pdfPath = testDocumentsDirectory + System.Guid.NewGuid().ToString() + ".pdf";
            FileInfo file = new FileInfo(path);
            try {
                tesseractReader.SetPreprocessingImages(false);
                IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                    ));
                PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter(pdfPath));
                NUnit.Framework.Assert.IsNotNull(doc);
                int numOfPages = doc.GetNumberOfPages();
                IList<PdfLayer> layers = doc.GetCatalog().GetOCProperties(true).GetLayers();
                NUnit.Framework.Assert.AreEqual(numOfPages * 2, layers.Count);
                NUnit.Framework.Assert.AreEqual("Image Layer", layers[2].GetPdfObject().Get(PdfName.Name).ToString());
                NUnit.Framework.Assert.AreEqual("Text Layer", layers[3].GetPdfObject().Get(PdfName.Name).ToString());
                doc.Close();
                //             Text layer should contain all text
                //             Image layer shouldn't contain any text
                String expectedOutput = "Multipage\nTIFF\nExample\nPage 5";
                NUnit.Framework.Assert.AreEqual(expectedOutput, GetTextFromPdfLayer(pdfPath, "Text Layer", 5));
                NUnit.Framework.Assert.AreEqual("", GetTextFromPdfLayer(pdfPath, "Image Layer", 5));
                NUnit.Framework.Assert.IsFalse(tesseractReader.IsPreprocessingImages());
            }
            finally {
                DeleteFile(pdfPath);
                tesseractReader.SetPreprocessingImages(preprocess);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPdfLayersFromMultiPagePdf() {
            String pdfPath = testImagesDirectory + System.Guid.NewGuid().ToString() + ".pdf";
            IList<FileInfo> files = JavaUtil.ArraysAsList(new FileInfo(testImagesDirectory + "german_01.jpg"), new FileInfo
                (testImagesDirectory + "noisy_01.png"), new FileInfo(testImagesDirectory + "numbers_01.jpg"), new FileInfo
                (testImagesDirectory + "example_04.png"));
            IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, files);
            pdfRenderer.SetImageLayerName("image");
            pdfRenderer.SetTextLayerName("text");
            PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter(pdfPath));
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
            NUnit.Framework.Assert.AreEqual(4, pdfRenderer.GetInputImages().Count);
            DeleteFile(pdfPath);
        }
    }
}
