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
    public class ImageResizeOptionsTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void InitWithInvalidChannelConfiguration() {
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new ImageResizeOptions(null, 800, 600));
        }

        [NUnit.Framework.Test]
        public virtual void InitWithInvalidPaddingStrategy() {
            NUnit.Framework.Assert.Catch(typeof(NullReferenceException), () => new ImageResizeOptions(ImageChannelConfiguration
                .BGR, 800, 600, null));
        }

        [NUnit.Framework.Test]
        public virtual void InitWithInvalidMinWidth() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new ImageResizeOptions(ImageChannelConfiguration
                .BGR, 0, 10, 100, 100));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.MIN_WIDTH_SHOULD_BE_POSITIVE
                , 0), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InitWithInvalidMinHeight() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new ImageResizeOptions(ImageChannelConfiguration
                .BGR, 10, 0, 100, 100));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.MIN_HEIGHT_SHOULD_BE_POSITIVE
                , 0), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InitWithInvalidMaxWidth() {
            // max < min
            Exception e1 = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new ImageResizeOptions(ImageChannelConfiguration
                .BGR, 10, 10, 9, 100));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.MAX_WIDTH_SHOULD_NOT_BE_LESS_THAN_MIN
                , 9), e1.Message);
            // max not multiple
            Exception e2 = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new ImageResizeOptions(ImageChannelConfiguration
                .BGR, 10, 10, 99, 100, 10, 1, PaddingStrategy.BOTTOM_RIGHT_BLACK));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.MAX_WIDTH_SHOULD_BE_A_MULTIPLE
                , 10, 99), e2.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InitWithInvalidMaxHeight() {
            // max < min
            Exception e1 = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new ImageResizeOptions(ImageChannelConfiguration
                .BGR, 10, 10, 100, 9));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.MAX_HEIGHT_SHOULD_NOT_BE_LESS_THAN_MIN
                , 9), e1.Message);
            // max not multiple
            Exception e2 = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new ImageResizeOptions(ImageChannelConfiguration
                .BGR, 10, 10, 100, 99, 1, 10, PaddingStrategy.BOTTOM_RIGHT_BLACK));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.MAX_HEIGHT_SHOULD_BE_A_MULTIPLE
                , 10, 99), e2.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InitWithInvalidWidthMultiple() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new ImageResizeOptions(ImageChannelConfiguration
                .BGR, 10, 10, 100, 100, 0, 10, PaddingStrategy.BOTTOM_RIGHT_BLACK));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.WIDTH_MULTIPLE_SHOULD_BE_POSITIVE
                , 0), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InitWithInvalidHeightMultiple() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => new ImageResizeOptions(ImageChannelConfiguration
                .BGR, 10, 10, 100, 100, 10, 0, PaddingStrategy.BOTTOM_RIGHT_BLACK));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxTrExceptionMessageConstant.HEIGHT_MULTIPLE_SHOULD_BE_POSITIVE
                , 0), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void ValidWithVariableSize() {
            ImageResizeOptions opts = new ImageResizeOptions(ImageChannelConfiguration.GRAYSCALE, 640, 480, 800, 600, 
                16, 8, PaddingStrategy.SYMMETRIC_BLACK);
            NUnit.Framework.Assert.AreEqual(ImageChannelConfiguration.GRAYSCALE, opts.GetChannelConfiguration());
            NUnit.Framework.Assert.AreEqual(640, opts.GetMinWidth());
            NUnit.Framework.Assert.AreEqual(480, opts.GetMinHeight());
            NUnit.Framework.Assert.AreEqual(800, opts.GetMaxWidth());
            NUnit.Framework.Assert.AreEqual(600, opts.GetMaxHeight());
            NUnit.Framework.Assert.AreEqual(16, opts.GetWidthMultiple());
            NUnit.Framework.Assert.AreEqual(8, opts.GetHeightMultiple());
            NUnit.Framework.Assert.AreEqual(PaddingStrategy.SYMMETRIC_BLACK, opts.GetPaddingStrategy());
            NUnit.Framework.Assert.IsFalse(opts.IsFixedSize());
            ImageResizeOptions optsCopy = new ImageResizeOptions(ImageChannelConfiguration.GRAYSCALE, 640, 480, 800, 600
                , 16, 8, PaddingStrategy.SYMMETRIC_BLACK);
            NUnit.Framework.Assert.AreEqual(optsCopy.GetHashCode(), opts.GetHashCode());
            NUnit.Framework.Assert.AreEqual(optsCopy, opts);
            NUnit.Framework.Assert.AreNotEqual(new ImageResizeOptions(ImageChannelConfiguration.GRAYSCALE, 640, 480), 
                opts);
        }

        [NUnit.Framework.Test]
        public virtual void ValidWithFixedSize() {
            ImageResizeOptions opts = new ImageResizeOptions(ImageChannelConfiguration.BGR, 1920, 1080);
            NUnit.Framework.Assert.AreEqual(ImageChannelConfiguration.BGR, opts.GetChannelConfiguration());
            NUnit.Framework.Assert.AreEqual(1920, opts.GetMinWidth());
            NUnit.Framework.Assert.AreEqual(1080, opts.GetMinHeight());
            NUnit.Framework.Assert.AreEqual(1920, opts.GetMaxWidth());
            NUnit.Framework.Assert.AreEqual(1080, opts.GetMaxHeight());
            NUnit.Framework.Assert.AreEqual(1, opts.GetWidthMultiple());
            NUnit.Framework.Assert.AreEqual(1, opts.GetHeightMultiple());
            NUnit.Framework.Assert.AreEqual(PaddingStrategy.BOTTOM_RIGHT_BLACK, opts.GetPaddingStrategy());
            NUnit.Framework.Assert.IsTrue(opts.IsFixedSize());
            ImageResizeOptions optsCopy = new ImageResizeOptions(ImageChannelConfiguration.BGR, 1920, 1080);
            NUnit.Framework.Assert.AreEqual(optsCopy.GetHashCode(), opts.GetHashCode());
            NUnit.Framework.Assert.AreEqual(optsCopy, opts);
            NUnit.Framework.Assert.AreNotEqual(new ImageResizeOptions(ImageChannelConfiguration.RGB, 1920, 1080), opts
                );
        }
    }
}
