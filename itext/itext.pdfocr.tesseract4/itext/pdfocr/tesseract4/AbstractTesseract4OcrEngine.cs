/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2020 iText Group NV
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
using System.Text;
using Common.Logging;
using iText.IO.Util;
using iText.Pdfocr;

namespace iText.Pdfocr.Tesseract4 {
    /// <summary>
    /// The implementation of
    /// <see cref="iText.Pdfocr.IOcrEngine"/>.
    /// </summary>
    /// <remarks>
    /// The implementation of
    /// <see cref="iText.Pdfocr.IOcrEngine"/>.
    /// This class provides possibilities to perform OCR, to read data from input
    /// files and to return contained text in the required format.
    /// Also there are possibilities to use features of "tesseract"
    /// (optical character recognition engine for various operating systems).
    /// </remarks>
    public abstract class AbstractTesseract4OcrEngine : IOcrEngine {
        /// <summary>Supported image formats.</summary>
        private static readonly ICollection<String> SUPPORTED_IMAGE_FORMATS = JavaCollectionsUtil.UnmodifiableSet(
            new HashSet<String>(JavaUtil.ArraysAsList("bmp", "png", "tiff", "tif", "jpeg", "jpg", "jpe", "jfif")));

        /// <summary>Set of properties.</summary>
        private Tesseract4OcrEngineProperties tesseract4OcrEngineProperties;

        public AbstractTesseract4OcrEngine(Tesseract4OcrEngineProperties tesseract4OcrEngineProperties) {
            this.tesseract4OcrEngineProperties = tesseract4OcrEngineProperties;
        }

        /// <summary>
        /// Performs tesseract OCR using command line tool
        /// or a wrapper for Tesseract OCR API.
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
        public abstract void DoTesseractOcr(FileInfo inputImage, IList<FileInfo> outputFiles, OutputFormat outputFormat
            , int pageNumber);

        /// <summary>Performs tesseract OCR for the first (or for the only) image page.</summary>
        /// <param name="inputImage">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="outputFile">output file for the result for the first page</param>
        /// <param name="outputFormat">
        /// selected
        /// <see cref="OutputFormat"/>
        /// for tesseract
        /// </param>
        public virtual void DoTesseractOcr(FileInfo inputImage, FileInfo outputFile, OutputFormat outputFormat) {
            DoTesseractOcr(inputImage, JavaCollectionsUtil.SingletonList<FileInfo>(outputFile), outputFormat, 1);
        }

        /// <summary>
        /// Performs OCR using provided
        /// <see cref="iText.Pdfocr.IOcrEngine"/>
        /// for the given list of
        /// input images and saves output to a text file using provided path.
        /// </summary>
        /// <param name="inputImages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of images to be OCRed
        /// </param>
        /// <param name="txtFile">file to be created</param>
        public virtual void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile) {
            LogManager.GetLogger(GetType()).Info(MessageFormatUtil.Format(Tesseract4LogMessageConstant.START_OCR_FOR_IMAGES
                , inputImages.Count));
            StringBuilder content = new StringBuilder();
            foreach (FileInfo inputImage in inputImages) {
                content.Append(DoImageOcr(inputImage, OutputFormat.TXT));
            }
            // write to file
            TesseractHelper.WriteToTextFile(txtFile.FullName, content.ToString());
        }

        /// <summary>
        /// Gets properties for
        /// <see cref="AbstractTesseract4OcrEngine"/>.
        /// </summary>
        /// <returns>
        /// set properties
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// </returns>
        public Tesseract4OcrEngineProperties GetTesseract4OcrEngineProperties() {
            return tesseract4OcrEngineProperties;
        }

        /// <summary>
        /// Sets properties for
        /// <see cref="AbstractTesseract4OcrEngine"/>.
        /// </summary>
        /// <param name="tesseract4OcrEngineProperties">
        /// set of properties
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// for
        /// <see cref="AbstractTesseract4OcrEngine"/>
        /// </param>
        public void SetTesseract4OcrEngineProperties(Tesseract4OcrEngineProperties tesseract4OcrEngineProperties) {
            this.tesseract4OcrEngineProperties = tesseract4OcrEngineProperties;
        }

        /// <summary>
        /// Gets list of languages concatenated with "+" symbol to a string
        /// in format required by tesseract.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// of concatenated languages
        /// </returns>
        public String GetLanguagesAsString() {
            if (GetTesseract4OcrEngineProperties().GetLanguages().Count > 0) {
                return String.Join("+", GetTesseract4OcrEngineProperties().GetLanguages());
            }
            else {
                return GetTesseract4OcrEngineProperties().GetDefaultLanguage();
            }
        }

        /// <summary>
        /// Reads data from the provided input image file and returns retrieved
        /// data in the format described below.
        /// </summary>
        /// <param name="input">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IDictionary{K, V}"/>
        /// where key is
        /// <see cref="int?"/>
        /// representing the number of the page and value is
        /// <see cref="System.Collections.IList{E}"/>
        /// of
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// elements where each
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// element contains a word or a line and its 4
        /// coordinates(bbox)
        /// </returns>
        public IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input) {
            IDictionary<int, IList<TextInfo>> result = new LinkedDictionary<int, IList<TextInfo>>();
            VerifyImageFormatValidity(input);
            IDictionary<String, IDictionary<int, IList<TextInfo>>> processedData = ProcessInputFiles(input, OutputFormat
                .HOCR);
            if (processedData != null && processedData.Count > 0) {
                IList<String> keys = new List<String>(processedData.Keys);
                result = processedData.Get(keys[0]);
            }
            return result;
        }

        /// <summary>
        /// Reads data from the provided input image file and returns retrieved
        /// data as string.
        /// </summary>
        /// <param name="input">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="outputFormat">
        /// return
        /// <see cref="OutputFormat"/>
        /// result
        /// </param>
        /// <returns>
        /// OCR result as a
        /// <see cref="System.String"/>
        /// that is
        /// returned after processing the given image
        /// </returns>
        public String DoImageOcr(FileInfo input, OutputFormat outputFormat) {
            String result = "";
            VerifyImageFormatValidity(input);
            IDictionary<String, IDictionary<int, IList<TextInfo>>> processedData = ProcessInputFiles(input, outputFormat
                );
            if (processedData != null && processedData.Count > 0) {
                IList<String> keys = new List<String>(processedData.Keys);
                if (outputFormat.Equals(OutputFormat.TXT)) {
                    result = keys[0];
                }
                else {
                    StringBuilder outputText = new StringBuilder();
                    IDictionary<int, IList<TextInfo>> outputMap = processedData.Get(keys[0]);
                    foreach (int page in outputMap.Keys) {
                        StringBuilder pageText = new StringBuilder();
                        foreach (TextInfo textInfo in outputMap.Get(page)) {
                            pageText.Append(textInfo.GetText());
                            pageText.Append(Environment.NewLine);
                        }
                        outputText.Append(pageText);
                        outputText.Append(Environment.NewLine);
                    }
                    result = outputText.ToString();
                }
            }
            return result;
        }

        /// <summary>Checks current os type.</summary>
        /// <returns>boolean true is current os is windows, otherwise - false</returns>
        public virtual bool IsWindows() {
            return IdentifyOsType().ToLowerInvariant().Contains("win");
        }

        /// <summary>Identifies type of current OS and return it (win, linux).</summary>
        /// <returns>
        /// type of current os as
        /// <see cref="System.String"/>
        /// </returns>
        public virtual String IdentifyOsType() {
            String os = Environment.GetEnvironmentVariable("os.name") == null ? Environment.GetEnvironmentVariable("OS"
                ) : Environment.GetEnvironmentVariable("os.name");
            return os.ToLowerInvariant();
        }

        /// <summary>
        /// Validates list of provided languages and
        /// checks if they all exist in given tess data directory.
        /// </summary>
        /// <param name="languagesList">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of provided languages
        /// </param>
        public virtual void ValidateLanguages(IList<String> languagesList) {
            String suffix = ".traineddata";
            if (languagesList.Count == 0) {
                if (!new FileInfo(GetTessData() + System.IO.Path.DirectorySeparatorChar + GetTesseract4OcrEngineProperties
                    ().GetDefaultLanguage() + suffix).Exists) {
                    throw new Tesseract4OcrException(Tesseract4OcrException.INCORRECT_LANGUAGE).SetMessageParams(GetTesseract4OcrEngineProperties
                        ().GetDefaultLanguage() + suffix, GetTessData());
                }
            }
            else {
                foreach (String lang in languagesList) {
                    if (!new FileInfo(GetTessData() + System.IO.Path.DirectorySeparatorChar + lang + suffix).Exists) {
                        throw new Tesseract4OcrException(Tesseract4OcrException.INCORRECT_LANGUAGE).SetMessageParams(lang + suffix
                            , GetTessData());
                    }
                }
            }
        }

        /// <summary>Reads data from the provided input image file.</summary>
        /// <param name="input">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="outputFormat">
        /// 
        /// <see cref="OutputFormat"/>
        /// for the result returned
        /// by
        /// <see cref="iText.Pdfocr.IOcrEngine"/>
        /// </param>
        /// <returns>
        /// Map<String, Map&lt;Integer, List&lt;textinfo>&gt;&gt;
        /// if output format is txt,
        /// result is key of the returned map(String),
        /// otherwise - the value (Map&lt;Integer, List
        /// <see cref="iText.Pdfocr.TextInfo"/>
        /// &gt;)
        /// </returns>
        internal virtual IDictionary<String, IDictionary<int, IList<TextInfo>>> ProcessInputFiles(FileInfo input, 
            OutputFormat outputFormat) {
            IDictionary<int, IList<TextInfo>> imageData = new LinkedDictionary<int, IList<TextInfo>>();
            StringBuilder data = new StringBuilder();
            try {
                // image needs to be paginated only if it's tiff
                // or preprocessing isn't required
                int realNumOfPages = !ImagePreprocessingUtil.IsTiffImage(input) ? 1 : ImagePreprocessingUtil.GetNumberOfPageTiff
                    (input);
                int numOfPages = GetTesseract4OcrEngineProperties().IsPreprocessingImages() ? realNumOfPages : 1;
                int numOfFiles = GetTesseract4OcrEngineProperties().IsPreprocessingImages() ? 1 : realNumOfPages;
                for (int page = 1; page <= numOfPages; page++) {
                    IList<FileInfo> tempFiles = new List<FileInfo>();
                    String extension = outputFormat.Equals(OutputFormat.HOCR) ? ".hocr" : ".txt";
                    for (int i = 0; i < numOfFiles; i++) {
                        tempFiles.Add(CreateTempFile(extension));
                    }
                    DoTesseractOcr(input, tempFiles, outputFormat, page);
                    if (outputFormat.Equals(OutputFormat.HOCR)) {
                        IDictionary<int, IList<TextInfo>> pageData = TesseractHelper.ParseHocrFile(tempFiles, GetTesseract4OcrEngineProperties
                            ().GetTextPositioning());
                        if (GetTesseract4OcrEngineProperties().IsPreprocessingImages()) {
                            imageData.Put(page, pageData.Get(1));
                        }
                        else {
                            imageData = pageData;
                        }
                    }
                    else {
                        foreach (FileInfo tmpFile in tempFiles) {
                            if (File.Exists(System.IO.Path.Combine(tmpFile.FullName))) {
                                data.Append(TesseractHelper.ReadTxtFile(tmpFile));
                            }
                        }
                    }
                    foreach (FileInfo file in tempFiles) {
                        TesseractHelper.DeleteFile(file.FullName);
                    }
                }
            }
            catch (System.IO.IOException e) {
                LogManager.GetLogger(GetType()).Error(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_OCR_INPUT_FILE
                    , e.Message));
            }
            IDictionary<String, IDictionary<int, IList<TextInfo>>> result = new LinkedDictionary<String, IDictionary<int
                , IList<TextInfo>>>();
            result.Put(data.ToString(), imageData);
            return result;
        }

        /// <summary>Gets path to provided tess data directory.</summary>
        /// <returns>
        /// path to provided tess data directory as
        /// <see cref="System.String"/>
        /// </returns>
        internal virtual String GetTessData() {
            if (GetTesseract4OcrEngineProperties().GetPathToTessData() == null) {
                throw new Tesseract4OcrException(Tesseract4OcrException.PATH_TO_TESS_DATA_IS_NOT_SET);
            }
            else {
                return GetTesseract4OcrEngineProperties().GetPathToTessData().FullName;
            }
        }

        /// <summary>Creates a temporary file with given extension.</summary>
        /// <param name="extension">
        /// file extesion for a new file
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// a new created
        /// <see cref="System.IO.FileInfo"/>
        /// instance
        /// </returns>
        private FileInfo CreateTempFile(String extension) {
            String tmpFileName = TesseractOcrUtil.GetTempDir() + Guid.NewGuid().ToString() + extension;
            return new FileInfo(tmpFileName);
        }

        /// <summary>Validates input image format.</summary>
        /// <remarks>
        /// Validates input image format.
        /// Allowed image formats are listed
        /// in
        /// <see cref="SUPPORTED_IMAGE_FORMATS"/>
        /// </remarks>
        /// <param name="image">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        private void VerifyImageFormatValidity(FileInfo image) {
            bool isValid = false;
            String extension = "incorrect extension";
            int index = image.FullName.LastIndexOf('.');
            if (index > 0) {
                extension = new String(image.FullName.ToCharArray(), index + 1, image.FullName.Length - index - 1);
                foreach (String format in SUPPORTED_IMAGE_FORMATS) {
                    if (format.Equals(extension.ToLowerInvariant())) {
                        isValid = true;
                        break;
                    }
                }
            }
            if (!isValid) {
                LogManager.GetLogger(GetType()).Error(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE
                    , image.FullName));
                throw new Tesseract4OcrException(Tesseract4OcrException.INCORRECT_INPUT_IMAGE_FORMAT).SetMessageParams(extension
                    );
            }
        }
    }
}
