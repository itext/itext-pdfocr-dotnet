using System;
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Orientation;
using iText.Pdfocr.Onnxtr.Recognition;
using iText.Test;

namespace iText.Pdfocr.Onnxtr {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxTRCmykIntegrationTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/OnnxTRCmykIntegrationTest/";

        private static readonly String TEST_IMAGE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxTRCmykIntegrationTest/";

        private static readonly String FAST = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/rep_fast_tiny-28867779.onnx";

        private static readonly String CRNNVGG16 = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/crnn_vgg16_bn-662979cc.onnx";

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
        public virtual void RainbowInvertedCmykTest() {
            String src = TEST_IMAGE_DIRECTORY + "rainbow_inverted_cmyk.jpg";
            String dest = TARGET_DIRECTORY + "rainbowInvertedCmykTest.pdf";
            String cmpTxt = TEST_DIRECTORY + "cmp_rainbowInvertedCmykTest.txt";
            DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual(GetCmpText(cmpTxt), extractionStrategy.GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void RainbowAdobeCmykTest() {
            String src = TEST_IMAGE_DIRECTORY + "rainbow_adobe_cmyk.jpg";
            String dest = TARGET_DIRECTORY + "rainbowAdobeCmykTest.pdf";
            String cmpTxt = TEST_DIRECTORY + "cmp_rainbowAdobeCmykTest.txt";
            DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual(GetCmpText(cmpTxt), extractionStrategy.GetResultantText());
            }
        }

        [NUnit.Framework.Test]
        public virtual void RainbowCmykNoProfileTest() {
            String src = TEST_IMAGE_DIRECTORY + "rainbow_cmyk_inverted_no_profile.jpg";
            String dest = TARGET_DIRECTORY + "rainbowCmykNoProfileTest.pdf";
            String cmpTxt = TEST_DIRECTORY + "cmp_rainbowCmykNoProfileTest.txt";
            DoOcrAndCreatePdf(src, dest, CreatorProperties("Text1", DeviceCmyk.MAGENTA));
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(dest))) {
                ExtractionStrategy extractionStrategy = OnnxTestUtils.ExtractTextFromLayer(pdfDocument, 1, "Text1");
                NUnit.Framework.Assert.AreEqual(DeviceCmyk.MAGENTA, extractionStrategy.GetFillColor());
                NUnit.Framework.Assert.AreEqual(GetCmpText(cmpTxt), extractionStrategy.GetResultantText());
            }
        }
        
        private OcrPdfCreatorProperties CreatorProperties(String layerName, Color color) {
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
            ocrPdfCreatorProperties.SetTextLayerName(layerName);
            ocrPdfCreatorProperties.SetTextColor(color);
            return ocrPdfCreatorProperties;
        }

        private void DoOcrAndCreatePdf(String imagePath, String destPdfPath, OcrPdfCreatorProperties ocrPdfCreatorProperties
            ) {
            OcrPdfCreator ocrPdfCreator = ocrPdfCreatorProperties != null ? new OcrPdfCreator(OCR_ENGINE, ocrPdfCreatorProperties
                ) : new OcrPdfCreator(OCR_ENGINE);
            using (PdfWriter writer = new PdfWriter(destPdfPath)) {
                ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList(new FileInfo(imagePath)), writer).Close();
            }
        }

        private String GetCmpText(String txtPath) {
            int bytesCount = (int)new FileInfo(txtPath).Length;
            char[] array = new char[bytesCount];
            using (StreamReader stream = new StreamReader(iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine
                (txtPath)))) {
                stream.Read(array, 0, bytesCount);
                return new String(array);
            }
        }
    }
}
