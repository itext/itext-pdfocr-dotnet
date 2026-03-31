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
using System.IO;
using iText.Commons.Utils;
using iText.Pdfocr.Exceptions;

namespace iText.Pdfocr.Util {
    /// <summary>Utility class for working with files.</summary>
    public sealed class PdfOcrFileUtil {
        private PdfOcrFileUtil() {
        }

        // do nothing
        /// <summary>
        /// Writes provided
        /// <see cref="System.String"/>
        /// to text file using provided path.
        /// </summary>
        /// <param name="path">
        /// path as
        /// <see cref="System.String"/>
        /// to file to be created
        /// </param>
        /// <param name="data">
        /// text data in required format as
        /// <see cref="System.String"/>
        /// </param>
        public static void WriteToTextFile(String path, String data) {
            try {
                using (TextWriter writer = new StreamWriter(FileUtil.GetFileOutputStream(path), System.Text.Encoding.UTF8)
                    ) {
                    writer.Write(data);
                }
            }
            catch (System.IO.IOException e) {
                throw new PdfOcrException(MessageFormatUtil.Format(PdfOcrExceptionMessageConstant.CANNOT_WRITE_TO_FILE, path
                    , e.Message), e);
            }
        }

        /// <summary>Gets path to temp file in current system temporary directory.</summary>
        /// <param name="name">temp file name</param>
        /// <param name="extension">temp file extension</param>
        /// <returns>path to temp file in the system temporary directory</returns>
        public static String GetTempFilePath(String name, String extension) {
            return System.IO.Path.GetTempPath() + name + extension;
        }
    }
}
