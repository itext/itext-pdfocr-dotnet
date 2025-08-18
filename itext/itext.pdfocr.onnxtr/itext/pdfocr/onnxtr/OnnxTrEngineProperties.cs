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
        private TextPositioning textPositioning;

        /// <summary>
        /// Defines the way text is retrieved from ocr engine output using
        /// <see cref="TextPositioning"/>.
        /// </summary>
        /// <returns>the way text is retrieved</returns>
        public virtual TextPositioning GetTextPositioning() {
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
        public virtual iText.Pdfocr.Onnxtr.OnnxTrEngineProperties SetTextPositioning(TextPositioning textPositioning
            ) {
            this.textPositioning = textPositioning;
            return this;
        }
    }
}
