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
using iText.Pdfocr.Onnx.Exceptions;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnx {
    /// <summary>Properties of the input of an ONNX model, which expects an image.</summary>
    /// <remarks>
    /// Properties of the input of an ONNX model, which expects an image.
    /// <para />
    /// It contains the input shape (batchSize, channel, height, width), mean and standard
    /// deviation values for normalization, what type of padding should be used.
    /// </remarks>
    public class OnnxInputProperties {
        /// <summary>Expected channel count.</summary>
        /// <remarks>Expected channel count. We expect RGB format.</remarks>
        [System.ObsoleteAttribute(@"Grayscale and BGR are now supported as well. Check the documentation for more information."
            )]
        public const int EXPECTED_CHANNEL_COUNT = 3;

        /// <summary>Expected shape size.</summary>
        /// <remarks>Expected shape size. We expect the standard BCHW format (batch, channel, height, width).</remarks>
        public const int EXPECTED_SHAPE_SIZE = 4;

        /// <summary>Per-channel mean, used for normalization.</summary>
        /// <remarks>
        /// Per-channel mean, used for normalization. Expected length
        /// of the array is based on the specified channel configuration in the
        /// image resize options.
        /// </remarks>
        private readonly float[] mean;

        /// <summary>Per-channel standard deviation, used for normalization.</summary>
        /// <remarks>
        /// Per-channel standard deviation, used for normalization. Expected length
        /// of the array is based on the specified channel configuration in the
        /// image resize options.
        /// </remarks>
        private readonly float[] std;

        /// <summary>
        /// Options, that control the way the input images for the models will be
        /// converted, resized and padded for ML model input.
        /// </summary>
        private readonly ImageResizeOptions imageResizeOptions;

        /// <summary>Batch size used for the ML model input.</summary>
        /// <remarks>
        /// Batch size used for the ML model input.
        /// <para />
        /// Default value is 1. If a GPU is used for calculations, it is worthwhile
        /// to bump this value as high as your VRAM allows you to.
        /// </remarks>
        private readonly int batchSize;

        /// <summary>Creates model input properties.</summary>
        /// <param name="mean">per-channel mean, used for normalization. Should be EXPECTED_CHANNEL_COUNT length</param>
        /// <param name="std">per-channel standard deviation, used for normalization. Should be EXPECTED_CHANNEL_COUNT length
        ///     </param>
        /// <param name="shape">target input shape. Should be EXPECTED_SHAPE_SIZE length</param>
        /// <param name="symmetricPad">whether padding should be symmetrical during input resizing</param>
        [System.ObsoleteAttribute(@"This is the original constructor, which only supported RGB inputs with a static width/height and black pixel values padding. Use constructors with an ImageResizeOptions parameter instead."
            )]
        public OnnxInputProperties(float[] mean, float[] std, long[] shape, bool symmetricPad) {
            Objects.RequireNonNull(mean);
            if (mean.Length != EXPECTED_CHANNEL_COUNT) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_MEAN_CHANNEL_COUNT
                    , EXPECTED_CHANNEL_COUNT));
            }
            Objects.RequireNonNull(std);
            if (std.Length != EXPECTED_CHANNEL_COUNT) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_STD_CHANNEL_COUNT
                    , EXPECTED_CHANNEL_COUNT));
            }
            Objects.RequireNonNull(shape);
            if (shape.Length != EXPECTED_SHAPE_SIZE) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_SHAPE_SIZE
                    , EXPECTED_SHAPE_SIZE));
            }
            if (shape[1] != EXPECTED_CHANNEL_COUNT) {
                throw new ArgumentException(PdfOcrOnnxExceptionMessageConstant.MODEL_ONLY_SUPPORTS_RGB);
            }
            foreach (long dim in shape) {
                if (dim <= 0 || ((int)dim) != dim) {
                    throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_DIMENSION_VALUE
                        , dim));
                }
            }
            this.mean = new float[mean.Length];
            Array.Copy(mean, 0, this.mean, 0, mean.Length);
            this.std = new float[std.Length];
            Array.Copy(std, 0, this.std, 0, std.Length);
            this.imageResizeOptions = new ImageResizeOptions(ImageChannelConfiguration.RGB, (int)shape[3], (int)shape[
                2], (symmetricPad ? PaddingStrategy.SYMMETRIC_BLACK : PaddingStrategy.BOTTOM_RIGHT_BLACK));
            this.batchSize = (int)shape[0];
        }

        /// <summary>Creates model input properties.</summary>
        /// <param name="imageResizeOptions">
        /// options, that control the way the input images for the models will
        /// be converted, resized and padded for ML model input
        /// </param>
        /// <param name="mean">
        /// per-channel mean, used for normalization. Length of the array
        /// should match the channel count in the image resize options
        /// </param>
        /// <param name="std">
        /// per-channel standard deviation, used for normalization. Length of
        /// the array should match the channel count in the image resize
        /// options
        /// </param>
        /// <param name="batchSize">
        /// size of the batch used for the ML model. Should be a positive
        /// number
        /// </param>
        public OnnxInputProperties(ImageResizeOptions imageResizeOptions, float[] mean, float[] std, int batchSize
            ) {
            Objects.RequireNonNull(imageResizeOptions);
            this.imageResizeOptions = imageResizeOptions;
            int channelCount = imageResizeOptions.GetChannelConfiguration().GetChannelCount();
            Objects.RequireNonNull(mean);
            if (mean.Length != channelCount) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_MEAN_CHANNEL_COUNT
                    , channelCount));
            }
            this.mean = new float[mean.Length];
            Array.Copy(mean, 0, this.mean, 0, mean.Length);
            Objects.RequireNonNull(std);
            if (std.Length != channelCount) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_STD_CHANNEL_COUNT
                    , channelCount));
            }
            this.std = new float[std.Length];
            Array.Copy(std, 0, this.std, 0, std.Length);
            if (batchSize < 1) {
                throw new ArgumentException(PdfOcrOnnxExceptionMessageConstant.BATCH_SIZE_SHOULD_BE_POSITIVE);
            }
            this.batchSize = batchSize;
        }

        /// <summary>Creates model input properties.</summary>
        /// <remarks>
        /// Creates model input properties.
        /// <para />
        /// With this constructor variant batching is disabled (i.e. batch size is set to 1).
        /// </remarks>
        /// <param name="imageResizeOptions">
        /// options, that control the way the input images for the models will
        /// be converted, resized and padded for ML model input
        /// </param>
        /// <param name="mean">
        /// per-channel mean, used for normalization. Length of the array
        /// should match the channel count in the image resize options
        /// </param>
        /// <param name="std">
        /// per-channel standard deviation, used for normalization. Length of
        /// the array should match the channel count in the image resize
        /// options
        /// </param>
        public OnnxInputProperties(ImageResizeOptions imageResizeOptions, float[] mean, float[] std)
            : this(imageResizeOptions, mean, std, 1) {
        }

        /// <summary>Creates model input properties.</summary>
        /// <remarks>
        /// Creates model input properties.
        /// <para />
        /// With this constructor variant no input normalization is done, only mapping to [0; 1].
        /// </remarks>
        /// <param name="imageResizeOptions">
        /// options, that control the way the input images for the models will
        /// be converted, resized and padded for ML model input
        /// </param>
        /// <param name="batchSize">
        /// size of the batch used for the ML model. Should be a positive
        /// number
        /// </param>
        public OnnxInputProperties(ImageResizeOptions imageResizeOptions, int batchSize)
            : this(imageResizeOptions, NewNoopMean(imageResizeOptions), NewNoopStd(imageResizeOptions), batchSize) {
        }

        /// <summary>Creates model input properties.</summary>
        /// <remarks>
        /// Creates model input properties.
        /// <para />
        /// With this constructor variant no input normalization is done, only mapping to [0; 1], and
        /// batching is disabled (i.e. batch size is set to 1).
        /// </remarks>
        /// <param name="imageResizeOptions">
        /// options, that control the way the input images for the models will
        /// be converted, resized and padded for ML model input
        /// </param>
        public OnnxInputProperties(ImageResizeOptions imageResizeOptions)
            : this(imageResizeOptions, 1) {
        }

        /// <summary>Returns image resize options for the input.</summary>
        /// <returns>image resize options for the input.</returns>
        public virtual ImageResizeOptions GetImageResizeOptions() {
            return imageResizeOptions;
        }

        /// <summary>Returns per-channel mean, used for normalization.</summary>
        /// <returns>per-channel mean, used for normalization</returns>
        public virtual float[] GetMean() {
            float[] copy = new float[mean.Length];
            Array.Copy(mean, 0, copy, 0, copy.Length);
            return copy;
        }

        /// <summary>Returns channel-specific mean, used for normalization.</summary>
        /// <param name="index">index of the channel</param>
        /// <returns>channel-specific mean, used for normalization</returns>
        public virtual float GetMean(int index) {
            return mean[index];
        }

        /// <summary>Returns gray channel mean, used for normalization.</summary>
        /// <returns>gray channel mean, used for normalization</returns>
        public virtual float GetGrayMean() {
            return GetMean(0);
        }

        /// <summary>Returns red channel mean, used for normalization.</summary>
        /// <returns>red channel mean, used for normalization</returns>
        public virtual float GetRedMean() {
            return GetMean(imageResizeOptions.GetChannelConfiguration().GetRedChannelIndex());
        }

        /// <summary>Returns green channel mean, used for normalization.</summary>
        /// <returns>green channel mean, used for normalization</returns>
        public virtual float GetGreenMean() {
            return GetMean(imageResizeOptions.GetChannelConfiguration().GetGreenChannelIndex());
        }

        /// <summary>Returns blue channel mean, used for normalization.</summary>
        /// <returns>blue channel mean, used for normalization</returns>
        public virtual float GetBlueMean() {
            return GetMean(imageResizeOptions.GetChannelConfiguration().GetBlueChannelIndex());
        }

        /// <summary>Returns per-channel standard deviation, used for normalization.</summary>
        /// <returns>per-channel standard deviation, used for normalization</returns>
        public virtual float[] GetStd() {
            float[] copy = new float[std.Length];
            Array.Copy(std, 0, copy, 0, copy.Length);
            return copy;
        }

        /// <summary>Returns channel-specific standard deviation, used for normalization.</summary>
        /// <param name="index">index of the channel</param>
        /// <returns>channel-specific standard deviation, used for normalization</returns>
        public virtual float GetStd(int index) {
            return std[index];
        }

        /// <summary>Returns gray channel standard deviation, used for normalization.</summary>
        /// <returns>gray channel standard deviation, used for normalization</returns>
        public virtual float GetGrayStd() {
            return GetStd(0);
        }

        /// <summary>Returns red channel standard deviation, used for normalization.</summary>
        /// <returns>red channel standard deviation, used for normalization</returns>
        public virtual float GetRedStd() {
            return GetStd(imageResizeOptions.GetChannelConfiguration().GetRedChannelIndex());
        }

        /// <summary>Returns green channel standard deviation, used for normalization.</summary>
        /// <returns>green channel standard deviation, used for normalization</returns>
        public virtual float GetGreenStd() {
            return GetStd(imageResizeOptions.GetChannelConfiguration().GetGreenChannelIndex());
        }

        /// <summary>Returns blue channel standard deviation, used for normalization.</summary>
        /// <returns>blue channel standard deviation, used for normalization</returns>
        public virtual float GetBlueStd() {
            return GetStd(imageResizeOptions.GetChannelConfiguration().GetBlueChannelIndex());
        }

        /// <summary>Returns target input shape.</summary>
        /// <remarks>Returns target input shape. Minimum height and width are used.</remarks>
        /// <returns>target input shape</returns>
        public virtual long[] GetShape() {
            return new long[] { GetBatchSize(), GetChannelCount(), GetHeight(), GetWidth() };
        }

        /// <summary>Returns target input dimension value.</summary>
        /// <param name="index">index of the dimension</param>
        /// <returns>target input dimension value</returns>
        public virtual int GetShape(int index) {
            switch (index) {
                case 0: {
                    return GetBatchSize();
                }

                case 1: {
                    return GetChannelCount();
                }

                case 2: {
                    return GetHeight();
                }

                case 3: {
                    return GetWidth();
                }

                default: {
                    break;
                }
            }
            // Fallthrough
            throw new IndexOutOfRangeException(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.INDEX_OUT_OF_BOUNDS
                , index));
        }

        /// <summary>Returns input batch size.</summary>
        /// <returns>input batch size</returns>
        public virtual int GetBatchSize() {
            return batchSize;
        }

        /// <summary>Returns input channel count.</summary>
        /// <returns>input channel count</returns>
        public virtual int GetChannelCount() {
            return imageResizeOptions.GetChannelConfiguration().GetChannelCount();
        }

        /// <summary>Returns input minimum height.</summary>
        /// <returns>input minimum height</returns>
        public virtual int GetHeight() {
            return imageResizeOptions.GetMinHeight();
        }

        /// <summary>Returns input minimum width.</summary>
        /// <returns>input minimum width</returns>
        public virtual int GetWidth() {
            return imageResizeOptions.GetMinWidth();
        }

        /// <summary>Returns whether padding should be symmetrical during input resizing.</summary>
        /// <returns>whether padding should be symmetrical during input resizing</returns>
        public virtual bool UseSymmetricPad() {
            return imageResizeOptions.GetPaddingStrategy().UsesSymmetricPadding();
        }

        /// <summary>Returns the padding strategy for image inputs.</summary>
        /// <returns>the padding strategy for image inputs</returns>
        public virtual PaddingStrategy GetPaddingStrategy() {
            return imageResizeOptions.GetPaddingStrategy();
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)JavaUtil.ArraysHashCode(mean), JavaUtil.ArraysHashCode(std), imageResizeOptions
                , batchSize);
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object o) {
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnx.OnnxInputProperties that = (iText.Pdfocr.Onnx.OnnxInputProperties)o;
            return batchSize == that.batchSize && JavaUtil.ArraysEquals(mean, that.mean) && JavaUtil.ArraysEquals(std, 
                that.std) && Object.Equals(imageResizeOptions, that.imageResizeOptions);
        }

        /// <summary><inheritDoc/></summary>
        public override String ToString() {
            return "OnnxInputProperties{" + "mean=" + JavaUtil.ArraysToString(mean) + ", std=" + JavaUtil.ArraysToString
                (std) + ", imageResizeOptions=" + imageResizeOptions + ", batchSize=" + batchSize + '}';
        }

        private static float[] NewNoopMean(ImageResizeOptions imageResizeOptions) {
            int channelCount = imageResizeOptions.GetChannelConfiguration().GetChannelCount();
            float[] mean = new float[channelCount];
            JavaUtil.Fill(mean, 0.0F);
            return mean;
        }

        private static float[] NewNoopStd(ImageResizeOptions imageResizeOptions) {
            int channelCount = imageResizeOptions.GetChannelConfiguration().GetChannelCount();
            float[] std = new float[channelCount];
            JavaUtil.Fill(std, 1.0F);
            return std;
        }
    }
}
