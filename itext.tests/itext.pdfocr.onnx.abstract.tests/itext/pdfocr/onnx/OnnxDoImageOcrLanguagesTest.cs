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
using iText.Test;

namespace iText.Pdfocr.Onnx
{
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxDoImageOcrLanguagesTest : ExtendedITextTest
    {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework
            .TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        private static readonly String TEST_IMAGE_DIRECTORY = TEST_DIRECTORY + "images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
                                                          + "/test/resources/itext/pdfocr/OnnxDoImageOcrLanguagesTest";

        private static readonly String FAST = TEST_DIRECTORY + "models/rep_fast_tiny-28867779.onnx";

        private static readonly String MULTILANG = TEST_DIRECTORY + "models/onnxtr-parseq-multilingual-v1.onnx";
        private static OnnxOcrEngine MULTILANG_ENGINE;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass()
        {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);

            IDetectionPredictor detectionPredictor = OnnxDetectionPredictor.Fast(FAST);
            IRecognitionPredictor recognitionPredictor = OnnxRecognitionPredictor.ParSeq(MULTILANG);
            MULTILANG_ENGINE = new OnnxOcrEngine(detectionPredictor, recognitionPredictor);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass()
        {
            MULTILANG_ENGINE.Close();
        }

        [NUnit.Framework.Test]
        public virtual void RussianDoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "russian.jpg";
            FileInfo imageFile = new FileInfo(src);

            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("Heẞpocerw\nV\nWX\n8.1m9me\nha\nXM3HL\n4eJTObeka\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Arabic1DoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "arabic_01.jpg";
            FileInfo imageFile = new FileInfo(src);

            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("_`{|}\n°\n<\nÌ\n¿tau2aÎÏ\nç\n;<=>?!\nż\nlive,\nlaugh,\nlove\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Arabic2DoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "arabic_02.png";
            FileInfo imageFile = new FileInfo(src);

            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("Ãńř;æÍÍ\nïłÍÍ[\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void BengaliDoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "bengali_01.jpeg";
            FileInfo imageFile = new FileInfo(src);

            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("š;3ï35Ě\n*T(3IT\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void ChineseDoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "chinese_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("/-\nltk\nu\n_`{|}\n_\nni\nhao\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void EngBmpDoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "englishText.bmp";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("This\n1S\na\ntest\nmessage\n-./:\nfor\nOCR\nScanner\nTest\nBMPTest\n",
                textFromImage
            );
        }

        [NUnit.Framework.Test]
        public virtual void FrenchDoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "french_01.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("RESTEZ\nCALME\nET\nPARLEZ\nEN\nFRANÇAIS\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void GeorgianDoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "georgian_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("m8gmono\nU\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void GermanDoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "german_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("Das\nGeheimnis\ndes\nKònnens\nliegt\nim\nWollen.\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void GreekDoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "greek_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual(
                "`\n°\nP\n\\\n;<=>?\nz\nO\n>\nË\nM\n€\n^\nO\nI\nI\nA\n~\nå\n715%ňř\nå\nzui.bozuss\nmu\n/\nzy\na!%0§\"ò7\\ïł'\n°\nÏ\n&XX7\\Y¡ÄŽ2\nå\nalvazxizzi=.\n~\n"
                , textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Hindi1DoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "hindi_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("@\n-\nç\n--TT-TT\nØøæøå\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Hindi2DoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "hindi_02.jpg";
            FileInfo imageFile = new FileInfo(src);
            
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("H|oich\n515Ěß1\nHindi\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void InvoiceThaiDoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "invoice_front_thai.jpg";
            FileInfo imageFile = new FileInfo(src);
            
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual(
                "QUANTITY\nDFSCRIPTION\nUNIT\nPRICE\nTOTAL\n10\nLasers\n$3000\n$30000\n2\nBand-Aids\n$1\n$2\n5\nduñnã3ñ3\n$99999\n$499995\n"
                
                , textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void JapaneseDoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "japanese_01.png";
            FileInfo imageFile = new FileInfo(src);
            
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("E\n`\næ\nãã\n`\nD\nsu\n;\n`\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void MultiLangDoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "multilang.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("The\n(quick)\n[brown]\n{fox}\njumps!\nOver\nthe\n$43,456.78\n<lazy>\n" +
                                            "#90\ndog\n&\nduck/goose,\nas\n12.5%\nof\nE-mai\nfrom\naspammer\n@website.com\nis\nspam.\nDer\n" +
                                            "schnelle\n\"J\nbraune\nFuchs\nspringt\nüber\nden\nfaulen\nHund.\nLe\nrenard\nbrun\n<rapide>\nsaute\n"
                                            + "par-dessus\nle\nchien\nparesseux.\nLa\nvolpe\nmarrone\nrapida\nsalta\nsopra\nil\ncane\npigro.\nEI\n"
                                            + "zorro\nmarron\nrapido\nsalta\nsobre\nel\nperro\nperezoso.\n%&'(\n4\nraposa\nmarrom\nrapida\nsalta\n"
                                            + "sobre\nO\ncao\npreguicoso.\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void SpanishDoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "spanish_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual("Aquí\nhablamos\nespañol\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Thai1DoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "thai_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.AreEqual(
                "ilssma\nIr\nŹ=ø\n=\n~\n'\n=\no\nIr\n1198B811.m113f1333\n131¥01010!30n3\n'ha\n"
                , textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Thai2DoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "thai_02.jpg";
            FileInfo imageFile = new FileInfo(src);
            
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.IsTrue(textFromImage.Contains("Auflusnurtumennertkummen\n"));
        }

        [NUnit.Framework.Test]
        public virtual void Thai3DoImageOcrTest()
        {
            String src = TEST_IMAGE_DIRECTORY + "thai_03.jpg";
            FileInfo imageFile = new FileInfo(src);
            
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, MULTILANG_ENGINE);
            NUnit.Framework.Assert.IsTrue(textFromImage.Contains("lauilsnumelnnìungs:\nniliama\nmø"));
        }
    }
}