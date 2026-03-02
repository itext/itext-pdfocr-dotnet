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
    public class RecResizeImgParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParseValidFullTest() {
            RecResizeImg expected = new RecResizeImg(new int[] { 3, 64, 480 });
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put("image_shape", JavaUtil.ArraysAsList("3", "64", "480"));
            NUnit.Framework.Assert.AreEqual(expected, RecResizeImgParser.Parse(mapping, "Op"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseValidDefaultsTest() {
            RecResizeImg expected = new RecResizeImg(new int[] { 3, 48, 320 });
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            NUnit.Framework.Assert.AreEqual(expected, RecResizeImgParser.Parse(mapping, "Op"));
            // null should also work
            NUnit.Framework.Assert.AreEqual(expected, RecResizeImgParser.Parse(null, "Op"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidRootTest() {
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => RecResizeImgParser.Parse(
                "invalid_value", "Op"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Op"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidImageShapeTypeTest() {
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put("image_shape", "invalid_value");
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => RecResizeImgParser.Parse(
                mapping, "Op"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Op.image_shape"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidImageShapeLengthTest() {
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put("image_shape", JavaUtil.ArraysAsList("3", "48"));
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => RecResizeImgParser.Parse(
                mapping, "Op"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Op.image_shape"), ex.Message);
        }
    }
}
