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
using System.Text;
using iText.Pdfocr.Onnx;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnx.Recognition {
    /// <summary>
    /// Implementation of a text recognition predictor post-processor, used for
    /// EasyOCR and PaddleOCR model outputs.
    /// </summary>
    /// <remarks>
    /// Implementation of a text recognition predictor post-processor, used for
    /// EasyOCR and PaddleOCR model outputs.
    /// <para />
    /// It has a single blank token, which is the first one just before the
    /// vocabulary. Multiple of the same label in a row is aggregated into one.
    /// </remarks>
    public class CtcLabelPostProcessor : BasicLabelPostProcessor {
        /// <summary>Label mapper used for the model output (without special tokens).</summary>
        private readonly IOutputLabelMapper<String> labelMapper;

        /// <summary>Creates a new post-processor.</summary>
        /// <param name="labelMapper">label mapper used for the model output (without special tokens)</param>
        public CtcLabelPostProcessor(IOutputLabelMapper<String> labelMapper) {
            this.labelMapper = Objects.RequireNonNull(labelMapper);
        }

        /// <summary><inheritDoc/></summary>
        protected internal override void AppendLabel(StringBuilder output, int labelIndex) {
            // First letter is <blank>
            if (labelIndex > 0) {
                output.Append(labelMapper.Map(labelIndex - 1));
            }
        }

        /// <summary><inheritDoc/></summary>
        public override int LabelDimension() {
            // First token is "[blank]"
            return 1 + labelMapper.Size();
        }
    }
}
