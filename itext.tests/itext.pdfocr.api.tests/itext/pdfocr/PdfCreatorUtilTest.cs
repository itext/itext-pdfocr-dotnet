/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.IO.Image;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Helpers;
using iText.Pdfocr.Logs;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfocr {
    public class PdfCreatorUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetImageDataFromValidSinglePagedTiffTest() {
            FileInfo image = new FileInfo(PdfHelper.GetImagesTestDirectory() + "single7x5cm.tif");
            IList<ImageData> images = PdfCreatorUtil.GetImageData(image, null);
            NUnit.Framework.Assert.AreEqual(1, images.Count);
            ImageData imageDate = images[0];
            NUnit.Framework.Assert.IsNotNull(imageDate);
            NUnit.Framework.Assert.IsTrue(imageDate is TiffImageData);
            NUnit.Framework.Assert.AreEqual(ImageType.TIFF, imageDate.GetOriginalType());
        }

        [NUnit.Framework.Test]
        public virtual void GetImageDataFromValidMultiPagedTiffTest() {
            FileInfo image = new FileInfo(PdfHelper.GetImagesTestDirectory() + "multipage.tiff");
            IList<ImageData> images = PdfCreatorUtil.GetImageData(image, null);
            NUnit.Framework.Assert.AreEqual(9, images.Count);
            foreach (ImageData imageDate in images) {
                NUnit.Framework.Assert.IsNotNull(imageDate);
                NUnit.Framework.Assert.IsTrue(imageDate is TiffImageData);
                NUnit.Framework.Assert.AreEqual(ImageType.TIFF, imageDate.GetOriginalType());
            }
        }

        [NUnit.Framework.Test]
        public virtual void GetImageDataFromValidNotTiffTest() {
            FileInfo image = new FileInfo(PdfHelper.GetImagesTestDirectory() + "numbers_01.jpg");
            IList<ImageData> images = PdfCreatorUtil.GetImageData(image, null);
            NUnit.Framework.Assert.AreEqual(1, images.Count);
            ImageData imageDate = images[0];
            NUnit.Framework.Assert.IsNotNull(imageDate);
            NUnit.Framework.Assert.IsTrue(imageDate is JpegImageData);
            NUnit.Framework.Assert.AreEqual(ImageType.JPEG, imageDate.GetOriginalType());
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfOcrLogMessageConstant.CANNOT_READ_INPUT_IMAGE)]
        public virtual void GetImageDataFromNotExistingImageTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCreatorUtil.GetImageData(new FileInfo("no such path"), null);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfOcrInputException>())
;
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfOcrLogMessageConstant.CANNOT_READ_INPUT_IMAGE)]
        public virtual void GetImageDataFromInvalidImageTest() {
            NUnit.Framework.Assert.That(() =>  {
                PdfCreatorUtil.GetImageData(new FileInfo(PdfHelper.GetImagesTestDirectory() + "corrupted.jpg"), null);
            }
            , NUnit.Framework.Throws.InstanceOf<PdfOcrInputException>().With.Message.EqualTo(MessageFormatUtil.Format(PdfOcrExceptionMessageConstant.CANNOT_READ_INPUT_IMAGE)))
;
        }
    }
}
