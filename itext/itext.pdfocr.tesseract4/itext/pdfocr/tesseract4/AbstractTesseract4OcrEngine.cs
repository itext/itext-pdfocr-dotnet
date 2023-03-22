/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 Apryse Group NV
Authors: Apryse Software.

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
using System.Threading;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Actions.Confirmations;
using iText.Commons.Actions.Contexts;
using iText.Commons.Actions.Data;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Pdfocr;
using iText.Pdfocr.Statistics;
using iText.Pdfocr.Tesseract4.Actions.Data;
using iText.Pdfocr.Tesseract4.Actions.Events;
using iText.Pdfocr.Tesseract4.Exceptions;
using iText.Pdfocr.Tesseract4.Logs;

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
    public abstract class AbstractTesseract4OcrEngine : IOcrEngine, IProductAware {
        /// <summary>Supported image formats.</summary>
        private static readonly ICollection<ImageType> SUPPORTED_IMAGE_FORMATS = JavaCollectionsUtil.UnmodifiableSet
            (new HashSet<ImageType>(JavaUtil.ArraysAsList(ImageType.BMP, ImageType.PNG, ImageType.TIFF, ImageType.
            JPEG)));

        internal ICollection<Guid> processedUUID = new HashSet<Guid>();

        /// <summary>Set of properties.</summary>
        private Tesseract4OcrEngineProperties tesseract4OcrEngineProperties;

        private ThreadLocal<IMetaInfo> threadLocalMetaInfo = new ThreadLocal<IMetaInfo>();

        /// <summary>
        /// Creates a new
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// instance
        /// based on another
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// instance (copy
        /// constructor).
        /// </summary>
        /// <param name="tesseract4OcrEngineProperties">
        /// the other
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// instance
        /// </param>
        public AbstractTesseract4OcrEngine(Tesseract4OcrEngineProperties tesseract4OcrEngineProperties) {
            this.tesseract4OcrEngineProperties = tesseract4OcrEngineProperties;
        }

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
            DoTesseractOcr(inputImage, outputFile, outputFormat, new OcrProcessContext(new Tesseract4EventHelper()));
        }

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
        /// <param name="ocrProcessContext">ocr process context</param>
        public virtual void DoTesseractOcr(FileInfo inputImage, FileInfo outputFile, OutputFormat outputFormat, OcrProcessContext
             ocrProcessContext) {
            DoTesseractOcr(inputImage, JavaCollectionsUtil.SingletonList<FileInfo>(outputFile), outputFormat, 1, ocrProcessContext
                .GetOcrEventHelper());
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
            CreateTxtFile(inputImages, txtFile, new OcrProcessContext(new Tesseract4EventHelper()));
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
        /// <param name="ocrProcessContext">ocr process context</param>
        public virtual void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile, OcrProcessContext ocrProcessContext
            ) {
            ITextLogManager.GetLogger(GetType()).LogInformation(MessageFormatUtil.Format(Tesseract4LogMessageConstant.
                START_OCR_FOR_IMAGES, inputImages.Count));
            AbstractPdfOcrEventHelper storedEventHelper;
            if (ocrProcessContext.GetOcrEventHelper() == null) {
                storedEventHelper = new Tesseract4EventHelper();
            }
            else {
                storedEventHelper = ocrProcessContext.GetOcrEventHelper();
            }
            PdfOcrTesseract4ProductEvent @event = PdfOcrTesseract4ProductEvent.CreateProcessImageEvent(storedEventHelper
                .GetSequenceId(), null, storedEventHelper.GetConfirmationType());
            storedEventHelper.OnEvent(@event);
            try {
                // set Tesseract4FileResultEventHelper
                ocrProcessContext.SetOcrEventHelper(new Tesseract4FileResultEventHelper(storedEventHelper));
                StringBuilder content = new StringBuilder();
                foreach (FileInfo inputImage in inputImages) {
                    content.Append(DoImageOcr(inputImage, OutputFormat.TXT, ocrProcessContext));
                }
                // write to file
                TesseractHelper.WriteToTextFile(txtFile.FullName, content.ToString());
                if (@event.GetConfirmationType() == EventConfirmationType.ON_DEMAND) {
                    storedEventHelper.OnEvent(new ConfirmEvent(@event));
                }
            }
            finally {
                ocrProcessContext.SetOcrEventHelper(storedEventHelper);
            }
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
            VerifyImageFormatValidity(input);
            return ((AbstractTesseract4OcrEngine.TextInfoTesseractOcrResult)ProcessInputFiles(input, OutputFormat.HOCR
                , new Tesseract4EventHelper())).GetTextInfos();
        }

        /// <summary>
        /// Reads data from the provided input image file and returns retrieved
        /// data in the format described below.
        /// </summary>
        /// <param name="input">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="ocrProcessContext">ocr process context</param>
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
        public IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input, OcrProcessContext ocrProcessContext) {
            VerifyImageFormatValidity(input);
            return ((AbstractTesseract4OcrEngine.TextInfoTesseractOcrResult)ProcessInputFiles(input, OutputFormat.HOCR
                , ocrProcessContext.GetOcrEventHelper())).GetTextInfos();
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
        /// <param name="ocrProcessContext">ocr process context</param>
        /// <returns>
        /// OCR result as a
        /// <see cref="System.String"/>
        /// that is
        /// returned after processing the given image
        /// </returns>
        public String DoImageOcr(FileInfo input, OutputFormat outputFormat, OcrProcessContext ocrProcessContext) {
            String result = "";
            VerifyImageFormatValidity(input);
            AbstractTesseract4OcrEngine.ITesseractOcrResult processedData = ProcessInputFiles(input, outputFormat, ocrProcessContext
                .GetOcrEventHelper());
            if (processedData != null) {
                if (outputFormat.Equals(OutputFormat.TXT)) {
                    result = ((AbstractTesseract4OcrEngine.StringTesseractOcrResult)processedData).GetData();
                }
                else {
                    StringBuilder outputText = new StringBuilder();
                    IDictionary<int, IList<TextInfo>> outputMap = ((AbstractTesseract4OcrEngine.TextInfoTesseractOcrResult)processedData
                        ).GetTextInfos();
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
            return DoImageOcr(input, outputFormat, new OcrProcessContext(new Tesseract4EventHelper()));
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
                    throw new PdfOcrInputTesseract4Exception(PdfOcrTesseract4ExceptionMessageConstant.INCORRECT_LANGUAGE).SetMessageParams
                        (GetTesseract4OcrEngineProperties().GetDefaultLanguage() + suffix, GetTessData());
                }
            }
            else {
                foreach (String lang in languagesList) {
                    if (!new FileInfo(GetTessData() + System.IO.Path.DirectorySeparatorChar + lang + suffix).Exists) {
                        throw new PdfOcrInputTesseract4Exception(PdfOcrTesseract4ExceptionMessageConstant.INCORRECT_LANGUAGE).SetMessageParams
                            (lang + suffix, GetTessData());
                    }
                }
            }
        }

        /// <summary><inheritDoc/></summary>
        public virtual PdfOcrMetaInfoContainer GetMetaInfoContainer() {
            return new PdfOcrMetaInfoContainer(new Tesseract4MetaInfo());
        }

        public virtual ProductData GetProductData() {
            return PdfOcrTesseract4ProductData.GetInstance();
        }

        /// <summary>
        /// Performs tesseract OCR using command line tool
        /// or a wrapper for Tesseract OCR API.
        /// </summary>
        /// <remarks>
        /// Performs tesseract OCR using command line tool
        /// or a wrapper for Tesseract OCR API.
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
        internal virtual void DoTesseractOcr(FileInfo inputImage, IList<FileInfo> outputFiles, OutputFormat outputFormat
            , int pageNumber, AbstractPdfOcrEventHelper eventHelper) {
            DoTesseractOcr(inputImage, outputFiles, outputFormat, pageNumber, true, eventHelper);
        }

        /// <summary>
        /// Performs tesseract OCR using command line tool
        /// or a wrapper for Tesseract OCR API.
        /// </summary>
        /// <remarks>
        /// Performs tesseract OCR using command line tool
        /// or a wrapper for Tesseract OCR API.
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
        /// <param name="dispatchEvent">indicates if event needs to be dispatched</param>
        /// <param name="eventHelper">event helper</param>
        internal abstract void DoTesseractOcr(FileInfo inputImage, IList<FileInfo> outputFiles, OutputFormat outputFormat
            , int pageNumber, bool dispatchEvent, AbstractPdfOcrEventHelper eventHelper);

        /// <summary>Gets path to provided tess data directory.</summary>
        /// <returns>
        /// path to provided tess data directory as
        /// <see cref="System.String"/>
        /// </returns>
        internal virtual String GetTessData() {
            if (GetTesseract4OcrEngineProperties().GetPathToTessData() == null) {
                throw new PdfOcrTesseract4Exception(PdfOcrTesseract4ExceptionMessageConstant.PATH_TO_TESS_DATA_IS_NOT_SET);
            }
            else {
                return GetTesseract4OcrEngineProperties().GetPathToTessData().FullName;
            }
        }

        internal virtual PdfOcrTesseract4ProductEvent OnEvent(AbstractPdfOcrEventHelper eventHelper) {
            // usage event
            PdfOcrTesseract4ProductEvent @event = PdfOcrTesseract4ProductEvent.CreateProcessImageEvent(eventHelper.GetSequenceId
                (), null, eventHelper.GetConfirmationType());
            eventHelper.OnEvent(@event);
            return @event;
        }

        internal virtual void OnEventStatistics(AbstractPdfOcrEventHelper eventHelper) {
            eventHelper.OnEvent(new PdfOcrOutputTypeStatisticsEvent(PdfOcrOutputType.DATA, PdfOcrTesseract4ProductData
                .GetInstance()));
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
        /// <param name="eventHelper">event helper</param>
        /// <returns>
        /// 
        /// <see cref="ITesseractOcrResult"/>
        /// instance, either
        /// <see cref="StringTesseractOcrResult"/>
        /// if output format is TXT, or
        /// <see cref="TextInfoTesseractOcrResult"/>
        /// if the output format is HOCR
        /// </returns>
        private AbstractTesseract4OcrEngine.ITesseractOcrResult ProcessInputFiles(FileInfo input, OutputFormat outputFormat
            , AbstractPdfOcrEventHelper eventHelper) {
            IDictionary<int, IList<TextInfo>> imageData = new LinkedDictionary<int, IList<TextInfo>>();
            StringBuilder data = new StringBuilder();
            IList<FileInfo> tempFiles = new List<FileInfo>();
            AbstractTesseract4OcrEngine.ITesseractOcrResult result = null;
            try {
                // image needs to be paginated only if it's tiff
                // or preprocessing isn't required
                int realNumOfPages = !ImagePreprocessingUtil.IsTiffImage(input) ? 1 : ImagePreprocessingUtil.GetNumberOfPageTiff
                    (input);
                int numOfPages = GetTesseract4OcrEngineProperties().IsPreprocessingImages() ? realNumOfPages : 1;
                int numOfFiles = GetTesseract4OcrEngineProperties().IsPreprocessingImages() ? 1 : realNumOfPages;
                for (int page = 1; page <= numOfPages; page++) {
                    String extension = outputFormat.Equals(OutputFormat.HOCR) ? ".hocr" : ".txt";
                    for (int i = 0; i < numOfFiles; i++) {
                        tempFiles.Add(CreateTempFile(extension));
                    }
                    DoTesseractOcr(input, tempFiles, outputFormat, page, true, eventHelper);
                    if (outputFormat.Equals(OutputFormat.HOCR)) {
                        IList<FileInfo> tempTxtFiles = null;
                        if (GetTesseract4OcrEngineProperties().IsUseTxtToImproveHocrParsing()) {
                            tempTxtFiles = new List<FileInfo>();
                            for (int i = 0; i < numOfFiles; i++) {
                                tempTxtFiles.Add(CreateTempFile(".txt"));
                            }
                            DoTesseractOcr(input, tempTxtFiles, OutputFormat.TXT, page, false, eventHelper);
                        }
                        IDictionary<int, IList<TextInfo>> pageData = TesseractHelper.ParseHocrFile(tempFiles, tempTxtFiles, GetTesseract4OcrEngineProperties
                            ());
                        if (GetTesseract4OcrEngineProperties().IsPreprocessingImages()) {
                            imageData.Put(page, pageData.Get(1));
                        }
                        else {
                            imageData = pageData;
                        }
                        result = new AbstractTesseract4OcrEngine.TextInfoTesseractOcrResult(imageData);
                    }
                    else {
                        foreach (FileInfo tmpFile in tempFiles) {
                            if (File.Exists(System.IO.Path.Combine(tmpFile.FullName))) {
                                data.Append(TesseractHelper.ReadTxtFile(tmpFile));
                            }
                        }
                        result = new AbstractTesseract4OcrEngine.StringTesseractOcrResult(data.ToString());
                    }
                }
            }
            catch (System.IO.IOException e) {
                ITextLogManager.GetLogger(GetType()).LogError(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_OCR_INPUT_FILE
                    , e.Message));
            }
            finally {
                foreach (FileInfo file in tempFiles) {
                    TesseractHelper.DeleteFile(file.FullName);
                }
            }
            return result;
        }

        /// <summary>Creates a temporary file with given extension.</summary>
        /// <param name="extension">
        /// file extension for a new file
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// a new created
        /// <see cref="System.IO.FileInfo"/>
        /// instance
        /// </returns>
        private FileInfo CreateTempFile(String extension) {
            String tmpFileName = TesseractOcrUtil.GetTempFilePath(Guid.NewGuid().ToString(), extension);
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
            ImageType type = ImagePreprocessingUtil.GetImageType(image);
            bool isValid = SUPPORTED_IMAGE_FORMATS.Contains(type);
            if (!isValid) {
                ITextLogManager.GetLogger(GetType()).LogError(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_READ_INPUT_IMAGE
                    , image.FullName));
                throw new PdfOcrInputTesseract4Exception(PdfOcrTesseract4ExceptionMessageConstant.INCORRECT_INPUT_IMAGE_FORMAT
                    ).SetMessageParams(image.Name);
            }
        }

        internal interface ITesseractOcrResult {
        }

        internal class StringTesseractOcrResult : AbstractTesseract4OcrEngine.ITesseractOcrResult {
            private String data;

            internal StringTesseractOcrResult(String data) {
                this.data = data;
            }

            internal virtual String GetData() {
                return data;
            }
        }

        internal class TextInfoTesseractOcrResult : AbstractTesseract4OcrEngine.ITesseractOcrResult {
            private IDictionary<int, IList<TextInfo>> textInfos;

            internal TextInfoTesseractOcrResult(IDictionary<int, IList<TextInfo>> textInfos) {
                this.textInfos = textInfos;
            }

            internal virtual IDictionary<int, IList<TextInfo>> GetTextInfos() {
                return this.textInfos;
            }
        }
    }
}
