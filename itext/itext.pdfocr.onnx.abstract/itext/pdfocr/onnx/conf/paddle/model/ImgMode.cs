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
namespace iText.Pdfocr.Onnx.Conf.Paddle.Model {
    /// <summary>
    /// Enum for values under the
    /// <c>img_mode</c>
    /// key within a DecodeImage
    /// transform operation object in a config file.
    /// </summary>
    public enum ImgMode {
        /// <summary>
        /// Value for
        /// <c>GRAY</c>
        /// config value.
        /// </summary>
        GRAY,
        /// <summary>
        /// Value for
        /// <c>RGB</c>
        /// config value.
        /// </summary>
        RGB,
        /// <summary>
        /// Value for
        /// <c>BGR</c>
        /// config value.
        /// </summary>
        BGR
    }
}
