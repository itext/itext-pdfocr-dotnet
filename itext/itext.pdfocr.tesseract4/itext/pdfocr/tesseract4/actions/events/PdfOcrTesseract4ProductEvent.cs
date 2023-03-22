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
using System;
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Sequence;
using iText.Pdfocr.Tesseract4.Actions.Data;

namespace iText.Pdfocr.Tesseract4.Actions.Events {
    /// <summary>Class represents events registered in iText pdfOcr Tesseract4 module.</summary>
    public class PdfOcrTesseract4ProductEvent : AbstractProductProcessITextEvent {
        /// <summary>Process image event type.</summary>
        public const String PROCESS_IMAGE = "process-image";

        private readonly String eventType;

        /// <summary>Creates an event associated with a general identifier and additional meta data.</summary>
        /// <param name="sequenceId">is an identifier associated with the event</param>
        /// <param name="metaInfo">is an additional meta info</param>
        /// <param name="eventType">is a string description of the event</param>
        /// <param name="eventConfirmationType">is an event confirmation type</param>
        private PdfOcrTesseract4ProductEvent(SequenceId sequenceId, IMetaInfo metaInfo, String eventType, EventConfirmationType
             eventConfirmationType)
            : base(sequenceId, PdfOcrTesseract4ProductData.GetInstance(), metaInfo, eventConfirmationType) {
            this.eventType = eventType;
        }

        /// <summary>Creates process-image event.</summary>
        /// <param name="sequenceId">is an identifier associated with the event</param>
        /// <param name="metaInfo">is an additional meta info</param>
        /// <param name="eventConfirmationType">is an event confirmation type</param>
        /// <returns>process-image event</returns>
        public static iText.Pdfocr.Tesseract4.Actions.Events.PdfOcrTesseract4ProductEvent CreateProcessImageEvent(
            SequenceId sequenceId, IMetaInfo metaInfo, EventConfirmationType eventConfirmationType) {
            return new iText.Pdfocr.Tesseract4.Actions.Events.PdfOcrTesseract4ProductEvent(sequenceId, metaInfo, PROCESS_IMAGE
                , eventConfirmationType);
        }

        public override String GetEventType() {
            return eventType;
        }
    }
}
