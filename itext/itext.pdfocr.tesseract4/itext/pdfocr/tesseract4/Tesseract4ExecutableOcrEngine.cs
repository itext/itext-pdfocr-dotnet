/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Common.Logging;
using Tesseract;
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
        /// <remarks>
        /// Performs tesseract OCR using command line tool for the selected page
        /// of input image (by default 1st).
        /// Please note that list of output files is accepted instead of a single file because
        /// page number parameter is not respected in case of TIFF images not requiring preprocessing.
        /// In other words, if the passed image is the TIFF image and according to the
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// no preprocessing is needed, each page of the TIFF image is OCRed and the number of output files in the list
        /// is expected to be same as number of pages in the image, otherwise, only one file is expected
        /// </remarks>
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
        /// <param name="dispatchEvent">
        /// indicates if
        /// <see cref="iText.Pdfocr.Tesseract4.Events.PdfOcrTesseract4Event"/>
        /// needs to be dispatched
        /// </param>
        internal override void DoTesseractOcr(FileInfo inputImage, IList<FileInfo> outputFiles, OutputFormat outputFormat
            , int pageNumber, bool dispatchEvent) {
            ScheduledCheck();
            IList<String> @params = new List<String>();
            String execPath = null;
            String imagePath = null;
            String workingDirectory = null;
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
                // preprocess input file if needed
                imagePath = PreprocessImage(inputImage, pageNumber);
                // get the input file parent directory as working directory
                // as tesseract cannot parse non ascii characters in input path
                String imageParentDir = TesseractOcrUtil.GetParentDirectory(imagePath);
                String replacement = IsWindows() ? "" : "/";
                workingDirectory = imageParentDir.Replace("file:///", replacement).Replace("file:/", replacement);
                // input file
                AddInputFile(@params, imagePath);
                // output file
                AddOutputFile(@params, outputFiles[0], outputFormat, imagePath);
                // page segmentation mode
                AddPageSegMode(@params);
                // add user words if needed
                AddUserWords(@params, imagePath);
                // required languages
                AddLanguages(@params);
                AddOutputFormat(@params, outputFormat);
                AddPreserveInterwordSpaces(@params);
                // set default user defined dpi
                AddDefaultDpi(@params);
                if (dispatchEvent) {
                    OnEvent();
                }
                // run tesseract process
                TesseractHelper.RunCommand(execPath, @params, workingDirectory);
            }
            catch (Tesseract4OcrException e) {
                LogManager.GetLogger(GetType()).Error(e.Message);
                throw new Tesseract4OcrException(e.Message, e);
            }
            finally {
                try {
                    if (imagePath != null && !inputImage.FullName.Equals(imagePath)) {
                        TesseractHelper.DeleteFile(imagePath);
                    }
                }
                catch (SecurityException e) {
                    LogManager.GetLogger(GetType()).Error(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_DELETE_FILE
                        , imagePath, e.Message));
                }
                try {
                    if (GetTesseract4OcrEngineProperties().GetPathToUserWordsFile() != null && GetTesseract4OcrEngineProperties
                        ().IsUserWordsFileTemporary()) {
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

        /// <summary>Sets preserve_interword_spaces option.</summary>
        /// <param name="command">result command as list of strings</param>
        private void AddPreserveInterwordSpaces(IList<String> command) {
            if (GetTesseract4OcrEngineProperties().IsUseTxtToImproveHocrParsing()) {
                command.Add("-c");
                command.Add("preserve_interword_spaces=1");
            }
        }

        /// <summary>Add output format.</summary>
        /// <param name="command">result command as list of strings</param>
        /// <param name="outputFormat">output format</param>
        private void AddOutputFormat(IList<String> command, OutputFormat outputFormat) {
            if (outputFormat == OutputFormat.HOCR) {
                SetHocrOutput(command);
            }
        }

        /// <summary>Add path to user-words file for tesseract executable.</summary>
        /// <param name="command">result command as list of strings</param>
        private void AddUserWords(IList<String> command, String imgPath) {
            if (GetTesseract4OcrEngineProperties().GetPathToUserWordsFile() != null && !String.IsNullOrEmpty(GetTesseract4OcrEngineProperties
                ().GetPathToUserWordsFile())) {
                FileInfo userWordsFile = new FileInfo(GetTesseract4OcrEngineProperties().GetPathToUserWordsFile());
                // Workaround for a non-ASCII characters in path
                // Currently works only if the user words (or output files) reside in the same directory as the input image
                // Leaves only a filename in this case, otherwise - absolute path to output file
                String filePath = AreEqualParentDirectories(imgPath, userWordsFile.FullName) ? userWordsFile.Name : userWordsFile
                    .FullName;
                command.Add("--user-words");
                command.Add(AddQuotes(filePath));
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
                command.Add("-c");
                command.Add("tessedit_pageseg_mode=" + GetTesseract4OcrEngineProperties().GetPageSegMode());
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
            command.Add(AddQuotes(new FileInfo(imagePath).Name));
        }

        /// <summary>Adds path to temporary output file with result.</summary>
        /// <param name="command">result command as list of strings</param>
        /// <param name="outputFile">output file with result</param>
        /// <param name="outputFormat">
        /// selected
        /// <see cref="OutputFormat"/>
        /// for tesseract
        /// </param>
        private void AddOutputFile(IList<String> command, FileInfo outputFile, OutputFormat outputFormat, String inputImagePath
            ) {
            String extension = outputFormat.Equals(OutputFormat.HOCR) ? ".hocr" : ".txt";
            try {
                // Workaround for a non-ASCII characters in path
                // Currently works only if the user words (or output files) reside in the same directory as the input image
                // Leaves only a filename in this case, otherwise - absolute path to output file
                String filePath = AreEqualParentDirectories(inputImagePath, outputFile.FullName) ? outputFile.Name : outputFile
                    .FullName;
                String fileName = new String(filePath.ToCharArray(), 0, filePath.IndexOf(extension, StringComparison.Ordinal
                    ));
                LogManager.GetLogger(GetType()).Info(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CREATED_TEMPORARY_FILE
                    , outputFile.FullName));
                command.Add(AddQuotes(fileName));
            }
            catch (Exception) {
                // NOSONAR
                throw new Tesseract4OcrException(Tesseract4OcrException.TESSERACT_FAILED);
            }
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
            String tmpFileName = TesseractOcrUtil.GetTempFilePath(Guid.NewGuid().ToString(), GetExtension(inputImage));
            String path = inputImage.FullName;
            try {
                if (GetTesseract4OcrEngineProperties().IsPreprocessingImages()) {
                    Pix pix = ImagePreprocessingUtil.PreprocessImage(inputImage, pageNumber, GetTesseract4OcrEngineProperties(
                        ).GetImagePreprocessingOptions());
                    TesseractOcrUtil.SavePixToPngFile(tmpFileName, pix);
                    if (!File.Exists(System.IO.Path.Combine(tmpFileName))) {
                        System.Drawing.Bitmap img = TesseractOcrUtil.ConvertPixToImage(pix);
                        if (img != null) {
                            TesseractOcrUtil.SaveImageToTempPngFile(tmpFileName, img);
                        }
                    }
                }
                if (!GetTesseract4OcrEngineProperties().IsPreprocessingImages() || !File.Exists(System.IO.Path.Combine(tmpFileName
                    ))) {
                    TesseractOcrUtil.CreateTempFileCopy(path, tmpFileName);
                }
                if (File.Exists(System.IO.Path.Combine(tmpFileName))) {
                    path = tmpFileName;
                }
            }
            catch (System.IO.IOException e) {
                LogManager.GetLogger(GetType()).Error(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE
                    , e.Message));
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

        /// <summary>Gets input image file extension.</summary>
        /// <param name="inputImage">input  file</param>
        /// <returns>
        /// file extension as a
        /// <see cref="System.String"/>
        /// </returns>
        private String GetExtension(FileInfo inputImage) {
            if (inputImage != null) {
                int index = inputImage.FullName.LastIndexOf('.');
                if (index > 0) {
                    String extension = new String(inputImage.FullName.ToCharArray(), index, inputImage.FullName.Length - index
                        );
                    return extension.ToLowerInvariant();
                }
            }
            return ".png";
        }

        /// <summary>Checks whether parent directories are equal for the passed file paths.</summary>
        /// <param name="firstPath">path to the first file</param>
        /// <param name="secondPath">path to the second file</param>
        /// <returns>true if parent directories are equal, otherwise - false</returns>
        private bool AreEqualParentDirectories(String firstPath, String secondPath) {
            String firstParentDir = TesseractOcrUtil.GetParentDirectory(firstPath);
            String secondParentDir = TesseractOcrUtil.GetParentDirectory(secondPath);
            return firstParentDir != null && firstParentDir.Equals(secondParentDir);
        }
    }
}
