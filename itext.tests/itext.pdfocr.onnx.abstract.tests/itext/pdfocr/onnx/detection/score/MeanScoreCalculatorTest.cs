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

namespace iText.Pdfocr.Onnx.Detection.Score {
    [NUnit.Framework.Category("UnitTest")]
    public class MeanScoreCalculatorTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void NoSamples() {
            NUnit.Framework.Assert.AreEqual(0.0F, new MeanScoreCalculator().Calculate());
        }

        [NUnit.Framework.Test]
        public virtual void SomeSamples() {
            MeanScoreCalculator calculator = new MeanScoreCalculator();
            calculator.Observe(2.5F);
            calculator.Observe(-4.5F);
            calculator.Observe(6.5F);
            calculator.Observe(8.0F);
            NUnit.Framework.Assert.AreEqual(3.125F, calculator.Calculate());
        }
    }
}
