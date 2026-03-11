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
using iText.Pdfocr.Onnx.Exceptions;
using iText.Pdfocr.Util;
using iText.pdfOcr.Onnx;

namespace iText.Pdfocr.Onnx {
    /// <summary>
    /// Multidimensional array with a
    /// <see cref="FloatBufferWrapper"/>
    /// backing storage.
    /// </summary>
    public class FloatBufferMdArray {
        private readonly FloatBufferWrapper data;

        private readonly long[] shape;

        /// <summary>
        /// Constructs a new
        /// <c>FloatBufferMdArray</c>
        /// with the specified data buffer and shape.
        /// </summary>
        /// <param name="data">
        /// the
        /// <see cref="FloatBufferWrapper"/>
        /// containing the data for this array
        /// </param>
        /// <param name="shape">the shape of the multidimensional array, where each entry specifies the size of a dimension
        ///     </param>
        public FloatBufferMdArray(FloatBufferWrapper data, long[] shape) {
            Objects.RequireNonNull(data);
            Objects.RequireNonNull(shape);
            if (!ValidateShape(shape)) {
                throw new ArgumentException(PdfOcrOnnxExceptionMessageConstant.SHAPE_IS_NOT_VALID);
            }
            if (data.Remaining() != ElementCount(shape)) {
                throw new ArgumentException(PdfOcrOnnxExceptionMessageConstant.ELEM_COUNT_DOES_NOT_MATCH_SHAPE);
            }
            this.data = data.Duplicate();
            this.shape = (long[])shape.Clone();
        }

        /// <summary>
        /// Returns a duplicate of the backing
        /// <see cref="FloatBufferWrapper"/>.
        /// </summary>
        /// <returns>
        /// a duplicate of the backing
        /// <see cref="FloatBufferWrapper"/>
        /// </returns>
        public virtual FloatBufferWrapper GetData() {
            return data.Duplicate();
        }

        /// <summary>Returns a copy of the shape array that defines the dimensions of this multidimensional array.</summary>
        /// <returns>a copy of the shape array</returns>
        public virtual long[] GetShape() {
            return (long[])shape.Clone();
        }

        /// <summary>Returns the number of dimensions of this multidimensional array.</summary>
        /// <returns>the number of dimensions</returns>
        public virtual int GetDimensionCount() {
            return shape.Length;
        }

        /// <summary>Returns the size of the specified dimension.</summary>
        /// <param name="index">the zero-based index of the dimension to query</param>
        /// <returns>the size of the dimension at the specified index</returns>
        public virtual int GetDimension(int index) {
            if (index < 0 || index >= shape.Length) {
                throw new IndexOutOfRangeException();
            }
            return (int)shape[index];
        }

        /// <summary>Returns a sub-array representing the slice at the specified index of the first dimension.</summary>
        /// <param name="index">the index along the first dimension to retrieve</param>
        /// <returns>
        /// a
        /// <c>FloatBufferMdArray</c>
        /// representing the specified sub-array
        /// </returns>
        public virtual iText.Pdfocr.Onnx.FloatBufferMdArray GetSubArray(int index) {
            if (shape.Length == 0) {
                throw new InvalidOperationException();
            }
            if (index < 0 || index >= shape[0]) {
                throw new IndexOutOfRangeException();
            }
            long[] newShape = new long[shape.Length - 1];
            Array.Copy(shape, 1, newShape, 0, newShape.Length);
            int subArraySize = (data.Remaining() / (int)shape[0]);
            FloatBufferWrapper newData = data.Duplicate();
            newData.Position(index * subArraySize);
            newData = newData.Slice();
            newData.Limit(subArraySize);
            return new iText.Pdfocr.Onnx.FloatBufferMdArray(newData, newShape);
        }

        /// <summary>Returns the scalar value at the specified index.</summary>
        /// <remarks>
        /// Returns the scalar value at the specified index.
        /// <para />
        /// This method only works on one-dimensional arrays, where the total element count matches the size of the first dimension
        /// </remarks>
        /// <param name="index">the index of the scalar to retrieve</param>
        /// <returns>the scalar float value at the specified index</returns>
        public virtual float GetScalar(int index) {
            if (shape.Length != 0 && (ElementCount(shape) != shape[0])) {
                throw new InvalidOperationException();
            }
            return data.Get(index);
        }

        /// <summary>Gets internal offset of the provided float buffer array.</summary>
        /// <returns>internal offset</returns>
        public virtual int GetArrayOffset() {
            return data.ArrayOffset();
        }

        /// <summary>Gets number of available bytes for read from provided float buffer array.</summary>
        /// <returns>number of available bytes for read</returns>
        public virtual int GetArraySize() {
            return data.Limit();
        }

        private static bool ValidateShape(long[] shape) {
            bool valid = true;
            foreach (long l in shape) {
                valid &= l > 0L;
                valid &= (long)((int)l) == l;
            }
            return valid && shape.Length <= 8;
        }

//\cond DO_NOT_DOCUMENT
        internal static long ElementCount(long[] shape) {
            long count = 1L;
            foreach (long l in shape) {
                if (l < 0L) {
                    throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.NEGATIVE_VALUE_IN_SHAPE
                        , JavaUtil.ArraysToString(shape)));
                }
                count *= l;
            }
            return count;
        }
//\endcond
    }
}
