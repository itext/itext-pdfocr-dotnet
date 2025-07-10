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
using iText.Test;

namespace iText.Pdfocr.Onnxtr.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class MathUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ArgmaxWithInvalidArgs() {
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => MathUtil.Argmax(null));
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => MathUtil.Argmax(new float[0]));
        }

        [NUnit.Framework.Test]
        public virtual void ArgmaxWithValidArgs() {
            NUnit.Framework.Assert.AreEqual(0, MathUtil.Argmax(new float[] { 1 }));
            NUnit.Framework.Assert.AreEqual(1, MathUtil.Argmax(new float[] { 1, 3, 2 }));
            NUnit.Framework.Assert.AreEqual(1, MathUtil.Argmax(new float[] { 1, 3, 3 }));
        }

        [NUnit.Framework.Test]
        public virtual void ClampWithInvalidArgs() {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => MathUtil.Clamp(2, 3, 1));
        }

        [NUnit.Framework.Test]
        public virtual void ClampWithValidArgs() {
            NUnit.Framework.Assert.AreEqual(1.1, MathUtil.Clamp(1.0, 1.1, 1.9));
            NUnit.Framework.Assert.AreEqual(1.5, MathUtil.Clamp(1.5, 1.1, 1.9));
            NUnit.Framework.Assert.AreEqual(1.9, MathUtil.Clamp(2.0, 1.1, 1.9));
        }
        
        [NUnit.Framework.Test]
        public virtual void CalculateLevenshteinDistance() {
            NUnit.Framework.Assert.AreEqual(5, MathUtil.CalculateLevenshteinDistance("kitten", "meat"));
            NUnit.Framework.Assert.AreEqual(1, MathUtil.CalculateLevenshteinDistance("kitten", "kitte"));
            NUnit.Framework.Assert.AreEqual(7, MathUtil.CalculateLevenshteinDistance("kitten", "testString"));
            NUnit.Framework.Assert.AreEqual(10, MathUtil.CalculateLevenshteinDistance("", "testString"));
            NUnit.Framework.Assert.AreEqual(6, MathUtil.CalculateLevenshteinDistance("kitten", ""));
            NUnit.Framework.Assert.AreEqual(0, MathUtil.CalculateLevenshteinDistance("", ""));
            NUnit.Framework.Assert.AreEqual(0, MathUtil.CalculateLevenshteinDistance(null, null));
            NUnit.Framework.Assert.AreEqual(10, MathUtil.CalculateLevenshteinDistance(null, "testString"));
        }
    }
}
