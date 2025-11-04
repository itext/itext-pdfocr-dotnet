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
using System;
using iText.Commons.Utils;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr.Util {
    /// <summary>A basic 2-element tuple with a width and a height.</summary>
    public class Dimensions2D {
        private readonly int width;

        private readonly int height;

        /// <summary>
        /// Creates new
        /// <see cref="Dimensions2D"/>
        /// instance.
        /// </summary>
        /// <param name="width">width dimension</param>
        /// <param name="height">height dimension</param>
        public Dimensions2D(int width, int height) {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Gets width of the
        /// <see cref="Dimensions2D"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// width of the
        /// <see cref="Dimensions2D"/>
        /// instance
        /// </returns>
        public virtual int GetWidth() {
            return width;
        }

        /// <summary>
        /// Gets height of the
        /// <see cref="Dimensions2D"/>
        /// instance.
        /// </summary>
        /// <returns>
        /// height of the
        /// <see cref="Dimensions2D"/>
        /// instance
        /// </returns>
        public virtual int GetHeight() {
            return height;
        }

        public override bool Equals(Object o) {
            if (o == null || this.GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnxtr.Util.Dimensions2D that = (iText.Pdfocr.Onnxtr.Util.Dimensions2D)o;
            return width == that.width && height == that.height;
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(width, height);
        }

        public override String ToString() {
            return width + "x" + height;
        }
    }
}
