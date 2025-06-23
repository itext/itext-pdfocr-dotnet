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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Recognition;
using iText.Test;

namespace iText.Pdfocr.Onnxtr {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxTRPdfAIntegrationTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/OnnxTRPdfAIntegrationTest/";

        private static readonly String TEST_IMAGE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxTRPdfAIntegrationTest/";

        private static readonly String COLOR_PROFILE_PATH = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/profiles/";

        private static readonly String FAST = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/rep_fast_tiny-28867779.onnx";

        private static readonly String CRNNVGG16 = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/crnn_vgg16_bn-662979cc.onnx";

        private static OnnxTrOcrEngine OCR_ENGINE;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
            IDetectionPredictor detectionPredictor = OnnxDetectionPredictor.Fast(FAST);
            IRecognitionPredictor recognitionPredictor = OnnxRecognitionPredictor.CrnnVgg16(CRNNVGG16);
            OCR_ENGINE = new OnnxTrOcrEngine(detectionPredictor, recognitionPredictor);
        }

        [NUnit.Framework.Test]
        public virtual void RgbPdfATest() {
            String src = TEST_IMAGE_DIRECTORY + "englishText.bmp";
            String dest = TARGET_DIRECTORY + "rgbpdfA.pdf";
            String cmp = TEST_DIRECTORY + "cmp_rgbpdfA.pdf";
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
            ocrPdfCreatorProperties.SetPdfLang("en-US");
            ocrPdfCreatorProperties.SetTitle("");
            ocrPdfCreatorProperties.SetTextLayerName("Text1");
            ocrPdfCreatorProperties.SetTextColor(DeviceRgb.BLUE);
            Stream @is = FileUtil.GetInputStreamForFile(COLOR_PROFILE_PATH + "sRGB_CS_profile.icm");
            PdfOutputIntent outputIntent = new PdfOutputIntent("", "", "", "sRGB IEC61966-2.1", @is);
            DoOcrAndCreatePdf(src, dest, ocrPdfCreatorProperties, outputIntent);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceRgb.BLUE, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("This test\n1S\na\nfor\nmessage\n.\nOCR\nScanner\nTest\nBMPTest", extractionStrategy
                    .GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void CmykPdfATest() {
            String src = TEST_IMAGE_DIRECTORY + "numbers_01.jpg";
            String dest = TARGET_DIRECTORY + "cmykpdfA.pdf";
            String cmp = TEST_DIRECTORY + "cmp_cmykpdfA.pdf";
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
            ocrPdfCreatorProperties.SetPdfLang("en-US");
            ocrPdfCreatorProperties.SetTitle("");
            ocrPdfCreatorProperties.SetTextLayerName("Text1");
            ocrPdfCreatorProperties.SetTextColor(DeviceCmyk.MAGENTA);
            Stream @is = FileUtil.GetInputStreamForFile(COLOR_PROFILE_PATH + "CoatedFOGRA27.icc");
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "Coated FOGRA27 (ISO 12647 - 2:2004)"
                , @is);
            DoOcrAndCreatePdf(src, dest, ocrPdfCreatorProperties, outputIntent);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("619121", extractionStrategy.GetResultantText());
            }
        }

        private void DoOcrAndCreatePdf(String imagePath, String destPdfPath, OcrPdfCreatorProperties ocrPdfCreatorProperties
            , PdfOutputIntent pdfOutputIntent) {
            OcrPdfCreator ocrPdfCreator = ocrPdfCreatorProperties != null ? new OcrPdfCreator(OCR_ENGINE, ocrPdfCreatorProperties
                ) : new OcrPdfCreator(OCR_ENGINE);
            using (PdfWriter writer = new PdfWriter(destPdfPath)) {
                ocrPdfCreator.CreatePdfA(JavaCollectionsUtil.SingletonList(new FileInfo(imagePath)), writer, pdfOutputIntent
                    ).Close();
            }
        }
    }
}
