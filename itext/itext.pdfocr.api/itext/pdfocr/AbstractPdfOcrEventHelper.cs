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
using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Sequence;

namespace iText.Pdfocr {
    /// <summary>Helper class for working with events.</summary>
    /// <remarks>Helper class for working with events. This class is for internal usage.</remarks>
    public abstract class AbstractPdfOcrEventHelper : AbstractITextEvent {
        /// <summary>Handles the event.</summary>
        /// <param name="event">event</param>
        public abstract void OnEvent(AbstractProductITextEvent @event);

        /// <summary>Returns the sequence id</summary>
        /// <returns>sequence id</returns>
        public abstract SequenceId GetSequenceId();

        /// <summary>Returns the confirmation type of event.</summary>
        /// <returns>event confirmation type</returns>
        public abstract EventConfirmationType GetConfirmationType();
    }
}
