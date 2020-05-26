using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Layer;
using iText.Pdfocr.Helpers;
using iText.Test;

namespace iText.Pdfocr {
    public class PdfLayersTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestPdfLayersWithDefaultNames() {
            String path = PdfHelper.GetDefaultImagePath();
            FileInfo file = new FileInfo(path);
            PdfRenderer pdfRenderer = new PdfRenderer(new CustomOcrEngine());
            PdfDocument doc = pdfRenderer.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), PdfHelper.GetPdfWriter
                ());
            NUnit.Framework.Assert.IsNotNull(doc);
            IList<PdfLayer> layers = doc.GetCatalog().GetOCProperties(true).GetLayers();
            NUnit.Framework.Assert.AreEqual(2, layers.Count);
            NUnit.Framework.Assert.AreEqual("Image Layer", layers[0].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.AreEqual("Text Layer", layers[1].GetPdfObject().Get(PdfName.Name).ToString());
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfLayersWithCustomNames() {
            String path = PdfHelper.GetDefaultImagePath();
            FileInfo file = new FileInfo(path);
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetImageLayerName("name image 1");
            properties.SetTextLayerName("name text 1");
            PdfRenderer pdfRenderer = new PdfRenderer(new CustomOcrEngine(), properties);
            PdfDocument doc = pdfRenderer.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), PdfHelper.GetPdfWriter
                ());
            NUnit.Framework.Assert.IsNotNull(doc);
            IList<PdfLayer> layers = doc.GetCatalog().GetOCProperties(true).GetLayers();
            NUnit.Framework.Assert.AreEqual(2, layers.Count);
            NUnit.Framework.Assert.AreEqual("name image 1", layers[0].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.IsTrue(layers[0].IsOn());
            NUnit.Framework.Assert.AreEqual("name text 1", layers[1].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.IsTrue(layers[1].IsOn());
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPdfLayers() {
            String testName = "testTextFromPdfLayers";
            String path = PdfHelper.GetDefaultImagePath();
            String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            PdfRenderer pdfRenderer = new PdfRenderer(new CustomOcrEngine());
            PdfDocument doc = pdfRenderer.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), PdfHelper.GetPdfWriter
                (pdfPath));
            NUnit.Framework.Assert.IsNotNull(doc);
            IList<PdfLayer> layers = doc.GetCatalog().GetOCProperties(true).GetLayers();
            NUnit.Framework.Assert.AreEqual(2, layers.Count);
            NUnit.Framework.Assert.AreEqual("Image Layer", layers[0].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.IsTrue(layers[0].IsOn());
            NUnit.Framework.Assert.AreEqual("Text Layer", layers[1].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.IsTrue(layers[1].IsOn());
            doc.Close();
            NUnit.Framework.Assert.AreEqual(PdfHelper.DEFAULT_EXPECTED_RESULT, PdfHelper.GetTextFromPdfLayer(pdfPath, 
                "Text Layer"));
            NUnit.Framework.Assert.AreEqual("", PdfHelper.GetTextFromPdfLayer(pdfPath, "Image Layer"));
        }
    }
}
