/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using System.IO;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Metainfo;
using iText.Pdfocr;
using iText.Pdfocr.Helpers;
using iText.Test;

namespace iText.Pdfocr.Events {
    public class EventCountingTest : ExtendedITextTest {
        protected internal static readonly String PROFILE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/profiles/";

        protected internal static readonly String SOURCE_FOLDER = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/events/";

        private IOcrEngine tesseractReader;

        public EventCountingTest() {
            tesseractReader = new CustomOcrEngine();
        }

        [NUnit.Framework.Test]
        public virtual void TestEventCountingPdfEvent() {
            ((CustomOcrEngine)tesseractReader).SetThreadLocalMetaInfo(new TestMetaInfo());
            DoImageToPdfOcr(tesseractReader, GetTestImageFile());
            NUnit.Framework.Assert.IsTrue(((CustomOcrEngine)tesseractReader).GetThreadLocalMetaInfo() is TestMetaInfo);
        }

        [NUnit.Framework.Test]
        public virtual void TestEventCountingPdfAEvent() {
            ((CustomOcrEngine)tesseractReader).SetThreadLocalMetaInfo(new TestMetaInfo());
            DoImageToPdfAOcr(tesseractReader, GetTestImageFile());
            NUnit.Framework.Assert.IsTrue(((CustomOcrEngine)tesseractReader).GetThreadLocalMetaInfo() is TestMetaInfo);
        }

        [NUnit.Framework.Test]
        public virtual void TestEventCountingImageEvent() {
            ((CustomOcrEngine)tesseractReader).SetThreadLocalMetaInfo(new TestMetaInfo());
            DoImageOcr(tesseractReader, GetTestImageFile());
            NUnit.Framework.Assert.IsTrue(((CustomOcrEngine)tesseractReader).GetThreadLocalMetaInfo() is TestMetaInfo);
        }

        private static void DoImageOcr(IOcrEngine tesseractReader, FileInfo imageFile) {
            tesseractReader.DoImageOcr(imageFile);
        }

        private static void DoImageToPdfOcr(IOcrEngine tesseractReader, FileInfo imageFile) {
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader);
            ocrPdfCreator.CreatePdf(JavaUtil.ArraysAsList(imageFile), new PdfWriter(new MemoryStream()));
        }

        private static void DoImageToPdfAOcr(IOcrEngine tesseractReader, FileInfo imageFile) {
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader, new OcrPdfCreatorProperties().SetPdfLang(
                "en-US"));
            Stream @is = null;
            try {
                @is = new FileStream(PROFILE_FOLDER + "sRGB_CS_profile.icm", FileMode.Open, FileAccess.Read);
            }
            catch (FileNotFoundException) {
            }
            // No expected
            PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1"
                , @is);
            ocrPdfCreator.CreatePdfA(JavaUtil.ArraysAsList(imageFile), new PdfWriter(new MemoryStream()), outputIntent
                );
        }

        private static FileInfo GetTestImageFile() {
            String imgPath = SOURCE_FOLDER + "numbers_01.jpg";
            return new FileInfo(imgPath);
        }
    }
}
