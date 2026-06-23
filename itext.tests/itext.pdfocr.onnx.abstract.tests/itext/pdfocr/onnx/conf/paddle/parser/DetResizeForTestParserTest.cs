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
    public class DetResizeForTestParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParseValidFullTest() {
            DetResizeForTest expected = new DetResizeForTest(new int[] { 640, 480 }, true);
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put("image_shape", JavaUtil.ArraysAsList("640", "480"));
            mapping.Put("keep_ratio", "true");
            NUnit.Framework.Assert.AreEqual(expected, DetResizeForTestParser.Parse(mapping, "Op"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseValidDefaultsTest() {
            DetResizeForTest expected = new DetResizeForTest(null, false);
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            NUnit.Framework.Assert.AreEqual(expected, DetResizeForTestParser.Parse(mapping, "Op"));
            // null should also work
            NUnit.Framework.Assert.AreEqual(expected, DetResizeForTestParser.Parse(null, "Op"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidRootTest() {
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => DetResizeForTestParser.Parse
                ("invalid_value", "Op"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Op"), ex.Message);
        }

        public static IEnumerable<Object[]> ParseInvalidFieldTestParams() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "image_shape" }, new Object[] { "keep_ratio" }
                 });
        }

        [NUnit.Framework.TestCaseSource("ParseInvalidFieldTestParams")]
        public virtual void ParseInvalidFieldTest(String field) {
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put(field, "invalid_value");
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => DetResizeForTestParser.Parse
                (mapping, "Op"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Op." + field), ex.Message);
        }
    }
}
