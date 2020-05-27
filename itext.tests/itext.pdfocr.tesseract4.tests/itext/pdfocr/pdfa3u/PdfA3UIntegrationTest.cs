using System;
using System.IO;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Tesseract4;

namespace iText.Pdfocr.Pdfa3u {
    public abstract class PdfA3UIntegrationTest : AbstractIntegrationTest {
        // path to default cmyk color profile
        private static readonly String DEFAULT_CMYK_COLOR_PROFILE_PATH = TEST_DIRECTORY + "profiles/CoatedFOGRA27.icc";

        // path to default rgb color profile
        private static readonly String DEFAULT_RGB_COLOR_PROFILE_PATH = TEST_DIRECTORY + "profiles/sRGB_CS_profile.icm";

        internal AbstractTesseract4OcrEngine tesseractReader;

        public PdfA3UIntegrationTest(AbstractIntegrationTest.ReaderType type) {
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.Test]
        public virtual void ComparePdfA3uCMYKColorSpaceSpanishJPG() {
            String testName = "comparePdfA3uCMYKColorSpaceSpanishJPG";
            String filename = "numbers_01";
            String expectedPdfPath = TEST_DOCUMENTS_DIRECTORY + filename + "_a3u.pdf";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + "_a3u.pdf";
            try {
                OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader);
                tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetTextPositioning
                    (TextPositioning.BY_WORDS));
                NUnit.Framework.Assert.AreEqual(tesseractReader, ocrPdfCreator.GetOcrEngine());
                ocrPdfCreator.SetOcrEngine(tesseractReader);
                PdfDocument doc = ocrPdfCreator.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(TEST_IMAGES_DIRECTORY
                     + filename + ".jpg")), GetPdfWriter(resultPdfPath), GetCMYKPdfOutputIntent());
                NUnit.Framework.Assert.IsNotNull(doc);
                doc.Close();
                new CompareTool().CompareByContent(expectedPdfPath, resultPdfPath, TEST_DOCUMENTS_DIRECTORY, "diff_");
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
            properties.SetPathToTessData(LANG_TESS_DATA_DIRECTORY);
            properties.SetLanguages(JavaCollectionsUtil.SingletonList<String>("spa"));
            tesseractReader.SetTesseract4OcrEngineProperties(properties);
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader, new OcrPdfCreatorProperties().SetTextColor
                (DeviceRgb.BLACK));
            PdfDocument doc = ocrPdfCreator.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(TEST_IMAGES_DIRECTORY
                 + filename + ".jpg")), GetPdfWriter(resultPdfPath), GetRGBPdfOutputIntent());
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            new CompareTool().CompareByContent(expectedPdfPath, resultPdfPath, TEST_DOCUMENTS_DIRECTORY, "diff_");
        }

        /// <summary>Creates pdf cmyk output intent for tests.</summary>
        protected internal virtual PdfOutputIntent GetCMYKPdfOutputIntent() {
            Stream @is = new FileStream(DEFAULT_CMYK_COLOR_PROFILE_PATH, FileMode.Open, FileAccess.Read);
            return new PdfOutputIntent("Custom", "", "http://www.color.org", "Coated FOGRA27 (ISO 12647 - 2:2004)", @is
                );
        }

        /// <summary>Creates pdf rgb output intent for tests.</summary>
        protected internal virtual PdfOutputIntent GetRGBPdfOutputIntent() {
            Stream @is = new FileStream(DEFAULT_RGB_COLOR_PROFILE_PATH, FileMode.Open, FileAccess.Read);
            return new PdfOutputIntent("", "", "", "sRGB IEC61966-2.1", @is);
        }
    }
}
