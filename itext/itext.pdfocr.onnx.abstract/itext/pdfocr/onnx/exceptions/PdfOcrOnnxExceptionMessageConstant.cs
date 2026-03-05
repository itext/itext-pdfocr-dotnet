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

namespace iText.Pdfocr.Onnx.Exceptions {
    /// <summary>Class that bundles all the error message templates as constants.</summary>
    public sealed class PdfOcrOnnxExceptionMessageConstant {
        public const String BATCH_SIZE_SHOULD_BE_POSITIVE = "Target batch size should be positive.";

        public const String BOX_TYPE_IS_NOT_SUPPORTED = "Only `box_type: quad` is supported for DBPostProcess.";

        public const String CANNOT_START_ANOTHER_MAPPING_OPERATION = "Cannot start another mapping operation till the previous one is finished.";

        public const String CHANNEL_FIRST_IS_NOT_SUPPORTED = "Only `channel_first: false` is supported for DecodeImage.";

        public const String ELEM_COUNT_DOES_NOT_MATCH_SHAPE = "Data buffer element count does not match the shape.";

        public const String FAILED_TO_CLOSE_ONNX_RUNTIME_SESSION = "Failed to close an ONNX Runtime session.";

        public const String FAILED_TO_INIT_ONNX_RUNTIME_SESSION = "Failed to init ONNX Runtime session.";

        public const String FAILED_TO_INIT_SESSION_OPTIONS = "Failed to init ONNX Runtime session options.";

        public const String FAILED_TO_LOAD_ONNXRUNTIME = "Failed to load ONNX Runtime native library.";

        public const String FAILED_TO_READ_IMAGE = "Failed to read image.";

        public const String HEIGHT_MULTIPLE_SHOULD_BE_POSITIVE = "HeightMultiple should be a positive value, found {0}.";

        public const String HW_ARRAY_SHOULD_BE_TWO_DIMENSIONAL = "HwMdArray should be two-dimensional";

        public const String IMAGE_SHAPE_IS_NOT_SUPPORTED = "`image_shape` is not supported for DetResizeForTest";

        public const String INDEX_OUT_OF_BOUNDS = "Index out of bounds: {0}.";

        public const String INVALID_NUMBER_OF_OUTPUTS = "Batch processing failed: invalid number of outputs.";

        public const String LOOK_UP_STRING_CONTAINS_2_CODE_UNITS_POINTS = "Look-up string contains code points, " 
            + "which are encoded with 2 code units.";

        public const String MAX_HEIGHT_SHOULD_BE_A_MULTIPLE = "MaxHeight should be a multiple of {0}, found {1}.";

        public const String MAX_HEIGHT_SHOULD_NOT_BE_LESS_THAN_MIN = "MaxHeight should not be less than minHeight, found {0}.";

        public const String MAX_SHOULD_NOT_BE_LESS_THAN_MIN = "Max should not be less than min.";

        public const String MAX_WIDTH_SHOULD_BE_A_MULTIPLE = "MaxWidth should be a multiple of {0}, found {1}.";

        public const String MAX_WIDTH_SHOULD_NOT_BE_LESS_THAN_MIN = "MaxWidth should not be less than minWidth, found {0}.";

        public const String MIN_HEIGHT_SHOULD_BE_POSITIVE = "MaxHeight should be a positive value, found {0}.";

        public const String MIN_WIDTH_SHOULD_BE_POSITIVE = "MinWidth should be a positive value, found {0}.";

        public const String MODEL_DID_NOT_PASS_VALIDATION = "ONNX Runtime model did not pass validation.";

        public const String MODEL_ONLY_SUPPORTS_RGB = "Model only supports RGB images with a BCHW input format.";

        public const String NEGATIVE_VALUE_IN_SHAPE = "Received negative value in shape {0}.";

        public const String ONNX_RUNTIME_OPERATION_FAILED = "ONNX Runtime operation failed.";

        public const String PRE_PROCESSOR_OPERATION_MISSING = "Expected pre-processor operation is missing: {0}.";

        public const String SCORE_MODE_IS_NOT_SUPPORTED = "Only `score_mode: fast` is supported for DBPostProcess.";

        public const String SHAPE_IS_NOT_VALID = "The shape of the data buffer is not valid.";

        public const String SHOULD_BE_AT_LEAST_ONE_IMAGE = "There should be at least one image.";

        public const String TOO_MANY_IMAGES = "Too many images ({0}) for the provided batch size ({1}).";

        public const String UNEXPECTED_CHANNEL_CONFIGURATION = "Unexpected channel configuration.";

        public const String UNEXPECTED_CONFIG_KEY = "Unexpected config key: {0}.";

        public const String UNEXPECTED_DIMENSION_VALUE = "Unexpected dimension value: {0}.";

        public const String UNEXPECTED_INPUT_SHAPE = "Expected {0} input shape, but got {1} instead.";

        public const String UNEXPECTED_INPUT_SIZE = "Expected 1 input, but got {0} instead.";

        public const String UNEXPECTED_INPUT_TYPE = "Unexpected input type. Expected float32 tensor.";

        public const String UNEXPECTED_MAT_TYPE = "Unexpected Mat type {0}.";

        public const String UNEXPECTED_MEAN_CHANNEL_COUNT = "Mean should be a {0}-element array.";

        public const String UNEXPECTED_OUTPUT_SHAPE = "Expected {0} output shape, but got {1} instead.";

        public const String UNEXPECTED_OUTPUT_SIZE = "Expected 1 output, but got  {0} instead.";

        public const String UNEXPECTED_OUTPUT_TYPE = "Unexpected output type. Expected float32 tensor.";

        public const String UNEXPECTED_PADDING_STRATEGY = "Unexpected padding strategy: {0}.";

        public const String UNEXPECTED_POST_PROCESSOR_TYPE = "Unexpected post-processor type: {0}.";

        public const String UNEXPECTED_SHAPE_SIZE = "Shape should be a {0}-element array (BCHW).";

        public const String UNEXPECTED_STD_CHANNEL_COUNT = "Std should be a {0}-element array.";

        public const String UNEXPECTED_VALUE_FOR_CONFIG_KEY = "Unexpected value for config key `{0}`.";

        public const String USE_DILATION_IS_NOT_SUPPORTED = "Only `use_dilation: false` is supported for DBPostProcess.";

        public const String VALUES_SHOULD_BE_A_NON_EMPTY_ARRAY = "Values should be a non-empty array.";

        public const String WIDTH_MULTIPLE_SHOULD_BE_POSITIVE = "WidthMultiple should be a positive value, found {0}.";

        public const String X_SHOULD_BE_IN_0_1_RANGE = "X should be in [0; 1] range.";

        private PdfOcrOnnxExceptionMessageConstant() {
        }
        // Private constructor will prevent the instantiation of this class directly.
    }
}
