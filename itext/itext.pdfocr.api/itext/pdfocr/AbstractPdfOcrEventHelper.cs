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
