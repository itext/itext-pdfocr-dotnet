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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout.Font;
using iText.Pdfocr.Helpers;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfocr {
    public class PdfFontTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestFontColor() {
            String testName = "testFontColor";
            String path = PdfHelper.GetImagesTestDirectory() + "multipage.tiff";
            String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetScaleMode(ScaleMode.SCALE_TO_FIT);
            properties.SetTextLayerName("Text1");
            Color color = DeviceCmyk.CYAN;
            properties.SetTextColor(color);
            PdfHelper.CreatePdf(pdfPath, file, properties);
            ExtractionStrategy strategy = PdfHelper.GetExtractionStrategy(pdfPath, "Text1");
            Color fillColor = strategy.GetFillColor();
            NUnit.Framework.Assert.AreEqual(color, fillColor);
        }

        [LogMessage(PdfOcrLogMessageConstant.PROVIDED_FONT_PROVIDER_IS_INVALID, Count = 1)]
        [LogMessage(OcrException.CANNOT_CREATE_PDF_DOCUMENT, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestInvalidFontWithInvalidDefaultFontFamily() {
            NUnit.Framework.Assert.That(() =>  {
                String testName = "testInvalidFontWithInvalidDefaultFontFamily";
                String path = PdfHelper.GetDefaultImagePath();
                String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
                FileInfo file = new FileInfo(path);
                OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
                FontProvider pdfOcrFontProvider = new FontProvider("Font");
                pdfOcrFontProvider.GetFontSet().AddFont("font.ttf", PdfEncodings.IDENTITY_H, "Font");
                properties.SetFontProvider(pdfOcrFontProvider, "Font");
                properties.SetScaleMode(ScaleMode.SCALE_TO_FIT);
                PdfHelper.CreatePdf(pdfPath, file, properties);
                String result = PdfHelper.GetTextFromPdfLayer(pdfPath, null);
                NUnit.Framework.Assert.AreEqual(PdfHelper.DEFAULT_TEXT, result);
                NUnit.Framework.Assert.AreEqual(ScaleMode.SCALE_TO_FIT, properties.GetScaleMode());
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(OcrException.CANNOT_CREATE_PDF_DOCUMENT, OcrException.CANNOT_RESOLVE_PROVIDED_FONTS)))
;
        }

        [NUnit.Framework.Test]
        public virtual void TestDefaultFontInPdfARgb() {
            String testName = "testDefaultFontInPdf";
            String path = PdfHelper.GetDefaultImagePath();
            String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
            ocrPdfCreatorProperties.SetPdfLang("en-US");
            ocrPdfCreatorProperties.SetTextColor(DeviceRgb.BLACK);
            PdfHelper.CreatePdfA(pdfPath, file, ocrPdfCreatorProperties, PdfHelper.GetRGBPdfOutputIntent());
            ExtractionStrategy strategy = PdfHelper.GetExtractionStrategy(pdfPath);
            PdfFont font = strategy.GetPdfFont();
            String fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("LiberationSans"));
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void TestInvalidCustomFontInPdfACMYK() {
            String testName = "testInvalidCustomFontInPdf";
            String path = PdfHelper.GetDefaultImagePath();
            String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
            ocrPdfCreatorProperties.SetPdfLang("en-US");
            ocrPdfCreatorProperties.SetFontProvider(new PdfOcrFontProvider());
            PdfHelper.CreatePdfA(pdfPath, file, ocrPdfCreatorProperties, PdfHelper.GetCMYKPdfOutputIntent());
            ExtractionStrategy strategy = PdfHelper.GetExtractionStrategy(pdfPath);
            PdfFont font = strategy.GetPdfFont();
            String fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("LiberationSans"));
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void TestCustomFontInPdf() {
            String testName = "testDefaultFontInPdf";
            String path = PdfHelper.GetDefaultImagePath();
            String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            FontProvider fontProvider = new FontProvider("FreeSans");
            fontProvider.GetFontSet().AddFont(PdfHelper.GetFreeSansFontPath(), PdfEncodings.IDENTITY_H, "FreeSans");
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
            ocrPdfCreatorProperties.SetPdfLang("en-US");
            ocrPdfCreatorProperties.SetFontProvider(fontProvider, "FreeSans");
            PdfHelper.CreatePdfA(pdfPath, file, ocrPdfCreatorProperties, PdfHelper.GetCMYKPdfOutputIntent());
            ExtractionStrategy strategy = PdfHelper.GetExtractionStrategy(pdfPath);
            PdfFont font = strategy.GetPdfFont();
            String fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("FreeSans"));
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
        }

        [LogMessage(PdfOcrLogMessageConstant.COULD_NOT_FIND_CORRESPONDING_GLYPH_TO_UNICODE_CHARACTER, Count = 7)]
        [NUnit.Framework.Test]
        public virtual void TestThaiImageWithNotDefGlyphs() {
            String testName = "testThaiImageWithNotDefGlyphs";
            String path = PdfHelper.GetThaiImagePath();
            String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
            PdfHelper.CreatePdf(pdfPath, new FileInfo(path), new OcrPdfCreatorProperties().SetTextColor(DeviceRgb.BLACK
                ));
            String resultWithActualText = PdfHelper.GetTextFromPdfLayerUseActualText(pdfPath, null);
            NUnit.Framework.Assert.AreEqual(PdfHelper.THAI_TEXT.Replace(" ", ""), resultWithActualText.Replace(" ", ""
                ));
            String resultWithoutUseActualText = PdfHelper.GetTextFromPdfLayer(pdfPath, null);
            NUnit.Framework.Assert.AreNotEqual(PdfHelper.THAI_TEXT, resultWithoutUseActualText);
            NUnit.Framework.Assert.AreNotEqual(resultWithoutUseActualText, resultWithActualText);
        }

        [NUnit.Framework.Test]
        public virtual void TestReusingFontProvider() {
            String testName = "testReusingFontProvider";
            String path = PdfHelper.GetDefaultImagePath();
            String pdfPathA3u = PdfHelper.GetTargetDirectory() + testName + "_a3u.pdf";
            String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            FontProvider fontProvider = new FontProvider("FreeSans");
            fontProvider.AddFont(PdfHelper.GetFreeSansFontPath());
            PdfOcrFontProvider pdfOcrFontProvider = new PdfOcrFontProvider(fontProvider.GetFontSet(), "FreeSans");
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
            ocrPdfCreatorProperties.SetPdfLang("en-US");
            ocrPdfCreatorProperties.SetFontProvider(pdfOcrFontProvider);
            PdfHelper.CreatePdfA(pdfPathA3u, file, ocrPdfCreatorProperties, PdfHelper.GetCMYKPdfOutputIntent());
            PdfHelper.CreatePdf(pdfPath, file, ocrPdfCreatorProperties);
            ExtractionStrategy strategy = PdfHelper.GetExtractionStrategy(pdfPathA3u);
            PdfFont font = strategy.GetPdfFont();
            String fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("FreeSans"));
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
            NUnit.Framework.Assert.AreEqual(PdfHelper.DEFAULT_TEXT, strategy.GetResultantText());
            strategy = PdfHelper.GetExtractionStrategy(pdfPath);
            font = strategy.GetPdfFont();
            fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("FreeSans"));
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
            NUnit.Framework.Assert.AreEqual(PdfHelper.DEFAULT_TEXT, strategy.GetResultantText());
        }
    }
}
