using System;
using System.IO;
using iText.Pdfocr;

namespace iText.Pdfocr.Tesseract4 {
    public class ImagePreprocessingUtilTest : IntegrationTestHelper {
        [NUnit.Framework.Test]
        public virtual void TestCheckForInvalidTiff() {
            String path = TEST_IMAGES_DIRECTORY + "example_03_10MB";
            FileInfo imgFile = new FileInfo(path);
            NUnit.Framework.Assert.IsFalse(ImagePreprocessingUtil.IsTiffImage(imgFile));
        }

        [NUnit.Framework.Test]
        public virtual void TestReadingInvalidImagePath() {
            NUnit.Framework.Assert.That(() =>  {
                String path = TEST_IMAGES_DIRECTORY + "numbers_02";
                FileInfo imgFile = new FileInfo(path);
                ImagePreprocessingUtil.PreprocessImage(imgFile, 1);
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>())
;
        }
    }
}
