/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using iText.Pdfocr;

namespace iText.Pdfocr.Tesseract4 {
    [NUnit.Framework.Category("IntegrationTest")]
    public class Tesseract4MetaInfoEventHandlingLibTest : Tesseract4MetaInfoEventHandlingTest {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfocr/tesseract4/Tesseract4MetaInfoEventHandlingLibTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeTests() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        public Tesseract4MetaInfoEventHandlingLibTest()
            : base(IntegrationTestHelper.ReaderType.LIB, DESTINATION_FOLDER) {
        }
    }
}
