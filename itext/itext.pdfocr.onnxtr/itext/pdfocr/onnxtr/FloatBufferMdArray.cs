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
using System.Linq;
using iText.Commons.Utils;
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr {
    /// <summary>
    /// Multidimensional array with a
    /// <see cref="Java.Nio.FloatBuffer"/>
    /// backing storage.
    /// </summary>
    public class FloatBufferMdArray {
        private readonly float[] data;

        private readonly long[] shape;

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatBufferMdArray"/> class with the specified data buffer and shape.
        /// </summary>
        /// 
        /// <param name="data">The <see cref="FloatBuffer"/> containing the data for this array</param>
        /// <param name="shape">The shape of the multidimensional array, where each entry specifies the size of a dimension</param>
        /// 
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> or <paramref name="shape"/> is <c>null</c></exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="shape"/> is invalid or the number of elements in <paramref name="data"/>
        /// does not match the element count derived from <paramref name="shape"/>.</exception>
        public FloatBufferMdArray(float[] data, long[] shape) {
            Objects.RequireNonNull(data);
            Objects.RequireNonNull(shape);
            if (!ValidateShape(shape)) {
                throw new ArgumentException(PdfOcrOnnxTrExceptionMessageConstant.SHAPE_IS_NOT_VALID);
            }
            if (data.Length != ElementCount(shape)) {
                throw new ArgumentException(PdfOcrOnnxTrExceptionMessageConstant.ELEM_COUNT_DOES_NOT_MATCH_SHAPE);
            }
            this.data = data;
            this.shape = (long[])shape.Clone();
        }

        /// <summary>
        /// Returns the backing float array.
        /// </summary>
        /// 
        /// <returns>The backing float array</returns>
        public virtual float[] GetData() {
            return data;
        }

        /// <summary>
        /// Returns a copy of the shape array that defines the dimensions of this multidimensional array.
        /// </summary>
        /// 
        /// <returns>A copy of the shape array</returns>
        public virtual long[] GetShape() {
            return (long[])shape.Clone();
        }

        /// <summary>
        /// Returns the number of dimensions of this multidimensional array.
        /// </summary>
        /// 
        /// <returns>The number of dimensions</returns>
        public virtual int GetDimensionCount() {
            return shape.Length;
        }

        /// <summary>
        /// Returns the size of the specified dimension.
        /// </summary>
        /// 
        /// <param name="index">The zero-based index of the dimension to query</param>
        /// 
        /// <returns>The size of the dimension at the specified index</returns>
        /// 
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown if <paramref name="index"/> is negative or greater than or equal to the number of dimensions.
        /// </exception>
        public virtual int GetDimension(int index) {
            if (index < 0 || index >= shape.Length) {
                throw new IndexOutOfRangeException();
            }
            return (int)shape[index];
        }

        /// <summary>
        /// Returns a sub-array representing the slice at the specified index of the first dimension.
        /// </summary>
        /// 
        /// <param name="index">The index along the first dimension to retrieve</param>
        /// 
        /// <returns>A <see cref="FloatBufferMdArray"/> representing the specified sub-array</returns>
        /// 
        /// <exception cref="InvalidOperationException">
        /// Thrown if this array has no dimensions
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown if <paramref name="index"/> is negative or exceeds the bounds of the first dimension
        /// </exception>
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
            float[] newData = data.Skip(index * subArraySize).Take(subArraySize).ToArray();
            return new iText.Pdfocr.Onnxtr.FloatBufferMdArray(newData, newShape);
        }

        /// <summary>
        /// Returns the scalar value at the specified index.
        /// </summary>
        /// 
        /// <remarks>
        /// This method only works on one-dimensional arrays, where the total element count matches the size of the first dimension.
        /// </remarks>
        /// 
        /// <param name="index">The index of the scalar to retrieve</param>
        /// <returns>The scalar <c>float</c> value at the specified index</returns>
        /// 
        /// <exception cref="InvalidOperationException">
        /// Thrown if this array is not properly shaped as a one-dimensional array
        /// </exception>
        public virtual float GetScalar(int index) {
            if (shape.Length != 0 && (ElementCount(shape) != shape[0])) {
                throw new InvalidOperationException();
            }
            return data[index];
        }

        /// <summary>
        /// Gets the internal offset of the provided float buffer array.
        /// </summary>
        /// 
        /// <returns>The internal offset</returns>
        public int GetArrayOffset() {
            return 0;
        }

        /// <summary>
        /// Gets the number of available bytes for reading from the provided float buffer array.
        /// </summary>
        /// 
        /// <returns>The number of available bytes for read</returns>
        public int GetArraySize() {
            return data.Length;
        }

        private bool ValidateShape(long[] shape) {
            Boolean valid = true;

            for (int i = 0; i < shape.Length; ++i) {
                valid &= shape[i] > 0L;
                valid &= (long)((int)shape[i]) == shape[i];
            }

            return valid && shape.Length <= 8;
        }

        private long ElementCount(long[] shape) {
            long count = 1L;

            for (int i = 0; i < shape.Length; ++i) {
                if (shape[i] < 0L) {
                    throw new ArgumentException(MessageFormatUtil.Format(
                        PdfOcrOnnxTrExceptionMessageConstant.NEGATIVE_VALUE_IN_SHAPE, shape));
                }
                count *= shape[i];
            }

            return count;
        }
    }
}
