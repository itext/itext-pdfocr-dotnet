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
using System.IO;
using Common.Logging;
using iText.IO.Util;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Pdfocr;
using iText.Test;

namespace iText.Pdfocr.Helpers {
    public class PdfHelper {
        public const String DEFAULT_IMAGE_NAME = "numbers_01.jpg";

        public const String DEFAULT_TEXT = "619121";

        public const String THAI_IMAGE_NAME = "thai.PNG";

        public const String THAI_TEXT = "ป ระ เท ศ ไ";

        // directory with test files
        public static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        public static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory 
            + "/test/resources/itext/pdfocr/";

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(PdfHelper));

        /// <summary>Returns images test directory.</summary>
        public static String GetImagesTestDirectory() {
            return TEST_DIRECTORY + "images/";
        }

        /// <summary>Returns path to default test image.</summary>
        public static String GetDefaultImagePath() {
            return GetImagesTestDirectory() + DEFAULT_IMAGE_NAME;
        }

        /// <summary>Returns path to thai test image.</summary>
        public static String GetThaiImagePath() {
            return GetImagesTestDirectory() + THAI_IMAGE_NAME;
        }

        /// <summary>Returns path to test font.</summary>
        public static String GetFreeSansFontPath() {
            return TEST_DIRECTORY + "fonts/FreeSans.ttf";
        }

        /// <summary>Returns path to test font.</summary>
        public static String GetKanitFontPath() {
            return TEST_DIRECTORY + "fonts/Kanit-Regular.ttf";
        }

        /// <summary>Returns target directory (because target/test could not exist).</summary>
        public static String GetTargetDirectory() {
            if (!File.Exists(System.IO.Path.Combine(TARGET_DIRECTORY))) {
                ExtendedITextTest.CreateDestinationFolder(TARGET_DIRECTORY);
            }
            return TARGET_DIRECTORY;
        }

        /// <summary>Create pdfWriter using provided path to destination file.</summary>
        public static PdfWriter GetPdfWriter(String pdfPath) {
            return new PdfWriter(pdfPath, new WriterProperties().AddUAXmpMetadata());
        }

        /// <summary>Create pdfWriter.</summary>
        public static PdfWriter GetPdfWriter() {
            return new PdfWriter(new MemoryStream(), new WriterProperties().AddUAXmpMetadata());
        }

        /// <summary>Creates PDF rgb output intent for tests.</summary>
        public static PdfOutputIntent GetRGBPdfOutputIntent() {
            String defaultRGBColorProfilePath = TEST_DIRECTORY + "profiles" + "/sRGB_CS_profile.icm";
            Stream @is = new FileStream(defaultRGBColorProfilePath, FileMode.Open, FileAccess.Read);
            return new PdfOutputIntent("", "", "", "sRGB IEC61966-2.1", @is);
        }

        /// <summary>Creates PDF cmyk output intent for tests.</summary>
        public static PdfOutputIntent GetCMYKPdfOutputIntent() {
            String defaultCMYKColorProfilePath = TEST_DIRECTORY + "profiles/CoatedFOGRA27.icc";
            Stream @is = new FileStream(defaultCMYKColorProfilePath, FileMode.Open, FileAccess.Read);
            return new PdfOutputIntent("Custom", "", "http://www.color.org", "Coated FOGRA27 (ISO 12647 - 2:2004)", @is
                );
        }

        /// <summary>Get text from layer specified by name from the first page.</summary>
        public static String GetTextFromPdfLayer(String pdfPath, String layerName) {
            ExtractionStrategy textExtractionStrategy = GetExtractionStrategy(pdfPath, layerName, false);
            return textExtractionStrategy.GetResultantText();
        }

        /// <summary>Get text from layer specified by name from the first page.</summary>
        public static String GetTextFromPdfLayerUseActualText(String pdfPath, String layerName) {
            ExtractionStrategy textExtractionStrategy = GetExtractionStrategy(pdfPath, layerName, true);
            return textExtractionStrategy.GetResultantText();
        }

        /// <summary>
        /// Perform OCR with custom ocr engine using provided input image and set
        /// of properties and save to the given path.
        /// </summary>
        public static void CreatePdf(String pdfPath, FileInfo inputFile, OcrPdfCreatorProperties properties) {
            CreatePdf(pdfPath, inputFile, properties, false);
        }

        /// <summary>
        /// Perform OCR with custom ocr engine using provided input image and set
        /// of properties and save to the given path.
        /// </summary>
        public static void CreatePdf(String pdfPath, FileInfo inputFile, OcrPdfCreatorProperties properties, bool 
            textInfoDeprecationMode) {
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(new CustomOcrEngine(textInfoDeprecationMode), properties);
            try {
                using (PdfWriter pdfWriter = GetPdfWriter(pdfPath)) {
                    ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList<FileInfo>(inputFile), pdfWriter).Close();
                }
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(e.Message);
            }
        }

        /// <summary>
        /// Perform OCR with custom ocr engine using provided input image and set
        /// of properties and save to the given path.
        /// </summary>
        public static void CreatePdfA(String pdfPath, FileInfo inputFile, OcrPdfCreatorProperties properties, PdfOutputIntent
             outputIntent) {
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(new CustomOcrEngine(), properties);
            try {
                using (PdfWriter pdfWriter = GetPdfWriter(pdfPath)) {
                    ocrPdfCreator.CreatePdfA(JavaCollectionsUtil.SingletonList<FileInfo>(inputFile), pdfWriter, outputIntent).
                        Close();
                }
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(e.Message);
            }
        }

        /// <summary>Retrieve text from specified page from given PDF document.</summary>
        public static String GetTextFromPdf(FileInfo file, String testName) {
            String result = null;
            String pdfPath = null;
            try {
                pdfPath = GetTargetDirectory() + testName + ".pdf";
                OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
                properties.SetTextLayerName("Text Layer");
                CreatePdf(pdfPath, file, properties);
                result = GetTextFromPdfLayer(pdfPath, "Text Layer");
            }
            catch (System.IO.IOException e) {
                LOGGER.Error(e.Message);
            }
            return result;
        }

        /// <summary>Get extraction strategy for given document.</summary>
        public static ExtractionStrategy GetExtractionStrategy(String pdfPath) {
            return GetExtractionStrategy(pdfPath, null);
        }

        /// <summary>Get extraction strategy for given document.</summary>
        public static ExtractionStrategy GetExtractionStrategy(String pdfPath, bool useActualText) {
            return GetExtractionStrategy(pdfPath, "Text Layer", useActualText);
        }

        /// <summary>Get extraction strategy for given document.</summary>
        public static ExtractionStrategy GetExtractionStrategy(String pdfPath, String layerName) {
            return GetExtractionStrategy(pdfPath, layerName, false);
        }

        /// <summary>Get extraction strategy for given document.</summary>
        public static ExtractionStrategy GetExtractionStrategy(String pdfPath, String layerName, bool useActualText
            ) {
            PdfDocument pdfDocument = new PdfDocument(new PdfReader(pdfPath));
            ExtractionStrategy strategy = new ExtractionStrategy(layerName);
            strategy.SetUseActualText(useActualText);
            PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
            processor.ProcessPageContent(pdfDocument.GetFirstPage());
            pdfDocument.Close();
            return strategy;
        }
    }
}
