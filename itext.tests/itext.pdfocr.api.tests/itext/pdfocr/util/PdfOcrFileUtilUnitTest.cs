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
using System.Collections.Generic;
using System.IO;
using iText.Pdfocr.Helpers;
using iText.Pdfocr.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfocr.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class PdfOcrFileUtilUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void WriteToTextFileTest() {
            FileInfo file = new FileInfo(PdfHelper.GetImagesTestDirectory() + "writeToTextFileTest.txt");
            NUnit.Framework.Assert.DoesNotThrow(() => PdfOcrFileUtil.WriteToTextFile(file.FullName, "some text"));
            file.Delete();
        }

        [NUnit.Framework.Test]
        public virtual void WriteToTextFileExceptionTest() {
            FileInfo nonExists = new FileInfo(PdfHelper.GetImagesTestDirectory());
            NUnit.Framework.Assert.Catch(typeof(Exception), () => PdfOcrFileUtil.WriteToTextFile(nonExists.FullName, "some text"
                ));
        }

        [NUnit.Framework.Test]
        public virtual void WriteToStreamTest() {
            FileInfo file = new FileInfo(PdfHelper.GetImagesTestDirectory() + "writeToStreamTest.txt");
            NUnit.Framework.Assert.DoesNotThrow(() => PdfOcrFileUtil.WriteToStream(PdfOcrFileUtil.ConvertToOutputStream
                (file), "text"));
            file.Delete();
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfOcrLogMessageConstant.CANNOT_OPEN_OUTPUT_STREAM, Ignore = true)]
        public virtual void ConvertToOutputStreamExceptionTest() {
            FileInfo nonExists = new FileInfo(PdfHelper.GetImagesTestDirectory());
            NUnit.Framework.Assert.Catch(typeof(Exception), () => PdfOcrFileUtil.ConvertToOutputStream(nonExists).Dispose
                ());
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfOcrLogMessageConstant.CANNOT_OPEN_INPUT_STREAM, Ignore = true)]
        public virtual void ConvertToInputStreamsExceptionTest() {
            FileInfo nonExists = new FileInfo(PdfHelper.GetImagesTestDirectory() + "nonExistsFile");
            IList<FileInfo> list = new List<FileInfo>();
            list.Add(nonExists);
            NUnit.Framework.Assert.Catch(typeof(Exception), () => PdfOcrFileUtil.ConvertToInputStreams(list));
        }
    }
}
