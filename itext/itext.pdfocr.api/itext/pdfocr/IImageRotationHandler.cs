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
using iText.IO.Image;

namespace iText.Pdfocr {
    /// <summary>Rotation information may be stored in image metadata.</summary>
    /// <remarks>
    /// Rotation information may be stored in image metadata.
    /// For OCR and adding image to document that rotation
    /// should be applied to the image, so that it is actually rotated,
    /// not via metadata properties.
    /// Interface ia responsible for extracting rotation from metadata
    /// and applying in to the image.
    /// </remarks>
    public interface IImageRotationHandler {
        /// <summary>Apply rotation to image data.</summary>
        /// <remarks>
        /// Apply rotation to image data.
        /// If image is not rotated - does nothing.
        /// </remarks>
        /// <param name="imageData">to apply rotation to</param>
        /// <returns>rotated image if rotation flag is set or self if no rotation</returns>
        ImageData ApplyRotation(ImageData imageData);
    }
}
