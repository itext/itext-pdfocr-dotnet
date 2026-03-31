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
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Pdfocr;
using iText.Pdfocr.Onnx;
using iText.Test;

namespace iText.Pdfocr.Onnx.Text {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TextPositioningModeEasyOcrTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/text/TextPositioningModeTest/";

        private static readonly String TEST_IMAGE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/text/TextPositioningModeEasyOcrTest/";

        public static IEnumerable<Object[]> Parameters() {
            return JavaUtil.ArraysAsList(new Object[] { OcrEngineTypeWithTextPositioning.EASY_LINES }, new Object[] { 
                OcrEngineTypeWithTextPositioning.EASY_WORDS }, new Object[] { OcrEngineTypeWithTextPositioning.EASY_WORDS_AND_LINES
                 });
        }

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            OcrEngineTypeWithTextPositioning.EASY_LINES.instance.Close();
            OcrEngineTypeWithTextPositioning.EASY_WORDS.instance.Close();
            OcrEngineTypeWithTextPositioning.EASY_WORDS_AND_LINES.instance.Close();
        }

        [NUnit.Framework.TestCaseSource("Parameters")]
        public virtual void LinesTest(OcrEngineTypeWithTextPositioning engineType) {
            IOcrEngine ocrEngine = engineType.Get();
            String name = engineType.GetDisplayName();
            String src = TEST_IMAGE_DIRECTORY + "lines.png";
            String dest = TARGET_DIRECTORY + name + "_lines.pdf";
            String cmp = TEST_DIRECTORY + "cmp_" + name + "_lines.pdf";
            DoOcrAndCreatePdf(src, dest, ocrEngine);
            OnnxTestUtils.ComparePdfs(dest, cmp, TARGET_DIRECTORY);
        }

        [NUnit.Framework.TestCaseSource("Parameters")]
        public virtual void ObliqueLinesTest(OcrEngineTypeWithTextPositioning engineType) {
            IOcrEngine ocrEngine = engineType.Get();
            String name = engineType.GetDisplayName();
            String src = TEST_IMAGE_DIRECTORY + "obliqueLines.png";
            String dest = TARGET_DIRECTORY + name + "_obliqueLines.pdf";
            String cmp = TEST_DIRECTORY + "cmp_" + name + "_obliqueLines.pdf";
            DoOcrAndCreatePdf(src, dest, ocrEngine);
            OnnxTestUtils.ComparePdfs(dest, cmp, TARGET_DIRECTORY);
        }

        [NUnit.Framework.TestCaseSource("Parameters")]
        public virtual void LinesWithSpacesTest(OcrEngineTypeWithTextPositioning engineType) {
            IOcrEngine ocrEngine = engineType.Get();
            String name = engineType.GetDisplayName();
            String src = TEST_IMAGE_DIRECTORY + "linesWithSpaces.png";
            String dest = TARGET_DIRECTORY + name + "_linesWithSpaces.pdf";
            String cmp = TEST_DIRECTORY + "cmp_" + name + "_linesWithSpaces.pdf";
            DoOcrAndCreatePdf(src, dest, ocrEngine);
            OnnxTestUtils.ComparePdfs(dest, cmp, TARGET_DIRECTORY);
        }

        private void DoOcrAndCreatePdf(String imagePath, String destPdfPath, IOcrEngine ocrEngine) {
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(ocrEngine, new OcrPdfCreatorProperties().SetTextLayerName(
                "Text1").SetTextColor(ColorConstants.MAGENTA).SetTextBBoxColor(ColorConstants.GREEN));
            using (PdfWriter writer = new PdfWriter(destPdfPath)) {
                ocrPdfCreator.CreatePdf(JavaCollectionsUtil.SingletonList(new FileInfo(imagePath)), writer).Close();
            }
        }
    }
}
