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
using System.IO;
using iText.Pdfocr.Tesseract4;
using iText.Test.Attributes;

namespace iText.Pdfocr {
    public class TesseractExecutableIntegrationTest : AbstractIntegrationTest {
        [LogMessage(Tesseract4OcrException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestNullPathToTesseractExecutable() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(TEST_IMAGES_DIRECTORY + "spanish_01.jpg");
                Tesseract4ExecutableOcrEngine tesseractExecutableReader = new Tesseract4ExecutableOcrEngine(new Tesseract4OcrEngineProperties
                    ());
                tesseractExecutableReader.SetPathToExecutable(null);
                GetTextFromPdf(tesseractExecutableReader, file);
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(Tesseract4OcrException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE))
;
        }

        [LogMessage(Tesseract4OcrException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestEmptyPathToTesseractExecutable() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(TEST_IMAGES_DIRECTORY + "spanish_01.jpg");
                GetTextFromPdf(new Tesseract4ExecutableOcrEngine("", new Tesseract4OcrEngineProperties()), file);
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(Tesseract4OcrException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE))
;
        }

        [LogMessage(Tesseract4LogMessageConstant.COMMAND_FAILED, Count = 1)]
        [LogMessage(Tesseract4OcrException.TESSERACT_NOT_FOUND, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestIncorrectPathToTesseractExecutable() {
            NUnit.Framework.Assert.That(() =>  {
                FileInfo file = new FileInfo(TEST_IMAGES_DIRECTORY + "spanish_01.jpg");
                GetTextFromPdf(new Tesseract4ExecutableOcrEngine("path\\to\\executable\\", new Tesseract4OcrEngineProperties
                    ()), file);
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(Tesseract4OcrException.TESSERACT_NOT_FOUND))
;
        }
    }
}
