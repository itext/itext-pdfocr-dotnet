/*
This file is part of the iText (R) project.
Copyright (c) 1998-2026 Apryse Group NV
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
using iText.Pdfocr.Tesseract4;

namespace iText.Pdfocr.Ocrpdf {
    public abstract class OcrPdfIntegrationTest : IntegrationTestHelper {
        private static readonly String TARGET_DIRECTORY = GetTargetDirectory() + "OcrPdfIntegrationTest/";

        private static readonly String CMP_DIRECTORY = TEST_DOCUMENTS_DIRECTORY + "OcrPdfIntegrationTest/";

        private readonly AbstractTesseract4OcrEngine tesseractReader;

        private readonly String testType;

        public OcrPdfIntegrationTest(IntegrationTestHelper.ReaderType type) {
            tesseractReader = GetTesseractReader(type);
            this.testType = StringNormalizer.ToLowerCase(type.ToString());
        }

        [NUnit.Framework.OneTimeSetUp]
        public static void Init() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
        }

        [NUnit.Framework.SetUp]
        public virtual void InitTesseractProperties() {
            Tesseract4OcrEngineProperties ocrEngineProperties = new Tesseract4OcrEngineProperties();
            ocrEngineProperties.SetPathToTessData(GetTessDataDirectory());
            tesseractReader.SetTesseract4OcrEngineProperties(ocrEngineProperties);
        }

        [NUnit.Framework.Test]
        public virtual void BasicTest() {
            MakeSearchable("numbers");
        }

        [NUnit.Framework.Test]
        public virtual void PageRotationTest() {
            MakeSearchable("pageRotation");
        }

        [NUnit.Framework.Test]
        public virtual void TwoImagesTest() {
            MakeSearchable("2images");
        }

        [NUnit.Framework.Test]
        public virtual void TwoPagesTest() {
            MakeSearchable("2pages");
        }

        [NUnit.Framework.Test]
        public virtual void RotatedTest() {
            // Tesseract doesn't return textangle, that is why the resulting text is not rotated here
            MakeSearchable("rotated");
        }

        [NUnit.Framework.Test]
        public virtual void MixedRotationTest() {
            MakeSearchable("mixedRotation");
        }

        [NUnit.Framework.Test]
        public virtual void NotRecognizableTest() {
            MakeSearchable("notRecognizable");
        }

        [NUnit.Framework.Test]
        public virtual void ImageIntersectionTest() {
            MakeSearchable("imageIntersection");
        }

        [NUnit.Framework.Test]
        public virtual void WhiteTextTest() {
            // Not OCRed by tesseract
            MakeSearchable("whiteText");
        }

        [NUnit.Framework.Test]
        public virtual void ChangedImageProportionTest() {
            MakeSearchable("changedImageProportion");
        }

        [NUnit.Framework.Test]
        public virtual void TextWithImagesTest() {
            MakeSearchable("textWithImages");
        }

        [NUnit.Framework.Test]
        public virtual void InvisibleTextImageTest() {
            MakeSearchable("invisibleTextImage");
        }

        [NUnit.Framework.Test]
        public virtual void SkewedRotated45Test() {
            MakeSearchable("skewedRotated45");
        }

        private void MakeSearchable(String fileName) {
            String path = TEST_PDFS_DIRECTORY + fileName + ".pdf";
            String expectedPdfPath = CMP_DIRECTORY + fileName + ".pdf";
            String resultPdfPath = TARGET_DIRECTORY + fileName + "_" + testType + ".pdf";
            DoOcrAndSavePdfToPath(tesseractReader, path, resultPdfPath, JavaCollectionsUtil.SingletonList<String>("eng"
                ), null, DeviceCmyk.MAGENTA, false, false);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, GetTargetDirectory
                (), "diff_"));
        }
    }
}
