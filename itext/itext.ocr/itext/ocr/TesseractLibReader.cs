using System;
using System.Collections.Generic;
using System.IO;
using Common.Logging;
using Tesseract;
using iText.IO.Util;

namespace iText.Ocr {
    /// <summary>Tesseract Library Reader class.</summary>
    /// <remarks>
    /// Tesseract Library Reader class.
    /// (extends TesseractReader class)
    /// <para />
    /// This class provides possibilities to use features of "tesseract"
    /// (optical character recognition engine for various operating systems)
    /// <para />
    /// This class provides possibility to perform OCR, read data from input files
    /// and return contained text in the described format
    /// <para />
    /// This class provides possibilities to set type of current os,
    /// required languages for OCR for input images,
    /// set path to directory with tess data.
    /// </remarks>
    public class TesseractLibReader : TesseractReader {
        /// <summary>Tesseract Instance.</summary>
        /// <remarks>
        /// Tesseract Instance.
        /// (depends on OS type)
        /// </remarks>
        private TesseractEngine tesseractInstance = null;

        /// <summary>TesseractLibReader constructor with path to tess data directory.</summary>
        /// <param name="tessDataPath">String</param>
        public TesseractLibReader(String tessDataPath) {
            SetOsType(IdentifyOSType());
            SetTesseractInstance();
            SetPathToTessData(tessDataPath);
        }

        /// <summary>
        /// TesseractLibReader constructor with path to tess data directory,
        /// list of languages and path to tess data directory.
        /// </summary>
        /// <param name="languagesList">List<string></param>
        /// <param name="tessDataPath">String</param>
        public TesseractLibReader(String tessDataPath, IList<String> languagesList) {
            SetOsType(IdentifyOSType());
            SetTesseractInstance();
            SetPathToTessData(tessDataPath);
            SetLanguages(JavaCollectionsUtil.UnmodifiableList<String>(languagesList));
        }

        /// <summary>Set tesseract instance depending on the OS type.</summary>
        public virtual void SetTesseractInstance() {
            tesseractInstance = TesseractUtil.CreateTesseractInstance(IsWindows());
        }

        /// <summary>Get tesseract instance depending on the OS type.</summary>
        /// <remarks>
        /// Get tesseract instance depending on the OS type.
        /// If instance is null, it will be initialized with parameters
        /// </remarks>
        /// <returns>ITesseract</returns>
        public virtual TesseractEngine GetTesseractInstance() {
            if (tesseractInstance == null || TesseractUtil.IsTesseractInstanceDisposed(tesseractInstance)) {
                tesseractInstance = TesseractUtil.InitializeTesseractInstanceWithParameters(GetTessData(), GetLanguagesAsString
                    (), IsWindows(), GetUserWordsFilePath());
            }
            return tesseractInstance;
        }

        /// <summary>Initialize instance of tesseract and set all the required properties.</summary>
        /// <param name="outputFormat">OutputFormat</param>
        public virtual void InitializeTesseract(IOcrReader.OutputFormat outputFormat) {
            SetTesseractInstance();
            GetTesseractInstance().SetVariable("tessedit_create_hocr", outputFormat.Equals(IOcrReader.OutputFormat.HOCR
                ) ? "1" : "0");
            if (GetUserWordsFilePath() != null) {
                GetTesseractInstance().SetVariable("load_system_dawg", "0");
                GetTesseractInstance().SetVariable("load_freq_dawg", "0");
                GetTesseractInstance().SetVariable("user_words_suffix", DEFAULT_USER_WORDS_SUFFIX);
                GetTesseractInstance().SetVariable("user_words_file", GetUserWordsFilePath());
            }
            TesseractUtil.SetTesseractProperties(GetTesseractInstance(), GetTessData(), GetLanguagesAsString(), GetPageSegMode
                (), GetUserWordsFilePath());
        }

        /// <summary>Perform tesseract OCR.</summary>
        /// <param name="inputImage">- input image file</param>
        /// <param name="outputFiles">- list of output file (one for each page)</param>
        /// <param name="outputFormat">- output format</param>
        /// <param name="pageNumber">- int</param>
        public override void DoTesseractOcr(FileInfo inputImage, IList<FileInfo> outputFiles, IOcrReader.OutputFormat
             outputFormat, int pageNumber) {
            try {
                ValidateLanguages(GetLanguagesAsList());
                InitializeTesseract(outputFormat);
                // if proprocessing is not needed and provided image is tiff,
                // the image will be paginated and separate pages will be OCRed
                IList<String> resultList = new List<String>();
                if (!IsPreprocessingImages() && ImageUtil.IsTiffImage(inputImage)) {
                    resultList = GetOCRResultForMultiPage(inputImage, outputFormat);
                }
                else {
                    resultList.Add(GetOCRResultForSinglePage(inputImage, outputFormat, pageNumber));
                }
                // list of result strings is written to separate files
                // (one for each page)
                for (int i = 0; i < resultList.Count; i++) {
                    String result = resultList[i];
                    FileInfo outputFile = i >= outputFiles.Count ? null : outputFiles[i];
                    if (result != null && outputFile != null) {
                        try {
                            using (TextWriter writer = new StreamWriter(new FileStream(outputFile.FullName, FileMode.Create), System.Text.Encoding
                                .UTF8)) {
                                writer.Write(result);
                            }
                        }
                        catch (System.IO.IOException e) {
                            String msg = MessageFormatUtil.Format(LogMessageConstant.TESSERACT_FAILED, "Cannot write to file: " + e.Message
                                );
                            LogManager.GetLogger(GetType()).Error(msg);
                            throw new OCRException(OCRException.TESSERACT_FAILED);
                        }
                    }
                    else {
                        LogManager.GetLogger(GetType()).Info("OCR result is NULL for " + inputImage.FullName);
                    }
                }
                TesseractUtil.DisposeTesseractInstance(GetTesseractInstance());
            }
            catch (OCRException e) {
                LogManager.GetLogger(GetType()).Error(e.Message);
                throw new OCRException(e.Message, e);
            }
            finally {
                if (GetUserWordsFilePath() != null) {
                    UtilService.DeleteFile(GetUserWordsFilePath());
                }
            }
        }

        /// <summary>
        /// Get ocr result from provided multipage image and
        /// and return result ad list fo strings for each page.
        /// </summary>
        /// <remarks>
        /// Get ocr result from provided multipage image and
        /// and return result ad list fo strings for each page.
        /// (this method is used when preprocessing is not needed)
        /// </remarks>
        /// <param name="inputImage">File</param>
        /// <param name="outputFormat">OutputFormat</param>
        /// <returns>List<string></returns>
        private IList<String> GetOCRResultForMultiPage(FileInfo inputImage, IOcrReader.OutputFormat outputFormat) {
            IList<String> resultList = new List<String>();
            try {
                TesseractUtil.InitializeImagesListFromTiff(inputImage);
                int numOfPages = TesseractUtil.GetListOfPages().Count;
                for (int i = 0; i < numOfPages; i++) {
                    InitializeTesseract(outputFormat);
                    String result = TesseractUtil.GetOcrResultAsString(GetTesseractInstance(), TesseractUtil.GetListOfPages()[
                        i], outputFormat);
                    resultList.Add(result);
                    TesseractUtil.DisposeTesseractInstance(GetTesseractInstance());
                }
            }
            catch (TesseractException e) {
                String msg = String.Format(LogMessageConstant.TESSERACT_FAILED, e.Message);
                LogManager.GetLogger(GetType()).Error(msg);
                throw new OCRException(OCRException.TESSERACT_FAILED);
            }
            return resultList;
        }

        /// <summary>
        /// Get ocr result from provided single page image
        /// and preprocess it if needed.
        /// </summary>
        /// <param name="inputImage">File</param>
        /// <param name="outputFormat">OutputFormat</param>
        /// <param name="pageNumber">int</param>
        /// <returns>String</returns>
        private String GetOCRResultForSinglePage(FileInfo inputImage, IOcrReader.OutputFormat outputFormat, int pageNumber
            ) {
            String result = null;
            FileInfo preprocessed = null;
            try {
                // preprocess if required
                if (IsPreprocessingImages()) {
                    preprocessed = new FileInfo(ImageUtil.PreprocessImage(inputImage, pageNumber));
                }
                if (!IsPreprocessingImages() || preprocessed == null) {
                    // try to open as buffered image if it's not a tiff image
                    System.Drawing.Bitmap bufferedImage = null;
                    try {
                        try {
                            bufferedImage = ImageUtil.ReadImageFromFile(inputImage);
                        }
                        catch (Exception ex) {
                            LogManager.GetLogger(GetType()).Info("Cannot create a buffered image " + "from the input image: " + ex.Message
                                );
                            bufferedImage = ImageUtil.ReadAsPixAndConvertToBufferedImage(inputImage);
                        }
                    }
                    catch (System.IO.IOException ex) {
                        LogManager.GetLogger(GetType()).Info("Cannot read image: " + ex.Message);
                    }
                    if (bufferedImage != null) {
                        try {
                            result = TesseractUtil.GetOcrResultAsString(GetTesseractInstance(), bufferedImage, outputFormat);
                        }
                        catch (TesseractException e) {
                            LogManager.GetLogger(GetType()).Info("Cannot process image: " + e.Message);
                        }
                    }
                    if (result == null) {
                        result = TesseractUtil.GetOcrResultAsString(GetTesseractInstance(), inputImage, outputFormat);
                    }
                }
                else {
                    result = TesseractUtil.GetOcrResultAsString(GetTesseractInstance(), preprocessed, outputFormat);
                }
            }
            catch (TesseractException e) {
                LogManager.GetLogger(GetType()).Error(String.Format(LogMessageConstant.TESSERACT_FAILED, e.Message));
                throw new OCRException(OCRException.TESSERACT_FAILED);
            }
            finally {
                if (preprocessed != null) {
                    UtilService.DeleteFile(preprocessed.FullName);
                }
            }
            return result;
        }
    }
}
