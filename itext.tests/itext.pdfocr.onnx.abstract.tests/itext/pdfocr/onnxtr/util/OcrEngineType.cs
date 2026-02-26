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
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Recognition;

namespace iText.Pdfocr.Onnxtr.Util {
    public sealed class OcrEngineType {
        public static readonly iText.Pdfocr.Onnxtr.Util.OcrEngineType PADDLE = new iText.Pdfocr.Onnxtr.Util.OcrEngineType
            ("PaddleOCR", () => CreatePaddleOcrEngine());

        public static readonly iText.Pdfocr.Onnxtr.Util.OcrEngineType EASY = new iText.Pdfocr.Onnxtr.Util.OcrEngineType
            ("EasyOCR", () => CreateEasyOcrEngine());

        public static readonly iText.Pdfocr.Onnxtr.Util.OcrEngineType DOCTR = new iText.Pdfocr.Onnxtr.Util.OcrEngineType
            ("DocTR", () => CreateDocTrEngine());

        public volatile OnnxTrOcrEngine instance;

        private readonly String displayName;

        private readonly Func<OnnxTrOcrEngine> supplier;

//\cond DO_NOT_DOCUMENT
        internal OcrEngineType(String displayName, Func<OnnxTrOcrEngine> supplier) {
            this.displayName = displayName;
            this.supplier = supplier;
        }
//\endcond

        public OnnxTrOcrEngine Get() {
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

        public static iText.Pdfocr.Onnxtr.Util.OcrEngineType[] All() {
            return new iText.Pdfocr.Onnxtr.Util.OcrEngineType[] { iText.Pdfocr.Onnxtr.Util.OcrEngineType.PADDLE, iText.Pdfocr.Onnxtr.Util.OcrEngineType
                .EASY, iText.Pdfocr.Onnxtr.Util.OcrEngineType.DOCTR };
        }

        private static OnnxTrOcrEngine CreatePaddleOcrEngine() {
            try {
                IDetectionPredictor paddleDetectionPredictor = OnnxDetectionPredictor.PaddleOcr(ModelPaths.GetPaddleOcrDetectionModel
                    ());
                IRecognitionPredictor paddleRecognitionPredictor = OnnxRecognitionPredictor.PaddleOcr(ModelPaths.GetPaddleOcrRecognitionModel
                    ());
                return new OnnxTrOcrEngine(paddleDetectionPredictor, paddleRecognitionPredictor);
            }
            catch (System.IO.IOException e) {
                // Shouldn't reach there.
                throw new Exception(e.Message, e);
            }
        }

        private static OnnxTrOcrEngine CreateEasyOcrEngine() {
            IDetectionPredictor easyDetectionPredictor = OnnxDetectionPredictor.EasyOcr(ModelPaths.GetEasyOcrDetectionModel
                ());
            IRecognitionPredictor easyRecognitionPredictor = OnnxRecognitionPredictor.EasyOcr(ModelPaths.GetEasyOcrRecognitionModel
                (), EasyOcrMapper.LATIN_G2);
            return new OnnxTrOcrEngine(easyDetectionPredictor, easyRecognitionPredictor);
        }

        private static OnnxTrOcrEngine CreateDocTrEngine() {
            IDetectionPredictor docTrDetectionPredictor = OnnxDetectionPredictor.Fast(ModelPaths.GetDocTrDetectionModel
                ());
            IRecognitionPredictor docTrRecognitionPredictor = OnnxRecognitionPredictor.CrnnVgg16(ModelPaths.GetDocTrRecognitionModel
                ());
            return new OnnxTrOcrEngine(docTrDetectionPredictor, docTrRecognitionPredictor);
        }
    }
}
