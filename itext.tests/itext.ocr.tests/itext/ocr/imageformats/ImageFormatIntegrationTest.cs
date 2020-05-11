using System;
using System.IO;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Utils;
using iText.Ocr;
using iText.Test.Attributes;

namespace iText.Ocr.Imageformats {
    public abstract class ImageFormatIntegrationTest : AbstractIntegrationTest {
        internal TesseractReader tesseractReader;

        internal String parameter;

        public ImageFormatIntegrationTest(String type) {
            parameter = type;
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.Test]
        public virtual void TestBMPText() {
            String path = testImagesDirectory + "example_01.BMP";
            String expectedOutput = "This is a test message for OCR Scanner Test";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path), JavaCollectionsUtil.SingletonList
                <String>("eng"));
            realOutputHocr = iText.IO.Util.StringUtil.ReplaceAll(realOutputHocr, "[\n]", " ");
            realOutputHocr = iText.IO.Util.StringUtil.ReplaceAll(realOutputHocr, "[‘]", "");
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains((expectedOutput)));
        }

        [NUnit.Framework.Test]
        public virtual void TestBMPText02() {
            String path = testImagesDirectory + "englishText.bmp";
            String expectedOutput = "This is a test message for OCR Scanner Test BMPTest";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path), JavaCollectionsUtil.SingletonList
                <String>("eng"));
            realOutputHocr = iText.IO.Util.StringUtil.ReplaceAll(realOutputHocr, "[\n]", " ");
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains((expectedOutput)));
        }

        [NUnit.Framework.Test]
        public virtual void CompareJFIF() {
            bool preprocess = tesseractReader.IsPreprocessingImages();
            String filename = "example_02";
            String expectedPdfPath = testDocumentsDirectory + filename + ".pdf";
            String resultPdfPath = testDocumentsDirectory + filename + "_created.pdf";
            DoOcrAndSavePdfToPath(tesseractReader, testImagesDirectory + filename + ".JFIF", resultPdfPath, null, DeviceCmyk
                .MAGENTA);
            try {
                new CompareTool().CompareByContent(expectedPdfPath, resultPdfPath, testDocumentsDirectory, "diff_");
            }
            finally {
                DeleteFile(resultPdfPath);
                tesseractReader.SetPreprocessingImages(preprocess);
                tesseractReader.SetTextPositioning(IOcrReader.TextPositioning.BY_LINES);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromJPG() {
            String path = testImagesDirectory + "numbers_02.jpg";
            String expectedOutput = "0123456789";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromJPE() {
            String path = testImagesDirectory + "numbers_01.jpe";
            String expectedOutput = "619121";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromTIF() {
            String path = testImagesDirectory + "numbers_01.tif";
            String expectedOutput = "619121";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPNM() {
            String path = testImagesDirectory + "numbers_01.pnm";
            String expectedOutput = "619121";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPPM() {
            String path = testImagesDirectory + "numbers_01.ppm";
            String expectedOutput = "619121";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.AreEqual(realOutputHocr, expectedOutput);
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPPM02() {
            String path = testImagesDirectory + "englishText.ppm";
            String expectedOutput = "This is a test message for OCR Scanner Test PPMTest";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            realOutputHocr = iText.IO.Util.StringUtil.ReplaceAll(realOutputHocr, "[\n]", " ");
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains((expectedOutput)));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPPMWithoutPreprocessing() {
            String path = testImagesDirectory + "numbers_01.ppm";
            String expectedOutput = "619121";
            tesseractReader.SetPreprocessingImages(false);
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.AreEqual(realOutputHocr, expectedOutput);
            tesseractReader.SetPreprocessingImages(true);
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPGM() {
            String path = testImagesDirectory + "numbers_01.pgm";
            String expectedOutput = "619121";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPGM02() {
            String path = testImagesDirectory + "englishText.pgm";
            String expectedOutput = "This is a test message for OCR Scanner Test PGMTest";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            realOutputHocr = iText.IO.Util.StringUtil.ReplaceAll(realOutputHocr, "[\n]", " ");
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains((expectedOutput)));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPBM() {
            String path = testImagesDirectory + "numbers_01.pbm";
            String expectedOutput = "619121";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromPBM02() {
            String path = testImagesDirectory + "englishText.pbm";
            String expectedOutput = "This is a test message for OCR Scanner Test PBMTest";
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            realOutputHocr = iText.IO.Util.StringUtil.ReplaceAll(realOutputHocr, "[\n]", " ");
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains((expectedOutput)));
        }

        [NUnit.Framework.Test]
        public virtual void TestBigTiffWithoutPreprocessing() {
            String path = testImagesDirectory + "example_03_10MB.tiff";
            String expectedOutput = "Image File Format";
            tesseractReader.SetPreprocessingImages(false);
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path), JavaCollectionsUtil.SingletonList
                <String>("eng"));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
            tesseractReader.SetPreprocessingImages(true);
        }

        [NUnit.Framework.Test]
        public virtual void TestInputMultipagesTIFFWithPreprocessing() {
            bool preprocess = tesseractReader.IsPreprocessingImages();
            String path = testImagesDirectory + "multipage.tiff";
            String expectedOutput = "Multipage\nTIFF\nExample\nPage 5";
            FileInfo file = new FileInfo(path);
            String realOutputHocr = GetTextFromPdf(tesseractReader, file, 5, JavaCollectionsUtil.SingletonList<String>
                ("eng"));
            NUnit.Framework.Assert.IsNotNull(realOutputHocr);
            NUnit.Framework.Assert.AreEqual(expectedOutput, realOutputHocr);
            tesseractReader.SetPreprocessingImages(preprocess);
        }

        [NUnit.Framework.Test]
        public virtual void TestInputMultipagesTIFFWithoutPreprocessing() {
            bool preprocess = tesseractReader.IsPreprocessingImages();
            String path = testImagesDirectory + "multipage.tiff";
            String expectedOutput = "Multipage\nTIFF\nExample\nPage 3";
            FileInfo file = new FileInfo(path);
            tesseractReader.SetPreprocessingImages(false);
            String realOutputHocr = GetTextFromPdf(tesseractReader, file, 3, JavaCollectionsUtil.SingletonList<String>
                ("eng"));
            NUnit.Framework.Assert.IsNotNull(realOutputHocr);
            NUnit.Framework.Assert.AreEqual(expectedOutput, realOutputHocr);
            tesseractReader.SetPreprocessingImages(preprocess);
        }

        [LogMessage(LogMessageConstant.CannotReadInputImage, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestInputWrongFormat() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(testImagesDirectory + "example.txt");
                String realOutput = GetTextFromPdf(tesseractReader, file);
                NUnit.Framework.Assert.IsNotNull(realOutput);
                NUnit.Framework.Assert.AreEqual("", realOutput);
            }
            , NUnit.Framework.Throws.InstanceOf<OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(OcrException.IncorrectInputImageFormat, "txt")))
;
        }

        [NUnit.Framework.Test]
        public virtual void CompareNumbersJPG() {
            String filename = "numbers_01";
            String expectedPdfPath = testDocumentsDirectory + filename + ".pdf";
            String resultPdfPath = testDocumentsDirectory + filename + "_created.pdf";
            tesseractReader.SetTextPositioning(IOcrReader.TextPositioning.BY_WORDS);
            DoOcrAndSavePdfToPath(tesseractReader, testImagesDirectory + filename + ".jpg", resultPdfPath);
            try {
                new CompareTool().CompareByContent(expectedPdfPath, resultPdfPath, testDocumentsDirectory, "diff_");
            }
            finally {
                DeleteFile(resultPdfPath);
                tesseractReader.SetTextPositioning(IOcrReader.TextPositioning.BY_LINES);
            }
        }
    }
}
