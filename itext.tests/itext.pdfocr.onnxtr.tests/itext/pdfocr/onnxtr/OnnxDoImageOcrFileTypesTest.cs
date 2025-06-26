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
    public class OnnxDoImageOcrFileTypesTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        private static readonly String TEST_IMAGE_DIRECTORY = TEST_DIRECTORY + "images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxDoImageOcrFileTypesTest";

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
        public virtual void NumbersJPEDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "numbers_01.jpe";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("619121\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void ExampleBMPDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_01.BMP";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("Test\nOCR\nScanner\n-\nmessage\nfor\n1S\na\ntest\nIhis\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void ExampleJFIFDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_02.JFIF";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("Test\nOCR\nScanner\nmessage\nfor\n1S\na\ntest\nIhis\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Example10mvTIFFDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_03_10MB.tiff";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("Image\nTagged\nFormat\nFile\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void MultipageTiffDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "multipage.tiff";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("1\nPage\nExample\nTIFF\nMultipage\n" + "-\nPage\n2\nExample\nTIFF\nMultipage\n"
                 + "Page\n3\nExample\nTIFF\nMultipage\n" + "4\nPage\nExample\nTIFF\nMultipage\n" + "Page5\nExample\nTIFF\nMultipage\n"
                 + "Page\n6\nExample\nTIFF\nMultipage\n" + "/\nPage\nExample\nTIFF\nMultipage\n" + "8\nPage\nExample\nTIFF\nMultipage\n"
                 + "Page\n9\nExample\nTIFF\nMultipage\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void NumbersJpgDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "numbers_01.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("619121\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void NumbersNnnDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "numbers_01.nnn";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("619121\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void NumbersTifDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "numbers_01.tif";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("619121\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void GifDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "weirdwords.gif";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("qwetyrtyqpwe-rty\nhe23llo\n", textFromImage);
        }
    }
}
