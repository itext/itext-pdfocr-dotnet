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
using System.IO;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Logs;

namespace iText.Pdfocr.Util {
    /// <summary>This file is a helper class for internal usage only.</summary>
    /// <remarks>
    /// This file is a helper class for internal usage only.
    /// Be aware that its API and functionality may be changed in future.
    /// </remarks>
    public sealed class ByteArrayStreamUtil {
        private static readonly ILogger LOGGER = ITextLogManager.GetLogger(typeof(iText.Pdfocr.Util.ByteArrayStreamUtil
            ));

        private ByteArrayStreamUtil() {
        }

        // do nothing
        /// <summary>
        /// Wraps
        /// <see cref="System.IO.Stream"/>
        /// into
        /// <see cref="System.IO.MemoryStream"/>.
        /// </summary>
        /// <param name="input">
        /// 
        /// <see cref="System.IO.Stream"/>
        /// to be wrapped
        /// </param>
        /// <returns>
        /// new
        /// <see cref="System.IO.MemoryStream"/>
        /// backed by provided
        /// <see cref="System.IO.Stream"/>
        /// </returns>
        public static MemoryStream CreateByteArrayInputStream(Stream input) {
            if (input is MemoryStream) {
                return (MemoryStream)input;
            }
            try {
                MemoryStream buffer = new MemoryStream();
                byte[] data = new byte[8192];
                int nRead;
                while ((nRead = input.JRead(data, 0, data.Length)) != -1) {
                    buffer.Write(data, 0, nRead);
                }
                return new MemoryStream(buffer.ToArray());
            }
            catch (System.IO.IOException e) {
                LOGGER.LogError(MessageFormatUtil.Format(PdfOcrLogMessageConstant.CANNOT_READ_INPUT_STREAM, e.Message));
                throw new PdfOcrInputException(PdfOcrExceptionMessageConstant.CANNOT_READ_INPUT_STREAM, e);
            }
        }
    }
}
