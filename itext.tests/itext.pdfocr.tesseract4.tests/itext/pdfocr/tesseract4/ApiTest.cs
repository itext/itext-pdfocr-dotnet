/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2020 iText Group NV
    Authors: iText Software.

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
using iText.Pdfocr;
using iText.Test.Attributes;

namespace iText.Pdfocr.Tesseract4 {
    public class ApiTest : AbstractIntegrationTest {
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

        [LogMessage(Tesseract4LogMessageConstant.PAGE_NUMBER_IS_INCORRECT)]
        [NUnit.Framework.Test]
        public virtual void TestReadingSecondPageFromOnePageTiff() {
            String path = TEST_IMAGES_DIRECTORY + "example_03_10MB.tiff";
            FileInfo imgFile = new FileInfo(path);
            Pix page = TesseractOcrUtil.ReadPixPageFromTiff(imgFile, 2);
            NUnit.Framework.Assert.IsNull(page);
        }

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

        [LogMessage(Tesseract4OcrException.PATH_TO_TESS_DATA_IS_NOT_SET)]
        [NUnit.Framework.Test]
        public virtual void TestDefaultTessDataPathValidationForLib() {
            NUnit.Framework.Assert.That(() =>  {
                String path = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
                FileInfo imgFile = new FileInfo(path);
                Tesseract4LibOcrEngine engine = new Tesseract4LibOcrEngine(new Tesseract4OcrEngineProperties());
                engine.DoImageOcr(imgFile);
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(Tesseract4OcrException.PATH_TO_TESS_DATA_IS_NOT_SET))
;
        }

        [LogMessage(Tesseract4OcrException.PATH_TO_TESS_DATA_IS_NOT_SET)]
        [NUnit.Framework.Test]
        public virtual void TestDefaultTessDataPathValidationForExecutable() {
            NUnit.Framework.Assert.That(() =>  {
                String path = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
                FileInfo imgFile = new FileInfo(path);
                Tesseract4ExecutableOcrEngine engine = new Tesseract4ExecutableOcrEngine(GetTesseractDirectory(), new Tesseract4OcrEngineProperties
                    ());
                engine.DoImageOcr(imgFile);
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(Tesseract4OcrException.PATH_TO_TESS_DATA_IS_NOT_SET))
;
        }
    }
}
