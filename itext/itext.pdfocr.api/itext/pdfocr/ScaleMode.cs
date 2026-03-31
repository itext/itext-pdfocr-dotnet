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
using iText.Kernel.Geom;

namespace iText.Pdfocr {
    /// <summary>Enumeration of the possible scale modes for input images.</summary>
    public enum ScaleMode {
        /// <summary>
        /// Only width of the image will be proportionally scaled to fit
        /// required size that is set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method.
        /// </summary>
        /// <remarks>
        /// Only width of the image will be proportionally scaled to fit
        /// required size that is set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method.
        /// Height will be equal to the page height that was set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method and
        /// width will be proportionally scaled to keep the original aspect ratio.
        /// </remarks>
        SCALE_WIDTH,
        /// <summary>
        /// Only height of the image will be proportionally scaled to fit
        /// required size that is set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method.
        /// </summary>
        /// <remarks>
        /// Only height of the image will be proportionally scaled to fit
        /// required size that is set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method.
        /// Width will be equal to the page width that was set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method and
        /// height will be proportionally scaled to keep the original aspect ratio.
        /// </remarks>
        SCALE_HEIGHT,
        /// <summary>
        /// The image will be scaled to fit within the page width and height dimensions that are set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method.
        /// </summary>
        /// <remarks>
        /// The image will be scaled to fit within the page width and height dimensions that are set using
        /// <see cref="OcrPdfCreatorProperties.SetPageSize(Rectangle)"/>
        /// method.
        /// Original aspect ratio of the image stays unchanged.
        /// </remarks>
        SCALE_TO_FIT
    }
}
