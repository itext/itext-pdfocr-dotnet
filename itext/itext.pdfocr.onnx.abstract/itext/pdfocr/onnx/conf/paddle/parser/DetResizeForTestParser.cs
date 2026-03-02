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
using iText.Pdfocr.Onnx.Conf.Paddle.Model;
using iText.Pdfocr.Onnx.Exceptions;
using iText.Pdfocr.Onnx.Util;

namespace iText.Pdfocr.Onnx.Conf.Paddle.Parser {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Static class with functions for parsing PaddleOCR YAML config objects into
    /// a
    /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.DetResizeForTest"/>
    /// POJO.
    /// </summary>
    internal sealed class DetResizeForTestParser {
        private const String IMAGE_SHAPE_KEY = "image_shape";

        private const String KEEP_RATIO_KEY = "keep_ratio";

        private static readonly int[] DEFAULT_IMAGE_SHAPE = null;

        private const bool DEFAULT_KEEP_RATIO = false;

        private DetResizeForTestParser() {
        }

//\cond DO_NOT_DOCUMENT
        // Static class
        /// <summary>
        /// Parses a YAML mapping from a PaddleOCR config file into a
        /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.DetResizeForTest"/>
        /// POJO.
        /// </summary>
        /// <param name="yamlObj">YAML mapping to parse</param>
        /// <param name="keyCtx">config key under which the object is located</param>
        /// <returns>the parsed object</returns>
        internal static DetResizeForTest Parse(Object yamlObj, String keyCtx) {
            // Since all fields we need are optional, we also accept null
            if (yamlObj == null) {
                return new DetResizeForTest(DEFAULT_IMAGE_SHAPE, DEFAULT_KEEP_RATIO);
            }
            IDictionary<Object, Object> root = YamlUtil.ObjToMapping(yamlObj);
            if (root == null) {
                throw ConfigParserException.UnexpectedValueForKey(keyCtx);
            }
            /*
            * There can be more keys in the config object, but with how the code
            * works for OCR, we only care about these two. If image_shape is
            * present it will override the configuration, that comes from the
            * global OCR config. And we don't support that at the moment...
            */
            return new DetResizeForTest(ParserUtil.GetOrDefault(root, keyCtx, IMAGE_SHAPE_KEY, DEFAULT_IMAGE_SHAPE), ParserUtil
                .GetOrDefault(root, keyCtx, KEEP_RATIO_KEY, DEFAULT_KEEP_RATIO));
        }
//\endcond
    }
//\endcond
}
