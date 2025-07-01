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
using System.Linq;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Pdfocr.Onnxtr.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class BatchProcessingGeneratorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InitWithInvalidArgs() {
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new BatchProcessingGenerator<Object, Object
                >(null, null));
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new BatchProcessingGenerator<Object, Object
                >(JavaCollectionsUtil.EmptyIterator<IList<Object>>(), null));
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new BatchProcessingGenerator<Object, int
                >(null, new _IBatchProcessor_53()));
        }

        private sealed class _IBatchProcessor_53 : IBatchProcessor<Object, int> {
            public _IBatchProcessor_53() {
            }

            public IList<int> ProcessBatch(IList<Object> batch) {
                return Enumerable.Repeat(1, batch.Count).ToList();
            }
        }

        [NUnit.Framework.Test]
        public virtual void ProcessorReturnsNull() {
            BatchProcessingGenerator<int, Object> generator = new BatchProcessingGenerator<int, Object>(JavaCollectionsUtil
                .SingletonList(JavaCollectionsUtil.SingletonList(1)).GetEnumerator(), new _IBatchProcessor_67());
            NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), () => generator.Next());
        }

        private sealed class _IBatchProcessor_67 : IBatchProcessor<int, Object> {
            public _IBatchProcessor_67() {
            }

            public IList<Object> ProcessBatch(IList<int> batch) {
                return null;
            }
        }

        [NUnit.Framework.Test]
        public virtual void ProcessorReturnsIncorrectSize() {
            BatchProcessingGenerator<int, int> generator = new BatchProcessingGenerator<int, int>(JavaCollectionsUtil
                .SingletonList(JavaCollectionsUtil.SingletonList(1)).GetEnumerator(), new _IBatchProcessor_84());
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => generator.Next());
        }

        private sealed class _IBatchProcessor_84 : IBatchProcessor<int, int> {
            public _IBatchProcessor_84() {
            }

            public IList<int> ProcessBatch(IList<int> batch)
            {
                return Enumerable.Repeat(batch[0], batch.Count + 1).ToList();
            }
        }

        [NUnit.Framework.Test]
        public virtual void Valid() {
            int[] processorCallCount = new int[] { 0 };
            BatchProcessingGenerator<int, String> generator = new BatchProcessingGenerator<int, String>(JavaUtil.ArraysAsList
                (JavaCollectionsUtil.SingletonList(1), JavaUtil.ArraysAsList(2, 3)).GetEnumerator(), new _IBatchProcessor_102
                (processorCallCount));
            NUnit.Framework.Assert.IsTrue(generator.MoveNext());
            NUnit.Framework.Assert.AreEqual("2", generator.Current);
            NUnit.Framework.Assert.IsTrue(generator.MoveNext());
            NUnit.Framework.Assert.AreEqual("4", generator.Current);
            NUnit.Framework.Assert.IsTrue(generator.MoveNext());
            NUnit.Framework.Assert.AreEqual("6", generator.Current);
            NUnit.Framework.Assert.IsFalse(generator.MoveNext());
            NUnit.Framework.Assert.AreEqual(2, processorCallCount[0]);
        }

        private sealed class _IBatchProcessor_102 : IBatchProcessor<int, String> {
            public _IBatchProcessor_102(int[] processorCallCount) {
                this.processorCallCount = processorCallCount;
            }

            public IList<String> ProcessBatch(IList<int> batch) {
                ++processorCallCount[0];
                return batch.Select((x) => JavaUtil.IntegerToString(x * 2)).ToList();
            }

            private readonly int[] processorCallCount;
        }
    }
}
