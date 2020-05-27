using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4;

namespace iText.Pdfocr.Tessdata {
    public abstract class TessDataIntegrationTest : AbstractIntegrationTest {
        internal AbstractTesseract4OcrEngine tesseractReader;

        internal String testFileTypeName;

        private bool isExecutableReaderType;

        public TessDataIntegrationTest(AbstractIntegrationTest.ReaderType type) {
            isExecutableReaderType = type.Equals(AbstractIntegrationTest.ReaderType.EXECUTABLE);
            if (isExecutableReaderType) {
                testFileTypeName = "executable";
            }
            else {
                testFileTypeName = "lib";
            }
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.Test]
        public virtual void TextGreekText() {
            String imgPath = testImagesDirectory + "greek_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "ΟΜΟΛΟΓΙΑ";
            if (isExecutableReaderType) {
                tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages
                    (false));
            }
            String real = GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("ell"), notoSansFontPath);
            // correct result with specified greek language
            NUnit.Framework.Assert.IsTrue(real.Contains(expected));
        }

        [NUnit.Framework.Test]
        public virtual void TextJapaneseText() {
            String imgPath = testImagesDirectory + "japanese_01.png";
            FileInfo file = new FileInfo(imgPath);
            String expected = "日 本 語\n文法";
            // correct result with specified japanese language
            NUnit.Framework.Assert.AreEqual(expected, GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("jpn"
                ), kosugiFontPath));
        }

        [NUnit.Framework.Test]
        public virtual void TestFrench() {
            String imgPath = testImagesDirectory + "french_01.png";
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
        public virtual void CompareSpanishPNG() {
            String testName = "compareSpanishPNG";
            String filename = "scanned_spa_01";
            String expectedPdfPath = testDocumentsDirectory + filename + testFileTypeName + ".pdf";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + ".pdf";
            IList<String> languages = JavaUtil.ArraysAsList("spa", "spa_old");
            if (isExecutableReaderType) {
                tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages
                    (false));
            }
            // locate text by words
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetTextPositioning
                (TextPositioning.BY_WORDS));
            DoOcrAndSavePdfToPath(tesseractReader, testImagesDirectory + filename + ".png", resultPdfPath, languages, 
                DeviceCmyk.BLACK);
            try {
                new CompareTool().CompareByContent(expectedPdfPath, resultPdfPath, testDocumentsDirectory, "diff_");
            }
            finally {
                NUnit.Framework.Assert.AreEqual(TextPositioning.BY_WORDS, tesseractReader.GetTesseract4OcrEngineProperties
                    ().GetTextPositioning());
            }
        }

        [NUnit.Framework.Test]
        public virtual void TextGreekOutputFromTxtFile() {
            String imgPath = testImagesDirectory + "greek_01.jpg";
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
            String imgPath = testImagesDirectory + "japanese_01.png";
            String expected = "日本語文法";
            String result = GetRecognizedTextFromTextFile(tesseractReader, imgPath, JavaCollectionsUtil.SingletonList<
                String>("jpn"));
            result = iText.IO.Util.StringUtil.ReplaceAll(result, "[\f\n]", "");
            // correct result with specified japanese language
            NUnit.Framework.Assert.IsTrue(result.Contains(expected));
        }

        [NUnit.Framework.Test]
        public virtual void TestFrenchOutputFromTxtFile() {
            String imgPath = testImagesDirectory + "french_01.png";
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
            String imgPath = testImagesDirectory + "arabic_02.png";
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
            String imgPath = testImagesDirectory + "german_01.jpg";
            String expectedTxt = testDocumentsDirectory + "german_01" + testFileTypeName + ".txt";
            bool result = DoOcrAndCompareTxtFiles(tesseractReader, imgPath, expectedTxt, JavaCollectionsUtil.SingletonList
                <String>("deu"));
            NUnit.Framework.Assert.IsTrue(result);
        }

        [NUnit.Framework.Test]
        public virtual void TestMultipageTiffAndCompareTxtFiles() {
            String imgPath = testImagesDirectory + "multipage.tiff";
            String expectedTxt = testDocumentsDirectory + "multipage_" + testFileTypeName + ".txt";
            bool result = DoOcrAndCompareTxtFiles(tesseractReader, imgPath, expectedTxt, JavaCollectionsUtil.SingletonList
                <String>("eng"));
            NUnit.Framework.Assert.IsTrue(result);
        }

        [NUnit.Framework.Test]
        public virtual void TestGermanWithTessData() {
            String imgPath = testImagesDirectory + "german_01.jpg";
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
            String imgPath = testImagesDirectory + "arabic_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "الحية. والضحك؛ والحب\nlive, laugh, love";
            String result = GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("ara", "eng"), cairoFontPath);
            // correct result with specified arabic+english languages
            NUnit.Framework.Assert.AreEqual(expected, iText.IO.Util.StringUtil.ReplaceAll(result, "[?]", ""));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("eng"), cairoFontPath));
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, new List<String>(), cairoFontPath
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestArabicText() {
            String imgPath = testImagesDirectory + "arabic_02.png";
            FileInfo file = new FileInfo(imgPath);
            // First sentence
            String expected = "اللغة العربية";
            // correct result with specified arabic language
            NUnit.Framework.Assert.AreEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("ara"), cairoFontPath));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("eng"), cairoFontPath));
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("spa"), cairoFontPath));
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, new List<String>(), cairoFontPath
                ));
        }

        [NUnit.Framework.Test]
        public virtual void CompareMultiLangImage() {
            String testName = "compareMultiLangImage";
            String filename = "multilang";
            String expectedPdfPath = testDocumentsDirectory + filename + "_" + testFileTypeName + ".pdf";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + ".pdf";
            try {
                Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
                properties.SetTextPositioning(TextPositioning.BY_WORDS);
                properties.SetPathToTessData(GetTessDataDirectory());
                tesseractReader.SetTesseract4OcrEngineProperties(properties);
                DoOcrAndSavePdfToPath(tesseractReader, testImagesDirectory + filename + ".png", resultPdfPath, JavaUtil.ArraysAsList
                    ("eng", "deu", "spa"), DeviceCmyk.BLACK);
                new CompareTool().CompareByContent(expectedPdfPath, resultPdfPath, testDocumentsDirectory, "diff_");
            }
            finally {
                NUnit.Framework.Assert.AreEqual(TextPositioning.BY_WORDS, tesseractReader.GetTesseract4OcrEngineProperties
                    ().GetTextPositioning());
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestHindiTextWithUrdu() {
            String imgPath = testImagesDirectory + "hindi_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            String expectedHindi = "हिन्दुस्तानी";
            String expectedUrdu = "وتالی";
            // correct result with specified arabic+urdu languages
            // but because of specified font only hindi will be displayed
            String resultHindiFont = GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("hin", "urd"), freeSansFontPath
                );
            NUnit.Framework.Assert.IsTrue(resultHindiFont.StartsWith(expectedHindi));
            NUnit.Framework.Assert.IsTrue(resultHindiFont.Contains(expectedHindi));
            NUnit.Framework.Assert.IsFalse(resultHindiFont.Contains(expectedUrdu));
            String resultArabic = GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("hin", "urd"), cairoFontPath
                );
            // correct result with specified arabic+urdu languages
            // but because of default font only urdu will be displayed
            NUnit.Framework.Assert.IsTrue(resultArabic.Contains(expectedUrdu));
            NUnit.Framework.Assert.IsFalse(resultArabic.Contains(expectedHindi));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            // with different fonts
            NUnit.Framework.Assert.IsTrue(GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String
                >("hin"), notoSansFontPath).Contains(expectedHindi));
            NUnit.Framework.Assert.IsFalse(GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String
                >("hin")).Contains(expectedHindi));
            NUnit.Framework.Assert.IsFalse(GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String
                >("eng")).Contains(expectedHindi));
            NUnit.Framework.Assert.IsFalse(GetTextFromPdf(tesseractReader, file).Contains(expectedHindi));
        }

        [NUnit.Framework.Test]
        public virtual void TestHindiTextWithEng() {
            String imgPath = testImagesDirectory + "hindi_02.jpg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "मानक हनिदी\nHindi";
            // correct result with specified arabic+english languages
            NUnit.Framework.Assert.AreEqual(expected, GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("hin"
                , "eng"), notoSansFontPath));
            // incorrect result without specified english language
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("hin"), notoSansFontPath));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("eng"), notoSansFontPath));
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file));
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, new List<String>(), notoSansFontPath
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestGeorgianText() {
            String imgPath = testImagesDirectory + "georgian_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            // First sentence
            String expected = "ღმერთი";
            String result = GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("kat"), freeSansFontPath
                );
            // correct result with specified georgian+eng language
            NUnit.Framework.Assert.AreEqual(expected, result);
            result = GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("kat", "kat_old"), freeSansFontPath);
            NUnit.Framework.Assert.AreEqual(expected, result);
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.IsFalse(GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String
                >("kat")).Contains(expected));
            NUnit.Framework.Assert.IsFalse(GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String
                >("eng")).Contains(expected));
            NUnit.Framework.Assert.IsFalse(GetTextFromPdf(tesseractReader, file, new List<String>()).Contains(expected
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestBengali() {
            String imgPath = testImagesDirectory + "bengali_01.jpeg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "ইংরজে\nশখো";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetTextPositioning
                (TextPositioning.BY_WORDS));
            // correct result with specified spanish language
            NUnit.Framework.Assert.AreEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("ben"), freeSansFontPath));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("ben")));
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("ben"), kosugiFontPath));
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, new List<String>()));
        }

        [NUnit.Framework.Test]
        public virtual void TestChinese() {
            String imgPath = testImagesDirectory + "chinese_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "你 好\nni hao";
            // correct result with specified spanish language
            NUnit.Framework.Assert.AreEqual(expected, GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("chi_sim"
                , "chi_tra"), notoSansSCFontPath));
            NUnit.Framework.Assert.AreEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("chi_sim"), notoSansSCFontPath));
            NUnit.Framework.Assert.AreEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("chi_tra"), notoSansSCFontPath));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("chi_sim")), notoSansSCFontPath);
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList
                <String>("chi_tra")), notoSansSCFontPath);
            NUnit.Framework.Assert.AreNotEqual(expected, GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("chi_sim"
                , "chi_tra")), notoSansSCFontPath);
            NUnit.Framework.Assert.IsFalse(GetTextFromPdf(tesseractReader, file, new List<String>()).Contains(expected
                ));
        }

        [NUnit.Framework.Test]
        public virtual void TestSpanishWithTessData() {
            String imgPath = testImagesDirectory + "spanish_01.jpg";
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
            String imgPath = testImagesDirectory + "bengali_01.jpeg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "ইংরজে";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPathToTessData
                (scriptTessDataDirectory));
            // correct result with specified spanish language
            NUnit.Framework.Assert.IsTrue(GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String
                >("Bengali"), freeSansFontPath).StartsWith(expected));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.IsFalse(GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String
                >("Bengali")).StartsWith(expected));
            NUnit.Framework.Assert.IsFalse(GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String
                >("Bengali"), kosugiFontPath).StartsWith(expected));
        }

        [NUnit.Framework.Test]
        public virtual void TestGeorgianTextWithScript() {
            String imgPath = testImagesDirectory + "georgian_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            // First sentence
            String expected = "ღმერთი";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPathToTessData
                (scriptTessDataDirectory));
            // correct result with specified georgian+eng language
            NUnit.Framework.Assert.IsTrue(GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String
                >("Georgian"), freeSansFontPath).StartsWith(expected));
            // incorrect result when languages are not specified
            // or languages were specified in the wrong order
            NUnit.Framework.Assert.IsFalse(GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String
                >("Georgian")).Contains(expected));
            NUnit.Framework.Assert.IsFalse(GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String
                >("Japanese")).Contains(expected));
        }

        [NUnit.Framework.Test]
        public virtual void TestJapaneseScript() {
            String imgPath = testImagesDirectory + "japanese_01.png";
            FileInfo file = new FileInfo(imgPath);
            String expected = "日 本 語\n文法";
            tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetPathToTessData
                (scriptTessDataDirectory));
            // correct result with specified japanese language
            String result = GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("Japanese"), kosugiFontPath);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void TestCustomUserWords() {
            String imgPath = testImagesDirectory + "wierdwords.png";
            IList<String> userWords = JavaUtil.ArraysAsList("he23llo", "qwetyrtyqpwe-rty");
            Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
            properties.SetLanguages(JavaUtil.ArraysAsList("fra"));
            properties.SetUserWords("fra", userWords);
            tesseractReader.SetTesseract4OcrEngineProperties(properties);
            String result = GetRecognizedTextFromTextFile(tesseractReader, imgPath);
            NUnit.Framework.Assert.IsTrue(result.Contains(userWords[0]) || result.Contains(userWords[1]));
            NUnit.Framework.Assert.IsTrue(tesseractReader.GetTesseract4OcrEngineProperties().GetPathToUserWordsFile().
                EndsWith("fra.user-words"));
        }

        [NUnit.Framework.Test]
        public virtual void TestCustomUserWordsWithListOfLanguages() {
            String imgPath = testImagesDirectory + "bogusText.jpg";
            String expectedOutput = "B1adeb1ab1a";
            Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
            properties.SetLanguages(JavaUtil.ArraysAsList("fra", "eng"));
            properties.SetUserWords("eng", JavaUtil.ArraysAsList("b1adeb1ab1a"));
            tesseractReader.SetTesseract4OcrEngineProperties(properties);
            String result = GetRecognizedTextFromTextFile(tesseractReader, imgPath);
            result = result.Replace("\n", "").Replace("\f", "");
            result = iText.IO.Util.StringUtil.ReplaceAll(result, "[^\\u0009\\u000A\\u000D\\u0020-\\u007E]", "");
            NUnit.Framework.Assert.IsTrue(result.StartsWith(expectedOutput));
            NUnit.Framework.Assert.IsTrue(tesseractReader.GetTesseract4OcrEngineProperties().GetPathToUserWordsFile().
                EndsWith("eng.user-words"));
        }

        [NUnit.Framework.Test]
        public virtual void TestUserWordsWithLanguageNotInList() {
            NUnit.Framework.Assert.That(() =>  {
                String userWords = testDocumentsDirectory + "userwords.txt";
                Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
                properties.SetUserWords("spa", new FileStream(userWords, FileMode.Open, FileAccess.Read));
                properties.SetLanguages(new List<String>());
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(Tesseract4OcrException.LANGUAGE_IS_NOT_IN_THE_LIST, "spa")))
;
        }

        [NUnit.Framework.Test]
        public virtual void TestIncorrectLanguageForUserWordsAsList() {
            NUnit.Framework.Assert.That(() =>  {
                Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
                properties.SetUserWords("eng1", JavaUtil.ArraysAsList("word1", "word2"));
                properties.SetLanguages(new List<String>());
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(Tesseract4OcrException.LANGUAGE_IS_NOT_IN_THE_LIST, "eng1")))
;
        }

        [NUnit.Framework.Test]
        public virtual void TestIncorrectLanguageForUserWordsAsInputStream() {
            NUnit.Framework.Assert.That(() =>  {
                String userWords = testDocumentsDirectory + "userwords.txt";
                Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
                properties.SetUserWords("test", new FileStream(userWords, FileMode.Open, FileAccess.Read));
                properties.SetLanguages(new List<String>());
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(Tesseract4OcrException.LANGUAGE_IS_NOT_IN_THE_LIST, "test")))
;
        }
    }
}
