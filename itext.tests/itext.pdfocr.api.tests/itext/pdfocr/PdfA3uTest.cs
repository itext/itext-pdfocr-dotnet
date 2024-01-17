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
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Exceptions;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Font;
using iText.Pdfa.Exceptions;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Helpers;
using iText.Pdfocr.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfocr {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PdfA3uTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void TestPdfA3uWithNullIntent() {
            String testName = "testPdfA3uWithNullIntent";
            String path = PdfHelper.GetDefaultImagePath();
            String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetTextColor(DeviceCmyk.BLACK);
            properties.SetScaleMode(ScaleMode.SCALE_TO_FIT);
            PdfHelper.CreatePdfA(pdfPath, new FileInfo(path), properties, null);
            String result = PdfHelper.GetTextFromPdfLayer(pdfPath, null);
            NUnit.Framework.Assert.AreEqual(PdfHelper.DEFAULT_TEXT, result);
            NUnit.Framework.Assert.AreEqual(ScaleMode.SCALE_TO_FIT, properties.GetScaleMode());
        }

        [NUnit.Framework.Test]
        public virtual void TestIncompatibleOutputIntentAndFontColorSpaceException() {
            NUnit.Framework.Assert.That(() =>  {
                String testName = "testIncompatibleOutputIntentAndFontColorSpaceException";
                String path = PdfHelper.GetDefaultImagePath();
                String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
                OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
                ocrPdfCreatorProperties.SetPdfLang("en-US");
                ocrPdfCreatorProperties.SetTextColor(DeviceCmyk.BLACK);
                PdfHelper.CreatePdfA(pdfPath, new FileInfo(path), ocrPdfCreatorProperties, PdfHelper.GetRGBPdfOutputIntent
                    ());
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfAConformanceException.DEVICECMYK_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT))
;
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfA3DefaultMetadata() {
            String testName = "testPdfDefaultMetadata";
            String path = PdfHelper.GetDefaultImagePath();
            String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
            ocrPdfCreatorProperties.SetPdfLang("en-US");
            ocrPdfCreatorProperties.SetTextColor(DeviceRgb.BLACK);
            PdfHelper.CreatePdfA(pdfPath, file, ocrPdfCreatorProperties, PdfHelper.GetRGBPdfOutputIntent());
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            NUnit.Framework.Assert.AreEqual("en-US", pdfDocument.GetCatalog().GetLang().ToString());
            NUnit.Framework.Assert.AreEqual(null, pdfDocument.GetDocumentInfo().GetTitle());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_3U, pdfDocument.GetReader().GetPdfAConformanceLevel
                ());
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfCustomMetadata() {
            String testName = "testPdfCustomMetadata";
            String path = PdfHelper.GetDefaultImagePath();
            String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            String locale = "nl-BE";
            properties.SetPdfLang(locale);
            String title = "Title";
            properties.SetTitle(title);
            PdfHelper.CreatePdfA(pdfPath, file, new OcrPdfCreatorProperties(properties), PdfHelper.GetCMYKPdfOutputIntent
                ());
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            NUnit.Framework.Assert.AreEqual(locale, pdfDocument.GetCatalog().GetLang().ToString());
            NUnit.Framework.Assert.AreEqual(title, pdfDocument.GetDocumentInfo().GetTitle());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_3U, pdfDocument.GetReader().GetPdfAConformanceLevel
                ());
            pdfDocument.Close();
        }

        [LogMessage(PdfOcrExceptionMessageConstant.CANNOT_CREATE_PDF_DOCUMENT, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestNonCompliantThaiPdfA() {
            NUnit.Framework.Assert.That(() =>  {
                String testName = "testNonCompliantThaiPdfA";
                String path = PdfHelper.GetThaiImagePath();
                String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
                OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
                ocrPdfCreatorProperties.SetPdfLang("en-US");
                ocrPdfCreatorProperties.SetTextColor(DeviceRgb.BLACK);
                PdfHelper.CreatePdfA(pdfPath, new FileInfo(path), ocrPdfCreatorProperties, PdfHelper.GetRGBPdfOutputIntent
                    ());
            }
            , NUnit.Framework.Throws.InstanceOf<PdfOcrException>().With.Message.EqualTo(MessageFormatUtil.Format(PdfOcrExceptionMessageConstant.CANNOT_CREATE_PDF_DOCUMENT, MessageFormatUtil.Format(PdfOcrLogMessageConstant.COULD_NOT_FIND_CORRESPONDING_GLYPH_TO_UNICODE_CHARACTER, 3611))))
;
        }

        [NUnit.Framework.Test]
        public virtual void TestCompliantThaiPdfA() {
            String testName = "testCompliantThaiPdfA";
            String path = PdfHelper.GetThaiImagePath();
            String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
            ocrPdfCreatorProperties.SetPdfLang("en-US");
            ocrPdfCreatorProperties.SetTextColor(DeviceRgb.BLACK);
            FontProvider fontProvider = new FontProvider("Kanit");
            fontProvider.AddFont(PdfHelper.GetKanitFontPath());
            PdfOcrFontProvider pdfOcrFontProvider = new PdfOcrFontProvider(fontProvider.GetFontSet(), "Kanit");
            ocrPdfCreatorProperties.SetFontProvider(pdfOcrFontProvider);
            PdfHelper.CreatePdfA(pdfPath, new FileInfo(path), ocrPdfCreatorProperties, PdfHelper.GetRGBPdfOutputIntent
                ());
            String resultWithActualText = PdfHelper.GetTextFromPdfLayerUseActualText(pdfPath, null);
            NUnit.Framework.Assert.AreEqual(PdfHelper.THAI_TEXT, resultWithActualText);
            String resultWithoutUseActualText = PdfHelper.GetTextFromPdfLayer(pdfPath, null);
            NUnit.Framework.Assert.AreEqual(PdfHelper.THAI_TEXT, resultWithoutUseActualText);
            NUnit.Framework.Assert.AreEqual(resultWithoutUseActualText, resultWithActualText);
            ExtractionStrategy strategy = PdfHelper.GetExtractionStrategy(pdfPath);
            PdfFont font = strategy.GetPdfFont();
            String fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("Kanit"));
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
        }

        [LogMessage(PdfOcrExceptionMessageConstant.CANNOT_CREATE_PDF_DOCUMENT, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestPdfACreateWithoutPdfLangProperty() {
            NUnit.Framework.Assert.That(() =>  {
                String testName = "testPdfACreateWithoutPdfLangProperty";
                String path = PdfHelper.GetThaiImagePath();
                String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
                PdfHelper.CreatePdfA(pdfPath, new FileInfo(path), new OcrPdfCreatorProperties(), PdfHelper.GetRGBPdfOutputIntent
                    ());
            }
            , NUnit.Framework.Throws.InstanceOf<PdfOcrException>().With.Message.EqualTo(MessageFormatUtil.Format(PdfOcrExceptionMessageConstant.CANNOT_CREATE_PDF_DOCUMENT, PdfOcrLogMessageConstant.PDF_LANGUAGE_PROPERTY_IS_NOT_SET)))
;
        }
    }
}
