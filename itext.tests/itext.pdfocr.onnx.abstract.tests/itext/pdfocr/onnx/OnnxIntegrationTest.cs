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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfocr.Onnx.Detection;
using iText.Pdfocr.Onnx.Orientation;
using iText.Pdfocr.Onnx.Recognition;
using iText.Pdfocr.Onnx.Util;
using iText.Pdfocr.Util;
using iText.Test;

namespace iText.Pdfocr.Onnx {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxIntegrationTest : ExtendedITextTest {
        private static readonly String FAST = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/rep_fast_tiny-28867779.onnx";

        private static readonly String CRNNVGG16 = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/crnn_vgg16_bn-662979cc.onnx";

        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/OnnxIntegrationTest/";

        private static readonly String TEST_IMAGE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxIntegrationTest/";

        private static OnnxOcrEngine OCR_ENGINE;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
            OCR_ENGINE = OcrEngineType.DOCTR.Get();
        }

        [NUnit.Framework.Test]
        public virtual void BasicTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_04.png";
            String dest = TARGET_DIRECTORY + "basicTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_basicTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void JfifTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_02.JFIF";
            String dest = TARGET_DIRECTORY + "jfifTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_jfifTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().SetContentStreamFloatTolerance(0.02f).CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("Test\nmessage for\nOCR Scanner\nIhis a test\n1S\n-", extractionStrategy.GetResultantText
                    ());
            }
        }

        [NUnit.Framework.Test]
        public virtual void Tiff10MBTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_03_10MB.tiff";
            String dest = TARGET_DIRECTORY + "tiff10MBTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_tiff10MBTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("Tagged Image File Format", extractionStrategy.GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void JpeTest() {
            String src = TEST_IMAGE_DIRECTORY + "numbers_01.jpe";
            String dest = TARGET_DIRECTORY + "jpeTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_jpeTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
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
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("619121", extractionStrategy.GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ScannedTest() {
            String src = TEST_IMAGE_DIRECTORY + "scanned_spa_01.png";
            String dest = TARGET_DIRECTORY + "scannedTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("-\n"
                    + "AY SI ENSAYARA COMO ACTUAR?\n"
                    + "Tanto peor, lo mejor es descansar y no pensar\n"
                    + "la fiesta, si se puede. No hay nada mas desalentador\n"
                    + "ver en las fiestas a jovenes con cara de lastima y\n"
                    + "iluslonadas y que se han pasado todo el dia tratando\n"
                    + "hallar lo mejor y la mas atractiva manera de pres\n"
                    + "tarse en publico. Hay que actuar con calma y no\n"
                    + "cansaremos de repetirlo, Lo mas importante es saber\n"
                    + "que se va a poner y tener todo a mano,\n"
                    + "Si intenta probar un nuevo lapiz labial para la o\n"
                    + "sion, asegurese que armonice con el vestido\n"
                    + "-\n"
                    + "que lle\n"
                    + "rà.\n"
                    + "También el maquillaje de los ojos debe armoni\n"
                    + "con el conjunto.",
                    extractionStrategy.GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void HalftoneTest() {
            String src = TEST_IMAGE_DIRECTORY + "halftone.jpg";
            String dest = TARGET_DIRECTORY + "halftoneTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("INVOICE\n"
                    + "Silliness Enablers\n"
                    + "You dream it we enable it\n"
                    + "Middle of Nowhere\n"
                    + "INVOICE #100\n"
                    + "Phone +329 292 22 22\n"
                    + "DATE: 6/30/2020\n"
                    + "Fax +32 9 270 00 00\n"
                    + "SHIP TO\n"
                    + "TO:\n"
                    + "André Lemos\n"
                    + "André Lemos\n"
                    + "Tycoon Corp\n"
                    + "Tycoon Corp.\n"
                    + "Wonderful Street\n"
                    + "Wonderful Street\n"
                    + "Lala Land\n"
                    + "Lala Land\n"
                    + "+351 911 111 111\n"
                    + "+351 911 111111\n"
                    + "COMMENT OR SPFCIAI INSTRUCTIONS\n"
                    + "ITEMS MUST BF DELIVER - FUL - ASSEMBLED\n"
                    + "RSON\n"
                    + "P.O NUMBER REQUISITIONER SHIPPED VIA F.O.B POINT TERMS\n"
                    + "3Vi #7394009320 Website form AIR\n"
                    + "Delivery Due or receipt\n"
                    + "DESCRIPTION UNIT PR RICE TOTAL\n"
                    + "QUANTITY\n"
                    + "$3000 $30000\n"
                    + "10\n"
                    + "Lasers\n"
                    + "$1 $2\n"
                    + "2 Band-Aids\n"
                    + "$99999 $499995\n"
                    + "5\n"
                    + "Sharks"
                    , extractionStrategy.GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ArabicDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "arabic_01.jpg";
            String dest = TARGET_DIRECTORY + "arabicTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_arabicTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().SetContentStreamFloatTolerance(0.02f).CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void BengaliDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "bengali_01.jpeg";
            String dest = TARGET_DIRECTORY + "bengaliTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_bengaliTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            OnnxTestUtils.ComparePdfs(dest, cmp, TARGET_DIRECTORY);
        }

        [NUnit.Framework.Test]
        public virtual void ChineseDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "chinese_01.jpg";
            String dest = TARGET_DIRECTORY + "chineseTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_chineseTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().SetContentStreamFloatTolerance(0.02f).CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void FrenchDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "french_01.png";
            String dest = TARGET_DIRECTORY + "frenchTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_frenchTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GeorgianDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "georgian_01.jpg";
            String dest = TARGET_DIRECTORY + "georgianTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_georgianTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void GermanDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "german_01.jpg";
            String dest = TARGET_DIRECTORY + "germanTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_germanTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void HindiDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "hindi_01.jpg";
            String dest = TARGET_DIRECTORY + "hindiTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_hindiTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().SetContentStreamFloatTolerance(0.02f).CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void JapaneseDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "japanese_01.png";
            String dest = TARGET_DIRECTORY + "japaneseTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_japaneseTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void SpanishDocTest() {
            String src = TEST_IMAGE_DIRECTORY + "spanish_01.jpg";
            String dest = TARGET_DIRECTORY + "spanishTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_spanishTest.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            OnnxTestUtils.ComparePdfs(dest, cmp, TARGET_DIRECTORY);
        }

        [NUnit.Framework.Test]
        public virtual void BmpByWordsTest() {
            String src = TEST_IMAGE_DIRECTORY + "englishText.bmp";
            String dest = TARGET_DIRECTORY + "bmpTestByWords.pdf";
            String cmp = TEST_DIRECTORY + "cmp_bmpTestByWords.pdf";
            OnnxDetectionPredictor detectionPredictor = OnnxDetectionPredictor.Fast(FAST);
            OnnxRecognitionPredictor recognitionPredictor = OnnxRecognitionPredictor.CrnnVgg16(CRNNVGG16);
            using (OnnxOcrEngine onnxOcrEngine = new OnnxIntegrationTest.RotationAgnosticOnnxOcrEngine(detectionPredictor
                , null, recognitionPredictor, new OnnxEngineProperties().SetTextPositioning(iText.Pdfocr.Onnx.Text.TextPositioning
                .BY_WORDS))) {
                OnnxTestUtils.DoOcrAndCreatePdf(src, dest, onnxOcrEngine);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual("This\n1S test\na\nfor\nmessage\n-\nOCR\nScanner\nTest\nBMPTest", extractionStrategy
                    .GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void ObliqueLinesTest() {
            String src = TEST_IMAGE_DIRECTORY + "obliqueLines.png";
            String dest = TARGET_DIRECTORY + "obliqueLines.pdf";
            String cmp = TEST_DIRECTORY + "cmp_obliqueLines.pdf";
            OnnxDetectionPredictor detectionPredictor = OnnxDetectionPredictor.Fast(FAST);
            OnnxRecognitionPredictor recognitionPredictor = OnnxRecognitionPredictor.CrnnVgg16(CRNNVGG16);
            using (OnnxOcrEngine onnxOcrEngine = new OnnxIntegrationTest.RotationAgnosticOnnxOcrEngine(detectionPredictor
                , null, recognitionPredictor, new OnnxEngineProperties().SetTextPositioning(iText.Pdfocr.Onnx.Text.TextPositioning
                .BY_LINES))) {
                OnnxTestUtils.DoOcrAndCreatePdf(src, dest, onnxOcrEngine);
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        /// <summary>
        /// Implementation of the
        /// <see cref="OnnxOcrEngine"/>
        /// supporting only 0, 90, 180 and 270 degrees text rotation.
        /// </summary>
        public class RotationAgnosticOnnxOcrEngine : OnnxOcrEngine {
            /// <summary>Create a new OCR engine with the provided predictors.</summary>
            /// <param name="detectionPredictor">text detector. For an input image it outputs a list of text boxes</param>
            /// <param name="orientationPredictor">
            /// text orientation predictor. For an input image, which is a tight  crop of text,
            /// it outputs its orientation in 90 degrees steps. Can be null, in that case all text
            /// is assumed to be upright
            /// </param>
            /// <param name="recognitionPredictor">
            /// text recognizer. For an input image, which is a tight crop of text, it outputs the
            /// displayed string
            /// </param>
            /// <param name="properties">set of properties</param>
            public RotationAgnosticOnnxOcrEngine(IDetectionPredictor detectionPredictor, IOrientationPredictor orientationPredictor
                , IRecognitionPredictor recognitionPredictor, OnnxEngineProperties properties)
                : base(detectionPredictor, orientationPredictor, recognitionPredictor, properties) {
            }

            public override IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input, OcrProcessContext ocrProcessContext
                ) {
                return PdfOcrTextBuilder.CorrectRotationAngle(base.DoImageOcr(input, ocrProcessContext));
            }
            
            public override IDictionary<int, IList<TextInfo>> DoImageOcr(IList<FileInfo> inputs, OcrProcessContext ocrProcessContext
            ) {
                return PdfOcrTextBuilder.CorrectRotationAngle(base.DoImageOcr(inputs, ocrProcessContext));
            }
        }
    }
}
