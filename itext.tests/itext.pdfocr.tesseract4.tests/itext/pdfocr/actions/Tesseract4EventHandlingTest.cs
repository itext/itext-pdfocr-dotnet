/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Pdfocr;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Statistics;
using iText.Pdfocr.Tesseract4.Exceptions;
using iText.Pdfocr.Tesseract4.Logs;
using iText.Test.Attributes;

namespace iText.Pdfocr.Actions {
    public abstract class Tesseract4EventHandlingTest : IntegrationEventHandlingTestHelper {
        public Tesseract4EventHandlingTest(IntegrationTestHelper.ReaderType type)
            : base(type) {
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfFileTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = FileUtil.CreateTempFile("test", ".pdf");
            new OcrPdfCreator(tesseractReader).CreatePdfFile(JavaCollectionsUtil.SingletonList(imgFile), outPdfFile);
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

        [NUnit.Framework.Test]
        [LogMessage(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE)]
        public virtual void OcrPdfCreatorCreatePdfFileNoImageTest() {
            FileInfo imgFile = new FileInfo("unknown");
            IList<FileInfo> images = JavaCollectionsUtil.SingletonList(imgFile);
            FileInfo outPdfFile = FileUtil.CreateTempFile("test", ".pdf");
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader);
            NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => ocrPdfCreator.CreatePdfFile(images, outPdfFile
                ));
            // check ocr events
            NUnit.Framework.Assert.AreEqual(0, eventsHandler.GetEvents().Count);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfFileNoOutputFileTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            IList<FileInfo> images = JavaCollectionsUtil.SingletonList(imgFile);
            FileInfo outPdfFile = new FileInfo("no/no_file");
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader);
            NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => ocrPdfCreator.CreatePdfFile(images, outPdfFile
                ));
            NUnit.Framework.Assert.AreEqual(0, eventsHandler.GetEvents().Count);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfFileNullOutputFileTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            IList<FileInfo> images = JavaCollectionsUtil.SingletonList(imgFile);
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader);
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => ocrPdfCreator.CreatePdfFile(images, null
                ));
            NUnit.Framework.Assert.AreEqual(0, eventsHandler.GetEvents().Count);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfFileTwoImagesTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = FileUtil.CreateTempFile("test", ".pdf");
            new OcrPdfCreator(tesseractReader).CreatePdfFile(JavaUtil.ArraysAsList(imgFile, imgFile), outPdfFile);
            // check ocr events
            NUnit.Framework.Assert.AreEqual(5, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent1 = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent1, EventConfirmationType.ON_CLOSE);
            IEvent ocrUsageEvent2 = eventsHandler.GetEvents()[1];
            ValidateUsageEvent(ocrUsageEvent2, EventConfirmationType.ON_CLOSE);
            ValidateStatisticEvent(eventsHandler.GetEvents()[2], PdfOcrOutputType.PDF);
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], ocrUsageEvent1);
            ValidateConfirmEvent(eventsHandler.GetEvents()[4], ocrUsageEvent2);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetPdfOcrEvent() });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfFileTwoRunningsTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = FileUtil.CreateTempFile("test", ".pdf");
            new OcrPdfCreator(tesseractReader).CreatePdfFile(JavaCollectionsUtil.SingletonList(imgFile), outPdfFile);
            new OcrPdfCreator(tesseractReader).CreatePdfFile(JavaCollectionsUtil.SingletonList(imgFile), outPdfFile);
            NUnit.Framework.Assert.AreEqual(6, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.PDF);
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], ocrUsageEvent);
            // usage event
            ocrUsageEvent = eventsHandler.GetEvents()[3];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            ValidateStatisticEvent(eventsHandler.GetEvents()[4], PdfOcrOutputType.PDF);
            ValidateConfirmEvent(eventsHandler.GetEvents()[5], ocrUsageEvent);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetPdfOcrEvent() });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = FileUtil.CreateTempFile("test", ".pdf");
            PdfWriter pdfWriter = new PdfWriter(outPdfFile);
            PdfDocument pdfDocument = new OcrPdfCreator(tesseractReader).CreatePdf(JavaCollectionsUtil.SingletonList(imgFile
                ), pdfWriter);
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.PDF);
            ValidateCoreConfirmEvent(eventsHandler.GetEvents()[2]);
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], ocrUsageEvent);
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetCoreEvent(), GetPdfOcrEvent
                () });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        [LogMessage(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE)]
        public virtual void OcrPdfCreatorCreatePdfNoImageTest() {
            IList<FileInfo> images = JavaCollectionsUtil.SingletonList(new FileInfo("no_image"));
            FileInfo outPdfFile = FileUtil.CreateTempFile("test", ".pdf");
            PdfWriter pdfWriter = new PdfWriter(outPdfFile);
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader);
            NUnit.Framework.Assert.Catch(typeof(PdfOcrTesseract4Exception), () => ocrPdfCreator.CreatePdf(images, pdfWriter
                ));
            pdfWriter.Dispose();
            NUnit.Framework.Assert.AreEqual(0, eventsHandler.GetEvents().Count);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfNullWriterTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            IList<FileInfo> images = JavaCollectionsUtil.SingletonList(imgFile);
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader);
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => ocrPdfCreator.CreatePdf(images, null));
            NUnit.Framework.Assert.AreEqual(1, eventsHandler.GetEvents().Count);
            ValidateUsageEvent(eventsHandler.GetEvents()[0], EventConfirmationType.ON_CLOSE);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfAFileTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = FileUtil.CreateTempFile("test", ".pdf");
            OcrPdfCreatorProperties props = new OcrPdfCreatorProperties().SetPdfLang("en-US");
            new OcrPdfCreator(tesseractReader, props).CreatePdfAFile(JavaCollectionsUtil.SingletonList(imgFile), outPdfFile
                , GetRGBPdfOutputIntent());
            // check ocr events
            NUnit.Framework.Assert.AreEqual(3, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.PDFA);
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], ocrUsageEvent);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetPdfOcrEvent() });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfATest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = FileUtil.CreateTempFile("test", ".pdf");
            PdfWriter pdfWriter = new PdfWriter(outPdfFile);
            OcrPdfCreatorProperties props = new OcrPdfCreatorProperties().SetPdfLang("en-US");
            PdfDocument pdfDocument = new OcrPdfCreator(tesseractReader, props).CreatePdfA(JavaCollectionsUtil.SingletonList
                (imgFile), pdfWriter, GetRGBPdfOutputIntent());
            pdfDocument.Close();
            // check ocr events
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.PDFA);
            ValidateCoreConfirmEvent(eventsHandler.GetEvents()[2]);
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], ocrUsageEvent);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetCoreEvent(), GetPdfOcrEvent
                () });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        public virtual void DoImageOcrTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            tesseractReader.DoImageOcr(imgFile);
            NUnit.Framework.Assert.AreEqual(3, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.DATA);
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], usageEvent);
        }

        [NUnit.Framework.Test]
        [LogMessage(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE)]
        public virtual void DoImageOcrNoImageTest() {
            FileInfo imgFile = new FileInfo("uncknown");
            NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => tesseractReader.DoImageOcr(imgFile));
            NUnit.Framework.Assert.AreEqual(0, eventsHandler.GetEvents().Count);
        }

        [NUnit.Framework.Test]
        public virtual void DoImageOcrTwoRunningsTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            tesseractReader.DoImageOcr(imgFile);
            tesseractReader.DoImageOcr(imgFile);
            NUnit.Framework.Assert.AreEqual(6, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.DATA);
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], usageEvent);
            usageEvent = eventsHandler.GetEvents()[3];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            ValidateStatisticEvent(eventsHandler.GetEvents()[4], PdfOcrOutputType.DATA);
            ValidateConfirmEvent(eventsHandler.GetEvents()[5], usageEvent);
        }

        [NUnit.Framework.Test]
        public virtual void CreateTxtFileTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            tesseractReader.CreateTxtFile(JavaUtil.ArraysAsList(imgFile, imgFile), FileUtil.CreateTempFile("test", ".txt"
                ));
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.DATA);
            ValidateStatisticEvent(eventsHandler.GetEvents()[2], PdfOcrOutputType.DATA);
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], usageEvent);
        }

        [NUnit.Framework.Test]
        public virtual void CreateTxtFileNullEventHelperTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            tesseractReader.CreateTxtFile(JavaUtil.ArraysAsList(imgFile, imgFile), FileUtil.CreateTempFile("test", ".txt"
                ), new OcrProcessContext(null));
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.DATA);
            ValidateStatisticEvent(eventsHandler.GetEvents()[2], PdfOcrOutputType.DATA);
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], usageEvent);
        }

        [NUnit.Framework.Test]
        [LogMessage(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE)]
        public virtual void CreateTxtFileNoImageTest() {
            FileInfo imgFile = new FileInfo("no_image");
            IList<FileInfo> images = JavaUtil.ArraysAsList(imgFile, imgFile);
            FileInfo outPdfFile = FileUtil.CreateTempFile("test", ".txt");
            NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => tesseractReader.CreateTxtFile(images, outPdfFile
                ));
            // only one usage event is expected and it is not confirmed (no confirm event
            NUnit.Framework.Assert.AreEqual(1, eventsHandler.GetEvents().Count);
            ValidateUsageEvent(eventsHandler.GetEvents()[0], EventConfirmationType.ON_DEMAND);
        }

        [NUnit.Framework.Test]
        public virtual void CreateTxtFileNoFileTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            IList<FileInfo> images = JavaUtil.ArraysAsList(imgFile, imgFile);
            FileInfo outPdfFile = new FileInfo("nopath/nofile");
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfOcrTesseract4Exception), () => tesseractReader.CreateTxtFile
                (images, outPdfFile));
            NUnit.Framework.Assert.AreEqual(PdfOcrTesseract4ExceptionMessageConstant.CANNOT_WRITE_TO_FILE, e.Message);
            NUnit.Framework.Assert.AreEqual(3, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.DATA);
            ValidateStatisticEvent(eventsHandler.GetEvents()[2], PdfOcrOutputType.DATA);
        }

        [NUnit.Framework.Test]
        public virtual void CreateTxtFileNullOutFileTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            IList<FileInfo> images = JavaUtil.ArraysAsList(imgFile, imgFile);
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => tesseractReader.CreateTxtFile(images, null
                ));
            NUnit.Framework.Assert.AreEqual(3, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.DATA);
            ValidateStatisticEvent(eventsHandler.GetEvents()[2], PdfOcrOutputType.DATA);
        }

        // set meta info tests
        [NUnit.Framework.Test]
        public virtual void SetEventCountingMetaInfoTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = FileUtil.CreateTempFile("test", ".pdf");
            CreatePdfAndSetEventCountingMetaInfo(tesseractReader, outPdfFile, imgFile, new Tesseract4EventHandlingTest.TestMetaInfo
                ());
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.PDF);
            ValidateCoreConfirmEvent(eventsHandler.GetEvents()[2]);
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], ocrUsageEvent);
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetCoreEvent(), GetPdfOcrEvent
                () });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfFileTestMetaInfoTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = FileUtil.CreateTempFile("test", ".pdf");
            CreatePdfFileAndSetMetaInfoToProps(tesseractReader, outPdfFile, imgFile, new Tesseract4EventHandlingTest.TestMetaInfo
                ());
            // check ocr events
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.PDF);
            ValidateCoreConfirmEvent(eventsHandler.GetEvents()[2]);
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], ocrUsageEvent);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetCoreEvent(), GetPdfOcrEvent
                () });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        public virtual void DoImageOcrCustomEventHelperTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            tesseractReader.DoImageOcr(imgFile, new OcrProcessContext(new Tesseract4EventHandlingTest.CustomEventHelper
                ()));
            NUnit.Framework.Assert.AreEqual(3, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.DATA);
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], usageEvent);
        }

        [NUnit.Framework.Test]
        public virtual void CreateTxtFileCustomEventHelperTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            tesseractReader.CreateTxtFile(JavaUtil.ArraysAsList(imgFile, imgFile), FileUtil.CreateTempFile("test", ".txt"
                ), new OcrProcessContext(new Tesseract4EventHandlingTest.CustomEventHelper()));
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.DATA);
            ValidateStatisticEvent(eventsHandler.GetEvents()[2], PdfOcrOutputType.DATA);
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], usageEvent);
        }

        private class CustomEventHelper : AbstractPdfOcrEventHelper {
            public override void OnEvent(AbstractProductITextEvent @event) {
                if (@event is AbstractContextBasedITextEvent) {
                    ((AbstractContextBasedITextEvent)@event).SetMetaInfo(new Tesseract4EventHandlingTest.TestMetaInfo());
                }
                EventManager.GetInstance().OnEvent(@event);
            }

            public override SequenceId GetSequenceId() {
                return new SequenceId();
            }

            public override EventConfirmationType GetConfirmationType() {
                return EventConfirmationType.ON_DEMAND;
            }
        }

        private class TestMetaInfo : IMetaInfo {
        }
    }
}
