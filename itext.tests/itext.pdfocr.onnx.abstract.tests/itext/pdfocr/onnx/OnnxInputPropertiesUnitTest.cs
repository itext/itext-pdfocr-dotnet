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
using iText.Pdfocr.Onnx.Exceptions;
using iText.Test;

namespace iText.Pdfocr.Onnx {
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

        [NUnit.Framework.Test]
        public virtual void InitWithInvalidImageResizeOptions() {
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new OnnxInputProperties(null));
        }

        [NUnit.Framework.Test]
        public virtual void InitWithInvalidMean() {
            ImageResizeOptions resizeOptions = new ImageResizeOptions(ImageChannelConfiguration.RGB, 800, 600);
            // null
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new OnnxInputProperties(resizeOptions, 
                null, new float[] { 1F, 2F, 3F }));
            // invalid size
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new OnnxInputProperties(resizeOptions
                , new float[] { 0.3F, 0.4F, 0.5F, 0.6F }, new float[] { 1F, 2F, 3F }));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_MEAN_CHANNEL_COUNT
                , resizeOptions.GetChannelConfiguration().GetChannelCount()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InitWithInvalidStd() {
            ImageResizeOptions resizeOptions = new ImageResizeOptions(ImageChannelConfiguration.RGB, 800, 600);
            // null
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new OnnxInputProperties(resizeOptions, 
                new float[] { 0.3F, 0.4F, 0.5F }, null));
            // invalid size
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new OnnxInputProperties(resizeOptions
                , new float[] { 0.3F, 0.4F, 0.5F }, new float[] { 1F, 2F }));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.UNEXPECTED_STD_CHANNEL_COUNT
                , resizeOptions.GetChannelConfiguration().GetChannelCount()), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InitWithInvalidBatchSize() {
            ImageResizeOptions resizeOptions = new ImageResizeOptions(ImageChannelConfiguration.RGB, 800, 600);
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new OnnxInputProperties(resizeOptions
                , 0));
            NUnit.Framework.Assert.AreEqual(PdfOcrOnnxTrExceptionMessageConstant.BATCH_SIZE_SHOULD_BE_POSITIVE, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void Valid() {
            ImageResizeOptions resizeOptions = new ImageResizeOptions(ImageChannelConfiguration.BGR, 800, 600, 1024, 768
                , 4, 2, PaddingStrategy.BOTTOM_RIGHT_WHITE);
            float[] mean = new float[] { 0.1F, 0.2F, 0.3F };
            float[] std = new float[] { 1F, 2F, 3F };
            int batchSize = 5;
            OnnxInputProperties props = new OnnxInputProperties(resizeOptions, mean, std, batchSize);
            NUnit.Framework.Assert.AreSame(resizeOptions, props.GetImageResizeOptions());
            NUnit.Framework.Assert.AreEqual(mean, props.GetMean());
            NUnit.Framework.Assert.AreEqual(mean[0], props.GetGrayMean());
            NUnit.Framework.Assert.AreEqual(mean[2], props.GetRedMean());
            NUnit.Framework.Assert.AreEqual(mean[1], props.GetGreenMean());
            NUnit.Framework.Assert.AreEqual(mean[0], props.GetBlueMean());
            NUnit.Framework.Assert.AreEqual(std, props.GetStd());
            NUnit.Framework.Assert.AreEqual(std[0], props.GetGrayStd());
            NUnit.Framework.Assert.AreEqual(std[2], props.GetRedStd());
            NUnit.Framework.Assert.AreEqual(std[1], props.GetGreenStd());
            NUnit.Framework.Assert.AreEqual(std[0], props.GetBlueStd());
            long[] expectedShape = new long[] { batchSize, resizeOptions.GetChannelConfiguration().GetChannelCount(), 
                resizeOptions.GetMinHeight(), resizeOptions.GetMinWidth() };
            NUnit.Framework.Assert.AreEqual(expectedShape, props.GetShape());
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => props.GetShape(-1));
            for (int i = 0; i < 4; ++i) {
                NUnit.Framework.Assert.AreEqual(expectedShape[i], props.GetShape(i));
            }
            NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => props.GetShape(4));
            NUnit.Framework.Assert.AreEqual(batchSize, props.GetBatchSize());
            NUnit.Framework.Assert.AreEqual(resizeOptions.GetChannelConfiguration().GetChannelCount(), props.GetChannelCount
                ());
            NUnit.Framework.Assert.AreEqual(resizeOptions.GetMinHeight(), props.GetHeight());
            NUnit.Framework.Assert.AreEqual(resizeOptions.GetMinWidth(), props.GetWidth());
            NUnit.Framework.Assert.AreEqual(resizeOptions.GetPaddingStrategy().UsesSymmetricPadding(), props.UseSymmetricPad
                ());
            NUnit.Framework.Assert.AreEqual(resizeOptions.GetPaddingStrategy(), props.GetPaddingStrategy());
            OnnxInputProperties propsCopy = new OnnxInputProperties(new ImageResizeOptions(ImageChannelConfiguration.BGR
                , 800, 600, 1024, 768, 4, 2, PaddingStrategy.BOTTOM_RIGHT_WHITE), new float[] { 0.1F, 0.2F, 0.3F }, new 
                float[] { 1F, 2F, 3F }, 5);
            NUnit.Framework.Assert.AreEqual(propsCopy.GetHashCode(), props.GetHashCode());
            NUnit.Framework.Assert.AreEqual(propsCopy, props);
            NUnit.Framework.Assert.AreNotEqual(new OnnxInputProperties(new ImageResizeOptions(ImageChannelConfiguration
                .GRAYSCALE, 800, 600)), props);
        }
    }
}
