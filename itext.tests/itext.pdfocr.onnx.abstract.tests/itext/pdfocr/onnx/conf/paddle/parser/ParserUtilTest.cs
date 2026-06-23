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
    public class ParserUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void CastToStringArrayTest() {
            // All strings are valid
            NUnit.Framework.Assert.AreEqual(new String[] { "a", "b", "c" }, ParserUtil.CastToStringArray(JavaUtil.ArraysAsList
                ((Object)"a", "b", "c"), "Obj"));
            // null instead of collection
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.CastToStringArray
                (null, "Obj"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj"), ex.Message);
            // null inside collection
            ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.CastToStringArray(JavaUtil.ArraysAsList
                ((Object)"a", null, "c"), "Obj"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.1"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetOrDefaultFloatTest() {
            // Key exists and valid
            NUnit.Framework.Assert.AreEqual(3.14F, ParserUtil.GetOrDefault(MapOf("k", "3.14"), "Obj", "k", 1.0F));
            NUnit.Framework.Assert.AreEqual(3.14F, ParserUtil.GetOrDefault(MapOf("k", 3.14), "Obj", "k", 1.0F));
            // Key does not exist
            NUnit.Framework.Assert.AreEqual(1.0F, ParserUtil.GetOrDefault(MapOf("a", "3.14"), "Obj", "b", 1.0F));
            // Key exists and invalid
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf
                ("k", "x.xx"), "Obj", "k", 1.0F));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
            ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf("k", 
                null), "Obj", "k", 1.0F));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetOrDefaultFloatArrayTest() {
            float[] defaultValue = new float[] { -1.0F, 0.0F, 1.0F };
            // Key exists and valid
            NUnit.Framework.Assert.AreEqual(new float[] { 3.14F, 13.37F }, ParserUtil.GetOrDefault(MapOf("k", JavaUtil.ArraysAsList
                ("3.14", "13.37")), "Obj", "k", defaultValue));
            // Key does not exist
            NUnit.Framework.Assert.AreSame(defaultValue, ParserUtil.GetOrDefault(MapOf("a", JavaUtil.ArraysAsList("3.14"
                , "13.37")), "Obj", "b", defaultValue));
            // Key exists and invalid
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf
                ("k", JavaUtil.ArraysAsList("3.14", "x.xx")), "Obj", "k", defaultValue));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k.1"), ex.Message);
            ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf("k", 
                "[]"), "Obj", "k", defaultValue));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
            ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf("k", 
                null), "Obj", "k", defaultValue));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetOrDefaultIntTest() {
            // Key exists and valid
            NUnit.Framework.Assert.AreEqual(42, ParserUtil.GetOrDefault(MapOf("k", "42"), "Obj", "k", -1));
            NUnit.Framework.Assert.AreEqual(42, ParserUtil.GetOrDefault(MapOf("k", 42), "Obj", "k", -1));
            // Key does not exist
            NUnit.Framework.Assert.AreEqual(-1, ParserUtil.GetOrDefault(MapOf("a", "42"), "Obj", "b", -1));
            // Key exists and invalid
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf
                ("k", "xx"), "Obj", "k", -1));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
            ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf("k", 
                null), "Obj", "k", -1));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetOrDefaultIntArrayTest() {
            int[] defaultValue = new int[] { -1, 0, 1 };
            // Key exists and valid
            NUnit.Framework.Assert.AreEqual(new int[] { 42, 777 }, ParserUtil.GetOrDefault(MapOf("k", JavaUtil.ArraysAsList
                ("42", "777")), "Obj", "k", defaultValue));
            // Key does not exist
            NUnit.Framework.Assert.AreSame(defaultValue, ParserUtil.GetOrDefault(MapOf("a", JavaUtil.ArraysAsList("42"
                , "777")), "Obj", "b", defaultValue));
            // Key exists and invalid
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf
                ("k", JavaUtil.ArraysAsList("42", "xxx")), "Obj", "k", defaultValue));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k.1"), ex.Message);
            ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf("k", 
                "[]"), "Obj", "k", defaultValue));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
            ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf("k", 
                null), "Obj", "k", defaultValue));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetOrDefaultBooleanTest() {
            // Key exists and valid
            NUnit.Framework.Assert.IsFalse(ParserUtil.GetOrDefault(MapOf("k", "false"), "Obj", "k", true));
            NUnit.Framework.Assert.IsTrue(ParserUtil.GetOrDefault(MapOf("k", "true"), "Obj", "k", false));
            NUnit.Framework.Assert.IsFalse(ParserUtil.GetOrDefault(MapOf("k", false), "Obj", "k", true));
            NUnit.Framework.Assert.IsTrue(ParserUtil.GetOrDefault(MapOf("k", true), "Obj", "k", false));
            // Key does not exist
            NUnit.Framework.Assert.IsFalse(ParserUtil.GetOrDefault(MapOf("a", "true"), "Obj", "b", false));
            // Key exists and invalid
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf
                ("k", "xxxxx"), "Obj", "k", false));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
            ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf("k", 
                null), "Obj", "k", false));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetOrDefaultScoreModeTest() {
            ScoreMode defaultValue = ScoreMode.SLOW;
            // Key exists and valid
            NUnit.Framework.Assert.AreEqual(ScoreMode.FAST, ParserUtil.GetOrDefault(MapOf("k", "fast"), "Obj", "k", defaultValue
                ));
            NUnit.Framework.Assert.AreEqual(ScoreMode.SLOW, ParserUtil.GetOrDefault(MapOf("k", "slow"), "Obj", "k", defaultValue
                ));
            // Key does not exist
            NUnit.Framework.Assert.AreEqual(defaultValue, ParserUtil.GetOrDefault(MapOf("a", "fast"), "Obj", "b", defaultValue
                ));
            // Key exists and invalid
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf
                ("k", "xxxx"), "Obj", "k", defaultValue));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
            ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf("k", 
                null), "Obj", "k", defaultValue));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetOrDefaultBoxTypeTest() {
            BoxType defaultValue = BoxType.POLY;
            // Key exists and valid
            NUnit.Framework.Assert.AreEqual(BoxType.QUAD, ParserUtil.GetOrDefault(MapOf("k", "quad"), "Obj", "k", defaultValue
                ));
            NUnit.Framework.Assert.AreEqual(BoxType.POLY, ParserUtil.GetOrDefault(MapOf("k", "poly"), "Obj", "k", defaultValue
                ));
            // Key does not exist
            NUnit.Framework.Assert.AreEqual(defaultValue, ParserUtil.GetOrDefault(MapOf("a", "quad"), "Obj", "b", defaultValue
                ));
            // Key exists and invalid
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf
                ("k", "xxx"), "Obj", "k", defaultValue));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
            ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf("k", 
                null), "Obj", "k", defaultValue));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void GetOrDefaultImgModeTest() {
            ImgMode defaultValue = ImgMode.BGR;
            // Key exists and valid
            NUnit.Framework.Assert.AreEqual(ImgMode.GRAY, ParserUtil.GetOrDefault(MapOf("k", "GRAY"), "Obj", "k", defaultValue
                ));
            NUnit.Framework.Assert.AreEqual(ImgMode.RGB, ParserUtil.GetOrDefault(MapOf("k", "RGB"), "Obj", "k", defaultValue
                ));
            NUnit.Framework.Assert.AreEqual(ImgMode.BGR, ParserUtil.GetOrDefault(MapOf("k", "BGR"), "Obj", "k", defaultValue
                ));
            // Key does not exist
            NUnit.Framework.Assert.AreEqual(defaultValue, ParserUtil.GetOrDefault(MapOf("a", "RGB"), "Obj", "b", defaultValue
                ));
            // Key exists and invalid
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf
                ("k", "XXX"), "Obj", "k", defaultValue));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
            ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => ParserUtil.GetOrDefault(MapOf("k", 
                null), "Obj", "k", defaultValue));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "Obj.k"), ex.Message);
        }

        private static IDictionary<Object, Object> MapOf(params Object[] kvPairs) {
            System.Diagnostics.Debug.Assert(kvPairs.Length % 2 == 0);
            IDictionary<Object, Object> obj = new Dictionary<Object, Object>(kvPairs.Length / 2);
            for (int i = 0; i < kvPairs.Length; i += 2) {
                obj.Put(kvPairs[i], kvPairs[i + 1]);
            }
            return obj;
        }
    }
}
