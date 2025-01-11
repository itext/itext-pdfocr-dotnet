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
