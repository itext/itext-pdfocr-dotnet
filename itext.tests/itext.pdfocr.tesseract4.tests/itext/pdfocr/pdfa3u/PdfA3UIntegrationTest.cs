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
        internal Tesseract4OcrEngine tesseractReader;

        public PdfA3UIntegrationTest(AbstractIntegrationTest.ReaderType type) {
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.Test]
        public virtual void ComparePdfA3uCMYKColorSpaceSpanishJPG() {
            String testName = "comparePdfA3uCMYKColorSpaceSpanishJPG";
            String filename = "numbers_01";
            String expectedPdfPath = testDocumentsDirectory + filename + "_a3u.pdf";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + "_a3u.pdf";
            try {
                PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader);
                tesseractReader.SetTesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties().SetTextPositioning
                    (TextPositioning.BY_WORDS));
                NUnit.Framework.Assert.AreEqual(tesseractReader, pdfRenderer.GetOcrReader());
                pdfRenderer.SetOcrReader(tesseractReader);
                PdfDocument doc = pdfRenderer.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(testImagesDirectory
                     + filename + ".jpg")), GetPdfWriter(resultPdfPath), GetCMYKPdfOutputIntent());
                NUnit.Framework.Assert.IsNotNull(doc);
                doc.Close();
                new CompareTool().CompareByContent(expectedPdfPath, resultPdfPath, testDocumentsDirectory, "diff_");
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
            String expectedPdfPath = testDocumentsDirectory + filename + "_a3u.pdf";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + "_a3u.pdf";
            Tesseract4OcrEngineProperties properties = new Tesseract4OcrEngineProperties(tesseractReader.GetTesseract4OcrEngineProperties
                ());
            properties.SetPathToTessData(langTessDataDirectory);
            properties.SetLanguages(JavaCollectionsUtil.SingletonList<String>("spa"));
            tesseractReader.SetTesseract4OcrEngineProperties(properties);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, new OcrPdfCreatorProperties().SetTextColor(DeviceRgb
                .BLACK));
            PdfDocument doc = pdfRenderer.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(testImagesDirectory
                 + filename + ".jpg")), GetPdfWriter(resultPdfPath), GetRGBPdfOutputIntent());
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            new CompareTool().CompareByContent(expectedPdfPath, resultPdfPath, testDocumentsDirectory, "diff_");
        }
    }
}
