using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Util;
using iText.Test.Attributes;

namespace iText.Ocr {
    public class TesseractExecutableIntegrationTest : AbstractIntegrationTest {
        [LogMessage(OCRException.INCORRECT_LANGUAGE, Count = 2)]
        [NUnit.Framework.Test]
        public virtual void TestIncorrectLanguages() {
            FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
            TesseractExecutableReader tesseractReader = new TesseractExecutableReader(GetTesseractDirectory(), GetTessDataDirectory
                ());
            tesseractReader.SetPathToExecutable(GetTesseractDirectory());
            tesseractReader.SetPathToScript(GetPathToHocrScript());
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
            NUnit.Framework.Assert.AreEqual(GetTesseractDirectory(), tesseractReader.GetPathToExecutable());
            NUnit.Framework.Assert.AreEqual(GetPathToHocrScript(), tesseractReader.GetPathToScript());
        }

        [LogMessage(OCRException.INCORRECT_LANGUAGE, Count = 3)]
        [NUnit.Framework.Test]
        public virtual void TestIncorrectLanguagesScripts() {
            FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
            TesseractExecutableReader tesseractReader = new TesseractExecutableReader(GetTesseractDirectory(), GetTessDataDirectory
                ());
            tesseractReader.SetPathToScript(GetPathToHocrScript());
            tesseractReader.SetPathToTessData(scriptTessDataDirectory);
            try {
                GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("English"));
            }
            catch (OCRException e) {
                String expectedMsg = String.Format(OCRException.INCORRECT_LANGUAGE, "English.traineddata", scriptTessDataDirectory
                    );
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
            }
            try {
                tesseractReader.SetPathToTessData(scriptTessDataDirectory);
                GetTextFromPdf(tesseractReader, file, JavaUtil.ArraysAsList("Georgian", "Japanese", "English"));
            }
            catch (OCRException e) {
                String expectedMsg = String.Format(OCRException.INCORRECT_LANGUAGE, "English.traineddata", scriptTessDataDirectory
                    );
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
            }
            try {
                tesseractReader.SetPathToTessData(scriptTessDataDirectory);
                GetTextFromPdf(tesseractReader, file, new List<String>());
            }
            catch (OCRException e) {
                String expectedMsg = String.Format(OCRException.INCORRECT_LANGUAGE, "eng.traineddata", scriptTessDataDirectory
                    );
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
                tesseractReader.SetPathToTessData(GetTessDataDirectory());
            }
            tesseractReader.SetPathToTessData(GetTessDataDirectory());
            NUnit.Framework.Assert.AreEqual(GetTesseractDirectory(), tesseractReader.GetPathToExecutable());
            NUnit.Framework.Assert.AreEqual(GetPathToHocrScript(), tesseractReader.GetPathToScript());
        }

        [LogMessage(OCRException.CANNOT_READ_SPECIFIED_INPUT_IMAGE, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestCorruptedImageAndCatchException() {
            FileInfo file = new FileInfo(testImagesDirectory + "corrupted.jpg");
            try {
                TesseractExecutableReader tesseractReader = new TesseractExecutableReader(GetTesseractDirectory(), GetTessDataDirectory
                    ());
                String realOutput = GetTextFromPdf(tesseractReader, file);
                NUnit.Framework.Assert.IsNotNull(realOutput);
                NUnit.Framework.Assert.AreEqual("", realOutput);
            }
            catch (OCRException e) {
                String expectedMsg = String.Format(OCRException.CANNOT_READ_SPECIFIED_INPUT_IMAGE, file.FullName);
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
            }
        }

        [LogMessage(OCRException.CANNOT_FIND_PATH_TO_TESSDATA, Count = 1)]
        [LogMessage(OCRException.INCORRECT_LANGUAGE, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestIncorrectPathToTessData() {
            FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
            TesseractExecutableReader tesseractReader = new TesseractExecutableReader(GetTesseractDirectory(), "", JavaCollectionsUtil
                .SingletonList<String>("eng"));
            try {
                NUnit.Framework.Assert.AreEqual("", tesseractReader.GetPathToTessData());
                GetTextFromPdf(tesseractReader, file);
            }
            catch (OCRException e) {
                NUnit.Framework.Assert.AreEqual(OCRException.CANNOT_FIND_PATH_TO_TESSDATA, e.Message);
            }
            tesseractReader.SetPathToTessData(GetTessDataDirectory());
            try {
                tesseractReader.SetPathToTessData("/test");
                GetTextFromPdf(tesseractReader, file, JavaCollectionsUtil.SingletonList<String>("eng"));
            }
            catch (OCRException e) {
                String expectedMsg = String.Format(OCRException.INCORRECT_LANGUAGE, "eng.traineddata", "/test");
                NUnit.Framework.Assert.AreEqual(expectedMsg, e.Message);
            }
            tesseractReader.SetPathToTessData(GetTessDataDirectory());
        }

        [LogMessage(OCRException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE, Count = 2)]
        [NUnit.Framework.Test]
        public virtual void TestIncorrectPathToTesseractExecutable() {
            FileInfo file = new FileInfo(testImagesDirectory + "spanish_01.jpg");
            try {
                GetTextFromPdf(new TesseractExecutableReader(null, null), file);
            }
            catch (OCRException e) {
                NUnit.Framework.Assert.AreEqual(OCRException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE, e.Message);
            }
            try {
                GetTextFromPdf(new TesseractExecutableReader("", ""), file);
            }
            catch (OCRException e) {
                NUnit.Framework.Assert.AreEqual(OCRException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE, e.Message);
            }
        }

        [LogMessage(OCRException.TESSERACT_FAILED, Count = 2)]
        [NUnit.Framework.Test]
        public virtual void TestRunningTesseractCmd() {
            bool catched = false;
            try {
                TesseractUtil.RunCommand(JavaUtil.ArraysAsList("tesseract", "random.jpg"), false);
            }
            catch (OCRException) {
                catched = true;
            }
            NUnit.Framework.Assert.IsTrue(catched);
            catched = false;
            try {
                TesseractUtil.RunCommand(null, false);
            }
            catch (OCRException) {
                catched = true;
            }
            NUnit.Framework.Assert.IsTrue(catched);
        }
    }
}
