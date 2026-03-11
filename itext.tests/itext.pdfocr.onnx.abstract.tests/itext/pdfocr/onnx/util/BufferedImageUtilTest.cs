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
using System.IO;
using System.Runtime.InteropServices;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Pdfocr.Onnx.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class BufferedImageUtilTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/onnx/util/BufferedImageUtilTest/";

        [NUnit.Framework.Test]
        public virtual void ToBchwInputGrayBasicTest() {
            // Intent is to test normalization and BufferedImage to
            // FloatBufferMdArray conversion, no resizing is expected here
            long[] expectedShape = new long[] { 2, 1, 2, 3 };
            float[] expectedData = new float[] { -0.333333F, -0.249673F, -0.166013F, -0.307190F, -0.223529F, -0.139869F
                , -0.082353F, 0.001307F, 0.084967F, -0.056209F, 0.027451F, 0.111111F };
            ICollection<IronSoftware.Drawing.AnyBitmap> images = JavaUtil.ArraysAsList(NewGrayImage(3, 2, new byte[] { 
                0x00, 0x10, 0x20, 0x05, 0x15, 0x25 }), NewGrayImage(3, 2, new byte[] { 0x30, 0x40, 0x50, 0x35, 0x45, 0x55
                 }));
            OnnxInputProperties props = new OnnxInputProperties(new ImageResizeOptions(ImageChannelConfiguration.GRAYSCALE
                , 3, 2), new float[] { 0.25F }, new float[] { 0.75F }, 3);
            ToBchwInputBasicTest(expectedShape, expectedData, images, props);
        }

        [NUnit.Framework.Test]
        public virtual void ToBchwInputRgbBasicTest() {
            long[] expectedShape = new long[] { 2, 3, 1, 2 };
            float[] expectedData = new float[] { 0.616073F, -0.211813F, -2.294872F, 0.148567F, -0.015602F, 0.549441F, 
                -0.308642F, 0.199710F, -0.228507F, 0.978130F, 0.815096F, 0.532574F };
            IList<IronSoftware.Drawing.AnyBitmap> images = JavaUtil.ArraysAsList(NewRgbImage(2, 1, new int[] { 0xBF2220
                , 0x14C4A6 }), NewRgbImage(2, 1, new int[] { 0x00ABE5, 0x69FBA2 }));
            OnnxInputProperties props = new OnnxInputProperties(new ImageResizeOptions(ImageChannelConfiguration.RGB, 
                2, 1), new float[] { 0.25F, 0.73F, 0.14F }, new float[] { 0.81F, 0.26F, 0.93F }, 4);
            ToBchwInputBasicTest(expectedShape, expectedData, images, props);
        }

        [NUnit.Framework.Test]
        public virtual void ToBchwInputBgrBasicTest() {
            long[] expectedShape = new long[] { 2, 3, 1, 2 };
            float[] expectedData = new float[] { 0.423770F, 0.403762F, 2.203361F, 1.564706F, -1.071012F, -0.625861F, -
                0.016407F, 0.679872F, 1.844818F, 2.024090F, -1.696343F, 0.200848F };
            IList<IronSoftware.Drawing.AnyBitmap> images = JavaUtil.ArraysAsList(NewRgbImage(2, 1, new int[] { 0x83F0A2
                , 0xADB79D }), NewRgbImage(2, 1, new int[] { 0x48D034, 0xFBE0E2 }));
            OnnxInputProperties props = new OnnxInputProperties(new ImageResizeOptions(ImageChannelConfiguration.BGR, 
                2, 1), new float[] { 0.22F, 0.17F, 0.91F }, new float[] { 0.98F, 0.35F, 0.37F }, 5);
            ToBchwInputBasicTest(expectedShape, expectedData, images, props);
        }

        public static IEnumerable<Object[]> ResizeTestParams() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "resize_120_80_brb.png", 120, 80, PaddingStrategy
                .BOTTOM_RIGHT_BLACK }, new Object[] { "resize_120_80_brg.png", 120, 80, PaddingStrategy.BOTTOM_RIGHT_GRAY
                 }, new Object[] { "resize_120_80_brw.png", 120, 80, PaddingStrategy.BOTTOM_RIGHT_WHITE }, new Object[
                ] { "resize_120_80_bre.png", 120, 80, PaddingStrategy.BOTTOM_RIGHT_EDGE }, new Object[] { "resize_120_80_sb.png"
                , 120, 80, PaddingStrategy.SYMMETRIC_BLACK }, new Object[] { "resize_120_80_sg.png", 120, 80, PaddingStrategy
                .SYMMETRIC_GRAY }, new Object[] { "resize_120_80_sw.png", 120, 80, PaddingStrategy.SYMMETRIC_WHITE }, 
                new Object[] { "resize_120_80_se.png", 120, 80, PaddingStrategy.SYMMETRIC_EDGE }, new Object[] { "resize_100_80_brb.png"
                , 100, 80, PaddingStrategy.BOTTOM_RIGHT_BLACK }, new Object[] { "resize_100_80_brg.png", 100, 80, PaddingStrategy
                .BOTTOM_RIGHT_GRAY }, new Object[] { "resize_100_80_brw.png", 100, 80, PaddingStrategy.BOTTOM_RIGHT_WHITE
                 }, new Object[] { "resize_100_80_bre.png", 100, 80, PaddingStrategy.BOTTOM_RIGHT_EDGE }, new Object[]
                 { "resize_100_80_sb.png", 100, 80, PaddingStrategy.SYMMETRIC_BLACK }, new Object[] { "resize_100_80_sg.png"
                , 100, 80, PaddingStrategy.SYMMETRIC_GRAY }, new Object[] { "resize_100_80_sw.png", 100, 80, PaddingStrategy
                .SYMMETRIC_WHITE }, new Object[] { "resize_100_80_se.png", 100, 80, PaddingStrategy.SYMMETRIC_EDGE } }
                );
        }

        [NUnit.Framework.TestCaseSource("ResizeTestParams")]
        public virtual void ResizeTest(String cmpFileName, int width, int height, PaddingStrategy paddingStrategy) {
            IronSoftware.Drawing.AnyBitmap inputImage = IronSoftware.Drawing.AnyBitmap.FromFile(new FileInfo(TEST_DIRECTORY
                 + "resize_base.png").FullName);
            IronSoftware.Drawing.AnyBitmap expectedImage = IronSoftware.Drawing.AnyBitmap.FromFile(new FileInfo(TEST_DIRECTORY
                 + cmpFileName).FullName);
            ImageResizeOptions resizeOptions = new ImageResizeOptions(BufferedImageUtil.GetImageType(expectedImage), 
                width, height, paddingStrategy);
            IronSoftware.Drawing.AnyBitmap actualImage = BufferedImageUtil.Resize(inputImage, width, height, resizeOptions);
            AssertImagesEqual(expectedImage, actualImage);
        }

        public static IEnumerable<Object[]> CalcOutputDimensionsSingleTestParams() {
            return JavaUtil.ArraysAsList(new Object[][] { 
                        // Fixed output size: should always be the same
                        new Object[] { new Dimensions2D(800, 600), new Dimensions2D(20, 30), new ImageResizeOptions(ImageChannelConfiguration
                .RGB, 800, 600) }, new Object[] { new Dimensions2D(800, 600), new Dimensions2D(30, 20), new ImageResizeOptions
                (ImageChannelConfiguration.RGB, 800, 600) }, new Object[] { new Dimensions2D(800, 600), new Dimensions2D
                (1000, 900), new ImageResizeOptions(ImageChannelConfiguration.RGB, 800, 600) }, new Object[] { new Dimensions2D
                (800, 600), new Dimensions2D(900, 1000), new ImageResizeOptions(ImageChannelConfiguration.RGB, 800, 600
                ) }, 
                        // Variable output size with matching input: input should remain as-is
                        new Object[] { new Dimensions2D(250, 350), new Dimensions2D(250, 350), new ImageResizeOptions(ImageChannelConfiguration
                .RGB, 200, 200, 400, 400) }, 
                        // Variable output size with non-matching input: input should get resized/padded
                        new Object[] { new Dimensions2D(150, 300), new Dimensions2D(75, 150), new ImageResizeOptions(ImageChannelConfiguration
                .RGB, 100, 300, 200, 400) }, new Object[] { new Dimensions2D(100, 400), new Dimensions2D(75, 300), new 
                ImageResizeOptions(ImageChannelConfiguration.RGB, 100, 300, 200, 400) }, new Object[] { new Dimensions2D
                (200, 300), new Dimensions2D(100, 150), new ImageResizeOptions(ImageChannelConfiguration.RGB, 100, 300
                , 200, 400) }, new Object[] { new Dimensions2D(150, 400), new Dimensions2D(300, 800), new ImageResizeOptions
                (ImageChannelConfiguration.RGB, 100, 300, 200, 400) }, new Object[] { new Dimensions2D(200, 300), new 
                Dimensions2D(400, 600), new ImageResizeOptions(ImageChannelConfiguration.RGB, 100, 300, 200, 400) }, new 
                Object[] { new Dimensions2D(200, 300), new Dimensions2D(600, 600), new ImageResizeOptions(ImageChannelConfiguration
                .RGB, 100, 300, 200, 400) }, new Object[] { new Dimensions2D(100, 400), new Dimensions2D(50, 300), new 
                ImageResizeOptions(ImageChannelConfiguration.RGB, 100, 300, 200, 400) }, new Object[] { new Dimensions2D
                (200, 300), new Dimensions2D(150, 200), new ImageResizeOptions(ImageChannelConfiguration.RGB, 100, 300
                , 200, 400) }, 
                        // Multiple sanity test
                        new Object[] { new Dimensions2D(155, 350), new Dimensions2D(152, 343), new ImageResizeOptions(ImageChannelConfiguration
                .RGB, 100, 300, 200, 400, 5, 10, PaddingStrategy.BOTTOM_RIGHT_BLACK) } });
        }

        [NUnit.Framework.TestCaseSource("CalcOutputDimensionsSingleTestParams")]
        public virtual void CalcOutputDimensionsSingleTest(Dimensions2D expectedOutputDimensions, Dimensions2D inputDimensions
            , ImageResizeOptions resizeOptions) {
            IronSoftware.Drawing.AnyBitmap img = NewBlankInputImage(inputDimensions);
            Dimensions2D actualOutputDimensions = BufferedImageUtil.CalcOutputDimensions(img, resizeOptions);
            NUnit.Framework.Assert.AreEqual(expectedOutputDimensions, actualOutputDimensions);
        }

        public static IEnumerable<Object[]> TruncateToRatioTestParams() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { new Dimensions2D(100, 20), new Dimensions2D(100
                , 20), 8.0 }, new Object[] { new Dimensions2D(160, 20), new Dimensions2D(1000, 20), 8.0 }, new Object[
                ] { new Dimensions2D(100, 800), new Dimensions2D(100, 2000), 8.0 } });
        }

        [NUnit.Framework.TestCaseSource("TruncateToRatioTestParams")]
        public virtual void TruncateToRatioTest(Dimensions2D expectedSize, Dimensions2D inputSize, double ratioLimit
            ) {
            IronSoftware.Drawing.AnyBitmap img = NewBlankInputImage(inputSize);
            IronSoftware.Drawing.AnyBitmap truncated = BufferedImageUtil.TruncateToRatio(img, ratioLimit);
            NUnit.Framework.Assert.AreEqual(expectedSize.GetWidth(), BufferedImageUtil.GetWidth(truncated));
            NUnit.Framework.Assert.AreEqual(expectedSize.GetHeight(), BufferedImageUtil.GetHeight(truncated));
        }

        private static void ToBchwInputBasicTest(long[] expectedShape, float[] expectedData, ICollection<IronSoftware.Drawing.AnyBitmap
            > images, OnnxInputProperties props) {
            FloatBufferMdArray result = BufferedImageUtil.ToBchwInput(images, props);
            NUnit.Framework.Assert.AreEqual(expectedShape, result.GetShape());
            float[] actualData = result.GetData().Array();
            iText.Test.TestUtil.AreEqual(expectedData, actualData, 1E-6F);
        }

        private static IronSoftware.Drawing.AnyBitmap NewBlankInputImage(Dimensions2D dims) {
            return new SkiaSharp.SKBitmap(dims.GetWidth(), dims.GetHeight(), 
                SkiaSharp.SKColorType.Rgb888x, SkiaSharp.SKAlphaType.Unpremul);
        }

        private static IronSoftware.Drawing.AnyBitmap NewGrayImage(int width, int height, byte[] pixels) {
            SkiaSharp.SKBitmap img = new SkiaSharp.SKBitmap(width, height,
                SkiaSharp.SKColorType.Gray8, SkiaSharp.SKAlphaType.Unpremul);
            using (SkiaSharp.SKPixmap pixmap = img.PeekPixels()) {
                IntPtr ptr = pixmap.GetPixels();
                int stride = pixmap.RowBytes;

                for (int y = 0; y < height; y++) {
                    int sourceIndex = y * width;
                    IntPtr destPtr = IntPtr.Add(ptr, y * stride);
                    Marshal.Copy(pixels, sourceIndex, destPtr, width);
                }
            }

            return img;
        }

        private static IronSoftware.Drawing.AnyBitmap NewRgbImage(int width, int height, int[] pixels) {
            SkiaSharp.SKBitmap img = new SkiaSharp.SKBitmap(width, height,
                SkiaSharp.SKColorType.Rgba8888, SkiaSharp.SKAlphaType.Unpremul);
            SetDataElements(width, height, pixels, img);
            return img;
        }

        private static void SetDataElements(int width, int height, int[] pixels, SkiaSharp.SKBitmap img) {
            using (SkiaSharp.SKPixmap pixmap = img.PeekPixels()) {
                int stride = pixmap.RowBytes;

                byte[] buffer = new byte[height * stride];

                for (int y = 0; y < height; y++) {
                    int rowOffset = y * stride;
                    for (int x = 0; x < width; x++) {
                        int pixelIndex = y * width + x;
                        int pixelValue = pixels[pixelIndex];

                        byte r = (byte)((pixelValue >> 16) & 0xFF);
                        byte g = (byte)((pixelValue >> 8) & 0xFF);
                        byte b = (byte)(pixelValue & 0xFF);

                        int bufferPos = rowOffset + x * 4;

                        buffer[bufferPos] = r;
                        buffer[bufferPos + 1] = g;
                        buffer[bufferPos + 2] = b;
                        buffer[bufferPos + 3] = 0xFF;
                    }
                }

                Marshal.Copy(buffer, 0, pixmap.GetPixels(), buffer.Length);
            }
        }

        private static void AssertImagesEqual(IronSoftware.Drawing.AnyBitmap expected, IronSoftware.Drawing.AnyBitmap
             actual) {
            NUnit.Framework.Assert.AreEqual(BufferedImageUtil.GetWidth(expected), BufferedImageUtil.GetWidth(actual), 
                "Image width differs");
            NUnit.Framework.Assert.AreEqual(BufferedImageUtil.GetHeight(expected), BufferedImageUtil.GetHeight(actual)
                , "Image height differs");
            NUnit.Framework.Assert.AreEqual(expected.GetType(), actual.GetType(), "Image type differs");
            // Kind of slow, but should be fine for small test images
            int width = BufferedImageUtil.GetWidth(expected);
            int height = BufferedImageUtil.GetHeight(expected);
            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width; ++x) {
                    NUnit.Framework.Assert.IsTrue(
                        EqualsWithFuzziness(expected.GetPixel(x, y), actual.GetPixel(x, y), 14), 
                        String.Format("Image pixel value differs at ({0}, {1})", 
                            expected.GetPixel(x, y), actual.GetPixel(x, y)));
                }
            }
        }

        private static bool EqualsWithFuzziness(IronSoftware.Drawing.Color c1, IronSoftware.Drawing.Color c2, 
            float fuzziness) {
            if (c1 == null && c2 == null) {
                return true;
            }

            if (c1 == null || c2 == null) {
                return false;
            }

            return Math.Abs(c1.R - c2.R) <= fuzziness &&
                   Math.Abs(c1.G - c2.G) <= fuzziness &&
                   Math.Abs(c1.B - c2.B) <= fuzziness &&
                   Math.Abs(c1.A - c2.A) <= fuzziness;
        }
    }
}
