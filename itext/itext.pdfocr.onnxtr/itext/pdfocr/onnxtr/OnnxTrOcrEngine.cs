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
using System.Text;
using iText.Commons.Utils;
using iText.Pdfocr;
using iText.Pdfocr.Exceptions;
using iText.Pdfocr.Onnxtr.Detection;
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Pdfocr.Onnxtr.Orientation;
using iText.Pdfocr.Onnxtr.Recognition;
using iText.Pdfocr.Util;

namespace iText.Pdfocr.Onnxtr {
    /// <summary>
    /// <see cref="iText.Pdfocr.IOcrEngine"/>
    /// implementation, based on OnnxTR/DocTR machine learning OCR projects.
    /// </summary>
    /// <remarks>
    /// <see cref="iText.Pdfocr.IOcrEngine"/>
    /// implementation, based on OnnxTR/DocTR machine learning OCR projects.
    /// <para />
    /// NOTE:
    /// <see cref="OnnxTrOcrEngine"/>
    /// instance shall be closed after all usages to avoid native allocations leak.
    /// </remarks>
    public class OnnxTrOcrEngine : IOcrEngine, IDisposable {
        /// <summary>Text detector.</summary>
        /// <remarks>Text detector. For an input image it outputs a list of text boxes.</remarks>
        private readonly IDetectionPredictor detectionPredictor;

        /// <summary>Text orientation predictor.</summary>
        /// <remarks>
        /// Text orientation predictor. For an input image, which is a tight crop of text, it outputs its orientation
        /// in 90 degrees steps. Can be null.
        /// </remarks>
        private readonly IOrientationPredictor orientationPredictor;

        /// <summary>Text recognizer.</summary>
        /// <remarks>Text recognizer. For an input image, which is a tight crop of text, it outputs the displayed string.
        ///     </remarks>
        private readonly IRecognitionPredictor recognitionPredictor;

        /// <summary>Create a new OCR engine with the provided predictors.</summary>
        /// <param name="detectionPredictor">text detector. For an input image it outputs a list of text boxes</param>
        /// <param name="orientationPredictor">
        /// text orientation predictor. For an input image, which is a tight  crop of text,
        /// it outputs its orientation in 90 degrees steps. Can be null, in that case all text
        /// is assumed to be upright
        /// </param>
        /// <param name="recognitionPredictor">
        /// text recognizer. For an input image, which is a tight crop of text, it outputs the
        /// displayed string
        /// </param>
        public OnnxTrOcrEngine(IDetectionPredictor detectionPredictor, IOrientationPredictor orientationPredictor, 
            IRecognitionPredictor recognitionPredictor) {
            this.detectionPredictor = Objects.RequireNonNull(detectionPredictor);
            this.orientationPredictor = orientationPredictor;
            this.recognitionPredictor = Objects.RequireNonNull(recognitionPredictor);
        }

        /// <summary>Create a new OCR engine with the provided predictors, without text orientation prediction.</summary>
        /// <param name="detectionPredictor">text detector. For an input image it outputs a list of text boxes</param>
        /// <param name="recognitionPredictor">
        /// text recognizer. For an input image, which is a tight crop of text,
        /// it outputs the displayed string
        /// </param>
        public OnnxTrOcrEngine(IDetectionPredictor detectionPredictor, IRecognitionPredictor recognitionPredictor)
            : this(detectionPredictor, null, recognitionPredictor) {
        }

        public virtual void Close() {
            detectionPredictor.Dispose();
            if (orientationPredictor != null) {
                orientationPredictor.Dispose();
            }
            recognitionPredictor.Dispose();
        }

        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input) {
            return DoImageOcr(input, null);
        }

        public virtual IDictionary<int, IList<TextInfo>> DoImageOcr(FileInfo input, OcrProcessContext ocrProcessContext
            ) {
            IList<System.Drawing.Bitmap> images = GetImages(input);
            return new OnnxTrProcessor(detectionPredictor, orientationPredictor, recognitionPredictor).DoOcr(images);
        }

        public virtual void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile) {
            CreateTxtFile(inputImages, txtFile, null);
        }

        public virtual void CreateTxtFile(IList<FileInfo> inputImages, FileInfo txtFile, OcrProcessContext ocrProcessContext
            ) {
            StringBuilder content = new StringBuilder();
            foreach (FileInfo inputImage in inputImages) {
                IDictionary<int, IList<TextInfo>> outputMap = DoImageOcr(inputImage, ocrProcessContext);
                content.Append(PdfOcrTextBuilder.BuildText(outputMap));
            }
            WriteToTextFile(txtFile.FullName, content.ToString());
        }

        public virtual bool IsTaggingSupported() {
            return false;
        }

        /// <summary>
        /// Writes provided
        /// <see cref="System.String"/>
        /// to text file using provided path.
        /// </summary>
        /// <param name="path">
        /// path as
        /// <see cref="System.String"/>
        /// to file to be created
        /// </param>
        /// <param name="data">
        /// text data in required format as
        /// <see cref="System.String"/>
        /// </param>
        private static void WriteToTextFile(String path, String data) {
            try {
                using (TextWriter writer = new StreamWriter(FileUtil.GetFileOutputStream(path), System.Text.Encoding.UTF8)
                    ) {
                    writer.Write(data);
                }
            }
            catch (System.IO.IOException e) {
                throw new PdfOcrException(MessageFormatUtil.Format(PdfOcrExceptionMessageConstant.CANNOT_WRITE_TO_FILE, path
                    , e.Message), e);
            }
        }

        private static IList<System.Drawing.Bitmap> GetImages(FileInfo input) {
            try {
                if (TiffImageUtil.IsTiffImage(input)) {
                    IList<System.Drawing.Bitmap> images = TiffImageUtil.GetAllImages(input);
                    if (images.Count == 0) {
                        throw new PdfOcrInputException(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_READ_IMAGE);
                    }
                    return images;
                }
                else {
                    System.Drawing.Bitmap image = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(input.FullName);
                    if (image == null) {
                        throw new PdfOcrInputException(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_READ_IMAGE);
                    }
                    return JavaCollectionsUtil.SingletonList(image);
                }
            }
            catch (Exception e) {
                throw new PdfOcrInputException(PdfOcrOnnxTrExceptionMessageConstant.FAILED_TO_READ_IMAGE, e);
            }
        }

        void System.IDisposable.Dispose() {
            Close();
        }
    }
}
