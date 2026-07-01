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
using iText.Pdfocr.Onnx.Detection;
using iText.Pdfocr.Onnx.Recognition;
using iText.Pdfocr.Onnx.Util;
using iText.Test;

namespace iText.Pdfocr.Onnx {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxDoImageOcrLanguagesTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        private static readonly String TEST_IMAGE_DIRECTORY = TEST_DIRECTORY + "images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxDoImageOcrLanguagesTest";

        private static readonly String FAST = TEST_DIRECTORY + "models/rep_fast_tiny-28867779.onnx";

        // We use MULTILANG for the languages it supports.
        private static readonly String MULTILANG = TEST_DIRECTORY + "models/onnxtr-parseq-multilingual-v1.onnx";

        private static OnnxOcrEngine MULTILANG_ENGINE;

        private static OnnxOcrEngine OCR_ENGINE;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
            IDetectionPredictor detectionPredictor = OnnxDetectionPredictor.Fast(FAST);
            IRecognitionPredictor recognitionPredictor = OnnxRecognitionPredictor.ParSeq(MULTILANG,
                Vocabulary.LATIN_EXTENDED, 0);
            MULTILANG_ENGINE = new OnnxOcrEngine(detectionPredictor, recognitionPredictor);
            OCR_ENGINE = OcrEngineType.DOCTR.Get();
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterAll() {
            MULTILANG_ENGINE.Close();
        }

        [NUnit.Framework.Test]
        public virtual void RussianDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "russian.jpg";
            FileInfo imageFile = new FileInfo(src);
            // Let's use multilang here though it doesn't support cyrillic
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("Heẞpocerw\nV\nWX\n8.1m9me\nha\nXM3HL\n4eJTObeka\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Arabic1DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "arabic_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("13\n-\n6\nSta:as)\n9\n4at\n-\nlive,\nlaugh,\nlove\nA\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Arabic2DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "arabic_02.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("Ayssell\nRalll\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void BengaliDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "bengali_01.jpeg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("3(5\nT(3T\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void ChineseDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "chinese_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("I\n4\n-\n-\nnI\nhao\nK/i\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void EngBmpDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "englishText.bmp";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("This\n1S\na\ntest\nmessage\nfor\n-./:\nOCR\nScanner\nTest\nBMPTest\n", textFromImage
                );
        }

        [NUnit.Framework.Test]
        public virtual void FrenchDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "french_01.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("RESTEZ\nCALME\nET\nPARLEZ\nEN\nFRANÇAIS\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void GeorgianDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "georgian_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("03960000\nL\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void GermanDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "german_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("Das\nGeheimnis\ndes\nKònnens\nliegt\nim\nWollen.\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void GreekDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "greek_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.IsTrue(textFromImage.Contains("dycGuxns"));
        }

        [NUnit.Framework.Test]
        public virtual void Hindi1DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "hindi_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("-\nG\ntTT\nBeas\no\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Hindi2DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "hindi_02.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("dloich\nSlaiai\nHindi\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void InvoiceThaiDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "invoice_front_thai.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("QUANTITY\nDESCRIPTION\nUNIT\nPRICE\nTOTAL\n10\nLasers\n$3000\n$30000\n2\nBand-Aids\n$1\n$2\n5\ndunnasi?\n$99999\n$499995\n"
                , textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void JapaneseDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "japanese_01.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("B\n*\n-\naa\n-\na\nK\n*\n-\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void MultiLangDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "multilang.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("The\n(quick)\n[brown]\n{fox}\njumps!\nOver\nthe\n$43,456.78\n<lazy>\n#90\ndog\n"
                 + "&\nduck/goose,\nas\n12.5%\nof\nE-mai\nfrom\naspammer\n@website.com\nis\nspam.\nDer\nschnelle\n" + 
                "\"J\nbraune\nFuchs\nspringt\nüber\nden\nfaulen\nHund.\nLe\nrenard\nbrun\n<rapide>\nsaute\npar-" + "dessus\nle\nchien\nparesseux.\nLa\nvolpe\nmarrone\nrapida\nsalta\nsopra\nil\ncane\npigro.\nEI\n"
                 + "zorro\nmarron\nrapido\nsalta\nsobre\nel\nperro\nperezoso.\n%&'(\nraposa\nmarrom\nrapida\nsalta\n" 
                + "sobre\nO\ncao\npreguicoso.\n4\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void SpanishDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "spanish_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("Aquí\nhablamos\nespañol\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Thai1DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "thai_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("3581981\n1\n19n8\nA\nA\nI\na\n&\n1\n1900879191497907597\n15790707047005\n19n8\n"
                , textFromImage, textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Thai2DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "thai_02.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.IsTrue(textFromImage.Contains("MANAMRAAMSLI"), textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Thai3DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "thai_03.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.IsTrue(textFromImage.Contains("ousosowuaSoodzub"));
            NUnit.Framework.Assert.IsTrue(textFromImage.Contains("mnsnduingunlfuusais"));
        }
    }
}
