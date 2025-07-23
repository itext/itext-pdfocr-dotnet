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
using System.Collections.Generic;
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Sequence;
using iText.Pdfocr;
using iText.Pdfocr.Onnxtr.Actions.Events;

namespace iText.Pdfocr.Onnxtr {
//\cond DO_NOT_DOCUMENT
    /// <summary>Helper class for working with events.</summary>
    internal sealed class OnnxTrFileResultEventHelper : AbstractPdfOcrEventHelper {
        private readonly AbstractPdfOcrEventHelper wrappedEventHelper;

        private readonly IList<ConfirmEvent> events;

//\cond DO_NOT_DOCUMENT
        internal OnnxTrFileResultEventHelper(AbstractPdfOcrEventHelper wrappedEventHelper) {
            this.wrappedEventHelper = wrappedEventHelper == null ? new OnnxTrEventHelper() : wrappedEventHelper;
            this.events = new List<ConfirmEvent>();
        }
//\endcond

        public override void OnEvent(AbstractProductITextEvent @event) {
            if (IsConfirmForProcessImageOnnxTrEvent(@event)) {
                events.Add((ConfirmEvent)@event);
            }
            else {
                wrappedEventHelper.OnEvent(@event);
            }
        }

        public override SequenceId GetSequenceId() {
            return wrappedEventHelper.GetSequenceId();
        }

        public override EventConfirmationType GetConfirmationType() {
            return wrappedEventHelper.GetConfirmationType();
        }

        /// <summary>
        /// Register all previously saved events to wrapped
        /// <see cref="iText.Pdfocr.AbstractPdfOcrEventHelper"/>.
        /// </summary>
        public void RegisterAllSavedEvents() {
            foreach (AbstractProductITextEvent @event in events) {
                wrappedEventHelper.OnEvent(@event);
            }
        }

        private static bool IsConfirmForProcessImageOnnxTrEvent(AbstractProductITextEvent @event) {
            return @event is ConfirmEvent && ((ConfirmEvent)@event).GetConfirmedEvent() is PdfOcrOnnxTrProductEvent &&
                 PdfOcrOnnxTrProductEvent.PROCESS_IMAGE_ONNXTR.Equals(((ConfirmEvent)@event).GetConfirmedEvent().GetEventType
                ());
        }
    }
//\endcond
}
