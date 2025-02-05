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
using Tesseract;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4.Exceptions;
using iText.Pdfocr.Tesseract4.Logs;
using iText.Test.Attributes;

namespace iText.Pdfocr.Tesseract4 {
    public class ImagePreprocessingUtilTest : IntegrationTestHelper {
        [NUnit.Framework.Test]
        public virtual void TestCheckForInvalidTiff() {
            String path = TEST_IMAGES_DIRECTORY + "example_04.png";
            FileInfo imgFile = new FileInfo(path);
            NUnit.Framework.Assert.IsFalse(ImagePreprocessingUtil.IsTiffImage(imgFile));
        }

        [LogMessage(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE)]
        [NUnit.Framework.Test]
        public virtual void TestReadingInvalidImagePath() {
            String path = TEST_IMAGES_DIRECTORY + "numbers_02";
            FileInfo imgFile = new FileInfo(path);
            NUnit.Framework.Assert.Catch(typeof(PdfOcrTesseract4Exception), () => ImagePreprocessingUtil.PreprocessImage
                (imgFile, 1, new ImagePreprocessingOptions()));
        }

        [NUnit.Framework.Test]
        public virtual void TestImagePreprocessingOptions() {
            String sourceImg = TEST_IMAGES_DIRECTORY + "thai_02.jpg";
            String processedImg = GetTargetDirectory() + "thai_02_processed.jpg";
            String compareImg = TEST_IMAGES_DIRECTORY + "thai_02_cmp_01.jpg";
            Pix pix = ImagePreprocessingUtil.PreprocessImage(new FileInfo(sourceImg), 1, new ImagePreprocessingOptions
                ());
            TesseractOcrUtil.SavePixToPngFile(processedImg, pix);
            TesseractOcrUtil.DestroyPix(pix);
            CompareImagesWithPrecision(compareImg, processedImg, 0.1);
            compareImg = TEST_IMAGES_DIRECTORY + "thai_02_cmp_02.jpg";
            pix = ImagePreprocessingUtil.PreprocessImage(new FileInfo(sourceImg), 1, new ImagePreprocessingOptions().SetTileWidth
                (300).SetTileHeight(300));
            TesseractOcrUtil.SavePixToPngFile(processedImg, pix);
            TesseractOcrUtil.DestroyPix(pix);
            CompareImagesWithPrecision(compareImg, processedImg, 0.1);
            compareImg = TEST_IMAGES_DIRECTORY + "thai_02_cmp_03.jpg";
            pix = ImagePreprocessingUtil.PreprocessImage(new FileInfo(sourceImg), 1, new ImagePreprocessingOptions().SetTileWidth
                (300).SetTileHeight(300).SetSmoothTiling(false));
            TesseractOcrUtil.SavePixToPngFile(processedImg, pix);
            TesseractOcrUtil.DestroyPix(pix);
            CompareImagesWithPrecision(compareImg, processedImg, 0.1);
        }

        private static void CompareImagesWithPrecision(String img1, String img2, double precisionPercents) {
            ImageData imageData1 = ImageDataFactory.Create(img1);
            ImageData imageData2 = ImageDataFactory.Create(img2);
            NUnit.Framework.Assert.AreEqual(0, JavaUtil.FloatCompare(imageData1.GetWidth(), imageData2.GetWidth()));
            NUnit.Framework.Assert.AreEqual(0, JavaUtil.FloatCompare(imageData1.GetHeight(), imageData2.GetHeight()));
            System.Drawing.Bitmap image1 = ImagePreprocessingUtil.ReadImageFromFile(new FileInfo(img1));
            System.Drawing.Bitmap image2 = ImagePreprocessingUtil.ReadImageFromFile(new FileInfo(img2));
            int inconsistentPixelsCount = 0;
            for (int x = 0; x < imageData1.GetWidth(); x++) {
                for (int y = 0; y < imageData1.GetHeight(); y++) {
                    if (TesseractOcrUtil.GetImagePixelColor(image1, x, y) != TesseractOcrUtil.GetImagePixelColor(image2, x, y)
                        ) {
                        inconsistentPixelsCount++;
                    }
                }
            }
            float differencePercentage = (float)(100 * inconsistentPixelsCount) / (imageData1.GetWidth() * imageData1.
                GetHeight());
            NUnit.Framework.Assert.IsTrue(differencePercentage < precisionPercents);
        }
    }
}
