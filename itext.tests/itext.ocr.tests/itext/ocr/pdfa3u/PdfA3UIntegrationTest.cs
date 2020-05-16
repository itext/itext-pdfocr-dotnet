using System;
using System.IO;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Utils;
using iText.Ocr;
using iText.Pdfa;
using iText.Test.Attributes;

namespace iText.Ocr.Pdfa3u {
    public abstract class PdfA3UIntegrationTest : AbstractIntegrationTest {
        internal TesseractReader tesseractReader;

        public PdfA3UIntegrationTest(AbstractIntegrationTest.ReaderType type) {
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfA3uWithNullIntent() {
            String testName = "testPdfA3uWithNullIntent";
            String imgPath = testImagesDirectory + "numbers_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "619121";
            String pdfPath = testImagesDirectory + testName + ".pdf";
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetTextColor(DeviceCmyk.BLACK);
            properties.SetScaleMode(ScaleMode.SCALE_TO_FIT);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, properties);
            PdfDocument doc = pdfRenderer.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter(pdfPath
                ), null);
            doc.Close();
            String result = GetTextFromPdfLayer(pdfPath, "Text Layer", 1);
            NUnit.Framework.Assert.AreEqual(expected, result);
        }

        [NUnit.Framework.Test]
        public virtual void TestIncompatibleOutputIntentAndFontColorSpaceException() {
            NUnit.Framework.Assert.That(() =>  {
                String path = testImagesDirectory + "example_01.BMP";
                FileInfo file = new FileInfo(path);
                PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, new OcrPdfCreatorProperties().SetTextColor(DeviceCmyk
                    .BLACK));
                PdfDocument doc = pdfRenderer.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter()
                    , GetRGBPdfOutputIntent());
                doc.Close();
            }
            , NUnit.Framework.Throws.InstanceOf<PdfException>().With.Message.EqualTo(PdfAConformanceException.DEVICECMYK_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT))
;
        }

        [NUnit.Framework.Test]
        public virtual void TestDefaultFontInPdf() {
            String testName = "testDefaultFontInPdf";
            String path = testImagesDirectory + "example_01.BMP";
            String pdfPath = testImagesDirectory + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, new OcrPdfCreatorProperties().SetTextColor(DeviceRgb
                .BLACK));
            PdfDocument doc = pdfRenderer.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter(pdfPath
                ), GetRGBPdfOutputIntent());
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            AbstractIntegrationTest.ExtractionStrategy strategy = new AbstractIntegrationTest.ExtractionStrategy("Text Layer"
                );
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetFirstPage());
            pdfDocument.Close();
            PdfFont font = strategy.GetPdfFont();
            String fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("LiberationSans"));
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
        }

        [NUnit.Framework.Test]
        public virtual void TestCustomFontInPdf() {
            String testName = "testDefaultFontInPdf";
            String imgPath = testImagesDirectory + "numbers_01.jpg";
            String pdfPath = testImagesDirectory + testName + ".pdf";
            FileInfo file = new FileInfo(imgPath);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, new OcrPdfCreatorProperties().SetFontPath(freeSansFontPath
                ));
            PdfDocument doc = pdfRenderer.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter(pdfPath
                ), GetCMYKPdfOutputIntent());
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            AbstractIntegrationTest.ExtractionStrategy strategy = new AbstractIntegrationTest.ExtractionStrategy("Text Layer"
                );
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetFirstPage());
            pdfDocument.Close();
            PdfFont font = strategy.GetPdfFont();
            String fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("FreeSans"));
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
            NUnit.Framework.Assert.AreEqual(freeSansFontPath, pdfRenderer.GetOcrPdfCreatorProperties().GetFontPath());
        }

        [LogMessage(iText.IO.IOException.TypeOfFontIsNotRecognized, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestInvalidCustomFontInPdf() {
            String testName = "testInvalidCustomFontInPdf";
            String path = testImagesDirectory + "numbers_01.jpg";
            String pdfPath = testImagesDirectory + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, new OcrPdfCreatorProperties().SetFontPath(path)
                );
            PdfDocument doc = pdfRenderer.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter(pdfPath
                ), GetCMYKPdfOutputIntent());
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            AbstractIntegrationTest.ExtractionStrategy strategy = new AbstractIntegrationTest.ExtractionStrategy("Text Layer"
                );
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetFirstPage());
            pdfDocument.Close();
            PdfFont font = strategy.GetPdfFont();
            String fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("LiberationSans"));
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
        }

        [LogMessage(LogMessageConstant.CannotReadProvidedFont, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestInvalidFont() {
            String path = testImagesDirectory + "numbers_01.jpg";
            FileInfo file = new FileInfo(path);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, new OcrPdfCreatorProperties().SetFontPath(path)
                );
            PdfDocument doc = pdfRenderer.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter()
                , GetCMYKPdfOutputIntent());
            doc.Close();
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfDefaultMetadata() {
            String testName = "testPdfDefaultMetadata";
            String path = testImagesDirectory + "example_04.png";
            String pdfPath = testImagesDirectory + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, new OcrPdfCreatorProperties().SetTextColor(DeviceRgb
                .BLACK));
            PdfDocument doc = pdfRenderer.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter(pdfPath
                ), GetRGBPdfOutputIntent());
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            NUnit.Framework.Assert.AreEqual("en-US", pdfDocument.GetCatalog().GetLang().ToString());
            NUnit.Framework.Assert.AreEqual("", pdfDocument.GetDocumentInfo().GetTitle());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_3U, pdfDocument.GetReader().GetPdfAConformanceLevel
                ());
            pdfDocument.Close();
        }

        [NUnit.Framework.Test]
        public virtual void ComparePdfA3uCMYKColorSpaceSpanishJPG() {
            String testName = "comparePdfA3uCMYKColorSpaceSpanishJPG";
            String filename = "numbers_01";
            String expectedPdfPath = testDocumentsDirectory + filename + "_a3u.pdf";
            String resultPdfPath = testDocumentsDirectory + filename + "_" + testName + "_a3u_created.pdf";
            try {
                PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader);
                tesseractReader.SetTextPositioning(IOcrReader.TextPositioning.BY_WORDS);
                NUnit.Framework.Assert.AreEqual(tesseractReader, pdfRenderer.GetOcrReader());
                pdfRenderer.SetOcrReader(tesseractReader);
                PdfDocument doc = pdfRenderer.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(testImagesDirectory
                     + filename + ".jpg")), GetPdfWriter(resultPdfPath), GetCMYKPdfOutputIntent());
                NUnit.Framework.Assert.IsNotNull(doc);
                doc.Close();
                new CompareTool().CompareByContent(expectedPdfPath, resultPdfPath, testDocumentsDirectory, "diff_");
            }
            finally {
                NUnit.Framework.Assert.AreEqual(IOcrReader.TextPositioning.BY_WORDS, tesseractReader.GetTextPositioning());
                tesseractReader.SetTextPositioning(IOcrReader.TextPositioning.BY_LINES);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ComparePdfA3uRGBSpanishJPG() {
            String testName = "comparePdfA3uRGBSpanishJPG";
            String filename = "spanish_01";
            String expectedPdfPath = testDocumentsDirectory + filename + "_a3u.pdf";
            String resultPdfPath = testDocumentsDirectory + filename + "_" + testName + "_a3u_created.pdf";
            tesseractReader.SetPathToTessData(langTessDataDirectory);
            tesseractReader.SetLanguages(JavaCollectionsUtil.SingletonList<String>("spa"));
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, new OcrPdfCreatorProperties().SetTextColor(DeviceRgb
                .BLACK));
            PdfDocument doc = pdfRenderer.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(testImagesDirectory
                 + filename + ".jpg")), GetPdfWriter(resultPdfPath), GetRGBPdfOutputIntent());
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            new CompareTool().CompareByContent(expectedPdfPath, resultPdfPath, testDocumentsDirectory, "diff_");
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfCustomMetadata() {
            String testName = "testPdfCustomMetadata";
            String path = testImagesDirectory + "numbers_02.jpg";
            String pdfPath = testImagesDirectory + testName + ".pdf";
            FileInfo file = new FileInfo(path);
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            String locale = "nl-BE";
            properties.SetPdfLang(locale);
            String title = "Title";
            properties.SetTitle(title);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, new OcrPdfCreatorProperties(properties));
            PdfDocument doc = pdfRenderer.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter(pdfPath
                ), GetCMYKPdfOutputIntent());
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            NUnit.Framework.Assert.AreEqual(locale, pdfDocument.GetCatalog().GetLang().ToString());
            NUnit.Framework.Assert.AreEqual(title, pdfDocument.GetDocumentInfo().GetTitle());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_3U, pdfDocument.GetReader().GetPdfAConformanceLevel
                ());
            pdfDocument.Close();
        }
    }
}
