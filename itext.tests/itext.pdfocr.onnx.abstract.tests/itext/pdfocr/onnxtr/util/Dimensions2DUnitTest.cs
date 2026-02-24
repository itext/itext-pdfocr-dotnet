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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Pdfocr.Onnxtr.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class Dimensions2DUnitTest : ExtendedITextTest {
        public static IEnumerable<Object[]> EqualsFalse() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { new Dimensions2D(100, 20), null }, new Object
                [] { new Dimensions2D(100, 20), "different class" }, new Object[] { new Dimensions2D(100, 700), new Dimensions2D
                (100, 800) }, new Object[] { new Dimensions2D(100, 800), new Dimensions2D(200, 800) }, new Object[] { 
                new Dimensions2D(100, 700), new Dimensions2D(200, 800) } });
        }

        public static IEnumerable<Object[]> EqualsTrue() {
            Dimensions2D dimensions2D = new Dimensions2D(100, 200);
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { dimensions2D, new Dimensions2D(100, 200) }, new 
                Object[] { dimensions2D, dimensions2D } });
        }

        [NUnit.Framework.TestCaseSource("EqualsFalse")]
        public virtual void EqualsNegativeTest(Dimensions2D first, Object second) {
            NUnit.Framework.Assert.AreNotEqual(first, second);
        }

        [NUnit.Framework.TestCaseSource("EqualsTrue")]
        public virtual void EqualsPositiveTest(Dimensions2D first, Dimensions2D second) {
            NUnit.Framework.Assert.AreEqual(first, second);
        }
    }
}
