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

namespace iText.Pdfocr.Onnx {
    /// <summary>
    /// Wrapper class around float array implementing functionality of the FloatBuffer from Java.
    /// </summary>
    public class FloatBufferWrapper {

        private readonly float[] floatBuffer;
        private readonly int offset;
        private int position;
        private int limit;

        /// <summary>
        /// Constructs
        /// <see cref="FloatBufferWrapper"/>
        /// on top of float array
        /// </summary>
        /// <param name="floatBuffer">buffer on top of which <see cref="FloatBufferWrapper"/> will be built</param>
        public FloatBufferWrapper(float[] data) : this(data, 0, data.Length, 0) {
        }

        /// <summary>
        /// Constructs
        /// <see cref="FloatBufferWrapper"/>
        /// on top of float array
        /// </summary>
        /// <param name="floatBuffer">buffer on top of which <see cref="FloatBufferWrapper"/> will be built</param>
        /// <param name="position">position from which buffer will start</param>
        /// <param name="limit">number of elements buffer can access</param>
        /// <param name="offset">offset from which buffer will start</param>
        internal FloatBufferWrapper(float[] data, int position, int limit, int offset) {
            this.floatBuffer = data;
            this.position = position;
            this.limit = limit;
            this.offset = offset;
        }

        /// <summary>
        /// Returns the float array that backs this
        /// buffer.
        /// </summary>
        /// <remarks>
        /// Returns the float array that backs this buffer.
        /// <para />
        /// Modifications to this buffer's content will cause the returned
        /// array's content to be modified, and vice versa.
        /// </remarks>
        /// <returns>the array that backs this buffer</returns>
        public virtual float[] Array() => this.floatBuffer;

        /// <summary>
        /// Returns the offset within this buffer's backing array of the first
        /// element of the buffer.
        /// </summary>
        /// <remarks>
        /// Returns the offset within this buffer's backing array of the first
        /// element of the buffer.
        /// <para />
        /// If this buffer is backed by an array then buffer position
        /// corresponds to array index position.
        /// </remarks>
        /// <returns>
        /// the offset within this buffer's array
        /// of the first element of the buffer
        /// </returns>
        public virtual int ArrayOffset() => this.offset;

        /// <summary>Relative get method.</summary>
        /// <remarks>
        /// Relative get method.  Reads the float at this buffer's
        /// current position, and then increments the position.
        /// </remarks>
        /// <returns>the float at the buffer's current position</returns>
        public virtual float Get() {
            if (position >= limit) {
                throw new InvalidOperationException($"Position {position} exceeds limit {limit}.");
            }

            float value = this.floatBuffer[offset + position];
            position++;
            return value; 
        }

        /// <summary>Absolute get method.</summary>
        /// <remarks>Absolute get method. Reads the float at the given index.</remarks>
        /// <param name="index">the index from which the float will be read</param>
        /// <returns>the float at the given index</returns>
        public virtual float Get(int index) {
            if (index < 0 || index >= limit) {
                throw new IndexOutOfRangeException($"Index {index} is out of bounds for limit [0, {limit})!");
            }
            return this.floatBuffer[offset + index]; 
        }

        /// <summary>Relative bulk <i>get</i> method.</summary>
        /// <remarks>
        /// Relative bulk <i>get</i> method.
        /// <para />
        /// This method transfers floats from this buffer into the given
        /// destination array.
        /// </remarks>
        /// <param name="dst">the destination array</param>
        /// <returns>this buffer</returns>
        public virtual FloatBufferWrapper Get(float[] dst) {
            int remaining = Remaining();
            if (dst.Length < remaining) {
                throw new ArgumentException($"Destination array is too small!\n" +
                                            $"Destination array size is {dst.Length}, " +
                                            $"remaining is {remaining}.");
            }

            System.Array.Copy(floatBuffer, offset + position, dst, 0, remaining);
            position += remaining;
            return this;
        }

        /// <summary>Rewinds this buffer.</summary>
        /// <remarks>
        /// Rewinds this buffer.  The position is set to zero.
        /// <para />
        /// Invoke this method before a sequence of channel-write or get
        /// operations, assuming that the limit has already been set
        /// appropriately.
        /// </remarks>
        /// <returns>this buffer</returns>
        public virtual FloatBufferWrapper Rewind() {
            position = 0;
            return this;
        }

        /// <summary>Relative put method.</summary>
        /// <remarks>
        /// Relative put method.
        /// <para />
        /// Writes the given float into this buffer at the current
        /// position, and then increments the position.
        /// </remarks>
        /// <param name="value">the float to be written</param>
        /// <returns>this buffer</returns>
        public virtual FloatBufferWrapper Put(float value) {
            if (position >= limit) {
                throw new InvalidOperationException($"Position {position} exceeds limit {limit}.");
            }
            floatBuffer[offset + position] = value;
            position++;
            return this;
        }

        /// <summary>Relative bulk put method.</summary>
        /// <remarks>
        /// Relative bulk put method.
        /// <para />
        /// This method transfers floats into this buffer from the given source array.
        /// </remarks>
        /// <param name="src">the array from which floats are to be read</param>
        /// <param name="srcOffset">
        /// the offset within the array of the first float to be read;
        /// must be non-negative and no larger than
        /// <c>array.length</c>
        /// </param>
        /// <param name="length">
        /// the number of floats to be read from the given array;
        /// must be non-negative and no larger than
        /// <c>array.length - offset</c>
        /// </param>
        /// <returns>this buffer</returns>
        public virtual FloatBufferWrapper Put(float[] src, int srcOffset, int length) {
            if (position + length > limit) {
                throw new InvalidOperationException($"Position {position + length} exceeds limit {limit}.");
            }
            System.Array.Copy(src, srcOffset, floatBuffer, srcOffset + position, length);
            position += length;
            return this;
        }

        /// <summary>Returns this buffer's limit.</summary>
        /// <returns>the limit of this buffer</returns>
        public virtual int Limit() => limit;

        /// <summary>Sets this buffer's limit.</summary>
        /// <remarks>
        /// Sets this buffer's limit. If the position is larger than the new limit
        /// then it is set to the new limit.
        /// </remarks>
        /// <param name="newLimit">the new limit value; must be non-negative and no larger than this buffer's capacity
        ///     </param>
        /// <returns>this buffer</returns>
        public virtual FloatBufferWrapper Limit(int newLimit) {
            int capacity = floatBuffer.Length - offset;
            if (newLimit < 0 || newLimit > capacity) {
                throw new ArgumentOutOfRangeException($"Limit {newLimit} is out of range for indexes [0, {capacity}]!");
            }

            limit = newLimit;
            if (position > newLimit) {
                position = newLimit;
            }

            return this;
        }

        /// <summary>Creates a new float buffer that shares this buffer's content.</summary>
        /// <remarks>
        /// Creates a new float buffer that shares this buffer's content.
        /// <para />
        /// The content of the new buffer will be that of this buffer.  Changes
        /// to this buffer's content will be visible in the new buffer, and vice
        /// versa; the two buffers' position and limit will be
        /// independent.
        /// <para />
        /// The new buffer's capacity, limit, position and byte order will be identical to those of this buffer.
        /// </remarks>
        /// <returns>the new float buffer</returns>
        public virtual FloatBufferWrapper Duplicate() {
            return new FloatBufferWrapper(floatBuffer, position, limit, offset);
        }

        /// <summary>Returns the number of elements between the current position and the limit.</summary>
        /// <returns>the number of elements remaining in this buffer</returns>
        public virtual int Remaining() {
            int rem = limit - position;
            return rem > 0 ? rem : 0;
        }

        /// <summary>Sets this buffer's position.</summary>
        /// <param name="newPosition">the new position value; must be non-negative and no larger than the current limit
        ///     </param>
        /// <returns>this buffer</returns>
        public virtual FloatBufferWrapper Position(int newPosition) {
            if (newPosition < 0 || newPosition > limit) {
                throw new ArgumentOutOfRangeException($"Position {newPosition} is out of range " +
                                                      $"for indexes [0, {limit}]!");
            }
            position = newPosition;
            return this;
        }

        /// <summary>
        /// Creates a new float buffer whose content is a shared subsequence of
        /// this buffer's content.
        /// </summary>
        /// <remarks>
        /// Creates a new float buffer whose content is a shared subsequence of
        /// this buffer's content.
        /// <para />
        /// The content of the new buffer will start at this buffer's current
        /// position.  Changes to this buffer's content will be visible in the new
        /// buffer, and vice versa; the two buffers' position and limit values will be independent.
        /// <para />
        /// The new buffer's position will be zero, its capacity and its limit
        /// will be the number of floats remaining in this buffer and its byte order
        /// will be identical to that of this buffer.
        /// </remarks>
        /// <returns>the new float buffer</returns>
        public virtual FloatBufferWrapper Slice() {
            return new FloatBufferWrapper(floatBuffer, 0, limit - position, offset + position);
        }

        /// <summary>Wraps a float array into a buffer.</summary>
        /// <remarks>
        /// Wraps a float array into a buffer.
        /// <para />
        /// The new buffer will be backed by the given float array;
        /// that is, modifications to the buffer will cause the array to be modified
        /// and vice versa.  The new buffer's capacity and limit will be
        /// <c>array.length</c>
        /// , its position will be zero and its byte order
        /// will be the
        /// <see cref="ByteOrder.NativeOrder()">native order</see>
        /// of the underlying hardware.
        /// Its
        /// <see cref="Array()">backing array</see>
        /// will be the given array, and its
        /// <see cref="ArrayOffset()">array offset</see>
        /// will be zero.
        /// </remarks>
        /// <param name="array">the array that will back this buffer</param>
        /// <returns>the new float buffer</returns>
        public static FloatBufferWrapper Wrap(float[] array) {
            return new FloatBufferWrapper(array);
        }

        /// <summary>Allocates a new float buffer.</summary>
        /// <remarks>
        /// Allocates a new float buffer.
        /// <para />
        /// The new buffer's position will be zero, its limit will be its
        /// capacity, its mark will be undefined, each of its elements will be
        /// initialized to zero, and its byte order will be
        /// the
        /// <see cref="ByteOrder.NativeOrder()">native order</see>
        /// of the underlying
        /// hardware.
        /// It will have a
        /// <see cref="Array()">backing array</see>
        /// , and its
        /// <see cref="ArrayOffset()">array offset</see>
        /// will be zero.
        /// </remarks>
        /// <param name="capacity">the new buffer's capacity, in floats</param>
        /// <returns>the new float buffer</returns>
        public static FloatBufferWrapper Allocate(int capacity) {
            return new FloatBufferWrapper(new float[capacity]);
        }
    }
}
