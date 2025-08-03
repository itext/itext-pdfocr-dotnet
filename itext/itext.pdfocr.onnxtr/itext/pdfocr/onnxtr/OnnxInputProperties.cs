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
using iText.Commons.Utils;
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr {
    /// <summary>Properties of the input of an ONNX model, which expects an RGB image.</summary>
    /// <remarks>
    /// Properties of the input of an ONNX model, which expects an RGB image.
    /// <para />
    /// It contains the input shape, as a [batchSize, channel, height, width] array, mean and standard
    /// deviation values for normalization, whether padding should be symmetrical or not.
    /// </remarks>
    public class OnnxInputProperties {
        /// <summary>Expected channel count.</summary>
        /// <remarks>Expected channel count. We expect RGB format.</remarks>
        public const int EXPECTED_CHANNEL_COUNT = 3;

        /// <summary>Expected shape size.</summary>
        /// <remarks>Expected shape size. We inspect the standard BCHW format (batch, channel, height, width).</remarks>
        public const int EXPECTED_SHAPE_SIZE = 4;

        /// <summary>Per-channel mean, used for normalization.</summary>
        /// <remarks>Per-channel mean, used for normalization. Should be EXPECTED_SHAPE_SIZE length.</remarks>
        private readonly float[] mean;

        /// <summary>Per-channel standard deviation, used for normalization.</summary>
        /// <remarks>Per-channel standard deviation, used for normalization. Should be EXPECTED_SHAPE_SIZE length.</remarks>
        private readonly float[] std;

        /// <summary>Target input shape.</summary>
        /// <remarks>Target input shape. Should be EXPECTED_SHAPE_SIZE length.</remarks>
        private readonly long[] shape;

        /// <summary>Whether padding should be symmetrical during input resizing.</summary>
        private readonly bool symmetricPad;

        /// <summary>Creates model input properties.</summary>
        /// <param name="mean">per-channel mean, used for normalization. Should be EXPECTED_SHAPE_SIZE length</param>
        /// <param name="std">per-channel standard deviation, used for normalization. Should be EXPECTED_SHAPE_SIZE length
        ///     </param>
        /// <param name="shape">target input shape. Should be EXPECTED_SHAPE_SIZE length</param>
        /// <param name="symmetricPad">whether padding should be symmetrical during input resizing</param>
        public OnnxInputProperties(float[] mean, float[] std, long[] shape, bool symmetricPad) {
            Objects.RequireNonNull(mean);
            if (mean.Length != EXPECTED_CHANNEL_COUNT) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_MEAN_CHANNEL_COUNT
                    , EXPECTED_CHANNEL_COUNT));
            }
            Objects.RequireNonNull(std);
            if (std.Length != EXPECTED_CHANNEL_COUNT) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_STD_CHANNEL_COUNT
                    , EXPECTED_CHANNEL_COUNT));
            }
            Objects.RequireNonNull(shape);
            if (shape.Length != EXPECTED_SHAPE_SIZE) {
                throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_SHAPE_SIZE
                    , EXPECTED_SHAPE_SIZE));
            }
            if (shape[1] != EXPECTED_CHANNEL_COUNT) {
                throw new ArgumentException(PdfOcrOnnxTrExceptionMessageConstant.MODEL_ONLY_SUPPORTS_RGB);
            }
            foreach (long dim in shape) {
                if (dim <= 0 || ((int)dim) != dim) {
                    throw new ArgumentException(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_DIMENSION_VALUE
                        , dim));
                }
            }
            this.mean = new float[mean.Length];
            Array.Copy(mean, 0, this.mean, 0, mean.Length);
            this.std = new float[std.Length];
            Array.Copy(std, 0, this.std, 0, std.Length);
            this.shape = new long[shape.Length];
            Array.Copy(shape, 0, this.shape, 0, shape.Length);
            this.symmetricPad = symmetricPad;
        }

        /// <summary>Returns per-channel mean, used for normalization.</summary>
        /// <returns>per-channel mean, used for normalization</returns>
        public virtual float[] GetMean() {
            float[] copy = new float[shape.Length];
            Array.Copy(mean, 0, copy, 0, copy.Length);
            return copy;
        }

        /// <summary>Returns channel-specific mean, used for normalization.</summary>
        /// <param name="index">index of the channel</param>
        /// <returns>channel-specific mean, used for normalization</returns>
        public virtual float GetMean(int index) {
            return mean[index];
        }

        /// <summary>Returns red channel mean, used for normalization.</summary>
        /// <returns>red channel mean, used for normalization</returns>
        public virtual float GetRedMean() {
            return GetMean(0);
        }

        /// <summary>Returns green channel mean, used for normalization.</summary>
        /// <returns>green channel mean, used for normalization</returns>
        public virtual float GetGreenMean() {
            return GetMean(1);
        }

        /// <summary>Returns blue channel mean, used for normalization.</summary>
        /// <returns>blue channel mean, used for normalization</returns>
        public virtual float GetBlueMean() {
            return GetMean(2);
        }

        /// <summary>Returns per-channel standard deviation, used for normalization.</summary>
        /// <returns>per-channel standard deviation, used for normalization</returns>
        public virtual float[] GetStd() {
            float[] copy = new float[shape.Length];
            Array.Copy(std, 0, copy, 0, copy.Length);
            return copy;
        }

        /// <summary>Returns channel-specific standard deviation, used for normalization.</summary>
        /// <param name="index">index of the channel</param>
        /// <returns>channel-specific standard deviation, used for normalization</returns>
        public virtual float GetStd(int index) {
            return std[index];
        }

        /// <summary>Returns red channel standard deviation, used for normalization.</summary>
        /// <returns>red channel standard deviation, used for normalization</returns>
        public virtual float GetRedStd() {
            return GetStd(0);
        }

        /// <summary>Returns green channel standard deviation, used for normalization.</summary>
        /// <returns>green channel standard deviation, used for normalization</returns>
        public virtual float GetGreenStd() {
            return GetStd(1);
        }

        /// <summary>Returns blue channel standard deviation, used for normalization.</summary>
        /// <returns>blue channel standard deviation, used for normalization</returns>
        public virtual float GetBlueStd() {
            return GetStd(2);
        }

        /// <summary>Returns target input shape.</summary>
        /// <returns>target input shape</returns>
        public virtual long[] GetShape() {
            long[] copy = new long[shape.Length];
            Array.Copy(shape, 0, copy, 0, copy.Length);
            return copy;
        }

        /// <summary>Returns target input dimension value.</summary>
        /// <param name="index">index of the dimension</param>
        /// <returns>target input dimension value</returns>
        public virtual int GetShape(int index) {
            return (int)shape[index];
        }

        /// <summary>Returns input batch size.</summary>
        /// <returns>input batch size</returns>
        public virtual int GetBatchSize() {
            return GetShape(0);
        }

        /// <summary>Returns input channel count.</summary>
        /// <returns>input channel count</returns>
        public virtual int GetChannelCount() {
            return GetShape(1);
        }

        /// <summary>Returns input height.</summary>
        /// <returns>input height</returns>
        public virtual int GetHeight() {
            return GetShape(2);
        }

        /// <summary>Returns input width.</summary>
        /// <returns>input width</returns>
        public virtual int GetWidth() {
            return GetShape(3);
        }

        /// <summary>Returns whether padding should be symmetrical during input resizing.</summary>
        /// <returns>whether padding should be symmetrical during input resizing</returns>
        public virtual bool UseSymmetricPad() {
            return symmetricPad;
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)JavaUtil.ArraysHashCode(mean), JavaUtil.ArraysHashCode(std), JavaUtil.ArraysHashCode
                (shape), symmetricPad);
        }

        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnxtr.OnnxInputProperties that = (iText.Pdfocr.Onnxtr.OnnxInputProperties)o;
            return symmetricPad == that.symmetricPad && JavaUtil.ArraysEquals(mean, that.mean) && JavaUtil.ArraysEquals
                (std, that.std) && JavaUtil.ArraysEquals(shape, that.shape);
        }

        public override String ToString() {
            return "OnnxInputProperties{" + "mean=" + JavaUtil.ArraysToString(mean) + ", std=" + JavaUtil.ArraysToString
                (std) + ", shape=" + JavaUtil.ArraysToString(shape) + ", symmetricPad=" + symmetricPad + '}';
        }
    }
}
