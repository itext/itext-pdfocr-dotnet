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

        public enum ReaderType {
            LIB,
            EXECUTABLE
        }

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

        protected internal static TesseractReader GetTesseractReader(AbstractIntegrationTest.ReaderType type) {
            if (type.Equals(AbstractIntegrationTest.ReaderType.LIB)) {
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

        /// <summary>Retrieve image from given pdf document.</summary>
        protected internal virtual Image GetImageFromPdf(TesseractReader tesseractReader, FileInfo file, ScaleMode
             scaleMode, Rectangle pageSize) {
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetScaleMode(scaleMode);
            properties.SetPageSize(pageSize);
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, properties);
            PdfDocument doc = pdfRenderer.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(file), GetPdfWriter());
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

        /// <summary>Retrieve image BBox rectangle from the first page from given pdf document.</summary>
        protected internal virtual Rectangle GetImageBBoxRectangleFromPdf(String path) {
            PdfDocument doc = new PdfDocument(new PdfReader(path));
            AbstractIntegrationTest.ExtractionStrategy extractionStrategy = new AbstractIntegrationTest.ExtractionStrategy
                ("Image Layer");
            PdfCanvasProcessor processor = new PdfCanvasProcessor(extractionStrategy);
            processor.ProcessPageContent(doc.GetFirstPage());
            doc.Close();
            return extractionStrategy.GetImageBBoxRectangle();
        }

        /// <summary>Retrieve text from specified page from given pdf document.</summary>
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
        protected internal virtual String GetTextFromPdf(TesseractReader tesseractReader, FileInfo file, IList<String
            > languages, String fontPath) {
            return GetTextFromPdf(tesseractReader, file, 1, languages, fontPath);
        }

        /// <summary>Retrieve text from the first page of given pdf document.</summary>
        protected internal virtual String GetTextFromPdf(TesseractReader tesseractReader, FileInfo file, IList<String
            > languages) {
            return GetTextFromPdf(tesseractReader, file, 1, languages, null);
        }

        /// <summary>Retrieve text from the required page of given pdf document.</summary>
        protected internal virtual String GetTextFromPdf(TesseractReader tesseractReader, FileInfo file, int page, 
            IList<String> languages) {
            return GetTextFromPdf(tesseractReader, file, page, languages, null);
        }

        /// <summary>Retrieve text from the first page of given pdf document.</summary>
        protected internal virtual String GetTextFromPdf(TesseractReader tesseractReader, FileInfo file) {
            return GetTextFromPdf(tesseractReader, file, 1, null, null);
        }

        /// <summary>Get text from layer specified by name from page.</summary>
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
        protected internal virtual String GetRecognizedTextFromTextFile(TesseractReader tesseractReader, String input
            , IList<String> languages) {
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
        protected internal virtual String GetRecognizedTextFromTextFile(TesseractReader tesseractReader, String input
            ) {
            return GetRecognizedTextFromTextFile(tesseractReader, input, null);
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result to text file.
        /// </summary>
        protected internal virtual void DoOcrAndSaveToTextFile(TesseractReader tesseractReader, String imgPath, String
             txtPath, IList<String> languages) {
            if (languages != null) {
                tesseractReader.SetLanguages(languages);
            }
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, new OcrPdfCreatorProperties());
            pdfRenderer.CreateTxt(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(imgPath)), txtPath);
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
        protected internal virtual void DoOcrAndSavePdfToPath(TesseractReader tesseractReader, String imgPath, String
             pdfPath, IList<String> languages, String fontPath, Color color) {
            if (languages != null) {
                tesseractReader.SetLanguages(languages);
            }
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            if (fontPath != null && !String.IsNullOrEmpty(fontPath)) {
                properties.SetFontPath(fontPath);
            }
            if (color != null) {
                properties.SetTextColor(color);
            }
            if (languages != null) {
                NUnit.Framework.Assert.AreEqual(languages.Count, tesseractReader.GetLanguagesAsList().Count);
            }
            PdfRenderer pdfRenderer = new PdfRenderer(tesseractReader, properties);
            try {
                using (PdfWriter pdfWriter = GetPdfWriter(pdfPath)) {
                    PdfDocument doc = pdfRenderer.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(imgPath))
                        , pdfWriter);
                    NUnit.Framework.Assert.IsNotNull(doc);
                    doc.Close();
                }
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(e.Message);
            }
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result pdf document to "pdfPath".
        /// </summary>
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
        protected internal virtual void DoOcrAndSavePdfToPath(TesseractReader tesseractReader, String imgPath, String
             pdfPath, IList<String> languages, String fontPath) {
            DoOcrAndSavePdfToPath(tesseractReader, imgPath, pdfPath, languages, fontPath, null);
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
        protected internal virtual void DoOcrAndSavePdfToPath(TesseractReader tesseractReader, String imgPath, String
             pdfPath) {
            DoOcrAndSavePdfToPath(tesseractReader, imgPath, pdfPath, null, null, null);
        }

        /// <summary>Retrieve text from given txt file.</summary>
        protected internal virtual String GetTextFromTextFile(FileInfo file) {
            String content = null;
            try {
                content = iText.IO.Util.JavaUtil.GetStringForBytes(System.IO.File.ReadAllBytes(file.FullName), System.Text.Encoding
                    .UTF8);
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(MessageFormatUtil.Format(LogMessageConstant.CannotReadFile, file.FullName, e.Message));
            }
            return content;
        }

        /// <summary>Delete file using provided path.</summary>
        protected internal virtual void DeleteFile(String filePath) {
            try {
                if (filePath != null && !String.IsNullOrEmpty(filePath) && File.Exists(System.IO.Path.Combine(filePath))) {
                    File.Delete(System.IO.Path.Combine(filePath));
                }
            }
            catch (Exception e) {
                LOGGER.Info(MessageFormatUtil.Format(LogMessageConstant.CannotDeleteFile, filePath, e.Message));
            }
        }

        /// <summary>Do OCR for given image and compare result etxt file with expected one.</summary>
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

        /// <summary>Create pdfWriter using provided path to destination file.</summary>
        protected internal virtual PdfWriter GetPdfWriter(String pdfPath) {
            return new PdfWriter(pdfPath, new WriterProperties().AddUAXmpMetadata());
        }

        /// <summary>Creates pdf cmyk output intent for tests.</summary>
        protected internal virtual PdfOutputIntent GetCMYKPdfOutputIntent() {
            Stream @is = new FileStream(defaultCMYKColorProfilePath, FileMode.Open, FileAccess.Read);
            return new PdfOutputIntent("Custom", "", "http://www.color.org", "Coated FOGRA27 (ISO 12647 - 2:2004)", @is
                );
        }

        /// <summary>Creates pdf rgb output intent for tests.</summary>
        protected internal virtual PdfOutputIntent GetRGBPdfOutputIntent() {
            Stream @is = new FileStream(defaultRGBColorProfilePath, FileMode.Open, FileAccess.Read);
            return new PdfOutputIntent("", "", "", "sRGB IEC61966-2.1", @is);
        }

        /// <summary>Converts value from pixels to points.</summary>
        /// <param name="pixels">input value in pixels</param>
        /// <returns>result value in points</returns>
        protected internal virtual float GetPoints(float pixels) {
            return pixels * 3f / 4f;
        }

        /// <summary>Create pdfWriter.</summary>
        protected internal virtual PdfWriter GetPdfWriter() {
            return new PdfWriter(new ByteArrayOutputStream(), new WriterProperties().AddUAXmpMetadata());
        }

        public class ExtractionStrategy : LocationTextExtractionStrategy {
            private Rectangle imageBBoxRectangle;

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

            public virtual Rectangle GetImageBBoxRectangle() {
                return this.imageBBoxRectangle;
            }

            protected override bool IsChunkAtWordBoundary(TextChunk chunk, TextChunk previousChunk) {
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
                IList<CanvasTag> tagHierarchy = null;
                if (type.Equals(EventType.RENDER_TEXT)) {
                    TextRenderInfo textRenderInfo = (TextRenderInfo)data;
                    tagHierarchy = textRenderInfo.GetCanvasTagHierarchy();
                }
                else {
                    if (type.Equals(EventType.RENDER_IMAGE)) {
                        ImageRenderInfo imageRenderInfo = (ImageRenderInfo)data;
                        tagHierarchy = imageRenderInfo.GetCanvasTagHierarchy();
                    }
                }
                if (tagHierarchy != null) {
                    foreach (CanvasTag tag in tagHierarchy) {
                        PdfDictionary dict = tag.GetProperties();
                        String name = dict.Get(PdfName.Name).ToString();
                        if (name.Equals(layerName)) {
                            if (type.Equals(EventType.RENDER_TEXT)) {
                                TextRenderInfo renderInfo = (TextRenderInfo)data;
                                SetFillColor(renderInfo.GetGraphicsState().GetFillColor());
                                SetPdfFont(renderInfo.GetGraphicsState().GetFont());
                                base.EventOccurred(data, type);
                                break;
                            }
                            else {
                                if (type.Equals(EventType.RENDER_IMAGE)) {
                                    ImageRenderInfo renderInfo = (ImageRenderInfo)data;
                                    Matrix ctm = renderInfo.GetImageCtm();
                                    this.imageBBoxRectangle = new Rectangle(ctm.Get(6), ctm.Get(7), ctm.Get(0), ctm.Get(4));
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
