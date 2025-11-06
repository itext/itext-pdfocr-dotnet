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
using System;
using System.Collections.Generic;
using System.IO;
using iText.Commons.Utils;
using iText.Pdfocr;
using iText.Test;

namespace iText.Pdfocr.Onnxtr.Orientation {
    [NUnit.Framework.Category("IntegrationTest")]
    public class OnnxOrientationPredictorTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/onnxtr/orientation/OnnxOrientationPredictorTest/";

        private static readonly String TARGET_DIRECTORY = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/resources/itext/pdfocr/onnxtr/orientation/OnnxOrientationPredictorTest/";

        private static readonly String MOBILENETV3 = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/models/mobilenet_v3_small_crop_orientation-5620cf7e.onnx";

        private static IOrientationPredictor PREDICTOR;

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(TARGET_DIRECTORY);
            PREDICTOR = OnnxOrientationPredictor.MobileNetV3(MOBILENETV3);
        }

        [NUnit.Framework.OneTimeTearDown]
        public static void AfterClass() {
            PREDICTOR.Close();
        }

        public static IEnumerable<Object[]> PredictWithLongLinesParams() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { TextOrientation.HORIZONTAL, "line_0.png" }, new 
                Object[] { TextOrientation.HORIZONTAL_ROTATED_90, "line_90.png" }, new Object[] { TextOrientation.HORIZONTAL_ROTATED_180
                , "line_180.png" }, new Object[] { TextOrientation.HORIZONTAL_ROTATED_270, "line_270.png" } });
        }

        [NUnit.Framework.TestCaseSource("PredictWithLongLinesParams")]
        public virtual void PredictWithLongLines(TextOrientation expectedResult, String inputFileName) {
            IronSoftware.Drawing.AnyBitmap inputImage = IronSoftware.Drawing.AnyBitmap.FromFile(new FileInfo(TEST_DIRECTORY
                 + inputFileName).FullName);
            IEnumerator<TextOrientation> result = PREDICTOR.Predict(JavaCollectionsUtil.Singleton(inputImage));
            result.MoveNext();
            TextOrientation actualResult = result.Current;
            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
