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
using iText.Pdfocr;

namespace iText.Pdfocr.Tesseract4 {
    public class Tesseract4OcrException : OcrException {
        public const String TESSERACT_LIB_NOT_INSTALLED_WIN = "Tesseract failed. "
             + "Please ensure you have at least Visual C++ 2015 Redistributable installed";

        public const String INCORRECT_INPUT_IMAGE_FORMAT = "{0} format is not supported.";

        public const String INCORRECT_LANGUAGE = "{0} does not exist in {1}";

        public const String LANGUAGE_IS_NOT_IN_THE_LIST = "Provided list of languages doesn't contain {0} language";

        public const String CANNOT_READ_PROVIDED_IMAGE = "Cannot read input image {0}";

        public const String TESSERACT_FAILED = "Tesseract failed. " + "Please check provided parameters";

        public const String TESSERACT_LIB_NOT_INSTALLED = "Tesseract failed. " + "Please ensure you have tesseract library installed";

        public const String TESSERACT_NOT_FOUND = "Tesseract failed. " + "Please check that tesseract is installed and provided path to "
             + "tesseract executable directory is correct";

        public const String CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE = "Cannot find path to tesseract executable.";

        public const String PATH_TO_TESS_DATA_DIRECTORY_IS_INVALID = "Provided path to tess data directory does not exist or it is "
             + "an invalid directory";

        public const String PATH_TO_TESS_DATA_IS_NOT_SET = "Path to tess data directory cannot be null and must be set "
             + "to a valid directory";

        public const String PATH_TO_TESS_DATA_DIRECTORY_CONTAINS_NON_ASCII_CHARACTERS = "Path to tess data should contain only ASCII characters";

        /// <summary>Creates a new TesseractException.</summary>
        /// <param name="msg">the detail message.</param>
        /// <param name="e">
        /// the cause
        /// (which is saved for later retrieval
        /// by
        /// <see cref="System.Exception.InnerException()"/>
        /// method).
        /// </param>
        public Tesseract4OcrException(String msg, Exception e)
            : base(msg, e) {
        }

        /// <summary>Creates a new TesseractException.</summary>
        /// <param name="msg">the detail message.</param>
        public Tesseract4OcrException(String msg)
            : base(msg) {
        }
    }
}
