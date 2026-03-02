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
using iText.Pdfocr.Onnx.Exceptions;

namespace iText.Pdfocr.Onnx {
    /// <summary>Enumeration of supported image channel configuration for buffers.</summary>
    /// <remarks>
    /// Enumeration of supported image channel configuration for buffers. These are
    /// used, when we need to provide an input to an ML model from an image.
    /// </remarks>
    public sealed class ImageChannelConfiguration {
        /// <summary>
        /// Image is represented with a single channel, which contains the
        /// grayscale version of the image.
        /// </summary>
        public static readonly ImageChannelConfiguration GRAYSCALE = new ImageChannelConfiguration();

        /// <summary>Image is represented with three channels: red, green, blue.</summary>
        public static readonly ImageChannelConfiguration RGB = new ImageChannelConfiguration();

        /// <summary>Image is represented with three channels: blue, green, red.</summary>
        public static readonly ImageChannelConfiguration BGR = new ImageChannelConfiguration();

        /// <summary>Returns the amount of channels used to store the image.</summary>
        /// <returns>the amount of channels used to store the image</returns>
        public int GetChannelCount() {
            if (ImageChannelConfiguration.GRAYSCALE == this) {
                return 1;
            }
            else {
                if (ImageChannelConfiguration.RGB == this || ImageChannelConfiguration.BGR == this) {
                    return 3;
                }
            }
            // Should not get here
            throw new InvalidOperationException(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_CHANNEL_CONFIGURATION);
        }

        /// <summary>Returns the index of the red channel in the resulting ML input buffer.</summary>
        /// <returns>the index of the red channel in the resulting ML input buffer</returns>
        public int GetRedChannelIndex() {
            if (ImageChannelConfiguration.GRAYSCALE == this || ImageChannelConfiguration.RGB == this) {
                return 0;
            }
            else {
                if (ImageChannelConfiguration.BGR == this) {
                    return 2;
                }
            }
            // Should not get here
            throw new InvalidOperationException(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_CHANNEL_CONFIGURATION);
        }

        /// <summary>Returns the index of the green channel in the resulting ML input buffer.</summary>
        /// <returns>the index of the green channel in the resulting ML input buffer</returns>
        public int GetGreenChannelIndex() {
            if (ImageChannelConfiguration.GRAYSCALE == this) {
                return 0;
            }
            else {
                if (ImageChannelConfiguration.RGB == this || ImageChannelConfiguration.BGR == this) {
                    return 1;
                }
            }
            // Should not get here
            throw new InvalidOperationException(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_CHANNEL_CONFIGURATION);
        }

        /// <summary>Returns the index of the blue channel in the resulting ML input buffer.</summary>
        /// <returns>the index of the blue channel in the resulting ML input buffer</returns>
        public int GetBlueChannelIndex() {
            if (ImageChannelConfiguration.GRAYSCALE == this || ImageChannelConfiguration.BGR == this) {
                return 0;
            }
            else {
                if (ImageChannelConfiguration.RGB == this) {
                    return 2;
                }
            }
            // Should not get here
            throw new InvalidOperationException(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_CHANNEL_CONFIGURATION);
        }
    }
}
