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

//\cond DO_NOT_DOCUMENT
        internal virtual IMetaInfo GetMetaInfo() {
            return metaInfo;
        }
//\endcond
    }
}
