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
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.IO.Util;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Logs;
using System.Drawing.Imaging;
using System.Drawing;

namespace iText.Pdfocr.Util {
    /// <summary>Utility class to handle tiff images.</summary>
    public class TiffImageUtil {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Pdfocr.Util.TiffImageUtil)
            );

        private TiffImageUtil() {
        }

        // Private constructor will prevent the instantiation of this class directly.
        /// <summary>Retrieves all images from a TIFF file.</summary>
        /// <param name="inputFile">TIFF file to retrieve images from</param>
        /// <returns>
        /// the list of
        /// <see cref="System.Drawing.Bitmap"/>
        /// 's in the TIFF file
        /// </returns>
        public static IList<System.Drawing.Bitmap> GetAllImages(FileInfo inputFile) {
            try
            {
                Image originalImage = Image.FromFile(inputFile.FullName);
                List<Bitmap> bitmapList = new List<Bitmap>();
                var xResolution = originalImage.HorizontalResolution;
                var yResolution = originalImage.VerticalResolution;

                FrameDimension frameDimension = new FrameDimension(originalImage.FrameDimensionsList[0]);
                int frameCount = originalImage.GetFrameCount(FrameDimension.Page);
                for (int i = 0; i < frameCount; ++i)
                {
                    originalImage.SelectActiveFrame(frameDimension, i);
                    Bitmap temp = new Bitmap(originalImage);
                    temp.SetResolution(2 * xResolution, 2 * yResolution);
                    bitmapList.Add(temp);
                }

                return bitmapList;
            } catch (Exception e)
            {
                LOGGER.LogError(MessageFormatUtil.Format(
                    PdfOcrLogMessageConstant.CANNOT_RETRIEVE_PAGES_FROM_IMAGE, inputFile.FullName, e.Message));
            }

            return new List<System.Drawing.Bitmap>();
        }

        /// <summary>Checks whether image type is TIFF.</summary>
        /// <param name="inputImage">
        /// input
        /// <see cref="System.IO.FileInfo"/>
        /// to check for TIFF image type
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if provided image is TIFF image,
        /// <see langword="false"/>
        /// otherwise
        /// </returns>
        public static bool IsTiffImage(FileInfo inputImage) {
            return GetImageType(inputImage) == ImageType.TIFF;
        }

        /// <summary>Gets the image type.</summary>
        /// <param name="inputImage">
        /// input
        /// <see cref="System.IO.FileInfo"/>
        /// to get image type for
        /// </param>
        /// <returns>
        /// image type
        /// <see cref="iText.IO.Image.ImageType"/>
        /// </returns>
        public static ImageType GetImageType(FileInfo inputImage) {
            ImageType type;
            try {
                type = ImageTypeDetector.DetectImageType(UrlUtil.ToURL(inputImage.FullName));
            }
            catch (Exception e) {
                LOGGER.LogError(MessageFormatUtil.Format(PdfOcrLogMessageConstant.CANNOT_READ_INPUT_IMAGE, e.Message));
                throw new PdfOcrInputException(PdfOcrExceptionMessageConstant.CANNOT_READ_INPUT_IMAGE_PARAMS).SetMessageParams
                    (inputImage.FullName);
            }
            return type;
        }
    }
}
