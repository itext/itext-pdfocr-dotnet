using System;
using System.IO;
using iText.Commons.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Orientation;
using iText.Pdfocr.Onnxtr.Recognition;
using iText.Test;
using NUnit.Framework;

namespace iText.Pdfocr.Onnxtr {
    public class OnnxCreateTxtFileTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        private static readonly String TEST_IMAGE_DIRECTORY = TEST_DIRECTORY + "images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxCreateTxtFileTest";

        private static readonly String FAST = TEST_DIRECTORY + "models/rep_fast_tiny-28867779.onnx";

        private static readonly String CRNNVGG16 = TEST_DIRECTORY + "models/crnn_vgg16_bn-662979cc.onnx";

        private static readonly String MOBILENETV3 = TEST_DIRECTORY + "models/mobilenet_v3_small_crop_orientation-5620cf7e.onnx";

        private static OnnxTrOcrEngine OCR_ENGINE;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
            IDetectionPredictor detectionPredictor = OnnxDetectionPredictor.Fast(FAST);
            IRecognitionPredictor recognitionPredictor = OnnxRecognitionPredictor.CrnnVgg16(CRNNVGG16);
            IOrientationPredictor orientationPredictor = OnnxOrientationPredictor.MobileNetV3(MOBILENETV3);
            OCR_ENGINE = new OnnxTrOcrEngine(detectionPredictor, orientationPredictor, recognitionPredictor);
        }

        [Test]
        public virtual void CreateTxtFileNullEventHelperTest() {
            //TODO DEVSIX-9153: Add tests for ocrEngine.createTxtFile();
            String src = TEST_DIRECTORY + "images/numbers_01.jpg";
            FileInfo imgFile = new FileInfo(src);
            String src1 = TEST_DIRECTORY + "images/270_degrees_rotated.jpg";
            FileInfo imgFile1 = new FileInfo(src1);
            String src2 = TEST_DIRECTORY + "images/example_04.png";
            FileInfo imgFile2 = new FileInfo(src2);
            
            Exception e = Assert.Catch(typeof(NotSupportedException), () => OCR_ENGINE.CreateTxtFile(JavaUtil.ArraysAsList(imgFile, imgFile1, imgFile2), FileUtil.CreateTempFile("test"
                , ".txt"), new OcrProcessContext(null)));
            Assert.AreEqual("Specified method is not supported.", e.Message);
        }
    }
}
