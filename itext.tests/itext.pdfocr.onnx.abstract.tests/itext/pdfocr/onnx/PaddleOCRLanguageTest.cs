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
using iText.Layout.Font;
using iText.Pdfocr;
using iText.Pdfocr.Onnx.Detection;
using iText.Pdfocr.Onnx.Recognition;
using iText.Test;

namespace iText.Pdfocr.Onnx {
    [NUnit.Framework.Category("IntegrationTest")]
    public class PaddleOCRLanguageTest : ExtendedITextTest {
        private static readonly String FONT_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/fonts/";

        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/PaddleOCRLanguageTest/";

        private static readonly String TEST_IMAGE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/PaddleOCRLanguageTest/";

        private static readonly String PADDLE_DET = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/paddleocr/PP-OCRv5_mobile_det_infer/";

        private static readonly String PADDLE_REC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/paddleocr/recognition/";

        private static readonly IDictionary<String, String> PADDLE_REC_PATHS = new Dictionary<String, String>();

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
            String[] entries = FileUtil.ListDirectoriesInDirectory(PADDLE_REC, false);
            foreach (String entryPath in entries) {
                if (FileUtil.DirectoryExists(entryPath)) {
                    FileInfo dir = new FileInfo(entryPath);
                    PADDLE_REC_PATHS.Put(dir.Name, dir.FullName);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ArabicTest() {
            String modelName = "arabic_PP-OCRv3_mobile_rec_infer";
            RunOcrTest(modelName, "arabic_01.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void BengaliTest() {
            String modelName = "devanagari_PP-OCRv5_mobile_rec_infer";
            RunOcrTest(modelName, "bengali_01.jpeg");
        }

        [NUnit.Framework.Test]
        public virtual void ChineseTest() {
            String modelName = "chinese_cht_PP-OCRv3_mobile_rec_infer";
            RunOcrTest(modelName, "chinese_01.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void French01Test() {
            String modelName = "cyrillic_PP-OCRv5_mobile_rec_infer";
            RunOcrTest(modelName, "french_01.png");
        }

        [NUnit.Framework.Test]
        public virtual void Georgian01Test() {
            String modelName = "cyrillic_PP-OCRv5_mobile_rec_infer";
            RunOcrTest(modelName, "georgian_01.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void German01Test() {
            String modelName = "PP-OCRv5_mobile_rec_infer";
            RunOcrTest(modelName, "german_01.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void GreekTest() {
            String modelName = "th_PP-OCRv5_mobile_rec_infer";
            RunOcrTest(modelName, "greek_01.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void Hindi02Test() {
            String modelName = "devanagari_PP-OCRv5_mobile_rec_infer";
            RunOcrTest(modelName, "hindi_02.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void Japanese01Test() {
            String modelName = "japan_PP-OCRv3_mobile_rec_infer";
            RunOcrTest(modelName, "japanese_01.png");
        }

        [NUnit.Framework.Test]
        public virtual void MultiLanguageTest() {
            String modelName = "PP-OCRv5_mobile_rec_infer";
            RunOcrTest(modelName, "multilang.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void RussianTest() {
            String modelName = "cyrillic_PP-OCRv5_mobile_rec_infer";
            RunOcrTest(modelName, "russian.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void SpanishTest() {
            String modelName = "PP-OCRv5_mobile_rec_infer";
            RunOcrTest(modelName, "spanish_01.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void ThaiTest() {
            String modelName = "th_PP-OCRv5_mobile_rec_infer";
            RunOcrTest(modelName, "thai_01.jpg");
        }

        private void RunOcrTest(String modelName, String imageFile) {
            IDetectionPredictor paddleDetectionPredictor = OnnxDetectionPredictor.PaddleOcr(PADDLE_DET);
            IRecognitionPredictor paddleRecognitionPredictor = OnnxRecognitionPredictor.PaddleOcr(PADDLE_REC_PATHS.Get
                (modelName));
            String cleanModelName = modelName.Replace(".onnx", "");
            String imageName = iText.Commons.Utils.StringUtil.Split(imageFile, "\\.")[0];
            String src = TEST_IMAGE_DIRECTORY + imageFile;
            String dest = TARGET_DIRECTORY + cleanModelName + "_" + imageName + ".pdf";
            String cmpTxt = TEST_DIRECTORY + imageName + ".txt";
            using (OnnxOcrEngine ocrEngine = new OnnxOcrEngine(paddleDetectionPredictor, paddleRecognitionPredictor)) {
                OnnxTestUtils.DoOcrAndCreatePdf(src, dest, ocrEngine, CreateOcrProperties());
                OnnxTestUtils.ExtractTextAndCompare(dest, cmpTxt, "Text1", 0.2);
            }
        }

        private OcrPdfCreatorProperties CreateOcrProperties() {
            FontProvider fontProvider = new FontProvider();
            fontProvider.AddDirectory(FONT_DIRECTORY);
            return new OcrPdfCreatorProperties().SetTextLayerName("Text1").SetTextColor(DeviceCmyk.MAGENTA).SetFontProvider
                (fontProvider);
        }
    }
}
