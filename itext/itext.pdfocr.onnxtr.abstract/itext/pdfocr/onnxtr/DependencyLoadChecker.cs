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
using System.Runtime.InteropServices;
using iText.Commons.Utils;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Onnxtr.Exceptions;

namespace iText.Pdfocr.Onnxtr {
//\cond DO_NOT_DOCUMENT
    /// <summary>Internal helper class to construct a friendlier error in case some dependency couldn’t be loaded.
    ///     </summary>
    /// <remarks>
    /// Internal helper class to construct a friendlier error in case some dependency couldn’t be loaded.
    /// <para />
    /// NOTE: for internal usage only. Be aware that its API and functionality may be changed in the future.
    /// </remarks>
    internal sealed class DependencyLoadChecker {
        private DependencyLoadChecker() {
            // Private constructor will prevent the instantiation of this class directly.
        }

        /// <summary>
        /// Processes the exception or error: checks if exception is related to some dependency that couldn’t be loaded and
        /// in that case constructs exception with a friendlier error message, otherwise, throws exception as is.
        /// </summary>
        /// <param name="throwable">exception or error to process</param>
        public static void ProcessException(Exception throwable) {
            String throwableMessage = throwable.Message;
            bool isOnnxRuntime = throwable is TypeInitializationException && 
                                 throwableMessage.Contains("Microsoft.ML.OnnxRuntime.NativeMethods");
            if (isOnnxRuntime) {
                String message = GetOnnxRuntimeError();
                throw new PdfOcrException(message, throwable);
            }
        }

        private static String GetOnnxRuntimeError() {
            String message = PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_LOAD_ONNXRUNTIME +
                             "\nDouble check that correct RuntimeIdentifier is specified.";
            if (IsWindows()) {
                message += " Also a possible cause on Windows is that the latest version of the VC++ redistributable " +
                           "is missing and needs to be installed.";
            }
            return message;
        }

        /// <summary>Checks current OS type.</summary>
        /// <returns>
        /// boolean
        /// <see langword="true"/>
        /// is current OS is Windows, otherwise -
        /// <see langword="false"/>
        /// </returns>
        private static bool IsWindows() {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }
    }
//\endcond
}
