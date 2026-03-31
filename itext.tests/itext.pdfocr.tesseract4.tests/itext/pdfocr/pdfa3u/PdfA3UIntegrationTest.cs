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
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4;

namespace iText.Pdfocr.Pdfa3u {
    public abstract class PdfA3UIntegrationTest : IntegrationTestHelper {
        // path to default cmyk color profile
        private static readonly String DEFAULT_CMYK_COLOR_PROFILE_PATH = TEST_DIRECTORY + "profiles/CoatedFOGRA27.icc";

        // path to default rgb color profile
        private static readonly String DEFAULT_RGB_COLOR_PROFILE_PATH = TEST_DIRECTORY + "profiles/sRGB_CS_profile.icm";

//\cond DO_NOT_DOCUMENT
        internal AbstractTesseract4OcrEngine tesseractReader;
//\endcond

        public PdfA3UIntegrationTest(IntegrationTestHelper.ReaderType type) {
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.Test]
        public virtual void ComparePdfA3uCMYKColorSpaceJPG() {
            String testName = "comparePdfA3uCMYKColorSpaceJPG";
            String filename = "numbers_01";
            String expectedPdfPath = TEST_DOCUMENTS_DIRECTORY + filename + "_a3u.pdf";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + "_a3u.pdf";
            try {
                OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
                ocrPdfCreatorProperties.SetPdfLang("en-US");
                ocrPdfCreatorProperties.SetTitle("");
                OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader, ocrPdfCreatorProperties);
                tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetTextPositioning
                    (TextPositioning.BY_WORDS));
                NUnit.Framework.Assert.AreEqual(tesseractReader, ocrPdfCreator.GetOcrEngine());
                ocrPdfCreator.SetOcrEngine(tesseractReader);
                PdfDocument doc = ocrPdfCreator.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(TEST_IMAGES_DIRECTORY
                     + filename + ".jpg")), GetPdfWriter(resultPdfPath), GetCMYKPdfOutputIntent());
                NUnit.Framework.Assert.IsNotNull(doc);
                doc.Close();
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, GetTargetDirectory
                    (), "diff_"));
            }
            finally {
                NUnit.Framework.Assert.AreEqual(TextPositioning.BY_WORDS, tesseractReader.GetTesseract4OcrEngineProperties
                    ().GetTextPositioning());
                tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetTextPositioning
                    (TextPositioning.BY_LINES));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ComparePdfA3uRGBSpanishJPG() {
            String testName = "comparePdfA3uRGBSpanishJPG";
            String filename = "spanish_01";
            String expectedPdfPath = TEST_DOCUMENTS_DIRECTORY + filename + "_a3u.pdf";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + "_a3u.pdf";
            Tesseract4OcrEngineProperties properties = new Tesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties
                ());
            properties.SetPathToTessData(GetTessDataDirectory());
            properties.SetLanguages(JavaCollectionsUtil.SingletonList<String>("spa"));
            tesseractReader.SetTesseract4OcrEngineProperties(properties);
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
            ocrPdfCreatorProperties.SetPdfLang("en-US");
            ocrPdfCreatorProperties.SetTitle("");
            ocrPdfCreatorProperties.SetTextColor(DeviceRgb.BLACK);
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader, ocrPdfCreatorProperties);
            PdfDocument doc = ocrPdfCreator.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(TEST_IMAGES_DIRECTORY
                 + filename + ".jpg")), GetPdfWriter(resultPdfPath), GetRGBPdfOutputIntent());
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, GetTargetDirectory
                (), "diff_"));
        }

        /// <summary>Creates PDF cmyk output intent for tests.</summary>
        protected internal virtual PdfOutputIntent GetCMYKPdfOutputIntent() {
            Stream @is = new FileStream(DEFAULT_CMYK_COLOR_PROFILE_PATH, FileMode.Open, FileAccess.Read);
            return new PdfOutputIntent("Custom", "", "http://www.color.org", "Coated FOGRA27 (ISO 12647 - 2:2004)", @is
                );
        }

        /// <summary>Creates PDF rgb output intent for tests.</summary>
        protected internal virtual PdfOutputIntent GetRGBPdfOutputIntent() {
            Stream @is = new FileStream(DEFAULT_RGB_COLOR_PROFILE_PATH, FileMode.Open, FileAccess.Read);
            return new PdfOutputIntent("", "", "", "sRGB IEC61966-2.1", @is);
        }
    }
}
