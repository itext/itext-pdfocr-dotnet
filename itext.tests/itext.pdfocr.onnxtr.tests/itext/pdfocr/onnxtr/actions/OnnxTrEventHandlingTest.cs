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
using iText.Pdfocr.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfocr.Onnxtr.Actions {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxTrEventHandlingTest : IntegrationEventHandlingTestHelper {
        private static readonly String DESTINATION_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfocr/onnxtr/actions/OnnxTrEventHandlingTest";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeTests() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        // Section with OcrPdfCreator#createPdfFile related tests
        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfFileTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + "ocrPdfCreatorCreatePdfFile.pdf");
            new OcrPdfCreator(OCR_ENGINE).CreatePdfFile(JavaCollectionsUtil.SingletonList(imgFile), outPdfFile);
            // check ocr events
            NUnit.Framework.Assert.AreEqual(2, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[1], ocrUsageEvent);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetPdfOcrEvent() });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfOcrLogMessageConstant.CANNOT_READ_INPUT_IMAGE, LogLevel = LogLevelConstants.ERROR)]
        public virtual void OcrPdfCreatorCreatePdfFileNoImageTest() {
            FileInfo imgFile = new FileInfo("unknown");
            IList<FileInfo> images = JavaCollectionsUtil.SingletonList(imgFile);
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + "ocrPdfCreatorCreatePdfFileNoImage.pdf");
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(OCR_ENGINE);
            NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => ocrPdfCreator.CreatePdfFile(images, outPdfFile
                ));
            // check ocr events
            NUnit.Framework.Assert.AreEqual(0, eventsHandler.GetEvents().Count);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfFileNoOutputFileTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            IList<FileInfo> images = JavaCollectionsUtil.SingletonList(imgFile);
            FileInfo outPdfFile = new FileInfo("no/no_file");
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(OCR_ENGINE);
            NUnit.Framework.Assert.Catch(typeof(System.IO.IOException), () => ocrPdfCreator.CreatePdfFile(images, outPdfFile
                ));
            // check ocr events
            NUnit.Framework.Assert.AreEqual(0, eventsHandler.GetEvents().Count);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfFileNullOutputFileTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            IList<FileInfo> images = JavaCollectionsUtil.SingletonList(imgFile);
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(OCR_ENGINE);
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => ocrPdfCreator.CreatePdfFile(images, null
                ));
            // check ocr events
            NUnit.Framework.Assert.AreEqual(0, eventsHandler.GetEvents().Count);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfFileTwoImagesTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + "ocrPdfCreatorCreatePdfFileTwoImages.pdf");
            new OcrPdfCreator(OCR_ENGINE).CreatePdfFile(JavaUtil.ArraysAsList(imgFile, imgFile), outPdfFile);
            // check ocr events
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent1 = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent1, EventConfirmationType.ON_CLOSE);
            IEvent ocrUsageEvent2 = eventsHandler.GetEvents()[1];
            ValidateUsageEvent(ocrUsageEvent2, EventConfirmationType.ON_CLOSE);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], ocrUsageEvent1);
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], ocrUsageEvent2);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetPdfOcrEvent() });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfFileTwoRunningsTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + "ocrPdfCreatorCreatePdfFileTwoRunnings");
            new OcrPdfCreator(OCR_ENGINE).CreatePdfFile(JavaCollectionsUtil.SingletonList(imgFile), outPdfFile);
            new OcrPdfCreator(OCR_ENGINE).CreatePdfFile(JavaCollectionsUtil.SingletonList(imgFile), outPdfFile);
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[1], ocrUsageEvent);
            // usage event
            ocrUsageEvent = eventsHandler.GetEvents()[2];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], ocrUsageEvent);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetPdfOcrEvent() });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        // Section with OcrPdfCreator#createPdf related tests
        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + "ocrPdfCreatorCreatePdf.pdf");
            PdfWriter pdfWriter = new PdfWriter(outPdfFile);
            PdfDocument pdfDocument = new OcrPdfCreator(OCR_ENGINE).CreatePdf(JavaCollectionsUtil.SingletonList(imgFile
                ), pdfWriter);
            pdfDocument.Close();
            NUnit.Framework.Assert.AreEqual(3, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            // there is no statistic event
            ValidateCoreConfirmEvent(eventsHandler.GetEvents()[1]);
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], ocrUsageEvent);
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetCoreEvent(), GetPdfOcrEvent
                () });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfOcrLogMessageConstant.CANNOT_READ_INPUT_IMAGE, LogLevel = LogLevelConstants.ERROR)]
        public virtual void OcrPdfCreatorCreatePdfNoImageTest() {
            IList<FileInfo> images = JavaCollectionsUtil.SingletonList(new FileInfo("no_image"));
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + "ocrPdfCreatorCreatePdfNoImage.pdf");
            PdfWriter pdfWriter = new PdfWriter(outPdfFile);
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(OCR_ENGINE);
            NUnit.Framework.Assert.Catch(typeof(PdfOcrInputException), () => ocrPdfCreator.CreatePdf(images, pdfWriter
                ));
            pdfWriter.Dispose();
            NUnit.Framework.Assert.AreEqual(0, eventsHandler.GetEvents().Count);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfNullWriterTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            IList<FileInfo> images = JavaCollectionsUtil.SingletonList(imgFile);
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(OCR_ENGINE);
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => ocrPdfCreator.CreatePdf(images, null));
            NUnit.Framework.Assert.AreEqual(1, eventsHandler.GetEvents().Count);
            ValidateUsageEvent(eventsHandler.GetEvents()[0], EventConfirmationType.ON_CLOSE);
        }

        // Section with OcrPdfCreator#createPdfAFile related tests
        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfAFileTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + "ocrPdfCreatorCreatePdfAFile.pdf");
            OcrPdfCreatorProperties props = new OcrPdfCreatorProperties().SetPdfLang("en-US");
            new OcrPdfCreator(OCR_ENGINE, props).CreatePdfAFile(JavaCollectionsUtil.SingletonList(imgFile), outPdfFile
                , GetRGBPdfOutputIntent());
            // check ocr events
            NUnit.Framework.Assert.AreEqual(2, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[1], ocrUsageEvent);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetPdfOcrEvent() });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        // Section with OcrPdfCreator#createPdfA related tests
        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfATest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + "ocrPdfCreatorCreatePdfA.pdf");
            PdfWriter pdfWriter = new PdfWriter(outPdfFile);
            OcrPdfCreatorProperties props = new OcrPdfCreatorProperties().SetPdfLang("en-US");
            PdfDocument pdfDocument = new OcrPdfCreator(OCR_ENGINE, props).CreatePdfA(JavaCollectionsUtil.SingletonList
                (imgFile), pdfWriter, GetRGBPdfOutputIntent());
            pdfDocument.Close();
            // check ocr events
            NUnit.Framework.Assert.AreEqual(3, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            // there is no statistic event
            ValidateCoreConfirmEvent(eventsHandler.GetEvents()[1]);
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], ocrUsageEvent);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetCoreEvent(), GetPdfOcrEvent
                () });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        // Section with OnnxTrOcrEngine#doImageOcr related tests
        [NUnit.Framework.Test]
        public virtual void DoImageOcrTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            OCR_ENGINE.DoImageOcr(imgFile);
            NUnit.Framework.Assert.AreEqual(2, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[1], usageEvent);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfOcrLogMessageConstant.CANNOT_READ_INPUT_IMAGE, LogLevel = LogLevelConstants.ERROR)]
        public virtual void DoImageOcrNoImageTest() {
            FileInfo imgFile = new FileInfo("uncknown");
            NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => OCR_ENGINE.DoImageOcr(imgFile));
            NUnit.Framework.Assert.AreEqual(0, eventsHandler.GetEvents().Count);
        }

        [NUnit.Framework.Test]
        public virtual void DoImageOcrTwoRunningsTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            OCR_ENGINE.DoImageOcr(imgFile);
            OCR_ENGINE.DoImageOcr(imgFile);
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[1], usageEvent);
            usageEvent = eventsHandler.GetEvents()[2];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], usageEvent);
        }

        // Section with OnnxTrOcrEngine#createTxtFile related tests
        [NUnit.Framework.Test]
        public virtual void CreateTxtFileTwoImagesTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            OCR_ENGINE.CreateTxtFile(JavaUtil.ArraysAsList(imgFile, imgFile), new FileInfo(DESTINATION_FOLDER + "createTxtFileTwoImages.txt"
                ));
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent usageEvent1 = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent1, EventConfirmationType.ON_DEMAND);
            IEvent usageEvent2 = eventsHandler.GetEvents()[1];
            ValidateUsageEvent(usageEvent2, EventConfirmationType.ON_DEMAND);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], usageEvent1);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], usageEvent2);
        }

        [NUnit.Framework.Test]
        public virtual void CreateTxtFileNullEventHelperTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            OCR_ENGINE.CreateTxtFile(JavaUtil.ArraysAsList(imgFile, imgFile), new FileInfo(DESTINATION_FOLDER + "createTxtFileNullEventHelper.txt"
                ), new OcrProcessContext(null));
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent usageEvent1 = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent1, EventConfirmationType.ON_DEMAND);
            IEvent usageEvent2 = eventsHandler.GetEvents()[1];
            ValidateUsageEvent(usageEvent2, EventConfirmationType.ON_DEMAND);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], usageEvent1);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], usageEvent2);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfOcrLogMessageConstant.CANNOT_READ_INPUT_IMAGE, LogLevel = LogLevelConstants.ERROR)]
        public virtual void CreateTxtFileNoImageTest() {
            FileInfo imgFile = new FileInfo("no_image");
            IList<FileInfo> images = JavaUtil.ArraysAsList(imgFile, imgFile);
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + " createTxtFileNoImage.pdf");
            NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => OCR_ENGINE.CreateTxtFile(images, outPdfFile));
            NUnit.Framework.Assert.AreEqual(0, eventsHandler.GetEvents().Count);
        }

        [NUnit.Framework.Test]
        public virtual void CreateTxtFileNoFileTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            IList<FileInfo> images = JavaUtil.ArraysAsList(imgFile, imgFile);
            FileInfo outPdfFile = new FileInfo("nopath/nofile");
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => OCR_ENGINE.CreateTxtFile(images, 
                outPdfFile));
            NUnit.Framework.Assert.IsTrue(e.Message.Contains(PdfOcrExceptionMessageConstant.CANNOT_WRITE_TO_FILE.JSubstring
                (0, 20)));
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("nopath"));
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("nofile"));
            NUnit.Framework.Assert.AreEqual(2, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            // there is no statistic event
            // there is no confirm event
            usageEvent = eventsHandler.GetEvents()[1];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
        }

        // there is no statistic event
        // there is no confirm event
        [NUnit.Framework.Test]
        public virtual void CreateTxtFileNullOutFileTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            IList<FileInfo> images = JavaUtil.ArraysAsList(imgFile, imgFile);
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => OCR_ENGINE.CreateTxtFile(images, null));
            NUnit.Framework.Assert.AreEqual(2, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            // there is no statistic event
            usageEvent = eventsHandler.GetEvents()[1];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
        }

        // there is no statistic event
        // Section with MetaInfo related tests
        [NUnit.Framework.Test]
        public virtual void SetEventCountingMetaInfoTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + "setEventCountingMetaInfo.pdf");
            CreatePdfAndSetEventCountingMetaInfo(OCR_ENGINE, outPdfFile, imgFile, new TestMetaInfo());
            // TestMetaInfo from com.itextpdf.pdfocr package which isn't
            // registered in ContextManager, it's why core events are passed
            NUnit.Framework.Assert.AreEqual(3, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            // there is no statistic event
            ValidateCoreConfirmEvent(eventsHandler.GetEvents()[1]);
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], ocrUsageEvent);
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetCoreEvent(), GetPdfOcrEvent
                () });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        public virtual void SetEventCountingOnnxTrMetaInfoTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + " setEventCountingOnnxTrMetaInfo.pdf");
            CreatePdfAndSetEventCountingMetaInfo(OCR_ENGINE, outPdfFile, imgFile, new OnnxTrEventHandlingTest.TestOnnxTrMetaInfo
                ());
            // TestOnnxTrMetaInfo from com.itextpdf.pdfocr.onnxtr package which
            // is registered in ContextManager, it's why core events are discarded
            NUnit.Framework.Assert.AreEqual(2, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[1], ocrUsageEvent);
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetPdfOcrEvent() });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfFileTestMetaInfoTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + "createPdfFileTestMetaInfo.pdf");
            CreatePdfFileAndSetMetaInfoToProps(OCR_ENGINE, outPdfFile, imgFile, new TestMetaInfo());
            // TestMetaInfo from com.itextpdf.pdfocr package which isn't
            // registered in ContextManager, it's why core events are passed
            NUnit.Framework.Assert.AreEqual(3, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            // there is no statistic event
            ValidateCoreConfirmEvent(eventsHandler.GetEvents()[1]);
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], ocrUsageEvent);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetCoreEvent(), GetPdfOcrEvent
                () });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        public virtual void CreatePdfFileTestOnnxTrMetaInfoTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + "createPdfFileTestOnnxTrMetaInfo.pdf");
            CreatePdfFileAndSetMetaInfoToProps(OCR_ENGINE, outPdfFile, imgFile, new OnnxTrEventHandlingTest.TestOnnxTrMetaInfo
                ());
            // TestOnnxTrMetaInfo from com.itextpdf.pdfocr.onnxtr package which
            // is registered in ContextManager, it's why core events are discarded
            NUnit.Framework.Assert.AreEqual(2, eventsHandler.GetEvents().Count);
            IEvent ocrUsageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(ocrUsageEvent, EventConfirmationType.ON_CLOSE);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[1], ocrUsageEvent);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetPdfOcrEvent() });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        // Section with custom EventHelper related tests
        [NUnit.Framework.Test]
        public virtual void DoImageOcrCustomEventHelperTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            OCR_ENGINE.DoImageOcr(imgFile, new OcrProcessContext(new OnnxTrEventHandlingTest.CustomEventHelper()));
            NUnit.Framework.Assert.AreEqual(2, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[1], usageEvent);
        }

        [NUnit.Framework.Test]
        public virtual void CreateTxtFileCustomEventHelperTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "numbers_01.jpg");
            OCR_ENGINE.CreateTxtFile(JavaUtil.ArraysAsList(imgFile, imgFile), new FileInfo(DESTINATION_FOLDER + "createTxtFileCustomEventHelper.txt"
                ), new OcrProcessContext(new OnnxTrEventHandlingTest.CustomEventHelper()));
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent usageEvent1 = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent1, EventConfirmationType.ON_DEMAND);
            IEvent usageEvent2 = eventsHandler.GetEvents()[1];
            ValidateUsageEvent(usageEvent2, EventConfirmationType.ON_DEMAND);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], usageEvent1);
            // there is no statistic event
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], usageEvent2);
        }

        // Section with multipage TIFF image related tests
        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfFileMultipageTiffTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "two_pages.tiff");
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + "ocrPdfCreatorCreatePdfFileMultipageTiff.pdf");
            new OcrPdfCreator(OCR_ENGINE).CreatePdfFile(JavaCollectionsUtil.SingletonList(imgFile), outPdfFile);
            // check ocr events
            // 2 pages in TIFF image
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            for (int i = 0; i < 2; i++) {
                IEvent usageEvent = eventsHandler.GetEvents()[i];
                ValidateUsageEvent(usageEvent, EventConfirmationType.ON_CLOSE);
                // there is no statistic event
                ValidateConfirmEvent(eventsHandler.GetEvents()[2 + i], usageEvent);
            }
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetPdfOcrEvent() });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        public virtual void CreateTxtFileMultipageTiffTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "two_pages.tiff");
            OCR_ENGINE.CreateTxtFile(JavaUtil.ArraysAsList(imgFile), new FileInfo(DESTINATION_FOLDER + "createTxtFileMultipageTiff.txt"
                ));
            // 2 pages in TIFF image
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            for (int i = 0; i < 2; i++) {
                IEvent usageEvent = eventsHandler.GetEvents()[i];
                ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
                // there is no statistic event
                ValidateConfirmEvent(eventsHandler.GetEvents()[2 + i], usageEvent);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DoImageOcrMultipageTiffTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGE_DIRECTORY + "two_pages.tiff");
            OCR_ENGINE.DoImageOcr(imgFile);
            // 2 pages in TIFF image
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            for (int i = 0; i < 2; i++) {
                IEvent usageEvent = eventsHandler.GetEvents()[i * 2];
                ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
                // there is no statistic event
                ValidateConfirmEvent(eventsHandler.GetEvents()[i * 2 + 1], usageEvent);
            }
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorMakeSearchableTest() {
            FileInfo inPdfFile = new FileInfo(TEST_PDFS_DIRECTORY + "2pages.pdf");
            FileInfo outPdfFile = new FileInfo(DESTINATION_FOLDER + "ocrPdfCreatorMakeSearchable.pdf");
            try {
                new OcrPdfCreator(OCR_ENGINE).MakePdfSearchable(inPdfFile, outPdfFile);
                // Check ocr events. No stats events.
                // 3 images == 6 events + 1 confirm event for process_pdf event which is not caught by eventHandler
                NUnit.Framework.Assert.AreEqual(7, eventsHandler.GetEvents().Count);
                for (int i = 0; i < 3; i++) {
                    IEvent usageEvent = eventsHandler.GetEvents()[i];
                    ValidateUsageEvent(usageEvent, EventConfirmationType.ON_CLOSE);
                    // There is no statistic event
                    ValidateConfirmEvent(eventsHandler.GetEvents()[4 + i], usageEvent);
                }
                // Check producer line in the output pdf
                String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetCoreEvent(), GetPdfOcrEvent
                    () });
                ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
            }
            finally {
                outPdfFile.Delete();
            }
        }

        private class CustomEventHelper : AbstractPdfOcrEventHelper {
            public override void OnEvent(AbstractProductITextEvent @event) {
                if (@event is AbstractContextBasedITextEvent) {
                    ((AbstractContextBasedITextEvent)@event).SetMetaInfo(new TestMetaInfo());
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

        private class TestOnnxTrMetaInfo : IMetaInfo {
        }
    }
}
