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
using System.Linq;
using System.Text;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Pdfocr;

namespace iText.Pdfocr.Util {
    /// <summary>Class to build text output from the provided image OCR result and write it to the TXT file.</summary>
    public sealed class PdfOcrTextBuilder {
        private const float DEFAULT_INTERSECTION_THRESHOLD = 0.55F;

        private static readonly double DEFAULT_ANGLE_THRESHOLD = MathUtil.ToRadians(10);

        private const double EPS = 1e-6;

        private PdfOcrTextBuilder() {
        }

        // Private constructor will prevent the instantiation of this class directly.
        /// <summary>
        /// Constructs string output from the provided
        /// <see cref="iText.Pdfocr.IOcrEngine.DoImageOcr(System.IO.FileInfo)"/>
        /// result.
        /// </summary>
        /// <param name="textInfos">
        /// 
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// where key is
        /// <see cref="int?"/>
        /// representing the number of the page
        /// and value is
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// elements where each
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// element contains a word or a line and its 4 coordinates (bbox)
        /// </param>
        /// <returns>string output of the OCR result</returns>
        public static String BuildText(IDictionary<int, IList<TextInfo>> textInfos) {
            StringBuilder outputText = new StringBuilder();
            iText.Pdfocr.Util.PdfOcrTextBuilder.CollectWordsIntoLines(textInfos);
            IList<int> pages = textInfos.Keys.OrderBy(i => i).ToList();
            foreach (int page in pages) {
                foreach (TextInfo chunk in textInfos.Get(page)) {
                    outputText.Append(chunk.GetText()).Append('\n');
                }
            }
            return outputText.ToString();
        }

        /// <summary>
        /// Sorts the provided
        /// <see cref="iText.Pdfocr.IOcrEngine.DoImageOcr(System.IO.FileInfo)"/>
        /// result by lines and updates line bboxes to match the largest words.
        /// </summary>
        /// <param name="textInfos">
        /// 
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// where key is
        /// <see cref="int?"/>
        /// representing the number of the page
        /// and value is
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// elements where each
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// element contains a word or a line and its 4 coordinates (bbox)
        /// </param>
        public static void GenerifyWordBBoxesByLine(IDictionary<int, IList<TextInfo>> textInfos) {
            iText.Pdfocr.Util.PdfOcrTextBuilder.SortTextInfosByLines(textInfos);
            IList<int> pages = textInfos.Keys.OrderBy(i => i).ToList();
            foreach (int page in pages) {
                IList<TextInfo> line = new List<TextInfo>();
                TextInfo prevChunk = null;
                foreach (TextInfo chunk in textInfos.Get(page)) {
                    if (prevChunk == null) {
                        line.Add(chunk);
                    }
                    else {
                        if (IsInTheSameLine(chunk, prevChunk)) {
                            line.Add(chunk);
                        }
                        else {
                            UpdateBBoxes(line);
                            line.Clear();
                            line.Add(chunk);
                        }
                    }
                    prevChunk = chunk;
                }
                UpdateBBoxes(line);
                line.Clear();
            }
        }

        /// <summary>
        /// Merges the provided
        /// <see cref="iText.Pdfocr.IOcrEngine.DoImageOcr(System.IO.FileInfo)"/>
        /// result into lines and
        /// updates line bounding boxes to match the largest words.
        /// </summary>
        /// <param name="textInfos">
        /// 
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// where key is
        /// <see cref="int?"/>
        /// representing the number of the page
        /// and value is
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// elements where each
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// element contains a word or a line and its 4 coordinates (bbox)
        /// </param>
        public static void CollectWordsIntoLines(IDictionary<int, IList<TextInfo>> textInfos) {
            iText.Pdfocr.Util.PdfOcrTextBuilder.SortTextInfosByLines(textInfos);
            IList<int> pages = textInfos.Keys.OrderBy(i => i).ToList();
            foreach (int page in pages) {
                IList<TextInfo> pageLines = new List<TextInfo>();
                IList<TextInfo> line = new List<TextInfo>();
                TextInfo prevChunk = null;
                foreach (TextInfo chunk in textInfos.Get(page)) {
                    if (prevChunk == null) {
                        line.Add(chunk);
                    }
                    else {
                        if (IsInTheSameLine(chunk, prevChunk)) {
                            line.Add(chunk);
                        }
                        else {
                            // Merge into one text chunk.
                            TextInfo newLine = MergeTextChunksIntoLine(line);
                            if (newLine != null) {
                                pageLines.Add(newLine);
                            }
                            line.Clear();
                            line.Add(chunk);
                        }
                    }
                    prevChunk = chunk;
                }
                // Merge into one text chunk.
                TextInfo newLine_1 = MergeTextChunksIntoLine(line);
                if (newLine_1 != null) {
                    pageLines.Add(newLine_1);
                }
                line.Clear();
                // Replace text chunks by lines.
                textInfos.Put(page, pageLines);
            }
        }

        /// <summary>
        /// Sorts the provided
        /// <see cref="iText.Pdfocr.IOcrEngine.DoImageOcr(System.IO.FileInfo)"/>
        /// result by lines.
        /// </summary>
        /// <param name="textInfos">
        /// 
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// where key is
        /// <see cref="int?"/>
        /// representing the number of the page
        /// and value is
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// elements where each
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// element contains a word or a line and its 4 coordinates (bbox)
        /// </param>
        public static void SortTextInfosByLines(IDictionary<int, IList<TextInfo>> textInfos) {
            foreach (KeyValuePair<int, IList<TextInfo>> entry in textInfos) {
                JavaCollectionsUtil.Sort(entry.Value, new _IComparer_155());
            }
        }

        private sealed class _IComparer_155 : IComparer<TextInfo> {
            public _IComparer_155() {
            }

            public int Compare(TextInfo first, TextInfo second) {
                // Not really needed, but just in case.
                if (first == second) {
                    return 0;
                }
                double angleDiff = iText.Pdfocr.Util.PdfOcrTextBuilder.GetAngleDiff(first, second);
                if (Math.Abs(angleDiff) > iText.Pdfocr.Util.PdfOcrTextBuilder.DEFAULT_ANGLE_THRESHOLD) {
                    double firstRoundAngle = iText.Pdfocr.Util.PdfOcrTextBuilder.RoundAngle(first.GetRotationAngle(), iText.Pdfocr.Util.PdfOcrTextBuilder
                        .DEFAULT_ANGLE_THRESHOLD);
                    double secondRoundAngle = iText.Pdfocr.Util.PdfOcrTextBuilder.RoundAngle(second.GetRotationAngle(), iText.Pdfocr.Util.PdfOcrTextBuilder
                        .DEFAULT_ANGLE_THRESHOLD);
                    return JavaUtil.DoubleCompare(firstRoundAngle, secondRoundAngle);
                }
                PdfOcrTextBuilder.BoundingBox[] boxes = PdfOcrTextBuilder.BoundingBox.GetNormalizedBBoxes(first, second);
                PdfOcrTextBuilder.BoundingBox box1 = boxes[0];
                PdfOcrTextBuilder.BoundingBox box2 = boxes[1];
                if (!iText.Pdfocr.Util.PdfOcrTextBuilder.AreIntersect(box1, box2)) {
                    double middleDistPerpendicularDiff = (box2.minY + box2.GetHeight() / 2) - (box1.minY + box1.GetHeight() / 
                        2);
                    return middleDistPerpendicularDiff > 0 ? 1 : -1;
                }
                return JavaUtil.DoubleCompare(box1.minX, box2.minX) > 0 ? 1 : -1;
            }
        }

        /// <summary>Processes all text infos to round the rotation angle to either 0, 90, 180 or 270 degrees.</summary>
        /// <remarks>
        /// Processes all text infos to round the rotation angle to either 0, 90, 180 or 270 degrees.
        /// Text bounding rectangle will be used for updated text bounding points.
        /// </remarks>
        /// <param name="result">OCR result to process</param>
        /// <returns>same result, but corrected</returns>
        public static IDictionary<int, IList<TextInfo>> CorrectRotationAngle(IDictionary<int, IList<TextInfo>> result
            ) {
            IList<int> pages = result.Keys.OrderBy(i => i).ToList();
            foreach (int page in pages) {
                IList<TextInfo> textInfos = result.Get(page);
                foreach (TextInfo textInfo in textInfos) {
                    Point[] textPoints = textInfo.GetBBoxRect().ToPointsArray();
                    double angle = RoundAngle(textInfo.GetRotationAngle(), Math.PI / 2);
                    if (Math.Abs(angle) < EPS) {
                        textInfo.SetTextPoints(new Point[] { textPoints[0], textPoints[3], textPoints[2], textPoints[1] });
                    }
                    else {
                        if (Math.Abs(Math.PI / 2 - angle) < EPS) {
                            textInfo.SetTextPoints(new Point[] { textPoints[1], textPoints[0], textPoints[3], textPoints[2] });
                        }
                        else {
                            if (Math.Abs(Math.PI - angle) < EPS) {
                                textInfo.SetTextPoints(new Point[] { textPoints[2], textPoints[1], textPoints[0], textPoints[3] });
                            }
                            else {
                                if (Math.Abs(3 * Math.PI / 2 - angle) < EPS) {
                                    textInfo.SetTextPoints(new Point[] { textPoints[3], textPoints[2], textPoints[1], textPoints[0] });
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks whether text chunks are in the same line.</summary>
        /// <remarks>
        /// Checks whether text chunks are in the same line.
        /// <para />
        /// We consider text chunks to be in the same line if they oriented in a same way and if their intersection
        /// is more than
        /// <see cref="DEFAULT_INTERSECTION_THRESHOLD"/>
        /// of at least one of the text chunks,
        /// e.g. for `one eight` intersection percentage will be 100% for `one` and less than 50% for `eight`.
        /// </remarks>
        /// <param name="currentTextInfo">
        /// current
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// </param>
        /// <param name="previousTextInfo">
        /// previous
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if both text chunks are in the same line,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        internal static bool IsInTheSameLine(TextInfo currentTextInfo, TextInfo previousTextInfo) {
            double angleDiff = GetAngleDiff(currentTextInfo, previousTextInfo);
            if (Math.Abs(angleDiff) > DEFAULT_ANGLE_THRESHOLD) {
                return false;
            }
            PdfOcrTextBuilder.BoundingBox[] boxes = PdfOcrTextBuilder.BoundingBox.GetNormalizedBBoxes(currentTextInfo, 
                previousTextInfo);
            return AreIntersect(boxes[0], boxes[1]);
        }
//\endcond

        /// <summary>
        /// Updates line bounding boxes to match the largest words, so all text infos will have the same height
        /// and bottom line (taking into account rotation angle).
        /// </summary>
        /// <remarks>
        /// Updates line bounding boxes to match the largest words, so all text infos will have the same height
        /// and bottom line (taking into account rotation angle).
        /// <para />
        /// Steps:
        /// 1) find average rotation angle for all text infos to update and use it as line rotation angle;
        /// 2) find
        /// <see cref="BoundingBox"/>
        /// for all text infos to update. It stores new coordinates after projection
        /// onto the rotated axes, so we could work with this bounding box as if text has no rotation (0 degrees);
        /// 3) find the height, top and bottom coordinates for the whole line in the rotated coordinate system;
        /// 4) translate lineBottomPoint back to initial (not rotated) coordinate system;
        /// 5) find bottom
        /// <see cref="Line"/>
        /// vector for the bottom line of the text infos to update;
        /// 6) find parallel top line by shifting bottom line to line height;
        /// 7) for each text info find its bounding points via intersection of its left and rights sides
        /// with found bottom and top lines.
        /// </remarks>
        /// <param name="line">list of text infos to update</param>
        private static void UpdateBBoxes(IList<TextInfo> line) {
            if (line.IsEmpty()) {
                return;
            }
            double avgAngle = CorrectAngle(GetAvgAngle(line));
            IList<PdfOcrTextBuilder.BoundingBox> boxes = new List<PdfOcrTextBuilder.BoundingBox>();
            foreach (TextInfo textInfo in line) {
                boxes.Add(PdfOcrTextBuilder.BoundingBox.ProjectToLine(textInfo.GetTextPoints(), avgAngle));
            }
            double lineTop = boxes.Aggregate((lhs, rhs) => JavaUtil.DoubleCompare(lhs.maxY, rhs.maxY) < 0 ? rhs : lhs)
                .maxY;
            double lineBottom = boxes.Aggregate((lhs, rhs) => JavaUtil.DoubleCompare(lhs.minY, rhs.minY) > 0 ? rhs : lhs
                ).minY;
            double lineHeight = boxes.Aggregate((lhs, rhs) => JavaUtil.DoubleCompare(lhs.GetHeight(), rhs.GetHeight())
                 < 0 ? rhs : lhs).GetHeight();
            double delta = (lineTop - lineBottom - lineHeight) / 2;
            Point lineBottomPoint = new Point(boxes[0].minX, lineBottom + delta);
            double x = lineBottomPoint.GetX();
            double y = lineBottomPoint.GetY();
            double xp = x * Math.Cos(-avgAngle) + y * Math.Sin(-avgAngle);
            double yp = -x * Math.Sin(-avgAngle) + y * Math.Cos(-avgAngle);
            double ux = Math.Cos(avgAngle);
            double uy = Math.Sin(avgAngle);
            PdfOcrTextBuilder.Line bottom = new PdfOcrTextBuilder.Line(xp, yp, ux, uy);
            PdfOcrTextBuilder.Line top = bottom.GetParallelLine(lineHeight);
            foreach (TextInfo word in line) {
                Point[] wordP = word.GetTextPoints();
                PdfOcrTextBuilder.Line left = new PdfOcrTextBuilder.Line(wordP[0], wordP[1]);
                PdfOcrTextBuilder.Line right = new PdfOcrTextBuilder.Line(wordP[3], wordP[2]);
                Point ll = left.Intersection(bottom);
                Point ul = left.Intersection(top);
                Point ur = right.Intersection(top);
                Point lr = right.Intersection(bottom);
                word.SetTextPoints(new Point[] { ll == null ? wordP[0] : ll, ul == null ? wordP[1] : ul, ur == null ? wordP
                    [2] : ur, lr == null ? wordP[3] : lr });
            }
        }

        /// <summary>Checks whether 2 text chunks are in the same line by their bounding boxes.</summary>
        /// <remarks>
        /// Checks whether 2 text chunks are in the same line by their bounding boxes. The horizontal intersection
        /// determined by the projection onto the y-axis must be more than
        /// <see cref="DEFAULT_INTERSECTION_THRESHOLD"/>
        /// for at least one of the text chunks.
        /// </remarks>
        /// <param name="box1">bounding box of the first text chunk</param>
        /// <param name="box2">bounding box of the second text chunk</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if chunks intersect horizontally,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        private static bool AreIntersect(PdfOcrTextBuilder.BoundingBox box1, PdfOcrTextBuilder.BoundingBox box2) {
            double intersection = Math.Min(box1.maxY, box2.maxY) - Math.Max(box1.minY, box2.minY);
            double firstIntersectPercentage = intersection / box1.GetHeight();
            double secondIntersectPercentage = intersection / box2.GetHeight();
            return Math.Max(firstIntersectPercentage, secondIntersectPercentage) > DEFAULT_INTERSECTION_THRESHOLD;
        }

        /// <summary>Merges list of text infos into single line.</summary>
        /// <param name="line">list of text infos to merge</param>
        /// <returns>
        /// 
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// for the whole line
        /// </returns>
        private static TextInfo MergeTextChunksIntoLine(IList<TextInfo> line) {
            if (line.IsEmpty()) {
                return null;
            }
            StringBuilder text = new StringBuilder();
            TextInfo prevChunk = null;
            foreach (TextInfo chunk in line) {
                if (prevChunk == null) {
                    text.Append(chunk.GetText());
                }
                else {
                    PdfOcrTextBuilder.BoundingBox[] boxes = PdfOcrTextBuilder.BoundingBox.GetNormalizedBBoxes(chunk, prevChunk
                        );
                    PdfOcrTextBuilder.BoundingBox box1 = boxes[0];
                    PdfOcrTextBuilder.BoundingBox box2 = boxes[1];
                    double dist = GetDistance(box1, box2);
                    double space = (box1.GetWidth() / chunk.GetText().Length + box2.GetWidth() / prevChunk.GetText().Length) /
                         2;
                    if (dist > space) {
                        for (int i = 0; i < (int)(dist / space); ++i) {
                            text.Append(' ');
                        }
                    }
                    else {
                        if (dist > 0 && !chunk.GetText().StartsWith(" ") && !prevChunk.GetText().EndsWith(" ")) {
                            // We only insert a blank space if the trailing character of the previous string wasn't a space,
                            // and the leading character of the current string isn't a space.
                            text.Append(' ');
                        }
                    }
                    text.Append(chunk.GetText());
                }
                prevChunk = chunk;
            }
            UpdateBBoxes(line);
            return MergeTextChunksIntoLine(line, text);
        }

        /// <summary>Merges list of text infos into single line with provided text.</summary>
        /// <param name="line">list of text infos to merge into line</param>
        /// <param name="text">text for the whole line</param>
        /// <returns>
        /// 
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// for the whole line with provided text
        /// </returns>
        private static TextInfo MergeTextChunksIntoLine(IList<TextInfo> line, StringBuilder text) {
            Point lineLeftBottomPoint = line[0].GetTextPoints()[0];
            Point lineLeftTopPoint = line[0].GetTextPoints()[1];
            Point lineRightTopPoint = line[line.Count - 1].GetTextPoints()[2];
            Point lineRightBottomPoint = line[line.Count - 1].GetTextPoints()[3];
            // Preserve orientation and image getHeight for line.
            TextInfo textInfo = new TextInfo(line[0]);
            textInfo.SetText(text.ToString());
            textInfo.SetTextPoints(new Point[] { lineLeftBottomPoint, lineLeftTopPoint, lineRightTopPoint, lineRightBottomPoint
                 });
            return textInfo;
        }

        /// <summary>Returns horizontal distance between provided current and previous text chunks.</summary>
        /// <param name="current">bounding box of the current text chunk</param>
        /// <param name="previous">bounding box of the previous text chunk</param>
        /// <returns>distance between 2 text chunks</returns>
        private static double GetDistance(PdfOcrTextBuilder.BoundingBox current, PdfOcrTextBuilder.BoundingBox previous
            ) {
            return current.minX - previous.maxX;
        }

        /// <summary>Returns normalized angle in radians in range from 0 to 2*pi.</summary>
        /// <param name="angle">angle in radians to normalize</param>
        /// <returns>normalized angle in range [0, 2*pi)</returns>
        private static double NormalizeAngle(double angle) {
            angle %= 2 * Math.PI;
            if (angle < 0) {
                angle += 2 * Math.PI;
            }
            return angle;
        }

        /// <summary>Rounds the angle to a certain value using specified step value.</summary>
        /// <remarks>
        /// Rounds the angle to a certain value using specified step value.
        /// <para />
        /// For example, if step is pi/2 (90 degrees), returned value will be either 0, 90, 180 or 270 degrees (in radians).
        /// And 0 will be returned for [-45, 45) degrees angles.
        /// </remarks>
        /// <param name="angle">angle in radians to round</param>
        /// <param name="step">step in radians to find values to round to</param>
        /// <returns>rounded angle</returns>
        private static double RoundAngle(double angle, double step) {
            double normalizedAngle = NormalizeAngle(angle);
            double lowBound = -step / 2;
            if (normalizedAngle >= 2 * Math.PI + lowBound) {
                return 0;
            }
            for (double upBound = step / 2; upBound < 2 * Math.PI + step / 2; upBound += step) {
                if (normalizedAngle >= lowBound && normalizedAngle < upBound) {
                    return (lowBound + upBound) / 2;
                }
                lowBound = upBound;
            }
            return angle;
        }

        /// <summary>Calculates the minimal difference between rotation angles of the provided text chunks from 0 to pi.
        ///     </summary>
        /// <param name="currentTextInfo">the first text chunk</param>
        /// <param name="previousTextInfo">the second text chunk</param>
        /// <returns>difference between rotation angles of the provided text chunks</returns>
        private static double GetAngleDiff(TextInfo currentTextInfo, TextInfo previousTextInfo) {
            // Compare rotation angles in the range of -pi to pi.
            double firstRotation = currentTextInfo.GetRotationAngle();
            double secondRotation = previousTextInfo.GetRotationAngle();
            double diff = firstRotation - secondRotation;
            // Diff can be from -2pi to 2pi.
            return Math.Min(Math.Abs(diff), Math.Min(2 * Math.PI - diff, 2 * Math.PI + diff));
        }

        /// <summary>Returns average rotation angle for all text infos from the provided list.</summary>
        /// <remarks>
        /// Returns average rotation angle for all text infos from the provided list.
        /// <para />
        /// Uses vector sum to correctly process cyclic angles, e.g. cases like -179 + 179 (avg is 180), 1 + 359 (avg 0).
        /// Note, that for mutually exclusive vectors this method will return 0 (e.g. 0 + 180 or 0 + 120 + 240).
        /// </remarks>
        /// <param name="textInfos">list of text infos to find average rotation angle for</param>
        /// <returns>average rotation angle for all text infos</returns>
        private static double GetAvgAngle(IList<TextInfo> textInfos) {
            double sumSin = 0;
            double sumCos = 0;
            foreach (TextInfo textInfo in textInfos) {
                double angle = textInfo.GetRotationAngle();
                sumSin += Math.Sin(angle);
                sumCos += Math.Cos(angle);
            }
            return (sumSin == 0 && sumCos == 0) ? 0 : Math.Atan2(sumSin, sumCos);
        }

        /// <summary>This method is needed to correct the rotation angle if it is really close to 0, 90, 180 or 270 degrees.
        ///     </summary>
        /// <param name="angle">angle to correct</param>
        /// <returns>corrected angle</returns>
        private static double CorrectAngle(double angle) {
            int absAngleDegrees = Math.Abs((int)ToDegrees(angle));
            if (absAngleDegrees <= 1) {
                return MathUtil.ToRadians(0);
            }
            if (Math.Abs(90 - absAngleDegrees) <= 1) {
                return MathUtil.ToRadians(angle > 0 ? 90 : -90);
            }
            if (Math.Abs(180 - absAngleDegrees) <= 1) {
                return MathUtil.ToRadians(180);
            }
            return angle;
        }

        /// <summary>Converts an angle measured in radians to an approximately equivalent angle measured in degrees.</summary>
        /// <remarks>
        /// Converts an angle measured in radians to an approximately equivalent angle measured in degrees.
        /// The conversion from radians to degrees is generally inexact; users should not expect cos(toRadians(90.0))
        /// to exactly equal 0.0.
        /// </remarks>
        /// <param name="radians">an angle, in radians</param>
        /// <returns>the measurement of the angle in degrees</returns>
        private static double ToDegrees(double radians) {
            return radians * 180.0 / Math.PI;
        }

        /// <summary>
        /// Class representing parametric representation of a line:
        /// point
        /// <c>(x, y)</c>
        /// and normalized unit direction vector
        /// <c>(ux, uy)</c>.
        /// </summary>
        private class Line {
            private readonly double x;

            private readonly double y;

            private readonly double ux;

            private readonly double uy;

            /// <summary>
            /// Creates new
            /// <see cref="Line"/>
            /// instance based on two lines.
            /// </summary>
            /// <param name="first">
            /// first
            /// <see cref="iText.Kernel.Geom.Point"/>
            /// on a line
            /// </param>
            /// <param name="second">
            /// second
            /// <see cref="iText.Kernel.Geom.Point"/>
            /// on a line
            /// </param>
            public Line(Point first, Point second) {
                this.x = first.GetX();
                this.y = first.GetY();
                double dx = second.GetX() - first.GetX();
                double dy = second.GetY() - first.GetY();
                double length = Math.Sqrt(dx * dx + dy * dy);
                this.ux = dx / length;
                this.uy = dy / length;
            }

            /// <summary>
            /// Creates new
            /// <see cref="Line"/>
            /// instance based on one point
            /// <c>(x, y)</c>
            /// and unit direction vector
            /// <c>(ux, uy)</c>.
            /// </summary>
            /// <param name="x">
            /// 
            /// <paramref name="x"/>
            /// coordinate of the point
            /// </param>
            /// <param name="y">
            /// 
            /// <paramref name="y"/>
            /// coordinate of the point
            /// </param>
            /// <param name="ux">
            /// 
            /// <paramref name="x"/>
            /// coordinate to specify the unit direction vector
            /// </param>
            /// <param name="uy">
            /// 
            /// <paramref name="y"/>
            /// coordinate to specify the unit direction vector
            /// </param>
            public Line(double x, double y, double ux, double uy) {
                this.x = x;
                this.y = y;
                this.ux = ux;
                this.uy = uy;
            }

            /// <summary>
            /// Finds intersection
            /// <see cref="iText.Kernel.Geom.Point"/>
            /// with provided line.
            /// </summary>
            /// <param name="other">
            /// other
            /// <see cref="Line"/>
            /// to find intersection with
            /// </param>
            /// <returns>
            /// intersection
            /// <see cref="iText.Kernel.Geom.Point"/>
            /// or
            /// <see langword="null"/>
            /// in case lines are parallel
            /// </returns>
            public virtual Point Intersection(PdfOcrTextBuilder.Line other) {
                double det = this.ux * other.uy - other.ux * this.uy;
                // ux and uy are normalized, so determinant equals to sin(a), where 'a' is an angle between lines.
                // If det is ~= 0, it means sin(a) ~= 0, what means 'a' ~= 0 or 180 degrees, so lines are either parallel
                // or collinear, and we won't be able to find an intersection point
                if (Math.Abs(det) < 1e-10) {
                    return null;
                }
                double dx = other.x - this.x;
                double dy = other.y - this.y;
                double t = (dx * other.uy - dy * other.ux) / det;
                double intersectX = this.x + t * this.ux;
                double intersectY = this.y + t * this.uy;
                return new Point(intersectX, intersectY);
            }

            /// <summary>
            /// Gets
            /// <see cref="Line"/>
            /// parallel to the current one on provided distance.
            /// </summary>
            /// <param name="distance">distance to shift the original point by (along the normal vector)</param>
            /// <returns>
            /// parallel
            /// <see cref="Line"/>
            /// on provided distance
            /// </returns>
            public virtual PdfOcrTextBuilder.Line GetParallelLine(double distance) {
                // Unit normal vector. It is perpendicular to the current line, the line will be shifted along it.
                double nx = -uy;
                double ny = ux;
                double shiftedX = x + distance * nx;
                double shiftedY = y + distance * ny;
                return new PdfOcrTextBuilder.Line(shiftedX, shiftedY, ux, uy);
            }
        }

        /// <summary>Helper class to store the bounding box of the text chunk after projection onto the new rotated axes.
        ///     </summary>
        private class BoundingBox {
//\cond DO_NOT_DOCUMENT
            internal readonly double minX;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly double maxX;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly double minY;
//\endcond

//\cond DO_NOT_DOCUMENT
            internal readonly double maxY;
//\endcond

            /// <summary>
            /// Creates new
            /// <see cref="BoundingBox"/>
            /// instance.
            /// </summary>
            /// <param name="minX">minimum value of the bounding rectangle along the x-axis</param>
            /// <param name="maxX">maximum value of the bounding rectangle along the x-axis</param>
            /// <param name="minY">minimum value of the bounding rectangle along the y-axis</param>
            /// <param name="maxY">maximum value of the bounding rectangle along the y-axis</param>
            public BoundingBox(double minX, double maxX, double minY, double maxY) {
                this.minX = minX;
                this.maxX = maxX;
                this.minY = minY;
                this.maxY = maxY;
            }

            /// <summary>Returns the width of the bounding box.</summary>
            /// <returns>the width of the bounding box</returns>
            public virtual double GetWidth() {
                return maxX - minX;
            }

            /// <summary>Returns the height of the bounding box.</summary>
            /// <returns>the height of the bounding box</returns>
            public virtual double GetHeight() {
                return maxY - minY;
            }

            /// <summary>Projects all points onto the new rotated (x, y) axes and returns the min/max values.</summary>
            /// <param name="points">
            /// 
            /// <see cref="iText.Kernel.Geom.Point"/>
            /// array to process
            /// </param>
            /// <param name="angle">the angle defining the directions of the new coordinate system</param>
            /// <returns>
            /// 
            /// <see cref="BoundingBox"/>
            /// instance containing the min/max values in the new coordinate system
            /// </returns>
            public static PdfOcrTextBuilder.BoundingBox ProjectToLine(Point[] points, double angle) {
                double cosA = Math.Cos(angle);
                double sinA = Math.Sin(angle);
                double minX = double.PositiveInfinity;
                double maxX = double.NegativeInfinity;
                double minY = double.PositiveInfinity;
                double maxY = double.NegativeInfinity;
                foreach (Point p in points) {
                    double x = p.GetX();
                    double y = p.GetY();
                    double xp = x * cosA + y * sinA;
                    double yp = -x * sinA + y * cosA;
                    minX = Math.Min(minX, xp);
                    maxX = Math.Max(maxX, xp);
                    minY = Math.Min(minY, yp);
                    maxY = Math.Max(maxY, yp);
                }
                return new PdfOcrTextBuilder.BoundingBox(minX, maxX, minY, maxY);
            }

            /// <summary>Calculates average angle for several text chunks and projects them onto the new axes rotated by that angle.
            ///     </summary>
            /// <remarks>
            /// Calculates average angle for several text chunks and projects them onto the new axes rotated by that angle.
            /// After that bounding boxes are calculated in that new coordinate system.
            /// </remarks>
            /// <param name="first">
            /// first
            /// <see cref="iText.Pdfocr.TextInfo"/>
            /// to calculate normalized bounding box for
            /// </param>
            /// <param name="second">
            /// second
            /// <see cref="iText.Pdfocr.TextInfo"/>
            /// to calculate normalized bounding box for
            /// </param>
            /// <returns>
            /// array of 2
            /// <see cref="BoundingBox"/>
            /// es in the new coordinate system for the 1st and the 2nd text chunks
            /// </returns>
            public static PdfOcrTextBuilder.BoundingBox[] GetNormalizedBBoxes(TextInfo first, TextInfo second) {
                // The average angle to construct a common coordinate system.
                double avgAngle = GetAvgAngle(JavaUtil.ArraysAsList(first, second));
                // Transform the points of each text chunk into new line coordinates (new x along the line, new y across).
                PdfOcrTextBuilder.BoundingBox box1 = PdfOcrTextBuilder.BoundingBox.ProjectToLine(first.GetTextPoints(), avgAngle
                    );
                PdfOcrTextBuilder.BoundingBox box2 = PdfOcrTextBuilder.BoundingBox.ProjectToLine(second.GetTextPoints(), avgAngle
                    );
                return new PdfOcrTextBuilder.BoundingBox[] { box1, box2 };
            }
        }
    }
}
