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
using iText.Commons.Internal.Runtime;
using iText.Commons.Utils;
using iText.Pdfocr.Onnx.Conf.Paddle.Model;
using iText.Pdfocr.Onnx.Exceptions;
using iText.Test;

namespace iText.Pdfocr.Onnx.Conf.Paddle.Parser {
    [NUnit.Framework.Category("UnitTest")]
    public class DbPostProcessParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParseValidFullTest() {
            DbPostProcess expected = new DbPostProcess(0.2F, 0.8F, 1.0F, 475, true, ScoreMode.SLOW, BoxType.POLY);
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put("name", "DBPostProcess");
            mapping.Put("thresh", "0.2");
            mapping.Put("box_thresh", "0.8");
            mapping.Put("unclip_ratio", "1");
            mapping.Put("max_candidates", "475");
            mapping.Put("use_dilation", "true");
            mapping.Put("score_mode", "slow");
            mapping.Put("box_type", "poly");
            NUnit.Framework.Assert.AreEqual(expected, DbPostProcessParser.Parse(mapping, "PostProcess"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseValidDefaultsTest() {
            DbPostProcess expected = new DbPostProcess(0.3F, 0.6F, 1.5F, 1000, false, ScoreMode.FAST, BoxType.QUAD);
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put("name", "DBPostProcess");
            NUnit.Framework.Assert.AreEqual(expected, DbPostProcessParser.Parse(mapping, "PostProcess"));
        }

        public static IEnumerable<Object[]> ParseInvalidFieldTestParams() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "thresh" }, new Object[] { "box_thresh" }, new 
                Object[] { "unclip_ratio" }, new Object[] { "max_candidates" }, new Object[] { "use_dilation" }, new Object
                [] { "score_mode" }, new Object[] { "box_type" } });
        }

        [NUnit.Framework.TestCaseSource("ParseInvalidFieldTestParams")]
        public virtual void ParseInvalidFieldTest(String field) {
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put("name", "DBPostProcess");
            mapping.Put(field, "invalid_value");
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => DbPostProcessParser.Parse
                (mapping, "PostProcess"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "PostProcess." + field), ex.Message);
        }
    }
}
