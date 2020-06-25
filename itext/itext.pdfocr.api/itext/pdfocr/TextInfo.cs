/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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

namespace iText.Pdfocr {
    /// <summary>
    /// This class describes how recognized text is positioned on the image
    /// providing bbox for each text item (could be a line or a word).
    /// </summary>
    public class TextInfo {
        /// <summary>Contains any text.</summary>
        private String text;

        /// <summary>Contains 4 float coordinates: bbox parameters.</summary>
        private IList<float> bbox;

        /// <summary>
        /// Creates a new
        /// <see cref="TextInfo"/>
        /// instance.
        /// </summary>
        public TextInfo() {
            text = null;
            bbox = JavaCollectionsUtil.EmptyList<float>();
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
        public TextInfo(String text, IList<float> bbox) {
            this.text = text;
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
        /// <see cref="System.Collections.IList{E}"/>
        /// of bbox parameters
        /// </returns>
        public virtual IList<float> GetBbox() {
            return new List<float>(bbox);
        }

        /// <summary>Sets bbox coordinates.</summary>
        /// <param name="bbox">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of bbox parameters
        /// </param>
        public virtual void SetBbox(IList<float> bbox) {
            this.bbox = JavaCollectionsUtil.UnmodifiableList<float>(bbox);
        }
    }
}
