/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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

using System.Collections.Generic;
using System.Runtime.InteropServices;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Pdfocr.Onnxtr.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class BufferedImageUtilTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void ToBchwInputRgbBasicTest() {
            long[] expectedShape = new long[] { 2, 3, 1, 2 };
            float[] expectedData = new float[] { 0.616073F, -0.211813F, -2.294872F, 0.148567F, -0.015602F, 0.549441F, 
                -0.308642F, 0.199710F, -0.228507F, 0.978130F, 0.815096F, 0.532574F };
            IList<IronSoftware.Drawing.AnyBitmap> images = JavaUtil.ArraysAsList(NewRgbImage(2, 1, new int[] { 0xBF2220
                , 0x14C4A6 }), NewRgbImage(2, 1, new int[] { 0x00ABE5, 0x69FBA2 }));
            OnnxInputProperties props = new OnnxInputProperties(new float[] { 0.25F, 0.73F, 0.14F }, new float[] { 0.81F
                , 0.26F, 0.93F }, new long[] { 4, 3, 1, 2 }, false);
            ToBchwInputBasicTest(expectedShape, expectedData, images, props);
        }

        private static void ToBchwInputBasicTest(long[] expectedShape, float[] expectedData, ICollection<IronSoftware.Drawing.AnyBitmap
            > images, OnnxInputProperties props) {
            FloatBufferMdArray result = BufferedImageUtil.ToBchwInput(images, props);
            NUnit.Framework.Assert.AreEqual(expectedShape, result.GetShape());
            float[] actualData = result.GetData();
            iText.Test.TestUtil.AreEqual(expectedData, actualData, 1E-6F);
        }

        private static IronSoftware.Drawing.AnyBitmap NewRgbImage(int width, int height, int[] pixels) {
            SkiaSharp.SKBitmap img = new SkiaSharp.SKBitmap(width, height,
                SkiaSharp.SKColorType.Rgb888x, SkiaSharp.SKAlphaType.Premul);

            byte[] bytePixels = new byte[width * height * 4];

            for (int i = 0; i < pixels.Length; i++) {
                int pixel = pixels[i];
        
                byte r = (byte)((pixel >> 16) & 0xFF);
                byte g = (byte)((pixel >> 8) & 0xFF);
                byte b = (byte)(pixel & 0xFF);
        
                int byteIndex = i * 4;
                bytePixels[byteIndex] = r;
                bytePixels[byteIndex + 1] = g;
                bytePixels[byteIndex + 2] = b;
                bytePixels[byteIndex + 3] = 0xFF;
            }

            using (SkiaSharp.SKPixmap pixmap = img.PeekPixels()) {
                Marshal.Copy(bytePixels, 0, pixmap.GetPixels(), bytePixels.Length);
            }

            return img;
        }
    }
}
