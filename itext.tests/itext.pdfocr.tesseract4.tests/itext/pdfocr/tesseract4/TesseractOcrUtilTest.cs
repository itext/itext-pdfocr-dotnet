/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using Tesseract;
using iText.IO.Image;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4.Logs;
using iText.Test.Attributes;

namespace iText.Pdfocr.Tesseract4 {
    public class TesseractOcrUtilTest : IntegrationTestHelper {
        [NUnit.Framework.Test]
        public virtual void TestTesseract4OcrForPix() {
            String path = TEST_IMAGES_DIRECTORY + "numbers_02.jpg";
            String expected = "0123456789";
            FileInfo imgFile = new FileInfo(path);
            Pix pix = TesseractOcrUtil.ReadPix(imgFile);
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
            NUnit.Framework.Assert.IsNull(TesseractOcrUtil.OtsuImageThresholding(pix, new ImagePreprocessingOptions())
                );
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
            TesseractOcrUtil.SavePixToPngFile(tmpFileName, null);
            NUnit.Framework.Assert.IsFalse(File.Exists(System.IO.Path.Combine(tmpFileName)));
        }

        [NUnit.Framework.Test]
        public virtual void TestPixSavingAsPng() {
            String path = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            String tmpFileName = GetTargetDirectory() + "testPixSavingAsPng.png";
            NUnit.Framework.Assert.IsFalse(File.Exists(System.IO.Path.Combine(tmpFileName)));
            Pix pix = TesseractOcrUtil.ReadPix(new FileInfo(path));
            TesseractOcrUtil.SavePixToPngFile(tmpFileName, pix);
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

        [NUnit.Framework.Test]
        public virtual void TestDetectImageRotationAndFix() {
            String path = TEST_IMAGES_DIRECTORY + "90_degrees_rotated.jpg";
            TesseractOcrUtil.DetectRotation(new FileInfo(path));
            ImageData imageData = ImageDataFactory.Create(path);
            int rotation = TesseractOcrUtil.DetectRotation(imageData);
            NUnit.Framework.Assert.AreEqual(90, rotation);
            imageData = TesseractOcrUtil.ApplyRotation(imageData);
            rotation = TesseractOcrUtil.DetectRotation(imageData);
            NUnit.Framework.Assert.AreEqual(0, rotation);
            path = TEST_IMAGES_DIRECTORY + "180_degrees_rotated.jpg";
            TesseractOcrUtil.DetectRotation(new FileInfo(path));
            imageData = ImageDataFactory.Create(path);
            rotation = TesseractOcrUtil.DetectRotation(imageData);
            NUnit.Framework.Assert.AreEqual(180, rotation);
            imageData = TesseractOcrUtil.ApplyRotation(imageData);
            rotation = TesseractOcrUtil.DetectRotation(imageData);
            NUnit.Framework.Assert.AreEqual(0, rotation);
            path = TEST_IMAGES_DIRECTORY + "270_degrees_rotated.jpg";
            TesseractOcrUtil.DetectRotation(new FileInfo(path));
            imageData = ImageDataFactory.Create(path);
            rotation = TesseractOcrUtil.DetectRotation(imageData);
            NUnit.Framework.Assert.AreEqual(270, rotation);
            imageData = TesseractOcrUtil.ApplyRotation(imageData);
            rotation = TesseractOcrUtil.DetectRotation(imageData);
            NUnit.Framework.Assert.AreEqual(0, rotation);
        }

        [LogMessage(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE, Ignore = true)]
        [NUnit.Framework.Test]
        public virtual void TestDetectImageRotationNegativeCases() {
            String path = TEST_IMAGES_DIRECTORY + "90_degrees_rotated.jpg_broken_path";
            int rotation = TesseractOcrUtil.DetectRotation(new FileInfo(path));
            NUnit.Framework.Assert.AreEqual(0, rotation);
            byte[] data = "broken image".GetBytes();
            rotation = TesseractOcrUtil.DetectRotation(data);
            NUnit.Framework.Assert.AreEqual(0, rotation);
        }
    }
}
