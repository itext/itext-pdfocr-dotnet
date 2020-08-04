/*
This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
using System.Collections.Generic;
using System.IO;
using Common.Logging;
using iText.IO.Font;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout.Font;
using iText.Pdfocr.Tesseract4;
using iText.Test;

namespace iText.Pdfocr {
    public class IntegrationTestHelper : ExtendedITextTest {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Pdfocr.IntegrationTestHelper));

        // directory with test files
        public static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        private static readonly String TARGET_FOLDER = NUnit.Framework.TestContext.CurrentContext.TestDirectory + 
            "/test/resources/itext/pdfocr/";

        private static readonly String NON_ASCII_TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/ñoñ-ascîî/";

        // directory with trained data for tests
        protected internal static readonly String LANG_TESS_DATA_DIRECTORY = TEST_DIRECTORY + "tessdata";

        // directory with trained data for tests
        protected internal static readonly String SCRIPT_TESS_DATA_DIRECTORY = TEST_DIRECTORY + "tessdata" + System.IO.Path.DirectorySeparatorChar
             + "script";

        // directory with trained data for tests
        protected internal static readonly String NON_ASCII_TESS_DATA_DIRECTORY = TEST_DIRECTORY + "tessdata" + System.IO.Path.DirectorySeparatorChar
             + "ñoñ-ascîî";

        // directory with test image files
        protected internal static readonly String TEST_IMAGES_DIRECTORY = TEST_DIRECTORY + "images" + System.IO.Path.DirectorySeparatorChar;

        // directory with fonts
        protected internal static readonly String TEST_FONTS_DIRECTORY = TEST_DIRECTORY + "fonts" + System.IO.Path.DirectorySeparatorChar;

        // directory with fonts
        protected internal static readonly String TEST_DOCUMENTS_DIRECTORY = TEST_DIRECTORY + "documents" + System.IO.Path.DirectorySeparatorChar;

        // path to font for hindi
        protected internal static readonly String NOTO_SANS_FONT_PATH = TEST_FONTS_DIRECTORY + "NotoSans-Regular.ttf";

        // path to font for thai
        protected internal static readonly String NOTO_SANS_THAI_FONT_PATH = TEST_FONTS_DIRECTORY + "NotoSansThai-Regular.ttf";

        // path to font for japanese
        protected internal static readonly String KOSUGI_FONT_PATH = TEST_FONTS_DIRECTORY + "Kosugi-Regular.ttf";

        // path to font for chinese
        protected internal static readonly String NOTO_SANS_SC_FONT_PATH = TEST_FONTS_DIRECTORY + "NotoSansSC-Regular.otf";

        // path to font for arabic
        protected internal static readonly String CAIRO_FONT_PATH = TEST_FONTS_DIRECTORY + "Cairo-Regular.ttf";

        // path to font for georgian
        protected internal static readonly String FREE_SANS_FONT_PATH = TEST_FONTS_DIRECTORY + "FreeSans.ttf";

        protected internal static readonly IDictionary<String, String> FONT_PATH_TO_FONT_NAME_MAP;

        static IntegrationTestHelper() {
            IDictionary<String, String> fontPathToNameMap = new Dictionary<String, String>();
            fontPathToNameMap.Put(NOTO_SANS_FONT_PATH, "NotoSans");
            fontPathToNameMap.Put(NOTO_SANS_THAI_FONT_PATH, "NotoSansThai");
            fontPathToNameMap.Put(KOSUGI_FONT_PATH, "Kosugi");
            fontPathToNameMap.Put(NOTO_SANS_SC_FONT_PATH, "NotoSansSC");
            fontPathToNameMap.Put(CAIRO_FONT_PATH, "Cairo");
            fontPathToNameMap.Put(FREE_SANS_FONT_PATH, "FreeSans");
            FONT_PATH_TO_FONT_NAME_MAP = JavaCollectionsUtil.UnmodifiableMap(fontPathToNameMap);
        }

        public enum ReaderType {
            LIB,
            EXECUTABLE
        }

        private static Tesseract4LibOcrEngine tesseractLibReader = null;

        private static Tesseract4ExecutableOcrEngine tesseractExecutableReader = null;

        public IntegrationTestHelper() {
            Tesseract4OcrEngineProperties ocrEngineProperties = new Tesseract4OcrEngineProperties();
            ocrEngineProperties.SetPathToTessData(GetTessDataDirectory());
            tesseractLibReader = new Tesseract4LibOcrEngine(ocrEngineProperties);
            tesseractExecutableReader = new Tesseract4ExecutableOcrEngine(ocrEngineProperties);
        }

        protected internal static AbstractTesseract4OcrEngine GetTesseractReader(IntegrationTestHelper.ReaderType 
            type) {
            if (type.Equals(IntegrationTestHelper.ReaderType.LIB)) {
                return tesseractLibReader;
            }
            else {
                return tesseractExecutableReader;
            }
        }

        protected internal static Tesseract4LibOcrEngine GetTesseract4LibOcrEngine() {
            return tesseractLibReader;
        }

        /// <summary>Returns target directory (because target/test could not exist).</summary>
        public static String GetTargetDirectory() {
            if (!File.Exists(System.IO.Path.Combine(TARGET_FOLDER))) {
                CreateDestinationFolder(TARGET_FOLDER);
            }
            return TARGET_FOLDER;
        }

        /// <summary>Returns a non ascii target directory.</summary>
        public static String GetNonAsciiTargetDirectory() {
            if (!File.Exists(System.IO.Path.Combine(NON_ASCII_TARGET_DIRECTORY))) {
                CreateDestinationFolder(NON_ASCII_TARGET_DIRECTORY);
            }
            return NON_ASCII_TARGET_DIRECTORY;
        }

        protected internal static FileInfo GetTessDataDirectory() {
            return new FileInfo(LANG_TESS_DATA_DIRECTORY);
        }

        /// <summary>Retrieve text from specified page from given PDF document.</summary>
        protected internal virtual String GetTextFromPdf(AbstractTesseract4OcrEngine tesseractReader, FileInfo file
            , int page, IList<String> languages, IList<String> fonts) {
            String result = null;
            String pdfPath = null;
            try {
                pdfPath = GetTargetDirectory() + GetImageName(file.FullName, languages) + ".pdf";
                DoOcrAndSavePdfToPath(tesseractReader, file.FullName, pdfPath, languages, fonts);
                result = GetTextFromPdfLayer(pdfPath, null, page);
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(e.Message);
            }
            return result;
        }

        /// <summary>Retrieve text from specified page from given PDF document.</summary>
        protected internal virtual String GetTextFromPdf(AbstractTesseract4OcrEngine tesseractReader, FileInfo file
            , int page, IList<String> languages, String fontPath) {
            return GetTextFromPdf(tesseractReader, file, page, languages, JavaCollectionsUtil.SingletonList<String>(fontPath
                ));
        }

        /// <summary>Retrieve text from the first page of given PDF document setting font.</summary>
        protected internal virtual String GetTextFromPdf(AbstractTesseract4OcrEngine tesseractReader, FileInfo file
            , IList<String> languages, String fontPath) {
            return GetTextFromPdf(tesseractReader, file, 1, languages, fontPath);
        }

        /// <summary>Retrieve text from the first page of given PDF document.</summary>
        protected internal virtual String GetTextFromPdf(AbstractTesseract4OcrEngine tesseractReader, FileInfo file
            , IList<String> languages) {
            return GetTextFromPdf(tesseractReader, file, 1, languages, new List<String>());
        }

        /// <summary>Retrieve text from the required page of given PDF document.</summary>
        protected internal virtual String GetTextFromPdf(AbstractTesseract4OcrEngine tesseractReader, FileInfo file
            , int page, IList<String> languages) {
            return GetTextFromPdf(tesseractReader, file, page, languages, new List<String>());
        }

        /// <summary>Retrieve text from the first page of given PDF document.</summary>
        protected internal virtual String GetTextFromPdf(AbstractTesseract4OcrEngine tesseractReader, FileInfo file
            ) {
            return GetTextFromPdf(tesseractReader, file, 1, null, new List<String>());
        }

        /// <summary>Get text from layer specified by name from page.</summary>
        protected internal virtual String GetTextFromPdfLayer(String pdfPath, String layerName, int page, bool useActualText
            ) {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath), new DocumentProperties().SetEventCountingMetaInfo
                (new PdfOcrMetaInfo()));
            IntegrationTestHelper.ExtractionStrategy textExtractionStrategy = new IntegrationTestHelper.ExtractionStrategy
                (layerName);
            textExtractionStrategy.SetUseActualText(useActualText);
            PdfCanvasProcessor processor = new PdfCanvasProcessor(textExtractionStrategy);
            processor.ProcessPageContent(pdfDocument.GetPage(page));
            pdfDocument.Close();
            return textExtractionStrategy.GetResultantText();
        }

        /// <summary>Get text from layer specified by name from page.</summary>
        protected internal virtual String GetTextFromPdfLayer(String pdfPath, String layerName, int page) {
            return GetTextFromPdfLayer(pdfPath, layerName, page, false);
        }

        /// <summary>
        /// Get text from layer specified by name from page
        /// removing unnecessary space that were added after each glyph in
        /// <see cref="iText.Kernel.Pdf.Canvas.Parser.Listener.LocationTextExtractionStrategy.GetResultantText()"/>.
        /// </summary>
        protected internal virtual String GetTextFromPdfLayerUsingActualText(String pdfPath, String layerName, int
             page) {
            return GetTextFromPdfLayer(pdfPath, layerName, page, true).Replace(" ", "");
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath),
        /// save to file and get text from file.
        /// </summary>
        protected internal virtual String GetRecognizedTextFromTextFile(AbstractTesseract4OcrEngine tesseractReader
            , String input, IList<String> languages) {
            String result = null;
            String txtPath = null;
            try {
                txtPath = GetTargetDirectory() + GetImageName(input, languages) + ".txt";
                DoOcrAndSaveToTextFile(tesseractReader, input, txtPath, languages);
                result = GetTextFromTextFile(new FileInfo(txtPath));
            }
            catch (Exception e) {
                LOGGER.Error(e.Message);
            }
            return result;
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath),
        /// save to file and get text from file.
        /// </summary>
        protected internal virtual String GetRecognizedTextFromTextFile(AbstractTesseract4OcrEngine tesseractReader
            , String input) {
            return GetRecognizedTextFromTextFile(tesseractReader, input, null);
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result to text file.
        /// </summary>
        protected internal virtual void DoOcrAndSaveToTextFile(AbstractTesseract4OcrEngine tesseractReader, String
             imgPath, String txtPath, IList<String> languages) {
            if (languages != null) {
                Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
                properties.SetLanguages(languages);
                tesseractReader.SetTesseract4OcrEngineProperties(properties);
            }
            tesseractReader.CreateTxtFile(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(imgPath)), new FileInfo
                (txtPath));
            if (languages != null) {
                NUnit.Framework.Assert.AreEqual(languages.Count, tesseractReader.GetTesseract4OcrEngineProperties().GetLanguages
                    ().Count);
            }
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result PDF document to "pdfPath".
        /// </summary>
        /// <remarks>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result PDF document to "pdfPath".
        /// (Method is used for compare tool)
        /// </remarks>
        protected internal virtual void DoOcrAndSavePdfToPath(AbstractTesseract4OcrEngine tesseractReader, String 
            imgPath, String pdfPath, IList<String> languages, IList<String> fonts, Color color) {
            if (languages != null) {
                Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
                properties.SetLanguages(languages);
                tesseractReader.SetTesseract4OcrEngineProperties(properties);
            }
            OcrPdfCreatorProperties properties_1 = new OcrPdfCreatorProperties();
            properties_1.SetPdfLang("en-US");
            properties_1.SetTitle("");
            if (fonts != null && fonts.Count > 0) {
                FontProvider fontProvider = new FontProvider();
                foreach (String fontPath in fonts) {
                    String name = FONT_PATH_TO_FONT_NAME_MAP.Get(fontPath);
                    fontProvider.GetFontSet().AddFont(fontPath, PdfEncodings.IDENTITY_H, name);
                }
                properties_1.SetFontProvider(fontProvider);
            }
            if (color != null) {
                properties_1.SetTextColor(color);
            }
            if (languages != null) {
                NUnit.Framework.Assert.AreEqual(languages.Count, tesseractReader.GetTesseract4OcrEngineProperties().GetLanguages
                    ().Count);
            }
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(tesseractReader, properties_1);
            try {
                using (PdfWriter pdfWriter = GetPdfWriter(pdfPath)) {
                    PdfDocument doc = ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(new FileInfo(imgPath
                        )), pdfWriter);
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
        /// and save result PDF document to "pdfPath".
        /// </summary>
        protected internal virtual void DoOcrAndSavePdfToPath(AbstractTesseract4OcrEngine tesseractReader, String 
            imgPath, String pdfPath, IList<String> languages, Color color) {
            DoOcrAndSavePdfToPath(tesseractReader, imgPath, pdfPath, languages, null, color);
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result PDF document to "pdfPath".
        /// </summary>
        /// <remarks>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result PDF document to "pdfPath".
        /// (Text will be invisible)
        /// </remarks>
        protected internal virtual void DoOcrAndSavePdfToPath(AbstractTesseract4OcrEngine tesseractReader, String 
            imgPath, String pdfPath, IList<String> languages, IList<String> fonts) {
            DoOcrAndSavePdfToPath(tesseractReader, imgPath, pdfPath, languages, fonts, null);
        }

        /// <summary>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result PDF document to "pdfPath".
        /// </summary>
        /// <remarks>
        /// Perform OCR using provided path to image (imgPath)
        /// and save result PDF document to "pdfPath".
        /// (Method is used for compare tool)
        /// </remarks>
        protected internal virtual void DoOcrAndSavePdfToPath(AbstractTesseract4OcrEngine tesseractReader, String 
            imgPath, String pdfPath) {
            DoOcrAndSavePdfToPath(tesseractReader, imgPath, pdfPath, null, null, null);
        }

        /// <summary>Retrieve text from given txt file.</summary>
        protected internal virtual String GetTextFromTextFile(FileInfo file) {
            String content = null;
            try {
                content = iText.IO.Util.JavaUtil.GetStringForBytes(File.ReadAllBytes(file.FullName), System.Text.Encoding.
                    UTF8);
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(MessageFormatUtil.Format(Tesseract4LogMessageConstant.CANNOT_READ_FILE, file.FullName, e.Message
                    ));
            }
            return content;
        }

        /// <summary>Create pdfWriter using provided path to destination file.</summary>
        protected internal virtual PdfWriter GetPdfWriter(String pdfPath) {
            return new PdfWriter(pdfPath, new WriterProperties().AddUAXmpMetadata());
        }

        /// <summary>Gets image name from path.</summary>
        protected internal virtual String GetImageName(String path, IList<String> languages) {
            String lang = (languages != null && languages.Count > 0) ? "_" + String.Join("", languages) : "";
            String img = path.Substring(path.LastIndexOf(System.IO.Path.DirectorySeparatorChar)).Substring(1).Replace(
                ".", "_");
            return img + lang;
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

            public virtual void SetImageBBoxRectangle(Rectangle imageBBoxRectangle) {
                this.imageBBoxRectangle = imageBBoxRectangle;
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
                if (type.Equals(EventType.RENDER_TEXT) || type.Equals(EventType.RENDER_IMAGE)) {
                    String tagName = GetTagName(data, type);
                    if ((tagName == null && layerName == null) || (layerName != null && layerName.Equals(tagName))) {
                        if (type.Equals(EventType.RENDER_TEXT)) {
                            TextRenderInfo renderInfo = (TextRenderInfo)data;
                            SetFillColor(renderInfo.GetGraphicsState().GetFillColor());
                            SetPdfFont(renderInfo.GetGraphicsState().GetFont());
                            base.EventOccurred(data, type);
                        }
                        else {
                            if (type.Equals(EventType.RENDER_IMAGE)) {
                                ImageRenderInfo renderInfo = (ImageRenderInfo)data;
                                Matrix ctm = renderInfo.GetImageCtm();
                                SetImageBBoxRectangle(new Rectangle(ctm.Get(6), ctm.Get(7), ctm.Get(0), ctm.Get(4)));
                            }
                        }
                    }
                }
            }

            private String GetTagName(IEventData data, EventType type) {
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
                return (tagHierarchy == null || tagHierarchy.Count == 0 || tagHierarchy[0].GetProperties().Get(PdfName.Name
                    ) == null) ? null : tagHierarchy[0].GetProperties().Get(PdfName.Name).ToString();
            }
        }
    }
}
