/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using System.IO;
using iText.IO.Util;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4;
using iText.Test.Attributes;

namespace iText.Pdfocr.Events {
    public class EventCountingExecutableTest : EventCountingTest {
        public EventCountingExecutableTest()
            : base(IntegrationTestHelper.ReaderType.EXECUTABLE) {
        }

        [NUnit.Framework.Test]
        [LogMessage(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE)]
        public override void TestEventCountingCustomMetaInfoError() {
            String imgPath = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_101.jpg").FullName;
            NUnit.Framework.Assert.That(() =>  {
                base.TestEventCountingCustomMetaInfoError();
            }
            , NUnit.Framework.Throws.InstanceOf<Tesseract4OcrException>().With.Message.EqualTo(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE, imgPath)))
;
        }
    }
}
