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
using System.Threading;
using iText.Kernel.Counter;
using iText.Kernel.Counter.Event;
using iText.Metainfo;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4;
using iText.Pdfocr.Tesseract4.Events;

namespace iText.Pdfocr.Events.Multithreading {
    public abstract class MultiThreadingTest : IntegrationTestHelper {
        protected internal static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/pdfocr/events/multithreading/";

        protected internal static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/events/multithreading/";

        internal AbstractTesseract4OcrEngine tesseractReader;

        public MultiThreadingTest(IntegrationTestHelper.ReaderType type) {
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        [NUnit.Framework.SetUp]
        public virtual void InitTesseractProperties() {
            Tesseract4OcrEngineProperties ocrEngineProperties = new Tesseract4OcrEngineProperties();
            ocrEngineProperties.SetPathToTessData(new FileInfo(sourceFolder + "../../tessdata/"));
            tesseractReader.SetTesseract4OcrEngineProperties(ocrEngineProperties);
        }

        [NUnit.Framework.Test]
        public virtual void TestEventCountingPdfEvent() {
            MultiThreadingTest.TestEventCounter eventCounter = new MultiThreadingTest.TestEventCounter();
            IEventCounterFactory factory = new SimpleEventCounterFactory(eventCounter);
            EventCounterHandler.GetInstance().Register(factory);
            try {
                int n = 16;
                IMetaInfo metainfo = new TestMetaInfo();
                Thread[] threads = new Thread[n];
                for (int i = 0; i < n; i++) {
                    // We do not use Runnable as the variable's type because of porting issues
                    DoImageOcrRunnable runnable = new DoImageOcrRunnable(tesseractReader, metainfo, new FileInfo(sourceFolder 
                        + "numbers_01.jpg"), new FileInfo(destinationFolder + "ocr-result-" + (i + 1) + ".txt"), 0 == i % 2);
                    threads[i] = GetThread(runnable);
                }
                for (int i = 0; i < n; i++) {
                    threads[i].Start();
                }
                for (int i = 0; i < n; i++) {
                    threads[i].Join();
                }
                NUnit.Framework.Assert.AreEqual(n, eventCounter.GetEvents().Count);
                int expectedPdfEvents = n / 2;
                int expectedImageEvents = n - expectedPdfEvents;
                int foundPdfEvents = 0;
                int foundImageEvents = 0;
                for (int i = 0; i < n; i++) {
                    if (PdfOcrTesseract4Event.TESSERACT4_IMAGE_TO_PDF == eventCounter.GetEvents()[i]) {
                        foundPdfEvents++;
                    }
                    else {
                        if (PdfOcrTesseract4Event.TESSERACT4_IMAGE_OCR == eventCounter.GetEvents()[i]) {
                            foundImageEvents++;
                        }
                    }
                    NUnit.Framework.Assert.AreEqual(metainfo, eventCounter.GetMetaInfos()[i]);
                }
                NUnit.Framework.Assert.AreEqual(expectedImageEvents, foundImageEvents);
                NUnit.Framework.Assert.AreEqual(expectedPdfEvents, foundPdfEvents);
            }
            finally {
                EventCounterHandler.GetInstance().Unregister(factory);
            }
        }

        public class TestEventCounter : EventCounter {
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

        private static Thread GetThread(DoImageOcrRunnable runnable) {
            return new Thread(new ThreadStart(runnable.Run));
        }
    }
}
