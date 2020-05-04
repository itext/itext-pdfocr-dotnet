using System;
using System.Collections.Generic;
using System.IO;
using Common.Logging;
using iText.IO.Util;

namespace iText.Ocr {
    /// <summary>Tesseract Executable Reader class.</summary>
    /// <remarks>
    /// Tesseract Executable Reader class.
    /// (extends Tesseract Reader class)
    /// This class provides possibilities to use features of "tesseract"
    /// (optical character recognition engine for various operating systems)
    /// This class provides possibility to perform OCR, read data from input files
    /// and return contained text in the described format
    /// This class provides possibilities to set type of current os,
    /// required languages for OCR for input images,
    /// set path to directory with tess data and set path
    /// to the tesseract executable
    /// Please note that It's assumed that "tesseract" is already
    /// installed in the system
    /// </remarks>
    public class TesseractExecutableReader : TesseractReader {
        /// <summary>Path to the script.</summary>
        private String pathToScript;

        /// <summary>Path to the tesseract executable.</summary>
        /// <remarks>
        /// Path to the tesseract executable.
        /// By default it's assumed that "tesseract" already exists in the PATH
        /// </remarks>
        private String pathToExecutable;

        /// <summary>Create new TesseractExecutableReader.</summary>
        /// <param name="tessDataPath">
        /// 
        /// <see cref="System.String"/>
        /// </param>
        public TesseractExecutableReader(String tessDataPath) {
            SetPathToExecutable("tesseract");
            SetOsType(IdentifyOSType());
            SetPathToTessData(tessDataPath);
        }

        /// <summary>Create new TesseractExecutableReader.</summary>
        /// <param name="executablePath">
        /// 
        /// <see cref="System.String"/>
        /// </param>
        /// <param name="tessDataPath">
        /// 
        /// <see cref="System.String"/>
        /// </param>
        public TesseractExecutableReader(String executablePath, String tessDataPath) {
            SetPathToExecutable(executablePath);
            SetOsType(IdentifyOSType());
            SetPathToTessData(tessDataPath);
        }

        /// <summary>Create new TesseractExecutableReader.</summary>
        /// <param name="path">
        /// 
        /// <see cref="System.String"/>
        /// </param>
        /// <param name="languagesList">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// </param>
        /// <param name="tessDataPath">
        /// 
        /// <see cref="System.String"/>
        /// </param>
        public TesseractExecutableReader(String path, String tessDataPath, IList<String> languagesList) {
            SetPathToExecutable(path);
            SetLanguages(JavaCollectionsUtil.UnmodifiableList<String>(languagesList));
            SetPathToTessData(tessDataPath);
            SetOsType(IdentifyOSType());
        }

        /// <summary>Set path to tesseract executable.</summary>
        /// <remarks>
        /// Set path to tesseract executable.
        /// By default it's assumed that "tesseract" already exists in the PATH
        /// </remarks>
        /// <param name="path">
        /// 
        /// <see cref="System.String"/>
        /// </param>
        public void SetPathToExecutable(String path) {
            pathToExecutable = path;
        }

        /// <summary>Get path to tesseract executable.</summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// </returns>
        public String GetPathToExecutable() {
            return pathToExecutable;
        }

        /// <summary>Set path to script.</summary>
        /// <param name="path">
        /// 
        /// <see cref="System.String"/>
        /// </param>
        public void SetPathToScript(String path) {
            pathToScript = path;
        }

        /// <summary>Get path to script.</summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// </returns>
        public String GetPathToScript() {
            return pathToScript;
        }

        /// <summary>Perform tesseract OCR.</summary>
        /// <param name="inputImage">
        /// 
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="outputFiles">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of output files
        /// (one for each page)
        /// for tesseract executable only the first file is required
        /// </param>
        /// <param name="outputFormat">
        /// 
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="pageNumber">int, number of page to be OCRed</param>
        public override void DoTesseractOcr(FileInfo inputImage, IList<FileInfo> outputFiles, IOcrReader.OutputFormat
             outputFormat, int pageNumber) {
            IList<String> command = new List<String>();
            String imagePath = inputImage.FullName;
            try {
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
                // required languages
                AddLanguages(command);
                if (outputFormat.Equals(IOcrReader.OutputFormat.HOCR)) {
                    // path to hocr script
                    SetHocrOutput(command);
                }
                AddPathToScript(command);
                TesseractUtil.RunCommand(command, IsWindows());
            }
            catch (OCRException e) {
                LogManager.GetLogger(GetType()).Error(e.Message);
                throw new OCRException(e.Message, e);
            }
            finally {
                if (imagePath != null && IsPreprocessingImages() && !inputImage.FullName.Equals(imagePath)) {
                    UtilService.DeleteFile(imagePath);
                }
                if (GetUserWordsFilePath() != null) {
                    UtilService.DeleteFile(GetUserWordsFilePath());
                }
            }
        }

        /// <summary>Add path to tesseract executable.</summary>
        private void AddPathToExecutable(IList<String> command) {
            // path to tesseract executable cannot be uninitialized
            if (GetPathToExecutable() == null || String.IsNullOrEmpty(GetPathToExecutable())) {
                throw new OCRException(OCRException.CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE);
            }
            else {
                command.Add(AddQuotes(GetPathToExecutable()));
            }
        }

        /// <summary>Set hocr output format.</summary>
        private void SetHocrOutput(IList<String> command) {
            command.Add("-c");
            command.Add("tessedit_create_hocr=1");
        }

        /// <summary>Add path to script.</summary>
        private void AddPathToScript(IList<String> command) {
            if (GetPathToScript() != null && !String.IsNullOrEmpty(GetPathToScript())) {
                command.Add(AddQuotes(GetPathToScript()));
            }
        }

        /// <summary>Add path to user-words file for tesseract executable.</summary>
        private void AddUserWords(IList<String> command) {
            if (GetUserWordsFilePath() != null && !String.IsNullOrEmpty(GetUserWordsFilePath())) {
                command.Add("--user-words");
                command.Add(AddQuotes(GetUserWordsFilePath()));
                command.Add("--oem");
                command.Add("0");
            }
        }

        /// <summary>Add path to tess data.</summary>
        private void AddTessData(IList<String> command) {
            if (GetPathToTessData() != null && !String.IsNullOrEmpty(GetPathToTessData())) {
                command.Add("--tessdata-dir");
                command.Add(AddQuotes(GetTessData()));
            }
        }

        /// <summary>Add select Page Segmentation Mode as parameter.</summary>
        private void AddPageSegMode(IList<String> command) {
            if (GetPageSegMode() != null) {
                command.Add("--psm");
                command.Add(GetPageSegMode().ToString());
            }
        }

        /// <summary>Add list pf selected languages as parameter.</summary>
        private void AddLanguages(IList<String> command) {
            if (GetLanguagesAsList().Count > 0) {
                command.Add("-l");
                command.Add(GetLanguagesAsString());
            }
        }

        /// <summary>Preprocess input image (if needed) and add path to this file.</summary>
        private void AddInputFile(IList<String> command, String imagePath) {
            command.Add(AddQuotes(imagePath));
        }

        /// <summary>Add path to temporary output file.</summary>
        private void AddOutputFile(IList<String> command, FileInfo outputFile, IOcrReader.OutputFormat outputFormat
            ) {
            String extension = outputFormat.Equals(IOcrReader.OutputFormat.HOCR) ? ".hocr" : ".txt";
            String fileName = new String(outputFile.FullName.ToCharArray(), 0, outputFile.FullName.IndexOf(extension, 
                StringComparison.Ordinal));
            LogManager.GetLogger(GetType()).Info(MessageFormatUtil.Format(LogMessageConstant.CREATED_TEMPORARY_FILE, outputFile
                .FullName));
            command.Add(AddQuotes(fileName));
        }

        /// <summary>Surrounds given string with quotes.</summary>
        private String AddQuotes(String value) {
            return "\"" + value + "\"";
        }

        /// <summary>Preprocess given image if it is needed.</summary>
        /// <param name="inputImage">
        /// 
        /// <see cref="System.IO.FileInfo"/>
        /// original input image
        /// </param>
        /// <param name="pageNumber">int, number of page to be OCRed</param>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// path to output image
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
