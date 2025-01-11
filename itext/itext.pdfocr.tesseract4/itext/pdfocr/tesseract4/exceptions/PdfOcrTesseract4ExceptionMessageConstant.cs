/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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

namespace iText.Pdfocr.Tesseract4.Exceptions {
    /// <summary>Class that bundles all the error message templates as constants.</summary>
    public class PdfOcrTesseract4ExceptionMessageConstant {
        public const String INCORRECT_INPUT_IMAGE_FORMAT = "{0} format is not supported.";

        public const String INCORRECT_LANGUAGE = "{0} does not exist in {1}";

        public const String LANGUAGE_IS_NOT_IN_THE_LIST = "Provided list of languages doesn't contain {0} language";

        public const String CANNOT_READ_PROVIDED_IMAGE = "Cannot read input image {0}";

        public const String CANNOT_WRITE_TO_FILE = "Cannot write to file {0}: {1}";

        public const String TESSERACT_FAILED = "Tesseract failed. Please check provided parameters";

        public const String TESSERACT_LIB_NOT_INSTALLED = "Tesseract failed. Please ensure you have tesseract library installed";

        public const String TESSERACT_LIB_NOT_INSTALLED_WIN = "Tesseract failed. Please ensure you have latest Visual C++ Redistributable installed";

        public const String TESSERACT_NOT_FOUND = "Tesseract failed. Please check that tesseract is installed and provided path to "
             + "tesseract executable directory is correct";

        public const String CANNOT_FIND_PATH_TO_TESSERACT_EXECUTABLE = "Cannot find path to tesseract executable.";

        public const String PATH_TO_TESS_DATA_DIRECTORY_IS_INVALID = "Provided path to tess data directory does not exist or it is an invalid directory";

        public const String PATH_TO_TESS_DATA_IS_NOT_SET = "Path to tess data directory cannot be null and must be set to a valid directory";

        public const String PATH_TO_TESS_DATA_DIRECTORY_CONTAINS_NON_ASCII_CHARACTERS = "Path to tess data should contain only ASCII characters";

        private PdfOcrTesseract4ExceptionMessageConstant() {
        }
        //Private constructor will prevent the instantiation of this class directly
    }
}
