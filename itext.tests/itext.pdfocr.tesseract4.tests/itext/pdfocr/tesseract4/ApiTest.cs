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
using iText.IO.Util;
using iText.Pdfocr;
using iText.Test.Attributes;

namespace iText.Pdfocr.Tesseract4 {
    public class ApiTest : IntegrationTestHelper {
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
                Tesseract4ExecutableOcrEngine engine = new Tesseract4ExecutableOcrEngine(new Tesseract4OcrEngineProperties
                    ());
                engine.DoImageOcr(imgFile);
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(Tesseract4OcrException.PATH_TO_TESS_DATA_IS_NOT_SET))
;
        }

        [LogMessage(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE, Count = 2)]
        [NUnit.Framework.Test]
        public virtual void TestDoTesseractOcrForIncorrectImageForExecutable() {
            NUnit.Framework.Assert.That(() =>  {
                String path = TEST_IMAGES_DIRECTORY + "numbers_01";
                FileInfo imgFile = new FileInfo(path);
                Tesseract4ExecutableOcrEngine engine = new Tesseract4ExecutableOcrEngine(new Tesseract4OcrEngineProperties
                    ().SetPathToTessData(GetTessDataDirectory()));
                engine.DoTesseractOcr(imgFile, null, OutputFormat.HOCR);
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(Tesseract4OcrException.CANNOT_READ_PROVIDED_IMAGE, new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01").FullName)))
;
        }

        [LogMessage(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE)]
        [LogMessage(Tesseract4OcrException.TESSERACT_FAILED)]
        [LogMessage(Tesseract4LogMessageConstant.TESSERACT_FAILED)]
        [NUnit.Framework.Test]
        public virtual void TestOcrResultForSinglePageForNullImage() {
            NUnit.Framework.Assert.That(() =>  {
                Tesseract4LibOcrEngine tesseract4LibOcrEngine = GetTesseract4LibOcrEngine();
                tesseract4LibOcrEngine.SetTesseract4OcrEngineProperties(new Tesseract4OcrEngineProperties().SetPathToTessData
                    (GetTessDataDirectory()));
                tesseract4LibOcrEngine.InitializeTesseract(OutputFormat.TXT);
                tesseract4LibOcrEngine.DoTesseractOcr(null, null, OutputFormat.HOCR);
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(Tesseract4OcrException.TESSERACT_FAILED))
;
        }

        [NUnit.Framework.Test]
        public virtual void TestDoTesseractOcrForNonAsciiPathForExecutable() {
            String path = TEST_IMAGES_DIRECTORY + "tèst/noisy_01.png";
            FileInfo imgFile = new FileInfo(path);
            FileInfo outputFile = new FileInfo(TesseractOcrUtil.GetTempFilePath("test", ".hocr"));
            Tesseract4OcrEngineProperties properties = new Tesseract4OcrEngineProperties();
            properties.SetPathToTessData(GetTessDataDirectory());
            properties.SetPreprocessingImages(false);
            Tesseract4ExecutableOcrEngine engine = new Tesseract4ExecutableOcrEngine(properties);
            engine.DoTesseractOcr(imgFile, outputFile, OutputFormat.HOCR);
            NUnit.Framework.Assert.IsTrue(File.Exists(System.IO.Path.Combine(outputFile.FullName)));
            TesseractHelper.DeleteFile(outputFile.FullName);
            NUnit.Framework.Assert.IsFalse(File.Exists(System.IO.Path.Combine(outputFile.FullName)));
        }

        [LogMessage(Tesseract4LogMessageConstant.CANNOT_PARSE_NODE_BBOX, Count = 4)]
        [NUnit.Framework.Test]
        public virtual void TestDetectAndFixBrokenBBoxes() {
            FileInfo hocrFile = new FileInfo(TEST_DOCUMENTS_DIRECTORY + "broken_bboxes.hocr");
            IDictionary<int, IList<TextInfo>> parsedHocr = TesseractHelper.ParseHocrFile(JavaCollectionsUtil.SingletonList
                (hocrFile), TextPositioning.BY_WORDS_AND_LINES);
            TextInfo textInfo = parsedHocr.Get(1)[1];
            NUnit.Framework.Assert.AreEqual(287.25, (float)textInfo.GetBboxRect().GetLeft(), 0.1);
            NUnit.Framework.Assert.AreEqual(136.5f, (float)textInfo.GetBboxRect().GetBottom(), 0.1);
            NUnit.Framework.Assert.AreEqual(385.5, (float)textInfo.GetBboxRect().GetRight(), 0.1);
            NUnit.Framework.Assert.AreEqual(162.75, (float)textInfo.GetBboxRect().GetTop(), 0.1);
            NUnit.Framework.Assert.AreEqual(383.0f, (float)textInfo.GetBbox()[0], 0.1);
            NUnit.Framework.Assert.AreEqual(101.0f, (float)textInfo.GetBbox()[1], 0.1);
            NUnit.Framework.Assert.AreEqual(514.0f, (float)textInfo.GetBbox()[2], 0.1);
            NUnit.Framework.Assert.AreEqual(136.0f, (float)textInfo.GetBbox()[3], 0.1);
        }
    }
}
