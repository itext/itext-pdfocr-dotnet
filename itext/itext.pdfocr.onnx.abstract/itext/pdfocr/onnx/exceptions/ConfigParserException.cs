/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Commons.Utils;
using iText.Pdfocr.Exceptions;

namespace iText.Pdfocr.Onnx.Exceptions {
    /// <summary>Exception class for exceptions during configuration file parsing.</summary>
    public class ConfigParserException : PdfOcrException {
        /// <summary>
        /// Creates new
        /// <see cref="ConfigParserException"/>
        /// instance.
        /// </summary>
        /// <param name="message">exception message</param>
        protected internal ConfigParserException(String message)
            : base(message) {
        }

        /// <summary>
        /// Creates an exception for cases, when an unexpected value was found in
        /// a configuration file mapping.
        /// </summary>
        /// <param name="key">key under which the value is located</param>
        /// <returns>the created exception</returns>
        public static iText.Pdfocr.Onnx.Exceptions.ConfigParserException UnexpectedValueForKey(String key) {
            return new iText.Pdfocr.Onnx.Exceptions.ConfigParserException(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant
                .UNEXPECTED_VALUE_FOR_CONFIG_KEY, key));
        }

        /// <summary>
        /// Creates an exception for cases, when an unexpected key was found in
        /// a configuration file mapping.
        /// </summary>
        /// <param name="key">key which was found</param>
        /// <returns>the created exception</returns>
        public static iText.Pdfocr.Onnx.Exceptions.ConfigParserException UnexpectedKey(String key) {
            return new iText.Pdfocr.Onnx.Exceptions.ConfigParserException(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant
                .UNEXPECTED_CONFIG_KEY, key));
        }
    }
}
