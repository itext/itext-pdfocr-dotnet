/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using System.Text;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Pdfocr;

namespace iText.Pdfocr.Tesseract4 {
    public abstract class TesseractHelperTest : IntegrationTestHelper {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Pdfocr.Tesseract4.TesseractHelperTest
            ));

//\cond DO_NOT_DOCUMENT
        internal AbstractTesseract4OcrEngine tesseractReader;
//\endcond

//\cond DO_NOT_DOCUMENT
        internal String testFileTypeName;
//\endcond

        private bool isExecutableReaderType;

        public TesseractHelperTest(IntegrationTestHelper.ReaderType type) {
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
        public virtual void TestTesseract4OcrForOnePageWithHocrFormat() {
            String path = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            String expected = "619121";
            FileInfo imgFile = new FileInfo(path);
            FileInfo outputFile = new FileInfo(GetTargetDirectory() + "testTesseract4OcrForOnePage.hocr");
            tesseractReader.DoTesseractOcr(imgFile, outputFile, OutputFormat.HOCR);
            IDictionary<int, IList<TextInfo>> pageData = TesseractHelper.ParseHocrFile(JavaCollectionsUtil.SingletonList
                <FileInfo>(outputFile), null, tesseractReader.GetTesseract4OcrEngineProperties());
            String result = GetTextFromPage(pageData.Get(1));
            NUnit.Framework.Assert.AreEqual(expected, result.Trim());
        }

        /// <summary>Concatenates provided text items to one string.</summary>
        private String GetTextFromPage(IList<TextInfo> pageText) {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TextInfo text in pageText) {
                stringBuilder.Append(text.GetText());
                stringBuilder.Append(" ");
            }
            return stringBuilder.ToString().Trim();
        }
    }
}
