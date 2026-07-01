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
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Layout.Font;
using iText.Pdfocr;
using iText.Pdfocr.Onnx.Detection;
using iText.Pdfocr.Onnx.Recognition;
using iText.Test;

namespace iText.Pdfocr.Onnx {
    [NUnit.Framework.Category("IntegrationTest")]
    public class EasyOCRLanguageTest : ExtendedITextTest {
        private static readonly String FONT_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/fonts/";

        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/EasyOCRLanguageTest/";

        private static readonly String TEST_IMAGE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/images/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/EasyOCRLanguageTest/";

        private static readonly String EASY_DET = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/easyocr/craft_mlt_25k.onnx";

        private static readonly String EASY_REC = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/easyocr/recognition/";

        private static readonly IDictionary<String, String> EASY_REC_PATHS = new Dictionary<String, String>();

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
            IList<FileInfo> fileList = new List<FileInfo>();
            String[] entries = FileUtil.ListFilesInDirectory(EASY_REC, false);
            foreach (String entryPath in entries) {
                if (entryPath.EndsWith(".onnx")) {
                    fileList.Add(new FileInfo(entryPath));
                }
            }
            if (!fileList.IsEmpty()) {
                foreach (FileInfo file in fileList) {
                    EASY_REC_PATHS.Put(file.Name, file.FullName);
                }
            }
        }

        [NUnit.Framework.Test]
        public virtual void ChineseTest() {
            String modelName = "zh_sim_g2.onnx";
            RunOcrTest(modelName, "chinese_01.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void French01Test() {
            String modelName = "latin_g2.onnx";
            RunOcrTest(modelName, "french_01.png");
        }

        [NUnit.Framework.Test]
        public virtual void Georgian01Test() {
            String modelName = "cyrillic_g2.onnx";
            RunOcrTest(modelName, "georgian_01.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void German01Test() {
            String modelName = "latin_g2.onnx";
            RunOcrTest(modelName, "german_01.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void GreekTest() {
            String modelName = "zh_sim_g2.onnx";
            RunOcrTest(modelName, "greek_01.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void Japanese01Test() {
            String modelName = "japanese_g2.onnx";
            RunOcrTest(modelName, "japanese_01.png");
        }

        [NUnit.Framework.Test]
        public virtual void MultiLanguageTest() {
            String modelName = "latin_g2.onnx";
            RunOcrTest(modelName, "multilang.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void RussianTest() {
            String modelName = "cyrillic_g2.onnx";
            RunOcrTest(modelName, "russian.jpg");
        }

        [NUnit.Framework.Test]
        public virtual void SpanishTest() {
            String modelName = "latin_g2.onnx";
            RunOcrTest(modelName, "spanish_01.jpg");
        }

        private static EasyOcrMapper ResolveMapper(String modelName) {
            if (modelName.Contains("latin")) {
                return EasyOcrMapper.LATIN_G2;
            }
            if (modelName.Contains("japanese")) {
                return EasyOcrMapper.JAPANESE_G2;
            }
            if (modelName.Contains("zh")) {
                return EasyOcrMapper.ZH_SIM_G2;
            }
            if (modelName.Contains("cyrillic")) {
                return EasyOcrMapper.CYRILLIC_G2;
            }
            throw new ArgumentException("Cannot determine EasyOcrMapper for model: " + modelName);
        }

        private OcrPdfCreatorProperties CreateOcrProperties() {
            FontProvider fontProvider = new FontProvider();
            fontProvider.AddDirectory(FONT_DIRECTORY);
            return new OcrPdfCreatorProperties().SetTextLayerName("Text1").SetTextColor(DeviceCmyk.MAGENTA).SetFontProvider
                (fontProvider);
        }

        private void RunOcrTest(String modelName, String imageFile) {
            IDetectionPredictor detectionPredictor = OnnxDetectionPredictor.EasyOcr(EASY_DET);
            EasyOcrMapper easyOcrMapper = ResolveMapper(modelName);
            IRecognitionPredictor recognitionPredictor = OnnxRecognitionPredictor.EasyOcr(EASY_REC_PATHS.Get(modelName
                ), easyOcrMapper);
            using (OnnxOcrEngine engine = new OnnxOcrEngine(detectionPredictor, recognitionPredictor)) {
                String cleanModelName = modelName.Replace(".onnx", "");
                String cleanImageName = iText.Commons.Utils.StringUtil.Split(imageFile, "\\.")[0];
                String src = TEST_IMAGE_DIRECTORY + imageFile;
                String dest = TARGET_DIRECTORY + cleanModelName + "_" + cleanImageName + ".pdf";
                String cmpTxt = TEST_DIRECTORY + cleanImageName + ".txt";
                OnnxTestUtils.DoOcrAndCreatePdf(src, dest, engine, CreateOcrProperties());
                OnnxTestUtils.ExtractTextAndCompare(dest, cmpTxt, "Text1", 0.2);
            }
        }
    }
}
