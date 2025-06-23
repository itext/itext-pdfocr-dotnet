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
using iText.Pdfocr.Onnxtr.Detection;
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

        [NUnit.Framework.Test]
        public virtual void BasicDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_04.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("font?\nWhat\ntis\nabowt\n123456789\n123\nbigger\na\nfont?\nabout\nHow\nHi\nreally\nwork\nthing\nDoes\nOCR\nthis\n"
                , textFromImage);
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
        [NUnit.Framework.Ignore("DEVSIX-9232")]
        public virtual void HalftoneDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "halftone.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("Sharks\n$499995\n$99999\nBand-Aids\n2\n$2\n$1\nLasers\n10\n" + "$30000\n$3000\nQUANTITY\nTOTAL\nPRICE\nUNIT\nDESCRIPTION\n\nV\nreceipt\non\nR\nVebsite\n#7394009320"
                 + "\n3Vi\nDue\nDelivery\nAl\nform\nES\nS/\nNUMBER\nP.O\nON\nRSC\nTERMS\nPOINT\nF.O.B\nVIA\nSHIPPED\n"
                 + "REQUISITIONER\nMUST\nITEMS\nASSEMBLED\nFULLY\nDELIVERED\nBE\nINSTRUCTIONS\nSPFCIAI\nOR\nAMENTS\nC"
                 + "\n+351\n111\n111\n911\n+351\n111\n111\n911\nLand\nLala\nLand\nLala\nWonderfulStreet\nStreet\n" + "Wonderfu\nTycoor\nCorp.\nTycoon\nCorp\nLe\nemos\nLemos\nAndré\nAndré\nTO:\nTO:\nSHIP\n270\n9\n+32"
                 + "\nFax\n00\n00\n6/30/2020\nDATE:\n22\n22\n292\n9\n+32\nPhone\n#100\nINVOICE\nNowhere\nof\nMiddle\nwe"
                 + "\ndream\nYou\nit\nenable\nit\nSilliness\nEnablers\nINVOICE\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void NoisyDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "noisy_01.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("Tesseract\nOCR\nto\ntest\nimage\nNoisy\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void NumbersDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "nümbérs.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("619121\n", textFromImage);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("This test is failing on java 8 with ImageIO exception. In newer versions it works that is why we don't"
             + "want to use Leptonica or any other 3rd-party to read such images.")]
        public virtual void Numbers2DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "numbers_02.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("-\n-\n&\n56\n0\n01\n12345\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void PantoneBlueDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "pantone_blue.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("", textFromImage);
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-9232")]
        public virtual void ScannedDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "scanned_spa_01.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("conjunto,\ncon\nel\narmonil\ndebe\nojos\nlos\nmaquillaje\nde\nel" + "\nTambién\nrà.\nque\nlle\nvestido\ncon\narmonice\nque\nel\nasegurese\nsion,\npara\na\nla\nlabial"
                 + "\nnuevo\nlapiz\nun\nprobar\nintenta\nSi\nmano,\na\ntodo\ny\ntener\nponer\na\nva\nse\nque\nes\nsaber"
                 + "\nimportante\nmas\nrepetirlo,\nLo\ncansaremos\nde\nno\ny\ncon\ncalma\nque\nactuar\nHay\npublico." 
                + "\nen\ntarse\npres\nmanera\nde\natractiva\nmas\ny\nmejor\nla\nhallar\nlo\ntratando\ndia\nel\ntodo\n"
                 + "pasado\nque\nse\ny\nhan\niluslonadas\ny\ncara\nlastima\ncon\nde\njovenes\na\nen\nfiestas\nver\nlas"
                 + "\ndesalentador\nmas\nnada\nhay\npuede.\nse\nNo\nfiesta,\nsi\nla\npensar\nno\ny\nes\ndescansar\nmejor"
                 + "\npeor,\nlo\nTanto\nACTUAR?\nCOMO\nENSAYARA\nSI\n1Y\n\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void WeirdWordsDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "weirdwords.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void CorruptedBmpDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_05_corrupted.bmp";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("a\nTest\nThis\nis\na\nTest\nThis\nis\na\nTest\nThis\nis\na\n" + "Test\nThis\nis\na\nTest\nis\nThis\na\nTest\nis\nThis\n"
                , textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void CorruptedDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "corrupted.jpg";
            FileInfo imageFile = new FileInfo(src);
            NUnit.Framework.Assert.Catch(typeof(OutOfMemoryException), () => OnnxTestUtils.GetTextFromImage(imageFile
                , OCR_ENGINE));
        }
    }
}
