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

namespace iText.Pdfocr.Onnxtr {
    /// <summary>Properties of the input of an ONNX model, which expects an RGB image.</summary>
    /// <remarks>
    /// Properties of the input of an ONNX model, which expects an RGB image.
    /// <para />
    /// It contains the input shape, as a [batchSize, channel, height, width] array, mean and standard
    /// deviation values for normalization, whether padding should be symmetrical or not.
    /// 
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
        /// <param name="mean">
        /// Per-channel mean, used for normalization.
        /// Should be EXPECTED_SHAPE_SIZE length.
        /// </param>
        /// <param name="std">
        /// Per-channel standard deviation, used for normalization.
        /// Should be EXPECTED_SHAPE_SIZE length.
        /// </param>
        /// <param name="shape">
        /// Target input shape.
        /// Should be EXPECTED_SHAPE_SIZE length.
        /// </param>
        /// <param name="symmetricPad">Whether padding should be symmetrical during input resizing.</param>
        public OnnxInputProperties(float[] mean, float[] std, long[] shape, bool symmetricPad) {
            Objects.RequireNonNull(mean);
            if (mean.Length != EXPECTED_CHANNEL_COUNT) {
                throw new ArgumentException("mean should be a " + EXPECTED_CHANNEL_COUNT + "-element array");
            }
            Objects.RequireNonNull(std);
            if (std.Length != EXPECTED_CHANNEL_COUNT) {
                throw new ArgumentException("std should be a " + EXPECTED_CHANNEL_COUNT + "-element array");
            }
            Objects.RequireNonNull(shape);
            if (shape.Length != EXPECTED_SHAPE_SIZE) {
                throw new ArgumentException("shape should be a " + EXPECTED_SHAPE_SIZE + "-element array (BCHW)");
            }
            if (shape[1] != EXPECTED_CHANNEL_COUNT) {
                throw new ArgumentException("Model only supports RGB images with a BCHW input format");
            }
            foreach (long dim in shape) {
                if (dim <= 0 || ((int)dim) != dim) {
                    throw new ArgumentException("Unexpected dimension value: " + dim);
                }
            }
            this.mean = mean.Clone();
            this.std = std.Clone();
            this.shape = shape.Clone();
            this.symmetricPad = symmetricPad;
        }

        /// <summary>Returns per-channel mean, used for normalization.</summary>
        /// <returns>Per-channel mean, used for normalization.</returns>
        public virtual float[] GetMean() {
            return mean.Clone();
        }

        /// <summary>Returns channel-specific mean, used for normalization.</summary>
        /// <param name="index">Index of the channel.</param>
        /// <returns>Channel-specific mean, used for normalization.</returns>
        public virtual float GetMean(int index) {
            return mean[index];
        }

        /// <summary>Returns red channel mean, used for normalization.</summary>
        /// <returns>Red channel mean, used for normalization.</returns>
        public virtual float GetRedMean() {
            return GetMean(0);
        }

        /// <summary>Returns green channel mean, used for normalization.</summary>
        /// <returns>Green channel mean, used for normalization.</returns>
        public virtual float GetGreenMean() {
            return GetMean(1);
        }

        /// <summary>Returns blue channel mean, used for normalization.</summary>
        /// <returns>Blue channel mean, used for normalization.</returns>
        public virtual float GetBlueMean() {
            return GetMean(2);
        }

        /// <summary>Returns per-channel standard deviation, used for normalization.</summary>
        /// <returns>Per-channel standard deviation, used for normalization.</returns>
        public virtual float[] GetStd() {
            return std.Clone();
        }

        /// <summary>Returns channel-specific standard deviation, used for normalization.</summary>
        /// <param name="index">Index of the channel.</param>
        /// <returns>Channel-specific standard deviation, used for normalization.</returns>
        public virtual float GetStd(int index) {
            return std[index];
        }

        /// <summary>Returns red channel standard deviation, used for normalization.</summary>
        /// <returns>Red channel standard deviation, used for normalization.</returns>
        public virtual float GetRedStd() {
            return GetStd(0);
        }

        /// <summary>Returns green channel standard deviation, used for normalization.</summary>
        /// <returns>Green channel standard deviation, used for normalization.</returns>
        public virtual float GetGreenStd() {
            return GetStd(1);
        }

        /// <summary>Returns blue channel standard deviation, used for normalization.</summary>
        /// <returns>Blue channel standard deviation, used for normalization.</returns>
        public virtual float GetBlueStd() {
            return GetStd(2);
        }

        /// <summary>Returns target input shape.</summary>
        /// <returns>Target input shape.</returns>
        public virtual long[] GetShape() {
            return shape.Clone();
        }

        /// <summary>Returns target input dimension value.</summary>
        /// <param name="index">Index of the dimension.</param>
        /// <returns>Target input dimension value.</returns>
        public virtual int GetShape(int index) {
            return (int)shape[index];
        }

        /// <summary>Returns input batch size.</summary>
        /// <returns>Input batch size.</returns>
        public virtual int GetBatchSize() {
            return GetShape(0);
        }

        /// <summary>Returns input channel count.</summary>
        /// <returns>Input channel count.</returns>
        public virtual int GetChannelCount() {
            return GetShape(1);
        }

        /// <summary>Returns input height.</summary>
        /// <returns>Input height.</returns>
        public virtual int GetHeight() {
            return GetShape(2);
        }

        /// <summary>Returns input width.</summary>
        /// <returns>Input width.</returns>
        public virtual int GetWidth() {
            return GetShape(3);
        }

        /// <summary>Returns whether padding should be symmetrical during input resizing.</summary>
        /// <returns>Whether padding should be symmetrical during input resizing.</returns>
        public virtual bool UseSymmetricPad() {
            return symmetricPad;
        }

        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(JavaUtil.ArraysHashCode(mean), JavaUtil.ArraysHashCode(std), JavaUtil.ArraysHashCode
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
            return symmetricPad == that.symmetricPad && Objects.DeepEquals(mean, that.mean) && Objects.DeepEquals(std, 
                that.std) && Objects.DeepEquals(shape, that.shape);
        }

        public override String ToString() {
            return "OnnxInputProperties{" + "mean=" + JavaUtil.ArraysToString(mean) + ", std=" + JavaUtil.ArraysToString
                (std) + ", shape=" + JavaUtil.ArraysToString(shape) + ", symmetricPad=" + symmetricPad + '}';
        }
    }
}
