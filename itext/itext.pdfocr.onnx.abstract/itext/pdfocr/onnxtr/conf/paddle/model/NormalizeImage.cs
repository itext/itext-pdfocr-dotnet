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
    /// POJO for the NormalizeImage transform operation within a
    /// <c>PreProcess</c>
    /// object in a config file.
    /// </summary>
    public class NormalizeImage : TransformOp {
        /// <summary>Expected wrapping key for the NormalizeImage operation.</summary>
        public const String WRAPPING_KEY = "NormalizeImage";

        /*
        * Not including the "order" field here, as in our logic we are always
        * working with CHW anyway. Also skipping scale, as we do 1 / 255 anyway...
        */
        private readonly float[] mean;

        private readonly float[] std;

        /// <summary>Creates a new POJO for the config file object.</summary>
        /// <param name="mean">
        /// values under the
        /// <paramref name="mean"/>
        /// key
        /// </param>
        /// <param name="std">
        /// values under the
        /// <paramref name="std"/>
        /// key
        /// </param>
        public NormalizeImage(float[] mean, float[] std) {
            this.mean = Objects.RequireNonNull(mean);
            this.std = Objects.RequireNonNull(std);
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetWrappingKey() {
            return WRAPPING_KEY;
        }

        /// <summary>
        /// Returns the values under the
        /// <c>mean</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the values under the
        /// <c>mean</c>
        /// key
        /// </returns>
        public virtual float[] GetMean() {
            return mean;
        }

        /// <summary>
        /// Returns the values under the
        /// <c>std</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the values under the
        /// <c>std</c>
        /// key
        /// </returns>
        public virtual float[] GetStd() {
            return std;
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object o) {
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnxtr.Conf.Paddle.Model.NormalizeImage that = (iText.Pdfocr.Onnxtr.Conf.Paddle.Model.NormalizeImage
                )o;
            return JavaUtil.ArraysEquals(mean, that.mean) && JavaUtil.ArraysEquals(std, that.std);
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(JavaUtil.ArraysHashCode(mean), JavaUtil.ArraysHashCode(std));
        }
    }
}
