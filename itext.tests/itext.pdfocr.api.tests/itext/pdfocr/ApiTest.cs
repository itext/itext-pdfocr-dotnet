/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Actions.Contexts;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfa;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Helpers;
using iText.Pdfocr.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfocr {
    [NUnit.Framework.Category("IntegrationTest")]
    public class ApiTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfocr";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfWithFileTest() {
            OcrPdfCreatorProperties props = new OcrPdfCreatorProperties().SetMetaInfo(new ApiTest.DummyMetaInfo());
            OcrPdfCreator pdfCreator = new OcrPdfCreator(new CustomOcrEngine(), props);
            using (PdfDocument pdf = pdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(PdfHelper
                .GetDefaultImagePath())), PdfHelper.GetPdfWriter(), new DocumentProperties().SetEventCountingMetaInfo(
                new ApiTest.DummyMetaInfo()))) {
                String contentBytes = iText.Commons.Utils.JavaUtil.GetStringForBytes(pdf.GetPage(1).GetContentBytes(), System.Text.Encoding
                    .UTF8);
                NUnit.Framework.Assert.IsTrue(contentBytes.Contains("<00190014001c001400150014>"));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfFileWithFileTest() {
            String output = DESTINATION_FOLDER + "createPdfFileWithFileTest.pdf";
            OcrPdfCreatorProperties props = new OcrPdfCreatorProperties().SetMetaInfo(new ApiTest.DummyMetaInfo());
            OcrPdfCreator pdfCreator = new OcrPdfCreator(new CustomOcrEngine(), props);
            pdfCreator.CreatePdfFile(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(PdfHelper.GetDefaultImagePath
                ())), new FileInfo(output));
            using (PdfDocument pdf = new PdfDocument(new PdfReader(output))) {
                String contentBytes = iText.Commons.Utils.JavaUtil.GetStringForBytes(pdf.GetPage(1).GetContentBytes(), System.Text.Encoding
                    .UTF8);
                NUnit.Framework.Assert.IsTrue(contentBytes.Contains("<00190014001c001400150014>"));
            }
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfAWithFileTest() {
            OcrPdfCreatorProperties props = new OcrPdfCreatorProperties().SetMetaInfo(new ApiTest.DummyMetaInfo()).SetPdfLang
                ("en-US");
            OcrPdfCreator pdfCreator = new OcrPdfCreator(new CustomOcrEngine(), props);
            using (PdfDocument pdf = pdfCreator.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(PdfHelper
                .GetDefaultImagePath())), PdfHelper.GetPdfWriter(), new DocumentProperties().SetEventCountingMetaInfo(
                new ApiTest.DummyMetaInfo()), PdfHelper.GetRGBPdfOutputIntent())) {
                String contentBytes = iText.Commons.Utils.JavaUtil.GetStringForBytes(pdf.GetPage(1).GetContentBytes(), System.Text.Encoding
                    .UTF8);
                NUnit.Framework.Assert.IsTrue(contentBytes.Contains("<00190014001c001400150014>"));
                NUnit.Framework.Assert.IsTrue(pdf is PdfADocument);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfAFileWithFileTest() {
            String output = DESTINATION_FOLDER + "createPdfAFileWithFileTest.pdf";
            OcrPdfCreatorProperties props = new OcrPdfCreatorProperties().SetMetaInfo(new ApiTest.DummyMetaInfo()).SetPdfLang
                ("en-US");
            OcrPdfCreator pdfCreator = new OcrPdfCreator(new CustomOcrEngine(), props);
            pdfCreator.CreatePdfAFile(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(PdfHelper.GetDefaultImagePath
                ())), new FileInfo(output), PdfHelper.GetRGBPdfOutputIntent());
            using (PdfDocument pdf = new PdfDocument(new PdfReader(output))) {
                String contentBytes = iText.Commons.Utils.JavaUtil.GetStringForBytes(pdf.GetPage(1).GetContentBytes(), System.Text.Encoding
                    .UTF8);
                NUnit.Framework.Assert.IsTrue(contentBytes.Contains("<00190014001c001400150014>"));
                PdfAConformance cl = pdf.GetReader().GetPdfConformance().GetAConformance();
                NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_3U.GetLevel(), cl.GetLevel());
                NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_3U.GetPart(), cl.GetPart());
            }
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfAFileWithFileNoMetaTest() {
            String output = DESTINATION_FOLDER + "createPdfAFileWithFileNoMetaTest.pdf";
            OcrPdfCreatorProperties props = new OcrPdfCreatorProperties().SetPdfLang("en-US");
            OcrPdfCreator pdfCreator = new OcrPdfCreator(new CustomOcrEngine(), props);
            pdfCreator.CreatePdfAFile(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(PdfHelper.GetDefaultImagePath
                ())), new FileInfo(output), PdfHelper.GetRGBPdfOutputIntent());
            using (PdfDocument pdf = new PdfDocument(new PdfReader(output))) {
                String contentBytes = iText.Commons.Utils.JavaUtil.GetStringForBytes(pdf.GetPage(1).GetContentBytes(), System.Text.Encoding
                    .UTF8);
                NUnit.Framework.Assert.IsTrue(contentBytes.Contains("<00190014001c001400150014>"));
                PdfAConformance cl = pdf.GetReader().GetPdfConformance().GetAConformance();
                NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_3U.GetLevel(), cl.GetLevel());
                NUnit.Framework.Assert.AreEqual(PdfAConformance.PDF_A_3U.GetPart(), cl.GetPart());
            }
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfAFileWithFileProductAwareEngineTest() {
            String output = DESTINATION_FOLDER + "createPdfAFileWithFileProductAwareEngineTest.pdf";
            OcrPdfCreatorProperties props = new OcrPdfCreatorProperties().SetPdfLang("en-US");
            CustomProductAwareOcrEngine ocrEngine = new CustomProductAwareOcrEngine();
            OcrPdfCreator pdfCreator = new OcrPdfCreator(ocrEngine, props);
            pdfCreator.CreatePdfAFile(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(PdfHelper.GetDefaultImagePath
                ())), new FileInfo(output), PdfHelper.GetRGBPdfOutputIntent());
            NUnit.Framework.Assert.IsTrue(ocrEngine.IsGetMetaInfoContainerTriggered());
        }

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
            Exception exception = NUnit.Framework.Assert.Catch(typeof(Exception), () => {
                OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
                properties.SetImageRotationHandler(new ApiTest.NotImplementedImageRotationHandler());
                String testName = "testSetAndGetImageRotationHandler";
                String path = PdfHelper.GetImagesTestDirectory() + "90_degrees_rotated.jpg";
                String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
                PdfHelper.CreatePdf(pdfPath, new FileInfo(path), properties);
                NUnit.Framework.Assert.IsNotNull(properties.GetImageRotationHandler());
            }
            );
            NUnit.Framework.Assert.AreEqual("applyRotation is not implemented", exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestImageRotationHandlerForTiff() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(Exception), () => {
                OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
                properties.SetImageRotationHandler(new ApiTest.NotImplementedImageRotationHandler());
                String testName = "testSetAndGetImageRotationHandler";
                String path = PdfHelper.GetImagesTestDirectory() + "multipage.tiff";
                String pdfPath = PdfHelper.GetTargetDirectory() + testName + ".pdf";
                PdfHelper.CreatePdf(pdfPath, new FileInfo(path), properties);
                NUnit.Framework.Assert.IsNotNull(properties.GetImageRotationHandler());
            }
            );
            NUnit.Framework.Assert.AreEqual("applyRotation is not implemented", exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TestTableStructureTree() {
            String pdfPath = PdfHelper.GetTargetDirectory() + "tableStructureTree.pdf";
            // Image doesn't really matter here
            String input = PdfHelper.GetImagesTestDirectory() + "numbers_01.jpg";
            IOcrEngine ocrEngine = new TestStructureDetectionOcrEngine();
            OcrPdfCreatorProperties creatorProperties = new OcrPdfCreatorProperties();
            creatorProperties.SetTextColor(DeviceRgb.RED);
            creatorProperties.SetTagged(true);
            OcrPdfCreator pdfCreator = new OcrPdfCreator(ocrEngine, creatorProperties);
            TestProcessProperties processProperties = new TestProcessProperties(5, 6, 50, 15, 100, 200);
            using (PdfWriter pdfWriter = PdfHelper.GetPdfWriter(pdfPath)) {
                pdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(input)), pdfWriter, new DocumentProperties
                    (), processProperties).Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(pdfPath, PdfHelper.TEST_DIRECTORY + "cmp_tableStructureTree.pdf"
                , PdfHelper.GetTargetDirectory(), "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestTaggingNotSupported() {
            String input = PdfHelper.GetImagesTestDirectory() + "numbers_01.jpg";
            String pdfPath = PdfHelper.GetTargetDirectory() + "taggingNotSupported.pdf";
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => PdfHelper.CreatePdf(pdfPath, new 
                FileInfo(input), new OcrPdfCreatorProperties().SetTagged(true)));
            NUnit.Framework.Assert.AreEqual(PdfOcrExceptionMessageConstant.TAGGING_IS_NOT_SUPPORTED, e.Message);
        }

//\cond DO_NOT_DOCUMENT
        internal class NotImplementedImageRotationHandler : IImageRotationHandler {
            public virtual ImageData ApplyRotation(ImageData imageData) {
                throw new Exception("applyRotation is not implemented");
            }
        }
//\endcond

        private class DummyMetaInfo : IMetaInfo {
        }
    }
}
