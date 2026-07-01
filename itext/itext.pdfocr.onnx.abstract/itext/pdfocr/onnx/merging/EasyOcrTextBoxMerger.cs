/*
Copyright (c) 2020 JaidedAI Authors.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System;
using System.Collections.Generic;
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;

namespace iText.Pdfocr.Onnx.Merging {
    /// <summary>Text box merger, based on the algorithm used in EasyOCR.</summary>
    public class EasyOcrTextBoxMerger : ITextBoxMerger {
        /// <summary>
        /// Pre-calculated
        /// <c>sqrt(2)</c>.
        /// </summary>
        private static readonly double SQRT_2 = Math.Sqrt(2);

        /// <summary>Threshold for the tangent of the box slope to consider it for merging.</summary>
        private const double SLOPE_THRESHOLD = 0.1;

        /// <summary>Threshold for the dimension ratio of the box slope to straighten it.</summary>
        private const double RATIO_THRESHOLD = 1.1;

        /// <summary>Threshold for the vertical distance between text boxes during merging.</summary>
        private const double Y_THRESHOLD = 0.5;

        /// <summary>Threshold for the height differences between text boxes during merging.</summary>
        private const double HEIGHT_THRESHOLD = 0.5;

        /// <summary>Threshold for the horizontal distance between text boxes during merging.</summary>
        private const double WIDTH_THRESHOLD = 0.5;

        /// <summary>Multiplier for calculating the added margin.</summary>
        private const double MARGIN_MUL = 0.1;

        /// <summary>
        /// Creates new
        /// <see cref="EasyOcrTextBoxMerger"/>
        /// instance.
        /// </summary>
        public EasyOcrTextBoxMerger() {
        }

        // Empty constructor in order for default one to not be removed if another one is added.
        /// <summary><inheritDoc/></summary>
        public virtual IList<iText.Kernel.Geom.Point[]> Process(IList<iText.Kernel.Geom.Point[]> detectedTextBoxes
            ) {
            // Separating aligned and sloped text boxes
            List<iText.Kernel.Geom.Point[]> alignedBoxes = new List<iText.Kernel.Geom.Point[]>();
            List<iText.Kernel.Geom.Point[]> slopedBoxes = new List<iText.Kernel.Geom.Point[]>();
            for (int i = 0; i < detectedTextBoxes.Count; ++i) {
                iText.Kernel.Geom.Point[] box = detectedTextBoxes[i];
                // If the box is square enough, we will ignore the slope
                if (CalcSlope(box) < SLOPE_THRESHOLD || CalcRatio(box) < RATIO_THRESHOLD) {
                    alignedBoxes.Add(ToBoundingBox(box));
                }
                else {
                    slopedBoxes.Add(AddMarginSloped(box));
                }
            }
            // If no aligned boxes, then there is nothing to merge
            // Early exit
            if (alignedBoxes.IsEmpty()) {
                return slopedBoxes;
            }
            // Sort by middle Y before merging
            JavaCollectionsUtil.Sort(alignedBoxes, new _IComparer_90());
            List<iText.Kernel.Geom.Point[]> finalBoxes = new List<iText.Kernel.Geom.Point[]>(slopedBoxes);
            // Grouping and merging
            List<iText.Kernel.Geom.Point[]> groupBoxes = new List<iText.Kernel.Geom.Point[]>();
            groupBoxes.Add(alignedBoxes[0]);
            double groupHeightSum = CalcHeightAligned(alignedBoxes[0]);
            double groupYSum = CalcYAligned(alignedBoxes[0]);
            for (int i = 1; i < alignedBoxes.Count; ++i) {
                iText.Kernel.Geom.Point[] box = alignedBoxes[i];
                double height = CalcHeightAligned(box);
                double y = CalcYAligned(box);
                double avgGroupY = groupYSum / groupBoxes.Count;
                double avgGroupHeight = groupHeightSum / groupBoxes.Count;
                if (Math.Abs(y - avgGroupY) < Y_THRESHOLD * avgGroupHeight) {
                    // Adding box to group, if comparable
                    groupBoxes.Add(box);
                    groupHeightSum += height;
                    groupYSum += y;
                }
                else {
                    // Otherwise process current vertical group and start a new one
                    ProcessVerticalGroup(groupBoxes, finalBoxes);
                    groupBoxes.Clear();
                    groupBoxes.Add(box);
                    groupHeightSum = height;
                    groupYSum = y;
                }
            }
            if (!groupBoxes.IsEmpty()) {
                ProcessVerticalGroup(groupBoxes, finalBoxes);
            }
            return finalBoxes;
        }

        private sealed class _IComparer_90 : IComparer<iText.Kernel.Geom.Point[]> {
            public _IComparer_90() {
            }

            public int Compare(iText.Kernel.Geom.Point[] o1, iText.Kernel.Geom.Point[] o2) {
                return JavaUtil.DoubleCompare(iText.Pdfocr.Onnx.Merging.EasyOcrTextBoxMerger.CalcYAligned(o1), iText.Pdfocr.Onnx.Merging.EasyOcrTextBoxMerger
                    .CalcYAligned(o2));
            }
        }

        /// <summary>Handles text box processing within a vertical group of text boxes.</summary>
        /// <param name="verticalGroup">vertical group of text boxes to process</param>
        /// <param name="out">output list to store merged text boxes in</param>
        private static void ProcessVerticalGroup(IList<iText.Kernel.Geom.Point[]> verticalGroup, IList<iText.Kernel.Geom.Point
            []> @out) {
            System.Diagnostics.Debug.Assert(!verticalGroup.IsEmpty());
            // If only one box in verticalGroup, just pass it padded to out
            if (verticalGroup.Count == 1) {
                @out.Add(AddMarginAligned(verticalGroup[0]));
                return;
            }
            JavaCollectionsUtil.Sort(verticalGroup, new _IComparer_144());
            List<iText.Kernel.Geom.Point[]> groupBoxes = new List<iText.Kernel.Geom.Point[]>();
            groupBoxes.Add(verticalGroup[0]);
            double groupHeightSum = CalcHeightAligned(verticalGroup[0]);
            double groupXMax = CalcXMaxAligned(verticalGroup[0]);
            for (int i = 1; i < verticalGroup.Count; ++i) {
                iText.Kernel.Geom.Point[] box = verticalGroup[i];
                double width = CalcWidthAligned(box);
                double height = CalcHeightAligned(box);
                double xMin = CalcXMinAligned(box);
                double avgGroupHeight = groupHeightSum / groupBoxes.Count;
                if ((Math.Abs(height - avgGroupHeight) < HEIGHT_THRESHOLD * avgGroupHeight) && (xMin - groupXMax < WIDTH_THRESHOLD
                     * width)) {
                    // Adding box to group, if comparable
                    groupBoxes.Add(box);
                    groupHeightSum += height;
                }
                else {
                    // Otherwise process current horizontal group and start a new one
                    @out.Add(AddMarginAligned(ToBoundingBox(groupBoxes)));
                    groupBoxes.Clear();
                    groupBoxes.Add(box);
                    groupHeightSum = height;
                }
                groupXMax = CalcXMaxAligned(box);
            }
            if (!groupBoxes.IsEmpty()) {
                @out.Add(AddMarginAligned(ToBoundingBox(groupBoxes)));
            }
        }

        private sealed class _IComparer_144 : IComparer<iText.Kernel.Geom.Point[]> {
            public _IComparer_144() {
            }

            public int Compare(iText.Kernel.Geom.Point[] o1, iText.Kernel.Geom.Point[] o2) {
                return JavaUtil.DoubleCompare(iText.Pdfocr.Onnx.Merging.EasyOcrTextBoxMerger.CalcXMinAligned(o1), iText.Pdfocr.Onnx.Merging.EasyOcrTextBoxMerger
                    .CalcXMinAligned(o2));
            }
        }

        /// <summary>Builds a bounding box for a list of text boxes.</summary>
        /// <param name="boxes">list of text boxes to build a bounding box for</param>
        /// <returns>built bounding box</returns>
        private static iText.Kernel.Geom.Point[] ToBoundingBox(IList<iText.Kernel.Geom.Point[]> boxes) {
            System.Diagnostics.Debug.Assert(!boxes.IsEmpty());
            double minX = boxes[0][0].GetX();
            double maxX = minX;
            double minY = boxes[0][0].GetY();
            double maxY = minY;
            for (int boxIdx = 0; boxIdx < boxes.Count; ++boxIdx) {
                iText.Kernel.Geom.Point[] points = boxes[boxIdx];
                for (int pointIdx = 0; pointIdx < points.Length; ++pointIdx) {
                    iText.Kernel.Geom.Point p = points[pointIdx];
                    double x = p.GetX();
                    if (x < minX) {
                        minX = x;
                    }
                    else {
                        if (x > maxX) {
                            maxX = x;
                        }
                    }
                    double y = p.GetY();
                    if (y < minY) {
                        minY = y;
                    }
                    else {
                        if (y > maxY) {
                            maxY = y;
                        }
                    }
                }
            }
            return new iText.Kernel.Geom.Point[] { new iText.Kernel.Geom.Point(minX, maxY), new iText.Kernel.Geom.Point
                (minX, minY), new iText.Kernel.Geom.Point(maxX, minY), new iText.Kernel.Geom.Point(maxX, maxY) };
        }

        /// <summary>Builds a bounding box for an array of points.</summary>
        /// <param name="points">array of points to build a bounding box for</param>
        /// <returns>built bounding box</returns>
        private static iText.Kernel.Geom.Point[] ToBoundingBox(iText.Kernel.Geom.Point[] points) {
            return ToBoundingBox(JavaCollectionsUtil.SingletonList(points));
        }

        /// <summary>Creates a new box, which adds a margin to an existing sloped one.</summary>
        /// <param name="box">sloped text box to add margin to</param>
        /// <returns>box with the added margin</returns>
        private static iText.Kernel.Geom.Point[] AddMarginSloped(iText.Kernel.Geom.Point[] box) {
            /*
            * The algorithm just extends the rotated rectangle by extending its
            * diagonals on both sides. And then it calculates new points from
            * that.
            */
            iText.Kernel.Geom.Point p0 = box[0];
            // Bottom-left  -X +Y
            iText.Kernel.Geom.Point p1 = box[1];
            // Top-left     -X -Y
            iText.Kernel.Geom.Point p2 = box[2];
            // Top-right    +X -Y
            iText.Kernel.Geom.Point p3 = box[3];
            // Bottom-right +X +Y
            double height = p1.Distance(p0);
            double width = p1.Distance(p2);
            double diagonalMargin = SQRT_2 * MARGIN_MUL * Math.Min(height, width);
            double theta02 = Math.Abs(Math.Atan((p0.GetY() - p2.GetY()) / (p0.GetX() - p2.GetX())));
            double theta13 = Math.Abs(Math.Atan((p1.GetY() - p3.GetY()) / (p1.GetX() - p3.GetX())));
            double dx02 = Math.Cos(theta02) * diagonalMargin;
            double dy02 = Math.Sin(theta02) * diagonalMargin;
            double dx13 = Math.Cos(theta13) * diagonalMargin;
            double dy13 = Math.Sin(theta13) * diagonalMargin;
            return new iText.Kernel.Geom.Point[] { new iText.Kernel.Geom.Point(p0.GetX() - dx02, p0.GetY() + dy02), new 
                iText.Kernel.Geom.Point(p1.GetX() - dx13, p1.GetY() - dy13), new iText.Kernel.Geom.Point(p2.GetX() + dx02
                , p2.GetY() - dy02), new iText.Kernel.Geom.Point(p3.GetX() + dx13, p3.GetY() + dy13) };
        }

        /// <summary>Creates a new box, which adds a margin to an existing aligned one.</summary>
        /// <param name="box">aligned text box to add margin to</param>
        /// <returns>box with the added margin</returns>
        private static iText.Kernel.Geom.Point[] AddMarginAligned(iText.Kernel.Geom.Point[] box) {
            iText.Kernel.Geom.Point p0 = box[0];
            // Bottom-left  -X +Y
            iText.Kernel.Geom.Point p1 = box[1];
            // Top-left     -X -Y
            iText.Kernel.Geom.Point p2 = box[2];
            // Top-right    +X -Y
            iText.Kernel.Geom.Point p3 = box[3];
            // Bottom-right +X +Y
            double height = CalcHeightAligned(box);
            double width = CalcWidthAligned(box);
            double margin = MARGIN_MUL * Math.Min(height, width);
            return new iText.Kernel.Geom.Point[] { new iText.Kernel.Geom.Point(p0.GetX() - margin, p0.GetY() + margin)
                , new iText.Kernel.Geom.Point(p1.GetX() - margin, p1.GetY() - margin), new iText.Kernel.Geom.Point(p2.
                GetX() + margin, p2.GetY() - margin), new iText.Kernel.Geom.Point(p3.GetX() + margin, p3.GetY() + margin
                ) };
        }

        /// <summary>Calculates slope for an arbitrary text box.</summary>
        /// <param name="box">text box to calculate slope for</param>
        /// <returns>calculated slope</returns>
        private static double CalcSlope(iText.Kernel.Geom.Point[] box) {
            return Math.Abs((box[0].GetY() - box[3].GetY()) / (box[0].GetX() - box[3].GetX()));
        }

        /// <summary>
        /// Calculates ratio of the text box from biggest dimension to the smallest
        /// one.
        /// </summary>
        /// <param name="box">text box to calculate ratio for</param>
        /// <returns>calculated ratio</returns>
        private static double CalcRatio(iText.Kernel.Geom.Point[] box) {
            double height = box[1].Distance(box[0]);
            double width = box[1].Distance(box[2]);
            if (height > width) {
                return height / width;
            }
            return width / height;
        }

        /// <summary>Calculates width for an aligned text box.</summary>
        /// <param name="box">aligned text box</param>
        /// <returns>width</returns>
        private static double CalcWidthAligned(iText.Kernel.Geom.Point[] box) {
            return box[2].GetX() - box[1].GetX();
        }

        /// <summary>Calculates height for an aligned text box.</summary>
        /// <param name="box">aligned text box</param>
        /// <returns>height</returns>
        private static double CalcHeightAligned(iText.Kernel.Geom.Point[] box) {
            return box[0].GetY() - box[1].GetY();
        }

        /// <summary>Calculates central y for an aligned text box.</summary>
        /// <param name="box">aligned text box</param>
        /// <returns>central y</returns>
        private static double CalcYAligned(iText.Kernel.Geom.Point[] box) {
            return (box[0].GetY() + box[1].GetY()) / 2;
        }

        /// <summary>Calculates minimum x for an aligned text box.</summary>
        /// <param name="box">aligned text box</param>
        /// <returns>minimum x</returns>
        private static double CalcXMinAligned(iText.Kernel.Geom.Point[] box) {
            return box[0].GetX();
        }

        /// <summary>Calculates maximum x for an aligned text box.</summary>
        /// <param name="box">aligned text box</param>
        /// <returns>maximum x</returns>
        private static double CalcXMaxAligned(iText.Kernel.Geom.Point[] box) {
            return box[2].GetX();
        }
    }
}
