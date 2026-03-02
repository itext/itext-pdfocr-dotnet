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
using iText.Pdfocr.Onnx.Conf.Paddle.Model;
using iText.Pdfocr.Onnx.Exceptions;
using iText.Test;

namespace iText.Pdfocr.Onnx.Conf.Paddle.Parser {
    [NUnit.Framework.Category("UnitTest")]
    public class InferenceConfigParserTest : ExtendedITextTest {
        private static readonly String BASE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        private static readonly String TEST_YAML_CONFIG = BASE_DIRECTORY + "models/paddleocr/PP-OCRv5_mobile_det_infer/inference.yml";

        [NUnit.Framework.Test]
        public virtual void ParseValidTest() {
            InferenceConfig expected = new InferenceConfig(new PreProcess(new TransformOp[] { new DecodeImage(false, ImgMode
                .BGR), new DetResizeForTest(null, false), new NormalizeImage(new float[] { 0.485F, 0.456F, 0.406F }, new 
                float[] { 0.229F, 0.224F, 0.225F }) }), new DbPostProcess(0.3F, 0.6F, 1.5F, 1000, false, ScoreMode.FAST
                , BoxType.QUAD));
            InferenceConfig actual;
            using (Stream stream = iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine(TEST_YAML_CONFIG
                ))) {
                actual = InferenceConfigParser.Parse(stream);
            }
            NUnit.Framework.Assert.AreEqual(expected, actual);
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidRootTest() {
            MemoryStream stream = CreateTestStream("\"string\"");
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => InferenceConfigParser.Parse
                (stream));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "<root>"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidPreProcessTest() {
            MemoryStream stream = CreateTestStream("PreProcess:\n" + "  unexpected: ~\n" + "PostProcess:\n" + "  name: DBPostProcess\n"
                );
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => InferenceConfigParser.Parse
                (stream));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_CONFIG_KEY
                , "PreProcess.unexpected"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidPostProcessTest() {
            MemoryStream stream = CreateTestStream("PreProcess:\n" + "  transform_ops:\n" + "  - DetLabelEncode: null\n"
                 + "PostProcess:\n" + "  name: Unsupported\n");
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => InferenceConfigParser.Parse
                (stream));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "PostProcess.name"), ex.Message);
        }

        private static MemoryStream CreateTestStream(String s) {
            return new MemoryStream(s.GetBytes(System.Text.Encoding.UTF8));
        }
    }
}
