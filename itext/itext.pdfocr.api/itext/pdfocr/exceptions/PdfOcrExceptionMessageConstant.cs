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

namespace iText.Pdfocr.Exceptions {
    /// <summary>Class that bundles all the exception message templates as constants.</summary>
    public class PdfOcrExceptionMessageConstant {
        public const String CANNOT_READ_INPUT_IMAGE = "Cannot read input image";

        public const String CANNOT_RESOLVE_PROVIDED_FONTS = "Cannot resolve any of provided fonts. Please check provided FontProvider.";

        public const String CANNOT_CREATE_PDF_DOCUMENT = "Cannot create PDF document: {0}";

        public const String STATISTICS_EVENT_TYPE_CANT_BE_NULL = "Statistics event type can't be null";

        public const String STATISTICS_EVENT_TYPE_IS_NOT_DETECTED = "Statistics event type is not detected.";

        private PdfOcrExceptionMessageConstant() {
        }
        //Private constructor will prevent the instantiation of this class directly
    }
}
