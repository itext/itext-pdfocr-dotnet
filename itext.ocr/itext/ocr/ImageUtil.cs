using System;
using System.IO;
using Common.Logging;
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
        /// <summary>ImageUtil logger.</summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Ocr.ImageUtil));

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
                pix = ReadPix(inputFile);
            }
            if (pix == null) {
                throw new OCRException(OCRException.CANNOT_READ_INPUT_IMAGE);
            }
            return TesseractUtil.PreprocessPixAndSave(pix);
        }

        /// <summary>Read Pix from file or convert from buffered image.</summary>
        /// <param name="inputFile">File</param>
        /// <returns>Pix</returns>
        public static Pix ReadPix(FileInfo inputFile) {
            Pix pix = null;
            try {
                System.Drawing.Bitmap bufferedImage = ReadImageFromFile(inputFile);
                if (bufferedImage != null) {
                    pix = TesseractUtil.ConvertImageToPix(bufferedImage);
                }
                else {
                    pix = Tesseract.Pix.LoadFromFile(inputFile.FullName);
                }
            }
            catch (Exception e) {
                LOGGER.Warn("Reading pix from file: " + e.Message);
                pix = Tesseract.Pix.LoadFromFile(inputFile.FullName);
            }
            return pix;
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
