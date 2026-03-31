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
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Pdfocr;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Statistics;
using iText.Pdfocr.Tesseract4.Logs;
using iText.Test.Attributes;

namespace iText.Pdfocr.Actions {
    public abstract class Tesseract4EventHandlingTest : IntegrationEventHandlingTestHelper {
        protected internal String destinationFolder;

        public Tesseract4EventHandlingTest(IntegrationTestHelper.ReaderType type, String destinationFolder)
            : base(type) {
            this.destinationFolder = destinationFolder;
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfFileTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            FileInfo outPdfFile = new FileInfo(destinationFolder + "ocrPdfCreatorCreatePdfFile.pdf");
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
            FileInfo outPdfFile = new FileInfo(destinationFolder + "ocrPdfCreatorCreatePdfFileNoImage.pdf");
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
            FileInfo outPdfFile = new FileInfo(destinationFolder + "ocrPdfCreatorCreatePdfFileTwoImages.pdf");
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
            FileInfo outPdfFile = new FileInfo(destinationFolder + "ocrPdfCreatorCreatePdfFileTwoRunnings.pdf");
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
            FileInfo outPdfFile = new FileInfo(destinationFolder + "ocrPdfCreatorCreatePdf.pdf");
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
            FileInfo outPdfFile = new FileInfo(destinationFolder + "ocrPdfCreatorCreatePdfNoImage.pdf");
            PdfWriter pdfWriter = new PdfWriter(outPdfFile);
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader);
            NUnit.Framework.Assert.Catch(typeof(PdfOcrInputException), () => ocrPdfCreator.CreatePdf(images, pdfWriter
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
            FileInfo outPdfFile = new FileInfo(destinationFolder + "ocrPdfCreatorCreatePdfAFile.pdf");
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
            FileInfo outPdfFile = new FileInfo(destinationFolder + "ocrPdfCreatorCreatePdfA.pdf");
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
        public virtual void CreateTxtFileTwoImagesTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "numbers_01.jpg");
            tesseractReader.CreateTxtFile(JavaUtil.ArraysAsList(imgFile, imgFile), new FileInfo(destinationFolder + "createTxtFileTwoImages.txt"
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
            tesseractReader.CreateTxtFile(JavaUtil.ArraysAsList(imgFile, imgFile), new FileInfo(destinationFolder + "createTxtFileNullEventHelper.txt"
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
            FileInfo outPdfFile = new FileInfo(destinationFolder + "createTxtFileNoImage.pdf");
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
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => tesseractReader.CreateTxtFile(images
                , outPdfFile));
            NUnit.Framework.Assert.IsTrue(e.Message.Contains(PdfOcrExceptionMessageConstant.CANNOT_WRITE_TO_FILE.JSubstring
                (0, 20)));
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("nopath"));
            NUnit.Framework.Assert.IsTrue(e.Message.Contains("nofile"));
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
            FileInfo outPdfFile = new FileInfo(destinationFolder + "setEventCountingMetaInfo.pdf");
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
            FileInfo outPdfFile = new FileInfo(destinationFolder + "createPdfFileTestMetaInfo.pdf");
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
            tesseractReader.CreateTxtFile(JavaUtil.ArraysAsList(imgFile, imgFile), new FileInfo(destinationFolder + "createTxtFileCustomEventHelper.txt"
                ), new OcrProcessContext(new Tesseract4EventHandlingTest.CustomEventHelper()));
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.DATA);
            ValidateStatisticEvent(eventsHandler.GetEvents()[2], PdfOcrOutputType.DATA);
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], usageEvent);
        }

        // Section with multipage TIFF image related tests
        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfFileMultipageTiffTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "two_pages.tiff");
            FileInfo outPdfFile = new FileInfo(destinationFolder + "ocrPdfCreatorCreatePdfFileMultipageTiff.pdf");
            new OcrPdfCreator(tesseractReader).CreatePdfFile(JavaCollectionsUtil.SingletonList(imgFile), outPdfFile);
            // check ocr events
            // 2 pages in TIFF image
            NUnit.Framework.Assert.AreEqual(5, eventsHandler.GetEvents().Count);
            for (int i = 0; i < 2; i++) {
                IEvent usageEvent = eventsHandler.GetEvents()[i];
                ValidateUsageEvent(usageEvent, EventConfirmationType.ON_CLOSE);
                ValidateConfirmEvent(eventsHandler.GetEvents()[3 + i], usageEvent);
            }
            ValidateStatisticEvent(eventsHandler.GetEvents()[2], PdfOcrOutputType.PDF);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetPdfOcrEvent() });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorCreatePdfFileMultipageTiffNoPreprocessingTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "two_pages.tiff");
            FileInfo outPdfFile = new FileInfo(destinationFolder + "ocrPdfCreatorCreatePdfFileMultipageTiffNoPreprocessing.pdf"
                );
            tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages(false);
            new OcrPdfCreator(tesseractReader).CreatePdfFile(JavaCollectionsUtil.SingletonList(imgFile), outPdfFile);
            // check ocr events
            // 2 pages in TIFF image
            NUnit.Framework.Assert.AreEqual(3, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_CLOSE);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.PDF);
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], usageEvent);
            // check producer line in the output pdf
            String expectedProdLine = CreateExpectedProducerLine(new ConfirmedEventWrapper[] { GetPdfOcrEvent() });
            ValidatePdfProducerLine(outPdfFile.FullName, expectedProdLine);
        }

        [NUnit.Framework.Test]
        public virtual void CreateTxtFileMultipageTiffTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "two_pages.tiff");
            tesseractReader.CreateTxtFile(JavaUtil.ArraysAsList(imgFile), new FileInfo(destinationFolder + "createTxtFileMultipageTiff.txt"
                ));
            // 2 pages in TIFF image
            NUnit.Framework.Assert.AreEqual(4, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            ValidateConfirmEvent(eventsHandler.GetEvents()[3], usageEvent);
            for (int i = 1; i < 3; i++) {
                ValidateStatisticEvent(eventsHandler.GetEvents()[i], PdfOcrOutputType.DATA);
            }
        }

        [NUnit.Framework.Test]
        public virtual void CreateTxtFileMultipageTiffNoPreprocessingTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "two_pages.tiff");
            tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages(false);
            tesseractReader.CreateTxtFile(JavaUtil.ArraysAsList(imgFile), new FileInfo(destinationFolder + "createTxtFileMultipageTiffNoPreprocessing.txt"
                ));
            // 2 pages in TIFF image
            NUnit.Framework.Assert.AreEqual(3, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.DATA);
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], usageEvent);
        }

        [NUnit.Framework.Test]
        public virtual void DoImageOcrMultipageTiffTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "two_pages.tiff");
            tesseractReader.DoImageOcr(imgFile);
            // 2 pages in TIFF image
            NUnit.Framework.Assert.AreEqual(6, eventsHandler.GetEvents().Count);
            for (int i = 0; i < 2; i++) {
                IEvent usageEvent = eventsHandler.GetEvents()[i * 3];
                ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
                ValidateStatisticEvent(eventsHandler.GetEvents()[i * 3 + 1], PdfOcrOutputType.DATA);
                ValidateConfirmEvent(eventsHandler.GetEvents()[i * 3 + 2], usageEvent);
            }
        }

        [NUnit.Framework.Test]
        public virtual void DoImageOcrMultipageTiffNoPreprocessingTest() {
            FileInfo imgFile = new FileInfo(TEST_IMAGES_DIRECTORY + "two_pages.tiff");
            tesseractReader.GetTesseract4OcrEngineProperties().SetPreprocessingImages(false);
            tesseractReader.DoImageOcr(imgFile);
            // 2 pages in TIFF image
            NUnit.Framework.Assert.AreEqual(3, eventsHandler.GetEvents().Count);
            IEvent usageEvent = eventsHandler.GetEvents()[0];
            ValidateUsageEvent(usageEvent, EventConfirmationType.ON_DEMAND);
            ValidateStatisticEvent(eventsHandler.GetEvents()[1], PdfOcrOutputType.DATA);
            ValidateConfirmEvent(eventsHandler.GetEvents()[2], usageEvent);
        }

        [NUnit.Framework.Test]
        public virtual void OcrPdfCreatorMakeSearchableTest() {
            FileInfo inPdfFile = new FileInfo(TEST_PDFS_DIRECTORY + "2pages.pdf");
            FileInfo outPdfFile = new FileInfo(destinationFolder + "ocrPdfCreatorMakeSearchable.pdf");
            try {
                new OcrPdfCreator(tesseractReader).MakePdfSearchable(inPdfFile, outPdfFile);
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
