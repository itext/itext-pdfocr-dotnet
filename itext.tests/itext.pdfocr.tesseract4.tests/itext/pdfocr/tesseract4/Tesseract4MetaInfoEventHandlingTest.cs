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
using System.IO;
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Contexts;
using iText.Commons.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Statistics;

namespace iText.Pdfocr.Tesseract4 {
    public abstract class Tesseract4MetaInfoEventHandlingTest : IntegrationEventHandlingTestHelper {
        public Tesseract4MetaInfoEventHandlingTest(IntegrationTestHelper.ReaderType type)
            : base(type) {
        }

        // set meta info tests
        [NUnit.Framework.Test]
        public virtual void SetEventCountingMetaInfoTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = FileUtil.CreateTempFile("test", ".pdf");
            CreatePdfAndSetEventCountingMetaInfo(tesseractReader, outPdfFile, imgFile, new Tesseract4MetaInfoEventHandlingTest.TestMetaInfo
                ());
            NUnit.Framework.Assert.AreEqual(3, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.PDF);
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], ocrUsageEvent);
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetPdfOcrEvent() });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfFileTestMetaInfoTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = FileUtil.CreateTempFile("test", ".pdf");
            CreatePdfFileAndSetMetaInfoToProps(tesseractReader, outPdfFile, imgFile, new Tesseract4MetaInfoEventHandlingTest.TestMetaInfo
                ());
            // check ocr events
            NUnit.Framework.Assert.AreEqual(3, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.PDF);
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], ocrUsageEvent);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetPdfOcrEvent() });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        private class TestMetaInfo : IMetaInfo {
        }
    }
}
