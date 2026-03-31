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
using System.Collections.Generic;
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Sequence;
using iText.Pdfocr.Statistics;
using iText.Pdfocr.Tesseract4.Actions.Data;
using iText.Pdfocr.Tesseract4.Actions.Events;
using iText.Test;

namespace iText.Pdfocr.Tesseract4 {
    [NUnit.Framework.Category("UnitTest")]
    public class Tesseract4FileResultEventHelperTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void DefaultProcessImageEventTest() {
            Tesseract4FileResultEventHelperTest.StoreEventsHandler eventsHandler = new Tesseract4FileResultEventHelperTest.StoreEventsHandler
                ();
            EventManager.GetInstance().Register(eventsHandler);
            Tesseract4FileResultEventHelper helper = new Tesseract4FileResultEventHelper();
            helper.OnEvent(PdfOcrTesseract4ProductEvent.CreateProcessImageEvent(new SequenceId(), null, EventConfirmationType
                .ON_CLOSE));
            NUnit.Framework.Assert.AreEqual(0, eventsHandler.GetEvents().Count);
            EventManager.GetInstance().Unregister(eventsHandler);
        }

        [NUnit.Framework.Test]
        public virtual void DefaultStatisticsEventTest() {
            Tesseract4FileResultEventHelperTest.StoreEventsHandler eventsHandler = new Tesseract4FileResultEventHelperTest.StoreEventsHandler
                ();
            EventManager.GetInstance().Register(eventsHandler);
            Tesseract4FileResultEventHelper helper = new Tesseract4FileResultEventHelper();
            helper.OnEvent(new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.PDF, PdfOcrTesseract4ProductData.GetInstance
                ()));
            NUnit.Framework.Assert.AreEqual(1, eventsHandler.GetEvents().Count);
            EventManager.GetInstance().Unregister(eventsHandler);
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
