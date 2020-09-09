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
using System.Collections.Generic;
using System.IO;
using Common.Logging;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4;
using iText.Test.Attributes;

namespace iText.Pdfocr.Tessdata {
    public abstract class TessDataIntegrationTest : IntegrationTestHelper {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Pdfocr.Tessdata.TessDataIntegrationTest
            ));

        internal AbstractTesseract4OcrEngine tesseractReader;

        internal String testFileTypeName;

        private bool isExecutableReaderType;

        public TessDataIntegrationTest(IntegrationTestHelper.ReaderType type) {
            isExecutableReaderType = type.Equals(IntegrationTestHelper.ReaderType.EXECUTABLE);
            if (isExecutableReaderType) {
                testFileTypeName = "executable";
            }
            else {
                testFileTypeName = "lib";
            }
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.SetUp]
        public virtual void InitTesseractProperties() {
            Tesseract4OcrEngineProperties ocrEngineProperties = new Tesseract4OcrEngineProperties();
            ocrEngineProperties.SetPathToTessData(GetTessDataDirectory());
            tesseractReader.SetTesseract4OcrEngineProperties(ocrEngineProperties);
        }

        [NUnit.Framework.Test]
        public virtual void TextGreekText() {
            String imgPath = TEST_IMAGES_DIRECTORY + "greek_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "ΟΜΟΛΟΓΙΑ";
            if (isExecutableReaderType) {
                tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages
                    (false));
            }
            String real = GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("ell"), NOTO_SANS_FONT_PATH);
            // correct result with specified greek language
            NUnit.Framework.Assert.IsTrue(real.Contains(expected));
        }

        [NUnit.Framework.Test]
        public virtual void TextJapaneseText() {
            String imgPath = TEST_IMAGES_DIRECTORY + "japanese_01.png";
            FileInfo file = new FileInfo(imgPath);
            String expected = "日 本 語\n文法";
            // correct result with specified japanese language
            NUnit.Framework.Assert.AreEqual(expected, GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("jpn"
                ), KOSUGI_FONT_PATH));
        }

        [NUnit.Framework.Test]
        public virtual void TestFrench() {
            String imgPath = TEST_IMAGES_DIRECTORY + "french_01.png";
            FileInfo file = new FileInfo(imgPath);
            String expectedFr = "RESTEZ\nCALME\nPARLEZ EN\nFRANÇAIS";
            // correct result with specified spanish language
            NUnit.Framework.Assert.IsTrue(GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String
                >("fra")).EndsWith(expectedFr));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.IsFalse(GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String
                >("eng")).EndsWith(expectedFr));
            NUnit.Framework.Assert.AreNotEqual(expectedFr, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("spa")));
            NUnit.Framework.Assert.AreNotEqual(expectedFr, GetTextFromPdf(tesseractReader, file, new List<String>()));
        }

        [NUnit.Framework.Test]
        public virtual void TestSpanishPNG() {
            String testName = "compareSpanishPNG";
            String filename = "scanned_spa_01";
            String expectedText1 = "¿Y SI ENSAYARA COMO ACTUAR?";
            String expectedText2 = "¿Y SI ENSAYARA ACTUAR?";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + "_" + testFileTypeName + ".pdf";
            IList<String> languages = JavaUtil.ArraysAsList("spa", "spa_old");
            Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
            if (isExecutableReaderType) {
                properties.SetPreprocessingImages(false);
            }
            // locate text by words
            properties.SetTextPositioning(TextPositioning.BY_WORDS);
            properties.SetLanguages(languages);
            tesseractReader.SetTesseract4OcrEngineProperties(properties);
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
            ocrPdfCreatorProperties.SetTextColor(DeviceCmyk.BLACK);
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader, ocrPdfCreatorProperties);
            using (PdfWriter pdfWriter = GetPdfWriter(resultPdfPath)) {
                ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(TEST_IMAGES_DIRECTORY + filename
                     + ".png")), pdfWriter).Close();
            }
            try {
                String result = GetTextFromPdfLayer(resultPdfPath, null, 1).Replace("\n", " ");
                NUnit.Framework.Assert.IsTrue(result.Contains(expectedText1) || result.Contains(expectedText2));
            }
            finally {
                NUnit.Framework.Assert.AreEqual(TextPositioning.BY_WORDS, tesseractReader.GetTesseract4OcrEngineProperties
                    ().GetTextPositioning());
            }
        }

        [NUnit.Framework.Test]
        public virtual void TextGreekOutputFromTxtFile() {
            String imgPath = TEST_IMAGES_DIRECTORY + "greek_01.jpg";
            String expected = "ΟΜΟΛΟΓΙΑ";
            if (isExecutableReaderType) {
                tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages
                    (false));
            }
            String result = GetRecognizedTextFromTextFile(tesseractReader, imgPath, JavaCollectionsUtil.SingletonList<
                String>("ell"));
            // correct result with specified greek language
            NUnit.Framework.Assert.IsTrue(result.Contains(expected));
        }

        [NUnit.Framework.Test]
        public virtual void TextJapaneseOutputFromTxtFile() {
            String imgPath = TEST_IMAGES_DIRECTORY + "japanese_01.png";
            String expected = "日本語文法";
            String result = GetRecognizedTextFromTextFile(tesseractReader, imgPath, JavaCollectionsUtil.SingletonList<
                String>("jpn"));
            result = iText.IO.Util.StringUtil.ReplaceAll(result, "[\f\n]", "");
            // correct result with specified japanese language
            NUnit.Framework.Assert.IsTrue(result.Contains(expected));
        }

        [NUnit.Framework.Test]
        public virtual void TestFrenchOutputFromTxtFile() {
            String imgPath = TEST_IMAGES_DIRECTORY + "french_01.png";
            String expectedFr = "RESTEZ\nCALME\nPARLEZ EN\nFRANÇAIS";
            String result = GetRecognizedTextFromTextFile(tesseractReader, imgPath, JavaCollectionsUtil.SingletonList<
                String>("fra"));
            result = iText.IO.Util.StringUtil.ReplaceAll(result, "(?:\\n\\f)+", "").Trim();
            result = iText.IO.Util.StringUtil.ReplaceAll(result, "\\n\\n", "\n").Trim();
            // correct result with specified spanish language
            NUnit.Framework.Assert.IsTrue(result.EndsWith(expectedFr));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.IsFalse(GetRecognizedTextFromTextFile(tesseractReader, imgPath, JavaCollectionsUtil
                .SingletonList<String>("eng")).EndsWith(expectedFr));
            NUnit.Framework.Assert.AreNotEqual(expectedFr, GetRecognizedTextFromTextFile(tesseractReader, imgPath, JavaCollectionsUtil
                .SingletonList<String>("spa")));
            NUnit.Framework.Assert.AreNotEqual(expectedFr, GetRecognizedTextFromTextFile(tesseractReader, imgPath, new 
                List<String>()));
        }

        [NUnit.Framework.Test]
        public virtual void TestArabicOutputFromTxtFile() {
            String imgPath = TEST_IMAGES_DIRECTORY + "arabic_02.png";
            // First sentence
            String expected = "اللغة العربية";
            String result = GetRecognizedTextFromTextFile(tesseractReader, imgPath, JavaCollectionsUtil.SingletonList<
                String>("ara"));
            // correct result with specified arabic language
            NUnit.Framework.Assert.IsTrue(result.StartsWith(expected));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            String engResult = GetRecognizedTextFromTextFile(tesseractReader, imgPath, JavaCollectionsUtil.SingletonList
                <String>("eng"));
            NUnit.Framework.Assert.IsFalse(engResult.StartsWith(expected));
            String spaResult = GetRecognizedTextFromTextFile(tesseractReader, imgPath, JavaCollectionsUtil.SingletonList
                <String>("spa"));
            NUnit.Framework.Assert.IsFalse(spaResult.StartsWith(expected));
            String langNotSpecifiedResult = GetRecognizedTextFromTextFile(tesseractReader, imgPath, new List<String>()
                );
            NUnit.Framework.Assert.IsFalse(langNotSpecifiedResult.StartsWith(expected));
        }

        [NUnit.Framework.Test]
        public virtual void TestGermanAndCompareTxtFiles() {
            String imgPath = TEST_IMAGES_DIRECTORY + "german_01.jpg";
            String expectedTxt = TEST_DOCUMENTS_DIRECTORY + "german_01" + testFileTypeName + ".txt";
            bool result = DoOcrAndCompareTxtFiles(tesseractReader, imgPath, expectedTxt, JavaCollectionsUtil.SingletonList
                <String>("deu"));
            NUnit.Framework.Assert.IsTrue(result);
        }

        [NUnit.Framework.Test]
        public virtual void TestMultipageTiffAndCompareTxtFiles() {
            String imgPath = TEST_IMAGES_DIRECTORY + "multîpage.tiff";
            String expectedTxt = TEST_DOCUMENTS_DIRECTORY + "multipage_" + testFileTypeName + ".txt";
            bool result = DoOcrAndCompareTxtFiles(tesseractReader, imgPath, expectedTxt, JavaCollectionsUtil.SingletonList
                <String>("eng"));
            NUnit.Framework.Assert.IsTrue(result);
        }

        [NUnit.Framework.Test]
        public virtual void TestGermanWithTessData() {
            String imgPath = TEST_IMAGES_DIRECTORY + "german_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            String expectedGerman = "Das Geheimnis\ndes Könnens\nliegt im Wollen.";
            String res = GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("deu"));
            // correct result with specified spanish language
            NUnit.Framework.Assert.AreEqual(expectedGerman, res);
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.AreNotEqual(expectedGerman, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil
                .SingletonList<String>("eng")));
            NUnit.Framework.Assert.AreNotEqual(expectedGerman, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil
                .SingletonList<String>("fra")));
            NUnit.Framework.Assert.AreNotEqual(expectedGerman, GetTextFromPdf(tesseractReader, file, new List<String>(
                )));
        }

        [NUnit.Framework.Test]
        public virtual void TestArabicTextWithEng() {
            String imgPath = TEST_IMAGES_DIRECTORY + "arabic_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "الحية. والضحك؛ والحب\nlive, laugh, love";
            String result = GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("ara", "eng"), CAIRO_FONT_PATH
                );
            // correct result with specified arabic+english languages
            NUnit.Framework.Assert.AreEqual(expected, iText.IO.Util.StringUtil.ReplaceAll(result, "[?]", ""));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("eng"), CAIRO_FONT_PATH));
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, new List<String>(), CAIRO_FONT_PATH
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestArabicText() {
            String imgPath = TEST_IMAGES_DIRECTORY + "arabic_02.png";
            FileInfo file = new FileInfo(imgPath);
            // First sentence
            String expected = "اللغة العربية";
            // correct result with specified arabic language
            NUnit.Framework.Assert.AreEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("ara"), CAIRO_FONT_PATH));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("eng"), CAIRO_FONT_PATH));
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("spa"), CAIRO_FONT_PATH));
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, new List<String>(), CAIRO_FONT_PATH
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CompareMultiLangImage() {
            String testName = "compareMultiLangImage";
            String filename = "multilang";
            String expectedPdfPath = TEST_DOCUMENTS_DIRECTORY + filename + "_" + testFileTypeName + ".pdf";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + "_" + testFileTypeName + ".pdf";
            try {
                Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
                properties.SetTextPositioning(TextPositioning.BY_WORDS);
                properties.SetPathToTessData(GetTessDataDirectory());
                properties.SetPageSegMode(3);
                tesseractReader.SetTesseract4OcrEngineProperties(properties);
                DoOcrAndSavePdfToPath(tesseractReader, TEST_IMAGES_DIRECTORY + filename + ".jpg", resultPdfPath, JavaUtil.ArraysAsList
                    ("eng", "deu", "spa"), DeviceCmyk.BLACK);
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, TEST_DOCUMENTS_DIRECTORY
                    , "diff_"));
            }
            finally {
                NUnit.Framework.Assert.AreEqual(TextPositioning.BY_WORDS, tesseractReader.GetTesseract4OcrEngineProperties
                    ().GetTextPositioning());
                NUnit.Framework.Assert.AreEqual(3, tesseractReader.GetTesseract4OcrEngineProperties().GetPageSegMode().Value
                    );
            }
        }

        [LogMessage(PdfOcrLogMessageConstant.COULD_NOT_FIND_CORRESPONDING_GLYPH_TO_UNICODE_CHARACTER, Count = 12)]
        [NUnit.Framework.Test]
        public virtual void TestHindiTextWithUrdu() {
            String testName = "testHindiTextWithUrdu";
            String imgPath = TEST_IMAGES_DIRECTORY + "hindi_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            String pdfPath = GetTargetDirectory() + testName + ".pdf";
            String expectedHindi = "हिन्दुस्तानी";
            String expectedUrdu = "وتالی";
            DoOcrAndSavePdfToPath(tesseractReader, file.FullName, pdfPath, JavaUtil.ArraysAsList("hin", "urd"), JavaCollectionsUtil
                .SingletonList(CAIRO_FONT_PATH));
            String resultWithoutActualText = GetTextFromPdfLayer(pdfPath, null, 1);
            // because of provided font only urdu will be displayed correctly
            NUnit.Framework.Assert.IsTrue(resultWithoutActualText.Contains(expectedUrdu));
            NUnit.Framework.Assert.IsFalse(resultWithoutActualText.Contains(expectedHindi));
            String resultWithActualText = GetTextFromPdfLayerUsingActualText(pdfPath, null, 1);
            // actual text should contain all text
            NUnit.Framework.Assert.IsTrue(resultWithActualText.Contains(expectedUrdu));
            NUnit.Framework.Assert.IsTrue(resultWithActualText.Contains(expectedHindi));
        }

        [LogMessage(PdfOcrLogMessageConstant.COULD_NOT_FIND_CORRESPONDING_GLYPH_TO_UNICODE_CHARACTER, Ignore = true
            )]
        [NUnit.Framework.Test]
        public virtual void TestHindiTextWithUrduActualTextWithIncorrectFont() {
            String testName = "testHindiTextWithUrduActualTextWithIncorrectFont";
            String imgPath = TEST_IMAGES_DIRECTORY + "hindi_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            String pdfPath = GetTargetDirectory() + testName + ".pdf";
            String expectedHindi = "हिन्दुस्तानी";
            String expectedUrdu = "وتالی";
            DoOcrAndSavePdfToPath(tesseractReader, file.FullName, pdfPath, JavaUtil.ArraysAsList("hin", "urd"), null, 
                null);
            String resultWithoutActualText = GetTextFromPdfLayer(pdfPath, null, 1);
            // because of provided font only urdu will be displayed correctly
            NUnit.Framework.Assert.IsFalse(resultWithoutActualText.Contains(expectedUrdu));
            NUnit.Framework.Assert.IsFalse(resultWithoutActualText.Contains(expectedHindi));
            String resultWithActualText = GetTextFromPdfLayerUsingActualText(pdfPath, null, 1);
            // actual text should contain all text
            NUnit.Framework.Assert.IsTrue(resultWithActualText.Contains(expectedUrdu));
            NUnit.Framework.Assert.IsTrue(resultWithActualText.Contains(expectedHindi));
        }

        [NUnit.Framework.Test]
        public virtual void TestHindiTextWithEng() {
            String imgPath = TEST_IMAGES_DIRECTORY + "hindi_02.jpg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "मानक हनिदी\nHindi";
            // correct result with specified arabic+english languages
            NUnit.Framework.Assert.AreEqual(expected, GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("hin"
                , "eng"), NOTO_SANS_FONT_PATH));
            // incorrect result without specified english language
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("hin"), NOTO_SANS_FONT_PATH));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("eng"), NOTO_SANS_FONT_PATH));
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file));
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, new List<String>(), NOTO_SANS_FONT_PATH
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestGeorgianText() {
            String imgPath = TEST_IMAGES_DIRECTORY + "georgian_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            // First sentence
            String expected = "ღმერთი";
            String result = GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("kat"), FREE_SANS_FONT_PATH
                );
            // correct result with specified georgian+eng language
            NUnit.Framework.Assert.AreEqual(expected, result);
            result = GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("kat", "kat_old"), FREE_SANS_FONT_PATH
                );
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [LogMessage(PdfOcrLogMessageConstant.COULD_NOT_FIND_CORRESPONDING_GLYPH_TO_UNICODE_CHARACTER, Count = 6)]
        [NUnit.Framework.Test]
        public virtual void TestGeorgianActualTextWithDefaultFont() {
            String testName = "testGeorgianActualTextWithDefaultFont";
            String pdfPath = GetTargetDirectory() + testName + ".pdf";
            String imgPath = TEST_IMAGES_DIRECTORY + "georgian_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            // First sentence
            String expected = "ღმერთი";
            DoOcrAndSavePdfToPath(tesseractReader, file.FullName, pdfPath, JavaCollectionsUtil.SingletonList<String>("kat"
                ), null, null);
            String resultWithoutActualText = GetTextFromPdfLayer(pdfPath, null, 1);
            NUnit.Framework.Assert.AreNotEqual(expected, resultWithoutActualText);
            String resultWithActualText = GetTextFromPdfLayerUsingActualText(pdfPath, null, 1);
            NUnit.Framework.Assert.AreEqual(expected, resultWithActualText);
        }

        [NUnit.Framework.Test]
        public virtual void TestBengali() {
            String imgPath = TEST_IMAGES_DIRECTORY + "bengali_01.jpeg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "ইংরজে\nশখো";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetTextPositioning
                (TextPositioning.BY_WORDS));
            // correct result with specified spanish language
            String result = GetTextFromPdf(tesseractReader, file, 1, JavaCollectionsUtil.SingletonList<String>("ben"), 
                JavaUtil.ArraysAsList(FREE_SANS_FONT_PATH, KOSUGI_FONT_PATH));
            NUnit.Framework.Assert.AreEqual(expected, result);
            NUnit.Framework.Assert.AreEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("ben"), FREE_SANS_FONT_PATH));
        }

        [LogMessage(PdfOcrLogMessageConstant.COULD_NOT_FIND_CORRESPONDING_GLYPH_TO_UNICODE_CHARACTER, Count = 8)]
        [NUnit.Framework.Test]
        public virtual void TestBengaliActualTextWithDefaultFont() {
            String testName = "testBengaliActualTextWithDefaultFont";
            String pdfPath = GetTargetDirectory() + testName + ".pdf";
            String imgPath = TEST_IMAGES_DIRECTORY + "bengali_01.jpeg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "ইংরজে\nশখো";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetTextPositioning
                (TextPositioning.BY_WORDS));
            DoOcrAndSavePdfToPath(tesseractReader, file.FullName, pdfPath, JavaCollectionsUtil.SingletonList<String>("ben"
                ), null, null);
            String resultWithoutActualText = GetTextFromPdfLayer(pdfPath, null, 1);
            NUnit.Framework.Assert.AreNotEqual(expected, resultWithoutActualText);
            String resultWithActualText = GetTextFromPdfLayerUsingActualText(pdfPath, null, 1);
            NUnit.Framework.Assert.AreEqual(expected, resultWithActualText);
        }

        [LogMessage(PdfOcrLogMessageConstant.COULD_NOT_FIND_CORRESPONDING_GLYPH_TO_UNICODE_CHARACTER, Count = 6)]
        [NUnit.Framework.Test]
        public virtual void TestChinese() {
            String imgPath = TEST_IMAGES_DIRECTORY + "chinese_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "你 好\nni hao";
            // correct result with specified spanish language
            NUnit.Framework.Assert.AreEqual(expected, GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("chi_sim"
                , "chi_tra"), NOTO_SANS_SC_FONT_PATH));
            NUnit.Framework.Assert.AreEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("chi_sim"), NOTO_SANS_SC_FONT_PATH));
            NUnit.Framework.Assert.AreEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("chi_tra"), NOTO_SANS_SC_FONT_PATH));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("chi_sim")));
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("chi_tra")));
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("chi_sim"
                , "chi_tra")));
            NUnit.Framework.Assert.IsFalse(GetTextFromPdf(tesseractReader, file, new List<String>()).Contains(expected
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestSpanishWithTessData() {
            String imgPath = TEST_IMAGES_DIRECTORY + "spanish_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            String expectedSpanish = "Aquí\nhablamos\nespañol";
            // correct result with specified spanish language
            NUnit.Framework.Assert.AreEqual(expectedSpanish, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil
                .SingletonList<String>("spa")));
            NUnit.Framework.Assert.AreEqual(expectedSpanish, GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList
                ("spa", "eng")));
            NUnit.Framework.Assert.AreEqual(expectedSpanish, GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList
                ("eng", "spa")));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.AreNotEqual(expectedSpanish, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil
                .SingletonList<String>("eng")));
            NUnit.Framework.Assert.AreNotEqual(expectedSpanish, GetTextFromPdf(tesseractReader, file, new List<String>
                ()));
        }

        [NUnit.Framework.Test]
        public virtual void TestBengaliScript() {
            String imgPath = TEST_IMAGES_DIRECTORY + "bengali_01.jpeg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "ইংরজে";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPathToTessData
                (new FileInfo(SCRIPT_TESS_DATA_DIRECTORY)));
            // correct result with specified spanish language
            NUnit.Framework.Assert.IsTrue(GetTextFromPdf(tesseractReader, file, 1, JavaCollectionsUtil.SingletonList<String
                >("Bengali"), JavaUtil.ArraysAsList(FREE_SANS_FONT_PATH, KOSUGI_FONT_PATH)).StartsWith(expected));
        }

        [NUnit.Framework.Test]
        public virtual void TestGeorgianTextWithScript() {
            String imgPath = TEST_IMAGES_DIRECTORY + "georgian_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            // First sentence
            String expected = "ღმერთი";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPathToTessData
                (new FileInfo(SCRIPT_TESS_DATA_DIRECTORY)));
            // correct result with specified georgian+eng language
            NUnit.Framework.Assert.IsTrue(GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String
                >("Georgian"), FREE_SANS_FONT_PATH).StartsWith(expected));
        }

        [NUnit.Framework.Test]
        public virtual void TestJapaneseScript() {
            String imgPath = TEST_IMAGES_DIRECTORY + "japanese_01.png";
            FileInfo file = new FileInfo(imgPath);
            String expected = "日 本 語\n文法";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPathToTessData
                (new FileInfo(SCRIPT_TESS_DATA_DIRECTORY)));
            // correct result with specified japanese language
            String result = GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("Japanese"), KOSUGI_FONT_PATH);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void TestTargetDirectoryWithNonAsciiPath() {
            String imgPath = TEST_IMAGES_DIRECTORY + "german_01.jpg";
            String expectedTxt = TEST_DOCUMENTS_DIRECTORY + "german_01" + testFileTypeName + ".txt";
            IList<String> languages = JavaCollectionsUtil.SingletonList<String>("deu");
            String resultTxtFile = GetNonAsciiTargetDirectory() + GetImageName(imgPath, languages) + ".txt";
            DoOcrAndSaveToTextFile(tesseractReader, imgPath, resultTxtFile, languages);
            bool result = CompareTxtFiles(expectedTxt, resultTxtFile);
            NUnit.Framework.Assert.IsTrue(result);
        }

        [NUnit.Framework.Test]
        public virtual void TestThai03ImageWithImprovedHocrParsing() {
            String[] expected = new String[] { "บ๊อบสตรอเบอรีออดิชั่นธัม โมเนิร์สเซอรี่", "ศากยบุตร เอเซีย", "หน่อมแน้ม เวอร์เบอร์เกอร์แชมป์"
                 };
            String imgPath = TEST_IMAGES_DIRECTORY + "thai_03.jpg";
            FileInfo file = new FileInfo(imgPath);
            Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
            properties.SetTextPositioning(TextPositioning.BY_WORDS_AND_LINES);
            properties.SetUseTxtToImproveHocrParsing(true);
            properties.SetMinimalConfidenceLevel(80);
            properties.SetPathToTessData(new FileInfo(LANG_TESS_DATA_DIRECTORY));
            tesseractReader.SetTesseract4OcrEngineProperties(properties);
            String pdfText = GetTextFromPdf(tesseractReader, file, 1, JavaUtil.ArraysAsList("tha"), JavaUtil.ArraysAsList
                (NOTO_SANS_THAI_FONT_PATH, NOTO_SANS_FONT_PATH));
            foreach (String e in expected) {
                NUnit.Framework.Assert.IsTrue(pdfText.Contains(e));
            }
        }

        /// <summary>
        /// Do OCR and retrieve text from the first page of result PDF document
        /// using tess data placed by path with non ASCII characters.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// </returns>
        protected internal virtual String DoOcrAndGetTextUsingTessDataByNonAsciiPath() {
            String imgPath = TEST_IMAGES_DIRECTORY + "georgian_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPathToTessData
                (new FileInfo(NON_ASCII_TESS_DATA_DIRECTORY)));
            return GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("Georgian"), FREE_SANS_FONT_PATH
                );
        }

        /// <summary>Do OCR for given image and compare result text file with expected one.</summary>
        private bool DoOcrAndCompareTxtFiles(AbstractTesseract4OcrEngine tesseractReader, String imgPath, String expectedPath
            , IList<String> languages) {
            String resultTxtFile = GetTargetDirectory() + GetImageName(imgPath, languages) + ".txt";
            DoOcrAndSaveToTextFile(tesseractReader, imgPath, resultTxtFile, languages);
            return CompareTxtFiles(expectedPath, resultTxtFile);
        }

        /// <summary>Compare two arrays of text lines.</summary>
        private bool CompareTxtLines(IList<String> expected, IList<String> result) {
            bool areEqual = true;
            if (expected.Count != result.Count) {
                return false;
            }
            for (int i = 0; i < expected.Count; i++) {
                String exp = expected[i].Replace("\n", "").Replace("\f", "");
                exp = iText.IO.Util.StringUtil.ReplaceAll(exp, "[^\\u0009\\u000A\\u000D\\u0020-\\u007E]", "");
                String res = result[i].Replace("\n", "").Replace("\f", "");
                res = iText.IO.Util.StringUtil.ReplaceAll(res, "[^\\u0009\\u000A\\u000D\\u0020-\\u007E]", "");
                if (expected[i] == null || result[i] == null) {
                    areEqual = false;
                    break;
                }
                else {
                    if (!exp.Equals(res)) {
                        areEqual = false;
                        break;
                    }
                }
            }
            return areEqual;
        }

        /// <summary>Compare two text files using provided paths.</summary>
        private bool CompareTxtFiles(String expectedFilePath, String resultFilePath) {
            bool areEqual = true;
            try {
                IList<String> expected = File.ReadAllLines(System.IO.Path.Combine(expectedFilePath));
                IList<String> result = File.ReadAllLines(System.IO.Path.Combine(resultFilePath));
                areEqual = CompareTxtLines(expected, result);
            }
            catch (System.IO.IOException e) {
                areEqual = false;
                LOGGER.Error(e.Message);
            }
            return areEqual;
        }
    }
}
