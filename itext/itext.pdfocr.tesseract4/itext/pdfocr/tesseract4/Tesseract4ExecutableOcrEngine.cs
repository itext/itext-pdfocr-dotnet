using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Common.Logging;
using iText.IO.Util;

namespace iText.Pdfocr.Tesseract4 {
    /// <summary>
    /// The implementation of
    /// <see cref="AbstractTesseract4OcrEngine"/>
    /// for tesseract OCR.
    /// </summary>
    /// <remarks>
    /// The implementation of
    /// <see cref="AbstractTesseract4OcrEngine"/>
    /// for tesseract OCR.
    /// This class provides possibilities to use features of "tesseract" CL tool
    /// (optical character recognition engine for various operating systems).
    /// Please note that it's assumed that "tesseract" has already been
    /// installed locally.
    /// </remarks>
    public class Tesseract4ExecutableOcrEngine : AbstractTesseract4OcrEngine {
        /// <summary>Path to the tesseract executable.</summary>
        /// <remarks>
        /// Path to the tesseract executable.
        /// By default it's assumed that "tesseract" already exists in the "PATH".
        /// </remarks>
        private String pathToExecutable;

        /// <summary>
        /// Creates a new
        /// <see cref="Tesseract4ExecutableOcrEngine"/>
        /// instance.
        /// </summary>
        /// <param name="tesseract4OcrEngineProperties">set of properties</param>
        public Tesseract4ExecutableOcrEngine(Tesseract4OcrEngineProperties tesseract4OcrEngineProperties)
            : base(tesseract4OcrEngineProperties) {
            SetPathToExecutable("tesseract");
        }

        /// <summary>
        /// Creates a new
        /// <see cref="Tesseract4ExecutableOcrEngine"/>
        /// instance.
        /// </summary>
        /// <param name="executablePath">path to tesseract executable</param>
        /// <param name="tesseract4OcrEngineProperties">set of properties</param>
        public Tesseract4ExecutableOcrEngine(String executablePath, Tesseract4OcrEngineProperties tesseract4OcrEngineProperties
            )
            : base(tesseract4OcrEngineProperties) {
            SetPathToExecutable(executablePath);
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
        /// for tesseract
        /// </param>
        /// <param name="pageNumber">number of page to be processed</param>
        public override void DoTesseractOcr(FileInfo inputImage, IList<FileInfo> outputFiles, OutputFormat outputFormat
            , int pageNumber) {
            IList<String> @params = new List<String>();
            String execPath = null;
            String imagePath = null;
            try {
                imagePath = inputImage.FullName;
                // path to tesseract executable
                if (GetPathToExecutable() == null || String.IsNullOrEmpty(GetPathToExecutable())) {
                    throw new Tesseract4OcrException(Tesseract4OcrException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE);
                }
                else {
                    if (IsWindows()) {
                        execPath = AddQuotes(GetPathToExecutable());
                    }
                    else {
                        execPath = GetPathToExecutable();
                    }
                }
                CheckTesseractInstalled(execPath);
                // path to tess data
                AddTessData(@params);
                // validate languages before preprocessing started
                ValidateLanguages(GetTesseract4OcrEngineProperties().GetLanguages());
                // preprocess input file if needed and add it
                imagePath = PreprocessImage(inputImage, pageNumber);
                AddInputFile(@params, imagePath);
                // output file
                AddOutputFile(@params, outputFiles[0], outputFormat);
                // page segmentation mode
                AddPageSegMode(@params);
                // add user words if needed
                AddUserWords(@params);
                // required languages
                AddLanguages(@params);
                if (outputFormat.Equals(OutputFormat.HOCR)) {
                    // path to hocr script
                    SetHocrOutput(@params);
                }
                // set default user defined dpi
                AddDefaultDpi(@params);
                TesseractHelper.RunCommand(execPath, @params);
            }
            catch (Tesseract4OcrException e) {
                LogManager.GetLogger(GetType()).Error(e.Message);
                throw new Tesseract4OcrException(e.Message, e);
            }
            finally {
                try {
                    if (imagePath != null && GetTesseract4OcrEngineProperties().IsPreprocessingImages() && !inputImage.FullName
                        .Equals(imagePath)) {
                        TesseractHelper.DeleteFile(imagePath);
                    }
                }
                catch (SecurityException e) {
                    LogManager.GetLogger(GetType()).Error(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_DELETE_FILE
                        , imagePath, e.Message));
                }
                try {
                    if (GetTesseract4OcrEngineProperties().GetPathToUserWordsFile() != null) {
                        TesseractHelper.DeleteFile(GetTesseract4OcrEngineProperties().GetPathToUserWordsFile());
                    }
                }
                catch (SecurityException e) {
                    LogManager.GetLogger(GetType()).Error(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_DELETE_FILE
                        , GetTesseract4OcrEngineProperties().GetPathToUserWordsFile(), e.Message));
                }
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
            if (GetTesseract4OcrEngineProperties().GetPathToUserWordsFile() != null && !String.IsNullOrEmpty(GetTesseract4OcrEngineProperties
                ().GetPathToUserWordsFile())) {
                command.Add("--user-words");
                command.Add(AddQuotes(GetTesseract4OcrEngineProperties().GetPathToUserWordsFile()));
                command.Add("--oem");
                command.Add("0");
            }
        }

        /// <summary>Set default DPI for image.</summary>
        /// <param name="command">result command as list of strings</param>
        private void AddDefaultDpi(IList<String> command) {
            command.Add("-c");
            command.Add("user_defined_dpi=300");
        }

        /// <summary>Adds path to tess data to the command list.</summary>
        /// <param name="command">result command as list of strings</param>
        private void AddTessData(IList<String> command) {
            command.Add("--tessdata-dir");
            command.Add(AddQuotes(GetTessData()));
        }

        /// <summary>Adds selected Page Segmentation Mode as parameter.</summary>
        /// <param name="command">result command as list of strings</param>
        private void AddPageSegMode(IList<String> command) {
            if (GetTesseract4OcrEngineProperties().GetPageSegMode() != null) {
                command.Add("--psm");
                command.Add(GetTesseract4OcrEngineProperties().GetPageSegMode().ToString());
            }
        }

        /// <summary>Add list of selected languages concatenated to a string as parameter.</summary>
        /// <param name="command">result command as list of strings</param>
        private void AddLanguages(IList<String> command) {
            if (GetTesseract4OcrEngineProperties().GetLanguages().Count > 0) {
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
        /// for tesseract
        /// </param>
        private void AddOutputFile(IList<String> command, FileInfo outputFile, OutputFormat outputFormat) {
            String extension = outputFormat.Equals(OutputFormat.HOCR) ? ".hocr" : ".txt";
            String fileName = new String(outputFile.FullName.ToCharArray(), 0, outputFile.FullName.IndexOf(extension, 
                StringComparison.Ordinal));
            LogManager.GetLogger(GetType()).Info(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CREATED_TEMPORARY_FILE
                , outputFile.FullName));
            command.Add(AddQuotes(fileName));
        }

        /// <summary>Surrounds given string with quotes.</summary>
        /// <param name="value">string to be wrapped into quotes</param>
        /// <returns>wrapped string</returns>
        private String AddQuotes(String value) {
            // choosing correct quotes for system
            if (IsWindows()) {
                return "\"" + value + "\"";
            }
            else {
                return "'" + value + "'";
            }
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
            if (GetTesseract4OcrEngineProperties().IsPreprocessingImages()) {
                path = ImagePreprocessingUtil.PreprocessImage(inputImage, pageNumber);
            }
            return path;
        }

        /// <summary>
        /// Check whether tesseract executable is installed on the machine and
        /// provided path to tesseract executable is correct.
        /// </summary>
        /// <param name="execPath">path to tesseract executable</param>
        private void CheckTesseractInstalled(String execPath) {
            try {
                TesseractHelper.RunCommand(execPath, JavaCollectionsUtil.SingletonList<String>("--version"));
            }
            catch (Tesseract4OcrException e) {
                throw new Tesseract4OcrException(Tesseract4OcrException.TESSERACT_NOT_FOUND, e);
            }
        }
    }
}
