/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
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
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Sequence;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4.Actions.Events;

namespace iText.Pdfocr.Tesseract4 {
    /// <summary>Helper class for working with events.</summary>
    internal class Tesseract4FileResultEventHelper : AbstractPdfOcrEventHelper {
        private AbstractPdfOcrEventHelper wrappedEventHelper;

        internal Tesseract4FileResultEventHelper()
            : this(null) {
        }

        internal Tesseract4FileResultEventHelper(AbstractPdfOcrEventHelper wrappedEventHelper) {
            this.wrappedEventHelper = wrappedEventHelper == null ? new Tesseract4EventHelper() : wrappedEventHelper;
        }

        public override void OnEvent(AbstractProductITextEvent @event) {
            if (!IsProcessImageEvent(@event) && !IsConfirmForProcessImageEvent(@event)) {
                wrappedEventHelper.OnEvent(@event);
            }
        }

        public override SequenceId GetSequenceId() {
            return wrappedEventHelper.GetSequenceId();
        }

        public override EventConfirmationType GetConfirmationType() {
            return wrappedEventHelper.GetConfirmationType();
        }

        private static bool IsProcessImageEvent(AbstractProductITextEvent @event) {
            return @event is PdfOcrTesseract4ProductEvent && PdfOcrTesseract4ProductEvent.PROCESS_IMAGE.Equals(((PdfOcrTesseract4ProductEvent
                )@event).GetEventType());
        }

        private static bool IsConfirmForProcessImageEvent(AbstractProductITextEvent @event) {
            return @event is ConfirmEvent && ((ConfirmEvent)@event).GetConfirmedEvent() is PdfOcrTesseract4ProductEvent
                 && PdfOcrTesseract4ProductEvent.PROCESS_IMAGE.Equals(((ConfirmEvent)@event).GetConfirmedEvent().GetEventType
                ());
        }
    }
}
