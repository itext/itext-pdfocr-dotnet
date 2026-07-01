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
using iText.Commons.Utils;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Onnx.Detection;
using iText.Pdfocr.Onnx.Exceptions;
using iText.Pdfocr.Onnx.Orientation;
using iText.Pdfocr.Onnx.Recognition;
using iText.Pdfocr.Onnx.Util;
using iText.Pdfocr.Util;
using iText.Test;

namespace iText.Pdfocr.Onnx {
    [NUnit.Framework.Category("UnitTest")]
    public class OnnxUnitTest : ExtendedITextTest {
        private static readonly String BASE_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/";

        private static readonly String FAST = BASE_DIRECTORY + "models/rep_fast_tiny-28867779.onnx";

        private static readonly String TIFF = BASE_DIRECTORY + "images/two_pages.tiff";

        [NUnit.Framework.Test]
        public virtual void EmptyImageInputTest() {
            ImageResizeOptions imageResizeOptions = new ImageResizeOptions(ImageChannelConfiguration.RGB, 1024, 1024, 
                PaddingStrategy.SYMMETRIC_BLACK);
            float[] mean = new float[] { 0.798F, 0.785F, 0.772F };
            float[] std = new float[] { 0.264F, 0.2749F, 0.287F };
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => BufferedImageUtil.ToBchwInput(
                new List<IronSoftware.Drawing.AnyBitmap>(), new OnnxInputProperties(imageResizeOptions, mean, std)));
            NUnit.Framework.Assert.AreEqual(PdfOcrOnnxExceptionMessageConstant.SHOULD_BE_AT_LEAST_ONE_IMAGE, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void TooManyImagesTest() {
            ImageResizeOptions imageResizeOptions = new ImageResizeOptions(ImageChannelConfiguration.RGB, 1024, 1024, 
                PaddingStrategy.SYMMETRIC_BLACK);
            float[] mean = new float[] { 0.798F, 0.785F, 0.772F };
            float[] std = new float[] { 0.264F, 0.2749F, 0.287F };
            MemoryStream stream = ByteArrayStreamUtil.CreateByteArrayInputStream(FileUtil.GetInputStreamForFile(TIFF));
            Exception e = NUnit.Framework.Assert.Catch(typeof(ArgumentException), () => BufferedImageUtil.ToBchwInput(
                OnnxOcrEngine.GetImages(stream), new OnnxInputProperties(imageResizeOptions, mean, std)));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.TOO_MANY_IMAGES
                , 2, 1), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidOrientationTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(IndexOutOfRangeException), () => new DefaultOrientationMapper
                ().Map(4));
            NUnit.Framework.Assert.AreEqual(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant.INDEX_OUT_OF_BOUNDS
                , 4), e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidModelPathTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => OnnxDetectionPredictor.Fast("invalid"
                ));
            NUnit.Framework.Assert.AreEqual(PdfOcrOnnxExceptionMessageConstant.FAILED_TO_INIT_ONNX_RUNTIME_SESSION, e.
                Message);
        }

        [NUnit.Framework.Test]
        public virtual void InvalidModelTest() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfOcrException), () => OnnxRecognitionPredictor.CrnnVgg16
                (FAST));
            NUnit.Framework.Assert.AreEqual(PdfOcrOnnxExceptionMessageConstant.MODEL_DID_NOT_PASS_VALIDATION, e.Message
                );
        }

        [NUnit.Framework.Test]
        public virtual void CompareDetectionPredictorPropertiesTest() {
            String model = "model";
            OnnxDetectionPredictorProperties properties = OnnxDetectionPredictorProperties.DbNet(model);
            NUnit.Framework.Assert.AreEqual(properties.GetHashCode(), OnnxDetectionPredictorProperties.DbNet(model).GetHashCode
                ());
            NUnit.Framework.Assert.AreEqual(properties, OnnxDetectionPredictorProperties.DbNet(model));
            NUnit.Framework.Assert.AreNotEqual(properties, OnnxRecognitionPredictorProperties.CrnnVgg16(model));
            NUnit.Framework.Assert.AreEqual(properties, properties);
            NUnit.Framework.Assert.AreNotEqual(properties, null);
            NUnit.Framework.Assert.AreNotEqual(properties, OnnxDetectionPredictorProperties.DbNet("model2"));
            ImageResizeOptions imageResizeOptions = new ImageResizeOptions(ImageChannelConfiguration.RGB, 32, 128);
            OnnxInputProperties inputProperties = new OnnxInputProperties(imageResizeOptions, new float[] { 1F, 1F, 1F
                 }, new float[] { 1F, 1F, 1F });
            NUnit.Framework.Assert.AreNotEqual(properties, new OnnxDetectionPredictorProperties(model, inputProperties
                , new OnnxDetectionPostProcessor()));
            NUnit.Framework.Assert.AreNotEqual(new OnnxDetectionPredictorProperties(model, inputProperties, new OnnxDetectionPostProcessor
                (1F, 1F)), new OnnxDetectionPredictorProperties(model, inputProperties, new OnnxDetectionPostProcessor
                (2F, 2F)));
        }

        [NUnit.Framework.Test]
        public virtual void CompareRecognitionPredictorPropertiesTest() {
            OnnxRecognitionPredictorProperties properties = OnnxRecognitionPredictorProperties.CrnnMobileNetV3("model"
                );
            NUnit.Framework.Assert.AreNotEqual(properties.GetHashCode(), OnnxRecognitionPredictorProperties.CrnnMobileNetV3
                ("model").GetHashCode());
            NUnit.Framework.Assert.AreNotEqual(properties, OnnxRecognitionPredictorProperties.CrnnMobileNetV3("model")
                );
            NUnit.Framework.Assert.AreEqual(properties, properties);
            NUnit.Framework.Assert.AreNotEqual(properties, null);
            ImageResizeOptions imageResizeOptions = new ImageResizeOptions(ImageChannelConfiguration.RGB, 32, 128);
            OnnxInputProperties inputProperties = new OnnxInputProperties(imageResizeOptions, new float[] { 1F, 1F, 1F
                 }, new float[] { 1F, 1F, 1F });
            NUnit.Framework.Assert.AreNotEqual(properties, new OnnxRecognitionPredictorProperties("model", inputProperties
                , new CrnnPostProcessor(Vocabulary.LEGACY_FRENCH)));
            NUnit.Framework.Assert.AreNotEqual(new OnnxRecognitionPredictorProperties("model", inputProperties, new CrnnPostProcessor
                (Vocabulary.FRENCH)), new OnnxRecognitionPredictorProperties("model", inputProperties, new CrnnPostProcessor
                (Vocabulary.ENGLISH)));
        }
    }
}
