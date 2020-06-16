/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2020 iText Group NV
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
using System;
using System.IO;
using Common.Logging;
using Tesseract;
using iText.IO.Image;
using iText.IO.Source;
using iText.IO.Util;

namespace iText.Pdfocr.Tesseract4 {
    /// <summary>Utilities class to work with images.</summary>
    /// <remarks>
    /// Utilities class to work with images.
    /// Class provides tools for basic image preprocessing.
    /// </remarks>
    internal class ImagePreprocessingUtil {
        /// <summary>
        /// Creates a new
        /// <see cref="ImagePreprocessingUtil"/>
        /// instance.
        /// </summary>
        private ImagePreprocessingUtil() {
        }

        /// <summary>Counts number of pages in the provided tiff image.</summary>
        /// <param name="inputImage">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <returns>number of pages in the provided TIFF image</returns>
        internal static int GetNumberOfPageTiff(FileInfo inputImage) {
            RandomAccessFileOrArray raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource
                (inputImage.FullName));
            int numOfPages = TiffImageData.GetNumberOfPages(raf);
            raf.Close();
            return numOfPages;
        }

        /// <summary>Checks whether image format is TIFF.</summary>
        /// <param name="inputImage">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <returns>true if provided image has 'tiff' or 'tif' extension</returns>
        internal static bool IsTiffImage(FileInfo inputImage) {
            int index = inputImage.FullName.LastIndexOf('.');
            if (index > 0) {
                String extension = new String(inputImage.FullName.ToCharArray(), index + 1, inputImage.FullName.Length - index
                     - 1);
                return extension.ToLowerInvariant().Contains("tif");
            }
            return false;
        }

        /// <summary>Reads provided image file using stream.</summary>
        /// <param name="inputFile">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <returns>
        /// returns a
        /// <see cref="System.Drawing.Bitmap"/>
        /// as the result
        /// </returns>
        internal static System.Drawing.Bitmap ReadImageFromFile(FileInfo inputFile) {
            FileStream @is = new FileStream(inputFile.FullName, FileMode.Open, FileAccess.Read);
            System.Drawing.Bitmap bi = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(@is);
            @is.Dispose();
            return bi;
        }

        /// <summary>
        /// Reads input file as Leptonica
        /// <see cref="Tesseract.Pix"/>
        /// and
        /// converts it to
        /// <see cref="System.Drawing.Bitmap"/>.
        /// </summary>
        /// <param name="inputImage">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <returns>
        /// returns a
        /// <see cref="System.Drawing.Bitmap"/>
        /// as the result
        /// </returns>
        internal static System.Drawing.Bitmap ReadAsPixAndConvertToBufferedImage(FileInfo inputImage) {
            Pix pix = Tesseract.Pix.LoadFromFile(inputImage.FullName);
            return TesseractOcrUtil.ConvertPixToImage(pix);
        }

        /// <summary>Performs basic image preprocessing using buffered image (if provided).</summary>
        /// <remarks>
        /// Performs basic image preprocessing using buffered image (if provided).
        /// Preprocessed image will be saved in temporary directory.
        /// </remarks>
        /// <param name="inputFile">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="pageNumber">number of page to be preprocessed</param>
        /// <returns>
        /// path to the created preprocessed image file as
        /// <see cref="System.String"/>
        /// </returns>
        internal static String PreprocessImage(FileInfo inputFile, int pageNumber) {
            Pix pix = null;
            // read image
            if (IsTiffImage(inputFile)) {
                pix = TesseractOcrUtil.ReadPixPageFromTiff(inputFile, pageNumber - 1);
            }
            else {
                pix = ReadPix(inputFile);
            }
            if (pix == null) {
                throw new Tesseract4OcrException(Tesseract4OcrException.CANNOT_READ_PROVIDED_IMAGE).SetMessageParams(inputFile
                    .FullName);
            }
            return TesseractOcrUtil.PreprocessPixAndSave(pix);
        }

        /// <summary>
        /// Reads
        /// <see cref="Tesseract.Pix"/>
        /// from input file or, if
        /// this is not possible, reads input file as
        /// <see cref="System.Drawing.Bitmap"/>
        /// and then converts to
        /// <see cref="Tesseract.Pix"/>.
        /// </summary>
        /// <param name="inputFile">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <returns>
        /// Pix result
        /// <see cref="Tesseract.Pix"/>
        /// object from
        /// input file
        /// </returns>
        internal static Pix ReadPix(FileInfo inputFile) {
            Pix pix = null;
            try {
                System.Drawing.Bitmap bufferedImage = iText.Pdfocr.Tesseract4.ImagePreprocessingUtil.ReadImageFromFile(inputFile
                    );
                if (bufferedImage != null) {
                    pix = TesseractOcrUtil.ConvertImageToPix(bufferedImage);
                }
            }
            catch (Exception e) {
                // NOSONAR
                LogManager.GetLogger(typeof(iText.Pdfocr.Tesseract4.ImagePreprocessingUtil)).Info(MessageFormatUtil.Format
                    (Tesseract4LogMessageConstant.CANNOT_CONVERT_IMAGE_TO_PIX, inputFile.FullName, e.Message));
            }
            if (pix == null) {
                try {
                    pix = Tesseract.Pix.LoadFromFile(inputFile.FullName);
                }
                catch (Exception e) {
                    // NOSONAR
                    LogManager.GetLogger(typeof(iText.Pdfocr.Tesseract4.ImagePreprocessingUtil)).Info(MessageFormatUtil.Format
                        (Tesseract4LogMessageConstant.CANNOT_CONVERT_IMAGE_TO_PIX, inputFile.FullName, e.Message));
                }
            }
            return pix;
        }

        /// <summary>
        /// Reads input image as a
        /// <see cref="System.Drawing.Bitmap"/>.
        /// </summary>
        /// <remarks>
        /// Reads input image as a
        /// <see cref="System.Drawing.Bitmap"/>.
        /// If it is not possible to read
        /// <see cref="System.Drawing.Bitmap"/>
        /// from
        /// input file, image will be read as a
        /// <see cref="Tesseract.Pix"/>
        /// and then converted to
        /// <see cref="System.Drawing.Bitmap"/>.
        /// </remarks>
        /// <param name="inputImage">original input image</param>
        /// <returns>
        /// input image as a
        /// <see cref="System.Drawing.Bitmap"/>
        /// </returns>
        internal static System.Drawing.Bitmap ReadImage(FileInfo inputImage) {
            System.Drawing.Bitmap bufferedImage = null;
            try {
                bufferedImage = iText.Pdfocr.Tesseract4.ImagePreprocessingUtil.ReadImageFromFile(inputImage);
            }
            catch (Exception ex) {
                LogManager.GetLogger(typeof(iText.Pdfocr.Tesseract4.ImagePreprocessingUtil)).Info(MessageFormatUtil.Format
                    (Tesseract4LogMessageConstant.CANNOT_CREATE_BUFFERED_IMAGE, ex.Message));
            }
            if (bufferedImage == null) {
                try {
                    bufferedImage = iText.Pdfocr.Tesseract4.ImagePreprocessingUtil.ReadAsPixAndConvertToBufferedImage(inputImage
                        );
                }
                catch (System.IO.IOException ex) {
                    LogManager.GetLogger(typeof(iText.Pdfocr.Tesseract4.ImagePreprocessingUtil)).Info(MessageFormatUtil.Format
                        (Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE, ex.Message));
                }
            }
            return bufferedImage;
        }
    }
}
