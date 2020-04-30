using System;
using System.Collections.Generic;
using System.IO;
using Common.Logging;
using iText.IO.Source;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Element;
using iText.Test;

namespace iText.Ocr {
    public class AbstractIntegrationTest : ExtendedITextTest {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Ocr.AbstractIntegrationTest));

        // path to hocr script for tesseract executable
        protected internal static String pathToHocrScript = null;

        // directory with trained data for tests
        protected internal static String langTessDataDirectory = null;

        // directory with trained data for tests
        protected internal static String scriptTessDataDirectory = null;

        // directory with test image files
        protected internal static String testImagesDirectory = null;

        // directory with fonts
        protected internal static String testFontsDirectory = null;

        // directory with fonts
        protected internal static String testDocumentsDirectory = null;

        // path to default cmyk color profile
        protected internal static String defaultCMYKColorProfilePath = null;

        // path to default rgb color profile
        protected internal static String defaultRGBColorProfilePath = null;

        // path to font for hindi
        protected internal static String notoSansFontPath = testFontsDirectory + "NotoSans-Regular.ttf";

        // path to font for japanese
        protected internal static String kosugiFontPath = testFontsDirectory + "Kosugi-Regular.ttf";

        // path to font for chinese
        protected internal static String notoSansSCFontPath = testFontsDirectory + "NotoSansSC-Regular.otf";

        // path to font for arabic
        protected internal static String cairoFontPath = testFontsDirectory + "Cairo-Regular.ttf";

        // path to font for georgian
        protected internal static String freeSansFontPath = testFontsDirectory + "FreeSans.ttf";

        protected internal static float delta = 1e-4f;

        internal static TesseractLibReader tesseractLibReader = null;

        internal static TesseractExecutableReader tesseractExecutableReader = null;

        public AbstractIntegrationTest() {
            SetResourceDirectories();
            tesseractLibReader = new TesseractLibReader(GetTessDataDirectory());
            tesseractExecutableReader = new TesseractExecutableReader(GetTesseractDirectory(), GetTessDataDirectory());
        }

        internal static void SetResourceDirectories() {
            String path = TestUtils.GetCurrentDirectory();
            if (testImagesDirectory == null) {
                testImagesDirectory = path + "images" + System.IO.Path.DirectorySeparatorChar;
            }
            if (langTessDataDirectory == null) {
                langTessDataDirectory = path + "tessdata";
            }
            if (scriptTessDataDirectory == null) {
                scriptTessDataDirectory = path + "tessdata" + System.IO.Path.DirectorySeparatorChar + "script";
            }
            if (pathToHocrScript == null) {
                pathToHocrScript = path + "hocr" + System.IO.Path.DirectorySeparatorChar;
            }
            if (testFontsDirectory == null) {
                testFontsDirectory = path + "fonts" + System.IO.Path.DirectorySeparatorChar;
                UpdateFonts();
            }
            if (testDocumentsDirectory == null) {
                testDocumentsDirectory = path + "documents" + System.IO.Path.DirectorySeparatorChar;
            }
            if (defaultCMYKColorProfilePath == null) {
                defaultCMYKColorProfilePath = path + "CoatedFOGRA27.icc";
            }
            if (defaultRGBColorProfilePath == null) {
                defaultRGBColorProfilePath = path + "sRGB_CS_profile.icm";
            }
        }

        internal static void UpdateFonts() {
            notoSansFontPath = testFontsDirectory + "NotoSans-Regular.ttf";
            kosugiFontPath = testFontsDirectory + "Kosugi-Regular.ttf";
            notoSansSCFontPath = testFontsDirectory + "NotoSansSC-Regular.otf";
            cairoFontPath = testFontsDirectory + "Cairo-Regular.ttf";
            freeSansFontPath = testFontsDirectory + "FreeSans.ttf";
        }

        protected internal static TesseractReader GetTesseractReader(String type) {
            if ("lib".Equals(type)) {
                return tesseractLibReader;
            }
            else {
                return tesseractExecutableReader;
            }
        }

        protected internal static String GetTesseractDirectory() {
            String tesseractDir = Environment.GetEnvironmentVariable("tesseractDir");
            String os = Environment.GetEnvironmentVariable("os.name") == null ? Environment.GetEnvironmentVariable("OS"
                ) : Environment.GetEnvironmentVariable("os.name");
            return os.ToLowerInvariant().Contains("win") && tesseractDir != null && !String.IsNullOrEmpty(tesseractDir
                ) ? tesseractDir + "\\tesseract.exe" : "tesseract";
        }

        protected internal static String GetTessDataDirectory() {
            return langTessDataDirectory;
        }

        protected internal static String GetPathToHocrScript() {
            return pathToHocrScript;
        }

        /// <summary>Retrieve image from given pdf document.</summary>
        /// <param name="tesseractReader"/>
        /// <param name="file"/>
        /// <param name="scaleMode"/>
        /// <param name="pageSize"/>
        /// <returns/>
        protected internal virtual Image GetImageFromPdf(TesseractReader tesseractReader, FileInfo file, IPdfRenderer.ScaleMode
             scaleMode, Rectangle pageSize) {
            IPdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(file
                ), scaleMode);
            pdfRenderer.SetPageSize(pageSize);
            PdfDocument doc = pdfRenderer.DoPdfOcr(GetPdfWriter());
            Image image = null;
            NUnit.Framework.Assert.IsNotNull(doc);
            if (!doc.IsClosed()) {
                PdfDictionary pageDict = doc.GetFirstPage().GetPdfObject();
                PdfDictionary pageResources = pageDict.GetAsDictionary(PdfName.Resources);
                PdfDictionary pageXObjects = pageResources.GetAsDictionary(PdfName.XObject);
                IList<PdfName> pdfNames = new List<PdfName>(pageXObjects.KeySet());
                PdfName imgRef = pdfNames[0];
                PdfStream imgStream = pageXObjects.GetAsStream(imgRef);
                PdfImageXObject imgObject = new PdfImageXObject(imgStream);
                image = new Image(imgObject);
                doc.Close();
            }
            return image;
        }

        /// <summary>Retrieve text from specified page from given pdf document.</summary>
        /// <param name="tesseractReader"/>
        /// <param name="file"/>
        /// <param name="page"/>
        /// <param name="languages"/>
        /// <param name="fontPath"/>
        /// <returns/>
        protected internal virtual String GetTextFromPdf(TesseractReader tesseractReader, FileInfo file, int page, 
            IList<String> languages, String fontPath) {
            String result = null;
            String pdfPath = null;
            try {
                pdfPath = TesseractUtil.GetTempDir() + Guid.NewGuid().ToString() + ".pdf";
                DoOcrAndSavePdfToPath(tesseractReader, file.FullName, pdfPath, languages, fontPath);
                result = GetTextFromPdfLayer(pdfPath, "Text Layer", page);
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(e.Message);
            }
            finally {
                DeleteFile(pdfPath);
            }
            return result;
        }

        /// <summary>Retrieve text from the first page of given pdf document setting font.</summary>
        /// <param name="tesseractReader"/>
        /// <param name="file"/>
        /// <param name="languages"/>
        /// <param name="fontPath"/>
        /// <returns/>
        protected internal virtual String GetTextFromPdf(TesseractReader tesseractReader, FileInfo file, IList<String
            > languages, String fontPath) {
            return GetTextFromPdf(tesseractReader, file, 1, languages, fontPath);
        }

        /// <summary>Retrieve text from the first page of given pdf document.</summary>
        /// <param name="tesseractReader"/>
        /// <param name="file"/>
        /// <param name="languages"/>
        /// <returns/>
        protected internal virtual String GetTextFromPdf(TesseractReader tesseractReader, FileInfo file, IList<String
            > languages) {
            return GetTextFromPdf(tesseractReader, file, 1, languages, null);
        }

        /// <summary>Retrieve text from the required page of given pdf document.</summary>
        /// <param name="tesseractReader"/>
        /// <param name="file"/>
        /// <param name="page"/>
        /// <param name="languages"/>
        /// <returns/>
        protected internal virtual String GetTextFromPdf(TesseractReader tesseractReader, FileInfo file, int page, 
            IList<String> languages) {
            return GetTextFromPdf(tesseractReader, file, page, languages, null);
        }

        /// <summary>Retrieve text from specified page from given pdf document.</summary>
        /// <param name="tesseractReader"/>
        /// <param name="file"/>
        /// <param name="page"/>
        /// <returns/>
        protected internal virtual String GetTextFromPdf(TesseractReader tesseractReader, FileInfo file, int page) {
            return GetTextFromPdf(tesseractReader, file, page, null, null);
        }

        /// <summary>Retrieve text from the first page of given pdf document.</summary>
        /// <param name="tesseractReader"/>
        /// <param name="file"/>
        /// <returns/>
        protected internal virtual String GetTextFromPdf(TesseractReader tesseractReader, FileInfo file) {
            return GetTextFromPdf(tesseractReader, file, 1, null, null);
        }

        /// <summary>Get text from layer specified by name from page.</summary>
        /// <param name="pdfPath"/>
        /// <param name="layerName"/>
        /// <param name="page"/>
        /// <returns/>
        protected internal virtual String GetTextFromPdfLayer(String pdfPath, String layerName, int page) {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            AbstractIntegrationTest.ExtractionStrategy textExtractionStrategy = new AbstractIntegrationTest.ExtractionStrategy
                (layerName);
            PdfCanvasProcessor processor = new PdfCanvasProcessor(textExtractionStrategy);
            processor.ProcessPageContent(pdfDocument.GetPage(page));
            pdfDocument.Close();
            return textExtractionStrategy.GetResultantText();
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath),
        /// save to file and get text from file.
        /// </summary>
        /// <param name="tesseractReader"/>
        /// <param name="input"/>
        /// <param name="languages"/>
        /// <returns/>
        protected internal virtual String GetOCRedTextFromTextFile(TesseractReader tesseractReader, String input, 
            IList<String> languages) {
            String result = null;
            String txtPath = null;
            try {
                txtPath = TesseractUtil.GetTempDir() + Guid.NewGuid().ToString() + ".txt";
                DoOcrAndSaveToTextFile(tesseractReader, input, txtPath, languages);
                result = GetTextFromTextFile(new FileInfo(txtPath));
            }
            catch (Exception e) {
                LOGGER.Error(e.Message);
            }
            finally {
                DeleteFile(txtPath);
            }
            return result;
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath),
        /// save to file and get text from file.
        /// </summary>
        /// <param name="tesseractReader"/>
        /// <param name="input"/>
        /// <returns/>
        protected internal virtual String GetOCRedTextFromTextFile(TesseractReader tesseractReader, String input) {
            return GetOCRedTextFromTextFile(tesseractReader, input, null);
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result to text file.
        /// </summary>
        /// <param name="tesseractReader"/>
        /// <param name="imgPath"/>
        /// <param name="txtPath"/>
        /// <param name="languages"/>
        protected internal virtual void DoOcrAndSaveToTextFile(TesseractReader tesseractReader, String imgPath, String
             txtPath, IList<String> languages) {
            if (languages != null) {
                tesseractReader.SetLanguages(languages);
            }
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(new 
                FileInfo(imgPath)));
            pdfRenderer.DoPdfOcr(txtPath);
            if (languages != null) {
                NUnit.Framework.Assert.AreEqual(languages.Count, tesseractReader.GetLanguagesAsList().Count);
            }
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result pdf document to "pdfPath".
        /// </summary>
        /// <remarks>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result pdf document to "pdfPath".
        /// (Method is used for compare tool)
        /// </remarks>
        /// <param name="tesseractReader"/>
        /// <param name="imgPath"/>
        /// <param name="pdfPath"/>
        /// <param name="languages"/>
        /// <param name="fontPath"/>
        /// <param name="color"/>
        protected internal virtual void DoOcrAndSavePdfToPath(TesseractReader tesseractReader, String imgPath, String
             pdfPath, IList<String> languages, String fontPath, Color color) {
            if (languages != null) {
                tesseractReader.SetLanguages(languages);
            }
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, JavaCollectionsUtil.SingletonList<FileInfo>(new 
                FileInfo(imgPath)));
            pdfRenderer.SetScaleMode(IPdfRenderer.ScaleMode.KEEP_ORIGINAL_SIZE);
            if (fontPath != null && !String.IsNullOrEmpty(fontPath)) {
                pdfRenderer.SetFontPath(fontPath);
            }
            if (color != null) {
                pdfRenderer.SetTextColor(color);
            }
            PdfDocument doc = null;
            try {
                doc = pdfRenderer.DoPdfOcr(GetPdfWriter(pdfPath));
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(e.Message);
            }
            if (languages != null) {
                NUnit.Framework.Assert.AreEqual(languages.Count, tesseractReader.GetLanguagesAsList().Count);
            }
            NUnit.Framework.Assert.IsNotNull(doc);
            if (!doc.IsClosed()) {
                doc.Close();
            }
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result pdf document to "pdfPath".
        /// </summary>
        /// <param name="tesseractReader"/>
        /// <param name="imgPath"/>
        /// <param name="pdfPath"/>
        /// <param name="languages"/>
        /// <param name="color"/>
        protected internal virtual void DoOcrAndSavePdfToPath(TesseractReader tesseractReader, String imgPath, String
             pdfPath, IList<String> languages, Color color) {
            DoOcrAndSavePdfToPath(tesseractReader, imgPath, pdfPath, languages, null, color);
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result pdf document to "pdfPath".
        /// </summary>
        /// <remarks>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result pdf document to "pdfPath".
        /// (Text will be invisible)
        /// </remarks>
        /// <param name="tesseractReader"/>
        /// <param name="imgPath"/>
        /// <param name="pdfPath"/>
        /// <param name="languages"/>
        /// <param name="fontPath"/>
        protected internal virtual void DoOcrAndSavePdfToPath(TesseractReader tesseractReader, String imgPath, String
             pdfPath, IList<String> languages, String fontPath) {
            DoOcrAndSavePdfToPath(tesseractReader, imgPath, pdfPath, languages, fontPath, null);
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result pdf document to "pdfPath".
        /// </summary>
        /// <param name="tesseractReader"/>
        /// <param name="imgPath"/>
        /// <param name="pdfPath"/>
        /// <param name="fontPath"/>
        protected internal virtual void DoOcrAndSavePdfToPath(TesseractReader tesseractReader, String imgPath, String
             pdfPath, String fontPath) {
            DoOcrAndSavePdfToPath(tesseractReader, imgPath, pdfPath, null, fontPath, null);
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result pdf document to "pdfPath".
        /// </summary>
        /// <remarks>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result pdf document to "pdfPath".
        /// (Method uses default font path)
        /// </remarks>
        /// <param name="tesseractReader"/>
        /// <param name="imgPath"/>
        /// <param name="pdfPath"/>
        /// <param name="languages"/>
        protected internal virtual void DoOcrAndSavePdfToPath(TesseractReader tesseractReader, String imgPath, String
             pdfPath, IList<String> languages) {
            DoOcrAndSavePdfToPath(tesseractReader, imgPath, pdfPath, languages, null, null);
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result pdf document to "pdfPath".
        /// </summary>
        /// <remarks>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result pdf document to "pdfPath".
        /// (Method is used for compare tool)
        /// </remarks>
        /// <param name="tesseractReader"/>
        /// <param name="imgPath"/>
        /// <param name="pdfPath"/>
        protected internal virtual void DoOcrAndSavePdfToPath(TesseractReader tesseractReader, String imgPath, String
             pdfPath) {
            DoOcrAndSavePdfToPath(tesseractReader, imgPath, pdfPath, null, null, null);
        }

        /// <summary>Retrieve text from given txt file.</summary>
        /// <param name="file"/>
        /// <returns/>
        protected internal virtual String GetTextFromTextFile(FileInfo file) {
            return UtilService.ReadTxtFile(file);
        }

        /// <summary>Delete file using provided path.</summary>
        /// <param name="filePath"/>
        protected internal virtual void DeleteFile(String filePath) {
            UtilService.DeleteFile(filePath);
        }

        /// <summary>Do OCR for given image and compare result etxt file with expected one.</summary>
        /// <param name="tesseractReader"/>
        /// <param name="imgPath"/>
        /// <param name="expectedPath"/>
        /// <param name="languages"/>
        /// <returns/>
        protected internal virtual bool DoOcrAndCompareTxtFiles(TesseractReader tesseractReader, String imgPath, String
             expectedPath, IList<String> languages) {
            bool result = false;
            String resutTxtFile = null;
            try {
                resutTxtFile = TesseractUtil.GetTempDir() + Guid.NewGuid().ToString() + ".txt";
                DoOcrAndSaveToTextFile(tesseractReader, imgPath, resutTxtFile, languages);
                result = CompareTxtFiles(expectedPath, resutTxtFile);
            }
            finally {
                DeleteFile(resutTxtFile);
            }
            return result;
        }

        /// <summary>Compare two text files using provided paths.</summary>
        /// <param name="expectedFilePath"/>
        /// <param name="resultFilePath"/>
        /// <returns/>
        protected internal virtual bool CompareTxtFiles(String expectedFilePath, String resultFilePath) {
            bool areEqual = true;
            try {
                IList<String> expected = System.IO.File.ReadAllLines(System.IO.Path.Combine(expectedFilePath));
                IList<String> result = System.IO.File.ReadAllLines(System.IO.Path.Combine(resultFilePath));
                if (expected.Count != result.Count) {
                    return false;
                }
                for (int i = 0; i < expected.Count; i++) {
                    String exp = expected[i].Replace("\n", "").Replace("\f", "");
                    exp = iText.IO.Util.StringUtil.ReplaceAll(exp, "[^\\u0009\\u000A\\u000D\\u0020-\\u007E]", "");
                    String res = result[i].Replace("\n", "").Replace("\f", "");
                    res = iText.IO.Util.StringUtil.ReplaceAll(res, "[^\\u0009\\u000A\\u000D\\u0020-\\u007E]", "");
                    if (expected[i] == null || result[i] == null) {
                        areEqual = false;
                        break;
                    }
                    else {
                        if (!exp.Equals(res)) {
                            areEqual = false;
                            break;
                        }
                    }
                }
            }
            catch (System.IO.IOException e) {
                areEqual = false;
                LOGGER.Error(e.Message);
            }
            return areEqual;
        }

        /// <summary>Create pdfWriter using provided ByteArrayOutputStream.</summary>
        /// <param name="baos"/>
        /// <returns/>
        protected internal virtual PdfWriter GetPdfWriter(ByteArrayOutputStream baos) {
            return new PdfWriter(baos, new WriterProperties().AddUAXmpMetadata());
        }

        /// <summary>Create pdfWriter using provided path to destination file.</summary>
        /// <param name="pdfPath"/>
        /// <returns/>
        protected internal virtual PdfWriter GetPdfWriter(String pdfPath) {
            return new PdfWriter(pdfPath, new WriterProperties().AddUAXmpMetadata());
        }

        /// <summary>Creates pdf cmyk output intent for tests.</summary>
        /// <returns/>
        protected internal virtual PdfOutputIntent GetCMYKPdfOutputIntent() {
            Stream @is = new FileStream(defaultCMYKColorProfilePath, FileMode.Open, FileAccess.Read);
            return new PdfOutputIntent("Custom", "", "http://www.color.org", "Coated FOGRA27 (ISO 12647 - 2:2004)", @is
                );
        }

        /// <summary>Creates pdf rgb output intent for tests.</summary>
        /// <returns/>
        protected internal virtual PdfOutputIntent GetRGBPdfOutputIntent() {
            Stream @is = new FileStream(defaultRGBColorProfilePath, FileMode.Open, FileAccess.Read);
            return new PdfOutputIntent("", "", "", "sRGB IEC61966-2.1", @is);
        }

        /// <summary>Create pdfWriter.</summary>
        /// <returns/>
        protected internal virtual PdfWriter GetPdfWriter() {
            return new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().AddUAXmpMetadata());
        }

        public class ExtractionStrategy : LocationTextExtractionStrategy {
            private Color fillColor;

            private String layerName;

            private PdfFont pdfFont;

            public ExtractionStrategy(String name)
                : base() {
                layerName = name;
            }

            public virtual void SetFillColor(Color color) {
                fillColor = color;
            }

            public virtual Color GetFillColor() {
                return fillColor;
            }

            public virtual void SetPdfFont(PdfFont font) {
                pdfFont = font;
            }

            public virtual PdfFont GetPdfFont() {
                return pdfFont;
            }

            protected override bool IsChunkAtWordBoundary(TextChunk chunk, TextChunk previousChunk) {
                String cur = chunk.GetText();
                String prev = previousChunk.GetText();
                ITextChunkLocation curLoc = chunk.GetLocation();
                ITextChunkLocation prevLoc = previousChunk.GetLocation();
                if (curLoc.GetStartLocation().Equals(curLoc.GetEndLocation()) || prevLoc.GetEndLocation().Equals(prevLoc.GetStartLocation
                    ())) {
                    return false;
                }
                return curLoc.DistParallelEnd() - prevLoc.DistParallelStart() > (curLoc.GetCharSpaceWidth() + prevLoc.GetCharSpaceWidth
                    ()) / 2.0f;
            }

            public override void EventOccurred(IEventData data, EventType type) {
                if (EventType.RENDER_TEXT.Equals(type)) {
                    TextRenderInfo renderInfo = (TextRenderInfo)data;
                    IList<CanvasTag> tagHierarchy = renderInfo.GetCanvasTagHierarchy();
                    foreach (CanvasTag tag in tagHierarchy) {
                        PdfDictionary dict = tag.GetProperties();
                        String name = dict.Get(PdfName.Name).ToString();
                        if (layerName.Equals(name)) {
                            SetFillColor(renderInfo.GetGraphicsState().GetFillColor());
                            SetPdfFont(renderInfo.GetGraphicsState().GetFont());
                            base.EventOccurred(data, type);
                            break;
                        }
                    }
                }
            }
        }
    }
}
