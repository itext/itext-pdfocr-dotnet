/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
Authors: iText Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
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
            OcrEngineProperties ocrEngineProperties = new OcrEngineProperties();
            ocrEngineProperties.SetLanguages(JavaCollectionsUtil.SingletonList<String>("eng"));
            CustomOcrEngine engine = new CustomOcrEngine(ocrEngineProperties);
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(engine);
            PdfDocument doc = ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), PdfHelper.GetPdfWriter
                ());
            NUnit.Framework.Assert.IsNotNull(doc);
            IList<PdfLayer> layers = doc.GetCatalog().GetOCProperties(true).GetLayers();
            NUnit.Framework.Assert.AreEqual(0, layers.Count);
            doc.Close();
            NUnit.Framework.Assert.AreEqual(engine, ocrPdfCreator.GetOcrEngine());
            NUnit.Framework.Assert.AreEqual(1, engine.GetOcrEngineProperties().GetLanguages().Count);
            NUnit.Framework.Assert.AreEqual("eng", engine.GetOcrEngineProperties().GetLanguages()[0]);
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfLayersWithCustomNames() {
            String path = PdfHelper.GetDefaultImagePath();
            FileInfo file = new FileInfo(path);
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetImageLayerName("name image 1");
            properties.SetTextLayerName("name text 1");
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(new CustomOcrEngine(), properties);
            PdfDocument doc = ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), PdfHelper.GetPdfWriter
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
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetImageLayerName("Image Layer");
            properties.SetTextLayerName("Text Layer");
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(new CustomOcrEngine(), properties);
            PdfDocument doc = ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), PdfHelper.GetPdfWriter
                (pdfPath));
            NUnit.Framework.Assert.IsNotNull(doc);
            IList<PdfLayer> layers = doc.GetCatalog().GetOCProperties(true).GetLayers();
            NUnit.Framework.Assert.AreEqual(2, layers.Count);
            NUnit.Framework.Assert.AreEqual("Image Layer", layers[0].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.IsTrue(layers[0].IsOn());
            NUnit.Framework.Assert.AreEqual("Text Layer", layers[1].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.IsTrue(layers[1].IsOn());
            doc.Close();
            NUnit.Framework.Assert.AreEqual(PdfHelper.DEFAULT_TEXT, PdfHelper.GetTextFromPdfLayer(pdfPath, "Text Layer"
                ));
            NUnit.Framework.Assert.AreEqual("", PdfHelper.GetTextFromPdfLayer(pdfPath, "Image Layer"));
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfLayersWithImageLayerOnly() {
            String path = PdfHelper.GetDefaultImagePath();
            FileInfo file = new FileInfo(path);
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetImageLayerName("Image Layer");
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(new CustomOcrEngine(), properties);
            PdfDocument doc = ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), PdfHelper.GetPdfWriter
                ());
            NUnit.Framework.Assert.IsNotNull(doc);
            IList<PdfLayer> layers = doc.GetCatalog().GetOCProperties(true).GetLayers();
            NUnit.Framework.Assert.AreEqual(1, layers.Count);
            NUnit.Framework.Assert.AreEqual("Image Layer", layers[0].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.IsTrue(layers[0].IsOn());
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfLayersWithTextLayerOnly() {
            String path = PdfHelper.GetDefaultImagePath();
            FileInfo file = new FileInfo(path);
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetTextLayerName("Text Layer");
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(new CustomOcrEngine(), properties);
            PdfDocument doc = ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), PdfHelper.GetPdfWriter
                ());
            NUnit.Framework.Assert.IsNotNull(doc);
            IList<PdfLayer> layers = doc.GetCatalog().GetOCProperties(true).GetLayers();
            NUnit.Framework.Assert.AreEqual(1, layers.Count);
            NUnit.Framework.Assert.AreEqual("Text Layer", layers[0].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.IsTrue(layers[0].IsOn());
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfLayersWithTextAndImageLayerWithTheSameName() {
            String path = PdfHelper.GetDefaultImagePath();
            FileInfo file = new FileInfo(path);
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetTextLayerName("Mixed Layer");
            properties.SetImageLayerName("Mixed Layer");
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(new CustomOcrEngine(), properties);
            PdfDocument doc = ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), PdfHelper.GetPdfWriter
                ());
            NUnit.Framework.Assert.IsNotNull(doc);
            IList<PdfLayer> layers = doc.GetCatalog().GetOCProperties(true).GetLayers();
            NUnit.Framework.Assert.AreEqual(1, layers.Count);
            NUnit.Framework.Assert.AreEqual("Mixed Layer", layers[0].GetPdfObject().Get(PdfName.Name).ToString());
            NUnit.Framework.Assert.IsTrue(layers[0].IsOn());
            doc.Close();
        }
    }
}
