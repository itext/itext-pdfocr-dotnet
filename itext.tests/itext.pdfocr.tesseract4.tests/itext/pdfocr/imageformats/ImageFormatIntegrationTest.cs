/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4;
using iText.Pdfocr.Tesseract4.Exceptions;
using iText.Pdfocr.Tesseract4.Logs;
using iText.Test.Attributes;

namespace iText.Pdfocr.Imageformats {
    public abstract class ImageFormatIntegrationTest : IntegrationTestHelper {
//\cond DO_NOT_DOCUMENT
        internal AbstractTesseract4OcrEngine tesseractReader;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal String testType;
//\endcond

        public ImageFormatIntegrationTest(IntegrationTestHelper.ReaderType type) {
            tesseractReader = GetTesseractReader(type);
            this.testType = type.ToString().ToLowerInvariant();
        }

        [NUnit.Framework.SetUp]
        public virtual void InitTesseractProperties() {
            Tesseract4OcrEngineProperties ocrEngineProperties = new Tesseract4OcrEngineProperties();
            ocrEngineProperties.SetPathToTessData(GetTessDataDirectory());
            tesseractReader.SetTesseract4OcrEngineProperties(ocrEngineProperties);
        }

        [NUnit.Framework.Test]
        public virtual void CompareBmp() {
            String testName = "compareBmp";
            String fileName = "example_01";
            String path = TEST_IMAGES_DIRECTORY + fileName + ".BMP";
            String expectedPdfPath = TEST_DOCUMENTS_DIRECTORY + fileName + "_" + testType + ".pdf";
            String resultPdfPath = GetTargetDirectory() + fileName + "_" + testName + "_" + testType + ".pdf";
            DoOcrAndSavePdfToPath(tesseractReader, path, resultPdfPath, JavaCollectionsUtil.SingletonList<String>("eng"
                ), DeviceCmyk.MAGENTA);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, GetTargetDirectory
                (), "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestBMPText() {
            String path = TEST_IMAGES_DIRECTORY + "example_01.BMP";
            String expectedOutput = "This is a test message for OCR Scanner Test";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path), JavaCollectionsUtil.SingletonList
                <String>("eng"));
            realOutputHocr = iText.Commons.Utils.StringUtil.ReplaceAll(realOutputHocr, "[\n]", " ");
            realOutputHocr = iText.Commons.Utils.StringUtil.ReplaceAll(realOutputHocr, "[‘]", "");
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains((expectedOutput)));
        }

        [NUnit.Framework.Test]
        public virtual void CompareBmp02() {
            String testName = "compareBmp02";
            String fileName = "englishText";
            String path = TEST_IMAGES_DIRECTORY + fileName + ".bmp";
            String expectedPdfPath = TEST_DOCUMENTS_DIRECTORY + fileName + "_" + testType + ".pdf";
            String resultPdfPath = GetTargetDirectory() + fileName + "_" + testName + "_" + testType + ".pdf";
            DoOcrAndSavePdfToPath(tesseractReader, path, resultPdfPath, JavaCollectionsUtil.SingletonList<String>("eng"
                ), DeviceCmyk.MAGENTA);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, GetTargetDirectory
                (), "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestBMPText02() {
            String path = TEST_IMAGES_DIRECTORY + "englishText.bmp";
            String expectedOutput = "This is a test message for OCR Scanner Test BMPTest";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path), JavaCollectionsUtil.SingletonList
                <String>("eng"));
            realOutputHocr = iText.Commons.Utils.StringUtil.ReplaceAll(realOutputHocr, "[\n]", " ");
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains((expectedOutput)));
        }

        [NUnit.Framework.Test]
        public virtual void CompareJFIF() {
            String testName = "compareJFIF";
            String filename = "example_02";
            String expectedPdfPath = TEST_DOCUMENTS_DIRECTORY + filename + ".pdf";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + ".pdf";
            DoOcrAndSavePdfToPath(tesseractReader, TEST_IMAGES_DIRECTORY + filename + ".JFIF", resultPdfPath, null, DeviceCmyk
                .MAGENTA);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, GetTargetDirectory
                (), "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void CompareJpg() {
            String testName = "compareJpg";
            String fileName = "numbers_02";
            String path = TEST_IMAGES_DIRECTORY + fileName + ".jpg";
            String pdfName = fileName + "_" + testName + "_" + testType + ".pdf";
            String expectedPdfPath = TEST_DOCUMENTS_DIRECTORY + pdfName;
            String resultPdfPath = GetTargetDirectory() + pdfName;
            DoOcrAndSavePdfToPath(tesseractReader, path, resultPdfPath, null, DeviceCmyk.BLACK);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, GetTargetDirectory
                (), "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromJPG() {
            String path = TEST_IMAGES_DIRECTORY + "numbers_02.jpg";
            String expectedOutput = "0123456789";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages
                (false));
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void CompareJpe() {
            String testName = "compareJpe";
            String fileName = "numbers_01";
            String path = TEST_IMAGES_DIRECTORY + fileName + ".jpe";
            String pdfName = fileName + "_" + testName + "_" + testType + ".pdf";
            String expectedPdfPath = TEST_DOCUMENTS_DIRECTORY + pdfName;
            String resultPdfPath = GetTargetDirectory() + pdfName;
            DoOcrAndSavePdfToPath(tesseractReader, path, resultPdfPath, null, DeviceCmyk.BLACK);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, GetTargetDirectory
                (), "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromJPE() {
            String path = TEST_IMAGES_DIRECTORY + "numbers_01.jpe";
            String expectedOutput = "619121";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void CompareTif() {
            String testName = "compareTif";
            String fileName = "numbers_01";
            String path = TEST_IMAGES_DIRECTORY + fileName + ".tif";
            String pdfName = fileName + "_" + testName + "_" + testType + ".pdf";
            String expectedPdfPath = TEST_DOCUMENTS_DIRECTORY + pdfName;
            String resultPdfPath = GetTargetDirectory() + pdfName;
            DoOcrAndSavePdfToPath(tesseractReader, path, resultPdfPath, null, DeviceCmyk.BLACK);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, GetTargetDirectory
                (), "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromTIF() {
            String path = TEST_IMAGES_DIRECTORY + "numbers_01.tif";
            String expectedOutput = "619121";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void TestBigTiffWithoutPreprocessing() {
            String path = TEST_IMAGES_DIRECTORY + "example_03_10MB.tiff";
            String expectedOutput = "Image File Format";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages
                (false).SetPageSegMode(null));
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path), JavaCollectionsUtil.SingletonList
                <String>("eng"));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void CompareMultipagesTIFFWithPreprocessing() {
            String testName = "compareMultipagesTIFFWithPreprocessing";
            String fileName = "multipage";
            String path = TEST_IMAGES_DIRECTORY + fileName + ".tiff";
            String expectedPdfPath = TEST_DOCUMENTS_DIRECTORY + fileName + "_" + testType + ".pdf";
            String resultPdfPath = GetTargetDirectory() + fileName + "_" + testName + "_" + testType + ".pdf";
            DoOcrAndSavePdfToPath(tesseractReader, path, resultPdfPath, JavaCollectionsUtil.SingletonList<String>("eng"
                ), DeviceCmyk.BLACK);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, GetTargetDirectory
                (), "diff_"));
        }

        [NUnit.Framework.Test]
        public virtual void TestInputMultipagesTIFFWithPreprocessing() {
            String path = TEST_IMAGES_DIRECTORY + "multîpage.tiff";
            String expectedOutput = "Multipage\nTIFF\nExample\nPage 5";
            FileInfo file = new FileInfo(path);
            String realOutputHocr = GetTextFromPdf(tesseractReader, file, 5, JavaCollectionsUtil.SingletonList<String>
                ("eng"));
            NUnit.Framework.Assert.IsNotNull(realOutputHocr);
            NUnit.Framework.Assert.AreEqual(expectedOutput, realOutputHocr);
        }

        [NUnit.Framework.Test]
        public virtual void TestInputMultipagesTIFFWithoutPreprocessing() {
            String path = TEST_IMAGES_DIRECTORY + "multîpage.tiff";
            String expectedOutput = "Multipage\nTIFF\nExample\nPage 3";
            FileInfo file = new FileInfo(path);
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages
                (false));
            String realOutputHocr = GetTextFromPdf(tesseractReader, file, 3, JavaCollectionsUtil.SingletonList<String>
                ("eng"));
            NUnit.Framework.Assert.IsNotNull(realOutputHocr);
            NUnit.Framework.Assert.AreEqual(expectedOutput, realOutputHocr);
        }

        [LogMessage(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestInputWrongFormat() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(TEST_IMAGES_DIRECTORY + "wierdwords.gif");
                GetTextFromPdf(tesseractReader, file);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfOcrTesseract4Exception>().With.Message.EqualTo(MessageFormatUtil.Format(PdfOcrTesseract4ExceptionMessageConstant.INCORRECT_INPUT_IMAGE_FORMAT, "wierdwords.gif")))
;
        }

        [NUnit.Framework.Test]
        public virtual void TestSupportedImageWithIncorrectTypeInName() {
            String path = TEST_IMAGES_DIRECTORY + "numbers_01.nnn";
            String expectedOutput = "619121";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void TestJpgWithoutPreprocessing() {
            String path = TEST_IMAGES_DIRECTORY + "nümbérs.jpg";
            String expectedOutput = "619121";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages
                (false));
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path), JavaCollectionsUtil.SingletonList
                <String>("eng"));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void CompareNumbersJPG() {
            String testName = "compareNumbersJPG";
            String filename = "nümbérs";
            String expectedPdfPath = TEST_DOCUMENTS_DIRECTORY + "numbers_01.pdf";
            String resultPdfPath = GetTargetDirectory() + "numbers_01_" + testName + ".pdf";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetTextPositioning
                (TextPositioning.BY_WORDS));
            DoOcrAndSavePdfToPath(tesseractReader, TEST_IMAGES_DIRECTORY + filename + ".jpg", resultPdfPath);
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetTextPositioning
                (TextPositioning.BY_LINES));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, GetTargetDirectory
                (), "diff_"));
        }
    }
}
