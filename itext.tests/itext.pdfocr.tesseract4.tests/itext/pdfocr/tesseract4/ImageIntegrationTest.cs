/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
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
using iText.IO.Util;
using iText.Kernel.Utils;
using iText.Pdfocr;

namespace iText.Pdfocr.Tesseract4 {
    public abstract class ImageIntegrationTest : IntegrationTestHelper {
        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Pdfocr.Tesseract4.ImageIntegrationTest
            ));

        internal AbstractTesseract4OcrEngine tesseractReader;

        internal String testFileTypeName;

        private bool isExecutableReaderType;

        public ImageIntegrationTest(IntegrationTestHelper.ReaderType type) {
            isExecutableReaderType = type.Equals(IntegrationTestHelper.ReaderType.EXECUTABLE);
            if (isExecutableReaderType) {
                testFileTypeName = "executable";
            }
            else {
                testFileTypeName = "lib";
            }
            tesseractReader = GetTesseractReader(type);
        }

        [NUnit.Framework.SetUp]
        public virtual void InitTesseractProperties() {
            Tesseract4OcrEngineProperties ocrEngineProperties = new Tesseract4OcrEngineProperties();
            ocrEngineProperties.SetPathToTessData(GetTessDataDirectory());
            tesseractReader.SetTesseract4OcrEngineProperties(ocrEngineProperties);
        }

        [NUnit.Framework.Test]
        public virtual void TestHocrRotatedImage() {
            String path = TEST_IMAGES_DIRECTORY + "90_degrees_rotated.jpg";
            FileInfo imgFile = new FileInfo(path);
            FileInfo outputFile = new FileInfo(GetTargetDirectory() + "90_degrees_rotated.hocr");
            tesseractReader.DoTesseractOcr(imgFile, outputFile, OutputFormat.HOCR);
            IDictionary<int, IList<TextInfo>> pageData = TesseractHelper.ParseHocrFile(JavaCollectionsUtil.SingletonList
                <FileInfo>(outputFile), null, new Tesseract4OcrEngineProperties().SetTextPositioning(TextPositioning.BY_WORDS
                ));
            NUnit.Framework.Assert.AreEqual("90", pageData.Get(1)[0].GetText());
            NUnit.Framework.Assert.AreEqual("degrees", pageData.Get(1)[1].GetText());
            NUnit.Framework.Assert.AreEqual("rotated", pageData.Get(1)[2].GetText());
            NUnit.Framework.Assert.AreEqual("image", pageData.Get(1)[3].GetText());
            NUnit.Framework.Assert.IsTrue(pageData.Get(1)[1].GetBbox()[2] - pageData.Get(1)[0].GetBbox()[0] > 100);
            NUnit.Framework.Assert.IsTrue(pageData.Get(1)[1].GetBbox()[3] - pageData.Get(1)[0].GetBbox()[1] < 100);
        }

        [NUnit.Framework.Test]
        public virtual void CompareRotatedImage() {
            String testName = "compareRotatedImage";
            String filename = "90_degrees_rotated";
            //Tesseract for Java and Tesseract for .NET give different output
            //So we cannot use one reference pdf file for them
            String expectedPdfPathJava = TEST_DOCUMENTS_DIRECTORY + filename + "_java.pdf";
            String expectedPdfPathDotNet = TEST_DOCUMENTS_DIRECTORY + filename + "_dotnet.pdf";
            String resultPdfPath = GetTargetDirectory() + filename + "_" + testName + ".pdf";
            Tesseract4OcrEngineProperties properties = tesseractReader.GetTesseract4OcrEngineProperties();
            properties.SetTextPositioning(TextPositioning.BY_WORDS);
            properties.SetPathToTessData(GetTessDataDirectory());
            tesseractReader.SetTesseract4OcrEngineProperties(properties);
            DoOcrAndSavePdfToPath(tesseractReader, TEST_IMAGES_DIRECTORY + filename + ".jpg", resultPdfPath, JavaUtil.ArraysAsList
                ("eng"), JavaUtil.ArraysAsList(NOTO_SANS_FONT_PATH), null, true);
            // Because of difference of tesseract 5 and tesseract 4 there're some differences in text recognition.
            // So the goal of this test is to make text invisible and check if image is rotated.
            // Proper text recognition is compared in testHocrRotatedImage test by checking HOCR file.
            bool javaTest = new CompareTool().CompareVisually(resultPdfPath, expectedPdfPathJava, TEST_DOCUMENTS_DIRECTORY
                , "diff_") == null;
            bool dotNetTest = new CompareTool().CompareVisually(resultPdfPath, expectedPdfPathDotNet, TEST_DOCUMENTS_DIRECTORY
                , "diff_") == null;
            NUnit.Framework.Assert.IsTrue(javaTest || dotNetTest);
            filename = "180_degrees_rotated";
            expectedPdfPathJava = TEST_DOCUMENTS_DIRECTORY + filename + "_java.pdf";
            expectedPdfPathDotNet = TEST_DOCUMENTS_DIRECTORY + filename + "_dotnet.pdf";
            resultPdfPath = GetTargetDirectory() + filename + "_" + testName + ".pdf";
            DoOcrAndSavePdfToPath(tesseractReader, TEST_IMAGES_DIRECTORY + filename + ".jpg", resultPdfPath, JavaUtil.ArraysAsList
                ("eng"), JavaUtil.ArraysAsList(NOTO_SANS_FONT_PATH), null, true);
            javaTest = new CompareTool().CompareVisually(resultPdfPath, expectedPdfPathJava, TEST_DOCUMENTS_DIRECTORY, 
                "diff_") == null;
            dotNetTest = new CompareTool().CompareVisually(resultPdfPath, expectedPdfPathDotNet, TEST_DOCUMENTS_DIRECTORY
                , "diff_") == null;
            NUnit.Framework.Assert.IsTrue(javaTest || dotNetTest);
            filename = "270_degrees_rotated";
            expectedPdfPathJava = TEST_DOCUMENTS_DIRECTORY + filename + "_java.pdf";
            expectedPdfPathDotNet = TEST_DOCUMENTS_DIRECTORY + filename + "_dotnet.pdf";
            resultPdfPath = GetTargetDirectory() + filename + "_" + testName + ".pdf";
            DoOcrAndSavePdfToPath(tesseractReader, TEST_IMAGES_DIRECTORY + filename + ".jpg", resultPdfPath, JavaUtil.ArraysAsList
                ("eng"), JavaUtil.ArraysAsList(NOTO_SANS_FONT_PATH), null, true);
            javaTest = new CompareTool().CompareVisually(resultPdfPath, expectedPdfPathJava, TEST_DOCUMENTS_DIRECTORY, 
                "diff_") == null;
            dotNetTest = new CompareTool().CompareVisually(resultPdfPath, expectedPdfPathDotNet, TEST_DOCUMENTS_DIRECTORY
                , "diff_") == null;
            NUnit.Framework.Assert.IsTrue(javaTest || dotNetTest);
        }
    }
}
