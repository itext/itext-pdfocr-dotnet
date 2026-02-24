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
using iText.Pdfocr.Onnxtr;
using iText.Test;

namespace iText.Pdfocr.Onnxtr.Orientation {
    [NUnit.Framework.Category("UnitTest")]
    public class OnnxOrientationPredictorPropertiesTest : ExtendedITextTest {
        private static readonly String BASE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        private static readonly String MOBILENETV3 = BASE_DIRECTORY + "models/mobilenet_v3_small_crop_orientation-5620cf7e.onnx";

        [NUnit.Framework.Test]
        public virtual void EqualsWithConstructorsTest() {
            ImageResizeOptions imageResizeOptions = new ImageResizeOptions(ImageChannelConfiguration.RGB, 10, 10);
            DefaultOrientationMapper outputMapper = new DefaultOrientationMapper();
            OnnxOrientationPredictorProperties first = new OnnxOrientationPredictorProperties(MOBILENETV3, new OnnxInputProperties
                (imageResizeOptions), outputMapper);
            OnnxOrientationPredictorProperties second = new OnnxOrientationPredictorProperties(MOBILENETV3, new OnnxInputProperties
                (imageResizeOptions), outputMapper, new DefaultOrtSessionOptionsCreator());
            NUnit.Framework.Assert.AreNotEqual(first, second);
            NUnit.Framework.Assert.AreNotEqual(first.GetHashCode(), second.GetHashCode());
            OnnxOrientationPredictorProperties third = new OnnxOrientationPredictorProperties(MOBILENETV3, new OnnxInputProperties
                (imageResizeOptions), new DefaultOrientationMapper());
            NUnit.Framework.Assert.AreNotEqual(first, third);
            NUnit.Framework.Assert.AreNotEqual(first.GetHashCode(), third.GetHashCode());
            OnnxOrientationPredictorProperties fourth = new OnnxOrientationPredictorProperties(MOBILENETV3, new OnnxInputProperties
                (imageResizeOptions), outputMapper);
            NUnit.Framework.Assert.AreEqual(first, fourth);
            NUnit.Framework.Assert.AreEqual(first.GetHashCode(), fourth.GetHashCode());
        }
    }
}
