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
using iText.Pdfocr.Onnxtr.Conf.Paddle.Model;
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Test;

namespace iText.Pdfocr.Onnxtr.Conf.Paddle.Parser {
    [NUnit.Framework.Category("UnitTest")]
    public class NormalizeImageParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParseValidFullTest() {
            NormalizeImage expected = new NormalizeImage(new float[] { -1.0F, 0.0F, 1.0F }, new float[] { 2.0F, 4.0F, 
                8.0F });
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put("mean", JavaUtil.ArraysAsList("-1.", "0.", "1"));
            mapping.Put("std", JavaUtil.ArraysAsList("2.", "4.", "8"));
            NUnit.Framework.Assert.AreEqual(expected, NormalizeImageParser.Parse(mapping, "Op"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseValidDefaultsTest() {
            NormalizeImage expected = new NormalizeImage(new float[] { 0.485F, 0.456F, 0.406F }, new float[] { 0.229F, 
                0.224F, 0.225F });
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            NUnit.Framework.Assert.AreEqual(expected, NormalizeImageParser.Parse(mapping, "Op"));
            // null should also work
            NUnit.Framework.Assert.AreEqual(expected, NormalizeImageParser.Parse(null, "Op"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidRootTest() {
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => NormalizeImageParser.Parse
                ("invalid_value", "Op"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Op"), ex.Message);
        }

        public static IEnumerable<Object[]> ParseInvalidFieldTestParams() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "mean" }, new Object[] { "std" } });
        }

        [NUnit.Framework.TestCaseSource("ParseInvalidFieldTestParams")]
        public virtual void ParseInvalidFieldTest(String field) {
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put(field, "invalid_value");
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => NormalizeImageParser.Parse
                (mapping, "Op"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Op." + field), ex.Message);
        }
    }
}
