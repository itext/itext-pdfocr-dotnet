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
using iText.Commons.Utils;
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Test;

namespace iText.Pdfocr.Onnxtr {
    [NUnit.Framework.Category("UnitTest")]
    public class OnnxInputPropertiesUnitTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void UnexpectedMeanChannelCountTest() {
            float[] mean = new float[] { 0.798F, 0.785F, 0.772F, 0.772F };
            float[] std = new float[] { 0.264F, 0.2749F, 0.287F };
            long[] shape = new long[] { 2, 3, 1024, 1024 };
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new OnnxInputProperties(mean, 
                std, shape, true));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_MEAN_CHANNEL_COUNT
                , 3), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void UnexpectedStdChannelCountTest() {
            float[] mean = new float[] { 0.798F, 0.785F, 0.772F };
            float[] std = new float[] { 0.264F, 0.2749F, 0.287F, 0.772F };
            long[] shape = new long[] { 2, 3, 1024, 1024 };
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new OnnxInputProperties(mean, 
                std, shape, true));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_STD_CHANNEL_COUNT
                , 3), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void UnexpectedShapeSizeTest() {
            float[] mean = new float[] { 0.798F, 0.785F, 0.772F };
            float[] std = new float[] { 0.264F, 0.2749F, 0.287F };
            long[] shape = new long[] { 2, 3, 1024 };
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new OnnxInputProperties(mean, 
                std, shape, true));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_SHAPE_SIZE
                , 4), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void UnexpectedShapeChannelCountTest() {
            float[] mean = new float[] { 0.798F, 0.785F, 0.772F };
            float[] std = new float[] { 0.264F, 0.2749F, 0.287F };
            long[] shape = new long[] { 2, 4, 1024, 1024 };
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new OnnxInputProperties(mean, 
                std, shape, true));
            NUnit.Framework.Assert.AreEqual(PdfOcrOnnxTrExceptionMessageConstant.MODEL_ONLY_SUPPORTS_RGB, e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void UnexpectedDimensionValueTest() {
            float[] mean = new float[] { 0.798F, 0.785F, 0.772F };
            float[] std = new float[] { 0.264F, 0.2749F, 0.287F };
            long[] shape = new long[] { -2, 3, 1024, 1024 };
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new OnnxInputProperties(mean, 
                std, shape, true));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_DIMENSION_VALUE
                , -2), e.Message);
        }
    }
}
