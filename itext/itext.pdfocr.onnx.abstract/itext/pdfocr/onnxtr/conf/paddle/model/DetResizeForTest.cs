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
    /// POJO for the DetResizeForTest transform operation within a
    /// <c>PreProcess</c>
    /// object in a config file.
    /// </summary>
    public class DetResizeForTest : TransformOp {
        /// <summary>Expected wrapping key for the DetResizeForTest operation.</summary>
        public const String WRAPPING_KEY = "DetResizeForTest";

        /*
        * With how this config is read within PaddleOCR in a regular OCR case,
        * only a small subset of configuration options is used. Everything else
        * is taken from a global "OCR" configuration.
        *
        * Only useful fields are set up here.
        */
        private readonly int[] imageShape;

        private readonly bool keepRatio;

        /// <summary>Creates a new POJO for the config file object.</summary>
        /// <param name="imageShape">
        /// value under the
        /// <c>image_shape</c>
        /// key, optional
        /// </param>
        /// <param name="keepRatio">
        /// value under the
        /// <c>keep_ratio</c>
        /// key
        /// </param>
        public DetResizeForTest(int[] imageShape, bool keepRatio) {
            this.imageShape = imageShape;
            this.keepRatio = keepRatio;
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetWrappingKey() {
            return WRAPPING_KEY;
        }

        /// <summary>
        /// Returns the values under the
        /// <c>image_shape</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the values under the
        /// <c>image_shape</c>
        /// key
        /// </returns>
        public virtual int[] GetImageShape() {
            return imageShape;
        }

        /// <summary>
        /// Returns the value under the
        /// <c>keep_ratio</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the value under the
        /// <c>keep_ratio</c>
        /// key
        /// </returns>
        public virtual bool GetKeepRatio() {
            return keepRatio;
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object o) {
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnxtr.Conf.Paddle.Model.DetResizeForTest that = (iText.Pdfocr.Onnxtr.Conf.Paddle.Model.DetResizeForTest
                )o;
            return keepRatio == that.keepRatio && JavaUtil.ArraysEquals(imageShape, that.imageShape);
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)JavaUtil.ArraysHashCode(imageShape), keepRatio);
        }
    }
}
