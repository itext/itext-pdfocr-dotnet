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
using iText.Test;

namespace iText.Pdfocr.Onnx {
    [NUnit.Framework.Category("UnitTest")]
    public class FloatBufferWrapperTest : ExtendedITextTest {
        private FloatBufferWrapper floatBufferWrapper;

        private float[] array;

        [NUnit.Framework.SetUp]
        public virtual void SetUp() {
            array = new float[] { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f };
            floatBufferWrapper = FloatBufferWrapper.Wrap(array);
        }

        [NUnit.Framework.Test]
        public virtual void WrapTest() {
            NUnit.Framework.Assert.AreEqual(array, floatBufferWrapper.Array());
        }

        [NUnit.Framework.Test]
        public virtual void OffsetTest() {
            NUnit.Framework.Assert.AreEqual(0, floatBufferWrapper.ArrayOffset());
        }

        [NUnit.Framework.Test]
        public virtual void GetTest() {
            NUnit.Framework.Assert.AreEqual(1.0f, floatBufferWrapper.Get());
            NUnit.Framework.Assert.AreEqual(2.0f, floatBufferWrapper.Get());
            NUnit.Framework.Assert.AreEqual(3.0f, floatBufferWrapper.Get());
        }

        [NUnit.Framework.Test]
        public virtual void Get2Test() {
            NUnit.Framework.Assert.AreEqual(3.0f, floatBufferWrapper.Get(2));
        }

        [NUnit.Framework.Test]
        public virtual void Get3Test() {
            float[] actualArray = new float[5];
            floatBufferWrapper.Get(actualArray);
            NUnit.Framework.Assert.AreEqual(array, actualArray);
        }

        [NUnit.Framework.Test]
        public virtual void RewindTest() {
            floatBufferWrapper.Get();
            floatBufferWrapper.Rewind();
            NUnit.Framework.Assert.AreEqual(1.0f, floatBufferWrapper.Get());
        }

        [NUnit.Framework.Test]
        public virtual void PutTest() {
            floatBufferWrapper.Put(6.0f);
            NUnit.Framework.Assert.AreEqual(6.0f, floatBufferWrapper.Get(0));
        }

        [NUnit.Framework.Test]
        public virtual void LimitTest() {
            NUnit.Framework.Assert.AreEqual(5, floatBufferWrapper.Limit());
        }

        [NUnit.Framework.Test]
        public virtual void Limit2Test() {
            floatBufferWrapper.Limit(2);
            NUnit.Framework.Assert.AreEqual(2, floatBufferWrapper.Limit());
        }

        [NUnit.Framework.Test]
        public virtual void DuplicateTest() {
            floatBufferWrapper.Position(2);
            floatBufferWrapper.Limit(4);
            FloatBufferWrapper duplicate = floatBufferWrapper.Duplicate();
            NUnit.Framework.Assert.AreEqual(floatBufferWrapper.Array(), duplicate.Array());
            NUnit.Framework.Assert.AreEqual(floatBufferWrapper.Limit(), duplicate.Limit());
            NUnit.Framework.Assert.AreEqual(floatBufferWrapper.Get(), duplicate.Get());
        }

        [NUnit.Framework.Test]
        public virtual void RemainingTest() {
            NUnit.Framework.Assert.AreEqual(5, floatBufferWrapper.Remaining());
        }

        [NUnit.Framework.Test]
        public virtual void PositionTest() {
            floatBufferWrapper.Position(2);
            NUnit.Framework.Assert.AreEqual(3.0f, floatBufferWrapper.Get());
        }

        [NUnit.Framework.Test]
        public virtual void SliceTest() {
            floatBufferWrapper.Position(2);
            FloatBufferWrapper newBuffer = floatBufferWrapper.Slice();
            NUnit.Framework.Assert.AreEqual(array, newBuffer.Array());
            NUnit.Framework.Assert.AreEqual(2, newBuffer.ArrayOffset());
        }

        [NUnit.Framework.Test]
        public virtual void AllocateTest() {
            FloatBufferWrapper buffer = FloatBufferWrapper.Allocate(3);
            NUnit.Framework.Assert.AreEqual(3, buffer.Limit());
        }
    }
}
