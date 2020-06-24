using System;
using System.IO;
using Tesseract;
using iText.Pdfocr;
using iText.Test.Attributes;

namespace iText.Pdfocr.Tesseract4 {
    public class TesseractOcrUtilTest : IntegrationTestHelper {
        [NUnit.Framework.Test]
        public virtual void TestTesseract4OcrForPix() {
            String path = TEST_IMAGES_DIRECTORY + "numbers_02.jpg";
            String expected = "0123456789";
            FileInfo imgFile = new FileInfo(path);
            Pix pix = ImagePreprocessingUtil.ReadPix(imgFile);
            Tesseract4LibOcrEngine tesseract4LibOcrEngine = GetTesseract4LibOcrEngine();
            tesseract4LibOcrEngine.SetTesseract4OcrEngineProperties(new Tesseract4OcrEngineProperties().SetPathToTessData
                (GetTessDataDirectory()));
            tesseract4LibOcrEngine.InitializeTesseract(OutputFormat.TXT);
            String result = new TesseractOcrUtil().GetOcrResultAsString(tesseract4LibOcrEngine.GetTesseractInstance(), 
                pix, OutputFormat.TXT);
            NUnit.Framework.Assert.IsTrue(result.Contains(expected));
        }

        [NUnit.Framework.Test]
        public virtual void TestGetOcrResultAsStringForFile() {
            String path = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            String expected = "619121";
            FileInfo imgFile = new FileInfo(path);
            Tesseract4LibOcrEngine tesseract4LibOcrEngine = GetTesseract4LibOcrEngine();
            tesseract4LibOcrEngine.SetTesseract4OcrEngineProperties(new Tesseract4OcrEngineProperties().SetPathToTessData
                (GetTessDataDirectory()));
            tesseract4LibOcrEngine.InitializeTesseract(OutputFormat.TXT);
            String result = new TesseractOcrUtil().GetOcrResultAsString(tesseract4LibOcrEngine.GetTesseractInstance(), 
                imgFile, OutputFormat.TXT);
            NUnit.Framework.Assert.IsTrue(result.Contains(expected));
        }

        [LogMessage(Tesseract4LogMessageConstant.PAGE_NUMBER_IS_INCORRECT)]
        [NUnit.Framework.Test]
        public virtual void TestReadingSecondPageFromOnePageTiff() {
            String path = TEST_IMAGES_DIRECTORY + "example_03_10MB.tiff";
            FileInfo imgFile = new FileInfo(path);
            Pix page = TesseractOcrUtil.ReadPixPageFromTiff(imgFile, 2);
            NUnit.Framework.Assert.IsNull(page);
        }

        [LogMessage(Tesseract4LogMessageConstant.CANNOT_RETRIEVE_PAGES_FROM_IMAGE)]
        [NUnit.Framework.Test]
        public virtual void TestReadingPageFromInvalidTiff() {
            String path = TEST_IMAGES_DIRECTORY + "example_03.tiff";
            FileInfo imgFile = new FileInfo(path);
            Pix page = TesseractOcrUtil.ReadPixPageFromTiff(imgFile, 0);
            NUnit.Framework.Assert.IsNull(page);
        }

        [LogMessage(Tesseract4LogMessageConstant.CANNOT_RETRIEVE_PAGES_FROM_IMAGE)]
        [NUnit.Framework.Test]
        public virtual void TestInitializeImagesListFromInvalidTiff() {
            String path = TEST_IMAGES_DIRECTORY + "example_03.tiff";
            FileInfo imgFile = new FileInfo(path);
            TesseractOcrUtil tesseractOcrUtil = new TesseractOcrUtil();
            tesseractOcrUtil.InitializeImagesListFromTiff(imgFile);
            NUnit.Framework.Assert.AreEqual(0, tesseractOcrUtil.GetListOfPages().Count);
        }

        [NUnit.Framework.Test]
        public virtual void TestPreprocessingConditions() {
            Pix pix = null;
            NUnit.Framework.Assert.IsNull(TesseractOcrUtil.ConvertToGrayscale(pix));
            NUnit.Framework.Assert.IsNull(TesseractOcrUtil.OtsuImageThresholding(pix));
            NUnit.Framework.Assert.IsNull(TesseractOcrUtil.ConvertPixToImage(pix));
            TesseractOcrUtil.DestroyPix(pix);
        }

        [NUnit.Framework.Test]
        public virtual void TestOcrResultConditions() {
            Tesseract4LibOcrEngine tesseract4LibOcrEngine = GetTesseract4LibOcrEngine();
            tesseract4LibOcrEngine.SetTesseract4OcrEngineProperties(new Tesseract4OcrEngineProperties().SetPathToTessData
                (GetTessDataDirectory()));
            tesseract4LibOcrEngine.InitializeTesseract(OutputFormat.HOCR);
            Pix pix = null;
            NUnit.Framework.Assert.IsNull(new TesseractOcrUtil().GetOcrResultAsString(tesseract4LibOcrEngine.GetTesseractInstance
                (), pix, OutputFormat.HOCR));
            FileInfo file = null;
            NUnit.Framework.Assert.IsNull(new TesseractOcrUtil().GetOcrResultAsString(tesseract4LibOcrEngine.GetTesseractInstance
                (), file, OutputFormat.HOCR));
            System.Drawing.Bitmap bi = null;
            NUnit.Framework.Assert.IsNull(new TesseractOcrUtil().GetOcrResultAsString(tesseract4LibOcrEngine.GetTesseractInstance
                (), bi, OutputFormat.HOCR));
        }

        [NUnit.Framework.Test]
        public virtual void TestImageSavingAsPng() {
            String path = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            String tmpFileName = GetTargetDirectory() + "testImageSavingAsPng.png";
            NUnit.Framework.Assert.IsFalse(File.Exists(System.IO.Path.Combine(tmpFileName)));
            System.Drawing.Bitmap bi = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(new FileStream(path, FileMode.Open
                , FileAccess.Read));
            TesseractOcrUtil.SaveImageToTempPngFile(tmpFileName, bi);
            NUnit.Framework.Assert.IsTrue(File.Exists(System.IO.Path.Combine(tmpFileName)));
            TesseractHelper.DeleteFile(tmpFileName);
            NUnit.Framework.Assert.IsFalse(File.Exists(System.IO.Path.Combine(tmpFileName)));
        }

        [NUnit.Framework.Test]
        public virtual void TestNullSavingAsPng() {
            String tmpFileName = TesseractOcrUtil.GetTempFilePath(GetTargetDirectory() + "/testNullSavingAsPng", ".png"
                );
            TesseractOcrUtil.SaveImageToTempPngFile(tmpFileName, null);
            NUnit.Framework.Assert.IsFalse(File.Exists(System.IO.Path.Combine(tmpFileName)));
            TesseractOcrUtil.SavePixToTempPngFile(tmpFileName, null);
            NUnit.Framework.Assert.IsFalse(File.Exists(System.IO.Path.Combine(tmpFileName)));
        }

        [NUnit.Framework.Test]
        public virtual void TestPixSavingAsPng() {
            String path = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            String tmpFileName = GetTargetDirectory() + "testPixSavingAsPng.png";
            NUnit.Framework.Assert.IsFalse(File.Exists(System.IO.Path.Combine(tmpFileName)));
            Pix pix = ImagePreprocessingUtil.ReadPix(new FileInfo(path));
            TesseractOcrUtil.SavePixToTempPngFile(tmpFileName, pix);
            NUnit.Framework.Assert.IsTrue(File.Exists(System.IO.Path.Combine(tmpFileName)));
            TesseractHelper.DeleteFile(tmpFileName);
            NUnit.Framework.Assert.IsFalse(File.Exists(System.IO.Path.Combine(tmpFileName)));
        }

        [LogMessage(Tesseract4LogMessageConstant.CANNOT_PROCESS_IMAGE)]
        [NUnit.Framework.Test]
        public virtual void TestImageSavingAsPngWithError() {
            String path = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            System.Drawing.Bitmap bi = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(new FileStream(path, FileMode.Open
                , FileAccess.Read));
            TesseractOcrUtil.SaveImageToTempPngFile(null, bi);
        }
    }
}
