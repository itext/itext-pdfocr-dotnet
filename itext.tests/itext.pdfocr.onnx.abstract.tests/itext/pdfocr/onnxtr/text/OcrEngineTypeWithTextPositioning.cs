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
using iText.Pdfocr.Onnxtr.Util;

namespace iText.Pdfocr.Onnxtr.Text {
    /// <summary>
    /// This enum is created for
    /// <see cref="TextPositioningModeTest"/>
    /// and should be used in it only
    /// since all engines (and so predictors) will be closed after these tests, and it won't be possible to reuse them.
    /// </summary>
    public sealed class OcrEngineTypeWithTextPositioning {
        public static readonly iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning PADDLE_LINES = new iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning
            ("PaddleOCR_BY_LINES", () => CreatePaddleOcrEngine(iText.Pdfocr.Onnxtr.Text.TextPositioning.BY_LINES));

        public static readonly iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning EASY_LINES = new iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning
            ("EasyOCR_BY_LINES", () => CreateEasyOcrEngine(iText.Pdfocr.Onnxtr.Text.TextPositioning.BY_LINES));

        public static readonly iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning DOCTR_LINES = new iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning
            ("DocTR_BY_LINES", () => CreateDocTrEngine(iText.Pdfocr.Onnxtr.Text.TextPositioning.BY_LINES));

        public static readonly iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning PADDLE_WORDS = new iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning
            ("PaddleOCR_BY_WORDS", () => CreatePaddleOcrEngine(iText.Pdfocr.Onnxtr.Text.TextPositioning.BY_WORDS));

        public static readonly iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning EASY_WORDS = new iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning
            ("EasyOCR_BY_WORDS", () => CreateEasyOcrEngine(iText.Pdfocr.Onnxtr.Text.TextPositioning.BY_WORDS));

        public static readonly iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning DOCTR_WORDS = new iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning
            ("DocTR_BY_WORDS", () => CreateDocTrEngine(iText.Pdfocr.Onnxtr.Text.TextPositioning.BY_WORDS));

        public static readonly iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning PADDLE_WORDS_AND_LINES = 
            new iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning("PaddleOCR_BY_WORDS_AND_LINES", () => CreatePaddleOcrEngine
            (iText.Pdfocr.Onnxtr.Text.TextPositioning.BY_WORDS_AND_LINES));

        public static readonly iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning EASY_WORDS_AND_LINES = new 
            iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning("EasyOCR_BY_WORDS_AND_LINES", () => CreateEasyOcrEngine
            (iText.Pdfocr.Onnxtr.Text.TextPositioning.BY_WORDS_AND_LINES));

        public static readonly iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning DOCTR_WORDS_AND_LINES = new 
            iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning("DocTR_BY_WORDS_AND_LINES", () => CreateDocTrEngine
            (iText.Pdfocr.Onnxtr.Text.TextPositioning.BY_WORDS_AND_LINES));

        public volatile OnnxTrOcrEngine instance;

        private readonly String displayName;

        private readonly Func<OnnxTrOcrEngine> supplier;

//\cond DO_NOT_DOCUMENT
        internal OcrEngineTypeWithTextPositioning(String displayName, Func<OnnxTrOcrEngine> supplier) {
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

        public static iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning[] All() {
            return new iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning[] { iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning
                .PADDLE_LINES, iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.EASY_LINES, iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning
                .DOCTR_LINES, iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.PADDLE_WORDS, iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning
                .EASY_WORDS, iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.DOCTR_WORDS, iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning
                .PADDLE_WORDS_AND_LINES, iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.EASY_WORDS_AND_LINES
                , iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.DOCTR_WORDS_AND_LINES };
        }

        private static IDetectionPredictor paddleDetectionPredictor;

        private static IRecognitionPredictor paddleRecognitionPredictor;

        private static IDetectionPredictor easyDetectionPredictor;

        private static IRecognitionPredictor easyRecognitionPredictor;

        private static IDetectionPredictor docTrDetectionPredictor;

        private static IRecognitionPredictor docTrRecognitionPredictor;

        private static OnnxTrOcrEngine CreatePaddleOcrEngine(iText.Pdfocr.Onnxtr.Text.TextPositioning textPositioning
            ) {
            try {
                if (iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.paddleDetectionPredictor == null) {
                    iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.paddleDetectionPredictor = OnnxDetectionPredictor
                        .PaddleOcr(ModelPaths.GetPaddleOcrDetectionModel());
                }
                if (iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.paddleRecognitionPredictor == null) {
                    iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.paddleRecognitionPredictor = OnnxRecognitionPredictor
                        .PaddleOcr(ModelPaths.GetPaddleOcrRecognitionModel());
                }
            }
            catch (System.IO.IOException e) {
                throw new Exception(e.Message, e);
            }
            return new OnnxTrOcrEngine(iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.paddleDetectionPredictor
                , null, iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.paddleRecognitionPredictor, new OnnxTrEngineProperties
                ().SetTextPositioning(textPositioning));
        }

        private static OnnxTrOcrEngine CreateEasyOcrEngine(iText.Pdfocr.Onnxtr.Text.TextPositioning textPositioning
            ) {
            if (iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.easyDetectionPredictor == null) {
                iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.easyDetectionPredictor = OnnxDetectionPredictor.
                    EasyOcr(ModelPaths.GetEasyOcrDetectionModel());
            }
            if (iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.easyRecognitionPredictor == null) {
                iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.easyRecognitionPredictor = OnnxRecognitionPredictor
                    .EasyOcr(ModelPaths.GetEasyOcrRecognitionModel(), EasyOcrMapper.LATIN_G2);
            }
            return new OnnxTrOcrEngine(iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.easyDetectionPredictor
                , null, iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.easyRecognitionPredictor, new OnnxTrEngineProperties
                ().SetTextPositioning(textPositioning));
        }

        private static OnnxTrOcrEngine CreateDocTrEngine(iText.Pdfocr.Onnxtr.Text.TextPositioning textPositioning) {
            if (iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.docTrDetectionPredictor == null) {
                iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.docTrDetectionPredictor = OnnxDetectionPredictor
                    .Fast(ModelPaths.GetDocTrDetectionModel());
            }
            if (iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.docTrRecognitionPredictor == null) {
                iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.docTrRecognitionPredictor = OnnxRecognitionPredictor
                    .CrnnVgg16(ModelPaths.GetDocTrRecognitionModel());
            }
            return new OnnxTrOcrEngine(iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.docTrDetectionPredictor
                , null, iText.Pdfocr.Onnxtr.Text.OcrEngineTypeWithTextPositioning.docTrRecognitionPredictor, new OnnxTrEngineProperties
                ().SetTextPositioning(textPositioning));
        }
    }
}
