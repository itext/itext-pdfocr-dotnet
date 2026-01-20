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
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr.Conf.Paddle.Model {
    /// <summary>
    /// POJO for the DecodeImage transform operation within a
    /// <c>PreProcess</c>
    /// object in a config file.
    /// </summary>
    public class DecodeImage : TransformOp {
        /// <summary>Expected wrapping key for the DecodeImage operation.</summary>
        public const String WRAPPING_KEY = "DecodeImage";

        private readonly bool channelFirst;

        private readonly ImgMode imgMode;

        /// <summary>Creates a new POJO for the config file object.</summary>
        /// <param name="channelFirst">
        /// value under the
        /// <c>channel_first</c>
        /// key
        /// </param>
        /// <param name="imgMode">
        /// value under the
        /// <c>img_mode</c>
        /// key
        /// </param>
        public DecodeImage(bool channelFirst, ImgMode imgMode) {
            this.channelFirst = channelFirst;
            this.imgMode = Objects.RequireNonNull(imgMode);
        }

        /// <summary><inheritDoc/></summary>
        public virtual String GetWrappingKey() {
            return WRAPPING_KEY;
        }

        /// <summary>
        /// Returns the value under the
        /// <c>channel_first</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the value under the
        /// <c>channel_first</c>
        /// key
        /// </returns>
        public virtual bool GetChannelFirst() {
            return channelFirst;
        }

        /// <summary>
        /// Returns the value under the
        /// <c>img_mode</c>
        /// key.
        /// </summary>
        /// <returns>
        /// the value under the
        /// <c>img_mode</c>
        /// key
        /// </returns>
        public virtual ImgMode GetImgMode() {
            return imgMode;
        }

        /// <summary><inheritDoc/></summary>
        public override bool Equals(Object o) {
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Pdfocr.Onnxtr.Conf.Paddle.Model.DecodeImage that = (iText.Pdfocr.Onnxtr.Conf.Paddle.Model.DecodeImage
                )o;
            return channelFirst == that.channelFirst && imgMode == that.imgMode;
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode((Object)channelFirst, imgMode);
        }
    }
}
