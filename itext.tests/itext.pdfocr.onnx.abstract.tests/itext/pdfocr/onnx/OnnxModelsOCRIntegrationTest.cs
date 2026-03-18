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
using System.Linq;
using iText.Commons.Utils;
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Onnx.Util;
using iText.Test;

namespace iText.Pdfocr.Onnx {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxModelsOCRIntegrationTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/OnnxModelsOCRIntegrationTest/";

        private static readonly String TEST_IMAGE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxModelsOCRIntegrationTest/";

        public static IEnumerable<Object[]> OcrEngines() {
            return JavaUtil.ArraysToEnumerable(OcrEngineType.All()).Select((type) => new Object[] { type }).ToList();
        }

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
        }

        [NUnit.Framework.TestCaseSource("OcrEngines")]
        public virtual void BmpTest(OcrEngineType engineType) {
            IOcrEngine ocrEngine = engineType.Get();
            String name = engineType.GetDisplayName();
            String src = TEST_IMAGE_DIRECTORY + "englishText.bmp";
            String dest = TARGET_DIRECTORY + name + "_bmp.pdf";
            String cmp = TEST_DIRECTORY + "cmp_" + name + "_bmp.pdf";
            String cmpTxt = TEST_DIRECTORY + "bmp" + name + ".txt";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, ocrEngine);
            OnnxTestUtils.ComparePdfs(dest, cmp, TARGET_DIRECTORY);
            ExtractTextAndCompare(dest, cmpTxt);
        }

        [NUnit.Framework.TestCaseSource("OcrEngines")]
        public virtual void InvoiceThaiTest(OcrEngineType engineType) {
            IOcrEngine ocrEngine = engineType.Get();
            String name = engineType.GetDisplayName();
            String cmp = TEST_DIRECTORY + "cmp_" + name + "_invoice_front_thai.pdf";
            String src = TEST_IMAGE_DIRECTORY + "invoice_front_thai.jpg";
            String dest = TARGET_DIRECTORY + name + "_invoice_front_thai.pdf";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, ocrEngine);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        [NUnit.Framework.TestCaseSource("OcrEngines")]
        public virtual void WeirdWordsGifTest(OcrEngineType engineType) {
            IOcrEngine ocrEngine = engineType.Get();
            String name = engineType.GetDisplayName();
            String src = TEST_IMAGE_DIRECTORY + "weirdwords.gif";
            String dest = TARGET_DIRECTORY + name + "_weirdwords.pdf";
            String cmp = TEST_DIRECTORY + "cmp_" + name + "_weirdwords.pdf";
            String cmpTxt = TEST_DIRECTORY + "weirdwords.txt";
            OnnxTestUtils.DoOcrAndCreatePdf(src, dest, ocrEngine);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
            OnnxTestUtils.ExtractTextAndCompare(dest, cmpTxt, "Text1", 0.7);
        }

        private void ExtractTextAndCompare(String dest, String cmpTxt) {
            OnnxTestUtils.ExtractTextAndCompare(dest, cmpTxt, "Text1", 0.16);
        }
    }
}
