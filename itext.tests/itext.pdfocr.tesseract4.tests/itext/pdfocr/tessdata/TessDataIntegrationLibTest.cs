/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Logs;
using iText.Pdfocr.Tesseract4;
using iText.Pdfocr.Tesseract4.Exceptions;
using iText.Test.Attributes;

namespace iText.Pdfocr.Tessdata {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TessDataIntegrationLibTest : TessDataIntegrationTest {
        public TessDataIntegrationLibTest()
            : base(IntegrationTestHelper.ReaderType.LIB) {
        }

        [LogMessage(PdfOcrTesseract4ExceptionMessageConstant.PATH_TO_TESS_DATA_DIRECTORY_CONTAINS_NON_ASCII_CHARACTERS
            )]
        [NUnit.Framework.Test]
        public virtual void TestTessDataWithNonAsciiPath() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(PdfOcrTesseract4Exception), () => 
                        // Throws exception for the tesseract lib test
                        DoOcrAndGetTextUsingTessDataByNonAsciiPath());
            NUnit.Framework.Assert.AreEqual(PdfOcrTesseract4ExceptionMessageConstant.PATH_TO_TESS_DATA_DIRECTORY_CONTAINS_NON_ASCII_CHARACTERS
                , exception.Message);
        }

#if !NETSTANDARD2_0
        [NUnit.Framework.Timeout(60000)]
#endif // !NETSTANDARD2_0
        [NUnit.Framework.Test]
        public virtual void TextOutputFromHalftoneFile() {
            String imgPath = TEST_IMAGES_DIRECTORY + "halftone.jpg";
            String expected01 = "Silliness Enablers";
            String expected02 = "You dream it, we enable it";
            String expected03 = "QUANTITY";
            String result = GetRecognizedTextFromTextFile(tesseractReader, imgPath, JavaCollectionsUtil.SingletonList<
                String>("eng"));
            // correct result for a halftone input image
            NUnit.Framework.Assert.IsTrue(result.Contains(expected01));
            NUnit.Framework.Assert.IsTrue(result.Contains(expected02));
            NUnit.Framework.Assert.IsTrue(result.Contains(expected03));
        }

        [NUnit.Framework.Test]
        public virtual void CompareInvoiceFrontThaiImage() {
            String testName = "compareInvoiceFrontThaiImage";
            String filename = "invoice_front_thai";
            //Tesseract for Java and Tesseract for .NET give different output
            //So we cannot use one reference pdf file for them
            String expectedPdfPathJava = TEST_DOCUMENTS_DIRECTORY + filename + "_" + testFileTypeName + "_java.pdf";
            String expectedPdfPathDotNet = TEST_DOCUMENTS_DIRECTORY + filename + "_" + testFileTypeName + "_dotnet.pdf";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + "_" + testFileTypeName + ".pdf";
            Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
            properties.SetTextPositioning(TextPositioning.BY_WORDS_AND_LINES);
            properties.SetPathToTessData(GetTessDataDirectory());
            properties.SetLanguages(JavaUtil.ArraysAsList("tha", "eng"));
            tesseractReader.SetTesseract4OcrEngineProperties(properties);
            DoOcrAndSavePdfToPath(tesseractReader, TEST_IMAGES_DIRECTORY + filename + ".jpg", resultPdfPath, JavaUtil.ArraysAsList
                ("tha", "eng"), JavaUtil.ArraysAsList(NOTO_SANS_THAI_FONT_PATH, NOTO_SANS_FONT_PATH), DeviceRgb.RED);
            bool javaTest = new CompareTool().CompareByContent(resultPdfPath, expectedPdfPathJava, GetTargetDirectory(
                ), "diff_") == null;
            bool dotNetTest = new CompareTool().CompareByContent(resultPdfPath, expectedPdfPathDotNet, GetTargetDirectory
                (), "diff_") == null;
            NUnit.Framework.Assert.IsTrue(javaTest || dotNetTest);
        }

        [LogMessage(PdfOcrLogMessageConstant.COULD_NOT_FIND_CORRESPONDING_GLYPH_TO_UNICODE_CHARACTER, Count = 2)]
        [NUnit.Framework.Test]
        public virtual void CompareThaiTextImage() {
            String testName = "compareThaiTextImage";
            String filename = "thai_01";
            //Tesseract for Java and Tesseract for .NET give different output
            //So we cannot use one reference pdf file for them
            String expectedPdfPathJava = TEST_DOCUMENTS_DIRECTORY + filename + "_" + testFileTypeName + "_java.pdf";
            String expectedPdfPathDotNet = TEST_DOCUMENTS_DIRECTORY + filename + "_" + testFileTypeName + "_dotnet.pdf";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + "_" + testFileTypeName + ".pdf";
            Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
            properties.SetTextPositioning(TextPositioning.BY_WORDS_AND_LINES);
            properties.SetPathToTessData(GetTessDataDirectory());
            properties.SetLanguages(JavaUtil.ArraysAsList("tha"));
            tesseractReader.SetTesseract4OcrEngineProperties(properties);
            DoOcrAndSavePdfToPath(tesseractReader, TEST_IMAGES_DIRECTORY + filename + ".jpg", resultPdfPath, JavaUtil.ArraysAsList
                ("tha"), JavaUtil.ArraysAsList(NOTO_SANS_THAI_FONT_PATH), DeviceRgb.RED);
            bool javaTest = new CompareTool().CompareByContent(resultPdfPath, expectedPdfPathJava, GetTargetDirectory(
                ), "diff_") == null;
            bool dotNetTest = new CompareTool().CompareByContent(resultPdfPath, expectedPdfPathDotNet, GetTargetDirectory
                (), "diff_") == null;
            NUnit.Framework.Assert.IsTrue(javaTest || dotNetTest);
        }
    }
}
