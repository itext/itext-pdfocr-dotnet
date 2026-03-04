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
using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;
using iText.Pdfocr.Onnx.Exceptions;

namespace iText.Pdfocr.Onnx.Util {
    /// <summary>Static class with OpenCV utility functions.</summary>
    public sealed class OpenCvUtil {
        private OpenCvUtil() {
        }

        /// <summary>
        /// Takes a two-dimensional MD-array and returns a binarized version of it
        /// as an OpenCV Mat.
        /// </summary>
        /// <param name="hwMdArray">MD-array to binarize</param>
        /// <param name="threshold">Threshold for a value to be 0xFF instead of 0x00</param>
        /// <returns>binarized version of the MD-array as an OpenCV Mat</returns>
        public static Mat BinarizeMdArray(FloatBufferMdArray hwMdArray, float threshold) {
            if (hwMdArray.GetDimensionCount() != 2) {
                throw new ArgumentException(PdfOcrOnnxExceptionMessageConstant.HW_ARRAY_SHOULD_BE_TWO_DIMENSIONAL);
            }
            int height = hwMdArray.GetDimension(0);
            int width = hwMdArray.GetDimension(1);
            Mat binaryImage = new Mat(height, width, MatType.CV_8U);
            Mat.Indexer<byte> binaryImageIndexer = binaryImage.GetGenericIndexer<byte>();
            for (int y = 0; y < height; y++) {
                FloatBufferMdArray valuesRow = hwMdArray.GetSubArray(y);
                for (int x = 0; x < width; ++x) {
                    float value = valuesRow.GetScalar(x);
                    binaryImageIndexer[y, x] = value >= threshold ? (byte)0xFF : (byte)0;
                }
            }
            return binaryImage;
        }

        /// <summary>OpenCV minAreaRect, but returns the normalized rectangle immediately.</summary>
        /// <remarks>
        /// OpenCV minAreaRect, but returns the normalized rectangle immediately.
        /// Equivalent to
        /// <c>normalizeRotatedRect(opencv_imgproc.minAreaRect(x))</c>.
        /// </remarks>
        /// <param name="points">vector of 2D points</param>
        /// <returns>normalized min area rect</returns>
        public static RotatedRect NormalizedMinAreaRect(Mat points) {
            return NormalizeRotatedRect(Cv2.MinAreaRect(points));
        }

        /// <summary>Normalizes RotatedRect, so that its angle is in the [-45; 45) range.</summary>
        /// <remarks>
        /// Normalizes RotatedRect, so that its angle is in the [-45; 45) range.
        /// <para />
        /// We want our boxes to have the point order, so that it matches input image orientation.
        /// Otherwise, the orientation detection model will get a different box, which is already
        /// pre-rotated in some way. Here we will alter the rectangle, so that points would output the
        /// expected order.
        /// <para />
        /// This will make box have points in the following order, relative to the page: BL, TL, TR, BR.
        /// Bottom as in bottom of the image, not the lowest Y coordinate.
        /// </remarks>
        /// <param name="rect">RotatedRect to normalize</param>
        /// <returns>normalized RotatedRect</returns>
        public static RotatedRect NormalizeRotatedRect(RotatedRect rect) {
            float angle = rect.Angle;
            float clampedAngle = MathUtil.EuclideanModulo(angle, 360);
            /*
            * For 90 and 270 degrees need to swap sizes.
            */
            if ((45F <= clampedAngle && clampedAngle < 135F) || (225F <= clampedAngle && clampedAngle < 315F)) {
                Size2f rectSize = rect.Size;
                try {
                    float temp = rectSize.Width;
                    rectSize.Width = rectSize.Height;
                    rectSize.Height = temp;
                    rect.Size = rectSize;
                }
                finally {
                }
                if (clampedAngle < 135F) {
                    rect.Angle = clampedAngle - 90F;
                }
                else {
                    rect.Angle = clampedAngle - 270F;
                }
            }
            else {
                if (135F <= clampedAngle && clampedAngle < 225F) {
                    rect.Angle = clampedAngle - 180F;
                }
                else {
                    if (315F <= clampedAngle) {
                        rect.Angle = clampedAngle - 360F;
                    }
                    else {
                        System.Diagnostics.Debug.Assert(0F <= clampedAngle && clampedAngle < 45F);
                        rect.Angle = clampedAngle;
                    }
                }
            }
            return rect;
        }

        /// <summary>
        /// Equivalent to calling OpenCV
        /// <c>minAreaRect</c>
        /// , followed by
        /// <c>boxPoints</c>
        /// , but with resource handling taken care of.
        /// </summary>
        /// <param name="points">points to get the rectangle for</param>
        /// <returns>min area rectangle points</returns>
        public static Mat MinAreaRectBoxPoints(Mat points) {
            RotatedRect rect = Cv2.MinAreaRect(points);
            Mat rectPoints = new Mat();
            try {
                Cv2.BoxPoints(rect, rectPoints);
                return rectPoints;
            }
            catch (Exception e) {
                throw;
            }
        }

        /// <summary>
        /// Creates an OpenCV polygon, based on the results of a
        /// <c>minAreaRectBoxPoints(points)</c>
        /// call, but with resource handling
        /// taken care of.
        /// </summary>
        /// <param name="points">points to get the rectangle for</param>
        /// <returns>min area rectangle polygon</returns>
        public static Mat MinAreaRectBoxPoly(Mat points) {
            using (Mat rectPoints = iText.Pdfocr.Onnx.Util.OpenCvUtil.MinAreaRectBoxPoints(points)) {
                using (Mat rectPointsInt = new Mat()) {
                    // +0.5, so that values are rounded, not floored
                    rectPoints.ConvertTo(rectPointsInt, MatType.CV_32S, 1, 0.5);
                    return rectPointsInt.Reshape(2, new int[] { 4, 1 });
                }
            }
        }

        /// <summary>Fill the polygon on the bitmap at a specific offset.</summary>
        /// <param name="bitmap">bitmap to fill the polygon on</param>
        /// <param name="poly">polygon to fill</param>
        /// <param name="color">color to fill the polygon with</param>
        /// <param name="xOffset">x offset for the polygon</param>
        /// <param name="yOffset">y offset for the polygon</param>
        public static void FillPolyAtOffset(Mat bitmap, Mat poly, Scalar color, int xOffset, int yOffset) {
            Point[][] offsetPolys = MakeOffsetPolys(poly, xOffset, yOffset);
            Cv2.FillPoly(bitmap, offsetPolys, color);
        }

        private static Point[][] MakeOffsetPolys(Mat poly, int xOffset, int yOffset) {
            Point[] points = GetPointsFromMat(poly);
    
            for (int i = 0; i < points.Length; i++) {
                points[i].X += xOffset;
                points[i].Y += yOffset;
            }
    
            return new[] { points };
        }

        private static Point[] GetPointsFromMat(Mat poly) {
            poly.GetArray<Point>(out Point[] array);
            return array;
        }
    }
}
