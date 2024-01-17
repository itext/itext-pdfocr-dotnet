/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Actions.Processors;
using iText.Commons.Actions.Producer;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.Kernel.Actions.Events;
using iText.Kernel.Pdf;
using iText.Pdfocr.Statistics;
using iText.Pdfocr.Tesseract4;
using iText.Pdfocr.Tesseract4.Actions.Data;
using iText.Pdfocr.Tesseract4.Actions.Events;

namespace iText.Pdfocr {
    [NUnit.Framework.Category("IntegrationTest")]
    public abstract class IntegrationEventHandlingTestHelper : IntegrationTestHelper {
        protected internal readonly AbstractTesseract4OcrEngine tesseractReader;

        protected internal IntegrationEventHandlingTestHelper.StoreEventsHandler eventsHandler;

        public IntegrationEventHandlingTestHelper(IntegrationTestHelper.ReaderType type) {
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            // init ocr engine
            Tesseract4OcrEngineProperties ocrEngineProperties = new Tesseract4OcrEngineProperties();
            ocrEngineProperties.SetPathToTessData(GetTessDataDirectory());
            tesseractReader.SetTesseract4OcrEngineProperties(ocrEngineProperties);
            // register event handler
            eventsHandler = new IntegrationEventHandlingTestHelper.StoreEventsHandler();
            EventManager.GetInstance().Register(eventsHandler);
        }

        [NUnit.Framework.TearDown]
        public virtual void After() {
            EventManager.GetInstance().Unregister(eventsHandler);
            eventsHandler = null;
        }

        protected internal static void ValidateUsageEvent(IEvent @event, EventConfirmationType expectedConfirmationType
            ) {
            NUnit.Framework.Assert.IsTrue(@event is PdfOcrTesseract4ProductEvent);
            NUnit.Framework.Assert.AreEqual("process-image", ((PdfOcrTesseract4ProductEvent)@event).GetEventType());
            NUnit.Framework.Assert.AreEqual(expectedConfirmationType, ((PdfOcrTesseract4ProductEvent)@event).GetConfirmationType
                ());
            NUnit.Framework.Assert.AreEqual(PdfOcrTesseract4ProductData.GetInstance(), ((PdfOcrTesseract4ProductEvent)
                @event).GetProductData());
        }

        protected internal static void ValidateStatisticEvent(IEvent @event, PdfOcrOutputType outputType) {
            NUnit.Framework.Assert.IsTrue(@event is PdfOcrOutputTypeStatisticsEvent);
            NUnit.Framework.Assert.AreEqual(outputType, ((PdfOcrOutputTypeStatisticsEvent)@event).GetPdfOcrStatisticsEventType
                ());
            NUnit.Framework.Assert.AreEqual(PdfOcrTesseract4ProductData.GetInstance(), ((PdfOcrOutputTypeStatisticsEvent
                )@event).GetProductData());
        }

        protected internal static void ValidateConfirmEvent(IEvent @event, IEvent expectedConfirmedEvent) {
            NUnit.Framework.Assert.IsTrue(@event is ConfirmEvent);
            NUnit.Framework.Assert.AreSame(expectedConfirmedEvent, ((ConfirmEvent)@event).GetConfirmedEvent());
        }

        // we expect core events in case of API methods returning PdfDocument
        protected internal static void ValidateCoreConfirmEvent(IEvent @event) {
            NUnit.Framework.Assert.IsTrue(@event is ConfirmEvent);
            NUnit.Framework.Assert.AreEqual(GetCoreEvent().GetEvent().GetEventType(), ((ConfirmEvent)@event).GetConfirmedEvent
                ().GetEventType());
            NUnit.Framework.Assert.AreEqual(GetCoreEvent().GetEvent().GetConfirmationType(), ((ConfirmEvent)@event).GetConfirmedEvent
                ().GetConfirmationType());
        }

        protected internal virtual void ValidatePdfProducerLine(String filePath, String expected) {
            using (PdfDocument pdfDocument = new PdfDocument(new PdfReader(filePath))) {
                NUnit.Framework.Assert.AreEqual(expected, pdfDocument.GetDocumentInfo().GetProducer());
            }
        }

        protected internal static String CreateExpectedProducerLine(ConfirmedEventWrapper[] expectedEvents) {
            IList<ConfirmedEventWrapper> listEvents = JavaUtil.ArraysAsList(expectedEvents);
            return ProducerBuilder.ModifyProducer(listEvents, null);
        }

        protected internal static ConfirmedEventWrapper GetPdfOcrEvent() {
            DefaultITextProductEventProcessor processor = new DefaultITextProductEventProcessor(ProductNameConstant.PDF_HTML
                );
            return new ConfirmedEventWrapper(PdfOcrTesseract4ProductEvent.CreateProcessImageEvent(new SequenceId(), null
                , EventConfirmationType.ON_CLOSE), processor.GetUsageType(), processor.GetProducer());
        }

        protected internal static ConfirmedEventWrapper GetCoreEvent() {
            DefaultITextProductEventProcessor processor = new DefaultITextProductEventProcessor(ProductNameConstant.ITEXT_CORE
                );
            return new ConfirmedEventWrapper(ITextCoreProductEvent.CreateProcessPdfEvent(new SequenceId(), null, EventConfirmationType
                .ON_CLOSE), processor.GetUsageType(), processor.GetProducer());
        }

        protected internal static PdfOutputIntent GetRGBPdfOutputIntent() {
            String defaultRGBColorProfilePath = TEST_DIRECTORY + "profiles" + "/sRGB_CS_profile.icm";
            Stream @is = new FileStream(defaultRGBColorProfilePath, FileMode.Open, FileAccess.Read);
            return new PdfOutputIntent("", "", "", "sRGB IEC61966-2.1", @is);
        }

        /// <summary>
        /// Creates PDF document with
        /// <see cref="OcrPdfCreator.CreatePdf(System.Collections.Generic.IList{E}, iText.Kernel.Pdf.PdfWriter)"/>
        /// and set event counting meta info.
        /// </summary>
        /// <param name="engine">
        /// engine to set in the
        /// <see cref="OcrPdfCreator"/>
        /// </param>
        /// <param name="outPdfFile">out pdf file</param>
        /// <param name="imgFile">image file</param>
        /// <param name="metaInfo">meta info</param>
        protected internal virtual void CreatePdfAndSetEventCountingMetaInfo(IOcrEngine engine, FileInfo outPdfFile
            , FileInfo imgFile, IMetaInfo metaInfo) {
            using (PdfWriter pdfWriter = new PdfWriter(outPdfFile)) {
                PdfDocument pdfDocument = new OcrPdfCreator(engine).CreatePdf(JavaCollectionsUtil.SingletonList(imgFile), 
                    pdfWriter, new DocumentProperties().SetEventCountingMetaInfo(metaInfo));
                pdfDocument.Close();
            }
        }

        /// <summary>
        /// Creates PDF document with
        /// <see cref="OcrPdfCreator.CreatePdf(System.Collections.Generic.IList{E}, iText.Kernel.Pdf.PdfWriter)"/>
        /// and set meta info to
        /// <see cref="OcrPdfCreatorProperties"/>.
        /// </summary>
        /// <param name="engine">
        /// engine to set in the
        /// <see cref="OcrPdfCreator"/>
        /// </param>
        /// <param name="outPdfFile">out pdf file</param>
        /// <param name="imgFile">image file</param>
        /// <param name="metaInfo">meta info</param>
        protected internal virtual void CreatePdfFileAndSetMetaInfoToProps(IOcrEngine engine, FileInfo outPdfFile, 
            FileInfo imgFile, IMetaInfo metaInfo) {
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties().SetMetaInfo(metaInfo);
            new OcrPdfCreator(engine, properties).CreatePdfFile(JavaCollectionsUtil.SingletonList(imgFile), outPdfFile
                );
        }

        protected internal class StoreEventsHandler : IEventHandler {
            private readonly IList<IEvent> events = new List<IEvent>();

            public virtual IList<IEvent> GetEvents() {
                return events;
            }

            public virtual void OnEvent(IEvent @event) {
                if (@event is PdfOcrTesseract4ProductEvent || @event is PdfOcrOutputTypeStatisticsEvent || @event is ConfirmEvent
                    ) {
                    events.Add(@event);
                }
            }
        }
    }
}
