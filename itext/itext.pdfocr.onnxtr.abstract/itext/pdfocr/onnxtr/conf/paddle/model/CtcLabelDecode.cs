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
using iText.Commons.Utils;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr.Conf.Paddle.Model {
    /// <summary>
    /// POJO for the CTCLabelDecode post-processor object under a
    /// <c>PostProcess</c>
    /// key in a config file.
    /// </summary>
    public class CtcLabelDecode : PostProcess {
        /// <summary>Expected name for the CTCLabelDecode post-processor.</summary>
        public const String NAME = "CTCLabelDecode";

        private readonly String[] characterDict;

        /// <summary>Creates a new POJO for the config file object.</summary>
        /// <param name="characterDict">
        /// values under the
        /// <c>character_dict</c>
        /// key
        /// </param>
        public CtcLabelDecode(String[] characterDict) {
            this.characterDict = Objects.RequireNonNull(characterDict);
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetName() {
            return NAME;
        }

        /// <summary>
        /// Returns the value under the
        /// <c>character_dict</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the value under the
        /// <c>character_dict</c>
        /// key
        /// </returns>
        public virtual String[] GetCharacterDict() {
            return characterDict;
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object o) {
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnxtr.Conf.Paddle.Model.CtcLabelDecode that = (iText.Pdfocr.Onnxtr.Conf.Paddle.Model.CtcLabelDecode
                )o;
            return JavaUtil.ArraysEquals(characterDict, that.characterDict);
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(characterDict);
        }
    }
}
