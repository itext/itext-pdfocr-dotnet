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
using System.IO;
using iText.Commons.Utils;
using iText.IO.Exceptions;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Helpers;
using iText.Pdfocr.Logs;
using iText.Test;
using iText.Test.Attributes;
using iText.Test.Pdfa;

namespace iText.Pdfocr {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OcrPdfApiTest : ExtendedITextTest {
        public static readonly String DESTINATION_FOLDER = PdfHelper.TARGET_DIRECTORY + "OcrPdfApiTest/";

        public static readonly String REFERENCE_FOLDER = PdfHelper.TEST_DIRECTORY + "OcrPdfApiTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(DESTINATION_FOLDER);
        }

        [NUnit.Framework.Test]
        public virtual void NotExtractableImageTest() {
            // Take something else for the test when we start supporting it
            Exception e = NUnit.Framework.Assert.Catch(typeof(iText.IO.Exceptions.IOException), () => MakeSearchable("deviceN8bit5Channels"
                ));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(IoExceptionMessageConstant.COLOR_SPACE_IS_NOT_SUPPORTED
                , "/DeviceN"), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NotExistingFileTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => MakeSearchable("notExistingFile"
                ));
            NUnit.Framework.Assert.AreEqual(PdfOcrExceptionMessageConstant.IO_EXCEPTION_OCCURRED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void BasicTest() {
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetTextColor(DeviceCmyk.MAGENTA);
            MakeSearchable("randomImage", "basic", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TextLayerTest() {
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetTextColor(DeviceCmyk.MAGENTA);
            properties.SetTextLayerName("text");
            MakeSearchable("randomImage", "textLayer", properties);
        }

        [NUnit.Framework.Test]
        [LogMessage(PdfOcrLogMessageConstant.PAGE_SIZE_IS_NOT_APPLIED)]
        [LogMessage(PdfOcrLogMessageConstant.IMAGE_LAYER_NAME_IS_NOT_APPLIED)]
        public virtual void NotRelevantPropertiesTest() {
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetTextColor(DeviceCmyk.MAGENTA);
            // Page size and image layer should not take any effect
            properties.SetPageSize(new Rectangle(500, 500));
            properties.SetImageLayerName("image");
            MakeSearchable("randomImage", "notRelevantProperties", properties);
        }

        [NUnit.Framework.Test]
        public virtual void TitleAndLangTest() {
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetTextColor(DeviceCmyk.MAGENTA);
            properties.SetTitle("Title");
            properties.SetPdfLang("de-DE");
            MakeSearchable("randomImage", "titleAndLang", properties);
        }

        [NUnit.Framework.Test]
        public virtual void NoReaderTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new MemoryStream()));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => MakeSearchable("noReader", pdfDoc
                ));
            NUnit.Framework.Assert.AreEqual(PdfOcrExceptionMessageConstant.PDF_DOCUMENT_MUST_BE_OPENED_IN_STAMPING_MODE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void NoWriterTest() {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(PdfHelper.GetPdfsTestDirectory() + "randomImage.pdf"));
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => MakeSearchable("noWriter", pdfDoc
                ));
            NUnit.Framework.Assert.AreEqual(PdfOcrExceptionMessageConstant.PDF_DOCUMENT_MUST_BE_OPENED_IN_STAMPING_MODE
                , e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void TaggedPdfTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => MakeSearchable("pdfUA"));
            NUnit.Framework.Assert.AreEqual(PdfOcrExceptionMessageConstant.TAGGED_PDF_IS_NOT_SUPPORTED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA3bTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => MakeSearchable("pdfA3b"));
            NUnit.Framework.Assert.AreEqual(PdfOcrExceptionMessageConstant.PDFA_IS_NOT_SUPPORTED, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void PdfA3bNoValidationTest() {
            String path = PdfHelper.GetPdfsTestDirectory() + "pdfA3b.pdf";
            String expectedPdfPath = REFERENCE_FOLDER + "cmp_pdfA3b.pdf";
            String resultPdfPath = DESTINATION_FOLDER + "pdfA3b.pdf";
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            properties.SetTextColor(DeviceCmyk.MAGENTA);
            OcrPdfCreator ocrPdfCreator = new _OcrPdfCreator_154(new CustomOcrEngine(), properties);
            ocrPdfCreator.MakePdfSearchable(new FileInfo(path), new FileInfo(resultPdfPath), null);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, DESTINATION_FOLDER
                , "diff_"));
            new VeraPdfValidator().Validate(resultPdfPath);
        }

        private sealed class _OcrPdfCreator_154 : OcrPdfCreator {
            public _OcrPdfCreator_154(IOcrEngine baseArg1, OcrPdfCreatorProperties baseArg2)
                : base(baseArg1, baseArg2) {
            }

            protected internal override void ValidateInputPdfDocument(PdfDocument pdfDoc) {
            }
        }

        private static void MakeSearchable(String fileName) {
            MakeSearchable(fileName, null, new OcrPdfCreatorProperties());
        }

        private static void MakeSearchable(String fileName, String outFileName, OcrPdfCreatorProperties properties
            ) {
            if (outFileName == null) {
                outFileName = fileName;
            }
            String path = PdfHelper.GetPdfsTestDirectory() + fileName + ".pdf";
            String expectedPdfPath = REFERENCE_FOLDER + "cmp_" + outFileName + ".pdf";
            String resultPdfPath = DESTINATION_FOLDER + outFileName + ".pdf";
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(new CustomOcrEngine(), properties);
            ocrPdfCreator.MakePdfSearchable(new FileInfo(path), new FileInfo(resultPdfPath));
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, DESTINATION_FOLDER
                , "diff_"));
        }

        private static void MakeSearchable(String fileName, PdfDocument pdfDoc) {
            String expectedPdfPath = REFERENCE_FOLDER + "cmp_" + fileName + ".pdf";
            String resultPdfPath = DESTINATION_FOLDER + fileName + ".pdf";
            OcrPdfCreatorProperties properties = new OcrPdfCreatorProperties();
            OcrPdfCreator ocrPdfCreator = new OcrPdfCreator(new CustomOcrEngine(), properties);
            ocrPdfCreator.MakePdfSearchable(pdfDoc);
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(resultPdfPath, expectedPdfPath, DESTINATION_FOLDER
                , "diff_"));
        }
    }
}
