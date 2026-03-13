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
using iText.Pdfocr.Onnx;
using iText.Pdfocr.Onnx.Detection;
using iText.Pdfocr.Onnx.Orientation;
using iText.Pdfocr.Onnx.Recognition;

namespace iText.Pdfocr.Onnx.Util {
    public sealed class OcrEngineTypeWithOrientation {
        public static readonly iText.Pdfocr.Onnx.Util.OcrEngineTypeWithOrientation PADDLE = new iText.Pdfocr.Onnx.Util.OcrEngineTypeWithOrientation
            ("PaddleOCR", () => CreatePaddleOcrEngine());

        public static readonly iText.Pdfocr.Onnx.Util.OcrEngineTypeWithOrientation EASY = new iText.Pdfocr.Onnx.Util.OcrEngineTypeWithOrientation
            ("EasyOCR", () => CreateEasyOcrEngine());

        public static readonly iText.Pdfocr.Onnx.Util.OcrEngineTypeWithOrientation DOCTR = new iText.Pdfocr.Onnx.Util.OcrEngineTypeWithOrientation
            ("DocTR", () => CreateDocTrEngine());

        public volatile OnnxOcrEngine instance;

        private static IOrientationPredictor orientationPredictor;

        private readonly String displayName;

        private readonly Func<OnnxOcrEngine> supplier;

//\cond DO_NOT_DOCUMENT
        internal OcrEngineTypeWithOrientation(String displayName, Func<OnnxOcrEngine> supplier) {
            this.displayName = displayName;
            this.supplier = supplier;
        }
//\endcond

        public OnnxOcrEngine Get() {
            if (this.instance == null) {
                lock (this) {
                    if (this.instance == null) {
                        this.instance = this.supplier();
                    }
                }
            }
            return this.instance;
        }

        public String GetDisplayName() {
            return this.displayName;
        }

        public static iText.Pdfocr.Onnx.Util.OcrEngineTypeWithOrientation[] All() {
            return new iText.Pdfocr.Onnx.Util.OcrEngineTypeWithOrientation[] { iText.Pdfocr.Onnx.Util.OcrEngineTypeWithOrientation
                .PADDLE, iText.Pdfocr.Onnx.Util.OcrEngineTypeWithOrientation.EASY, iText.Pdfocr.Onnx.Util.OcrEngineTypeWithOrientation
                .DOCTR };
        }

        private static OnnxOcrEngine CreatePaddleOcrEngine() {
            try {
                IDetectionPredictor paddleDetectionPredictor = OnnxDetectionPredictor.PaddleOcr(ModelPaths.GetPaddleOcrDetectionModel
                    ());
                IRecognitionPredictor paddleRecognitionPredictor = OnnxRecognitionPredictor.PaddleOcr(ModelPaths.GetPaddleOcrRecognitionModel
                    ());
                return new OnnxOcrEngine(paddleDetectionPredictor, GetOrientationPredictor(), paddleRecognitionPredictor);
            }
            catch (System.IO.IOException e) {
                // Shouldn't reach there.
                throw new Exception(e.Message, e);
            }
        }

        private static OnnxOcrEngine CreateEasyOcrEngine() {
            IDetectionPredictor easyDetectionPredictor = OnnxDetectionPredictor.EasyOcr(ModelPaths.GetEasyOcrDetectionModel
                ());
            IRecognitionPredictor easyRecognitionPredictor = OnnxRecognitionPredictor.EasyOcr(ModelPaths.GetEasyOcrRecognitionModel
                (), EasyOcrMapper.LATIN_G2);
            return new OnnxOcrEngine(easyDetectionPredictor, GetOrientationPredictor(), easyRecognitionPredictor);
        }

        private static OnnxOcrEngine CreateDocTrEngine() {
            IDetectionPredictor docTrDetectionPredictor = OnnxDetectionPredictor.Fast(ModelPaths.GetDocTrDetectionModel
                ());
            IRecognitionPredictor docTrRecognitionPredictor = OnnxRecognitionPredictor.CrnnVgg16(ModelPaths.GetDocTrRecognitionModel
                ());
            return new OnnxOcrEngine(docTrDetectionPredictor, GetOrientationPredictor(), docTrRecognitionPredictor);
        }

        private static IOrientationPredictor GetOrientationPredictor() {
            if (iText.Pdfocr.Onnx.Util.OcrEngineTypeWithOrientation.orientationPredictor == null) {
                iText.Pdfocr.Onnx.Util.OcrEngineTypeWithOrientation.orientationPredictor = OnnxOrientationPredictor.MobileNetV3
                    (ModelPaths.GetOrientationModel());
            }
            return iText.Pdfocr.Onnx.Util.OcrEngineTypeWithOrientation.orientationPredictor;
        }
    }
}
