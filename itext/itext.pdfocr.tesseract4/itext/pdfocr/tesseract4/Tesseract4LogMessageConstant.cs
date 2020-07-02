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

namespace iText.Pdfocr.Tesseract4 {
    public class Tesseract4LogMessageConstant {
        public const String TESSERACT_FAILED = "Tesseract failed: {0}";

        public const String COMMAND_FAILED = "Command failed: {0}";

        public const String CANNOT_READ_FILE = "Cannot read file {0}: {1}";

        public const String CANNOT_OCR_INPUT_FILE = "Cannot ocr input file: {1}";

        public const String CANNOT_USE_USER_WORDS = "Cannot use custom user words: {0}";

        public const String CANNOT_RETRIEVE_PAGES_FROM_IMAGE = "Cannot get pages from image {0}: {1}";

        public const String PAGE_NUMBER_IS_INCORRECT = "Provided number of page ({0}) is incorrect for {1}";

        public const String CANNOT_DELETE_FILE = "File {0} cannot be deleted: {1}";

        public const String CANNOT_PROCESS_IMAGE = "Cannot process " + "image: {0}";

        public const String CANNOT_WRITE_TO_FILE = "Cannot write to file {0}: {1}";

        public const String CREATED_TEMPORARY_FILE = "Created temp file {0}";

        public const String CANNOT_CONVERT_IMAGE_TO_GRAYSCALE = "Cannot convert to gray image with depth {0}";

        public const String CANNOT_BINARIZE_IMAGE = "Cannot binarize image with depth {0}";

        public const String CANNOT_CREATE_BUFFERED_IMAGE = "Cannot create a buffered image from the input image: {0}";

        public const String START_OCR_FOR_IMAGES = "Starting ocr for {0} image(s)";

        public const String CANNOT_READ_INPUT_IMAGE = "Cannot read input image {0}";

        public const String CANNOT_GET_TEMPORARY_DIRECTORY = "Cannot get " + "temporary directory: {0}";

        public const String CANNOT_CONVERT_IMAGE_TO_PIX = "Cannot convert image to pix: {0}";

        public const String CANNOT_PARSE_NODE_BBOX = "Cannot parse node BBox, defaults to 0, 0, 0, 0. Node: {0}";

        private Tesseract4LogMessageConstant() {
        }
    }
}
