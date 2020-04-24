using System;
using System.IO;
using Tesseract;
using iText.IO.Image;
using iText.IO.Source;

namespace iText.Ocr {
    /// <summary>Image Util class.</summary>
    /// <remarks>
    /// Image Util class.
    /// <para />
    /// Class provides tool for basic image preprocessing.
    /// </remarks>
    public sealed class ImageUtil {
        /// <summary>Private constructor for util class.</summary>
        private ImageUtil() {
        }

        /// <summary>Performs basic image preprocessing using buffered image (if provided).</summary>
        /// <remarks>
        /// Performs basic image preprocessing using buffered image (if provided).
        /// Preprocessed image file will be saved in temporary directory
        /// (warning will be logged if file isn't deleted)
        /// </remarks>
        /// <param name="inputFile">File</param>
        /// <param name="pageNumber">int</param>
        /// <returns>String</returns>
        public static String PreprocessImage(FileInfo inputFile, int pageNumber) {
            Pix pix = null;
            // read image
            if (IsTiffImage(inputFile)) {
                pix = TesseractUtil.ReadPixPageFromTiff(inputFile, pageNumber - 1);
            }
            else {
                pix = TesseractUtil.ReadPix(inputFile);
            }
            if (pix == null) {
                throw new OCRException(OCRException.CANNOT_READ_SPECIFIED_INPUT_IMAGE).SetMessageParams(inputFile.FullName
                    );
            }
            return TesseractUtil.PreprocessPixAndSave(pix);
        }

        /// <summary>
        /// Return true if provided image has 'tiff'
        /// or 'tif' extension, otherwise - false.
        /// </summary>
        /// <param name="inputImage">File</param>
        /// <returns>boolean</returns>
        public static bool IsTiffImage(FileInfo inputImage) {
            int index = inputImage.FullName.LastIndexOf('.');
            if (index > 0) {
                String extension = new String(inputImage.FullName.ToCharArray(), index + 1, inputImage.FullName.Length - index
                     - 1);
                return extension.ToLowerInvariant().Contains("tif");
            }
            return false;
        }

        /// <summary>Count number of page in provided tiff image.</summary>
        /// <param name="inputImage">File</param>
        /// <returns>int</returns>
        public static int GetNumberOfPageTiff(FileInfo inputImage) {
            RandomAccessFileOrArray raf = new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateBestSource
                (inputImage.FullName));
            int numOfPages = TiffImageData.GetNumberOfPages(raf);
            raf.Close();
            return numOfPages;
        }

        /// <summary>Read image file from input stream from using provided file.</summary>
        /// <param name="inputFile">File</param>
        /// <returns>BufferedImage</returns>
        public static System.Drawing.Bitmap ReadImageFromFile(FileInfo inputFile) {
            return (System.Drawing.Bitmap)System.Drawing.Image.FromStream(new FileStream(inputFile.FullName, FileMode.Open
                , FileAccess.Read));
        }

        /// <summary>Reading file a Leptonica pix and converting it to image.</summary>
        /// <param name="inputImage">File</param>
        /// <returns>BufferedImage</returns>
        public static System.Drawing.Bitmap ReadAsPixAndConvertToBufferedImage(FileInfo inputImage) {
            Pix pix = Tesseract.Pix.LoadFromFile(inputImage.FullName);
            return TesseractUtil.ConvertPixToImage(pix);
        }
    }
}
