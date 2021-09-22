using iText.Commons.Actions;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Sequence;
using iText.Pdfocr;

namespace iText.Pdfocr.Tesseract4 {
    /// <summary>Helper class for working with events.</summary>
    internal class Tesseract4EventHelper : AbstractPdfOcrEventHelper {
        public override void OnEvent(AbstractProductITextEvent @event) {
            if (@event is AbstractContextBasedITextEvent) {
                ((AbstractContextBasedITextEvent)@event).SetMetaInfo(new Tesseract4MetaInfo());
            }
            EventManager.GetInstance().OnEvent(@event);
        }

        public override SequenceId GetSequenceId() {
            return new SequenceId();
        }

        public override EventConfirmationType GetConfirmationType() {
            return EventConfirmationType.ON_DEMAND;
        }
    }
}
