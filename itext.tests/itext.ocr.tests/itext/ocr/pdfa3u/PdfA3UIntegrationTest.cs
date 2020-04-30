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

        internal String parameter;

        public PdfA3UIntegrationTest(String type) {
            parameter = type;
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfA3uWithNullIntent() {
            String imgPath = testImagesDirectory + "numbers_01.jpg";
            FileInfo file = new FileInfo(imgPath);
            String expected = "619121";
            String pdfPath = testImagesDirectory + Guid.NewGuid().ToString() + ".pdf";
            IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                ), DeviceCmyk.BLACK, IPdfRenderer.ScaleMode.SCALE_TO_FIT);
            PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter(pdfPath), null);
            doc.Close();
            String result = GetTextFromPdfLayer(pdfPath, "Text Layer", 1);
            NUnit.Framework.Assert.AreEqual(expected, result);
            DeleteFile(pdfPath);
        }

        [NUnit.Framework.Test]
        public virtual void TestIncompatibleOutputIntentAndFontColorSpaceException() {
            String path = testImagesDirectory + "example_01.BMP";
            try {
                FileInfo file = new FileInfo(path);
                IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                    ), DeviceCmyk.BLACK);
                pdfRenderer.DoPdfOcr(GetPdfWriter(), GetRGBPdfOutputIntent());
            }
            catch (PdfException e) {
                NUnit.Framework.Assert.AreEqual(PdfAConformanceException.DEVICECMYK_MAY_BE_USED_ONLY_IF_THE_FILE_HAS_A_CMYK_PDFA_OUTPUT_INTENT_OR_DEFAULTCMYK_IN_USAGE_CONTEXT
                    , e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestDefaultFontInPdf() {
            String path = testImagesDirectory + "example_01.BMP";
            String pdfPath = testImagesDirectory + Guid.NewGuid().ToString() + ".pdf";
            FileInfo file = new FileInfo(path);
            IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                ), DeviceRgb.BLACK);
            PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter(pdfPath), GetRGBPdfOutputIntent());
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            AbstractIntegrationTest.ExtractionStrategy strategy = new AbstractIntegrationTest.ExtractionStrategy("Text Layer"
                );
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetFirstPage());
            PdfFont font = strategy.GetPdfFont();
            String fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("LiberationSans"));
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
            pdfDocument.Close();
            DeleteFile(pdfPath);
        }

        [NUnit.Framework.Test]
        public virtual void TestCustomFontInPdf() {
            String imgPath = testImagesDirectory + "numbers_01.jpg";
            String pdfPath = testImagesDirectory + Guid.NewGuid().ToString() + ".pdf";
            FileInfo file = new FileInfo(imgPath);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                ));
            pdfRenderer.SetFontPath(freeSansFontPath);
            PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter(pdfPath), GetCMYKPdfOutputIntent());
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            AbstractIntegrationTest.ExtractionStrategy strategy = new AbstractIntegrationTest.ExtractionStrategy("Text Layer"
                );
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetFirstPage());
            PdfFont font = strategy.GetPdfFont();
            String fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("FreeSans"));
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
            NUnit.Framework.Assert.AreEqual(freeSansFontPath, pdfRenderer.GetFontPath());
            pdfDocument.Close();
            DeleteFile(pdfPath);
        }

        [LogMessage(iText.IO.IOException.TypeOfFontIsNotRecognized, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestInvalidCustomFontInPdf() {
            String path = testImagesDirectory + "numbers_01.jpg";
            String pdfPath = testImagesDirectory + Guid.NewGuid().ToString() + ".pdf";
            FileInfo file = new FileInfo(path);
            IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                ));
            pdfRenderer.SetFontPath(path);
            PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter(pdfPath), GetCMYKPdfOutputIntent());
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            AbstractIntegrationTest.ExtractionStrategy strategy = new AbstractIntegrationTest.ExtractionStrategy("Text Layer"
                );
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetFirstPage());
            PdfFont font = strategy.GetPdfFont();
            String fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("LiberationSans"));
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
            pdfDocument.Close();
            DeleteFile(pdfPath);
        }

        [LogMessage(LogMessageConstant.CANNOT_READ_PROVIDED_FONT, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestInvalidFont() {
            String path = testImagesDirectory + "numbers_01.jpg";
            try {
                FileInfo file = new FileInfo(path);
                PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                    ));
                pdfRenderer.SetFontPath(path);
                pdfRenderer.DoPdfOcr(GetPdfWriter(), GetCMYKPdfOutputIntent());
            }
            catch (Exception e) {
                NUnit.Framework.Assert.AreEqual(OCRException.CANNOT_READ_FONT, e.Message);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfDefaultMetadata() {
            String path = testImagesDirectory + "example_04.png";
            String pdfPath = testImagesDirectory + Guid.NewGuid().ToString() + ".pdf";
            FileInfo file = new FileInfo(path);
            IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                ), DeviceRgb.BLACK);
            PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter(pdfPath), GetRGBPdfOutputIntent());
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            NUnit.Framework.Assert.AreEqual("en-US", pdfDocument.GetCatalog().GetLang().ToString());
            NUnit.Framework.Assert.AreEqual("", pdfDocument.GetDocumentInfo().GetTitle());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_3U, pdfDocument.GetReader().GetPdfAConformanceLevel
                ());
            pdfDocument.Close();
            DeleteFile(pdfPath);
        }

        [NUnit.Framework.Test]
        public virtual void ComparePdfA3uCMYKColorSpaceSpanishJPG() {
            String filename = "numbers_01";
            String expectedPdfPath = testDocumentsDirectory + filename + "_a3u.pdf";
            String resultPdfPath = testDocumentsDirectory + filename + "_a3u_created.pdf";
            try {
                PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(new 
                    FileInfo(testImagesDirectory + filename + ".jpg")));
                pdfRenderer.SetScaleMode(IPdfRenderer.ScaleMode.KEEP_ORIGINAL_SIZE);
                pdfRenderer.SetOcrReader(tesseractReader);
                tesseractReader.SetTextPositioning(IOcrReader.TextPositioning.BY_WORDS);
                NUnit.Framework.Assert.AreEqual(tesseractReader, pdfRenderer.GetOcrReader());
                PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter(resultPdfPath), GetCMYKPdfOutputIntent());
                NUnit.Framework.Assert.IsNotNull(doc);
                doc.Close();
                new CompareTool().CompareByContent(expectedPdfPath, resultPdfPath, testDocumentsDirectory, "diff_");
            }
            finally {
                DeleteFile(resultPdfPath);
                NUnit.Framework.Assert.AreEqual(IOcrReader.TextPositioning.BY_WORDS, tesseractReader.GetTextPositioning());
                tesseractReader.SetTextPositioning(IOcrReader.TextPositioning.BY_LINES);
            }
        }

        [NUnit.Framework.Test]
        public virtual void ComparePdfA3uRGBSpanishJPG() {
            String filename = "spanish_01";
            String expectedPdfPath = testDocumentsDirectory + filename + "_a3u.pdf";
            String resultPdfPath = testDocumentsDirectory + filename + "_a3u_created.pdf";
            tesseractReader.SetPathToTessData(langTessDataDirectory);
            tesseractReader.SetLanguages(JavaCollectionsUtil.SingletonList<String>("spa"));
            try {
                PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(new 
                    FileInfo(testImagesDirectory + filename + ".jpg")));
                pdfRenderer.SetTextColor(DeviceRgb.BLACK);
                pdfRenderer.SetScaleMode(IPdfRenderer.ScaleMode.KEEP_ORIGINAL_SIZE);
                PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter(resultPdfPath), GetRGBPdfOutputIntent());
                NUnit.Framework.Assert.IsNotNull(doc);
                doc.Close();
                new CompareTool().CompareByContent(expectedPdfPath, resultPdfPath, testDocumentsDirectory, "diff_");
            }
            finally {
                DeleteFile(resultPdfPath);
            }
        }

        [NUnit.Framework.Test]
        public virtual void TestPdfCustomMetadata() {
            String path = testImagesDirectory + "numbers_02.jpg";
            String pdfPath = testImagesDirectory + Guid.NewGuid().ToString() + ".pdf";
            FileInfo file = new FileInfo(path);
            IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                ));
            String locale = "nl-BE";
            pdfRenderer.SetPdfLang(locale);
            String title = "Title";
            pdfRenderer.SetTitle(title);
            PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter(pdfPath), GetCMYKPdfOutputIntent());
            NUnit.Framework.Assert.IsNotNull(doc);
            doc.Close();
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            NUnit.Framework.Assert.AreEqual(locale, pdfDocument.GetCatalog().GetLang().ToString());
            NUnit.Framework.Assert.AreEqual(title, pdfDocument.GetDocumentInfo().GetTitle());
            NUnit.Framework.Assert.AreEqual(PdfAConformanceLevel.PDF_A_3U, pdfDocument.GetReader().GetPdfAConformanceLevel
                ());
            pdfDocument.Close();
            DeleteFile(pdfPath);
        }
    }
}
