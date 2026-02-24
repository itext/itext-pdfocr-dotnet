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
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Pdfocr.Onnxtr.Recognition;
using iText.Test;

namespace iText.Pdfocr.Onnxtr {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxDoImageOcrTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        private static readonly String TEST_IMAGE_DIRECTORY = TEST_DIRECTORY + "images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxDoImageOcrTest";

        private static readonly String FAST = TEST_DIRECTORY + "models/rep_fast_tiny-28867779.onnx";

        private static readonly String CRNNVGG16 = TEST_DIRECTORY + "models/crnn_vgg16_bn-662979cc.onnx";

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
        public virtual void BasicDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_04.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("Does\nthis\nOCR\nthing\nreally\nwork?\nHi\nHow\nabout\na\nbigger\nfont?\n"
                 + "123456789\n123\nWhat\nabowt\ntris\nfont?\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void NumbersJPEDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "numbers_01.jpe";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("619121\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void BogusTextDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "bogusText.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("Bladeblabla\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void HalftoneDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "halftone.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("Silliness\nEnablers\nINVOICE\nYou\ndream\nit\nwe\nenable\nit\n" +
                                            "Middle\nof\nNowhere\nPhone\n+329\n292\n22\n22\nINVOICE\n#100\nFax\n+32" +
                                            "\n9\n270\n00\n00\nDATE:\n6/30/2020\nTO:\nSHIP\nTO\nAndré\nLemos\nAndré\n" +
                                            "Lemos\nTycoon\nCorp.\nTycoon\nCorp\nWonderful\nStreet\nWonderful\n" +
                                            "Street\nLala\nLand\nLala\nLand\n+351\n911\n111111\n+351\n911\n111\n" +
                                            "111\nCOMMENT\nOR\nSPFCIAI\nINSTRUCTIONS\nITEMS\nMUST\nBF\nDELIVER\n" +
                                            "-\nFUL\n-\nASSEMBLED\nRSON\nP.O\nNUMBER\nREQUISITIONER\nSHIPPED\n" +
                                            "VIA\nF.O.B\nPOINT\nTERMS\n3Vi\n#7394009320\nWebsite\nform\nAIR\n" +
                                            "Delivery\nDue\nor\nreceipt\nQUANTITY\nDESCRIPTION\nUNIT\nPR\nRICE\n" +
                                            "TOTAL\n10\nLasers\n$3000\n$30000\n2\nBand-Aids\n$1\n$2\n5\nSharks\n" +
                                            "$99999\n$499995\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void NoisyDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "noisy_01.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("Noisy\nimage\nto\ntest\nTesseract\nOCR\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void NumbersDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "nümbérs.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("619121\n", textFromImage);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("This test is failing on java 8 with ImageIO exception. In newer versions it works that is why we don't want to use Leptonica or any other 3rd-party to read such images."
            )]
        public virtual void Numbers2DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "numbers_02.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("-\n-\n&\n56\n-\n0\n01\n12345\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void PantoneBlueDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "pantone_blue.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void ScannedDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "scanned_spa_01.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("(\nAY\nSI\nENSAYARA\nCOMO\nACTUAR?\nTanto\npeor,\nlo\nmejor\nes\n" +
                                            "descansar\ny\nno\npensar\nla\nfiesta,\nsi\nse\npuede.\nNo\nhay\nnada\n" +
                                            "mas\ndesalentador\nver\nen\nlas\nfiestas\na\njovenes\ncon\ncara\nde\n" +
                                            "lastima\ny\nilusionadas\ny\nque\nse\nhan\npasado\ntodo\nel\ndia\n" +
                                            "tratando\nhallar\nlo\nmejor\ny\nla\nmas\natractiva\nmanera\nde\npres\n" +
                                            "tarse\nen\npublico.\nHay\nque\nactuar\ncon\ncalma\ny\nno\ncansaremos\n" +
                                            "de\nrepetirlo,\nLo\nmas\nimportante\nes\nsaber\nque\nse\nva\na\nponer\n" +
                                            "y\ntener\ntodo\na\nmano,\nSi\nintenta\nprobar\nun\nnuevo\nlapiz\n" +
                                            "labial\npara\nla\no\nsion,\nasegurese\nque\narmonice\ncon\nel\n-\n" +
                                            "vestido\nque\nlle\nrà.\nTambién\nel\nmaquillaje\nde\nlos\nojos\n" +
                                            "debe\narmoni\ncon\nel\nconjunto.\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void WeirdWordsDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "weirdwords.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("he23llo\nqwetyrtyqpwe-rty\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void CorruptedBmpDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_05_corrupted.bmp";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("This\nis\na\nTest\nThis\nis\na\nTest\nThis\nis\na\nTest\nThis\nis\na\n" +
                 "Test\nThis\nis\na\nTest\nThis\nis\na\nTest\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void CorruptedDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "corrupted.jpg";
            FileInfo imageFile = new FileInfo(src);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfOcrInputException), () => OnnxTestUtils.GetTextFromImage
                (imageFile, OCR_ENGINE));
            NUnit.Framework.Assert.AreEqual(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_READ_IMAGE, e.Message);
        }
    }
}
