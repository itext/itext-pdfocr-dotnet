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
using Common.Logging;
using iText.IO.Util;
using iText.Pdfocr;

namespace iText.Pdfocr.Tesseract4 {
    /// <summary>
    /// Properties that will be used by the
    /// <see cref="iText.Pdfocr.IOcrEngine"/>.
    /// </summary>
    public class Tesseract4OcrEngineProperties : OcrEngineProperties {
        /// <summary>Default suffix for user-word file.</summary>
        /// <remarks>
        /// Default suffix for user-word file.
        /// (e.g. name: 'eng.user-words')
        /// </remarks>
        internal const String DEFAULT_USER_WORDS_SUFFIX = "user-words";

        /// <summary>Default language for OCR.</summary>
        private const String DEFAULT_LANGUAGE = "eng";

        /// <summary>Path to directory with tess data.</summary>
        private FileInfo tessDataDir;

        /// <summary>Page Segmentation Mode.</summary>
        private int? pageSegMode;

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
        private TextPositioning textPositioning = TextPositioning.BY_LINES;

        /// <summary>Path to the file containing user words.</summary>
        /// <remarks>
        /// Path to the file containing user words.
        /// Each word should be on a new line,
        /// file should end with a newline character.
        /// </remarks>
        private String pathToUserWordsFile = null;

        /// <summary>
        /// Creates a new
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// instance.
        /// </summary>
        public Tesseract4OcrEngineProperties() {
        }

        /// <summary>
        /// Creates a new
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// instance
        /// based on another
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// instance (copy
        /// constructor).
        /// </summary>
        /// <param name="other">
        /// the other
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// instance
        /// </param>
        public Tesseract4OcrEngineProperties(iText.Pdfocr.Tesseract4.Tesseract4OcrEngineProperties other)
            : base(other) {
            this.tessDataDir = other.tessDataDir;
            this.pageSegMode = other.pageSegMode;
            this.preprocessingImages = other.preprocessingImages;
            this.textPositioning = other.textPositioning;
            this.pathToUserWordsFile = other.pathToUserWordsFile;
        }

        /// <summary>Gets default language for ocr.</summary>
        /// <returns>default language - "eng"</returns>
        public String GetDefaultLanguage() {
            return DEFAULT_LANGUAGE;
        }

        /// <summary>Gets default user words suffix.</summary>
        /// <returns>default suffix for user words files</returns>
        public String GetDefaultUserWordsSuffix() {
            return DEFAULT_USER_WORDS_SUFFIX;
        }

        /// <summary>Gets path to directory with tess data.</summary>
        /// <returns>path to directory with tess data</returns>
        public FileInfo GetPathToTessData() {
            return tessDataDir;
        }

        /// <summary>Sets path to directory with tess data.</summary>
        /// <param name="tessData">
        /// path to train directory as
        /// <see cref="System.IO.FileInfo"/>
        /// </param>
        /// <returns>
        /// the
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// instance
        /// </returns>
        public iText.Pdfocr.Tesseract4.Tesseract4OcrEngineProperties SetPathToTessData(FileInfo tessData) {
            if (tessData == null || !FileUtil.DirectoryExists(tessData.FullName)) {
                throw new Tesseract4OcrException(Tesseract4OcrException.PATH_TO_TESS_DATA_DIRECTORY_IS_INVALID);
            }
            this.tessDataDir = tessData;
            return this;
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
        /// <returns>
        /// the
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// instance
        /// </returns>
        public iText.Pdfocr.Tesseract4.Tesseract4OcrEngineProperties SetPageSegMode(int? mode) {
            pageSegMode = mode;
            return this;
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
        /// <returns>
        /// the
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// instance
        /// </returns>
        public iText.Pdfocr.Tesseract4.Tesseract4OcrEngineProperties SetPreprocessingImages(bool preprocess) {
            preprocessingImages = preprocess;
            return this;
        }

        /// <summary>
        /// Defines the way text is retrieved from tesseract output using
        /// <see cref="TextPositioning"/>.
        /// </summary>
        /// <returns>the way text is retrieved</returns>
        public TextPositioning GetTextPositioning() {
            return textPositioning;
        }

        /// <summary>
        /// Defines the way text is retrieved from tesseract output
        /// using
        /// <see cref="TextPositioning"/>.
        /// </summary>
        /// <param name="positioning">the way text is retrieved</param>
        /// <returns>
        /// the
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// instance
        /// </returns>
        public iText.Pdfocr.Tesseract4.Tesseract4OcrEngineProperties SetTextPositioning(TextPositioning positioning
            ) {
            textPositioning = positioning;
            return this;
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
        /// <returns>
        /// the
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Pdfocr.Tesseract4.Tesseract4OcrEngineProperties SetUserWords(String language, IList<String
            > userWords) {
            SetPathToUserWordsFile(null);
            if (userWords != null && userWords.Count > 0) {
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
                    LogManager.GetLogger(GetType()).Warn(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_USE_USER_WORDS
                        , e.Message));
                }
            }
            return this;
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
        /// <returns>
        /// the
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// instance
        /// </returns>
        public virtual iText.Pdfocr.Tesseract4.Tesseract4OcrEngineProperties SetUserWords(String language, Stream 
            inputStream) {
            SetPathToUserWordsFile(null);
            String userWordsFileName = TesseractOcrUtil.GetTempDir() + System.IO.Path.DirectorySeparatorChar + language
                 + "." + DEFAULT_USER_WORDS_SUFFIX;
            if (!GetLanguages().Contains(language)) {
                if (DEFAULT_LANGUAGE.Equals(language.ToLowerInvariant())) {
                    IList<String> languagesList = GetLanguages();
                    languagesList.Add(language);
                    SetLanguages(languagesList);
                }
                else {
                    throw new Tesseract4OcrException(Tesseract4OcrException.LANGUAGE_IS_NOT_IN_THE_LIST).SetMessageParams(language
                        );
                }
            }
            try {
                using (StreamWriter writer = new StreamWriter(userWordsFileName)) {
                    TextReader reader = new StreamReader(inputStream, System.Text.Encoding.UTF8);
                    int data;
                    while ((data = reader.Read()) != -1) {
                        writer.Write(data);
                    }
                    writer.Write(Environment.NewLine);
                    SetPathToUserWordsFile(userWordsFileName);
                }
            }
            catch (System.IO.IOException e) {
                SetPathToUserWordsFile(null);
                LogManager.GetLogger(GetType()).Warn(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_USE_USER_WORDS
                    , e.Message));
            }
            return this;
        }

        /// <summary>Returns path to the user words file.</summary>
        /// <returns>
        /// path to user words file as
        /// <see cref="System.String"/>
        /// if it
        /// exists, otherwise - null
        /// </returns>
        public String GetPathToUserWordsFile() {
            return pathToUserWordsFile;
        }

        /// <summary>Sets path to the user words file.</summary>
        /// <param name="pathToUserWordsFile">
        /// path to user words file
        /// as
        /// <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// the
        /// <see cref="Tesseract4OcrEngineProperties"/>
        /// instance
        /// </returns>
        public iText.Pdfocr.Tesseract4.Tesseract4OcrEngineProperties SetPathToUserWordsFile(String pathToUserWordsFile
            ) {
            this.pathToUserWordsFile = pathToUserWordsFile;
            return this;
        }
    }
}
