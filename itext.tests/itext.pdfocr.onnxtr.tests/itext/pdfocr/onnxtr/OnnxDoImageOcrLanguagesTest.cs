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
    public class OnnxDoImageOcrLanguagesTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        private static readonly String TEST_IMAGE_DIRECTORY = TEST_DIRECTORY + "images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxDoImageOcrLanguagesTest";

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
        public virtual void Arabic1DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "arabic_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("laugh,\nlove\nlive,\n€\n6\n-\n9\n1\nStsas\nC4at\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Arabic2DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "arabic_02.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("Aysell\nRalll\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void BengaliDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "bengali_01.jpeg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("T(3T\n383(51\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void ChineseDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "chinese_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("nI\nhao\nK/i\n-\n4\n\n-\nA\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void EngBmpDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "englishText.bmp";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("BMPTest\nTest\nOCR\nScanner\n.\nmessage\nfor\n1S\na\ntest\nThis\n", textFromImage
                );
        }

        [NUnit.Framework.Test]
        public virtual void FrenchDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "french_01.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("FRANÇAIS\n-\nPARLEZ\nEN\nET\nCALME\nRESTEZ\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void GeorgianDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "georgian_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("L\n03960000\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void GermanDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "german_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("1m\nWollen.\nliegt\nKonnens\ndes\nDas\nGeheimnis\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void GreekDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "greek_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("9XX240109\n2\n$\n4\ndycGuxns.\n2\n715EWg\n7\nCTOS02u2n5\nxabozuxns\n" +
                                            "$\n$\n/\n2\n2\nC\nA\nC)\nM\nA\nI\nI\n-\n0\nV\nP\n-\nE\n0\n3\n"
                , textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Hindi1DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "hindi_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("Beas\n-\n-\nG\ntT\no\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Hindi2DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "hindi_02.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("Hindi\ndoch\nSloial\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void InvoiceThaiDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "invoice_front_thai.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("aufnasi?\n$499995\n$99999\n5\nBand-Aids\n2\n$2\n$1\nLasers\n$3000\n" +
                                            "10\n$30000\nTOTAL\nPRICE\nUNIT\nDESCRIPTION\nQUANTITY\n"

                , textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void JapaneseDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "japanese_01.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("K\n*\nA\nB:\n-\na\nE\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void MultiLangDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "multilang.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("cao\npreguicoso.\nO\nsobre\nsalta\nraposa\nperezoso.\nmarrom\n-\n" +
                                            "rapida\nA\nperro\nmarron\nsobre\nsalta\nrapido\nel\nsopra\nzorro\ncane\n" +
                                            "pigro.\nsalta\nEl\nil\nmarrone\nparesseux.\nrapida\nvolpe\nLa\npar-dessus\n" +
                                            "<rapiden\nsaute\nchien\nle\nrenard\nbrun\nHund.\nfaulen\nuber\nLe\nden\n" +
                                            "springt\nbraune\nschnelle\nFuchs\nDer\n33\nspam.\naspammer\n@website.com\n" +
                                            "from\nis\nas\nduck/goose,\nE-mail\n12.5%\nof\n&\ndog\n<lazy>\n$43,456.78\n" +
                                            "Over\n#90\nthe\njumps!\n[brown]\n(quick)\nffox)\nThe\n"
                , textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void SpanishDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "spanish_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("espafiol\nhablamos\n-\nAqui\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Thai1DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "thai_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("3900879191497907597\n15790707047005\n19n8\n35819F1\n19n8\n11\n&\na\n1\n11\nA\nA\n", textFromImage,
                textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Thai2DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "thai_02.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.IsTrue(textFromImage.Contains("GNILwEnyEwaUChYONWEnNAVoupruselisurulunaunwwuryeuAu\n"
                + "wsoluwnanaurw)\n"
                + "lniloumenounguivyuEREnRonananuituwdryensahluwun"));
        }

        [NUnit.Framework.Test]
        public virtual void Thai3DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "thai_03.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.IsTrue(textFromImage.Contains("masnduingunilfuinisn\n" +
                "sosna\n" +
                "Laiuinss\n" +
                "aMWASuOADA\n" +
                "vOng\n" +
                "WwunnlASMElNOunalnuar\n" +
                "Loninaclae"));
        }
    }
}
