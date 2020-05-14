using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Common.Logging;
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
    /// This class provides possibilities to use features of "tesseract" CL tool
    /// (optical character recognition engine for various operating systems).
    /// Please note that it's assumed that "tesseract" has already been
    /// installed locally.
    /// </remarks>
    public class TesseractExecutableReader : TesseractReader {
        /// <summary>Path to the tesseract executable.</summary>
        /// <remarks>
        /// Path to the tesseract executable.
        /// By default it's assumed that "tesseract" already exists in the "PATH".
        /// </remarks>
        private String pathToExecutable;

        /// <summary>
        /// Creates a new
        /// <see cref="TesseractExecutableReader"/>
        /// instance.
        /// </summary>
        /// <param name="tessDataPath">path to tess data directory</param>
        public TesseractExecutableReader(String tessDataPath) {
            SetPathToExecutable("tesseract");
            SetOsType(IdentifyOsType());
            SetPathToTessData(tessDataPath);
        }

        /// <summary>
        /// Creates a new
        /// <see cref="TesseractExecutableReader"/>
        /// instance.
        /// </summary>
        /// <param name="executablePath">path to tesseract executable</param>
        /// <param name="tessDataPath">path to tess data directory</param>
        public TesseractExecutableReader(String executablePath, String tessDataPath) {
            SetPathToExecutable(executablePath);
            SetOsType(IdentifyOsType());
            SetPathToTessData(tessDataPath);
        }

        /// <summary>
        /// Creates a new
        /// <see cref="TesseractExecutableReader"/>
        /// instance.
        /// </summary>
        /// <param name="executablePath">path to tesseract executable</param>
        /// <param name="languagesList">list of required languages</param>
        /// <param name="tessDataPath">path to tess data directory</param>
        public TesseractExecutableReader(String executablePath, String tessDataPath, IList<String> languagesList) {
            SetPathToExecutable(executablePath);
            SetLanguages(JavaCollectionsUtil.UnmodifiableList<String>(languagesList));
            SetPathToTessData(tessDataPath);
            SetOsType(IdentifyOsType());
        }

        /// <summary>Gets path to tesseract executable.</summary>
        /// <returns>path to tesseract executable</returns>
        public String GetPathToExecutable() {
            return pathToExecutable;
        }

        /// <summary>Sets path to tesseract executable.</summary>
        /// <remarks>
        /// Sets path to tesseract executable.
        /// By default it's assumed that "tesseract" already exists in the "PATH".
        /// </remarks>
        /// <param name="path">path to tesseract executable</param>
        public void SetPathToExecutable(String path) {
            pathToExecutable = path;
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
            IList<String> command = new List<String>();
            String imagePath = null;
            try {
                imagePath = inputImage.FullName;
                // path to tesseract executable
                AddPathToExecutable(command);
                // path to tess data
                AddTessData(command);
                // validate languages before preprocessing started
                ValidateLanguages(GetLanguagesAsList());
                // preprocess input file if needed and add it
                imagePath = PreprocessImage(inputImage, pageNumber);
                AddInputFile(command, imagePath);
                // output file
                AddOutputFile(command, outputFiles[0], outputFormat);
                // page segmentation mode
                AddPageSegMode(command);
                // add user words if needed
                AddUserWords(command);
                // set default user defined dpi
                AddDefaultDpi(command);
                // required languages
                AddLanguages(command);
                if (outputFormat.Equals(IOcrReader.OutputFormat.HOCR)) {
                    // path to hocr script
                    SetHocrOutput(command);
                }
                TesseractUtil.RunCommand(command, IsWindows());
            }
            catch (OcrException e) {
                LogManager.GetLogger(GetType()).Error(e.Message);
                throw new OcrException(e.Message, e);
            }
            finally {
                try {
                    if (imagePath != null && IsPreprocessingImages() && !inputImage.FullName.Equals(imagePath)) {
                        UtilService.DeleteFile(imagePath);
                    }
                }
                catch (SecurityException e) {
                    LogManager.GetLogger(GetType()).Error(MessageFormatUtil.Format(LogMessageConstant.CannotDeleteFile, imagePath
                        , e.Message));
                }
                try {
                    if (GetUserWordsFilePath() != null) {
                        UtilService.DeleteFile(GetUserWordsFilePath());
                    }
                }
                catch (SecurityException e) {
                    LogManager.GetLogger(GetType()).Error(MessageFormatUtil.Format(LogMessageConstant.CannotDeleteFile, GetUserWordsFilePath
                        (), e.Message));
                }
            }
        }

        /// <summary>Adds path to tesseract executable to the command.</summary>
        /// <param name="command">result command as list of strings</param>
        private void AddPathToExecutable(IList<String> command) {
            // path to tesseract executable cannot be uninitialized
            if (GetPathToExecutable() == null || String.IsNullOrEmpty(GetPathToExecutable())) {
                throw new OcrException(OcrException.CannotFindPathToTesseractExecutable);
            }
            else {
                command.Add(AddQuotes(GetPathToExecutable()));
            }
        }

        /// <summary>Sets hocr output format.</summary>
        /// <param name="command">result command as list of strings</param>
        private void SetHocrOutput(IList<String> command) {
            command.Add("-c");
            command.Add("tessedit_create_hocr=1");
        }

        /// <summary>Add path to user-words file for tesseract executable.</summary>
        /// <param name="command">result command as list of strings</param>
        private void AddUserWords(IList<String> command) {
            if (GetUserWordsFilePath() != null && !String.IsNullOrEmpty(GetUserWordsFilePath())) {
                command.Add("--user-words");
                command.Add(AddQuotes(GetUserWordsFilePath()));
                command.Add("--oem");
                command.Add("0");
            }
        }

        /// <summary>Set default DPI for image.</summary>
        /// <param name="command">result command as list of strings</param>
        private void AddDefaultDpi(IList<String> command) {
            command.Add("--dpi");
            command.Add("300");
        }

        /// <summary>Adds path to tess data to the command list.</summary>
        /// <param name="command">result command as list of strings</param>
        private void AddTessData(IList<String> command) {
            if (GetPathToTessData() != null && !String.IsNullOrEmpty(GetPathToTessData())) {
                command.Add("--tessdata-dir");
                command.Add(AddQuotes(GetTessData()));
            }
        }

        /// <summary>Adds selected Page Segmentation Mode as parameter.</summary>
        /// <param name="command">result command as list of strings</param>
        private void AddPageSegMode(IList<String> command) {
            if (GetPageSegMode() != null) {
                command.Add("--psm");
                command.Add(GetPageSegMode().ToString());
            }
        }

        /// <summary>Add list of selected languages concatenated to a string as parameter.</summary>
        /// <param name="command">result command as list of strings</param>
        private void AddLanguages(IList<String> command) {
            if (GetLanguagesAsList().Count > 0) {
                command.Add("-l");
                command.Add(GetLanguagesAsString());
            }
        }

        /// <summary>Adds path to the input image file.</summary>
        /// <param name="command">result command as list of strings</param>
        /// <param name="imagePath">path to the input image file as string</param>
        private void AddInputFile(IList<String> command, String imagePath) {
            command.Add(AddQuotes(imagePath));
        }

        /// <summary>Adds path to temporary output file with result.</summary>
        /// <param name="command">result command as list of strings</param>
        /// <param name="outputFile">output file with result</param>
        /// <param name="outputFormat">
        /// selected
        /// <see cref="OutputFormat"/>
        /// for
        /// tesseract
        /// </param>
        private void AddOutputFile(IList<String> command, FileInfo outputFile, IOcrReader.OutputFormat outputFormat
            ) {
            String extension = outputFormat.Equals(IOcrReader.OutputFormat.HOCR) ? ".hocr" : ".txt";
            String fileName = new String(outputFile.FullName.ToCharArray(), 0, outputFile.FullName.IndexOf(extension, 
                StringComparison.Ordinal));
            LogManager.GetLogger(GetType()).Info(MessageFormatUtil.Format(LogMessageConstant.CreatedTemporaryFile, outputFile
                .FullName));
            command.Add(AddQuotes(fileName));
        }

        /// <summary>Surrounds given string with quotes.</summary>
        /// <param name="value">string to be wrapped into quotes</param>
        /// <returns>wrapped string</returns>
        private String AddQuotes(String value) {
            return "\"" + value + "\"";
        }

        /// <summary>Preprocess given image if it is needed.</summary>
        /// <param name="inputImage">
        /// original input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="pageNumber">number of page to be OCRed</param>
        /// <returns>
        /// path to output image as
        /// <see cref="System.String"/>
        /// </returns>
        private String PreprocessImage(FileInfo inputImage, int pageNumber) {
            String path = inputImage.FullName;
            if (IsPreprocessingImages()) {
                path = ImageUtil.PreprocessImage(inputImage, pageNumber);
            }
            return path;
        }
    }
}
