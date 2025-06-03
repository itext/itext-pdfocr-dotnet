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

using OpenCvSharp;

namespace iText.Pdfocr.Onnxtr.Util {
    /// <summary>Static class with OpenCV utility functions.</summary>
    public sealed class OpenCvUtil {
        private OpenCvUtil() {
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
                (rectSize.Width, rectSize.Height) = (rectSize.Height, rectSize.Width);
                rect.Size = rectSize;
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
    }
}
