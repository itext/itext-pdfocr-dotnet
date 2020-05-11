using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common.Logging;
using iText.IO.Util;

namespace iText.Ocr {
    /// <summary>
    /// The implementation of
    /// <see cref="IOcrReader"/>.
    /// </summary>
    /// <remarks>
    /// The implementation of
    /// <see cref="IOcrReader"/>.
    /// This class provides possibilities to perform OCR, to read data from input
    /// files and to return contained text in the required format.
    /// Also there are possibilities to use features of "tesseract"
    /// (optical character recognition engine for various operating systems).
    /// </remarks>
    public abstract class TesseractReader : IOcrReader {
        /// <summary>Default language for OCR.</summary>
        public const String DEFAULT_LANGUAGE = "eng";

        /// <summary>Default suffix for user-word file.</summary>
        /// <remarks>
        /// Default suffix for user-word file.
        /// (e.g. name: 'eng.user-words')
        /// </remarks>
        public const String DEFAULT_USER_WORDS_SUFFIX = "user-words";

        /// <summary>List of languages required for ocr for provided images.</summary>
        private IList<String> languages = JavaCollectionsUtil.EmptyList<String>();

        /// <summary>Path to directory with tess data.</summary>
        private String tessDataDir;

        /// <summary>Page Segmentation Mode.</summary>
        private int? pageSegMode;

        /// <summary>Type of current OS.</summary>
        private String osType;

        /// <summary>"True" - if images need to be preprocessed, otherwise - false.</summary>
        /// <remarks>
        /// "True" - if images need to be preprocessed, otherwise - false.
        /// True by default.
        /// </remarks>
        private bool preprocessingImages = true;

        /// <summary>Defines the way text is retrieved from tesseract output.</summary>
        /// <remarks>
        /// Defines the way text is retrieved from tesseract output.
        /// Default text positioning is by lines.
        /// </remarks>
        private IOcrReader.TextPositioning textPositioning = IOcrReader.TextPositioning.BY_LINES;

        /// <summary>Path to the file containing user words.</summary>
        /// <remarks>
        /// Path to the file containing user words.
        /// Each word should be on a new line,
        /// file should end with a newline character.
        /// </remarks>
        private String userWordsFile = null;

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
        /// for
        /// tesseract
        /// </param>
        /// <param name="pageNumber">number of page to be processed</param>
        public abstract void DoTesseractOcr(FileInfo inputImage, IList<FileInfo> outputFiles, IOcrReader.OutputFormat
             outputFormat, int pageNumber);

        /// <summary>Performs tesseract OCR for the first (or for the only) image page.</summary>
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
        public virtual void DoTesseractOcr(FileInfo inputImage, IList<FileInfo> outputFiles, IOcrReader.OutputFormat
             outputFormat) {
            DoTesseractOcr(inputImage, outputFiles, outputFormat, 1);
        }

        /// <summary>Sets list of languages to be recognized in provided images.</summary>
        /// <param name="requiredLanguages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of languages in string
        /// format
        /// </param>
        public void SetLanguages(IList<String> requiredLanguages) {
            languages = JavaCollectionsUtil.UnmodifiableList<String>(requiredLanguages);
        }

        /// <summary>Gets list of languages required for provided images.</summary>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of languages
        /// </returns>
        public IList<String> GetLanguagesAsList() {
            return new List<String>(languages);
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
            if (GetLanguagesAsList().Count > 0) {
                return String.Join("+", GetLanguagesAsList());
            }
            else {
                return DEFAULT_LANGUAGE;
            }
        }

        /// <summary>Gets path to directory with tess data.</summary>
        /// <returns>path to directory with tess data</returns>
        public String GetPathToTessData() {
            return tessDataDir;
        }

        /// <summary>Sets path to directory with tess data.</summary>
        /// <param name="tessData">
        /// path to train directory as
        /// <see cref="System.String"/>
        /// </param>
        public void SetPathToTessData(String tessData) {
            tessDataDir = tessData;
        }

        /// <summary>Gets Page Segmentation Mode.</summary>
        /// <returns>
        /// psm mode as
        /// <see cref="int?"/>
        /// </returns>
        public int? GetPageSegMode() {
            return pageSegMode;
        }

        /// <summary>Sets Page Segmentation Mode.</summary>
        /// <remarks>
        /// Sets Page Segmentation Mode.
        /// More detailed explanation about psm modes could be found
        /// here https://github.com/tesseract-ocr/tesseract/blob/master/doc/tesseract.1.asc#options
        /// </remarks>
        /// <param name="mode">
        /// psm mode as
        /// <see cref="int?"/>
        /// </param>
        public void SetPageSegMode(int? mode) {
            pageSegMode = mode;
        }

        /// <summary>Get type of current OS.</summary>
        /// <returns>
        /// os type as
        /// <see cref="System.String"/>
        /// </returns>
        public String GetOsType() {
            return osType;
        }

        /// <summary>Sets type of current OS.</summary>
        /// <param name="os">
        /// os type as
        /// <see cref="System.String"/>
        /// </param>
        public void SetOsType(String os) {
            osType = os;
        }

        /// <summary>Checks whether image preprocessing is needed.</summary>
        /// <returns>true if images need to be preprocessed, otherwise - false</returns>
        public bool IsPreprocessingImages() {
            return preprocessingImages;
        }

        /// <summary>Sets true if image preprocessing is needed.</summary>
        /// <param name="preprocess">
        /// true if images need to be preprocessed,
        /// otherwise - false
        /// </param>
        public void SetPreprocessingImages(bool preprocess) {
            preprocessingImages = preprocess;
        }

        /// <summary>
        /// Defines the way text is retrieved from tesseract output using
        /// <see cref="TextPositioning"/>.
        /// </summary>
        /// <returns>the way text is retrieved</returns>
        public IOcrReader.TextPositioning GetTextPositioning() {
            return textPositioning;
        }

        /// <summary>
        /// Defines the way text is retrieved from tesseract output
        /// using
        /// <see cref="TextPositioning"/>.
        /// </summary>
        /// <param name="positioning">the way text is retrieved</param>
        public void SetTextPositioning(IOcrReader.TextPositioning positioning) {
            textPositioning = positioning;
        }

        /// <summary>
        /// Reads data from the provided input image file and returns retrieved data
        /// in the format described below.
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
        /// <see cref="TextInfo"/>
        /// elements where each
        /// <see cref="TextInfo"/>
        /// element contains a word or a line and its 4
        /// coordinates(bbox)
        /// </returns>
        public sealed override IDictionary<int, IList<TextInfo>> ReadDataFromInput(FileInfo input) {
            IDictionary<String, IDictionary<int, IList<TextInfo>>> result = ProcessInputFiles(input, IOcrReader.OutputFormat
                .HOCR);
            if (result != null && result.Count > 0) {
                IList<String> keys = new List<String>(result.Keys);
                return result.Get(keys[0]);
            }
            else {
                return new LinkedDictionary<int, IList<TextInfo>>();
            }
        }

        /// <summary>
        /// Reads data from the provided input image file and returns retrieved data
        /// as string.
        /// </summary>
        /// <param name="input">
        /// input image
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="outputFormat">
        /// 
        /// <see cref="OutputFormat"/>
        /// for the result
        /// returned by
        /// <see cref="IOcrReader"/>
        /// </param>
        /// <returns>
        /// OCR result as a
        /// <see cref="System.String"/>
        /// that is
        /// returned after processing the given image
        /// </returns>
        public sealed override String ReadDataFromInput(FileInfo input, IOcrReader.OutputFormat outputFormat) {
            IDictionary<String, IDictionary<int, IList<TextInfo>>> result = ProcessInputFiles(input, outputFormat);
            if (result != null && result.Count > 0) {
                IList<String> keys = new List<String>(result.Keys);
                if (outputFormat.Equals(IOcrReader.OutputFormat.TXT)) {
                    return keys[0];
                }
                else {
                    StringBuilder outputText = new StringBuilder();
                    IDictionary<int, IList<TextInfo>> outputMap = result.Get(keys[0]);
                    foreach (int page in outputMap.Keys) {
                        StringBuilder pageText = new StringBuilder();
                        foreach (TextInfo textInfo in outputMap.Get(page)) {
                            pageText.Append(textInfo.GetText());
                            pageText.Append(Environment.NewLine);
                        }
                        outputText.Append(pageText);
                        outputText.Append(Environment.NewLine);
                    }
                    return outputText.ToString();
                }
            }
            else {
                return "";
            }
        }

        /// <summary>
        /// Using provided list of words there will be created
        /// temporary file containing words (one per line) which
        /// ends with a new line character.
        /// </summary>
        /// <remarks>
        /// Using provided list of words there will be created
        /// temporary file containing words (one per line) which
        /// ends with a new line character. Train data for provided language
        /// should exist in specified tess data directory.
        /// </remarks>
        /// <param name="language">
        /// language as
        /// <see cref="System.String"/>
        /// , tessdata for
        /// this languages has to exist in tess data directory
        /// </param>
        /// <param name="userWords">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// of custom words
        /// </param>
        public virtual void SetUserWords(String language, IList<String> userWords) {
            if (userWords == null || userWords.Count == 0) {
                userWordsFile = null;
            }
            else {
                try {
                    MemoryStream baos = new MemoryStream();
                    foreach (String word in userWords) {
                        byte[] bytesWord = word.GetBytes();
                        baos.Write(bytesWord, 0, bytesWord.Length);
                        byte[] bytesSeparator = Environment.NewLine.GetBytes();
                        baos.Write(bytesSeparator, 0, bytesSeparator.Length);
                    }
                    Stream inputStream = new MemoryStream(baos.ToArray());
                    baos.Dispose();
                    SetUserWords(language, inputStream);
                }
                catch (System.IO.IOException e) {
                    LogManager.GetLogger(GetType()).Warn(MessageFormatUtil.Format(LogMessageConstant.CannotUseUserWords, e.Message
                        ));
                }
            }
        }

        /// <summary>
        /// Using provided input stream there will be created
        /// temporary file (with name 'language.user-words')
        /// containing words (one per line) which ends with
        /// a new line character.
        /// </summary>
        /// <remarks>
        /// Using provided input stream there will be created
        /// temporary file (with name 'language.user-words')
        /// containing words (one per line) which ends with
        /// a new line character. Train data for provided language
        /// should exist in specified tess data directory.
        /// </remarks>
        /// <param name="language">
        /// language as
        /// <see cref="System.String"/>
        /// , tessdata for
        /// this languages has to exist in tess data directory
        /// </param>
        /// <param name="inputStream">
        /// custom user words as
        /// <see cref="System.IO.Stream"/>
        /// </param>
        public virtual void SetUserWords(String language, Stream inputStream) {
            String userWordsFileName = TesseractUtil.GetTempDir() + System.IO.Path.DirectorySeparatorChar + language +
                 "." + DEFAULT_USER_WORDS_SUFFIX;
            if (!GetLanguagesAsList().Contains(language)) {
                if (DEFAULT_LANGUAGE.Equals(language.ToLowerInvariant())) {
                    IList<String> languagesList = GetLanguagesAsList();
                    languagesList.Add(language);
                    SetLanguages(languagesList);
                }
                else {
                    throw new OcrException(OcrException.LanguageIsNotInTheList).SetMessageParams(language);
                }
            }
            ValidateLanguages(JavaCollectionsUtil.SingletonList<String>(language));
            try {
                using (StreamWriter writer = new StreamWriter(userWordsFileName)) {
                    TextReader reader = new StreamReader(inputStream, System.Text.Encoding.UTF8);
                    int data;
                    while ((data = reader.Read()) != -1) {
                        writer.Write(data);
                    }
                    writer.Write(Environment.NewLine);
                    userWordsFile = userWordsFileName;
                }
            }
            catch (System.IO.IOException e) {
                userWordsFile = null;
                LogManager.GetLogger(GetType()).Warn(MessageFormatUtil.Format(LogMessageConstant.CannotUseUserWords, e.Message
                    ));
            }
        }

        /// <summary>Returns path to the user words file.</summary>
        /// <returns>
        /// path to user words file as
        /// <see cref="System.String"/>
        /// if it
        /// exists, otherwise - null
        /// </returns>
        public String GetUserWordsFilePath() {
            return userWordsFile;
        }

        /// <summary>Checks current os type.</summary>
        /// <returns>boolean true is current os is windows, otherwise - false</returns>
        public virtual bool IsWindows() {
            return GetOsType().ToLowerInvariant().Contains("win");
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
                if (!new FileInfo(GetTessData() + System.IO.Path.DirectorySeparatorChar + DEFAULT_LANGUAGE + suffix).Exists
                    ) {
                    throw new OcrException(OcrException.IncorrectLanguage).SetMessageParams(DEFAULT_LANGUAGE + suffix, GetTessData
                        ());
                }
            }
            else {
                foreach (String lang in languagesList) {
                    if (!new FileInfo(GetTessData() + System.IO.Path.DirectorySeparatorChar + lang + suffix).Exists) {
                        throw new OcrException(OcrException.IncorrectLanguage).SetMessageParams(lang + suffix, GetTessData());
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
        /// <see cref="IOcrReader"/>
        /// </param>
        /// <returns>
        /// Map<String, Map&lt;Integer, List&lt;textinfo>&gt;&gt;
        /// if output format is txt,
        /// result is key of the returned map(String),
        /// otherwise - the value (Map&lt;Integer, List
        /// <see cref="TextInfo"/>
        /// &gt;)
        /// </returns>
        internal virtual IDictionary<String, IDictionary<int, IList<TextInfo>>> ProcessInputFiles(FileInfo input, 
            IOcrReader.OutputFormat outputFormat) {
            IDictionary<int, IList<TextInfo>> imageData = new LinkedDictionary<int, IList<TextInfo>>();
            StringBuilder data = new StringBuilder();
            try {
                // image needs to be paginated only if it's tiff
                // or preprocessing isn't required
                int realNumOfPages = !ImageUtil.IsTiffImage(input) ? 1 : ImageUtil.GetNumberOfPageTiff(input);
                int numOfPages = IsPreprocessingImages() ? realNumOfPages : 1;
                int numOfFiles = IsPreprocessingImages() ? 1 : realNumOfPages;
                for (int page = 1; page <= numOfPages; page++) {
                    IList<FileInfo> tempFiles = new List<FileInfo>();
                    String extension = outputFormat.Equals(IOcrReader.OutputFormat.HOCR) ? ".hocr" : ".txt";
                    for (int i = 0; i < numOfFiles; i++) {
                        tempFiles.Add(CreateTempFile(extension));
                    }
                    DoTesseractOcr(input, tempFiles, outputFormat, page);
                    if (outputFormat.Equals(IOcrReader.OutputFormat.HOCR)) {
                        IDictionary<int, IList<TextInfo>> pageData = UtilService.ParseHocrFile(tempFiles, GetTextPositioning());
                        if (IsPreprocessingImages()) {
                            imageData.Put(page, pageData.Get(1));
                        }
                        else {
                            imageData = pageData;
                        }
                    }
                    else {
                        foreach (FileInfo tmpFile in tempFiles) {
                            if (File.Exists(System.IO.Path.Combine(tmpFile.FullName))) {
                                data.Append(UtilService.ReadTxtFile(tmpFile));
                            }
                        }
                    }
                    foreach (FileInfo file in tempFiles) {
                        UtilService.DeleteFile(file.FullName);
                    }
                }
            }
            catch (System.IO.IOException e) {
                LogManager.GetLogger(GetType()).Error(MessageFormatUtil.Format(LogMessageConstant.CannotOcrInputFile, e.Message
                    ));
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
        /// , otherwise - the default one
        /// </returns>
        internal virtual String GetTessData() {
            if (GetPathToTessData() != null && !String.IsNullOrEmpty(GetPathToTessData())) {
                return GetPathToTessData();
            }
            else {
                throw new OcrException(OcrException.CannotFindPathToTessDataDirectory);
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
            String tmpFileName = TesseractUtil.GetTempDir() + Guid.NewGuid().ToString() + extension;
            return new FileInfo(tmpFileName);
        }
    }
}
