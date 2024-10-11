/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

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
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Layer;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4;

namespace iText.Pdfocr.Pdflayers {
    public abstract class PdfLayersIntegrationTest : IntegrationTestHelper {
//\cond DO_NOT_DOCUMENT
        internal AbstractTesseract4OcrEngine tesseractReader;
//\endcond

        public PdfLayersIntegrationTest(IntegrationTestHelper.ReaderType type) {
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPdfLayersFromMultiPageTiff() {
            String testName = "testTextFromPdfLayersFromMultiPageTiff";
            bool preprocess = tesseractReader.GetTesseract4OcrEngineProperties().IsPreprocessingImages();
            String path = TEST_IMAGES_DIRECTORY + "multîpage.tiff";
            String pdfPath = GetTargetDirectory() + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages
                (false));
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetTextLayerName("Text Layer");
            properties.SetImageLayerName("Image Layer");
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader, properties);
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
        public virtual void TestTextFromMultiPageTiff() {
            String testName = "testTextFromMultiPageTiff";
            bool preprocess = tesseractReader.GetTesseract4OcrEngineProperties().IsPreprocessingImages();
            String path = TEST_IMAGES_DIRECTORY + "multîpage.tiff";
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
            NUnit.Framework.Assert.AreEqual(0, layers.Count);
            doc.Close();
            // Text layer should contain all text
            // Image layer shouldn't contain any text
            String expectedOutput = "Multipage\nTIFF\nExample\nPage 5";
            NUnit.Framework.Assert.AreEqual(expectedOutput, GetTextFromPdfLayer(pdfPath, null, 5));
            NUnit.Framework.Assert.IsFalse(tesseractReader.GetTesseract4OcrEngineProperties().IsPreprocessingImages());
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages
                (preprocess));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPdfLayersFromMultiPagePdf() {
            String testName = "testTextFromPdfLayersFromMultiPagePdf";
            String pdfPath = GetTargetDirectory() + testName + ".pdf";
            IList<FileInfo> files = JavaUtil.ArraysAsList(new FileInfo(TEST_IMAGES_DIRECTORY + "german_01.jpg"), new FileInfo
                (TEST_IMAGES_DIRECTORY + "tèst/noisy_01.png"), new FileInfo(TEST_IMAGES_DIRECTORY + "nümbérs.jpg"), new 
                FileInfo(TEST_IMAGES_DIRECTORY + "example_04.png"));
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
