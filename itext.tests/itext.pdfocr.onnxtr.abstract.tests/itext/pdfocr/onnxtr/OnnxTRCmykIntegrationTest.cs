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
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Pdfocr;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Pdfocr.Onnxtr.Orientation;
using iText.Pdfocr.Onnxtr.Recognition;
using iText.Pdfocr.Onnxtr.Util;
using iText.Test;

namespace iText.Pdfocr.Onnxtr {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxTRCmykIntegrationTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/OnnxTRCmykIntegrationTest/";

        private static readonly String TEST_IMAGE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxTRCmykIntegrationTest/";

        private static readonly String FAST = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/rep_fast_tiny-28867779.onnx";

        private static readonly String CRNNVGG16 = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/crnn_vgg16_bn-662979cc.onnx";

        private static readonly String MOBILENETV3 = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/mobilenet_v3_small_crop_orientation-5620cf7e.onnx";

        private static OnnxTrOcrEngine OCR_ENGINE;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
            IDetectionPredictor detectionPredictor = OnnxDetectionPredictor.Fast(FAST);
            IRecognitionPredictor recognitionPredictor = OnnxRecognitionPredictor.CrnnVgg16(CRNNVGG16);
            IOrientationPredictor orientationPredictor = OnnxOrientationPredictor.MobileNetV3(MOBILENETV3);
            OCR_ENGINE = new OnnxTrOcrEngine(detectionPredictor, orientationPredictor, recognitionPredictor);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            OCR_ENGINE.Close();
        }

        [NUnit.Framework.Test]
        public virtual void RainbowInvertedCmykTest() {
            String src = TEST_IMAGE_DIRECTORY + "rainbow_inverted_cmyk.jpg";
            String dest = TARGET_DIRECTORY + "rainbowInvertedCmykTest.pdf";
            String cmpTxt = TEST_DIRECTORY + "cmp_rainbowInvertedCmykTest.txt";
            try {
                DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
                using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                    ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                    NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                    NUnit.Framework.Assert.AreEqual(GetCmpText(cmpTxt), extractionStrategy.GetResultantText());
                }
            }
            catch (PdfOcrInputException e) {
                // CMYK bug https://bugs.openjdk.org/browse/JDK-8274735 in openJDK:
                // fixed for jdk8 from 351 onwards, for jdk11 from 16 onwards and for jdk17 starting from 4.
                // Amazon corretto jdk started support CMYK for JPEG from 11 version.
                // Temurin 8 does not support CMYK for JPEG either.
                NUnit.Framework.Assert.AreEqual(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_READ_IMAGE, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void RainbowAdobeCmykTest() {
            String src = TEST_IMAGE_DIRECTORY + "rainbow_adobe_cmyk.jpg";
            String dest = TARGET_DIRECTORY + "rainbowAdobeCmykTest.pdf";
            String cmpTxt = TEST_DIRECTORY + "cmp_rainbowAdobeCmykTest.txt";
            try {
                DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
                using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                    ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                    NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                    double relativeDistance = (double)MathUtil.CalculateLevenshteinDistance(GetCmpText(cmpTxt), extractionStrategy
                        .GetResultantText()) / GetCmpText(cmpTxt).Length;
                    NUnit.Framework.Assert.IsTrue(relativeDistance < 0.05);
                }
            }
            catch (PdfOcrInputException e) {
                // CMYK bug https://bugs.openjdk.org/browse/JDK-8274735 in openJDK:
                // fixed for jdk8 from 351 onwards, for jdk11 from 16 onwards and for jdk17 starting from 4.
                // Amazon corretto jdk started support CMYK for JPEG from 11 version.
                // Temurin 8 does not support CMYK for JPEG either.
                NUnit.Framework.Assert.AreEqual(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_READ_IMAGE, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void RainbowCmykNoProfileTest() {
            String src = TEST_IMAGE_DIRECTORY + "rainbow_cmyk_inverted_no_profile.jpg";
            String dest = TARGET_DIRECTORY + "rainbowCmykNoProfileTest.pdf";
            String cmpTxt = TEST_DIRECTORY + "cmp_rainbowCmykNoProfileTest.txt";
            try {
                DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
                using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                    ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                    NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                    NUnit.Framework.Assert.AreEqual(GetCmpText(cmpTxt), extractionStrategy.GetResultantText());
                }
            }
            catch (PdfOcrInputException e) {
                // CMYK bug https://bugs.openjdk.org/browse/JDK-8274735 in openJDK:
                // fixed for jdk8 from 351 onwards, for jdk11 from 16 onwards and for jdk17 starting from 4.
                // Amazon corretto jdk started support CMYK for JPEG from 11 version.
                // Temurin 8 does not support CMYK for JPEG either.
                NUnit.Framework.Assert.AreEqual(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_READ_IMAGE, e.Message);
            }
        }

        private OcrPdfCreatorProperties CreatorProperties(String layerName, Color color) {
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
            ocrPdfCreatorProperties.SetTextLayerName(layerName);
            ocrPdfCreatorProperties.SetTextColor(color);
            return ocrPdfCreatorProperties;
        }

        private void DoOcrAndCreatePdf(String imagePath, String destPdfPath, OcrPdfCreatorProperties ocrPdfCreatorProperties
            ) {
            OcrPdfCreator ocrPdfCreator = ocrPdfCreatorProperties != null ? new OcrPdfCreator(OCR_ENGINE, ocrPdfCreatorProperties
                ) : new OcrPdfCreator(OCR_ENGINE);
            using (PdfWriter writer = new PdfWriter(destPdfPath)) {
                ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList(new FileInfo(imagePath)), writer).Close();
            }
        }

        private String GetCmpText(String txtPath) {
            int bytesCount = (int)new FileInfo(txtPath).Length;
            char[] array = new char[bytesCount];
            using (StreamReader stream = new StreamReader(iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine
                (txtPath)))) {
                stream.Read(array, 0, bytesCount);
                return new String(array);
            }
        }
    }
}
