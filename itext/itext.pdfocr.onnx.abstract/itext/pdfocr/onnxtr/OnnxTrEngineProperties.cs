/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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

namespace iText.Pdfocr.Onnxtr {
    /// <summary>
    /// Properties that are used by the
    /// <see cref="OnnxTrOcrEngine"/>.
    /// </summary>
    public class OnnxTrEngineProperties {
        /// <summary>
        /// Creates a new
        /// <see cref="OnnxTrEngineProperties"/>
        /// instance.
        /// </summary>
        public OnnxTrEngineProperties() {
        }

        /// <summary>Defines the way text is retrieved and grouped from onnxtr engine output.</summary>
        /// <remarks>
        /// Defines the way text is retrieved and grouped from onnxtr engine output.
        /// It changes the way text is selected in the result pdf document.
        /// Does not affect the result of
        /// <see cref="iText.Pdfocr.IOcrEngine.CreateTxtFile(System.Collections.Generic.IList{E}, System.IO.FileInfo)"
        ///     />.
        /// </remarks>
        private iText.Pdfocr.Onnxtr.Text.TextPositioning textPositioning = iText.Pdfocr.Onnxtr.Text.TextPositioning
            .BY_WORDS_AND_LINES;

        /// <summary>
        /// Defines the way text is retrieved from ocr engine output using
        /// <see cref="TextPositioning"/>.
        /// </summary>
        /// <returns>the way text is retrieved</returns>
        [System.ObsoleteAttribute(@"in favour of GetTextPositioningMode()")]
        public virtual TextPositioning GetTextPositioning() {
            if (iText.Pdfocr.Onnxtr.Text.TextPositioning.BY_WORDS.Equals(textPositioning)) {
                return TextPositioning.BY_WORDS;
            }
            return TextPositioning.BY_LINES;
        }

        /// <summary>
        /// Gets the way text is retrieved from ocr engine output
        /// using
        /// <see cref="iText.Pdfocr.Onnxtr.Text.TextPositioning"/>.
        /// </summary>
        /// <returns>the way text is retrieved</returns>
        public virtual iText.Pdfocr.Onnxtr.Text.TextPositioning GetTextPositioningMode() {
            return textPositioning;
        }

        /// <summary>
        /// Defines the way text is retrieved from ocr engine output
        /// using
        /// <see cref="TextPositioning"/>.
        /// </summary>
        /// <param name="textPositioning">the way text is retrieved</param>
        /// <returns>
        /// the
        /// <see cref="OnnxTrEngineProperties"/>
        /// instance
        /// </returns>
        [System.ObsoleteAttribute(@"in favour of SetTextPositioning(iText.Pdfocr.Onnxtr.Text.TextPositioning)")]
        public virtual iText.Pdfocr.Onnxtr.OnnxTrEngineProperties SetTextPositioning(TextPositioning textPositioning
            ) {
            if (TextPositioning.BY_LINES.Equals(textPositioning)) {
                this.textPositioning = iText.Pdfocr.Onnxtr.Text.TextPositioning.BY_WORDS_AND_LINES;
            }
            else {
                this.textPositioning = iText.Pdfocr.Onnxtr.Text.TextPositioning.BY_WORDS;
            }
            return this;
        }

        /// <summary>
        /// Defines the way text is retrieved from ocr engine output
        /// using
        /// <see cref="iText.Pdfocr.Onnxtr.Text.TextPositioning"/>.
        /// </summary>
        /// <param name="textPositioning">the way text is retrieved</param>
        /// <returns>
        /// the
        /// <see cref="OnnxTrEngineProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Pdfocr.Onnxtr.OnnxTrEngineProperties SetTextPositioning(iText.Pdfocr.Onnxtr.Text.TextPositioning
             textPositioning) {
            this.textPositioning = textPositioning;
            return this;
        }
    }
}
