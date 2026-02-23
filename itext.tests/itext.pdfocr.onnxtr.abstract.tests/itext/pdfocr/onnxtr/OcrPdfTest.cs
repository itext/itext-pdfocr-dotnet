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
using iText.Kernel.Colors;
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Orientation;
using iText.Pdfocr.Onnxtr.Recognition;
using iText.Test;

namespace iText.Pdfocr.Onnxtr {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OcrPdfTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/OcrPdfTest/";

        private static readonly String TEST_PDFS_DIRECTORY = TEST_DIRECTORY + "../pdfs/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OcrPdfTest/";

        private static readonly String FAST = TEST_DIRECTORY + "../models/rep_fast_tiny-28867779.onnx";

        private static readonly String CRNNVGG16 = TEST_DIRECTORY + "../models/crnn_vgg16_bn-662979cc.onnx";

        private static readonly String MOBILENETV3 = TEST_DIRECTORY + "../models/mobilenet_v3_small_crop_orientation-5620cf7e.onnx";

        private static OnnxTrOcrEngine OCR_ENGINE;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
            IDetectionPredictor detectionPredictor = OnnxDetectionPredictor.Fast(FAST);
            IOrientationPredictor orientationPredictor = OnnxOrientationPredictor.MobileNetV3(MOBILENETV3);
            IRecognitionPredictor recognitionPredictor = OnnxRecognitionPredictor.CrnnVgg16(CRNNVGG16);
            OCR_ENGINE = new OnnxTrOcrEngine(detectionPredictor, orientationPredictor, recognitionPredictor);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            OCR_ENGINE.Close();
        }

        [NUnit.Framework.Test]
        public virtual void BasicTest() {
            MakeSearchable("numbers");
        }

        [NUnit.Framework.Test]
        public virtual void PageRotationTest() {
            MakeSearchable("pageRotation");
        }

        [NUnit.Framework.Test]
        public virtual void TwoImagesTest() {
            MakeSearchable("2images");
        }

        [NUnit.Framework.Test]
        public virtual void TwoPagesTest() {
            MakeSearchable("2pages");
        }

        [NUnit.Framework.Test]
        public virtual void RotatedTest() {
            MakeSearchable("rotated");
        }

        [NUnit.Framework.Test]
        public virtual void MixedRotationTest() {
            MakeSearchable("mixedRotation");
        }

        [NUnit.Framework.Test]
        public virtual void NotRecognizableTest() {
            // OnnxTr engine could recognize
            MakeSearchable("notRecognizable");
        }

        [NUnit.Framework.Test]
        public virtual void ImageIntersectionTest() {
            MakeSearchable("imageIntersection");
        }

        [NUnit.Framework.Test]
        public virtual void WhiteTextTest() {
            // OCRed by onnxtr. Almost good, w is OCRed as rotated m.
            // If you don't use orientation predictor, the result becomes very good.
            MakeSearchable("whiteText");
        }

        [NUnit.Framework.Test]
        public virtual void ChangedImageProportionTest() {
            MakeSearchable("changedImageProportion");
        }

        [NUnit.Framework.Test]
        public virtual void TextWithImagesTest() {
            MakeSearchable("textWithImages");
        }

        [NUnit.Framework.Test]
        public virtual void InvisibleTextImageTest() {
            MakeSearchable("invisibleTextImage");
        }

        [NUnit.Framework.Test]
        public virtual void LayersTest() {
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties().SetTextColor(DeviceCmyk.MAGENTA
                ).SetTextLayerName("Text");
            MakeSearchable("2pages", "layers", ocrPdfCreatorProperties);
        }

        [NUnit.Framework.Test]
        public virtual void SkewedRotated45Test() {
            MakeSearchable("skewedRotated45");
        }

        private void MakeSearchable(String fileName) {
            MakeSearchable(fileName, fileName, null);
        }

        private void MakeSearchable(String fileName, String outFileName, OcrPdfCreatorProperties ocrPdfCreatorProperties
            ) {
            String srcPath = TEST_PDFS_DIRECTORY + fileName + ".pdf";
            String outPath = TARGET_DIRECTORY + outFileName + ".pdf";
            String cmpPath = TEST_DIRECTORY + "cmp_" + outFileName + ".pdf";
            if (ocrPdfCreatorProperties == null) {
                ocrPdfCreatorProperties = new OcrPdfCreatorProperties().SetTextColor(DeviceCmyk.MAGENTA);
            }
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(OCR_ENGINE, ocrPdfCreatorProperties);
            ocrPdfCreator.MakePdfSearchable(new FileInfo(srcPath), new FileInfo(outPath));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outPath, cmpPath, TARGET_DIRECTORY, "diff_"
                ));
        }
    }
}
