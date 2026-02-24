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
    /// POJO for the object under the
    /// <c>PreProcess</c>
    /// key in a config file.
    /// </summary>
    public class PreProcess {
        private readonly TransformOp[] transformOps;

        /// <summary>Creates a new POJO for the config file object.</summary>
        /// <param name="transformOps">
        /// value under the
        /// <c>transform_ops</c>
        /// key
        /// </param>
        public PreProcess(TransformOp[] transformOps) {
            this.transformOps = Objects.RequireNonNull(transformOps);
        }

        /// <summary>
        /// Returns the values under the
        /// <c>transform_ops</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the values under the
        /// <c>transform_ops</c>
        /// key
        /// </returns>
        public virtual TransformOp[] GetTransformOps() {
            return transformOps;
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object o) {
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnxtr.Conf.Paddle.Model.PreProcess that = (iText.Pdfocr.Onnxtr.Conf.Paddle.Model.PreProcess)
                o;
            return Objects.DeepEquals(transformOps, that.transformOps);
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(transformOps);
        }
    }
}
