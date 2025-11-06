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
using System;
using iText.Kernel.Geom;
using iText.Pdfocr.Structuretree;
using iText.Pdfocr.Util;

namespace iText.Pdfocr {
    /// <summary>
    /// This class describes how recognized text is positioned on the image
    /// providing bbox for each text item (could be a line or a word).
    /// </summary>
    public class TextInfo {
        /// <summary>Contains any text.</summary>
        private String text;

        /// <summary>
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// describing text bbox (lower-left based) expressed in points.
        /// </summary>
        private Rectangle bboxRect;

        /// <summary>
        /// <see cref="TextOrientation"/>
        /// describing the orientation of the text (i.e. rotation).
        /// </summary>
        /// <remarks>
        /// <see cref="TextOrientation"/>
        /// describing the orientation of the text (i.e. rotation). Text is
        /// assumed to be horizontal without any rotation by default.
        /// </remarks>
        private TextOrientation orientation = TextOrientation.HORIZONTAL;

        /// <summary>
        /// If LogicalStructureTreeItem is set, then
        /// <see cref="TextInfo"/>
        /// s are expected to be in logical order.
        /// </summary>
        private LogicalStructureTreeItem logicalStructureTreeItem;

        /// <summary>
        /// Creates a new
        /// <see cref="TextInfo"/>
        /// instance.
        /// </summary>
        public TextInfo() {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="TextInfo"/>
        /// instance from existing one.
        /// </summary>
        /// <param name="textInfo">to create from</param>
        public TextInfo(iText.Pdfocr.TextInfo textInfo) {
            this.text = textInfo.text;
            this.bboxRect = new Rectangle(textInfo.bboxRect);
            this.orientation = textInfo.orientation;
        }

        /// <summary>
        /// Creates a new
        /// <see cref="TextInfo"/>
        /// instance.
        /// </summary>
        /// <param name="text">any text</param>
        /// <param name="bbox">
        /// 
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// describing text bbox
        /// </param>
        public TextInfo(String text, Rectangle bbox) {
            this.text = text;
            this.bboxRect = new Rectangle(bbox);
        }

        /// <summary>
        /// Creates a new
        /// <see cref="TextInfo"/>
        /// instance.
        /// </summary>
        /// <param name="text">any text</param>
        /// <param name="bbox">
        /// 
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// describing text bbox
        /// </param>
        /// <param name="orientation">orientation of the text</param>
        public TextInfo(String text, Rectangle bbox, TextOrientation orientation) {
            this.text = text;
            this.bboxRect = new Rectangle(bbox);
            this.orientation = Objects.RequireNonNull(orientation);
        }

        /// <summary>Gets text element.</summary>
        /// <returns>String</returns>
        public virtual String GetText() {
            return text;
        }

        /// <summary>Sets text element.</summary>
        /// <param name="newText">retrieved text</param>
        public virtual void SetText(String newText) {
            text = newText;
        }

        /// <summary>Gets bbox coordinates.</summary>
        /// <returns>
        /// 
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// describing text bbox
        /// </returns>
        public virtual Rectangle GetBboxRect() {
            return bboxRect;
        }

        /// <summary>Sets text bbox.</summary>
        /// <param name="bbox">
        /// 
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// describing text bbox
        /// </param>
        public virtual void SetBboxRect(Rectangle bbox) {
            this.bboxRect = new Rectangle(bbox);
        }

        /// <summary>Gets the text orientation.</summary>
        /// <returns>
        /// 
        /// <see cref="TextOrientation"/>
        /// describing the orientation of the text (i.e. rotation)
        /// </returns>
        public virtual TextOrientation GetOrientation() {
            return orientation;
        }

        /// <summary>Sets the text orientation.</summary>
        /// <param name="orientation">
        /// 
        /// <see cref="TextOrientation"/>
        /// describing the orientation of the text (i.e. rotation)
        /// </param>
        public virtual void SetOrientation(TextOrientation orientation) {
            this.orientation = Objects.RequireNonNull(orientation);
        }

        /// <summary>Retrieves structure tree item for the text item.</summary>
        /// <returns>structure tree item.</returns>
        public virtual LogicalStructureTreeItem GetLogicalStructureTreeItem() {
            return logicalStructureTreeItem;
        }

        /// <summary>Sets logical structure tree parent item for the text info.</summary>
        /// <remarks>
        /// Sets logical structure tree parent item for the text info. It allows to organize text chunks
        /// into logical hierarchy, e.g. specify document paragraphs, tables, etc.
        /// <para />
        /// If LogicalStructureTreeItem is set, then the list of
        /// <see cref="TextInfo"/>
        /// s in
        /// <see cref="IOcrEngine.DoImageOcr(System.IO.FileInfo)"/>
        /// return value is expected to be in logical order.
        /// </remarks>
        /// <param name="logicalStructureTreeItem">structure tree item.</param>
        public virtual void SetLogicalStructureTreeItem(LogicalStructureTreeItem logicalStructureTreeItem) {
            this.logicalStructureTreeItem = logicalStructureTreeItem;
        }
    }
}
