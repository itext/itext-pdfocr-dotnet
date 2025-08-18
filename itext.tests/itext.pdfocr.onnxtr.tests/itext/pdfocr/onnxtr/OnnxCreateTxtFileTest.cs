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
using System.Collections.Generic;
using System.IO;
using iText.IO.Util;
using iText.Pdfocr;
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Orientation;
using iText.Pdfocr.Onnxtr.Recognition;
using iText.Test;

namespace iText.Pdfocr.Onnxtr {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxCreateTxtFileTest : ExtendedITextTest {
        private static readonly String BASE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        private static readonly String TEST_IMAGE_DIRECTORY = BASE_DIRECTORY + "images/";

        private static readonly String SOURCE_DIRECTORY = BASE_DIRECTORY + "OnnxCreateTxtFileTest/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxCreateTxtFileTest/";

        private static readonly String FAST = BASE_DIRECTORY + "models/rep_fast_tiny-28867779.onnx";

        private static readonly String CRNNVGG16 = BASE_DIRECTORY + "models/crnn_vgg16_bn-662979cc.onnx";

        private static readonly String MOBILENETV3 = BASE_DIRECTORY + "models/mobilenet_v3_small_crop_orientation-5620cf7e.onnx";

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
        public virtual void CreateTxtFileTest() {
            String cmpPath = SOURCE_DIRECTORY + "cmp_createTxtFile.txt";
            String outputPath = TARGET_DIRECTORY + "createTxtFile.txt";
            String[] sourceImages = new String[] { TEST_IMAGE_DIRECTORY + "englishText.bmp", TEST_IMAGE_DIRECTORY + "rotatedTextBasic.png"
                , TEST_IMAGE_DIRECTORY + "scanned_spa_01.png", SOURCE_DIRECTORY + "regularText.png", SOURCE_DIRECTORY 
                + "basicCloud.png", SOURCE_DIRECTORY + "differentSizes.png", SOURCE_DIRECTORY + "rotated.png" };
            IList<FileInfo> images = new List<FileInfo>(sourceImages.Length * 2);
            foreach (String sourceImage in sourceImages) {
                images.Add(new FileInfo(sourceImage));
            }
            OCR_ENGINE.CreateTxtFile(images, new FileInfo(outputPath), new OcrProcessContext(null));
            NUnit.Framework.Assert.IsNull(CompareTxt(cmpPath, outputPath));
        }

        private static String CompareTxt(String cmp, String dest) {
            String errorMessage = null;
            System.Console.Out.WriteLine("Out txt: " + UrlUtil.GetNormalizedFileUriString(dest));
            System.Console.Out.WriteLine("Cmp txt: " + UrlUtil.GetNormalizedFileUriString(cmp) + "\n");
            IList<String> result = File.ReadAllLines(System.IO.Path.Combine(dest));
            IList<String> expected = File.ReadAllLines(System.IO.Path.Combine(cmp));
            int lineNumber = 0;
            String destLine = ReadLine(result, lineNumber);
            String cmpLine = ReadLine(expected, lineNumber);
            while (destLine != null || cmpLine != null) {
                if (destLine == null || cmpLine == null) {
                    errorMessage = "The number of lines is different.\n";
                    break;
                }
                if (!destLine.Equals(cmpLine)) {
                    errorMessage = "Txt files differ at line " + (lineNumber + 1) + "\n See difference: cmp file: \"" + cmpLine
                         + "\"\n" + "target file: \"" + destLine + "\n";
                }
                lineNumber++;
                destLine = ReadLine(result, lineNumber);
                cmpLine = ReadLine(expected, lineNumber);
            }
            return errorMessage;
        }

        private static String ReadLine(IList<String> lines, int lineNumber) {
            if (lineNumber < lines.Count) {
                return lines[lineNumber];
            }
            return null;
        }
    }
}
