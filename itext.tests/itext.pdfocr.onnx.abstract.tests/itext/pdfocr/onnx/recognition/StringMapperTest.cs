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
using iText.Test;

namespace iText.Pdfocr.Onnx.Recognition {
    [NUnit.Framework.Category("UnitTest")]
    public class StringMapperTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InitWithInvalidArgs() {
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new StringMapper((String)null));
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new StringMapper((String[])null));
        }

        [NUnit.Framework.Test]
        public virtual void MapperFromString() {
            //                                             U+1FAE0
            StringMapper mapper = new StringMapper("A\uD83E\uDEE0B");
            NUnit.Framework.Assert.AreEqual(3, mapper.Size());
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => mapper.Map(-1));
            NUnit.Framework.Assert.AreEqual("A", mapper.Map(0));
            NUnit.Framework.Assert.AreEqual("\uD83E\uDEE0", mapper.Map(1));
            NUnit.Framework.Assert.AreEqual("B", mapper.Map(2));
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => mapper.Map(3));
            NUnit.Framework.Assert.AreEqual(new StringMapper("A\uD83E\uDEE0B"), mapper);
            NUnit.Framework.Assert.AreNotEqual(new StringMapper("AB"), mapper);
        }

        [NUnit.Framework.Test]
        public virtual void MapperFromArray() {
            //                                                 DE flag: U+1F1E9 U+1F1EA
            String[] backingArray = new String[] { "AB", "\uD83C\uDDE9\uD83C\uDDEA", "CD" };
            StringMapper mapper = new StringMapper(backingArray);
            NUnit.Framework.Assert.AreEqual(3, mapper.Size());
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => mapper.Map(-1));
            NUnit.Framework.Assert.AreEqual("AB", mapper.Map(0));
            NUnit.Framework.Assert.AreEqual("\uD83C\uDDE9\uD83C\uDDEA", mapper.Map(1));
            NUnit.Framework.Assert.AreEqual("CD", mapper.Map(2));
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => mapper.Map(3));
            NUnit.Framework.Assert.AreEqual(new StringMapper(backingArray).GetHashCode(), mapper.GetHashCode());
            NUnit.Framework.Assert.AreEqual(new StringMapper(backingArray), mapper);
            NUnit.Framework.Assert.AreNotEqual(new StringMapper("ABCD"), mapper);
        }
    }
}
