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
using iText.Commons.Utils;
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr {
    /// <summary>
    /// Options, that describe the way an image will be resized before being
    /// converted to a tensor for an ML model input.
    /// </summary>
    /// <remarks>
    /// Options, that describe the way an image will be resized before being
    /// converted to a tensor for an ML model input.
    /// <para />
    /// At the moment only ratio-preserving resizing is supported.
    /// </remarks>
    public class ImageResizeOptions {
        /// <summary>
        /// Specifies the image channel configuration, that will be used when
        /// passing the image to the ML model.
        /// </summary>
        /// <remarks>
        /// Specifies the image channel configuration, that will be used when
        /// passing the image to the ML model.
        /// <para />
        /// While this is not directly relates to image resizing and padding, it is
        /// important for us to know to which color model to use for the final
        /// image.
        /// </remarks>
        private readonly ImageChannelConfiguration channelConfiguration;

        /// <summary>Minimum width the image should be after resizing.</summary>
        /// <remarks>
        /// Minimum width the image should be after resizing. Padding might be used
        /// to get to this value.
        /// <para />
        /// Should be a positive value.
        /// </remarks>
        private readonly int minWidth;

        /// <summary>Minimum height the image should be after resizing.</summary>
        /// <remarks>
        /// Minimum height the image should be after resizing. Padding might be used
        /// to get to this value.
        /// <para />
        /// Should be a positive value.
        /// </remarks>
        private readonly int minHeight;

        /// <summary>Maximum width the image should be after resizing.</summary>
        /// <remarks>
        /// Maximum width the image should be after resizing.
        /// <para />
        /// Should be a positive value not less than
        /// <c>minWidth</c>
        /// . This value
        /// should also be a multiple of
        /// <c>widthMultiple</c>.
        /// </remarks>
        private readonly int maxWidth;

        /// <summary>Maximum height the image should be after resizing.</summary>
        /// <remarks>
        /// Maximum height the image should be after resizing.
        /// <para />
        /// Should be a positive value not less than
        /// <c>minHeight</c>
        /// . This value
        /// should also be a multiple of
        /// <c>heightMultiple</c>.
        /// </remarks>
        private readonly int maxHeight;

        /// <summary>
        /// After resizing, the width of the image should be a multiple of this
        /// value.
        /// </summary>
        /// <remarks>
        /// After resizing, the width of the image should be a multiple of this
        /// value.
        /// <para />
        /// Very likely, that you don't need to worry about this parameter, and
        /// should leave this at the default value (1). But some models (ex.
        /// PaddleOCR detection ones) are picky about image size in this way.
        /// <para />
        /// Should be a positive value.
        /// </remarks>
        private readonly int widthMultiple;

        /// <summary>
        /// After resizing, the height of the image should be a multiple of this
        /// value.
        /// </summary>
        /// <remarks>
        /// After resizing, the height of the image should be a multiple of this
        /// value.
        /// <para />
        /// Very likely, that you don't need to worry about this parameter, and
        /// should leave this at the default value (1). But some models (ex.
        /// PaddleOCR detection ones) are picky about image size in this way.
        /// <para />
        /// Should be a positive value.
        /// </remarks>
        private readonly int heightMultiple;

        /// <summary>
        /// Padding strategy to be used, when resizing an image to the target
        /// size.
        /// </summary>
        /// <remarks>
        /// Padding strategy to be used, when resizing an image to the target
        /// size.
        /// <para />
        /// We only do ratio-preserving resizing, so padding might be required to
        /// get the image to the target size.
        /// </remarks>
        private readonly PaddingStrategy paddingStrategy;

        /// <summary>Creates image resize options.</summary>
        /// <param name="channelConfiguration">
        /// channel configuration, that will be used, when passing the image
        /// to the ML model
        /// </param>
        /// <param name="minWidth">
        /// minimum width the image should be after resizing. Should be a
        /// positive value
        /// </param>
        /// <param name="minHeight">
        /// minimum height the image should be after resizing. Should be a
        /// positive value
        /// </param>
        /// <param name="maxWidth">
        /// maximum width the image should be after resizing. Should not be
        /// less than
        /// <paramref name="minWidth"/>
        /// . Should be a multiple of
        /// <paramref name="widthMultiple"/>
        /// </param>
        /// <param name="maxHeight">
        /// maximum height the image should be after resizing. Should not be
        /// less than
        /// <paramref name="minHeight"/>
        /// . Should be a multiple of
        /// <paramref name="heightMultiple"/>
        /// </param>
        /// <param name="widthMultiple">
        /// after resizing, the width of the image should be a multiple of
        /// this value
        /// </param>
        /// <param name="heightMultiple">
        /// after resizing, the height of the image should be a multiple of
        /// this value
        /// </param>
        /// <param name="paddingStrategy">padding strategy to be used</param>
        public ImageResizeOptions(ImageChannelConfiguration channelConfiguration, int minWidth, int minHeight, int
             maxWidth, int maxHeight, int widthMultiple, int heightMultiple, PaddingStrategy paddingStrategy) {
            Objects.RequireNonNull(channelConfiguration);
            this.channelConfiguration = channelConfiguration;
            if (minWidth < 1) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.MIN_WIDTH_SHOULD_BE_POSITIVE
                    , minWidth));
            }
            this.minWidth = minWidth;
            if (minHeight < 1) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.MIN_HEIGHT_SHOULD_BE_POSITIVE
                    , minHeight));
            }
            this.minHeight = minHeight;
            if (widthMultiple < 1) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.WIDTH_MULTIPLE_SHOULD_BE_POSITIVE
                    , widthMultiple));
            }
            this.widthMultiple = widthMultiple;
            if (heightMultiple < 1) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.HEIGHT_MULTIPLE_SHOULD_BE_POSITIVE
                    , heightMultiple));
            }
            this.heightMultiple = heightMultiple;
            if (maxWidth < minWidth) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.MAX_WIDTH_SHOULD_NOT_BE_LESS_THAN_MIN
                    , maxWidth));
            }
            if (maxWidth % widthMultiple != 0) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.MAX_WIDTH_SHOULD_BE_A_MULTIPLE
                    , widthMultiple, maxWidth));
            }
            this.maxWidth = maxWidth;
            if (maxHeight < minHeight) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.MAX_HEIGHT_SHOULD_NOT_BE_LESS_THAN_MIN
                    , maxHeight));
            }
            if (maxHeight % heightMultiple != 0) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.MAX_HEIGHT_SHOULD_BE_A_MULTIPLE
                    , heightMultiple, maxHeight));
            }
            this.maxHeight = maxHeight;
            Objects.RequireNonNull(paddingStrategy);
            this.paddingStrategy = paddingStrategy;
        }

        /// <summary>Creates image resize options.</summary>
        /// <remarks>
        /// Creates image resize options.
        /// <para />
        /// With this constructor variant output image dimensions are not bumped up to be a multiple of
        /// some integer value.
        /// </remarks>
        /// <param name="channelConfiguration">
        /// channel configuration, that will be used, when passing the image
        /// to the ML model
        /// </param>
        /// <param name="minWidth">
        /// minimum width the image should be after resizing. Should be a
        /// positive value
        /// </param>
        /// <param name="minHeight">
        /// minimum height the image should be after resizing. Should be a
        /// positive value
        /// </param>
        /// <param name="maxWidth">
        /// maximum width the image should be after resizing. Should not be
        /// less than
        /// <paramref name="minWidth"/>
        /// </param>
        /// <param name="maxHeight">
        /// maximum height the image should be after resizing. Should not be
        /// less than
        /// <paramref name="minHeight"/>
        /// </param>
        /// <param name="paddingStrategy">padding strategy to be used</param>
        public ImageResizeOptions(ImageChannelConfiguration channelConfiguration, int minWidth, int minHeight, int
             maxWidth, int maxHeight, PaddingStrategy paddingStrategy)
            : this(channelConfiguration, minWidth, minHeight, maxWidth, maxHeight, 1, 1, paddingStrategy) {
        }

        /// <summary>Creates image resize options.</summary>
        /// <remarks>
        /// Creates image resize options.
        /// <para />
        /// With this constructor variant output image dimensions are not bumped up to be a multiple of
        /// some integer value and a default black padding is added at the bottom-right of the image.
        /// </remarks>
        /// <param name="channelConfiguration">
        /// channel configuration, that will be used, when passing the image
        /// to the ML model
        /// </param>
        /// <param name="minWidth">
        /// minimum width the image should be after resizing. Should be a
        /// positive value
        /// </param>
        /// <param name="minHeight">
        /// minimum height the image should be after resizing. Should be a
        /// positive value
        /// </param>
        /// <param name="maxWidth">
        /// maximum width the image should be after resizing. Should not be
        /// less than
        /// <paramref name="minWidth"/>
        /// </param>
        /// <param name="maxHeight">
        /// maximum height the image should be after resizing. Should not be
        /// less than
        /// <paramref name="minHeight"/>
        /// </param>
        public ImageResizeOptions(ImageChannelConfiguration channelConfiguration, int minWidth, int minHeight, int
             maxWidth, int maxHeight)
            : this(channelConfiguration, minWidth, minHeight, maxWidth, maxHeight, PaddingStrategy.BOTTOM_RIGHT_BLACK) {
        }

        /// <summary>Creates image resize options.</summary>
        /// <remarks>
        /// Creates image resize options.
        /// <para />
        /// With this constructor variant output image dimensions are fixed to the provided values.
        /// </remarks>
        /// <param name="channelConfiguration">
        /// channel configuration, that will be used, when passing the image
        /// to the ML model
        /// </param>
        /// <param name="targetWidth">
        /// width the image should be after resizing. Should be a positive
        /// value
        /// </param>
        /// <param name="targetHeight">
        /// height the image should be after resizing. Should be a positive
        /// value
        /// </param>
        /// <param name="paddingStrategy">padding strategy to be used</param>
        public ImageResizeOptions(ImageChannelConfiguration channelConfiguration, int targetWidth, int targetHeight
            , PaddingStrategy paddingStrategy)
            : this(channelConfiguration, targetWidth, targetHeight, targetWidth, targetHeight, paddingStrategy) {
        }

        /// <summary>Creates image resize options.</summary>
        /// <remarks>
        /// Creates image resize options.
        /// <para />
        /// With this constructor variant output image dimensions are fixed to the provided values and
        /// a default black padding is added at the bottom-right of the image.
        /// </remarks>
        /// <param name="channelConfiguration">
        /// channel configuration, that will be used, when passing the image
        /// to the ML model
        /// </param>
        /// <param name="targetWidth">
        /// width the image should be after resizing. Should be a positive
        /// value
        /// </param>
        /// <param name="targetHeight">
        /// height the image should be after resizing. Should be a positive
        /// value
        /// </param>
        public ImageResizeOptions(ImageChannelConfiguration channelConfiguration, int targetWidth, int targetHeight
            )
            : this(channelConfiguration, targetWidth, targetHeight, PaddingStrategy.BOTTOM_RIGHT_BLACK) {
        }

        /// <summary>
        /// Returns the image channel configuration, that will be used when passing
        /// the image to the ML model.
        /// </summary>
        /// <returns>the image channel configuration</returns>
        public virtual ImageChannelConfiguration GetChannelConfiguration() {
            return channelConfiguration;
        }

        /// <summary>Returns the minimum width the image should be after resizing.</summary>
        /// <returns>the minimum width the image should be after resizing</returns>
        public virtual int GetMinWidth() {
            return minWidth;
        }

        /// <summary>Returns the minimum height the image should be after resizing.</summary>
        /// <returns>the minimum height the image should be after resizing</returns>
        public virtual int GetMinHeight() {
            return minHeight;
        }

        /// <summary>Returns the maximum width the image should be after resizing.</summary>
        /// <returns>the maximum width the image should be after resizing</returns>
        public virtual int GetMaxWidth() {
            return maxWidth;
        }

        /// <summary>Returns the maximum height the image should be after resizing.</summary>
        /// <returns>the maximum height the image should be after resizing</returns>
        public virtual int GetMaxHeight() {
            return maxHeight;
        }

        /// <summary>Returns the width multiple.</summary>
        /// <remarks>
        /// Returns the width multiple.
        /// <para />
        /// After resizing, the width of the image should be a multiple of this
        /// value.
        /// </remarks>
        /// <returns>the width multiple</returns>
        public virtual int GetWidthMultiple() {
            return widthMultiple;
        }

        /// <summary>Returns the height multiple.</summary>
        /// <remarks>
        /// Returns the height multiple.
        /// <para />
        /// After resizing, the height of the image should be a multiple of this
        /// value.
        /// </remarks>
        /// <returns>the height multiple</returns>
        public virtual int GetHeightMultiple() {
            return heightMultiple;
        }

        /// <summary>Returns the padding strategy.</summary>
        /// <returns>the padding strategy</returns>
        public virtual PaddingStrategy GetPaddingStrategy() {
            return paddingStrategy;
        }

        /// <summary>Returns whether the target size is fixed.</summary>
        /// <returns>whether the target size is fixed.</returns>
        public virtual bool IsFixedSize() {
            return (minWidth == maxWidth) && (minHeight == maxHeight);
        }

        public override bool Equals(Object o) {
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnxtr.ImageResizeOptions that = (iText.Pdfocr.Onnxtr.ImageResizeOptions)o;
            return minWidth == that.minWidth && minHeight == that.minHeight && maxWidth == that.maxWidth && maxHeight 
                == that.maxHeight && widthMultiple == that.widthMultiple && heightMultiple == that.heightMultiple && channelConfiguration
                 == that.channelConfiguration && paddingStrategy == that.paddingStrategy;
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)channelConfiguration, minWidth, minHeight, maxWidth, maxHeight, widthMultiple
                , heightMultiple, paddingStrategy);
        }

        public override String ToString() {
            return "ImageResizeOptions{" + "channelConfiguration=" + channelConfiguration + ", minWidth=" + minWidth +
                 ", minHeight=" + minHeight + ", maxWidth=" + maxWidth + ", maxHeight=" + maxHeight + ", widthMultiple="
                 + widthMultiple + ", heightMultiple=" + heightMultiple + ", paddingStrategy=" + paddingStrategy + '}';
        }
    }
}
