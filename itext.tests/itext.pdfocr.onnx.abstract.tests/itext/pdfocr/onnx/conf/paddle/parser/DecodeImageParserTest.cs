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
    public class DecodeImageParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParseValidFullTest() {
            DecodeImage expected = new DecodeImage(true, ImgMode.GRAY);
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put("channel_first", "true");
            mapping.Put("img_mode", "GRAY");
            NUnit.Framework.Assert.AreEqual(expected, DecodeImageParser.Parse(mapping, "Op"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseValidDefaultsTest() {
            DecodeImage expected = new DecodeImage(false, ImgMode.BGR);
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            NUnit.Framework.Assert.AreEqual(expected, DecodeImageParser.Parse(mapping, "Op"));
            // null should also work
            NUnit.Framework.Assert.AreEqual(expected, DecodeImageParser.Parse(null, "Op"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidRootTest() {
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => DecodeImageParser.Parse("invalid_value"
                , "Op"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Op"), ex.Message);
        }

        public static IEnumerable<Object[]> ParseInvalidFieldTestParams() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "channel_first" }, new Object[] { "img_mode" }
                 });
        }

        [NUnit.Framework.TestCaseSource("ParseInvalidFieldTestParams")]
        public virtual void ParseInvalidFieldTest(String field) {
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put(field, "invalid_value");
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => DecodeImageParser.Parse(mapping
                , "Op"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Op." + field), ex.Message);
        }
    }
}
