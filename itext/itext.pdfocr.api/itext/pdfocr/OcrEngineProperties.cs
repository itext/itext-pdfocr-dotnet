/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
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
using iText.Commons.Utils;

namespace iText.Pdfocr {
    /// <summary>This class contains additional properties for ocr engine.</summary>
    public class OcrEngineProperties {
        /// <summary>List of languages required for ocr for provided images.</summary>
        private IList<String> languages = JavaCollectionsUtil.EmptyList<String>();

        /// <summary>
        /// Creates a new
        /// <see cref="OcrEngineProperties"/>
        /// instance.
        /// </summary>
        public OcrEngineProperties() {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="OcrEngineProperties"/>
        /// instance
        /// based on another
        /// <see cref="OcrEngineProperties"/>
        /// instance (copy
        /// constructor).
        /// </summary>
        /// <param name="other">
        /// the other
        /// <see cref="OcrEngineProperties"/>
        /// instance
        /// </param>
        public OcrEngineProperties(iText.Pdfocr.OcrEngineProperties other) {
            this.languages = other.languages;
        }

        /// <summary>Gets list of languages required for provided images.</summary>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of languages
        /// </returns>
        public IList<String> GetLanguages() {
            return new List<String>(languages);
        }

        /// <summary>Sets list of languages to be recognized in provided images.</summary>
        /// <remarks>
        /// Sets list of languages to be recognized in provided images.
        /// Consult with documentation of specific engine implementations
        /// to check on which format to give the language in.
        /// </remarks>
        /// <param name="requiredLanguages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of languages in string
        /// format
        /// </param>
        /// <returns>
        /// the
        /// <see cref="OcrEngineProperties"/>
        /// instance
        /// </returns>
        public iText.Pdfocr.OcrEngineProperties SetLanguages(IList<String> requiredLanguages) {
            languages = JavaCollectionsUtil.UnmodifiableList<String>(requiredLanguages);
            return this;
        }
    }
}
