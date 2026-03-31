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
using iText.Kernel.Geom;
using iText.Pdfocr;
using iText.Test;

namespace iText.Pdfocr.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfOcrTextBuilderTest : ExtendedITextTest {
        private static readonly String DESTINATION_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/util/PdfOcrTextBuilderTest/";

        private const double EPS = 1e-4;

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
        public virtual void GenerifyLineTest() {
            IDictionary<int, IList<TextInfo>> textInfoMap = new Dictionary<int, IList<TextInfo>>();
            IList<TextInfo> textInfos = new List<TextInfo>();
            textInfos.Add(new TextInfo("Third", new Rectangle(200, 0, 100, 25)));
            textInfos.Add(new TextInfo("Fourth", new Rectangle(310, 0, 100, 50)));
            textInfos.Add(new TextInfo("Second", new Rectangle(100, 0, 120, 35)));
            textInfos.Add(new TextInfo("First", new Rectangle(0, 0, 100, 30)));
            textInfoMap.Put(1, textInfos);
            PdfOcrTextBuilder.GenerifyWordBBoxesByLine(textInfoMap);
            NUnit.Framework.Assert.IsTrue(new Rectangle(0, 0, 100, 50).EqualsWithEpsilon(textInfos[0].GetBBoxRect()));
            NUnit.Framework.Assert.IsTrue(new Rectangle(100, 0, 120, 50).EqualsWithEpsilon(textInfos[1].GetBBoxRect())
                );
            NUnit.Framework.Assert.IsTrue(new Rectangle(200, 0, 100, 50).EqualsWithEpsilon(textInfos[2].GetBBoxRect())
                );
            NUnit.Framework.Assert.IsTrue(new Rectangle(310, 0, 100, 50).EqualsWithEpsilon(textInfos[3].GetBBoxRect())
                );
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
            Point a = new Point(0, 0);
            Point b = new Point(0, 200);
            Point c = new Point(200, 200);
            Point d = new Point(200, 0);
            Point e = new Point(300, 0);
            Point f = new Point(300, 200);
            Point g = new Point(500, 200);
            Point h = new Point(500, 0);
            Point i = new Point(300, -400);
            Point j = new Point(300, -200);
            Point k = new Point(500, -200);
            Point l = new Point(500, -400);
            textInfos.Add(new TextInfo("Third", new Point[] { g, h, e, f }));
            textInfos.Add(new TextInfo("First 2", new Point[] { i, j, k, l }));
            textInfos.Add(new TextInfo("Fourth", new Point[] { f, g, h, e }));
            textInfos.Add(new TextInfo(" Second 1", new Point[] { h, e, f, g }));
            textInfos.Add(new TextInfo("Fourth 1", new Point[] { j, k, l, i }));
            textInfos.Add(new TextInfo("First ", new Point[] { a, b, c, d }));
            textInfos.Add(new TextInfo("First 1", new Point[] { e, f, g, h }));
            textInfos.Add(new TextInfo("Third 1", new Point[] { c, d, a, b }));
            textInfos.Add(new TextInfo("Second", new Point[] { l, i, j, k }));
            textInfoMap.Put(1, textInfos);
            String actualResult = PdfOcrTextBuilder.BuildText(textInfoMap);
            String expectedResult = "First    First 1\n" + "First 2\n" + "Second        Second 1\n" + "Third  Third 1\n"
                 + "Fourth      Fourth 1\n";
            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [NUnit.Framework.Test]
        public virtual void GenerifyLineOrientationsTest() {
            IDictionary<int, IList<TextInfo>> textInfoMap = new Dictionary<int, IList<TextInfo>>();
            IList<TextInfo> textInfos = new List<TextInfo>();
            Point a = new Point(0, 400);
            Point b = new Point(0, 600);
            Point c = new Point(200, 600);
            Point d = new Point(200, 400);
            Point e = new Point(300, 400);
            Point f = new Point(300, 580);
            Point g = new Point(480, 580);
            Point h = new Point(480, 400);
            Point i = new Point(300, 0);
            Point j = new Point(300, 200);
            Point k = new Point(500, 200);
            Point l = new Point(500, 0);
            textInfos.Add(new TextInfo("Third", new Point[] { g, h, e, f }));
            textInfos.Add(new TextInfo("First 2", new Point[] { i, j, k, l }));
            textInfos.Add(new TextInfo("Fourth", new Point[] { f, g, h, e }));
            textInfos.Add(new TextInfo("Second 1", new Point[] { h, e, f, g }));
            textInfos.Add(new TextInfo("Fourth 1", new Point[] { j, k, l, i }));
            textInfos.Add(new TextInfo("First", new Point[] { a, b, c, d }));
            textInfos.Add(new TextInfo("First 1", new Point[] { e, f, g, h }));
            textInfos.Add(new TextInfo("Third 1", new Point[] { c, d, a, b }));
            textInfos.Add(new TextInfo("Second", new Point[] { l, i, j, k }));
            textInfoMap.Put(1, textInfos);
            PdfOcrTextBuilder.GenerifyWordBBoxesByLine(textInfoMap);
            foreach (TextInfo textInfo in textInfos) {
                Point point1 = textInfo.GetTextPoints()[1];
                Point point2 = textInfo.GetTextPoints()[0];
                double dx = point1.GetX() - point2.GetX();
                double dy = point1.GetY() - point2.GetY();
                NUnit.Framework.Assert.AreEqual(200, Math.Sqrt(dx * dx + dy * dy), EPS);
            }
        }

        [NUnit.Framework.Test]
        public virtual void IntersectionsTest() {
            IDictionary<int, IList<TextInfo>> textInfoMap = new Dictionary<int, IList<TextInfo>>();
            IList<TextInfo> textInfos = new List<TextInfo>();
            Point a = new Point(0, 200);
            Point b = new Point(0, 400);
            Point c = new Point(200, 400);
            Point d = new Point(200, 200);
            Point e = new Point(190, 200);
            Point f = new Point(190, 400);
            Point g = new Point(500, 400);
            Point h = new Point(500, 200);
            Point i = new Point(300, 0);
            Point j = new Point(300, 210);
            Point k = new Point(500, 210);
            Point l = new Point(500, 0);
            textInfos.Add(new TextInfo("Third", new Point[] { g, h, e, f }));
            textInfos.Add(new TextInfo("Fourth", new Point[] { f, g, h, e }));
            textInfos.Add(new TextInfo("Second 1", new Point[] { h, e, f, g }));
            textInfos.Add(new TextInfo("Fourth 1", new Point[] { j, k, l, i }));
            textInfos.Add(new TextInfo("First", new Point[] { a, b, c, d }));
            textInfos.Add(new TextInfo("First 1", new Point[] { e, f, g, h }));
            textInfos.Add(new TextInfo("Third 1", new Point[] { c, d, a, b }));
            textInfos.Add(new TextInfo("Second", new Point[] { l, i, j, k }));
            textInfoMap.Put(1, textInfos);
            String actualResult = PdfOcrTextBuilder.BuildText(textInfoMap);
            String expectedResult = "FirstFirst 1\nSecondSecond 1\nThirdThird 1\nFourthFourth 1\n";
            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [NUnit.Framework.Test]
        public virtual void DistPerpendicularDiffTest() {
            IDictionary<int, IList<TextInfo>> textInfoMap = new Dictionary<int, IList<TextInfo>>();
            IList<TextInfo> textInfos = new List<TextInfo>();
            Point a = new Point(0, 0);
            Point b = new Point(0, 200);
            Point c = new Point(200, 200);
            Point d = new Point(200, 0);
            Point[] bbox0 = new Point[] { a, b, c, d };
            Point[] bbox90 = new Point[] { d, a, b, c };
            Point[] bbox180 = new Point[] { c, d, a, b };
            Point[] bbox270 = new Point[] { b, c, d, a };
            textInfos.Add(new TextInfo("First,", bbox0));
            textInfos.Add(new TextInfo("First down", ShiftPoints(bbox0, 0, -200)));
            textInfos.Add(new TextInfo("First up,", ShiftPoints(bbox0, 0, 200)));
            textInfos.Add(new TextInfo("Second,", bbox90));
            textInfos.Add(new TextInfo("Second down", ShiftPoints(bbox90, 200, 0)));
            textInfos.Add(new TextInfo("Second up,", ShiftPoints(bbox90, -200, 0)));
            textInfos.Add(new TextInfo("Third,", bbox180));
            textInfos.Add(new TextInfo("Third down", ShiftPoints(bbox180, 0, 200)));
            textInfos.Add(new TextInfo("Third up,", ShiftPoints(bbox180, 0, -200)));
            textInfos.Add(new TextInfo("Fourth,", bbox270));
            textInfos.Add(new TextInfo("Fourth down", ShiftPoints(bbox270, -200, 0)));
            textInfos.Add(new TextInfo("Fourth up,", ShiftPoints(bbox270, 200, 0)));
            textInfoMap.Put(1, textInfos);
            String actualResult = PdfOcrTextBuilder.BuildText(textInfoMap);
            String expectedResult = "First up,\nFirst,\nFirst down\n" + "Second up,\nSecond,\nSecond down\n" + "Third up,\nThird,\nThird down\n"
                 + "Fourth up,\nFourth,\nFourth down\n";
            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [NUnit.Framework.Test]
        public virtual void IsInTheSameLineDifferentOrientationsTest() {
            Point a = new Point(0, 200);
            Point b = new Point(0, 0);
            Point c = new Point(200, 0);
            Point d = new Point(200, 200);
            Point[] bbox0 = new Point[] { a, b, c, d };
            Point[] bbox90 = new Point[] { d, a, b, c };
            NUnit.Framework.Assert.IsFalse(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", bbox0), new TextInfo(
                "Two", bbox90)));
        }

        [NUnit.Framework.Test]
        public virtual void IsInTheSameLinePositiveTest() {
            Point a = new Point(0, 200);
            Point b = new Point(0, 0);
            Point c = new Point(200, 0);
            Point d = new Point(200, 200);
            Point[] bbox0 = new Point[] { a, b, c, d };
            Point[] bbox90 = new Point[] { d, a, b, c };
            Point[] bbox180 = new Point[] { c, d, a, b };
            Point[] bbox270 = new Point[] { b, c, d, a };
            NUnit.Framework.Assert.IsTrue(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", bbox0), new TextInfo("Two"
                , ShiftPoints(bbox0, 250, 0))));
            NUnit.Framework.Assert.IsTrue(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", bbox90), new TextInfo(
                "Two", ShiftPoints(bbox90, 0, 250))));
            NUnit.Framework.Assert.IsTrue(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", bbox180), new TextInfo
                ("Two", ShiftPoints(bbox180, -250, 0))));
            NUnit.Framework.Assert.IsTrue(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", bbox270), new TextInfo
                ("Two", ShiftPoints(bbox270, 0, -250))));
        }

        [NUnit.Framework.Test]
        public virtual void IsInTheSameLineNegativeTest() {
            Point a = new Point(0, 200);
            Point b = new Point(0, 0);
            Point c = new Point(200, 0);
            Point d = new Point(200, 200);
            Point[] bbox0 = new Point[] { a, b, c, d };
            Point[] bbox90 = new Point[] { d, a, b, c };
            Point[] bbox180 = new Point[] { c, d, a, b };
            Point[] bbox270 = new Point[] { b, c, d, a };
            NUnit.Framework.Assert.IsFalse(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", bbox0), new TextInfo(
                "Two", ShiftPoints(bbox0, 0, 250))));
            NUnit.Framework.Assert.IsFalse(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", bbox90), new TextInfo
                ("Two", ShiftPoints(bbox90, 250, 0))));
            NUnit.Framework.Assert.IsFalse(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", bbox180), new TextInfo
                ("Two", ShiftPoints(bbox180, 0, -250))));
            NUnit.Framework.Assert.IsFalse(PdfOcrTextBuilder.IsInTheSameLine(new TextInfo("One", bbox270), new TextInfo
                ("Two", ShiftPoints(bbox270, -250, 0))));
        }

        [NUnit.Framework.Test]
        public virtual void CollectWordsIntoLinesTest() {
            IDictionary<int, IList<TextInfo>> textInfoMap = new Dictionary<int, IList<TextInfo>>();
            IList<TextInfo> textInfos = new List<TextInfo>();
            textInfos.Add(new TextInfo("Third", GetPointsFromRect(new Rectangle(240, 100, 100, 25))));
            textInfos.Add(new TextInfo("Fourth", GetPointsFromRect(new Rectangle(350, 100, 100, 50))));
            textInfos.Add(new TextInfo("Second", GetPointsFromRect(new Rectangle(110, 100, 120, 35))));
            textInfos.Add(new TextInfo("First", GetPointsFromRect(new Rectangle(0, 100, 100, 30))));
            textInfos.Add(new TextInfo("New line", GetPointsFromRect(new Rectangle(0, 0, 100, 30))));
            textInfoMap.Put(1, textInfos);
            PdfOcrTextBuilder.CollectWordsIntoLines(textInfoMap);
            IList<TextInfo> mergedTextInfos = textInfoMap.Get(1);
            NUnit.Framework.Assert.AreEqual(2, mergedTextInfos.Count);
            NUnit.Framework.Assert.AreEqual("First Second Third Fourth", mergedTextInfos[0].GetText());
            NUnit.Framework.Assert.AreEqual(50, mergedTextInfos[0].GetBBoxRect().GetHeight());
            NUnit.Framework.Assert.AreEqual("New line", mergedTextInfos[1].GetText());
        }

        [NUnit.Framework.Test]
        public virtual void CorrectRotationAngleTest() {
            Point a = new Point(0, 200);
            Point b = new Point(0, 400);
            Point c = new Point(200, 400);
            Point d = new Point(200, 200);
            IDictionary<int, IList<TextInfo>> textInfoMap = new Dictionary<int, IList<TextInfo>>();
            IList<TextInfo> textInfos = new List<TextInfo>();
            textInfos.Add(new TextInfo("First", new Point[] { ShiftPoint(a, 5, 5), b, c, d }));
            textInfos.Add(new TextInfo("Second", new Point[] { ShiftPoint(d, -5, 5), a, b, c }));
            textInfos.Add(new TextInfo("Third", new Point[] { ShiftPoint(c, -5, -5), d, a, b }));
            textInfos.Add(new TextInfo("Fourth", new Point[] { ShiftPoint(b, 5, -5), c, d, a }));
            textInfoMap.Put(1, textInfos);
            IList<Point[]> expected = new List<Point[]>();
            expected.Add(new Point[] { a, b, c, d });
            expected.Add(new Point[] { d, a, b, c });
            expected.Add(new Point[] { c, d, a, b });
            expected.Add(new Point[] { b, c, d, a });
            PdfOcrTextBuilder.CorrectRotationAngle(textInfoMap);
            for (int i = 0; i < expected.Count; i++) {
                Point[] expectedPoints = expected[i];
                Point[] actualPoints = textInfos[i].GetTextPoints();
                for (int j = 0; j < expectedPoints.Length; j++) {
                    NUnit.Framework.Assert.AreEqual(expectedPoints[j], actualPoints[j]);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void EmptyResultTest() {
            IDictionary<int, IList<TextInfo>> textInfoMap = new Dictionary<int, IList<TextInfo>>();
            textInfoMap.Put(1, new List<TextInfo>());
            PdfOcrTextBuilder.GenerifyWordBBoxesByLine(textInfoMap);
            NUnit.Framework.Assert.IsTrue(textInfoMap.Get(1).IsEmpty());
        }

        private Point[] GetPointsFromRect(Rectangle rectangle) {
            Point[] rectPoints = rectangle.ToPointsArray();
            return new Point[] { rectPoints[0], rectPoints[3], rectPoints[2], rectPoints[1] };
        }

        private static Point[] ShiftPoints(Point[] points, int dx, int dy) {
            Point[] shiftedPoints = new Point[points.Length];
            for (int i = 0; i < points.Length; i++) {
                shiftedPoints[i] = ShiftPoint(points[i], dx, dy);
            }
            return shiftedPoints;
        }

        private static Point ShiftPoint(Point point, int dx, int dy) {
            return new Point(point.GetX() + dx, point.GetY() + dy);
        }
    }
}
