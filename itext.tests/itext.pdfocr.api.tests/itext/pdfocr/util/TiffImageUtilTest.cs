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
using System.Collections.Generic;
using System.IO;
using iText.Pdfocr.Helpers;
using iText.Pdfocr.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfocr.Util {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TiffImageUtilTest : ExtendedITextTest {
        [LogMessage(PdfOcrLogMessageConstant.CANNOT_RETRIEVE_PAGES_FROM_IMAGE)]
        [NUnit.Framework.Test]
        public virtual void GetAllImagesMissingFileTest() {
            String path = PdfHelper.GetImagesTestDirectory() + "missing.tiff";
            IList<IronSoftware.Drawing.AnyBitmap> images = TiffImageUtil.GetAllImages(new FileInfo(path));
            NUnit.Framework.Assert.AreEqual(0, images.Count);
        }

        [NUnit.Framework.Test]
        public virtual void GetAllImagesTest() {
            String path = PdfHelper.GetImagesTestDirectory() + "multipage.tiff";
            IList<IronSoftware.Drawing.AnyBitmap> images = TiffImageUtil.GetAllImages(new FileInfo(path));
            NUnit.Framework.Assert.AreEqual(9, images.Count);
        }

        [NUnit.Framework.Test]
        public virtual void IsTiffImageTest() {
            String path = PdfHelper.GetImagesTestDirectory() + "thai.PNG";
            NUnit.Framework.Assert.IsFalse(TiffImageUtil.IsTiffImage(new FileInfo(path)));
            path = PdfHelper.GetImagesTestDirectory() + "single7x5cm.tif";
            NUnit.Framework.Assert.IsTrue(TiffImageUtil.IsTiffImage(new FileInfo(path)));
        }
    }
}
