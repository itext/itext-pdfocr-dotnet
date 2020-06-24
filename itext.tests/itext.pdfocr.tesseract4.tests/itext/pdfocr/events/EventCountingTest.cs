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
using iText.Kernel.Counter;
using iText.Kernel.Counter.Event;
using iText.Kernel.Pdf;
using iText.Metainfo;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4;
using iText.Pdfocr.Tesseract4.Events;

namespace iText.Pdfocr.Events {
    public abstract class EventCountingTest : IntegrationTestHelper {
        protected internal static readonly String PROFILE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/events/";

        internal AbstractTesseract4OcrEngine tesseractReader;

        internal String testFileTypeName;

        private bool isExecutableReaderType;

        public EventCountingTest(IntegrationTestHelper.ReaderType type) {
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
        public virtual void TestEventCountingPdfEvent() {
            String imgPath = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            EventCountingTest.TestEventCounter eventCounter = new EventCountingTest.TestEventCounter();
            IEventCounterFactory factory = new SimpleEventCounterFactory(eventCounter);
            EventCounterHandler.GetInstance().Register(factory);
            try {
                DoImageToPdfOcr(tesseractReader, JavaUtil.ArraysAsList(file));
                NUnit.Framework.Assert.AreEqual(1, eventCounter.GetEvents().Count);
                NUnit.Framework.Assert.AreSame(PdfOcrTesseract4Event.TESSERACT4_IMAGE_TO_PDF, eventCounter.GetEvents()[0]);
                NUnit.Framework.Assert.IsNull(eventCounter.GetMetaInfos()[0]);
            }
            finally {
                EventCounterHandler.GetInstance().Unregister(factory);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestEventCountingSeveralImagesOneImageToPdfEvent() {
            String imgPath = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            EventCountingTest.TestEventCounter eventCounter = new EventCountingTest.TestEventCounter();
            IEventCounterFactory factory = new SimpleEventCounterFactory(eventCounter);
            EventCounterHandler.GetInstance().Register(factory);
            try {
                DoImageToPdfOcr(tesseractReader, JavaUtil.ArraysAsList(file, file));
                NUnit.Framework.Assert.AreEqual(1, eventCounter.GetEvents().Count);
                NUnit.Framework.Assert.AreSame(PdfOcrTesseract4Event.TESSERACT4_IMAGE_TO_PDF, eventCounter.GetEvents()[0]);
                NUnit.Framework.Assert.IsNull(eventCounter.GetMetaInfos()[0]);
            }
            finally {
                EventCounterHandler.GetInstance().Unregister(factory);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestEventCountingPdfAEvent() {
            String imgPath = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            EventCountingTest.TestEventCounter eventCounter = new EventCountingTest.TestEventCounter();
            IEventCounterFactory factory = new SimpleEventCounterFactory(eventCounter);
            EventCounterHandler.GetInstance().Register(factory);
            try {
                DoImageToPdfAOcr(tesseractReader, JavaUtil.ArraysAsList(file));
                NUnit.Framework.Assert.AreEqual(1, eventCounter.GetEvents().Count);
                NUnit.Framework.Assert.AreSame(PdfOcrTesseract4Event.TESSERACT4_IMAGE_TO_PDFA, eventCounter.GetEvents()[0]
                    );
                NUnit.Framework.Assert.IsNull(eventCounter.GetMetaInfos()[0]);
            }
            finally {
                EventCounterHandler.GetInstance().Unregister(factory);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestEventCountingTwoPdfEvents() {
            String imgPath = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            EventCountingTest.TestEventCounter eventCounter = new EventCountingTest.TestEventCounter();
            IEventCounterFactory factory = new SimpleEventCounterFactory(eventCounter);
            EventCounterHandler.GetInstance().Register(factory);
            try {
                DoImageToPdfOcr(tesseractReader, JavaUtil.ArraysAsList(file));
                DoImageToPdfOcr(tesseractReader, JavaUtil.ArraysAsList(file));
                NUnit.Framework.Assert.AreEqual(2, eventCounter.GetEvents().Count);
                for (int i = 0; i < eventCounter.GetEvents().Count; i++) {
                    NUnit.Framework.Assert.AreSame(PdfOcrTesseract4Event.TESSERACT4_IMAGE_TO_PDF, eventCounter.GetEvents()[i]);
                    NUnit.Framework.Assert.IsNull(eventCounter.GetMetaInfos()[i]);
                }
            }
            finally {
                EventCounterHandler.GetInstance().Unregister(factory);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestEventCountingImageEvent() {
            String imgPath = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            EventCountingTest.TestEventCounter eventCounter = new EventCountingTest.TestEventCounter();
            IEventCounterFactory factory = new SimpleEventCounterFactory(eventCounter);
            EventCounterHandler.GetInstance().Register(factory);
            try {
                DoImageOcr(tesseractReader, file);
                NUnit.Framework.Assert.AreEqual(1, eventCounter.GetEvents().Count);
                NUnit.Framework.Assert.AreSame(PdfOcrTesseract4Event.TESSERACT4_IMAGE_OCR, eventCounter.GetEvents()[0]);
                NUnit.Framework.Assert.IsNull(eventCounter.GetMetaInfos()[0]);
            }
            finally {
                EventCounterHandler.GetInstance().Unregister(factory);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestEventCountingImageEventCustomMetaInfo() {
            String imgPath = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            EventCountingTest.TestEventCounter eventCounter = new EventCountingTest.TestEventCounter();
            IEventCounterFactory factory = new SimpleEventCounterFactory(eventCounter);
            EventCounterHandler.GetInstance().Register(factory);
            try {
                tesseractReader.SetThreadLocalMetaInfo(new TestMetaInfo());
                DoImageOcr(tesseractReader, file);
                NUnit.Framework.Assert.AreEqual(1, eventCounter.GetEvents().Count);
                NUnit.Framework.Assert.AreSame(PdfOcrTesseract4Event.TESSERACT4_IMAGE_OCR, eventCounter.GetEvents()[0]);
                NUnit.Framework.Assert.IsTrue(eventCounter.GetMetaInfos()[0] is TestMetaInfo);
            }
            finally {
                EventCounterHandler.GetInstance().Unregister(factory);
                tesseractReader.SetThreadLocalMetaInfo(null);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestEventCountingPdfEventCustomMetaInfo() {
            String imgPath = TEST_IMAGES_DIRECTORY + "numbers_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            EventCountingTest.TestEventCounter eventCounter = new EventCountingTest.TestEventCounter();
            IEventCounterFactory factory = new SimpleEventCounterFactory(eventCounter);
            EventCounterHandler.GetInstance().Register(factory);
            try {
                tesseractReader.SetThreadLocalMetaInfo(new TestMetaInfo());
                DoImageToPdfOcr(tesseractReader, JavaUtil.ArraysAsList(file));
                NUnit.Framework.Assert.AreEqual(1, eventCounter.GetEvents().Count);
                NUnit.Framework.Assert.AreSame(PdfOcrTesseract4Event.TESSERACT4_IMAGE_TO_PDF, eventCounter.GetEvents()[0]);
                NUnit.Framework.Assert.IsTrue(eventCounter.GetMetaInfos()[0] is TestMetaInfo);
            }
            finally {
                EventCounterHandler.GetInstance().Unregister(factory);
                tesseractReader.SetThreadLocalMetaInfo(null);
            }
        }

        public virtual void TestEventCountingCustomMetaInfoError() {
            String imgPath = TEST_IMAGES_DIRECTORY + "numbers_101.jpg";
            FileInfo file = new FileInfo(imgPath);
            EventCountingTest.TestEventCounter eventCounter = new EventCountingTest.TestEventCounter();
            IEventCounterFactory factory = new SimpleEventCounterFactory(eventCounter);
            EventCounterHandler.GetInstance().Register(factory);
            IMetaInfo metaInfo = new TestMetaInfo();
            try {
                tesseractReader.SetThreadLocalMetaInfo(metaInfo);
                DoImageToPdfOcr(tesseractReader, JavaUtil.ArraysAsList(file));
            }
            finally {
                NUnit.Framework.Assert.AreEqual(metaInfo, tesseractReader.GetThreadLocalMetaInfo());
                EventCounterHandler.GetInstance().Unregister(factory);
                tesseractReader.SetThreadLocalMetaInfo(null);
            }
        }

        private static void DoImageOcr(AbstractTesseract4OcrEngine tesseractReader, FileInfo imageFile) {
            tesseractReader.DoImageOcr(imageFile);
        }

        private static void DoImageToPdfOcr(AbstractTesseract4OcrEngine tesseractReader, IList<FileInfo> imageFiles
            ) {
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader);
            ocrPdfCreator.CreatePdf(imageFiles, new PdfWriter(new MemoryStream()));
        }

        private static void DoImageToPdfAOcr(AbstractTesseract4OcrEngine tesseractReader, IList<FileInfo> imageFiles
            ) {
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader, new OcrPdfCreatorProperties().SetPdfLang(
                "en-US"));
            Stream @is = null;
            try {
                @is = new FileStream(PROFILE_FOLDER + "sRGB_CS_profile.icm", FileMode.Open, FileAccess.Read);
            }
            catch (FileNotFoundException) {
            }
            // No expected
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            ocrPdfCreator.CreatePdfA(imageFiles, new PdfWriter(new MemoryStream()), outputIntent);
        }

        private class TestEventCounter : EventCounter {
            private IList<IEvent> events = new List<IEvent>();

            private IList<IMetaInfo> metaInfos = new List<IMetaInfo>();

            public virtual IList<IEvent> GetEvents() {
                return events;
            }

            public virtual IList<IMetaInfo> GetMetaInfos() {
                return metaInfos;
            }

            protected override void OnEvent(IEvent @event, IMetaInfo metaInfo) {
                this.events.Add(@event);
                this.metaInfos.Add(metaInfo);
            }
        }
    }
}
