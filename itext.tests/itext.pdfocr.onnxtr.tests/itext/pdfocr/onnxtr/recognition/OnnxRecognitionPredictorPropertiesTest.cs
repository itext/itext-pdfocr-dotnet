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
using iText.Test;

namespace iText.Pdfocr.Onnxtr.Recognition {
    [NUnit.Framework.Category("UnitTest")]
    public class OnnxRecognitionPredictorPropertiesTest : ExtendedITextTest {
        private static readonly String BASE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        private static readonly String CRNNVGG16 = BASE_DIRECTORY + "models/crnn_vgg16_bn-662979cc.onnx";

        private static readonly String MOBILENETV3 = BASE_DIRECTORY + "models/mobilenet_v3_small_crop_orientation-5620cf7e.onnx";

        [NUnit.Framework.Test]
        public virtual void TestSameModels() {
            OnnxRecognitionPredictorProperties first = OnnxRecognitionPredictorProperties.CrnnVgg16(CRNNVGG16);
            NUnit.Framework.Assert.AreEqual(first, first);
        }

        [NUnit.Framework.Test]
        public virtual void TestEqualModels() {
            OnnxRecognitionPredictorProperties first = OnnxRecognitionPredictorProperties.CrnnVgg16(CRNNVGG16);
            OnnxRecognitionPredictorProperties second = OnnxRecognitionPredictorProperties.CrnnVgg16(CRNNVGG16);
            //They will still have different IRecognitionPostProcessor
            NUnit.Framework.Assert.AreNotEqual(first, second);
        }

        [NUnit.Framework.Test]
        public virtual void TestEqualWithDifferentObject() {
            OnnxRecognitionPredictorProperties first = OnnxRecognitionPredictorProperties.CrnnVgg16(CRNNVGG16);
            String second = "test";
            NUnit.Framework.Assert.AreNotEqual(first, second);
        }

        [NUnit.Framework.Test]
        public virtual void TestNotEqualModels() {
            OnnxRecognitionPredictorProperties first = OnnxRecognitionPredictorProperties.CrnnVgg16(CRNNVGG16);
            OnnxRecognitionPredictorProperties second = OnnxRecognitionPredictorProperties.CrnnMobileNetV3(MOBILENETV3
                );
            NUnit.Framework.Assert.AreNotEqual(first, second);
        }
    }
}
