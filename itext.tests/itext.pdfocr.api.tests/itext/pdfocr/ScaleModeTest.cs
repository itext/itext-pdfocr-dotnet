/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Pdfocr.Helpers;
using iText.Test;

namespace iText.Pdfocr {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ScaleModeTest : ExtendedITextTest {
        private const float DELTA = 1e-4f;

        [NUnit.Framework.Test]
        public virtual void TestScaleWidthMode() {
            String testName = "testScaleWidthMode";
            String path = PdfHelper.GetDefaultImagePath();
            String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            float pageWidthPt = 400f;
            float pageHeightPt = 400f;
            Rectangle pageSize = new Rectangle(pageWidthPt, pageHeightPt);
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetScaleMode(ScaleMode.SCALE_WIDTH);
            properties.SetPageSize(pageSize);
            PdfHelper.CreatePdf(pdfPath, file, properties);
            Rectangle rect = GetImageBBoxRectangleFromPdf(pdfPath);
            ImageData originalImageData = ImageDataFactory.Create(file.FullName);
            // page size should be equal to the result image size
            // result image height should be equal to the value that
            // was set as page height result image width should be scaled
            // proportionally according to the provided image height
            // and original image size
            NUnit.Framework.Assert.AreEqual(pageHeightPt, rect.GetHeight(), DELTA);
            NUnit.Framework.Assert.AreEqual(originalImageData.GetWidth() / originalImageData.GetHeight(), rect.GetWidth
                () / rect.GetHeight(), DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void TestScaleHeightMode() {
            String testName = "testScaleHeightMode";
            String path = PdfHelper.GetDefaultImagePath();
            String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            float pageWidthPt = 400f;
            float pageHeightPt = 400f;
            Rectangle pageSize = new Rectangle(pageWidthPt, pageHeightPt);
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetScaleMode(ScaleMode.SCALE_HEIGHT);
            properties.SetPageSize(pageSize);
            PdfHelper.CreatePdf(pdfPath, file, properties);
            Rectangle rect = GetImageBBoxRectangleFromPdf(pdfPath);
            ImageData originalImageData = ImageDataFactory.Create(file.FullName);
            NUnit.Framework.Assert.AreEqual(pageWidthPt, rect.GetWidth(), DELTA);
            NUnit.Framework.Assert.AreEqual(originalImageData.GetWidth() / originalImageData.GetHeight(), rect.GetWidth
                () / rect.GetHeight(), DELTA);
        }

        [NUnit.Framework.Test]
        public virtual void TestOriginalSizeScaleMode() {
            String path = PdfHelper.GetDefaultImagePath();
            FileInfo file = new FileInfo(path);
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(new CustomOcrEngine());
            PdfDocument doc = ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), PdfHelper.GetPdfWriter
                ());
            NUnit.Framework.Assert.IsNotNull(doc);
            ImageData imageData = ImageDataFactory.Create(file.FullName);
            float imageWidth = GetPoints(imageData.GetWidth());
            float imageHeight = GetPoints(imageData.GetHeight());
            float realWidth = doc.GetFirstPage().GetPageSize().GetWidth();
            float realHeight = doc.GetFirstPage().GetPageSize().GetHeight();
            NUnit.Framework.Assert.AreEqual(imageWidth, realWidth, DELTA);
            NUnit.Framework.Assert.AreEqual(imageHeight, realHeight, DELTA);
            doc.Close();
        }

        /// <summary>Converts value from pixels to points.</summary>
        /// <param name="pixels">input value in pixels</param>
        /// <returns>result value in points</returns>
        protected internal virtual float GetPoints(float pixels) {
            return pixels * 3f / 4f;
        }

        /// <summary>Retrieve image BBox rectangle from the first page from given PDF document.</summary>
        public static Rectangle GetImageBBoxRectangleFromPdf(String path) {
            ExtractionStrategy extractionStrategy = PdfHelper.GetExtractionStrategy(path);
            return extractionStrategy.GetImageBBoxRectangle();
        }
    }
}
