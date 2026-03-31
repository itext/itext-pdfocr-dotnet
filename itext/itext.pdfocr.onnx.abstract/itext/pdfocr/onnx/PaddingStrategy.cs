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
using IronSoftware.Drawing;

namespace iText.Pdfocr.Onnx {
    /// <summary>Enumeration of implemented padding strategies for padding images.</summary>
    /// <remarks>
    /// Enumeration of implemented padding strategies for padding images. These are
    /// used, when we need to adapt the image to fit an input of a ML model.
    /// </remarks>
    public sealed class PaddingStrategy {
        /// <summary>Image will be put into the top-left corner.</summary>
        /// <remarks>
        /// Image will be put into the top-left corner. Remaining pixels are filled
        /// with
        /// <c>#000000</c>.
        /// </remarks>
        public static readonly PaddingStrategy BOTTOM_RIGHT_BLACK = new PaddingStrategy();

        /// <summary>Image will be put into the middle.</summary>
        /// <remarks>
        /// Image will be put into the middle. Remaining pixels are filled with
        /// <c>#000000</c>.
        /// </remarks>
        public static readonly PaddingStrategy SYMMETRIC_BLACK = new PaddingStrategy();

        /// <summary>Image will be put into the top-left corner.</summary>
        /// <remarks>
        /// Image will be put into the top-left corner. Remaining pixels are filled
        /// with
        /// <c>#808080</c>.
        /// </remarks>
        public static readonly PaddingStrategy BOTTOM_RIGHT_GRAY = new PaddingStrategy();

        /// <summary>Image will be put into the middle.</summary>
        /// <remarks>
        /// Image will be put into the middle. Remaining pixels are filled with
        /// <c>#808080</c>.
        /// </remarks>
        public static readonly PaddingStrategy SYMMETRIC_GRAY = new PaddingStrategy();

        /// <summary>Image will be put into the top-left corner.</summary>
        /// <remarks>
        /// Image will be put into the top-left corner. Remaining pixels are filled
        /// with
        /// <c>#FFFFFF</c>.
        /// </remarks>
        public static readonly PaddingStrategy BOTTOM_RIGHT_WHITE = new PaddingStrategy();

        /// <summary>Image will be put into the middle.</summary>
        /// <remarks>
        /// Image will be put into the middle. Remaining pixels are filled with
        /// <c>#FFFFFF</c>.
        /// </remarks>
        public static readonly PaddingStrategy SYMMETRIC_WHITE = new PaddingStrategy();

        /// <summary>Image will be put into the top-left corner.</summary>
        /// <remarks>
        /// Image will be put into the top-left corner. Pixels to the right are
        /// repeats of the right-most pixel column of the image. Pixels to the top
        /// are repeats of the top-most pixel row of the image.
        /// </remarks>
        public static readonly PaddingStrategy BOTTOM_RIGHT_EDGE = new PaddingStrategy();

        /// <summary>Image will be put into the middle.</summary>
        /// <remarks>
        /// Image will be put into the middle. Pixels to the left and to the right
        /// are repeats of the left-most and the right-most pixel columns of the
        /// image respectfully. Pixels to the bottom and to the top are repeats of
        /// the bottom-most and the top-most pixel rows of the image.
        /// </remarks>
        public static readonly PaddingStrategy SYMMETRIC_EDGE = new PaddingStrategy();

        /// <summary>Returns the solid color used for padding.</summary>
        /// <remarks>
        /// Returns the solid color used for padding. If the strategy doesn't use a
        /// solid color, returns
        /// <see langword="null"/>.
        /// </remarks>
        /// <returns>
        /// the solid color used for padding, or
        /// <see langword="null"/>
        /// </returns>
        public Color GetSolidColor() {
            if (PaddingStrategy.BOTTOM_RIGHT_BLACK == this || PaddingStrategy.SYMMETRIC_BLACK == this) {
                return IronSoftware.Drawing.Color.Black;
            }
            else {
                if (PaddingStrategy.BOTTOM_RIGHT_GRAY == this || PaddingStrategy.SYMMETRIC_GRAY == this) {
                    return IronSoftware.Drawing.Color.Gray;
                }
                else {
                    if (PaddingStrategy.BOTTOM_RIGHT_WHITE == this || PaddingStrategy.SYMMETRIC_WHITE == this) {
                        return IronSoftware.Drawing.Color.White;
                    }
                    else {
                        if (PaddingStrategy.SYMMETRIC_EDGE == this || PaddingStrategy.BOTTOM_RIGHT_EDGE == this) {
                            return null;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>Returns whether the strategy uses a solid color for padding.</summary>
        /// <returns>whether the strategy uses a solid color for padding</returns>
        public bool UsesSolidColor() {
            if (PaddingStrategy.BOTTOM_RIGHT_BLACK == this || PaddingStrategy.BOTTOM_RIGHT_GRAY == this || PaddingStrategy
                .BOTTOM_RIGHT_WHITE == this || PaddingStrategy.SYMMETRIC_BLACK == this || PaddingStrategy.SYMMETRIC_GRAY
                 == this || PaddingStrategy.SYMMETRIC_WHITE == this) {
                return true;
            }
            else {
                if (PaddingStrategy.SYMMETRIC_EDGE == this || PaddingStrategy.BOTTOM_RIGHT_EDGE == this) {
                    return false;
                }
            }
            return false;
        }

        /// <summary>Returns whether the strategy uses the edge of the image for padding.</summary>
        /// <returns>whether the strategy uses the edge of the image for padding</returns>
        public bool UsesImageEdge() {
            if (PaddingStrategy.SYMMETRIC_EDGE == this || PaddingStrategy.BOTTOM_RIGHT_EDGE == this) {
                return true;
            }
            else {
                if (PaddingStrategy.BOTTOM_RIGHT_BLACK == this || PaddingStrategy.BOTTOM_RIGHT_GRAY == this || PaddingStrategy
                    .BOTTOM_RIGHT_WHITE == this || PaddingStrategy.SYMMETRIC_BLACK == this || PaddingStrategy.SYMMETRIC_GRAY
                     == this || PaddingStrategy.SYMMETRIC_WHITE == this) {
                    return false;
                }
            }
            return false;
        }

        /// <summary>Returns whether the strategy uses symmetric padding.</summary>
        /// <returns>whether the strategy uses symmetric padding</returns>
        public bool UsesSymmetricPadding() {
            if (PaddingStrategy.SYMMETRIC_BLACK == this || PaddingStrategy.SYMMETRIC_GRAY == this || PaddingStrategy.SYMMETRIC_WHITE
                 == this || PaddingStrategy.SYMMETRIC_EDGE == this) {
                return true;
            }
            else {
                if (PaddingStrategy.BOTTOM_RIGHT_BLACK == this || PaddingStrategy.BOTTOM_RIGHT_GRAY == this || PaddingStrategy
                    .BOTTOM_RIGHT_WHITE == this || PaddingStrategy.BOTTOM_RIGHT_EDGE == this) {
                    return false;
                }
            }
            return false;
        }

        /// <summary>Returns whether the strategy uses bottom-right padding.</summary>
        /// <returns>whether the strategy uses bottom-right padding</returns>
        public bool UsesBottomRightPadding() {
            if (PaddingStrategy.BOTTOM_RIGHT_BLACK == this || PaddingStrategy.BOTTOM_RIGHT_GRAY == this || PaddingStrategy
                .BOTTOM_RIGHT_WHITE == this || PaddingStrategy.BOTTOM_RIGHT_EDGE == this) {
                return true;
            }
            else {
                if (PaddingStrategy.SYMMETRIC_BLACK == this || PaddingStrategy.SYMMETRIC_GRAY == this || PaddingStrategy.SYMMETRIC_WHITE
                     == this || PaddingStrategy.SYMMETRIC_EDGE == this) {
                    return false;
                }
            }
            return false;
        }
    }
}
