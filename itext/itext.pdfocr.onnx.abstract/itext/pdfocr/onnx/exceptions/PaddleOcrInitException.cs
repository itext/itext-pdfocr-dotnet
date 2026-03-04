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
using iText.Commons.Utils;
using iText.Pdfocr.Exceptions;

namespace iText.Pdfocr.Onnx.Exceptions {
    /// <summary>Exception class for exceptions during PaddleOCR initialization.</summary>
    public class PaddleOcrInitException : PdfOcrException {
        /// <summary>
        /// Creates new
        /// <see cref="PaddleOcrInitException"/>
        /// instance.
        /// </summary>
        /// <param name="message">exception message</param>
        protected internal PaddleOcrInitException(String message)
            : base(message) {
        }

        /// <summary>
        /// Creates an exception for cases, when the detection model does not
        /// return quads.
        /// </summary>
        /// <returns>the created exception</returns>
        public static iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException BoxTypeIsNotSupported() {
            return new iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException(PdfOcrOnnxExceptionMessageConstant.BOX_TYPE_IS_NOT_SUPPORTED
                );
        }

        /// <summary>
        /// Creates an exception to assert
        /// <c>channel_first</c>
        /// is set to
        /// <see langword="false"/>.
        /// </summary>
        /// <returns>the created exception</returns>
        public static iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException ChannelFirstIsNotSupported() {
            return new iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException(PdfOcrOnnxExceptionMessageConstant.CHANNEL_FIRST_IS_NOT_SUPPORTED
                );
        }

        /// <summary>
        /// Creates an exception for cases, when the detection model uses an
        /// unsupported method for resizing input images.
        /// </summary>
        /// <returns>the created exception</returns>
        public static iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException ImageShapeIsNotSupported() {
            return new iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException(PdfOcrOnnxExceptionMessageConstant.IMAGE_SHAPE_IS_NOT_SUPPORTED
                );
        }

        /// <summary>
        /// Creates an exception for cases, when an expected pre-processing
        /// operation is missing in the configuration file.
        /// </summary>
        /// <param name="name">name of the missing pre-processing operation</param>
        /// <returns>the created exception</returns>
        public static iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException PreProcessorOperationMissing(String name
            ) {
            return new iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant
                .PRE_PROCESSOR_OPERATION_MISSING, name));
        }

        /// <summary>
        /// Creates an exception for cases, when the detection model uses an
        /// unsupported score calculation method.
        /// </summary>
        /// <returns>the created exception</returns>
        public static iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException ScoreModeIsNotSupported() {
            return new iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException(PdfOcrOnnxExceptionMessageConstant.SCORE_MODE_IS_NOT_SUPPORTED
                );
        }

        /// <summary>
        /// Creates an exception for cases, when the size of the array of means for
        /// normalization has an unexpected size.
        /// </summary>
        /// <param name="expectedCount">expected size of the array</param>
        /// <returns>the created exception</returns>
        public static iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException UnexpectedMeanChannelCount(int expectedCount
            ) {
            return new iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant
                .UNEXPECTED_MEAN_CHANNEL_COUNT, expectedCount));
        }

        /// <summary>
        /// Creates an exception for cases, when an unexpected post-processor is
        /// specified in the configuration file.
        /// </summary>
        /// <param name="name">name of the post-processor that was found</param>
        /// <returns>the created exception</returns>
        public static iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException UnexpectedPostProcessorType(String name) {
            return new iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant
                .UNEXPECTED_POST_PROCESSOR_TYPE, name));
        }

        /// <summary>
        /// Creates an exception for cases, when the size of the array of standard
        /// deviations for normalization has an unexpected size.
        /// </summary>
        /// <param name="expectedCount">expected size of the array</param>
        /// <returns>the created exception</returns>
        public static iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException UnexpectedStdChannelCount(int expectedCount
            ) {
            return new iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException(MessageFormatUtil.Format(PdfOcrOnnxExceptionMessageConstant
                .UNEXPECTED_STD_CHANNEL_COUNT, expectedCount));
        }

        /// <summary>
        /// Creates an exception for cases, when the detection model uses an
        /// unsupported pre-processing step for input images.
        /// </summary>
        /// <returns>the created exception</returns>
        public static iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException UseDilationIsNotSupported() {
            return new iText.Pdfocr.Onnx.Exceptions.PaddleOcrInitException(PdfOcrOnnxExceptionMessageConstant.USE_DILATION_IS_NOT_SUPPORTED
                );
        }
    }
}
