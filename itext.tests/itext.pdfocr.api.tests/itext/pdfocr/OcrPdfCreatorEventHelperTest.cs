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
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Data;
using iText.Commons.Actions.Sequence;
using iText.Commons.Utils;
using iText.Kernel.Actions.Data;
using iText.Pdfocr.Statistics;
using iText.Test;

namespace iText.Pdfocr {
    [NUnit.Framework.Category("UnitTest")]
    public class OcrPdfCreatorEventHelperTest : ExtendedITextTest {
        private static readonly ProductData DUMMY_PRODUCT_DATA = new ProductData("test-product", "inner_product", 
            "1.0.0", 1900, 2100);

        private OcrPdfCreatorEventHelperTest.StoreEventsHandler storeEventsHandler;

        [NUnit.Framework.SetUp]
        public virtual void Before() {
            storeEventsHandler = new OcrPdfCreatorEventHelperTest.StoreEventsHandler();
            EventManager.GetInstance().Register(storeEventsHandler);
        }

        [NUnit.Framework.TearDown]
        public virtual void After() {
            EventManager.GetInstance().Unregister(storeEventsHandler);
            storeEventsHandler = null;
        }

        [NUnit.Framework.Test]
        public virtual void ProductContextBasedEventTest() {
            OcrPdfCreatorEventHelper helper = new OcrPdfCreatorEventHelper(new SequenceId(), new OcrPdfCreatorEventHelperTest.DummyMetaInfo
                ());
            OcrPdfCreatorEventHelperTest.DummyITextEvent @event = new OcrPdfCreatorEventHelperTest.DummyITextEvent();
            helper.OnEvent(@event);
            NUnit.Framework.Assert.AreEqual(1, storeEventsHandler.GetEvents().Count);
            NUnit.Framework.Assert.AreEqual(@event, storeEventsHandler.GetEvents()[0]);
        }

        [NUnit.Framework.Test]
        public virtual void PdfOcrStatisticsEventTest() {
            OcrPdfCreatorEventHelper helper = new OcrPdfCreatorEventHelper(new SequenceId(), new OcrPdfCreatorEventHelperTest.DummyMetaInfo
                ());
            PdfOcrOutputTypeStatisticsEvent e = new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.PDF, DUMMY_PRODUCT_DATA
                );
            helper.OnEvent(e);
            NUnit.Framework.Assert.AreEqual(0, storeEventsHandler.GetEvents().Count);
        }

        [NUnit.Framework.Test]
        public virtual void CustomProductEventTest() {
            OcrPdfCreatorEventHelper helper = new OcrPdfCreatorEventHelper(new SequenceId(), new OcrPdfCreatorEventHelperTest.DummyMetaInfo
                ());
            AbstractProductITextEvent @event = new OcrPdfCreatorEventHelperTest.CustomProductITextEvent(DUMMY_PRODUCT_DATA
                );
            helper.OnEvent(@event);
            NUnit.Framework.Assert.AreEqual(1, storeEventsHandler.GetEvents().Count);
            NUnit.Framework.Assert.AreEqual(@event, storeEventsHandler.GetEvents()[0]);
        }

        [NUnit.Framework.Test]
        public virtual void CustomStatisticsEventTest() {
            OcrPdfCreatorEventHelper helper = new OcrPdfCreatorEventHelper(new SequenceId(), new OcrPdfCreatorEventHelperTest.DummyMetaInfo
                ());
            OcrPdfCreatorEventHelperTest.CustomStatisticsEvent @event = new OcrPdfCreatorEventHelperTest.CustomStatisticsEvent
                (DUMMY_PRODUCT_DATA);
            helper.OnEvent(@event);
            NUnit.Framework.Assert.AreEqual(1, storeEventsHandler.GetEvents().Count);
            NUnit.Framework.Assert.AreEqual(@event, storeEventsHandler.GetEvents()[0]);
        }

        private class DummyMetaInfo : IMetaInfo {
        }

        private class DummyITextEvent : AbstractProductProcessITextEvent {
            protected internal DummyITextEvent()
                : base(ITextCoreProductData.GetInstance(), null, EventConfirmationType.ON_DEMAND) {
            }

            public override String GetEventType() {
                return "test-event";
            }
        }

        private class CustomProductITextEvent : AbstractProductITextEvent {
            protected internal CustomProductITextEvent(ProductData productData)
                : base(productData) {
            }
        }

        private class CustomStatisticsEvent : AbstractStatisticsEvent {
            protected internal CustomStatisticsEvent(ProductData productData)
                : base(productData) {
            }

            public override IList<String> GetStatisticsNames() {
                return JavaCollectionsUtil.SingletonList("custom-statistics");
            }
        }

        private class StoreEventsHandler : IEventHandler {
            private IList<IEvent> events = new List<IEvent>();

            public virtual IList<IEvent> GetEvents() {
                return events;
            }

            public virtual void OnEvent(IEvent @event) {
                events.Add(@event);
            }
        }
    }
}
