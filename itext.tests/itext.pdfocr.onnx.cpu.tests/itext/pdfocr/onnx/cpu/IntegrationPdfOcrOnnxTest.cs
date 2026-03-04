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
using System.Collections.Generic;
using System.IO;
using System.Text;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Onnx;
using iText.Pdfocr.Onnx.Detection;
using iText.Pdfocr.Onnx.Orientation;
using iText.Pdfocr.Onnx.Recognition;
using iText.Test;

namespace iText.Pdfocr.Onnx.Cpu {
    [NUnit.Framework.Category("IntegrationTest")]
    public class IntegrationPdfOcrOnnxTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        private static readonly String TEST_IMAGE_DIRECTORY = TEST_DIRECTORY + "images/";

        private static readonly String TEST_PDFS_DIRECTORY = TEST_DIRECTORY + "pdfs/";

        private static readonly String SOURCE_FOLDER = TEST_DIRECTORY + "IntegrationPdfOcrOnnxTest/";

        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/IntegrationPdfOcrOnnxTest/";

        private static readonly String FAST = TEST_DIRECTORY + "models/rep_fast_tiny-28867779.onnx";

        private static readonly String CRNNVGG16 = TEST_DIRECTORY + "models/crnn_vgg16_bn-662979cc.onnx";

        private static readonly String MOBILENETV3 = TEST_DIRECTORY + "models/mobilenet_v3_small_crop_orientation-5620cf7e.onnx";

        private static OnnxOcrEngine OCR_ENGINE_MAKE_PDF_SEARCHABLE;

        private static OnnxOcrEngine OCR_ENGINE_IMAGE_OCR;

        private static OnnxOcrEngine OCR_ENGINE_CREATE_PDF;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
            IDetectionPredictor detectionPredictor = OnnxDetectionPredictor.Fast(FAST);
            IOrientationPredictor orientationPredictor = OnnxOrientationPredictor.MobileNetV3(MOBILENETV3);
            IRecognitionPredictor recognitionPredictor = OnnxRecognitionPredictor.CrnnVgg16(CRNNVGG16);
            OCR_ENGINE_MAKE_PDF_SEARCHABLE = new OnnxOcrEngine(detectionPredictor, orientationPredictor, recognitionPredictor
                );
            OCR_ENGINE_IMAGE_OCR = new OnnxOcrEngine(detectionPredictor, null, recognitionPredictor, new OnnxEngineProperties
                ().SetTextPositioning(iText.Pdfocr.Onnx.Text.TextPositioning.BY_WORDS));
            OCR_ENGINE_CREATE_PDF = new OnnxOcrEngine(detectionPredictor, recognitionPredictor);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            OCR_ENGINE_MAKE_PDF_SEARCHABLE.Close();
            OCR_ENGINE_IMAGE_OCR.Close();
            OCR_ENGINE_CREATE_PDF.Close();
        }

        [NUnit.Framework.Test]
        public virtual void MakePdfSearchableBasicTest() {
            MakePdfSearchable("numbers");
        }

        [NUnit.Framework.Test]
        public virtual void MakePdfSearchablePageRotatedTest() {
            MakePdfSearchable("pageRotation");
        }

        [NUnit.Framework.Test]
        public virtual void ImageOcrBasicTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_01.BMP";
            FileInfo imageFile = new FileInfo(src);
            String textFromImage = GetTextFromImage(imageFile, OCR_ENGINE_IMAGE_OCR);
            NUnit.Framework.Assert.AreEqual("Ihis\n1S\na\ntest\nmessage\n-\nfor\nOCR\nScanner\nTest\n", textFromImage);
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfBasicTest() {
            String src = TEST_IMAGE_DIRECTORY + "example_04.png";
            String dest = DESTINATION_FOLDER + "basicTest.pdf";
            String cmp = SOURCE_FOLDER + "cmp_basicTest.pdf";
            DoOcrAndCreatePdf(src, dest);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, DESTINATION_FOLDER, "diff_"));
        }

        private static void DoOcrAndCreatePdf(String imagePath, String destPdfPath) {
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(OCR_ENGINE_CREATE_PDF);
            using (PdfWriter writer = new PdfWriter(destPdfPath)) {
                ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList(new FileInfo(imagePath)), writer).Close();
            }
        }

        private static String GetTextFromImage(FileInfo imageFile, IOcrEngine ocrEngine) {
            IDictionary<int, IList<TextInfo>> integerListMap = ocrEngine.DoImageOcr(imageFile);
            return GetStringFromListMap(integerListMap);
        }

        private static String GetStringFromListMap(IDictionary<int, IList<TextInfo>> listMap) {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<int, IList<TextInfo>> entry in listMap) {
                foreach (TextInfo textInfo in entry.Value) {
                    if (textInfo.GetText() != null) {
                        stringBuilder.Append(textInfo.GetText()).Append('\n');
                    }
                }
            }
            return stringBuilder.ToString();
        }

        private void MakePdfSearchable(String fileName) {
            String srcPath = TEST_PDFS_DIRECTORY + fileName + ".pdf";
            String outPath = DESTINATION_FOLDER + fileName + ".pdf";
            String cmpPath = SOURCE_FOLDER + "cmp_" + fileName + ".pdf";
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(OCR_ENGINE_MAKE_PDF_SEARCHABLE, new OcrPdfCreatorProperties
                ().SetTextColor(DeviceCmyk.MAGENTA));
            ocrPdfCreator.MakePdfSearchable(new FileInfo(srcPath), new FileInfo(outPath));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, DESTINATION_FOLDER, "diff_"
                ));
        }
    }
}
