using iText.Commons.Actions.Contexts;

namespace iText.Pdfocr {
    /// <summary>Container to keep meta info.</summary>
    public class PdfOcrMetaInfoContainer {
        private readonly IMetaInfo metaInfo;

        /// <summary>Creates instance of container to keep passed meta info.</summary>
        /// <param name="metaInfo">meta info</param>
        public PdfOcrMetaInfoContainer(IMetaInfo metaInfo) {
            this.metaInfo = metaInfo;
        }

        internal virtual IMetaInfo GetMetaInfo() {
            return metaInfo;
        }
    }
}
