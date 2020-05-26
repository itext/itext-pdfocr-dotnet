using System.IO;
using iText.IO.Image;
using iText.IO.Source;

namespace iText.Pdfocr {
    /// <summary>Utilities class to work with images.</summary>
    /// <remarks>
    /// Utilities class to work with images.
    /// Class provides tools for basic image preprocessing.
    /// </remarks>
    internal class ImageUtil {
        /// <summary>
        /// Creates a new
        /// <see cref="ImageUtil"/>
        /// instance.
        /// </summary>
        private ImageUtil() {
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
    }
}
