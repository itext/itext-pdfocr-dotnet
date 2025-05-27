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
using Org.Bytedeco.Opencv.Opencv_core;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Pdfocr;
using iText.Pdfocr.Onnxtr;
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Recognition;

namespace iText.Pdfocr.Onnxtr.Util {
//\cond DO_NOT_DOCUMENT
    internal class OpenCvUtilTest {
        private const float WIDTH = 5;

        private const float HEIGHT = 10;

        [NUnit.Framework.Test]
        public virtual void Test() {
            String src = "/Users/angelinapavlovets/itext/java/pdfocr/pdfocr-tesseract4/src/test/resources/com/itextpdf/pdfocr/images/example_04.png";
            String dest = "/Users/angelinapavlovets/Downloads/out.pdf";
            System.Console.Out.WriteLine("Out pdf: " + UrlUtil.GetNormalizedFileUriString(dest));
            IDetectionPredictor detectionPredictor = OnnxDetectionPredictor.Fast("/Users/angelinapavlovets/Downloads/rep_fast_tiny-28867779.onnx"
                );
            //        IOrientationPredictor orientationPredictor = OnnxOrientationPredictor.mobileNetV3();
            IRecognitionPredictor recognitionPredictor = OnnxRecognitionPredictor.CrnnVgg16("/Users/angelinapavlovets/Downloads/crnn_vgg16_bn-662979cc.onnx"
                );
            OnnxTrOcrEngine reader = new OnnxTrOcrEngine(detectionPredictor, recognitionPredictor);
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(reader);
            try {
                using (PdfWriter writer = new PdfWriter(dest)) {
                    ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList(new FileInfo(src)), writer).Close();
                }
            }
            catch (System.IO.IOException e) {
                throw new Exception(e);
            }
        }

//\cond DO_NOT_DOCUMENT
        [NUnit.Framework.Test]
        internal virtual void NormalizeRotatedRect() {
            for (int rotationIdx = -2; rotationIdx <= 2; ++rotationIdx) {
                float baseAngle = rotationIdx * 360;
                TestNormalizeRotatedRect(baseAngle + 0, WIDTH, HEIGHT, 0);
                TestNormalizeRotatedRect(baseAngle + 30, WIDTH, HEIGHT, 30);
                TestNormalizeRotatedRect(baseAngle + 60, HEIGHT, WIDTH, -30);
                TestNormalizeRotatedRect(baseAngle + 90, HEIGHT, WIDTH, 0);
                TestNormalizeRotatedRect(baseAngle + 120, HEIGHT, WIDTH, 30);
                TestNormalizeRotatedRect(baseAngle + 150, WIDTH, HEIGHT, -30);
                TestNormalizeRotatedRect(baseAngle + 180, WIDTH, HEIGHT, 0);
                TestNormalizeRotatedRect(baseAngle + 210, WIDTH, HEIGHT, 30);
                TestNormalizeRotatedRect(baseAngle + 240, HEIGHT, WIDTH, -30);
                TestNormalizeRotatedRect(baseAngle + 270, HEIGHT, WIDTH, 0);
                TestNormalizeRotatedRect(baseAngle + 300, HEIGHT, WIDTH, 30);
                TestNormalizeRotatedRect(baseAngle + 330, WIDTH, HEIGHT, -30);
            }
        }
//\endcond

        private void TestNormalizeRotatedRect(float originalAngle, float newWidth, float newHeight, float newAngle
            ) {
            using (Point2f center = new Point2f(0, 0)) {
                using (Size2f size = new Size2f(WIDTH, HEIGHT)) {
                    using (RotatedRect rect = new RotatedRect(center, size, originalAngle)) {
                        OpenCvUtil.NormalizeRotatedRect(rect);
                        using (Size2f newSize = rect.Size()) {
                            NUnit.Framework.Assert.AreEqual(newWidth, newSize.Width(), 1e-6);
                            NUnit.Framework.Assert.AreEqual(newHeight, newSize.Height(), 1e-6);
                        }
                        NUnit.Framework.Assert.AreEqual(newAngle, rect.Angle(), 1e-6);
                    }
                }
            }
        }
    }
//\endcond
}
