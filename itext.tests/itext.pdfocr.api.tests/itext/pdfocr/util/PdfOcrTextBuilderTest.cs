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
using iText.Kernel.Geom;
using iText.Pdfocr;
using iText.Test;

namespace iText.Pdfocr.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfOcrTextBuilderTest : ExtendedITextTest {
        private static readonly String DESTINATION_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/util/PdfOcrTextBuilderTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_DIRECTORY);
        }

        [NUnit.Framework.Test]
        public virtual void BuildTextTest() {
            IDictionary<int, IList<TextInfo>> textInfoMap = new Dictionary<int, IList<TextInfo>>();
            IList<TextInfo> textInfos = new List<TextInfo>();
            textInfos.Add(new TextInfo("Third", new Rectangle(200, 0, 100, 100)));
            textInfos.Add(new TextInfo("Fourth", new Rectangle(310, 0, 100, 100)));
            textInfos.Add(new TextInfo("Second", new Rectangle(100, 100, 120, 65)));
            textInfos.Add(new TextInfo("First", new Rectangle(0, 200, 100, 30)));
            textInfoMap.Put(1, textInfos);
            String actualResult = PdfOcrTextBuilder.BuildText(textInfoMap);
            String expectedResult = "First\nSecond\nThird Fourth\n";
            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [NUnit.Framework.Test]
        public virtual void PagesOrderTest() {
            IDictionary<int, IList<TextInfo>> textInfoMap = new Dictionary<int, IList<TextInfo>>();
            textInfoMap.Put(3, JavaUtil.ArraysAsList(new TextInfo("Third", new Rectangle(200, 0, 100, 100))));
            textInfoMap.Put(2, JavaUtil.ArraysAsList(new TextInfo("Second", new Rectangle(100, 100, 120, 65))));
            textInfoMap.Put(1, JavaUtil.ArraysAsList(new TextInfo("First", new Rectangle(0, 200, 100, 30))));
            String actualResult = PdfOcrTextBuilder.BuildText(textInfoMap);
            String expectedResult = "First\nSecond\nThird\n";
            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [NUnit.Framework.Test]
        public virtual void OrientationsTest() {
            IDictionary<int, IList<TextInfo>> textInfoMap = new Dictionary<int, IList<TextInfo>>();
            IList<TextInfo> textInfos = new List<TextInfo>();
            textInfos.Add(new TextInfo("Third", new Rectangle(200, 0, 100, 50), TextOrientation.HORIZONTAL_ROTATED_180
                ));
            textInfos.Add(new TextInfo("Fourth", new Rectangle(300, 180, 40, 120), TextOrientation.HORIZONTAL_ROTATED_270
                ));
            textInfos.Add(new TextInfo(" Second 1", new Rectangle(100, 140, 60, 160), TextOrientation.HORIZONTAL_ROTATED_90
                ));
            textInfos.Add(new TextInfo("Fourth 1", new Rectangle(300, 10, 40, 160), TextOrientation.HORIZONTAL_ROTATED_270
                ));
            textInfos.Add(new TextInfo("First ", new Rectangle(0, 200, 100, 30), TextOrientation.HORIZONTAL));
            textInfos.Add(new TextInfo("First 1", new Rectangle(110, 200, 140, 30), TextOrientation.HORIZONTAL));
            textInfos.Add(new TextInfo("Third 1", new Rectangle(50, 0, 140, 50), TextOrientation.HORIZONTAL_ROTATED_180
                ));
            textInfos.Add(new TextInfo("Second", new Rectangle(100, 10, 60, 120), TextOrientation.HORIZONTAL_ROTATED_90
                ));
            textInfoMap.Put(1, textInfos);
            String actualResult = PdfOcrTextBuilder.BuildText(textInfoMap);
            String expectedResult = "First First 1\nSecond Second 1\nThird Third 1\nFourth Fourth 1\n";
            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [NUnit.Framework.Test]
        public virtual void IntersectionsTest() {
            IDictionary<int, IList<TextInfo>> textInfoMap = new Dictionary<int, IList<TextInfo>>();
            IList<TextInfo> textInfos = new List<TextInfo>();
            textInfos.Add(new TextInfo("First", new Rectangle(0, 200, 100, 30), TextOrientation.HORIZONTAL));
            textInfos.Add(new TextInfo("First 1", new Rectangle(70, 200, 140, 30), TextOrientation.HORIZONTAL));
            textInfos.Add(new TextInfo("Second", new Rectangle(100, 10, 60, 120), TextOrientation.HORIZONTAL_ROTATED_90
                ));
            textInfos.Add(new TextInfo("Second 1", new Rectangle(100, 100, 60, 160), TextOrientation.HORIZONTAL_ROTATED_90
                ));
            textInfos.Add(new TextInfo("Third", new Rectangle(200, 0, 100, 50), TextOrientation.HORIZONTAL_ROTATED_180
                ));
            textInfos.Add(new TextInfo("Third 1", new Rectangle(80, 0, 140, 50), TextOrientation.HORIZONTAL_ROTATED_180
                ));
            textInfos.Add(new TextInfo("Fourth", new Rectangle(300, 180, 40, 120), TextOrientation.HORIZONTAL_ROTATED_270
                ));
            textInfos.Add(new TextInfo("Fourth 1", new Rectangle(300, 50, 40, 160), TextOrientation.HORIZONTAL_ROTATED_270
                ));
            textInfoMap.Put(1, textInfos);
            String actualResult = PdfOcrTextBuilder.BuildText(textInfoMap);
            String expectedResult = "FirstFirst 1\nSecondSecond 1\nThirdThird 1\nFourthFourth 1\n";
            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [NUnit.Framework.Test]
        public virtual void DistPerpendicularDiffTest() {
            IDictionary<int, IList<TextInfo>> textInfoMap = new Dictionary<int, IList<TextInfo>>();
            IList<TextInfo> textInfos = new List<TextInfo>();
            textInfos.Add(new TextInfo("First,", new Rectangle(0, 200, 100, 30), TextOrientation.HORIZONTAL));
            textInfos.Add(new TextInfo("First down", new Rectangle(110, 180, 140, 30), TextOrientation.HORIZONTAL));
            textInfos.Add(new TextInfo("First up,", new Rectangle(110, 220, 140, 30), TextOrientation.HORIZONTAL));
            textInfos.Add(new TextInfo("Second,", new Rectangle(100, 10, 60, 120), TextOrientation.HORIZONTAL_ROTATED_90
                ));
            textInfos.Add(new TextInfo("Second down", new Rectangle(140, 140, 60, 160), TextOrientation.HORIZONTAL_ROTATED_90
                ));
            textInfos.Add(new TextInfo("Second up,", new Rectangle(60, 140, 60, 160), TextOrientation.HORIZONTAL_ROTATED_90
                ));
            textInfos.Add(new TextInfo("Third,", new Rectangle(200, 0, 100, 50), TextOrientation.HORIZONTAL_ROTATED_180
                ));
            textInfos.Add(new TextInfo("Third down", new Rectangle(50, 30, 140, 50), TextOrientation.HORIZONTAL_ROTATED_180
                ));
            textInfos.Add(new TextInfo("Third up,", new Rectangle(50, -30, 140, 50), TextOrientation.HORIZONTAL_ROTATED_180
                ));
            textInfos.Add(new TextInfo("Fourth,", new Rectangle(300, 180, 40, 120), TextOrientation.HORIZONTAL_ROTATED_270
                ));
            textInfos.Add(new TextInfo("Fourth down", new Rectangle(270, 10, 40, 160), TextOrientation.HORIZONTAL_ROTATED_270
                ));
            textInfos.Add(new TextInfo("Fourth up,", new Rectangle(330, 10, 40, 160), TextOrientation.HORIZONTAL_ROTATED_270
                ));
            textInfoMap.Put(1, textInfos);
            String actualResult = PdfOcrTextBuilder.BuildText(textInfoMap);
            String expectedResult = "First up,\nFirst,\nFirst down\n" + "Second up,\nSecond,\nSecond down\n" + "Third up,\nThird,\nThird down\n"
                 + "Fourth up,\nFourth,\nFourth down\n";
            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [NUnit.Framework.Test]
        public virtual void IsInTheSameLineDifferentOrientationsTest() {
            NUnit.Framework.Assert.IsFalse(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", new Rectangle(0, 200, 
                100, 30), TextOrientation.HORIZONTAL), new TextInfo("Two", new Rectangle(0, 200, 100, 30), TextOrientation
                .HORIZONTAL_ROTATED_90)));
        }

        [NUnit.Framework.Test]
        public virtual void IsInTheSameLinePositiveTest() {
            // Compare y
            NUnit.Framework.Assert.IsTrue(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", new Rectangle(0, 186, 
                100, 60)), new TextInfo("Two", new Rectangle(110, 200, 100, 30))));
            // Compare x
            NUnit.Framework.Assert.IsTrue(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", new Rectangle(300, 110
                , 20, 100), TextOrientation.HORIZONTAL_ROTATED_270), new TextInfo("Two", new Rectangle(291, 0, 40, 100
                ), TextOrientation.HORIZONTAL_ROTATED_270)));
            // Compare y + h
            NUnit.Framework.Assert.IsTrue(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", new Rectangle(110, 0, 
                100, 70), TextOrientation.HORIZONTAL_ROTATED_180), new TextInfo("Two", new Rectangle(0, 30, 100, 50), 
                TextOrientation.HORIZONTAL_ROTATED_180)));
            // Compare x + w
            NUnit.Framework.Assert.IsTrue(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", new Rectangle(100, 0, 
                30, 100), TextOrientation.HORIZONTAL_ROTATED_90), new TextInfo("Two", new Rectangle(111, 110, 25, 100)
                , TextOrientation.HORIZONTAL_ROTATED_90)));
        }

        [NUnit.Framework.Test]
        public virtual void IsInTheSameLineNegativeTest() {
            // Compare y
            NUnit.Framework.Assert.IsFalse(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", new Rectangle(0, 180, 
                100, 30)), new TextInfo("Two", new Rectangle(110, 200, 100, 60))));
            // Compare x
            NUnit.Framework.Assert.IsFalse(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", new Rectangle(300, 110
                , 40, 100), TextOrientation.HORIZONTAL_ROTATED_270), new TextInfo("Two", new Rectangle(285, 0, 20, 100
                ), TextOrientation.HORIZONTAL_ROTATED_270)));
            // Compare y + h
            NUnit.Framework.Assert.IsFalse(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", new Rectangle(110, 15
                , 100, 70), TextOrientation.HORIZONTAL_ROTATED_180), new TextInfo("Two", new Rectangle(0, 0, 100, 50), 
                TextOrientation.HORIZONTAL_ROTATED_180)));
            // Compare x + w
            NUnit.Framework.Assert.IsFalse(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", new Rectangle(110, 0, 
                30, 100), TextOrientation.HORIZONTAL_ROTATED_90), new TextInfo("Two", new Rectangle(100, 110, 25, 100)
                , TextOrientation.HORIZONTAL_ROTATED_90)));
        }
    }
}
