using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Common.Logging;
using iText.IO.Util;
using Tesseract;

namespace iText.Ocr
{
    /// <summary>Tesseract Utils class.</summary>
    /// <remarks>
    /// Tesseract Utils class.
    /// Here are listed all methods that have to be ported to .Net manually.
    /// </remarks>
    public class TesseractUtil
    {
        /// <summary>Path to the file with the default font.</summary>
        public const String FONT_RESOURCE_PATH = "iText.Ocr.font.";

        /// <summary>Utils logger.</summary>
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(TesseractUtil));

        /// <summary>List of page of processing image.</summary>
        private static IList<Pix> imagePages = new List<Pix>();

        /// <summary>Retrieve list of pages from provided image.</summary>
        /// <param name="inputFile">File</param>
        public static void InitializeImagesListFromTiff(FileInfo inputFile)
        {
            try
            {
                IList<Pix> pages = new List<Pix>();
                PixArray pixa = PixArray.LoadMultiPageTiffFromFile(inputFile.FullName);                
                for (int i = 0; i < pixa.Count; i++)
                {
                    pages.Add(pixa.GetPix(i));
                }
                imagePages = pages;
                DestroyPixa(pixa);
            }
            catch (Exception e)
            {
                LOGGER.Error("Cannot get pages from image: " + e.Message);
            }
        }

        /// <summary>Get list of page of processing image.</summary>
        /// <returns>List<bufferedimage></returns>
        public static IList<Pix> GetListOfPages()
        {
            return new List<Pix>(imagePages);
        }

        /// <summary>Run given command in command line.</summary>
        /// <param name="command">List<string></param>
        /// <param name="isWindows">boolean</param>
        public static void RunCommand(IList<String> command, bool isWindows)
        {
            Process process = null;
            try
            {
                LOGGER.Info("Running command: " + String.Join(" ", command));
                if (isWindows)
                {
                    String cmd = "";
                    for (int i = 1; i < command.Count; ++i)
                    {
                        cmd += command[i] + " ";
                    }

                    process = new Process();
                    process.StartInfo.FileName = command[0];
                    process.StartInfo.Arguments = cmd;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.Start();

                    //* Read the output (or the error)
                    string output = process.StandardOutput.ReadToEnd();
                    Console.WriteLine(output);
                    string err = process.StandardError.ReadToEnd();
                    Console.WriteLine(err);
                }
                else
                {
                    String cmd_str = "bash" + "-c" + String.Join(" ", command);
                    ProcessStartInfo pb = new ProcessStartInfo(cmd_str);
                    //NOSONAR
                    process = System.Diagnostics.Process.Start(pb);
                }
                bool cmdSucceeded = process.WaitForExit(3 * 60 * 60 * 1000);
                Console.WriteLine("cmdSucceeded " + cmdSucceeded);
                if (!cmdSucceeded)
                {
                    LOGGER.Error("Error occurred during running command: " + String.Join(" ", command));
                    throw new OCRException(OCRException.TESSERACT_FAILED);
                }
            }
            catch (Exception e)
            {
                throw new OCRException(OCRException.TESSERACT_FAILED);
                LOGGER.Error("Error occurred:" + e.Message);
            }
        }

        /// <summary>Reads required page from provided tiff image.</summary>
        /// <param name="inputFile">File</param>
        /// <param name="pageNumber">int</param>
        /// <returns>Pix</returns>
        public static Pix ReadPixPageFromTiff(FileInfo inputFile, int pageNumber)
        {
            // read image
            PixArray pixa = PixArray.LoadMultiPageTiffFromFile(inputFile.FullName);
            int size = pixa.Count;
            // in case page number is incorrect
            if (pageNumber >= size)
            {
                LOGGER.Warn("Provided number of page (" + pageNumber + ") is incorrect for " + inputFile.FullName);
                return null;
            }
            Pix pix = pixa.GetPix(pageNumber);
            DestroyPixa(pixa);
            // return required page to be preprocessed
            return pix;
        }

        /// <summary>
        /// Performs default image preprocessing
        /// and saves result to temporary file.
        /// </summary>
        /// <param name="pix">Pix</param>
        /// <returns>String Path to create preprocesed image</returns>
        public static String PreprocessPixAndSave(Pix pix)
        {
            // preprocess image
            pix = PreprocessPix(pix);
            // save preprocessed file
            String tmpFileName = UtilService.GetTempDir() + System.Guid.NewGuid().ToString() + ".png";
            pix.Save(tmpFileName, ImageFormat.Png);
            DestroyPix(pix);
            return tmpFileName;
        }

        /// <summary>Performs default image preprocessing.</summary>
        /// <remarks>
        /// Performs default image preprocessing.
        /// It includes the following actions:
        /// - remove alpha channel
        /// - convert to grayscale
        /// - thresholding
        /// - basic deskewing
        /// </remarks>
        /// <param name="pix">Pix</param>
        /// <returns>Pix</returns>
        public static Pix PreprocessPix(Pix pix)
        {
            pix = ConvertToGrayscale(pix);
            pix = OtsuImageThresholding(pix);
            return pix;
        }

        /// <summary>Convert Leptonica <c>Pix</c> to grayscale.</summary>
        /// <param name="pix">source pix</param>
        /// <returns>Pix output pix</returns>
        public static Pix ConvertToGrayscale(Pix pix)
        {
            // Leptonica instance = Leptonica.INSTANCE;
            if (pix != null)
            {
                int depth = pix.Depth;
                if (depth == 32)
                {
                    return pix.ConvertRGBToGray();
                }
                else
                {
                    LOGGER.Warn("Cannot convert to gray image with depth " + depth);
                    return pix;
                }
            }
            else
            {
                return pix;
            }
        }

        /// <summary>Perform Leptonica Otsu adaptive image thresholding.</summary>
        /// <param name="pix">source pix</param>
        /// <returns>Pix output pix</returns>
        public static Pix OtsuImageThresholding(Pix pix)
        {
            if (pix != null)
            {
                // pix = pix.BinarizeOtsuAdaptiveThreshold();
                //PointerByReference pointer = new PointerByReference();
                //Leptonica.INSTANCE.PixOtsuAdaptiveThreshold(pix, pix.w, pix.h, 0, 0, 0, null, pointer);
                Pix thresholdPix = null;
                if (pix.Depth == 8)
                {
                    thresholdPix = pix.BinarizeOtsuAdaptiveThreshold(pix.Width, pix.Height, 0, 0, 0);
                    if (thresholdPix != null && thresholdPix.Width > 0 && thresholdPix.Height > 0)
                    {
                        return thresholdPix;
                    }
                    else
                    {
                        return pix;
                    }
                }
                else
                {
                    LOGGER.Warn("Cannot binarize image with depth " + pix.Depth);
                    return pix;
                }
            }
            else
            {
                return pix;
            }
        }

        /// <summary>Get png image format in required format</summary>
        /// <returns>String</returns>
        public static System.Drawing.Imaging.ImageFormat GetPngImageFormat()
        {
            return System.Drawing.Imaging.ImageFormat.Png;
        }

        /// <summary>Setting tesseract properties.</summary>
        /// <remarks>
        /// Setting tesseract properties.
        /// In java: path to tess data, languages, psm
        /// In .Net: psm
        /// </remarks>
        /// <param name="tesseractInstance">ITesseract</param>
        /// <param name="tessData">String</param>
        /// <param name="languages">String</param>
        /// <param name="pageSegMode">Integer</param>
        /// <param name="userWordsFilePath">String</param>
        public static void SetTesseractProperties(TesseractEngine tesseractInstance, String tessData, String languages
            , int? pageSegMode, String userWordsFilePath)
        {
            if (pageSegMode != null)
            {
                tesseractInstance.DefaultPageSegMode = (PageSegMode)pageSegMode;
            }
        }

        /// <summary>Create teseract instance without parameters.</summary>
        /// <param name="isWindows">boolean</param>
        /// <returns>ITesseract</returns>
        public static TesseractEngine CreateTesseractInstance(bool isWindows)
        {
            return null;
        }

        /// <summary>Create teseract instance with parameters.</summary>
        /// <param name="tessData">String</param>
        /// <param name="languages">String</param>
        /// <param name="isWindows">boolean</param>
        /// <param name="userWordsFilePath">String</param>
        /// <returns>ITesseract</returns>
        public static TesseractEngine InitializeTesseractInstanceWithParameters(String tessData, String languages,
            bool isWindows, String userWordsFilePath)
        {
            return new TesseractEngine(tessData, languages,
                userWordsFilePath != null ? EngineMode.TesseractOnly : EngineMode.Default);
        }

        /// <summary>
        /// Perform ocr for the provided image
        /// and return result as string in required format.
        /// </summary>
        /// <param name="tesseractInstance">ITesseract</param>
        /// <param name="image">BufferedImage</param>
        /// <param name="outputFormat">OutputFormat</param>
        /// <returns>String</returns>
        public static String GetOcrResultAsString(TesseractEngine tesseractInstance, System.Drawing.Bitmap image,
            IOcrReader.OutputFormat outputFormat)
        {
            Page page = tesseractInstance.Process(image);
            if (outputFormat.Equals(IOcrReader.OutputFormat.hocr))
            {
                return page.GetHOCRText(0);
            }
            else
            {
                return page.GetText();
            }
        }

        /// <summary>
        /// Perform ocr for the provided image file
        /// and return result as string in required format.
        /// </summary>
        /// <param name="tesseractInstance">ITesseract</param>
        /// <param name="image">File</param>
        /// <param name="outputFormat">OutputFormat</param>
        /// <returns>String</returns>
        public static String GetOcrResultAsString(TesseractEngine tesseractInstance, FileInfo image, IOcrReader.OutputFormat
             outputFormat)
        {
            Pix pix = Pix.LoadFromFile(image.FullName);
            return GetOcrResultAsString(tesseractInstance, pix, outputFormat);
        }

        /// <summary>
        /// Perform ocr for the provided Pix object
        /// and return result as string in required format.
        /// </summary>
        /// <param name="tesseractInstance">ITesseract</param>
        /// <param name="pix">Pix</param>
        /// <param name="outputFormat">OutputFormat</param>
        /// <returns>String</returns>
        public static String GetOcrResultAsString(TesseractEngine tesseractInstance, Pix pix, IOcrReader.OutputFormat
             outputFormat)
        {
            Page page = tesseractInstance.Process(pix);
            String result = null;
            if (outputFormat.Equals(IOcrReader.OutputFormat.hocr))
            {
                result = page.GetHOCRText(0);
            }
            else
            {
                result = page.GetText();
            }
            DestroyPix(pix);
            return result;
        }

        /// <summary>Return true if tesseract instance is disposed.</summary>
        /// <remarks>
        /// Return true if tesseract instance is disposed.
        /// (used in .net version)
        /// </remarks>
        /// <param name="tesseractInstance">ITesseract</param>
        /// <returns>boolean</returns>
        public static bool IsTesseractInstanceDisposed(TesseractEngine tesseractInstance)
        {
            return tesseractInstance.IsDisposed;
        }

        /// <summary>Dispose Tesseract instance.</summary>
        /// <remarks>
        /// Dispose Tesseract instance.
        /// (used in .net version)
        /// </remarks>
        /// <param name="tesseractInstance">ITesseract</param>
        public static void DisposeTesseractInstance(TesseractEngine tesseractInstance)
        {
            tesseractInstance.Dispose();
        }

        /// <summary>Destroy pix object.</summary>
        /// <param name="pix">Pix</param>
        public static void DestroyPix(Pix pix)
        {
            pix.Dispose();
        }

        /// <summary>Destroy pixa object.</summary>
        /// <param name="pixa">Pixa</param>
        public static void DestroyPixa(PixArray pixa)
        {
            pixa.Dispose();
        }

        /// <summary>Return vaue by provided key or empty list if key doesn't exist.</summary>
        /// <param name="data">Map<Integer, List&lt;textinfo>&gt;</param>
        /// <param name="key">key</param>
        /// <returns>List<textinfo></returns>
        public static IList<TextInfo> GetValueByKey(IDictionary<int, IList<TextInfo>> data, int key)
        {
            IList<TextInfo> value = new List<TextInfo>();
            data.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// Helper to perform ocr for the provided Pix object.
        /// </summary>
        /// <param name="tesseractInstance">ITesseract</param>
        /// <param name="pix">Pix</param>
        /// <param name="outputFormat">OutputFormat</param>
        /// <returns>String</returns>
        private static String GetValueFromPix(TesseractEngine tesseractInstance,
            Pix pix, IOcrReader.OutputFormat outputFormat)
        {
            Page page = tesseractInstance.Process(pix);
            if (outputFormat.Equals(IOcrReader.OutputFormat.hocr))
            {
                return page.GetHOCRText(0);
            }
            else
            {
                return page.GetText();
            }
        }

        /// <summary>Converts <c>BufferedImage</c> to Leptonica <c>Pix</c>.</summary>
        /// <param name="bufferedImage">Bitmap</param>
        /// <returns>Pix</returns>
        public static Pix ConvertImageToPix(System.Drawing.Bitmap bufferedImage)
        {
            return PixConverter.ToPix(bufferedImage);
        }

        /// <summary>Converts Leptonica Pix to image.</summary>
        /// <param name="pix">source pix</param>
        /// <returns>Bitmap output image</returns>
        public static System.Drawing.Bitmap ConvertPixToImage(Pix pix)
        {
            return PixConverter.ToBitmap(pix);
        }
    }
}
