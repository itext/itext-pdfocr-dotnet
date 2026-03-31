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
using iText.Kernel.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Logs;
using iText.Pdfocr.Onnx.Util;
using iText.Test;
using iText.Test.Attributes;

namespace iText.Pdfocr.Onnx {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxMultiFilesIntegrationTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/OnnxMultiFilesIntegrationTest/";

        private static readonly String TEST_IMAGE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/OnnxMultiFilesIntegrationTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfOcrLogMessageConstant.COULD_NOT_FIND_CORRESPONDING_GLYPH_TO_UNICODE_CHARACTER, Ignore = true
            )]
        public virtual void MultiFilesTest() {
            IList<FileInfo> files = JavaUtil.ArraysAsList(new FileInfo(TEST_IMAGE_DIRECTORY + "german_01.jpg"), new FileInfo
                (TEST_IMAGE_DIRECTORY + "noisy_01.png"), new FileInfo(TEST_IMAGE_DIRECTORY + "nümbérs.jpg"), new FileInfo
                (TEST_IMAGE_DIRECTORY + "example_04.png"));
            String dest = TARGET_DIRECTORY + "multiFiles.pdf";
            String cmp = TEST_DIRECTORY + "cmp_multiFiles.pdf";
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(OcrEngineType.PADDLE.Get(), CreatorProperties());
            using (PdfWriter writer = new PdfWriter(dest)) {
                ocrPdfCreator.CreatePdf(files, writer).Close();
            }
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(dest, cmp, TARGET_DIRECTORY, "diff_"));
        }

        private OcrPdfCreatorProperties CreatorProperties() {
            OcrPdfCreatorProperties ocrPdfCreatorProperties = new OcrPdfCreatorProperties();
            ocrPdfCreatorProperties.SetTextLayerName("Text1");
            ocrPdfCreatorProperties.SetTextColor(DeviceCmyk.CYAN);
            ocrPdfCreatorProperties.SetImageLayerName("Image1");
            return ocrPdfCreatorProperties;
        }
    }
}
