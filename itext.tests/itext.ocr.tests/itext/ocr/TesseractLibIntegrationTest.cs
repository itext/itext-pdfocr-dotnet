using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Util;
using iText.Test.Attributes;

namespace iText.Ocr {
    public class TesseractLibIntegrationTest : AbstractIntegrationTest {
        [LogMessage(OCRException.INCORRECT_LANGUAGE, Count = 2)]
        [NUnit.Framework.Test]
        public virtual void TestIncorrectLanguages() {
            FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
            TesseractReader tesseractReader = new TesseractLibReader(GetTessDataDirectory(), JavaCollectionsUtil.SingletonList
                <String>("spa"));
            try {
                GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("spa", "spa_new", "spa_old"));
            }
            catch (OCRException e) {
                String expectedMsg = String.Format(OCRException.INCORRECT_LANGUAGE, "spa_new.traineddata", langTessDataDirectory
                    );
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
            }
            try {
                GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("spa_new"));
            }
            catch (OCRException e) {
                String expectedMsg = String.Format(OCRException.INCORRECT_LANGUAGE, "spa_new.traineddata", langTessDataDirectory
                    );
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
            }
        }

        [LogMessage(OCRException.INCORRECT_LANGUAGE, Count = 3)]
        [NUnit.Framework.Test]
        public virtual void TestIncorrectLanguagesScript() {
            FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
            TesseractReader tesseractReader = new TesseractLibReader(scriptTessDataDirectory, JavaCollectionsUtil.SingletonList
                <String>("spa"));
            try {
                GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("English"));
            }
            catch (OCRException e) {
                String expectedMsg = String.Format(OCRException.INCORRECT_LANGUAGE, "English.traineddata", scriptTessDataDirectory
                    );
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
            }
            try {
                GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("Georgian", "Japanese", "English"));
            }
            catch (OCRException e) {
                String expectedMsg = String.Format(OCRException.INCORRECT_LANGUAGE, "English.traineddata", scriptTessDataDirectory
                    );
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
            }
            try {
                GetTextFromPdf(tesseractReader, file, new List<String>());
            }
            catch (OCRException e) {
                String expectedMsg = String.Format(OCRException.INCORRECT_LANGUAGE, "eng.traineddata", scriptTessDataDirectory
                    );
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
                tesseractReader.SetPathToTessData(GetTessDataDirectory());
            }
        }

        [LogMessage(OCRException.CANNOT_READ_SPECIFIED_INPUT_IMAGE, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestCorruptedImageAndCatchException() {
            FileInfo file = new FileInfo(testImagesDirectory + "corrupted.jpg");
            try {
                TesseractReader tesseractReader = new TesseractLibReader(GetTessDataDirectory());
                String realOutput = GetTextFromPdf(tesseractReader, file);
                NUnit.Framework.Assert.IsNotNull(realOutput);
                NUnit.Framework.Assert.AreEqual("", realOutput);
            }
            catch (OCRException e) {
                String expectedMsg = String.Format(OCRException.CANNOT_READ_SPECIFIED_INPUT_IMAGE, file.FullName);
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestTextFromJPE() {
            String path = testImagesDirectory + "numbers_01.jpe";
            String expectedOutput = "619121";
            TesseractReader tesseractReader = new TesseractLibReader(GetTessDataDirectory());
            String realOutputHocr = GetTextFromPdf(tesseractReader, new FileInfo(path));
            NUnit.Framework.Assert.IsTrue(realOutputHocr.Contains(expectedOutput));
        }
    }
}
