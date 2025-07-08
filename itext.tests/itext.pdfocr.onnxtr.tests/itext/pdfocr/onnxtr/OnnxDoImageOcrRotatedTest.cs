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
using iText.Pdfocr.Onnxtr.Orientation;
using iText.Pdfocr.Onnxtr.Recognition;
using iText.Test;

namespace iText.Pdfocr.Onnxtr {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxDoImageOcrRotatedTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        private static readonly String TEST_IMAGE_DIRECTORY = TEST_DIRECTORY + "images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxDoImageOcrRotatedTest";

        private static readonly String FAST = TEST_DIRECTORY + "models/rep_fast_tiny-28867779.onnx";

        private static readonly String CRNNVGG16 = TEST_DIRECTORY + "models/crnn_vgg16_bn-662979cc.onnx";

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
        public virtual void Rotated270DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "270_degrees_rotated.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("image\ndegrees\n270\nrotated\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Rotated180DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "180_degrees_rotated.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("180\ndegrees\nrotated\nimage\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void Rotated90DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "90_degrees_rotated.jpg";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("90\nrotated\ndegrees\nimage\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void RotatedTextBasicDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "rotatedTextBasic.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("TEST\nThis\ntext\nLIS\nTxT\nsideways\nDiagonal\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void RotatedCapsLCDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "rotatedCapsLC.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("TEsTinG\nmix\nanD\nlowerCaSE\nCapITALS\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void RotatedColorsMixDoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "rotatedColorsMix.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("ReD\nCOIORS\ntEXT\nMixed\nTEXT\nColored\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void RotatedColorsMix2DoImageOcrTest() {
            String src = TEST_IMAGE_DIRECTORY + "rotatedColorsMix2.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("not\nhave\na\nproblem.\n&%!Housten\nwe\nshould\n123456789-Fdppt\nwork?\nthis\ndoes\n"
                , textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void RotatedBy90DegreesTest() {
            String src = TEST_IMAGE_DIRECTORY + "rotatedBy90Degrees.png";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = OnnxTestUtils.GetTextFromImage(imageFile, OCR_ENGINE);
            NUnit.Framework.Assert.AreEqual("TEXT\n180\n270\nTEXT\n-\n90\nTEXT\nTEXT\n0\n", textFromImage);
        }
    }
}
