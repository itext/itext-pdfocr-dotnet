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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4.Exceptions;

namespace iText.Pdfocr.Tesseract4 {
    public abstract class UserWordsTest : IntegrationTestHelper {
        internal AbstractTesseract4OcrEngine tesseractReader;

        internal String testFileTypeName;

        private bool isExecutableReaderType;

        public UserWordsTest(IntegrationTestHelper.ReaderType type) {
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
        public virtual void TestCustomUserWords() {
            String imgPath = TEST_IMAGES_DIRECTORY + "wierdwords.png";
            IList<String> userWords = JavaUtil.ArraysAsList("he23llo", "qwetyrtyqpwe-rty");
            Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
            properties.SetLanguages(JavaUtil.ArraysAsList("fra"));
            properties.SetUserWords("fra", userWords);
            tesseractReader.SetTesseract4OcrEngineProperties(properties);
            String result = GetRecognizedTextFromTextFile(tesseractReader, imgPath);
            NUnit.Framework.Assert.IsTrue(result.Contains(userWords[0]) || result.Contains(userWords[1]));
            NUnit.Framework.Assert.IsTrue(tesseractReader.GetTesseract4OcrEngineProperties().GetPathToUserWordsFile().
                EndsWith(".user-words"));
        }

        [NUnit.Framework.Test]
        public virtual void TestCustomUserWordsWithListOfLanguages() {
            String imgPath = TEST_IMAGES_DIRECTORY + "bogusText.jpg";
            String expectedOutput = "B1adeb1ab1a";
            Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
            properties.SetLanguages(JavaUtil.ArraysAsList("fra", "eng"));
            properties.SetUserWords("eng", JavaUtil.ArraysAsList("b1adeb1ab1a"));
            tesseractReader.SetTesseract4OcrEngineProperties(properties);
            String result = GetRecognizedTextFromTextFile(tesseractReader, imgPath);
            result = result.Replace("\n", "").Replace("\f", "");
            result = iText.Commons.Utils.StringUtil.ReplaceAll(result, "[^\\u0009\\u000A\\u000D\\u0020-\\u007E]", "");
            NUnit.Framework.Assert.IsTrue(result.StartsWith(expectedOutput));
            NUnit.Framework.Assert.IsTrue(tesseractReader.GetTesseract4OcrEngineProperties().GetPathToUserWordsFile().
                EndsWith(".user-words"));
        }

        [NUnit.Framework.Test]
        public virtual void TestUserWordsWithLanguageNotInList() {
            NUnit.Framework.Assert.That(() =>  {
                String userWords = TEST_DOCUMENTS_DIRECTORY + "userwords.txt";
                Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
                properties.SetUserWords("spa", new FileStream(userWords, FileMode.Open, FileAccess.Read));
                properties.SetLanguages(new List<String>());
            }
            , NUnit.Framework.Throws.InstanceOf<PdfOcrTesseract4Exception>().With.Message.EqualTo(MessageFormatUtil.Format(PdfOcrTesseract4ExceptionMessageConstant.LANGUAGE_IS_NOT_IN_THE_LIST, "spa")))
;
        }

        [NUnit.Framework.Test]
        public virtual void TestIncorrectLanguageForUserWordsAsList() {
            NUnit.Framework.Assert.That(() =>  {
                Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
                properties.SetUserWords("eng1", JavaUtil.ArraysAsList("word1", "word2"));
                properties.SetLanguages(new List<String>());
            }
            , NUnit.Framework.Throws.InstanceOf<PdfOcrTesseract4Exception>().With.Message.EqualTo(MessageFormatUtil.Format(PdfOcrTesseract4ExceptionMessageConstant.LANGUAGE_IS_NOT_IN_THE_LIST, "eng1")))
;
        }

        [NUnit.Framework.Test]
        public virtual void TestUserWordsWithDefaultLanguageNotInList() {
            String userWords = TEST_DOCUMENTS_DIRECTORY + "userwords.txt";
            Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
            properties.SetUserWords("eng", new FileStream(userWords, FileMode.Open, FileAccess.Read));
            properties.SetLanguages(new List<String>());
            tesseractReader.SetTesseract4OcrEngineProperties(properties);
            String imgPath = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            String expectedOutput = "619121";
            String result = GetRecognizedTextFromTextFile(tesseractReader, imgPath);
            NUnit.Framework.Assert.IsTrue(result.StartsWith(expectedOutput));
        }

        [NUnit.Framework.Test]
        public virtual void TestUserWordsFileNotDeleted() {
            String userWords = TEST_DOCUMENTS_DIRECTORY + "userwords.txt";
            Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
            properties.SetPathToUserWordsFile(userWords);
            properties.SetLanguages(JavaUtil.ArraysAsList("eng"));
            tesseractReader.SetTesseract4OcrEngineProperties(properties);
            String imgPath = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            tesseractReader.DoImageOcr(new FileInfo(imgPath));
            NUnit.Framework.Assert.IsTrue(new FileInfo(userWords).Exists);
        }
    }
}
