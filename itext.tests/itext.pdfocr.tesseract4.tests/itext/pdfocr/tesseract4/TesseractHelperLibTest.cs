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

namespace iText.Pdfocr.Tesseract4 {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TesseractHelperLibTest : TesseractHelperTest {
        public TesseractHelperLibTest()
            : base(IntegrationTestHelper.ReaderType.LIB) {
        }

#if !NETSTANDARD2_0
        [NUnit.Framework.Timeout(60000)]
#endif // !NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void HocrOutputFromHalftoneFile() {
            String path = TEST_IMAGES_DIRECTORY + "halftone.jpg";
            String expected01 = "Silliness";
            String expected02 = "Enablers";
            String expected03 = "You";
            String expected04 = "Middle";
            String expected05 = "André";
            String expected06 = "QUANTITY";
            String expected07 = "DESCRIPTION";
            String expected08 = "Silliness Enablers";
            String expected09 = "QUANTITY DESCRIPTION UNIT PRICE TOTAL";
            FileInfo imgFile = new FileInfo(path);
            FileInfo outputFile = new FileInfo(GetTargetDirectory() + "hocrOutputFromHalftoneFile.hocr");
            tesseractReader.DoTesseractOcr(imgFile, outputFile, OutputFormat.HOCR);
            IDictionary<int, IList<TextInfo>> pageData = TesseractHelper.ParseHocrFile(JavaCollectionsUtil.SingletonList
                <FileInfo>(outputFile), null, new Tesseract4OcrEngineProperties().SetTextPositioning(TextPositioning.BY_WORDS
                ));
            NUnit.Framework.Assert.IsTrue(FindTextInPageData(pageData, 1, expected01));
            NUnit.Framework.Assert.IsTrue(FindTextInPageData(pageData, 1, expected02));
            NUnit.Framework.Assert.IsTrue(FindTextInPageData(pageData, 1, expected03));
            NUnit.Framework.Assert.IsTrue(FindTextInPageData(pageData, 1, expected04));
            NUnit.Framework.Assert.IsTrue(FindTextInPageData(pageData, 1, expected05));
            NUnit.Framework.Assert.IsTrue(FindTextInPageData(pageData, 1, expected06));
            NUnit.Framework.Assert.IsTrue(FindTextInPageData(pageData, 1, expected07));
            pageData = TesseractHelper.ParseHocrFile(JavaCollectionsUtil.SingletonList<FileInfo>(outputFile), null, new 
                Tesseract4OcrEngineProperties().SetTextPositioning(TextPositioning.BY_LINES));
            NUnit.Framework.Assert.IsTrue(FindTextInPageData(pageData, 1, expected08));
            NUnit.Framework.Assert.IsTrue(FindTextInPageData(pageData, 1, expected09));
        }

        /// <summary>Searches for certain text in page data.</summary>
        private bool FindTextInPageData(IDictionary<int, IList<TextInfo>> pageData, int page, String textToSearchFor
            ) {
            foreach (TextInfo textInfo in pageData.Get(page)) {
                if (textToSearchFor.Equals(textInfo.GetText())) {
                    return true;
                }
            }
            return false;
        }
    }
}
