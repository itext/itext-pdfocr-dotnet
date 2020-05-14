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
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetTextColor(DeviceCmyk.BLACK);
            properties.SetScaleMode(ScaleMode.SCALE_TO_FIT);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, properties);
            PdfDocument doc = pdfRenderer.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter(pdfPath
                ), null);
            doc.Close();
            String result = GetTextFromPdfLayer(pdfPath, "Text Layer", 1);
            NUnit.Framework.Assert.AreEqual(expected, result);
            DeleteFile(pdfPath);
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
            String path = testImagesDirectory + "example_01.BMP";
            String pdfPath = testImagesDirectory + Guid.NewGuid().ToString() + ".pdf";
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
            PdfFont font = strategy.GetPdfFont();
            String fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("FreeSans"));
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
            NUnit.Framework.Assert.AreEqual(freeSansFontPath, pdfRenderer.GetOcrPdfCreatorProperties().GetFontPath());
            pdfDocument.Close();
            DeleteFile(pdfPath);
        }

        [LogMessage(iText.IO.IOException.TypeOfFontIsNotRecognized, Count = 1)]
        [NUnit.Framework.Test]
        public virtual void TestInvalidCustomFontInPdf() {
            String path = testImagesDirectory + "numbers_01.jpg";
            String pdfPath = testImagesDirectory + Guid.NewGuid().ToString() + ".pdf";
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
            PdfFont font = strategy.GetPdfFont();
            String fontName = font.GetFontProgram().GetFontNames().GetFontName();
            NUnit.Framework.Assert.IsTrue(fontName.Contains("LiberationSans"));
            NUnit.Framework.Assert.IsTrue(font.IsEmbedded());
            pdfDocument.Close();
            DeleteFile(pdfPath);
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
            String path = testImagesDirectory + "example_04.png";
            String pdfPath = testImagesDirectory + Guid.NewGuid().ToString() + ".pdf";
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
            DeleteFile(pdfPath);
        }

        [NUnit.Framework.Test]
        public virtual void ComparePdfA3uCMYKColorSpaceSpanishJPG() {
            String filename = "numbers_01";
            String expectedPdfPath = testDocumentsDirectory + filename + "_a3u.pdf";
            String resultPdfPath = testDocumentsDirectory + filename + "_a3u_created.pdf";
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
                PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, new OcrPdfCreatorProperties().SetTextColor(DeviceRgb
                    .BLACK));
                PdfDocument doc = pdfRenderer.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(testImagesDirectory
                     + filename + ".jpg")), GetPdfWriter(resultPdfPath), GetRGBPdfOutputIntent());
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
            try {
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
            finally {
                DeleteFile(pdfPath);
            }
        }
    }
}
