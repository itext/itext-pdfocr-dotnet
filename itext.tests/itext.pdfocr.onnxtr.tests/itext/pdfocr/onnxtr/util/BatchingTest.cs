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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Pdfocr.Onnxtr.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class BatchingTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void WrapWithValidArgs() {
            IEnumerator<IList<int>> wrapped = Batching.Wrap(JavaUtil.ArraysAsList(1, 2, 3, 4, 5, 6, 7).GetEnumerator()
                , 2);
            NUnit.Framework.Assert.IsTrue(wrapped.MoveNext());
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(1, 2), wrapped.Current);
            NUnit.Framework.Assert.IsTrue(wrapped.MoveNext());
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(3, 4), wrapped.Current);
            NUnit.Framework.Assert.IsTrue(wrapped.MoveNext());
            NUnit.Framework.Assert.AreEqual(JavaUtil.ArraysAsList(5, 6), wrapped.Current);
            NUnit.Framework.Assert.IsTrue(wrapped.MoveNext());
            NUnit.Framework.Assert.AreEqual(JavaCollectionsUtil.SingletonList(7), wrapped.Current);
            NUnit.Framework.Assert.IsFalse(wrapped.MoveNext());
        }

        [NUnit.Framework.Test]
        public virtual void WrapWithInvalidArgs() {
            Exception nullPtrException = NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => Batching.Wrap
                ((IEnumerator<Object>)null, 2).MoveNext());
            NUnit.Framework.Assert.AreEqual(typeof(NullReferenceException), nullPtrException.GetType());
            Exception illegalArgException = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => Batching.Wrap
                (JavaCollectionsUtil.EmptyIterator<Object>(), 0).MoveNext());
            NUnit.Framework.Assert.AreEqual(typeof(ArgumentException), illegalArgException.GetType());
        }
    }
}
