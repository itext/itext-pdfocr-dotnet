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
﻿using OpenCvSharp;

namespace iText.Pdfocr.Onnxtr.Util
{
    public class OnnxTRTestUtil
    {
        public static void TestNormalizeRotatedRect(float originalAngle, float newWidth, float newHeight, float newAngle
        )
        {
            Point2f center = new Point2f(0, 0);
            Size2f size = new Size2f(5, 10);
            RotatedRect rect = new RotatedRect(center, size, originalAngle);
            RotatedRect newRect = OpenCvUtil.NormalizeRotatedRect(rect);
            Size2f newSize = newRect.Size;
            NUnit.Framework.Assert.AreEqual(newWidth, newSize.Width, 1e-6);
            NUnit.Framework.Assert.AreEqual(newHeight, newSize.Height, 1e-6);
            NUnit.Framework.Assert.AreEqual(newAngle, newRect.Angle, 1e-6);
        }
    }
}