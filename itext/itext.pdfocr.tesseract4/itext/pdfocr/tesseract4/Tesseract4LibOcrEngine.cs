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
using System.Text.RegularExpressions;
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
    /// This class provides possibilities to use features of "tesseract"
    /// using tess4j.
    /// Please note that this class is not thread-safe, in other words this Tesseract engine cannot
    /// be used for multithreaded processing. You should create one instance per thread
    /// </remarks>
    public class Tesseract4LibOcrEngine : AbstractTesseract4OcrEngine {
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

        /// <summary>Pattern for matching ASCII string.</summary>
        private static readonly Regex ASCII_STRING_PATTERN = iText.IO.Util.StringUtil.RegexCompile("^[\\u0000-\\u007F]*$"
            );

        /// <summary>
        /// Creates a new
        /// <see cref="Tesseract4LibOcrEngine"/>
        /// instance.
        /// </summary>
        /// <param name="tesseract4OcrEngineProperties">set of properteis</param>
        public Tesseract4LibOcrEngine(Tesseract4OcrEngineProperties tesseract4OcrEngineProperties)
            : base(tesseract4OcrEngineProperties) {
            tesseractInstance = TesseractOcrUtil.InitializeTesseractInstance(IsWindows(), null, null, null);
        }

        /// <summary>Gets tesseract instance.</summary>
        /// <returns>
        /// initialized
        /// <see cref="Tesseract.TesseractEngine"/>
        /// instance
        /// </returns>
        public virtual TesseractEngine GetTesseractInstance() {
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
        /// for tesseract
        /// </param>
        public virtual void InitializeTesseract(OutputFormat outputFormat) {
            if (GetTesseractInstance() == null || TesseractOcrUtil.IsTesseractInstanceDisposed(GetTesseractInstance())
                ) {
                tesseractInstance = TesseractOcrUtil.InitializeTesseractInstance(IsWindows(), GetTessData(), GetLanguagesAsString
                    (), GetTesseract4OcrEngineProperties().GetPathToUserWordsFile());
            }
            GetTesseractInstance().SetVariable("tessedit_create_hocr", outputFormat.Equals(OutputFormat.HOCR) ? "1" : 
                "0");
            if (GetTesseract4OcrEngineProperties().IsUseTxtToImproveHocrParsing()) {
                GetTesseractInstance().SetVariable("preserve_interword_spaces", "1");
            }
            GetTesseractInstance().SetVariable("user_defined_dpi", "300");
            if (GetTesseract4OcrEngineProperties().GetPathToUserWordsFile() != null) {
                GetTesseractInstance().SetVariable("load_system_dawg", "0");
                GetTesseractInstance().SetVariable("load_freq_dawg", "0");
                GetTesseractInstance().SetVariable("user_words_suffix", GetTesseract4OcrEngineProperties().GetDefaultUserWordsSuffix
                    ());
                GetTesseractInstance().SetVariable("user_words_file", GetTesseract4OcrEngineProperties().GetPathToUserWordsFile
                    ());
            }
            TesseractOcrUtil.SetTesseractProperties(GetTesseractInstance(), GetTessData(), GetLanguagesAsString(), GetTesseract4OcrEngineProperties
                ().GetPageSegMode(), GetTesseract4OcrEngineProperties().GetPathToUserWordsFile());
        }

        /// <summary>
        /// Performs tesseract OCR using wrapper for Tesseract OCR API for the selected page
        /// of input image (by default 1st).
        /// </summary>
        /// <remarks>
        /// Performs tesseract OCR using wrapper for Tesseract OCR API for the selected page
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
            try {
                // check tess data path for non ASCII characters
                ValidateTessDataPath(GetTessData());
                ValidateLanguages(GetTesseract4OcrEngineProperties().GetLanguages());
                InitializeTesseract(outputFormat);
                if (dispatchEvent) {
                    OnEvent();
                }
                // if preprocessing is not needed and provided image is tiff,
                // the image will be paginated and separate pages will be OCRed
                IList<String> resultList = new List<String>();
                if (!GetTesseract4OcrEngineProperties().IsPreprocessingImages() && ImagePreprocessingUtil.IsTiffImage(inputImage
                    )) {
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
                            LogManager.GetLogger(GetType()).Error(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_WRITE_TO_FILE
                                , e.Message));
                            throw new Tesseract4OcrException(Tesseract4OcrException.TESSERACT_FAILED);
                        }
                    }
                }
            }
            catch (Tesseract4OcrException e) {
                LogManager.GetLogger(GetType()).Error(e.Message);
                throw new Tesseract4OcrException(e.Message, e);
            }
            finally {
                if (tesseractInstance != null) {
                    TesseractOcrUtil.DisposeTesseractInstance(tesseractInstance);
                }
                if (GetTesseract4OcrEngineProperties().GetPathToUserWordsFile() != null && GetTesseract4OcrEngineProperties
                    ().IsUserWordsFileTemporary()) {
                    TesseractHelper.DeleteFile(GetTesseract4OcrEngineProperties().GetPathToUserWordsFile());
                }
            }
        }

        /// <summary>
        /// Validates Tess Data path,
        /// checks if tess data path contains only ASCII charset.
        /// </summary>
        /// <remarks>
        /// Validates Tess Data path,
        /// checks if tess data path contains only ASCII charset.
        /// Note: tesseract lib has issues with non ASCII characters in tess data path.
        /// </remarks>
        /// <param name="tessDataPath">
        /// 
        /// <see cref="System.String"/>
        /// path to tess data
        /// </param>
        private static void ValidateTessDataPath(String tessDataPath) {
            Matcher asciiStringMatcher = iText.IO.Util.Matcher.Match(ASCII_STRING_PATTERN, tessDataPath);
            if (!asciiStringMatcher.Matches()) {
                throw new Tesseract4OcrException(Tesseract4OcrException.PATH_TO_TESS_DATA_DIRECTORY_CONTAINS_NON_ASCII_CHARACTERS
                    );
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
        /// for tesseract
        /// </param>
        /// <returns>
        /// list of result string that will be written to a temporary files
        /// later
        /// </returns>
        private IList<String> GetOcrResultForMultiPage(FileInfo inputImage, OutputFormat outputFormat) {
            IList<String> resultList = new List<String>();
            try {
                InitializeTesseract(outputFormat);
                TesseractOcrUtil util = new TesseractOcrUtil();
                util.InitializeImagesListFromTiff(inputImage);
                int numOfPages = util.GetListOfPages().Count;
                for (int i = 0; i < numOfPages; i++) {
                    String result = util.GetOcrResultAsString(GetTesseractInstance(), util.GetListOfPages()[i], outputFormat);
                    resultList.Add(result);
                }
            }
            catch (TesseractException e) {
                String msg = MessageFormatUtil.Format(Tesseract4LogMessageConstant.TESSERACT_FAILED, e.Message);
                LogManager.GetLogger(GetType()).Error(msg);
                throw new Tesseract4OcrException(Tesseract4OcrException.TESSERACT_FAILED);
            }
            finally {
                TesseractOcrUtil.DisposeTesseractInstance(GetTesseractInstance());
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
        /// for tesseract
        /// </param>
        /// <param name="pageNumber">number of page to be OCRed</param>
        /// <returns>result as string that will be written to a temporary file later</returns>
        private String GetOcrResultForSinglePage(FileInfo inputImage, OutputFormat outputFormat, int pageNumber) {
            String result = null;
            try {
                // preprocess if required
                if (GetTesseract4OcrEngineProperties().IsPreprocessingImages()) {
                    // preprocess and try to ocr
                    result = new TesseractOcrUtil().GetOcrResultAsString(GetTesseractInstance(), ImagePreprocessingUtil.PreprocessImage
                        (inputImage, pageNumber, GetTesseract4OcrEngineProperties().GetImagePreprocessingOptions()), outputFormat
                        );
                }
                if (result == null) {
                    System.Drawing.Bitmap bufferedImage = ImagePreprocessingUtil.ReadImage(inputImage);
                    if (bufferedImage != null) {
                        try {
                            result = new TesseractOcrUtil().GetOcrResultAsString(GetTesseractInstance(), bufferedImage, outputFormat);
                        }
                        catch (Exception e) {
                            // NOSONAR
                            LogManager.GetLogger(GetType()).Info(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_PROCESS_IMAGE
                                , e.Message));
                        }
                    }
                    if (result == null) {
                        // perform ocr using original input image
                        result = new TesseractOcrUtil().GetOcrResultAsString(GetTesseractInstance(), inputImage, outputFormat);
                    }
                }
            }
            catch (Exception e) {
                // NOSONAR
                LogManager.GetLogger(GetType()).Error(MessageFormatUtil.Format(Tesseract4LogMessageConstant.TESSERACT_FAILED
                    , e.Message));
                throw new Tesseract4OcrException(Tesseract4OcrException.TESSERACT_FAILED);
            }
            return result;
        }
    }
}
