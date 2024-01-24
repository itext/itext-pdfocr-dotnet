namespace iText.Pdfocr.Structuretree {
    /// <summary>This class represents artifact structure tree item.</summary>
    /// <remarks>
    /// This class represents artifact structure tree item. Attaching such item to the text info means that
    /// the text will be marked as artifact.
    /// </remarks>
    public sealed class ArtifactItem : LogicalStructureTreeItem {
        private static readonly iText.Pdfocr.Structuretree.ArtifactItem ARTIFACT_INSTANCE = new iText.Pdfocr.Structuretree.ArtifactItem
            ();

        private ArtifactItem()
            : base() {
        }

        /// <summary>
        /// Retrieve an instance of
        /// <see cref="ArtifactItem"/>.
        /// </summary>
        /// <returns>
        /// an instance of
        /// <see cref="ArtifactItem"/>.
        /// </returns>
        public static iText.Pdfocr.Structuretree.ArtifactItem GetInstance() {
            return ARTIFACT_INSTANCE;
        }
    }
}
