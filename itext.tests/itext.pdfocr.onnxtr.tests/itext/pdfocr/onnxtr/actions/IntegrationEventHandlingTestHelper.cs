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
using iText.Commons.Actions.Processors;
using iText.Commons.Actions.Producer;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.Kernel.Actions.Events;
using iText.Kernel.Pdf;
using iText.Pdfocr;
using iText.Pdfocr.Onnxtr;
using iText.Pdfocr.Onnxtr.Actions.Data;
using iText.Pdfocr.Onnxtr.Actions.Events;
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Recognition;
using iText.Pdfocr.Statistics;
using iText.Test;

namespace iText.Pdfocr.Onnxtr.Actions {
    [NUnit.Framework.Category("IntegrationTest")]
    public abstract class IntegrationEventHandlingTestHelper : ExtendedITextTest {
        protected internal static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        protected internal static readonly String TEST_IMAGE_DIRECTORY = TEST_DIRECTORY + "images/";

        protected internal static readonly String TEST_PDFS_DIRECTORY = TEST_DIRECTORY + "pdfs/";

        private static readonly String FAST = TEST_DIRECTORY + "models/rep_fast_tiny-28867779.onnx";

        private static readonly String CRNNVGG16 = TEST_DIRECTORY + "models/crnn_vgg16_bn-662979cc.onnx";

        protected internal static OnnxTrOcrEngine OCR_ENGINE;

        protected internal IntegrationEventHandlingTestHelper.StoreEventsHandler eventsHandler;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            // init ocr engine
            IDetectionPredictor detectionPredictor = OnnxDetectionPredictor.Fast(FAST);
            IRecognitionPredictor recognitionPredictor = OnnxRecognitionPredictor.CrnnVgg16(CRNNVGG16);
            OCR_ENGINE = new OnnxTrOcrEngine(detectionPredictor, recognitionPredictor);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            OCR_ENGINE.Close();
        }

        [NUnit.Framework.SetUp]
        public virtual void Before() {
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
            NUnit.Framework.Assert.IsTrue(@event is PdfOcrOnnxTrProductEvent);
            NUnit.Framework.Assert.AreEqual("process-image-onnxtr", ((PdfOcrOnnxTrProductEvent)@event).GetEventType());
            NUnit.Framework.Assert.AreEqual(expectedConfirmationType, ((PdfOcrOnnxTrProductEvent)@event).GetConfirmationType
                ());
            NUnit.Framework.Assert.AreEqual(PdfOcrOnnxTrProductData.GetInstance(), ((PdfOcrOnnxTrProductEvent)@event).
                GetProductData());
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
                NUnit.Framework.Assert.IsTrue(pdfDocument.GetDocumentInfo().GetProducer().Contains(expected));
            }
        }

        protected internal static String CreateExpectedProducerLine(ConfirmedEventWrapper[] expectedEvents) {
            IList<ConfirmedEventWrapper> listEvents = JavaUtil.ArraysAsList(expectedEvents);
            return ProducerBuilder.ModifyProducer(listEvents, null);
        }

        protected internal static ConfirmedEventWrapper GetPdfOcrEvent() {
            PdfOcrOnnxTrProductEvent @event = PdfOcrOnnxTrProductEvent.CreateProcessImageOnnxTrEvent(new SequenceId(), 
                null, EventConfirmationType.ON_CLOSE);
            DefaultITextProductEventProcessor processor = new DefaultITextProductEventProcessor(ProductNameConstant.PDF_OCR_ONNXTR
                );
            return new ConfirmedEventWrapper(@event, processor.GetUsageType(), processor.GetProducer());
        }

        protected internal static ConfirmedEventWrapper GetCoreEvent() {
            DefaultITextProductEventProcessor processor = new DefaultITextProductEventProcessor(ProductNameConstant.ITEXT_CORE
                );
            ITextCoreProductEvent @event = ITextCoreProductEvent.CreateProcessPdfEvent(new SequenceId(), null, EventConfirmationType
                .ON_CLOSE);
            return new ConfirmedEventWrapper(@event, processor.GetUsageType(), processor.GetProducer());
        }

        protected internal static PdfOutputIntent GetRGBPdfOutputIntent() {
            String defaultRGBColorProfilePath = TEST_DIRECTORY + "profiles/sRGB_CS_profile.icm";
            Stream @is = FileUtil.GetInputStreamForFile(defaultRGBColorProfilePath);
            return new PdfOutputIntent("", "", "", "sRGB IEC61966-2.1", @is);
        }

        /// <summary>
        /// Creates PDF document with
        /// <see cref="iText.Pdfocr.OcrPdfCreator.CreatePdf(System.Collections.Generic.IList{E}, iText.Kernel.Pdf.PdfWriter)
        ///     "/>
        /// and set event counting meta info.
        /// </summary>
        /// <param name="engine">
        /// engine to set in the
        /// <see cref="iText.Pdfocr.OcrPdfCreator"/>
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
        /// <see cref="iText.Pdfocr.OcrPdfCreator.CreatePdf(System.Collections.Generic.IList{E}, iText.Kernel.Pdf.PdfWriter)
        ///     "/>
        /// and set meta info to
        /// <see cref="iText.Pdfocr.OcrPdfCreatorProperties"/>.
        /// </summary>
        /// <param name="engine">
        /// engine to set in the
        /// <see cref="iText.Pdfocr.OcrPdfCreator"/>
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
                if (@event is PdfOcrOnnxTrProductEvent || @event is PdfOcrOutputTypeStatisticsEvent || @event is ConfirmEvent
                    ) {
                    events.Add(@event);
                }
            }
        }
    }
}
