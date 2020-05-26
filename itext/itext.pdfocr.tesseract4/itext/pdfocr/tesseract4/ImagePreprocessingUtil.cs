using System;
using System.IO;
using Tesseract;
using iText.IO.Image;
using iText.IO.Source;

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
                pix = TesseractOcrUtil.ReadPix(inputFile);
            }
            if (pix == null) {
                throw new Tesseract4OcrException(Tesseract4OcrException.CannotReadProvidedImage).SetMessageParams(inputFile
                    .FullName);
            }
            return TesseractOcrUtil.PreprocessPixAndSave(pix);
        }
    }
}
