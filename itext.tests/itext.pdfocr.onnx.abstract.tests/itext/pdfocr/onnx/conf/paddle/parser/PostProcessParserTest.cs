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
using iText.Commons.Utils;
using iText.Pdfocr.Onnx.Conf.Paddle.Model;
using iText.Pdfocr.Onnx.Exceptions;
using iText.Test;

namespace iText.Pdfocr.Onnx.Conf.Paddle.Parser {
    [NUnit.Framework.Category("UnitTest")]
    public class PostProcessParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParseDbPostProcessTest() {
            DbPostProcess expected = new DbPostProcess(0.3F, 0.6F, 1.5F, 1000, false, ScoreMode.FAST, BoxType.QUAD);
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put("name", "DBPostProcess");
            NUnit.Framework.Assert.AreEqual(expected, PostProcessParser.Parse(mapping, "PostProcess"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseCtcLabelDecodeTest() {
            CtcLabelDecode expected = new CtcLabelDecode(new String[] { "a", "b" });
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put("name", "CTCLabelDecode");
            mapping.Put("character_dict", JavaUtil.ArraysAsList("a", "b"));
            NUnit.Framework.Assert.AreEqual(expected, PostProcessParser.Parse(mapping, "PostProcess"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidRootTest() {
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => PostProcessParser.Parse("invalid_type"
                , "PostProcess"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "PostProcess"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidNameTest() {
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put("name", "something unsupported");
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => PostProcessParser.Parse(mapping
                , "PostProcess"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "PostProcess.name"), ex.Message);
        }
    }
}
