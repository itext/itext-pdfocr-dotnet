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
    public class OnnxTRIntegrationTest : ExtendedITextTest {
        private static readonly String FAST = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/rep_fast_tiny-28867779.onnx";

        private static readonly String CRNNVGG16 = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/crnn_vgg16_bn-662979cc.onnx";

        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/OnnxTRIntegrationTest/";

        private static readonly String TEST_IMAGE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxTRIntegrationTest/";

        private static OnnxTrOcrEngine OCR_ENGINE;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
            IDetectionPredictor detectionPredictor = OnnxDetectionPredictor.Fast(FAST);
            IRecognitionPredictor recognitionPredictor = OnnxRecognitionPredictor.CrnnVgg16(CRNNVGG16);
            OCR_ENGINE = new OnnxTrOcrEngine(detectionPredictor, recognitionPredictor);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            OCR_ENGINE.Close();
        }

        [NUnit.Framework.Test]
        public virtual void BasicTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_04.png";
            String dest = TARGET_DIRECTORY + "basicTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_basicTest.pdf";
            DoOcrAndCreatePdf(src, dest);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void BmpTest() {
            String src = TEST_IMAGE_DIRECTORY + "englishText.bmp";
            String dest = TARGET_DIRECTORY + "bmpTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_bmpTest.pdf";
            DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("This\n1S test\na\nfor\nmessage\n-\nOCR\nScanner\nTest\nBMPTest", extractionStrategy
                    .GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void JfifTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_02.JFIF";
            String dest = TARGET_DIRECTORY + "jfifTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_jfifTest.pdf";
            DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("Ihis\n1S test\na\nfor\nmessage\n-\nOCR\nScanner\nTest", extractionStrategy
                    .GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void Tiff10MBTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_03_10MB.tiff";
            String dest = TARGET_DIRECTORY + "tiff10MBTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_tiff10MBTest.pdf";
            DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("File\nFormat\nTagged\nImage", extractionStrategy.GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void JpeTest() {
            String src = TEST_IMAGE_DIRECTORY + "numbers_01.jpe";
            String dest = TARGET_DIRECTORY + "jpeTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_jpeTest.pdf";
            DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("619121", extractionStrategy.GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void NnnTest() {
            String src = TEST_IMAGE_DIRECTORY + "numbers_01.nnn";
            String dest = TARGET_DIRECTORY + "nnnTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_nnnTest.pdf";
            DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("619121", extractionStrategy.GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void GifTest() {
            String src = TEST_IMAGE_DIRECTORY + "weirdwords.gif";
            String dest = TARGET_DIRECTORY + "gifTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_gifTest.pdf";
            DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("he23llo\nqwetyrtyqpwe-rty", extractionStrategy.GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void MultipageTiffTest() {
            String src = TEST_IMAGE_DIRECTORY + "multipage.tiff";
            String dest = TARGET_DIRECTORY + "multipageTiffTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_multipageTiffTest.pdf";
            DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("Multipage\nTIFF\nExample\n1\nPage", extractionStrategy.GetResultantText()
                    );
                extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 7, "Text1");
                // Model glitch
                NUnit.Framework.Assert.AreEqual("Multipage\nTIFF\nExample\n/\nPage", extractionStrategy.GetResultantText()
                    );
                extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 9, "Text1");
                NUnit.Framework.Assert.AreEqual("Multipage\nTIFF\nExample\n9\nPage", extractionStrategy.GetResultantText()
                    );
            }
        }

        [NUnit.Framework.Test]
        public virtual void ScannedTest() {
            String src = TEST_IMAGE_DIRECTORY + "scanned_spa_01.png";
            String dest = TARGET_DIRECTORY + "scannedTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_scannedTest.pdf";
            DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("(\nSI\nAY ENSAYARA\nCOMO\nACTUAR?\nTanto\nlo\npeor,\nmejor es" +
                                                "\ndescansar\nno\ny\npensar\nla\nsi\nfiesta, se\npuede. No\nnada" +
                                                "\nhay\nmas\ndesalentador\nver las\nen\nfiestas\na\njovenes\ncon cara" +
                                                "\nde\nlastima\ny\nilusionadas\nhan\nse\ny que\npasado todo\nel dia" +
                                                "\ntratando\nhallar lo\nla\nmejor\nmas atractiva\ny\nmanera\nde\npres" +
                                                "\ntarse en\npublico.\nHay actuar\nque\ncon calma\nno\ny\ncansaremos" +
                                                "\nde\nrepetirlo, Lo\nmas\nimportante\nes saber\nque\nse va a\nponer" +
                                                " tener\ny todo\na\nmano,\nSi\nintenta\nprobar\nun\nnuevo\nlapiz" +
                                                " labial\nla\npara o\nsion,\nasegurese\n-\nque\narmonice\ncon\nel" +
                                                " vestido\nlle\nque\nTambién\nrà.\nel\nmaquillaje\nde\nlos\ndebe\n" +
                                                "ojos\narmoni\ncon el\nconjunto.", 
                    extractionStrategy.GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void HalftoneTest() {
            String src = TEST_IMAGE_DIRECTORY + "halftone.jpg";
            String dest = TARGET_DIRECTORY + "halftoneTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_halftoneTest.pdf";
            DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("INVOICE\nEnablers\nSilliness\nenable\nit\nYou dream we\nit\n" +
                                                "Nowhere\nof\nMiddle\nINVOICE #100\nPhone +329 292\n22 22\n6/30/2020" +
                                                "\nDATE:\n9 00\n+32 270\nFax 00\nSHIP TO\nTO:\nAndré\nAndré Lemos\n" +
                                                "Lemos\nTycoon Corp\nTycoon Corp.\nWonderful\nStreet Street\n" +
                                                "Wonderful\nLala Land\nLala Land\n+351 911 111 111\n+351 911 111111\n" +
                                                "COMMENT OR SPFCIAI INSTRUCTIONS\n-\nBF -\nITEMS MUST DELIVER FUL\n" +
                                                "ASSEMBLED\nTERMS\nRSON NUMBER SHIPPED VIA F.O.B POINT\nREQUISITIONER" +
                                                "\nP.O\nWebsite AIR Due\nor\n3Vi form receipt\n#7394009320 Delivery\n" +
                                                "PR\nDESCRIPTION UNIT RICE TOTAL\nQUANTITY\n$3000 $30000\n10\nLasers" +
                                                "\n$1 $2\n2 Band-Aids\n$99999 $499995\n5\nSharks", 
                    extractionStrategy.GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ArabicDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "arabic_01.jpg";
            String dest = TARGET_DIRECTORY + "arabicTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_arabicTest.pdf";
            DoOcrAndCreatePdf(src, dest);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void BengaliDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "bengali_01.jpeg";
            String dest = TARGET_DIRECTORY + "bengaliTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_bengaliTest.pdf";
            DoOcrAndCreatePdf(src, dest);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ChineseDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "chinese_01.jpg";
            String dest = TARGET_DIRECTORY + "chineseTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_chineseTest.pdf";
            DoOcrAndCreatePdf(src, dest);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FrenchDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "french_01.png";
            String dest = TARGET_DIRECTORY + "frenchTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_frenchTest.pdf";
            DoOcrAndCreatePdf(src, dest);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GeorgianDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "georgian_01.jpg";
            String dest = TARGET_DIRECTORY + "georgianTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_georgianTest.pdf";
            DoOcrAndCreatePdf(src, dest);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GermanDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "german_01.jpg";
            String dest = TARGET_DIRECTORY + "germanTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_germanTest.pdf";
            DoOcrAndCreatePdf(src, dest);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GreekDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "greek_01.jpg";
            String dest = TARGET_DIRECTORY + "greekTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_greekTest.pdf";
            DoOcrAndCreatePdf(src, dest);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void HindiDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "hindi_01.jpg";
            String dest = TARGET_DIRECTORY + "hindiTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_hindiTest.pdf";
            DoOcrAndCreatePdf(src, dest);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void JapaneseDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "japanese_01.png";
            String dest = TARGET_DIRECTORY + "japaneseTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_japaneseTest.pdf";
            DoOcrAndCreatePdf(src, dest);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SpanishDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "spanish_01.jpg";
            String dest = TARGET_DIRECTORY + "spanishTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_spanishTest.pdf";
            DoOcrAndCreatePdf(src, dest);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void ThaiDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "thai_01.jpg";
            String dest = TARGET_DIRECTORY + "thaiTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_thaiTest.pdf";
            DoOcrAndCreatePdf(src, dest);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
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

        private void DoOcrAndCreatePdf(String imagePath, String destPdfPath) {
            DoOcrAndCreatePdf(imagePath, destPdfPath, null);
        }
    }
}
