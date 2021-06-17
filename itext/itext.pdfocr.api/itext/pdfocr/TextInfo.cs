/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

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
using System.Collections.Generic;
using iText.IO.Util;
using iText.Kernel.Geom;

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

        /// <summary>Contains 4 float coordinates: bbox parameters.</summary>
        /// <remarks>
        /// Contains 4 float coordinates: bbox parameters.
        /// Alike bboxRect described by
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// coordinates are upper-left based and expressed in pixels.
        /// </remarks>
        [System.ObsoleteAttribute(@"since 1.0.1. Use bboxRect instead")]
        private IList<float> bbox = JavaCollectionsUtil.EmptyList<float>();

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
            this.bbox = JavaCollectionsUtil.UnmodifiableList<float>(textInfo.bbox);
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
        /// <see cref="System.Collections.IList{E}"/>
        /// of bbox parameters
        /// </param>
        [System.ObsoleteAttribute(@"since 1.0.1. Use TextInfo(System.String, iText.Kernel.Geom.Rectangle) instead"
            )]
        public TextInfo(String text, IList<float> bbox) {
            this.text = text;
            this.bbox = JavaCollectionsUtil.UnmodifiableList<float>(bbox);
        }

        /// <summary>
        /// Creates a new
        /// <see cref="TextInfo"/>
        /// instance.
        /// </summary>
        /// <param name="text">any text</param>
        /// <param name="bboxRect">
        /// 
        /// <see cref="iText.Kernel.Geom.Rectangle"/>
        /// describing text bbox
        /// </param>
        /// <param name="bbox">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of bbox parameters
        /// </param>
        [System.ObsoleteAttribute(@"since 1.0.1. Use TextInfo(System.String, iText.Kernel.Geom.Rectangle) instead"
            )]
        public TextInfo(String text, Rectangle bboxRect, IList<float> bbox) {
            this.text = text;
            this.bboxRect = bboxRect;
            this.bbox = JavaCollectionsUtil.UnmodifiableList<float>(bbox);
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
            this.bbox = JavaCollectionsUtil.EmptyList<float>();
        }

        /// <summary>Gets bbox coordinates.</summary>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of bbox parameters
        /// </returns>
        [System.ObsoleteAttribute(@"since 1.0.1. Use GetBboxRect() instead")]
        public virtual IList<float> GetBbox() {
            return new List<float>(bbox);
        }

        /// <summary>Sets bbox coordinates.</summary>
        /// <param name="bbox">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of bbox parameters
        /// </param>
        [System.ObsoleteAttribute(@"since 1.0.1. Use SetBboxRect(iText.Kernel.Geom.Rectangle) instead")]
        public virtual void SetBbox(IList<float> bbox) {
            this.bbox = JavaCollectionsUtil.UnmodifiableList<float>(bbox);
            this.bboxRect = null;
        }
    }
}
