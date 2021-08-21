/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.IO.Image;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Pdfocr.Helpers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfocr {
    public class ApiTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestTextInfo() {
            String path = PdfHelper.GetDefaultImagePath();
            IDictionary<int, IList<TextInfo>> result = new CustomOcrEngine().DoImageOcr(new FileInfo(path));
            NUnit.Framework.Assert.AreEqual(1, result.Count);
            TextInfo textInfo = new TextInfo();
            textInfo.SetText("text");
            textInfo.SetBboxRect(new Rectangle(204.0f, 158.0f, 538.0f, 136.0f));
            int page = 2;
            result.Put(page, JavaCollectionsUtil.SingletonList<TextInfo>(textInfo));
            NUnit.Framework.Assert.AreEqual(2, result.Count);
            NUnit.Framework.Assert.AreEqual(textInfo.GetText(), result.Get(page)[0].GetText());
        }

        [LogMessage(PdfOcrLogMessageConstant.COULD_NOT_FIND_CORRESPONDING_GLYPH_TO_UNICODE_CHARACTER, Count = 7)]
        [NUnit.Framework.Test]
        public virtual void TestThaiImageWithNotDefGlyphs() {
            String testName = "testThaiImageWithNotdefGlyphs";
            String path = PdfHelper.GetThaiImagePath();
            String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
            PdfHelper.CreatePdf(pdfPath, new FileInfo(path), new OcrPdfCreatorProperties().SetTextColor(DeviceRgb.BLACK
                ));
            ExtractionStrategy strategy = PdfHelper.GetExtractionStrategy(pdfPath);
            PdfFont font = strategy.GetPdfFont();
            String fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("LiberationSans"));
        }

        [NUnit.Framework.Test]
        public virtual void TestImageRotationHandler() {
            NUnit.Framework.Assert.That(() =>  {
                OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
                properties.SetImageRotationHandler(new ApiTest.NotImplementedImageRotationHandler());
                String testName = "testSetAndGetImageRotationHandler";
                String path = PdfHelper.GetImagesTestDirectory() + "90_degrees_rotated.jpg";
                String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
                PdfHelper.CreatePdf(pdfPath, new FileInfo(path), properties);
                NUnit.Framework.Assert.IsNotNull(properties.GetImageRotationHandler());
            }
            , NUnit.Framework.Throws.InstanceOf<Exception>().With.Message.EqualTo("applyRotation is not implemented"))
;
        }

        [NUnit.Framework.Test]
        public virtual void TestImageRotationHandlerForTiff() {
            NUnit.Framework.Assert.That(() =>  {
                OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
                properties.SetImageRotationHandler(new ApiTest.NotImplementedImageRotationHandler());
                String testName = "testSetAndGetImageRotationHandler";
                String path = PdfHelper.GetImagesTestDirectory() + "multipage.tiff";
                String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
                PdfHelper.CreatePdf(pdfPath, new FileInfo(path), properties);
                NUnit.Framework.Assert.IsNotNull(properties.GetImageRotationHandler());
            }
            , NUnit.Framework.Throws.InstanceOf<Exception>().With.Message.EqualTo("applyRotation is not implemented"))
;
        }

        internal class NotImplementedImageRotationHandler : IImageRotationHandler {
            public virtual ImageData ApplyRotation(ImageData imageData) {
                throw new Exception("applyRotation is not implemented");
            }
        }
    }
}
