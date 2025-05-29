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
using System.Linq;

namespace iText.Pdfocr.Onnxtr {
    /// <summary>
    /// Multidimensional array with a
    /// <see cref="Java.Nio.FloatBuffer"/>
    /// backing storage.
    /// </summary>
    public class FloatBufferMdArray {
        private readonly float[] data;

        private readonly long[] shape;

        public FloatBufferMdArray(float[] data, long[] shape) {
            this.data = ((float[])data.Clone());
            this.shape = (long[])shape.Clone();
        }

        public virtual float[] GetData() {
            return ((float[])data.Clone());
        }

        public virtual long[] GetShape() {
            return (long[])shape.Clone();
        }

        public virtual int GetDimensionCount() {
            return shape.Length;
        }

        public virtual int GetDimension(int index) {
            if (index < 0 || index >= shape.Length) {
                throw new IndexOutOfRangeException();
            }
            return (int)shape[index];
        }

        public virtual iText.Pdfocr.Onnxtr.FloatBufferMdArray GetSubArray(int index) {
            if (shape.Length == 0) {
                throw new InvalidOperationException();
            }
            if (index < 0 || index >= shape[0]) {
                throw new IndexOutOfRangeException();
            }
            long[] newShape = new long[shape.Length - 1];
            Array.Copy(shape, 1, newShape, 0, newShape.Length);
            int subArraySize = (data.Length / (int)shape[0]);
            float[] newData = ((float[])data.Clone()).Skip(index * subArraySize).Take(subArraySize).ToArray();
            return new iText.Pdfocr.Onnxtr.FloatBufferMdArray(newData, newShape);
        }

        public virtual float GetScalar(int index) {
            return data[index];
        }
    }
}
