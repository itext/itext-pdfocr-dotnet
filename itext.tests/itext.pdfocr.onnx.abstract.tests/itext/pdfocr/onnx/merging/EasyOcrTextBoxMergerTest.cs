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
using System.Collections.Generic;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Pdfocr.Onnx.Merging {
    [NUnit.Framework.Category("UnitTest")]
    public class EasyOcrTextBoxMergerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void Basic() {
            IList<iText.Kernel.Geom.Point[]> detectedTextBoxes = JavaUtil.ArraysAsList(new iText.Kernel.Geom.Point[] { 
                        // Three aligned rectangles close on the first line
                        new iText.Kernel.Geom.Point(0.15, 0.10), new iText.Kernel.Geom.Point(0.15, 0.05), new iText.Kernel.Geom.Point
                (0.25, 0.05), new iText.Kernel.Geom.Point(0.25, 0.10) }, new iText.Kernel.Geom.Point[] { new iText.Kernel.Geom.Point
                (0.05, 0.10), new iText.Kernel.Geom.Point(0.05, 0.05), new iText.Kernel.Geom.Point(0.10, 0.05), new iText.Kernel.Geom.Point
                (0.10, 0.10) }, new iText.Kernel.Geom.Point[] { new iText.Kernel.Geom.Point(0.30, 0.10), new iText.Kernel.Geom.Point
                (0.30, 0.05), new iText.Kernel.Geom.Point(0.45, 0.05), new iText.Kernel.Geom.Point(0.45, 0.10) }, new 
                iText.Kernel.Geom.Point[] { 
                        // Sloped rectangle on the first line
                        new iText.Kernel.Geom.Point(0.525, 0.150), new iText.Kernel.Geom.Point(0.500, 0.100), new iText.Kernel.Geom.Point
                (0.575, 0.050), new iText.Kernel.Geom.Point(0.600, 0.100) }, new iText.Kernel.Geom.Point[] { 
                        // Two separated aligned rectangles on the second line
                        new iText.Kernel.Geom.Point(0.05, 0.25), new iText.Kernel.Geom.Point(0.05, 0.20), new iText.Kernel.Geom.Point
                (0.20, 0.20), new iText.Kernel.Geom.Point(0.20, 0.25) }, new iText.Kernel.Geom.Point[] { new iText.Kernel.Geom.Point
                (0.35, 0.25), new iText.Kernel.Geom.Point(0.35, 0.20), new iText.Kernel.Geom.Point(0.55, 0.20), new iText.Kernel.Geom.Point
                (0.55, 0.25) }, new iText.Kernel.Geom.Point[] { 
                        // Single aligned vertical rectangle on the third line
                        new iText.Kernel.Geom.Point(0.05, 0.45), new iText.Kernel.Geom.Point(0.05, 0.35), new iText.Kernel.Geom.Point
                (0.10, 0.35), new iText.Kernel.Geom.Point(0.10, 0.45) });
            IList<iText.Kernel.Geom.Point[]> expectedResult = JavaUtil.ArraysAsList(new iText.Kernel.Geom.Point[] { 
                        // Sloped rectangle on the first line
                        new iText.Kernel.Geom.Point(0.5215, 0.1571), new iText.Kernel.Geom.Point(0.4921, 0.1000), new iText.Kernel.Geom.Point
                (0.5785, 0.0429), new iText.Kernel.Geom.Point(0.6079, 0.1000) }, new iText.Kernel.Geom.Point[] { 
                        // Three merged aligned rectangles on the first line
                        new iText.Kernel.Geom.Point(0.0450, 0.1050), new iText.Kernel.Geom.Point(0.0450, 0.0450), new iText.Kernel.Geom.Point
                (0.4550, 0.0450), new iText.Kernel.Geom.Point(0.4550, 0.1050) }, new iText.Kernel.Geom.Point[] { 
                        // Two separated aligned rectangles on the second line
                        new iText.Kernel.Geom.Point(0.0450, 0.2550), new iText.Kernel.Geom.Point(0.0450, 0.1950), new iText.Kernel.Geom.Point
                (0.2050, 0.1950), new iText.Kernel.Geom.Point(0.2050, 0.2550) }, new iText.Kernel.Geom.Point[] { new iText.Kernel.Geom.Point
                (0.3450, 0.2550), new iText.Kernel.Geom.Point(0.3450, 0.1950), new iText.Kernel.Geom.Point(0.5550, 0.1950
                ), new iText.Kernel.Geom.Point(0.5550, 0.2550) }, new iText.Kernel.Geom.Point[] { 
                        // Single aligned vertical rectangle on the third line
                        new iText.Kernel.Geom.Point(0.0450, 0.4550), new iText.Kernel.Geom.Point(0.0450, 0.3450), new iText.Kernel.Geom.Point
                (0.1050, 0.3450), new iText.Kernel.Geom.Point(0.1050, 0.4550) });
            IList<iText.Kernel.Geom.Point[]> actualResult = new EasyOcrTextBoxMerger().Process(detectedTextBoxes);
            AssertEquals(expectedResult, actualResult);
        }

        [NUnit.Framework.Test]
        public virtual void NoAlignedBoxes() {
            IList<iText.Kernel.Geom.Point[]> detectedTextBoxes = JavaUtil.ArraysAsList(new iText.Kernel.Geom.Point[] { 
                        // Two sloped rectangles
                        new iText.Kernel.Geom.Point(0.525, 0.150), new iText.Kernel.Geom.Point(0.500, 0.100), new iText.Kernel.Geom.Point
                (0.575, 0.050), new iText.Kernel.Geom.Point(0.600, 0.100) }, new iText.Kernel.Geom.Point[] { new iText.Kernel.Geom.Point
                (0.100, 0.150), new iText.Kernel.Geom.Point(0.050, 0.050), new iText.Kernel.Geom.Point(0.100, 0.025), 
                new iText.Kernel.Geom.Point(0.150, 0.125) });
            IList<iText.Kernel.Geom.Point[]> expectedResult = JavaUtil.ArraysAsList(new iText.Kernel.Geom.Point[] { 
                        // Two sloped rectangles
                        new iText.Kernel.Geom.Point(0.5215, 0.1571), new iText.Kernel.Geom.Point(0.4921, 0.1000), new iText.Kernel.Geom.Point
                (0.5785, 0.0429), new iText.Kernel.Geom.Point(0.6079, 0.1000) }, new iText.Kernel.Geom.Point[] { new iText.Kernel.Geom.Point
                (0.1000, 0.1579), new iText.Kernel.Geom.Point(0.0437, 0.0453), new iText.Kernel.Geom.Point(0.1000, 0.0171
                ), new iText.Kernel.Geom.Point(0.1563, 0.1297) });
            IList<iText.Kernel.Geom.Point[]> actualResult = new EasyOcrTextBoxMerger().Process(detectedTextBoxes);
            AssertEquals(expectedResult, actualResult);
        }

        private static void AssertEquals(IList<iText.Kernel.Geom.Point[]> expected, IList<iText.Kernel.Geom.Point[]
            > actual) {
            NUnit.Framework.Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; ++i) {
                iText.Kernel.Geom.Point[] expectedBox = expected[i];
                iText.Kernel.Geom.Point[] actualBox = actual[i];
                NUnit.Framework.Assert.AreEqual(expectedBox.Length, actualBox.Length);
                for (int j = 0; j < expectedBox.Length; ++j) {
                    iText.Kernel.Geom.Point expectedPoint = expectedBox[j];
                    iText.Kernel.Geom.Point actualPoint = actualBox[j];
                    NUnit.Framework.Assert.AreEqual(expectedPoint.GetX(), actualPoint.GetX(), 1E-4);
                    NUnit.Framework.Assert.AreEqual(expectedPoint.GetY(), actualPoint.GetY(), 1E-4);
                }
            }
        }
    }
}
