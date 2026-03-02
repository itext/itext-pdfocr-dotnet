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
    public class PreProcessParserTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ParseValidFullTest() {
            PreProcess expected = new PreProcess(new TransformOp[] { new DecodeImage(false, ImgMode.BGR), new DetResizeForTest
                (null, false), new NormalizeImage(new float[] { 0.485F, 0.456F, 0.406F }, new float[] { 0.229F, 0.224F
                , 0.225F }), new RecResizeImg(new int[] { 3, 48, 320 }) });
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            List<Object> ops = new List<Object>();
            ops.Add(CreateEmptyOp("DecodeImage"));
            ops.Add(CreateEmptyOp("DetResizeForTest"));
            ops.Add(CreateEmptyOp("NormalizeImage"));
            ops.Add(CreateEmptyOp("RecResizeImg"));
            // The ones below are ignored
            ops.Add(CreateEmptyOp("DetLabelEncode"));
            ops.Add(CreateEmptyOp("KeepKeys"));
            ops.Add(CreateEmptyOp("MultiLabelEncode"));
            ops.Add(CreateEmptyOp("ToCHWImage"));
            mapping.Put("transform_ops", ops);
            NUnit.Framework.Assert.AreEqual(expected, PreProcessParser.Parse(mapping, "PreProcess"));
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidRootTest() {
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => PreProcessParser.Parse("invalid_type"
                , "PreProcess"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "PreProcess"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ParseUnexpectedKeyTest() {
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put("transform_ops", new List<Object>());
            mapping.Put("unexpected", "key");
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => PreProcessParser.Parse(mapping
                , "PreProcess"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_CONFIG_KEY
                , "PreProcess.unexpected"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidOpsTypeTest() {
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            mapping.Put("transform_ops", new Dictionary<Object, Object>());
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => PreProcessParser.Parse(mapping
                , "PreProcess"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "PreProcess.transform_ops"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ParseUnexpectedOpTypeTest() {
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            List<Object> ops = new List<Object>();
            ops.Add(new List<Object>());
            mapping.Put("transform_ops", ops);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => PreProcessParser.Parse(mapping
                , "PreProcess"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "PreProcess.transform_ops.0"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ParseInvalidOpKeyTypeTest() {
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            List<Object> ops = new List<Object>();
            IDictionary<Object, Object> op = new Dictionary<Object, Object>();
            op.Put(42, new Dictionary<Object, Object>());
            ops.Add(op);
            mapping.Put("transform_ops", ops);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => PreProcessParser.Parse(mapping
                , "PreProcess"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "PreProcess.transform_ops.0"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ParseUnexpectedOpKeyTest() {
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            List<Object> ops = new List<Object>();
            IDictionary<Object, Object> op = new Dictionary<Object, Object>();
            op.Put("DecodeImage", new Dictionary<Object, Object>());
            op.Put("unexpected", "key");
            ops.Add(op);
            mapping.Put("transform_ops", ops);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => PreProcessParser.Parse(mapping
                , "PreProcess"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "PreProcess.transform_ops.0"), ex.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ParseUnexpectedOpTest() {
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>();
            List<Object> ops = new List<Object>();
            IDictionary<Object, Object> op = new Dictionary<Object, Object>();
            op.Put("UnexpectedOp", new Dictionary<Object, Object>());
            ops.Add(op);
            mapping.Put("transform_ops", ops);
            Exception ex = NUnit.Framework.Assert.Catch(typeof(ConfigParserException), () => PreProcessParser.Parse(mapping
                , "PreProcess"));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_VALUE_FOR_CONFIG_KEY
                , "PreProcess.transform_ops.0"), ex.Message);
        }

        private static IDictionary<Object, Object> CreateEmptyOp(String name) {
            IDictionary<Object, Object> mapping = new Dictionary<Object, Object>(1);
            mapping.Put(name, new Dictionary<Object, Object>());
            return mapping;
        }
    }
}
