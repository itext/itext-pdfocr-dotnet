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
using System.Linq;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Test;

namespace iText.Pdfocr.Onnx.Text {
    [NUnit.Framework.Category("IntegrationTest")]
    public class TextPositioningModeTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/text/TextPositioningModeTest/";

        private static readonly String TEST_IMAGE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/text/TextPositioningModeTest/";

        public static IEnumerable<Object[]> Parameters() {
            return JavaUtil.ArraysToEnumerable(OcrEngineTypeWithTextPositioning.All()).Select((type) => new Object[] { 
                type }).ToList();
        }

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            foreach (OcrEngineTypeWithTextPositioning engineType in OcrEngineTypeWithTextPositioning.All()) {
                if (engineType.instance != null) {
                    engineType.instance.Close();
                }
            }
        }

        [NUnit.Framework.TestCaseSource("Parameters")]
        public virtual void LinesTest(OcrEngineTypeWithTextPositioning engineType) {
            IOcrEngine ocrEngine = engineType.Get();
            String name = engineType.GetDisplayName();
            String src = TEST_IMAGE_DIRECTORY + "lines.png";
            String dest = TARGET_DIRECTORY + name + "_lines.pdf";
            String cmp = TEST_DIRECTORY + "cmp_" + name + "_lines.pdf";
            DoOcrAndCreatePdf(src, dest, ocrEngine);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.TestCaseSource("Parameters")]
        public virtual void ObliqueLinesTest(OcrEngineTypeWithTextPositioning engineType) {
            IOcrEngine ocrEngine = engineType.Get();
            String name = engineType.GetDisplayName();
            String src = TEST_IMAGE_DIRECTORY + "obliqueLines.png";
            String dest = TARGET_DIRECTORY + name + "_obliqueLines.pdf";
            String cmp = TEST_DIRECTORY + "cmp_" + name + "_obliqueLines.pdf";
            DoOcrAndCreatePdf(src, dest, ocrEngine);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.TestCaseSource("Parameters")]
        public virtual void LinesWithSpacesTest(OcrEngineTypeWithTextPositioning engineType) {
            IOcrEngine ocrEngine = engineType.Get();
            String name = engineType.GetDisplayName();
            String src = TEST_IMAGE_DIRECTORY + "linesWithSpaces.png";
            String dest = TARGET_DIRECTORY + name + "_linesWithSpaces.pdf";
            String cmp1 = TEST_DIRECTORY + "cmp_" + name + "_linesWithSpaces.pdf";
            String cmp2 = TEST_DIRECTORY + "cmp_" + name + "_linesWithSpaces_2.pdf";
            DoOcrAndCreatePdf(src, dest, ocrEngine);
            String diff = new CompareTool().CompareByContent(dest, cmp1, TARGET_DIRECTORY, "diff_");
            if (diff != null && FileUtil.FileExists(cmp2)) {
                // Second cmp is required for DocTR BY_WORDS on .NET because of different results on .NET CoreApp and .NET Framework
                NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp2, TARGET_DIRECTORY, "diff_"));
            }
            else {
                NUnit.Framework.Assert.IsNull(diff);
            }
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
