/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Kernel.Pdf.Tagutils;

namespace iText.Pdfocr.Structuretree {
    /// <summary>This class represents structure tree item of the text item put into the pdf document.</summary>
    /// <remarks>
    /// This class represents structure tree item of the text item put into the pdf document.
    /// See
    /// <see cref="iText.Pdfocr.TextInfo"/>.
    /// </remarks>
    public class LogicalStructureTreeItem {
        private AccessibilityProperties accessibilityProperties;

        private IList<iText.Pdfocr.Structuretree.LogicalStructureTreeItem> children = new List<iText.Pdfocr.Structuretree.LogicalStructureTreeItem
            >();

        private iText.Pdfocr.Structuretree.LogicalStructureTreeItem parent;

        /// <summary>
        /// Instantiate a new
        /// <see cref="LogicalStructureTreeItem"/>
        /// instance.
        /// </summary>
        public LogicalStructureTreeItem()
            : this(null) {
        }

        /// <summary>
        /// Instantiate a new
        /// <see cref="LogicalStructureTreeItem"/>
        /// instance.
        /// </summary>
        /// <param name="accessibilityProperties">properties to define and describe pdf structure elements.</param>
        public LogicalStructureTreeItem(AccessibilityProperties accessibilityProperties) {
            this.accessibilityProperties = accessibilityProperties;
        }

        /// <summary>Retrieve structure tree element's properties.</summary>
        /// <returns>structure tree element's properties.</returns>
        public virtual AccessibilityProperties GetAccessibilityProperties() {
            return accessibilityProperties;
        }

        /// <summary>Set structure tree element's properties.</summary>
        /// <param name="accessibilityProperties">structure tree element's properties.</param>
        /// <returns>
        /// this
        /// <see cref="LogicalStructureTreeItem"/>
        /// instance.
        /// </returns>
        public virtual iText.Pdfocr.Structuretree.LogicalStructureTreeItem SetAccessibilityProperties(AccessibilityProperties
             accessibilityProperties) {
            this.accessibilityProperties = accessibilityProperties;
            return this;
        }

        /// <summary>Retrieve parent structure tree item.</summary>
        /// <returns>parent structure tree item.</returns>
        public virtual iText.Pdfocr.Structuretree.LogicalStructureTreeItem GetParent() {
            return parent;
        }

        /// <summary>Add child structure tree item.</summary>
        /// <param name="child">child structure tree item.</param>
        /// <returns>
        /// this
        /// <see cref="LogicalStructureTreeItem"/>
        /// instance.
        /// </returns>
        public virtual iText.Pdfocr.Structuretree.LogicalStructureTreeItem AddChild(iText.Pdfocr.Structuretree.LogicalStructureTreeItem
             child) {
            children.Add(child);
            if (child.GetParent() != null) {
                child.GetParent().RemoveChild(child);
            }
            child.parent = this;
            return this;
        }

        /// <summary>Remove child structure tree item.</summary>
        /// <param name="child">child structure tree item.</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the child was removed,
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool RemoveChild(iText.Pdfocr.Structuretree.LogicalStructureTreeItem child) {
            if (children.Remove(child)) {
                child.parent = null;
                return true;
            }
            return false;
        }

        /// <summary>Retrieve all child structure tree items.</summary>
        /// <returns>all child structure tree items.</returns>
        public virtual IList<iText.Pdfocr.Structuretree.LogicalStructureTreeItem> GetChildren() {
            return children;
        }
    }
}
