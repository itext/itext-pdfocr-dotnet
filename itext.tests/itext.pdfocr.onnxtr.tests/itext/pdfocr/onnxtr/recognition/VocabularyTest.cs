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

namespace iText.Pdfocr.Onnxtr.Recognition {
    [NUnit.Framework.Category("UnitTest")]
    public class VocabularyTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InitWithInvalidArgs() {
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new Vocabulary(null));
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => 
                        //                            U+1FAE0
                        new Vocabulary("ABC" + "\uD83E\uDEE0" + "DEF"));
        }

        [NUnit.Framework.Test]
        public virtual void Valid() {
            Vocabulary vocabulary = new Vocabulary("ABC");
            NUnit.Framework.Assert.AreEqual("ABC", vocabulary.GetLookUpString());
            NUnit.Framework.Assert.AreEqual("ABC", vocabulary.ToString());
            NUnit.Framework.Assert.AreEqual(3, vocabulary.Size());
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => vocabulary.Map(-1));
            NUnit.Framework.Assert.AreEqual('A', vocabulary.Map(0));
            NUnit.Framework.Assert.AreEqual('B', vocabulary.Map(1));
            NUnit.Framework.Assert.AreEqual('C', vocabulary.Map(2));
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => vocabulary.Map(3));
            NUnit.Framework.Assert.AreEqual(new Vocabulary("ABC").GetHashCode(), vocabulary.GetHashCode());
            NUnit.Framework.Assert.AreEqual(new Vocabulary("ABC"), vocabulary);
            NUnit.Framework.Assert.AreNotEqual(new Vocabulary("ABCD"), vocabulary);
        }
    }
}
