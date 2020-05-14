using System;
using System.Collections.Generic;
using System.IO;
using Common.Logging;
using Tesseract;
using iText.IO.Util;

namespace iText.Ocr {
    /// <summary>
    /// The implementation of
    /// <see cref="TesseractReader"/>
    /// for tesseract OCR.
    /// </summary>
    /// <remarks>
    /// The implementation of
    /// <see cref="TesseractReader"/>
    /// for tesseract OCR.
    /// This class provides possibilities to use features of "tesseract"
    /// using tess4j.
    /// </remarks>
    public class TesseractLibReader : TesseractReader {
        /// <summary>
        /// <see cref="Tesseract.TesseractEngine"/>
        /// Instance.
        /// </summary>
        /// <remarks>
        /// <see cref="Tesseract.TesseractEngine"/>
        /// Instance.
        /// (depends on OS type)
        /// </remarks>
        private TesseractEngine tesseractInstance = null;

        /// <summary>
        /// Creates a new
        /// <see cref="TesseractLibReader"/>
        /// instance.
        /// </summary>
        /// <param name="tessDataPath">path to tess data directory</param>
        public TesseractLibReader(String tessDataPath) {
            SetOsType(IdentifyOsType());
            SetPathToTessData(tessDataPath);
            TesseractUtil.InitializeTesseractInstance(IsWindows());
        }

        /// <summary>
        /// Creates a new
        /// <see cref="TesseractLibReader"/>
        /// instance.
        /// </summary>
        /// <param name="languagesList">list of required languages</param>
        /// <param name="tessDataPath">path to tess data directory</param>
        public TesseractLibReader(String tessDataPath, IList<String> languagesList) {
            SetOsType(IdentifyOsType());
            SetPathToTessData(tessDataPath);
            SetLanguages(JavaCollectionsUtil.UnmodifiableList<String>(languagesList));
            TesseractUtil.InitializeTesseractInstance(IsWindows());
        }

        /// <summary>Gets tesseract instance depending on the OS type.</summary>
        /// <remarks>
        /// Gets tesseract instance depending on the OS type.
        /// If instance is null or it was already disposed, it will be initialized
        /// with parameters.
        /// </remarks>
        /// <returns>
        /// initialized
        /// <see cref="Tesseract.TesseractEngine"/>
        /// instance
        /// </returns>
        public virtual TesseractEngine GetTesseractInstance() {
            if (tesseractInstance == null || TesseractUtil.IsTesseractInstanceDisposed(tesseractInstance)) {
                tesseractInstance = TesseractUtil.InitializeTesseractInstance(GetTessData(), GetLanguagesAsString(), IsWindows
                    (), GetUserWordsFilePath());
            }
            return tesseractInstance;
        }

        /// <summary>
        /// Initializes instance of tesseract if it haven't been already
        /// initialized or it have been disposed and sets all the required
        /// properties.
        /// </summary>
        /// <param name="outputFormat">
        /// selected
        /// <see cref="OutputFormat"/>
        /// for
        /// tesseract
        /// </param>
        public virtual void InitializeTesseract(IOcrReader.OutputFormat outputFormat) {
            GetTesseractInstance().SetVariable("tessedit_create_hocr", outputFormat.Equals(IOcrReader.OutputFormat.HOCR
                ) ? "1" : "0");
            GetTesseractInstance().SetVariable("user_defined_dpi", "300");
            if (GetUserWordsFilePath() != null) {
                GetTesseractInstance().SetVariable("load_system_dawg", "0");
                GetTesseractInstance().SetVariable("load_freq_dawg", "0");
                GetTesseractInstance().SetVariable("user_words_suffix", DEFAULT_USER_WORDS_SUFFIX);
                GetTesseractInstance().SetVariable("user_words_file", GetUserWordsFilePath());
            }
            TesseractUtil.SetTesseractProperties(GetTesseractInstance(), GetTessData(), GetLanguagesAsString(), GetPageSegMode
                (), GetUserWordsFilePath());
        }

        /// <summary>
        /// Performs tesseract OCR using command line tool for the selected page
        /// of input image (by default 1st).
        /// </summary>
        /// <param name="inputImage">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="outputFiles">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of output files
        /// (one per each page)
        /// </param>
        /// <param name="outputFormat">
        /// selected
        /// <see cref="OutputFormat"/>
        /// for
        /// tesseract
        /// </param>
        /// <param name="pageNumber">number of page to be processed</param>
        public override void DoTesseractOcr(FileInfo inputImage, IList<FileInfo> outputFiles, IOcrReader.OutputFormat
             outputFormat, int pageNumber) {
            try {
                ValidateLanguages(GetLanguagesAsList());
                InitializeTesseract(outputFormat);
                // if preprocessing is not needed and provided image is tiff,
                // the image will be paginated and separate pages will be OCRed
                IList<String> resultList = new List<String>();
                if (!IsPreprocessingImages() && ImageUtil.IsTiffImage(inputImage)) {
                    resultList = GetOcrResultForMultiPage(inputImage, outputFormat);
                }
                else {
                    resultList.Add(GetOcrResultForSinglePage(inputImage, outputFormat, pageNumber));
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
                            String msg = MessageFormatUtil.Format(LogMessageConstant.TesseractFailed, "Cannot write to file: " + e.Message
                                );
                            LogManager.GetLogger(GetType()).Error(msg);
                            throw new OcrException(OcrException.TesseractFailed);
                        }
                    }
                }
            }
            catch (OcrException e) {
                LogManager.GetLogger(GetType()).Error(e.Message);
                throw new OcrException(e.Message, e);
            }
            finally {
                if (tesseractInstance != null) {
                    TesseractUtil.DisposeTesseractInstance(tesseractInstance);
                }
                if (GetUserWordsFilePath() != null) {
                    UtilService.DeleteFile(GetUserWordsFilePath());
                }
            }
        }

        /// <summary>
        /// Gets OCR result from provided multi-page image and returns result as
        /// list of strings for each page.
        /// </summary>
        /// <remarks>
        /// Gets OCR result from provided multi-page image and returns result as
        /// list of strings for each page. This method is used for tiff images
        /// when preprocessing is not needed.
        /// </remarks>
        /// <param name="inputImage">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="outputFormat">
        /// selected
        /// <see cref="OutputFormat"/>
        /// for
        /// tesseract
        /// </param>
        /// <returns>
        /// list of result string that will be written to a temporary files
        /// later
        /// </returns>
        private IList<String> GetOcrResultForMultiPage(FileInfo inputImage, IOcrReader.OutputFormat outputFormat) {
            IList<String> resultList = new List<String>();
            try {
                TesseractUtil util = new TesseractUtil();
                util.InitializeImagesListFromTiff(inputImage);
                int numOfPages = util.GetListOfPages().Count;
                for (int i = 0; i < numOfPages; i++) {
                    try {
                        InitializeTesseract(outputFormat);
                        String result = util.GetOcrResultAsString(GetTesseractInstance(), util.GetListOfPages()[i], outputFormat);
                        resultList.Add(result);
                    }
                    finally {
                        TesseractUtil.DisposeTesseractInstance(GetTesseractInstance());
                    }
                }
            }
            catch (TesseractException e) {
                String msg = MessageFormatUtil.Format(LogMessageConstant.TesseractFailed, e.Message);
                LogManager.GetLogger(GetType()).Error(msg);
                throw new OcrException(OcrException.TesseractFailed);
            }
            return resultList;
        }

        /// <summary>
        /// Gets OCR result from provided single page image and preprocesses it if
        /// it is needed.
        /// </summary>
        /// <param name="inputImage">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="outputFormat">
        /// selected
        /// <see cref="OutputFormat"/>
        /// for
        /// tesseract
        /// </param>
        /// <param name="pageNumber">number of page to be OCRed</param>
        /// <returns>result as string that will be written to a temporary file later</returns>
        private String GetOcrResultForSinglePage(FileInfo inputImage, IOcrReader.OutputFormat outputFormat, int pageNumber
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
                            LogManager.GetLogger(GetType()).Info(MessageFormatUtil.Format(LogMessageConstant.CannotCreateBufferedImage
                                , ex.Message));
                            bufferedImage = ImageUtil.ReadAsPixAndConvertToBufferedImage(inputImage);
                        }
                    }
                    catch (System.IO.IOException ex) {
                        LogManager.GetLogger(GetType()).Info(MessageFormatUtil.Format(LogMessageConstant.CannotReadInputImage, ex.
                            Message));
                    }
                    if (bufferedImage != null) {
                        try {
                            result = new TesseractUtil().GetOcrResultAsString(GetTesseractInstance(), bufferedImage, outputFormat);
                        }
                        catch (TesseractException e) {
                            LogManager.GetLogger(GetType()).Info(MessageFormatUtil.Format(LogMessageConstant.CannotProcessImage, e.Message
                                ));
                        }
                    }
                    if (result == null) {
                        result = new TesseractUtil().GetOcrResultAsString(GetTesseractInstance(), inputImage, outputFormat);
                    }
                }
                else {
                    result = new TesseractUtil().GetOcrResultAsString(GetTesseractInstance(), preprocessed, outputFormat);
                }
            }
            catch (TesseractException e) {
                LogManager.GetLogger(GetType()).Error(MessageFormatUtil.Format(LogMessageConstant.TesseractFailed, e.Message
                    ));
                throw new OcrException(OcrException.TesseractFailed);
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
