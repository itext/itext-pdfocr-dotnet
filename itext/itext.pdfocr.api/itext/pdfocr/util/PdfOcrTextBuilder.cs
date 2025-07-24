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
using System.Linq;
using System.Text;
using iText.Commons.Utils;
using iText.Commons.Utils.Collections;
using iText.Pdfocr;

namespace iText.Pdfocr.Util {
    /// <summary>Class to build text output from the provided image OCR result and write it to the TXT file.</summary>
    public sealed class PdfOcrTextBuilder {
        private const float DEFAULT_INTERSECTION_THRESHOLD = 0.7f;

        private const float DEFAULT_GAP_THRESHOLD = 0.1f;

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
            iText.Pdfocr.Util.PdfOcrTextBuilder.SortTextInfosByLines(textInfos);
            foreach (int page in textInfos.Keys.OrderBy(i => i).ToList()) {
                StringBuilder sb = new StringBuilder();
                TextInfo lastChunk = null;
                foreach (TextInfo chunk in textInfos.Get(page)) {
                    if (lastChunk == null) {
                        sb.Append(chunk.GetText());
                    }
                    else {
                        if (IsInTheSameLine(chunk, lastChunk)) {
                            // We only insert a blank space if the trailing character of the previous string wasn't a space,
                            // and the leading character of the current string isn't a space.
                            if (IsChunkAtWordBoundary(chunk, lastChunk) && !chunk.GetText().StartsWith(" ") && !lastChunk.GetText().EndsWith
                                (" ")) {
                                sb.Append(' ');
                            }
                            sb.Append(chunk.GetText());
                        }
                        else {
                            sb.Append('\n').Append(chunk.GetText());
                        }
                    }
                    lastChunk = chunk;
                }
                outputText.Append(sb).Append('\n');
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
            foreach (int page in textInfos.Keys.OrderBy(i => i).ToList()) {
                IList<TextInfo> line = new List<TextInfo>();
                TextInfo lastChunk = null;
                foreach (TextInfo chunk in textInfos.Get(page)) {
                    if (lastChunk == null) {
                        line.Add(chunk);
                    }
                    else {
                        if (IsInTheSameLine(chunk, lastChunk)) {
                            line.Add(chunk);
                        }
                        else {
                            UpdateBBoxes(line);
                            line.Clear();
                            line.Add(chunk);
                        }
                    }
                    lastChunk = chunk;
                }
                UpdateBBoxes(line);
                line.Clear();
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
                JavaCollectionsUtil.Sort(entry.Value, new _IComparer_125());
            }
        }

        private sealed class _IComparer_125 : IComparer<TextInfo> {
            public _IComparer_125() {
            }

            public int Compare(TextInfo first, TextInfo second) {
                // Not really needed, but just in case.
                if (first == second) {
                    return 0;
                }
                int result = JavaUtil.IntegerCompare(iText.Pdfocr.Util.PdfOcrTextBuilder.GetOrientation(first.GetOrientation
                    ()), iText.Pdfocr.Util.PdfOcrTextBuilder.GetOrientation(second.GetOrientation()));
                if (result != 0) {
                    return result;
                }
                if (!iText.Pdfocr.Util.PdfOcrTextBuilder.AreIntersect(first, second)) {
                    float middleDistPerpendicularDiff = iText.Pdfocr.Util.PdfOcrTextBuilder.GetDistPerpendicularBottom(second)
                         + iText.Pdfocr.Util.PdfOcrTextBuilder.GetHeight(second) / 2 - (iText.Pdfocr.Util.PdfOcrTextBuilder.GetDistPerpendicularBottom
                        (first) + iText.Pdfocr.Util.PdfOcrTextBuilder.GetHeight(first) / 2);
                    return middleDistPerpendicularDiff > 0 ? 1 : -1;
                }
                return JavaUtil.FloatCompare(iText.Pdfocr.Util.PdfOcrTextBuilder.GetDistParallelStart(first), iText.Pdfocr.Util.PdfOcrTextBuilder
                    .GetDistParallelStart(second)) > 0 ? 1 : -1;
            }
        }

//\cond DO_NOT_DOCUMENT
        /// <summary>Checks whether text chunks are in the same line.</summary>
        /// <remarks>
        /// Checks whether text chunks are in the same line.
        /// <para />
        /// We consider text chunks to be in the same line if they oriented in a same way and
        /// if their intersection is more than 70% of at least one of the text chunks, e.g. for `one eight`
        /// intersection percentage will be 100% for `one` and less than 50% for `eight`.
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
            if (currentTextInfo.GetOrientation() != previousTextInfo.GetOrientation()) {
                return false;
            }
            return AreIntersect(currentTextInfo, previousTextInfo);
        }
//\endcond

        private static void UpdateBBoxes(IList<TextInfo> line) {
            if (line.Count == 0) {
                return;
            }
            float lineTop;
            float lineHeight;
            float lineBottom;
            float delta;
            switch (line[0].GetOrientation()) {
                case TextOrientation.HORIZONTAL:
                case TextOrientation.HORIZONTAL_ROTATED_180: {
                    //Using orElseThrow instead of get for correct autoporting, orElseThrow won't be called since reduce()
                    // will always return something if there is at least one element in stream
                    lineTop = line.Aggregate((lhs, rhs) => JavaUtil.FloatCompare(lhs.GetBboxRect().GetTop(), rhs.GetBboxRect()
                        .GetTop()) < 0 ? rhs : lhs).GetBboxRect().GetTop();
                    lineHeight = line.Aggregate((lhs, rhs) => JavaUtil.FloatCompare(lhs.GetBboxRect().GetHeight(), rhs.GetBboxRect
                        ().GetHeight()) < 0 ? rhs : lhs).GetBboxRect().GetHeight();
                    lineBottom = line.Aggregate((lhs, rhs) => JavaUtil.FloatCompare(lhs.GetBboxRect().GetBottom(), rhs.GetBboxRect
                        ().GetBottom()) > 0 ? rhs : lhs).GetBboxRect().GetBottom();
                    delta = (lineTop - lineBottom - lineHeight) / 2;
                    foreach (TextInfo word in line) {
                        word.GetBboxRect().SetY(lineBottom + delta).SetHeight(lineHeight);
                    }
                    break;
                }

                case TextOrientation.HORIZONTAL_ROTATED_90:
                case TextOrientation.HORIZONTAL_ROTATED_270: {
                    //Using orElseThrow instead of get for correct autoporting, orElseThrow won't be called since reduce()
                    // will always return something if there is at least one element in stream
                    lineTop = line.Aggregate((lhs, rhs) => JavaUtil.FloatCompare(lhs.GetBboxRect().GetRight(), rhs.GetBboxRect
                        ().GetRight()) < 0 ? rhs : lhs).GetBboxRect().GetRight();
                    lineHeight = line.Aggregate((lhs, rhs) => JavaUtil.FloatCompare(lhs.GetBboxRect().GetWidth(), rhs.GetBboxRect
                        ().GetWidth()) < 0 ? rhs : lhs).GetBboxRect().GetWidth();
                    lineBottom = line.Aggregate((lhs, rhs) => JavaUtil.FloatCompare(lhs.GetBboxRect().GetLeft(), rhs.GetBboxRect
                        ().GetLeft()) > 0 ? rhs : lhs).GetBboxRect().GetLeft();
                    delta = (lineTop - lineBottom - lineHeight) / 2;
                    foreach (TextInfo word in line) {
                        word.GetBboxRect().SetX(lineBottom).SetWidth(lineHeight);
                    }
                    break;
                }

                default: {
                    throw new NotSupportedException();
                }
            }
        }

        private static bool AreIntersect(TextInfo first, TextInfo second) {
            float intersection = Math.Min(GetDistPerpendicularTop(first), GetDistPerpendicularTop(second)) - Math.Max(
                GetDistPerpendicularBottom(first), GetDistPerpendicularBottom(second));
            float firstIntersectPercentage = intersection / GetHeight(first);
            float secondIntersectPercentage = intersection / GetHeight(second);
            return Math.Max(firstIntersectPercentage, secondIntersectPercentage) > DEFAULT_INTERSECTION_THRESHOLD;
        }

        private static bool IsChunkAtWordBoundary(TextInfo currentTextInfo, TextInfo previousTextInfo) {
            float dist = GetDistParallelStart(currentTextInfo) - GetDistParallelEnd(previousTextInfo);
            if (dist < 0) {
                dist = GetDistParallelStart(previousTextInfo) - GetDistParallelEnd(currentTextInfo);
                // The situation when the chunks intersect. We don't need to add space in this case.
                if (dist < 0) {
                    return false;
                }
            }
            // We consider that space should be added in case the difference is
            // more than 10% of the minimal of the two average widths per char.
            return dist > DEFAULT_GAP_THRESHOLD * Math.Min(GetWidth(currentTextInfo) / currentTextInfo.GetText().Length
                , GetWidth(previousTextInfo) / previousTextInfo.GetText().Length);
        }

        private static int GetOrientation(TextOrientation orientation) {
            switch (orientation) {
                case TextOrientation.HORIZONTAL_ROTATED_90: {
                    return 90;
                }

                case TextOrientation.HORIZONTAL_ROTATED_180: {
                    return 180;
                }

                case TextOrientation.HORIZONTAL_ROTATED_270: {
                    return 270;
                }

                case TextOrientation.HORIZONTAL:
                default: {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Distance of the start of the chunk parallel to the orientation unit vector (i.e. the left (X) position
        /// in the not rotated coordinate system) with a minus sign for 180 and 270 degrees, since we need the coordinates
        /// of the text to the left to be less than those of the text to the right.
        /// </summary>
        /// <param name="textInfo">
        /// 
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// to get distance parallel start
        /// </param>
        /// <returns>distance parallel start</returns>
        private static float GetDistParallelStart(TextInfo textInfo) {
            switch (textInfo.GetOrientation()) {
                case TextOrientation.HORIZONTAL_ROTATED_270: {
                    return -textInfo.GetBboxRect().GetTop();
                }

                case TextOrientation.HORIZONTAL_ROTATED_180: {
                    return -textInfo.GetBboxRect().GetRight();
                }

                case TextOrientation.HORIZONTAL_ROTATED_90: {
                    return textInfo.GetBboxRect().GetBottom();
                }

                case TextOrientation.HORIZONTAL:
                default: {
                    return textInfo.GetBboxRect().GetLeft();
                }
            }
        }

        /// <summary>
        /// Distance of the end of the chunk parallel to the orientation unit vector
        /// (i.e. the right (X + width) position in the not rotated coordinate system)
        /// with a minus sign for 180 and 270 degrees, since we need the coordinates
        /// of the text to the left to be less than those of the text to the right.
        /// </summary>
        /// <param name="textInfo">
        /// 
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// to get distance parallel end
        /// </param>
        /// <returns>distance parallel end</returns>
        private static float GetDistParallelEnd(TextInfo textInfo) {
            switch (textInfo.GetOrientation()) {
                case TextOrientation.HORIZONTAL_ROTATED_270: {
                    return -textInfo.GetBboxRect().GetBottom();
                }

                case TextOrientation.HORIZONTAL_ROTATED_180: {
                    return -textInfo.GetBboxRect().GetLeft();
                }

                case TextOrientation.HORIZONTAL_ROTATED_90: {
                    return textInfo.GetBboxRect().GetTop();
                }

                case TextOrientation.HORIZONTAL:
                default: {
                    return textInfo.GetBboxRect().GetRight();
                }
            }
        }

        /// <summary>
        /// Gets perpendicular distance to the orientation unit vector
        /// (i.e. the bottom (Y) position in the not rotated coordinate system)
        /// with a minus sign for 90 and 180 degrees, since we need the coordinates
        /// of the text to the bottom to be less than those of the text to the top.
        /// </summary>
        /// <param name="textInfo">
        /// 
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// to get distance perpendicular
        /// </param>
        /// <returns>distance perpendicular</returns>
        private static float GetDistPerpendicularBottom(TextInfo textInfo) {
            switch (textInfo.GetOrientation()) {
                case TextOrientation.HORIZONTAL_ROTATED_270: {
                    return textInfo.GetBboxRect().GetLeft();
                }

                case TextOrientation.HORIZONTAL_ROTATED_180: {
                    return -textInfo.GetBboxRect().GetTop();
                }

                case TextOrientation.HORIZONTAL_ROTATED_90: {
                    return -textInfo.GetBboxRect().GetRight();
                }

                case TextOrientation.HORIZONTAL:
                default: {
                    return textInfo.GetBboxRect().GetBottom();
                }
            }
        }

        /// <summary>
        /// Gets perpendicular distance to the orientation unit vector from the top point
        /// (i.e. the top (Y + height) position in the not rotated coordinate system)
        /// with a minus sign for 90 and 180 degrees, since we need the coordinates
        /// of the text to the bottom to be less than those of the text to the top.
        /// </summary>
        /// <param name="textInfo">
        /// 
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// to get distance perpendicular
        /// </param>
        /// <returns>distance perpendicular</returns>
        private static float GetDistPerpendicularTop(TextInfo textInfo) {
            switch (textInfo.GetOrientation()) {
                case TextOrientation.HORIZONTAL_ROTATED_270: {
                    return textInfo.GetBboxRect().GetRight();
                }

                case TextOrientation.HORIZONTAL_ROTATED_180: {
                    return -textInfo.GetBboxRect().GetBottom();
                }

                case TextOrientation.HORIZONTAL_ROTATED_90: {
                    return -textInfo.GetBboxRect().GetLeft();
                }

                case TextOrientation.HORIZONTAL:
                default: {
                    return textInfo.GetBboxRect().GetTop();
                }
            }
        }

        private static float GetWidth(TextInfo textInfo) {
            switch (textInfo.GetOrientation()) {
                case TextOrientation.HORIZONTAL_ROTATED_90:
                case TextOrientation.HORIZONTAL_ROTATED_270: {
                    return textInfo.GetBboxRect().GetHeight();
                }

                case TextOrientation.HORIZONTAL_ROTATED_180:
                case TextOrientation.HORIZONTAL:
                default: {
                    return textInfo.GetBboxRect().GetWidth();
                }
            }
        }

        private static float GetHeight(TextInfo textInfo) {
            switch (textInfo.GetOrientation()) {
                case TextOrientation.HORIZONTAL_ROTATED_90:
                case TextOrientation.HORIZONTAL_ROTATED_270: {
                    return textInfo.GetBboxRect().GetWidth();
                }

                case TextOrientation.HORIZONTAL_ROTATED_180:
                case TextOrientation.HORIZONTAL:
                default: {
                    return textInfo.GetBboxRect().GetHeight();
                }
            }
        }
    }
}
