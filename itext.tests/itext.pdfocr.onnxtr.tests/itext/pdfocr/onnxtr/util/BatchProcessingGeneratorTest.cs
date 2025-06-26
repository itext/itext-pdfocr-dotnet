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
using iText.Commons.Utils;
using iText.Test;

namespace iText.Pdfocr.Onnxtr.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class BatchProcessingGeneratorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InitWithInvalidArgs() {
            //NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new BatchProcessingGenerator<Object, Object
            //    >(null, null));
            //NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new BatchProcessingGenerator<Object, Object
            //    >(JavaCollectionsUtil.EmptyIterator(), null));
            //NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new BatchProcessingGenerator<Object, int
            //    >(null, (batch) => JavaCollectionsUtil.NCopies(batch.Count, 1)));
        }

        [NUnit.Framework.Test]
        public virtual void ProcessorReturnsNull() {
            //BatchProcessingGenerator<int, Object> generator = new BatchProcessingGenerator<int, Object>(JavaCollectionsUtil
            //    .SingletonList(JavaCollectionsUtil.SingletonList(1)).GetEnumerator(), (batch) => null);
            //NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), generator);
        }

        [NUnit.Framework.Test]
        public virtual void ProcessorReturnsIncorrectSize() {
            //BatchProcessingGenerator<int, Object> generator = new BatchProcessingGenerator<int, Object>(JavaCollectionsUtil
            //    .SingletonList(JavaCollectionsUtil.SingletonList(1)).GetEnumerator(), (batch) => JavaCollectionsUtil.NCopies
            //    (batch.Count + 1, batch[0]));
            //NUnit.Framework.Assert.Catch(typeof(InvalidOperationException), generator);
        }

        [NUnit.Framework.Test]
        public virtual void Valid() {
            int[] processorCallCount = new int[] { 0 };
            //BatchProcessingGenerator<int, String> generator = new BatchProcessingGenerator<int, String>(JavaUtil.ArraysAsList
            //    (JavaCollectionsUtil.SingletonList(1), JavaUtil.ArraysAsList(2, 3)).GetEnumerator(), (batch) => {
            //    ++processorCallCount[0];
            //    return batch.Select((x) => JavaUtil.IntegerToString(x * 2)).ToList();
            //}
            //);
            //NUnit.Framework.Assert.AreEqual("2", generator.Next());
            //NUnit.Framework.Assert.AreEqual("4", generator.Next());
            //NUnit.Framework.Assert.AreEqual("6", generator.Next());
            //NUnit.Framework.Assert.Catch(typeof(NullReferenceException), generator);
            //NUnit.Framework.Assert.AreEqual(2, processorCallCount[0]);
        }
    }
}
