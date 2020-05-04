using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common.Logging;
using iText.IO.Util;

namespace iText.Ocr {
    /// <summary>Tesseract reader class.</summary>
    /// <remarks>
    /// Tesseract reader class.
    /// This class provides possibilities to use features of "tesseract"
    /// (optical character recognition engine for various operating systems)
    /// This class provides possibility to perform OCR, read data from input files
    /// and return contained text in the described format
    /// This class provides possibilities to set type of current os,
    /// required languages for OCR for input images,
    /// set path to directory with tess data.
    /// </remarks>
    public abstract class TesseractReader : IOcrReader {
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
        /// By default - true.
        /// </remarks>
        private bool preprocessingImages = true;

        /// <summary>Default text positioning is by lines.</summary>
        private IOcrReader.TextPositioning textPositioning = IOcrReader.TextPositioning.BY_LINES;

        /// <summary>Path to the file containing user words.</summary>
        /// <remarks>
        /// Path to the file containing user words.
        /// Each word should on new line , file should end with a newline.
        /// </remarks>
        private String userWordsFile = null;

        /// <summary>Perform tesseract OCR.</summary>
        /// <param name="inputImage">
        /// 
        /// <see cref="System.IO.FileInfo"/>
        /// input image file
        /// </param>
        /// <param name="outputFiles">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// list of output file
        /// (one per each page)
        /// </param>
        /// <param name="outputFormat">
        /// 
        /// <see cref="OutputFormat"/>
        /// </param>
        /// <param name="pageNumber">int</param>
        public abstract void DoTesseractOcr(FileInfo inputImage, IList<FileInfo> outputFiles, IOcrReader.OutputFormat
             outputFormat, int pageNumber);

        /// <summary>Set list of languages required for provided images.</summary>
        /// <param name="requiredLanguages">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// </param>
        public void SetLanguages(IList<String> requiredLanguages) {
            languages = JavaCollectionsUtil.UnmodifiableList<String>(requiredLanguages);
        }

        /// <summary>Get list of languages required for provided images.</summary>
        /// <returns>
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// </returns>
        public IList<String> GetLanguagesAsList() {
            return new List<String>(languages);
        }

        /// <summary>
        /// Get list of languages converted to a string
        /// in format required by tesseract.
        /// </summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// </returns>
        public String GetLanguagesAsString() {
            if (GetLanguagesAsList().Count > 0) {
                return String.Join("+", GetLanguagesAsList());
            }
            else {
                return "eng";
            }
        }

        /// <summary>Set path to directory with tess data.</summary>
        /// <param name="tessData">
        /// 
        /// <see cref="System.String"/>
        /// </param>
        public void SetPathToTessData(String tessData) {
            tessDataDir = tessData;
        }

        /// <summary>Get path to directory with tess data.</summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// </returns>
        public String GetPathToTessData() {
            return tessDataDir;
        }

        /// <summary>Set Page Segmentation Mode.</summary>
        /// <param name="mode">
        /// 
        /// <see cref="int?"/>
        /// </param>
        public void SetPageSegMode(int? mode) {
            pageSegMode = mode;
        }

        /// <summary>Get Page Segmentation Mode.</summary>
        /// <returns>
        /// 
        /// <see cref="int?"/>
        /// pageSegMode
        /// </returns>
        public int? GetPageSegMode() {
            return pageSegMode;
        }

        /// <summary>Set type of current OS.</summary>
        /// <param name="os">
        /// 
        /// <see cref="System.String"/>
        /// </param>
        public void SetOsType(String os) {
            osType = os;
        }

        /// <summary>Get type of current OS.</summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// </returns>
        public String GetOsType() {
            return osType;
        }

        /// <summary>Set true if images need to be preprocessed, otherwise - false.</summary>
        /// <param name="preprocess">boolean</param>
        public void SetPreprocessingImages(bool preprocess) {
            preprocessingImages = preprocess;
        }

        /// <returns>true if images need to be preprocessed, otherwise - false</returns>
        public bool IsPreprocessingImages() {
            return preprocessingImages;
        }

        /// <summary>Set text positioning (by lines or by words).</summary>
        /// <param name="positioning">
        /// 
        /// <see cref="TextPositioning"/>
        /// </param>
        public void SetTextPositioning(IOcrReader.TextPositioning positioning) {
            textPositioning = positioning;
        }

        /// <returns>
        /// 
        /// <see cref="TextPositioning"/>
        /// </returns>
        public IOcrReader.TextPositioning GetTextPositioning() {
            return textPositioning;
        }

        /// <summary>
        /// Reads data from the provided input image file and
        /// returns retrieved data as a string.
        /// </summary>
        /// <param name="input">
        /// 
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="outputFormat">
        /// 
        /// <see cref="OutputFormat"/>
        /// "txt" output format:
        /// tesseract performs ocr and returns output in txt format
        /// "hocr" output format:
        /// tesseract performs ocr and returns output in hocr format,
        /// then result text is extracted
        /// </param>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
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
        /// Reads data from the provided input image file and returns
        /// retrieved data in the following format:
        /// Map<Integer, List&lt;textinfo>&gt;:
        /// key: number of page,
        /// value: list of
        /// <see cref="TextInfo"/>
        /// objects where each list element
        /// Map.Entry<String, List&lt;float>&gt; contains word or line as a key
        /// and its 4 coordinates(bbox) as a values.
        /// </summary>
        /// <param name="input">
        /// 
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <returns>Map<Integer, List&lt;textinfo>&gt;</returns>
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

        /// <summary>Reads data from the provided input image file.</summary>
        /// <param name="input">
        /// 
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <param name="outputFormat">
        /// 
        /// <see cref="OutputFormat"/>
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
                LogManager.GetLogger(GetType()).Error(MessageFormatUtil.Format(LogMessageConstant.CANNOT_OCR_INPUT_FILE, e
                    .Message));
            }
            IDictionary<String, IDictionary<int, IList<TextInfo>>> result = new LinkedDictionary<String, IDictionary<int
                , IList<TextInfo>>>();
            result.Put(data.ToString(), imageData);
            return result;
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
        /// 
        /// <see cref="System.String"/>
        /// </param>
        /// <param name="userWords">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
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
                    LogManager.GetLogger(GetType()).Warn(MessageFormatUtil.Format(LogMessageConstant.CANNOT_USE_USER_WORDS, e.
                        Message));
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
        /// 
        /// <see cref="System.String"/>
        /// </param>
        /// <param name="inputStream">
        /// 
        /// <see cref="System.IO.Stream"/>
        /// </param>
        public virtual void SetUserWords(String language, Stream inputStream) {
            String userWordsFileName = TesseractUtil.GetTempDir() + System.IO.Path.DirectorySeparatorChar + language +
                 "." + DEFAULT_USER_WORDS_SUFFIX;
            if (!GetLanguagesAsList().Contains(language)) {
                if ("eng".Equals(language.ToLowerInvariant())) {
                    IList<String> languagesList = GetLanguagesAsList();
                    languagesList.Add(language);
                    SetLanguages(languagesList);
                }
                else {
                    throw new OCRException(OCRException.LANGUAGE_IS_NOT_IN_THE_LIST).SetMessageParams(language);
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
                LogManager.GetLogger(GetType()).Warn(MessageFormatUtil.Format(LogMessageConstant.CANNOT_USE_USER_WORDS, e.
                    Message));
            }
        }

        /// <summary>Return path to the user words file if exists, otherwise null.</summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// </returns>
        public String GetUserWordsFilePath() {
            return userWordsFile;
        }

        /// <summary>Get path to provided tess data directory or return default one.</summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// </returns>
        internal virtual String GetTessData() {
            if (GetPathToTessData() != null && !String.IsNullOrEmpty(GetPathToTessData())) {
                return GetPathToTessData();
            }
            else {
                throw new OCRException(OCRException.CANNOT_FIND_PATH_TO_TESSDATA);
            }
        }

        /// <summary>
        /// Return 'true' if current OS is windows
        /// otherwise 'false'.
        /// </summary>
        /// <returns>boolean</returns>
        public virtual bool IsWindows() {
            return GetOsType().ToLowerInvariant().Contains("win");
        }

        /// <summary>Check type of current OS and return it (mac, win, linux).</summary>
        /// <returns>
        /// 
        /// <see cref="System.String"/>
        /// </returns>
        public virtual String IdentifyOSType() {
            String os = Environment.GetEnvironmentVariable("os.name") == null ? Environment.GetEnvironmentVariable("OS"
                ) : Environment.GetEnvironmentVariable("os.name");
            return os.ToLowerInvariant();
        }

        /// <summary>
        /// Validate provided languages and
        /// check if they exist in provided tess data directory.
        /// </summary>
        /// <param name="languagesList">
        /// 
        /// <see cref="System.Collections.IList{E}"/>
        /// </param>
        public virtual void ValidateLanguages(IList<String> languagesList) {
            String suffix = ".traineddata";
            if (languagesList.Count == 0) {
                if (!new FileInfo(GetTessData() + System.IO.Path.DirectorySeparatorChar + "eng" + suffix).Exists) {
                    throw new OCRException(OCRException.INCORRECT_LANGUAGE).SetMessageParams("eng" + suffix, GetTessData());
                }
            }
            else {
                foreach (String lang in languagesList) {
                    if (!new FileInfo(GetTessData() + System.IO.Path.DirectorySeparatorChar + lang + suffix).Exists) {
                        throw new OCRException(OCRException.INCORRECT_LANGUAGE).SetMessageParams(lang + suffix, GetTessData());
                    }
                }
            }
        }

        /// <summary>Create temporary file with given extension.</summary>
        /// <param name="extension">
        /// 
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// 
        /// <see cref="System.IO.FileInfo"/>
        /// </returns>
        private FileInfo CreateTempFile(String extension) {
            String tmpFileName = TesseractUtil.GetTempDir() + Guid.NewGuid().ToString() + extension;
            return new FileInfo(tmpFileName);
        }
    }
}
