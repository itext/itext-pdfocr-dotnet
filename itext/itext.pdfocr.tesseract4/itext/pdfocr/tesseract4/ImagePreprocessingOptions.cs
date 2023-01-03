/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: iText Software.

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
namespace iText.Pdfocr.Tesseract4 {
    /// <summary>Additional options applied on image preprocessing step.</summary>
    public class ImagePreprocessingOptions {
        /// <summary>Adaptive threshold tile width as described here: http://www.leptonica.org/binarization.html.</summary>
        /// <remarks>
        /// Adaptive threshold tile width as described here: http://www.leptonica.org/binarization.html.
        /// Default value of 0 is considered as full image width which means no tiling.
        /// </remarks>
        private int tileWidth;

        /// <summary>Adaptive threshold tile height as described here: http://www.leptonica.org/binarization.html.</summary>
        /// <remarks>
        /// Adaptive threshold tile height as described here: http://www.leptonica.org/binarization.html.
        /// Default value of 0 is considered as full image height which means no tiling.
        /// </remarks>
        private int tileHeight;

        /// <summary>Adaptive threshold smoothing as described here: http://www.leptonica.org/binarization.html.</summary>
        private bool smoothTiling = true;

        /// <summary>
        /// Creates
        /// <see cref="ImagePreprocessingOptions"/>
        /// instance.
        /// </summary>
        public ImagePreprocessingOptions() {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="ImagePreprocessingOptions"/>
        /// instance
        /// based on another
        /// <see cref="ImagePreprocessingOptions"/>
        /// instance (copy
        /// constructor).
        /// </summary>
        /// <param name="imagePreprocessingOptions">
        /// the other
        /// <see cref="ImagePreprocessingOptions"/>
        /// instance
        /// </param>
        public ImagePreprocessingOptions(iText.Pdfocr.Tesseract4.ImagePreprocessingOptions imagePreprocessingOptions
            ) {
            this.tileWidth = imagePreprocessingOptions.tileWidth;
            this.tileHeight = imagePreprocessingOptions.tileHeight;
            this.smoothTiling = imagePreprocessingOptions.smoothTiling;
        }

        /// <summary>
        /// Gets
        /// <see cref="tileWidth"/>.
        /// </summary>
        /// <returns>tile width</returns>
        public int GetTileWidth() {
            return tileWidth;
        }

        /// <summary>
        /// Sets
        /// <see cref="tileWidth"/>.
        /// </summary>
        /// <param name="tileWidth">tile width</param>
        /// <returns>
        /// 
        /// <see cref="ImagePreprocessingOptions"/>
        /// </returns>
        public iText.Pdfocr.Tesseract4.ImagePreprocessingOptions SetTileWidth(int tileWidth) {
            this.tileWidth = tileWidth;
            return this;
        }

        /// <summary>
        /// Gets
        /// <see cref="tileHeight"/>.
        /// </summary>
        /// <returns>tile height</returns>
        public int GetTileHeight() {
            return tileHeight;
        }

        /// <summary>
        /// Sets
        /// <see cref="tileHeight"/>.
        /// </summary>
        /// <param name="tileHeight">tile height</param>
        /// <returns>
        /// 
        /// <see cref="ImagePreprocessingOptions"/>
        /// </returns>
        public iText.Pdfocr.Tesseract4.ImagePreprocessingOptions SetTileHeight(int tileHeight) {
            this.tileHeight = tileHeight;
            return this;
        }

        /// <summary>
        /// Gets
        /// <see cref="smoothTiling"/>.
        /// </summary>
        /// <returns>smooth tiling flag</returns>
        public bool IsSmoothTiling() {
            return smoothTiling;
        }

        /// <summary>
        /// Sets
        /// <see cref="smoothTiling"/>.
        /// </summary>
        /// <param name="smoothTiling">smooth tiling flag</param>
        /// <returns>
        /// 
        /// <see cref="ImagePreprocessingOptions"/>
        /// </returns>
        public iText.Pdfocr.Tesseract4.ImagePreprocessingOptions SetSmoothTiling(bool smoothTiling) {
            this.smoothTiling = smoothTiling;
            return this;
        }
    }
}
