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
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Onnx.Util;
using iText.Test;

namespace iText.Pdfocr.Onnx {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxRotationIntegrationPADDLETest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/OnnxRotationIntegrationTest/";

        private static readonly String TEST_IMAGE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxTRRotationIntegrationPADDLETest/";

        private static readonly IOcrEngine OCR_ENGINE = OcrEngineTypeWithOrientation.PADDLE.Get();

        private const String OCR_NAME = "PaddleOCR";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
        }

        [NUnit.Framework.Test]
        public virtual void RotatedTextBasicTest() {
            String src = TEST_IMAGE_DIRECTORY + "rotatedTextBasic.png";
            String dest = TARGET_DIRECTORY + OCR_NAME + "_rotatedTextBasicTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_" + OCR_NAME + "_rotatedTextBasicTest.pdf";
            String cmpTxt = TEST_DIRECTORY + "cmp_rotatedTextBasicTest.txt";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            OnnxTestUtils.ExtractTextAndCompare(dest, cmpTxt, "Text1", 0.22);
        }

        [NUnit.Framework.Test]
        public virtual void Rotated90Test() {
            String src = TEST_IMAGE_DIRECTORY + "90_degrees_rotated.jpg";
            String dest = TARGET_DIRECTORY + OCR_NAME + "_rotated90Test.pdf";
            String cmp = TEST_DIRECTORY + "cmp_" + OCR_NAME + "_rotated90Test.pdf";
            String cmpTxt = TEST_DIRECTORY + "cmp_rotated90Test.txt";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            OnnxTestUtils.ExtractTextAndCompare(dest, cmpTxt, "Text1", 0.55);
        }

        [NUnit.Framework.Test]
        public virtual void Rotated180Test() {
            String src = TEST_IMAGE_DIRECTORY + "180_degrees_rotated.jpg";
            String dest = TARGET_DIRECTORY + OCR_NAME + "_rotated180Test.pdf";
            String cmp = TEST_DIRECTORY + "cmp_" + OCR_NAME + "_rotated180Test.pdf";
            String cmpTxt = TEST_DIRECTORY + "cmp_rotated180Test.txt";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            OnnxTestUtils.ExtractTextAndCompare(dest, cmpTxt, "Text1", 0.05);
        }

        [NUnit.Framework.Test]
        public virtual void Rotated270Test() {
            String src = TEST_IMAGE_DIRECTORY + "270_degrees_rotated.jpg";
            String dest = TARGET_DIRECTORY + OCR_NAME + "_rotated270Test.pdf";
            String cmpTxt = TEST_DIRECTORY + "cmp_rotated270Test.txt";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            OnnxTestUtils.ExtractTextAndCompare(dest, cmpTxt, "Text1", 0.58);
        }

        [NUnit.Framework.Test]
        public virtual void RotatedColorsMixTest() {
            String src = TEST_IMAGE_DIRECTORY + "rotatedColorsMix.png";
            String dest = TARGET_DIRECTORY + OCR_NAME + "_rotatedColorsMixTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_" + OCR_NAME + "_rotatedColorsMixTest.pdf";
            String cmpTxt = TEST_DIRECTORY + "cmp_rotatedColorsMixTest.txt";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            OnnxTestUtils.ExtractTextAndCompare(dest, cmpTxt, "Text1", 0.2);
        }

        [NUnit.Framework.Test]
        public virtual void RotatedColorsMix2Test() {
            String src = TEST_IMAGE_DIRECTORY + "rotatedColorsMix2.png";
            String dest = TARGET_DIRECTORY + OCR_NAME + "_rotatedColorsMix2Test.pdf";
            String cmpTxt = TEST_DIRECTORY + "cmp_rotatedColorsMix2Test.txt";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            OnnxTestUtils.ExtractTextAndCompare(dest, cmpTxt, "Text1", 0.6);
        }

        [NUnit.Framework.Test]
        public virtual void RotatedCapsLCTest() {
            String src = TEST_IMAGE_DIRECTORY + "rotatedCapsLC.png";
            String dest = TARGET_DIRECTORY + OCR_NAME + "_rotatedCapsLCTest.pdf";
            String cmpTxt = TEST_DIRECTORY + "cmp_rotatedCapsLCTest.txt";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            OnnxTestUtils.ExtractTextAndCompare(dest, cmpTxt, "Text1", 0.46);
        }

        [NUnit.Framework.Test]
        public virtual void RotatedBy90DegreesTest() {
            String src = TEST_IMAGE_DIRECTORY + "rotatedBy90Degrees.png";
            String dest = TARGET_DIRECTORY + OCR_NAME + "_rotatedBy90DegreesTest.pdf";
            String cmpTxt = TEST_DIRECTORY + "cmp_rotatedBy90DegreesTest.txt";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, OCR_ENGINE);
            OnnxTestUtils.ExtractTextAndCompare(dest, cmpTxt, "Text1", 0.16);
        }
    }
}
