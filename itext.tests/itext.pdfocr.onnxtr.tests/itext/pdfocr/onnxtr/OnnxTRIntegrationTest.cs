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
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Orientation;
using iText.Pdfocr.Onnxtr.Recognition;
using iText.Test;

namespace iText.Pdfocr.Onnxtr {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxTRIntegrationTest : ExtendedITextTest {
        public static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        public static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory 
            + "/test/resources/itext/pdfocr/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
        }

        [NUnit.Framework.Test]
        public virtual void BasicTest() {
            String src = TEST_DIRECTORY + "images/example_04.png";
            String dest = TARGET_DIRECTORY + "basicTest.pdf";
            String cmp = TEST_DIRECTORY + "cmp_basicTest.pdf";
            String fast = TEST_DIRECTORY + "models/rep_fast_tiny-28867779.onnx";
            String crnnVgg16 = TEST_DIRECTORY + "models/crnn_vgg16_bn-662979cc.onnx";
            String mobileNetV3 = TEST_DIRECTORY + "models/mobilenet_v3_small_crop_orientation-5620cf7e.onnx";
            IDetectionPredictor detectionPredictor = OnnxDetectionPredictor.Fast(fast);
            IOrientationPredictor orientationPredictor = OnnxOrientationPredictor.MobileNetV3(mobileNetV3);
            IRecognitionPredictor recognitionPredictor = OnnxRecognitionPredictor.CrnnVgg16(crnnVgg16);
            OnnxTrOcrEngine ocrEngine = new OnnxTrOcrEngine(detectionPredictor, orientationPredictor, recognitionPredictor
                );
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(ocrEngine);
            using (PdfWriter writer = new PdfWriter(dest)) {
                ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList(new FileInfo(src)), writer).Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }
    }
}
