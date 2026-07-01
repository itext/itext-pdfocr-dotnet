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
using iText.IO.Source;
using iText.Test;

namespace iText.Pdfocr.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class ByteArrayStreamUtilUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ByteArrayInputStreamReadTest() {
            byte[] data = ByteUtils.GetIsoBytes("Hello Test");
            MemoryStream bis = ByteArrayStreamUtil.CreateByteArrayInputStream(new MemoryStream(data));
            byte[] buffer = new byte[8];
            byte[] secondBuffer = new byte[8];
            bis.JRead(buffer, 0, buffer.Length);
            bis.Reset();
            bis.JRead(secondBuffer, 0, secondBuffer.Length);
            NUnit.Framework.Assert.AreEqual(secondBuffer, buffer);
        }
    }
}
