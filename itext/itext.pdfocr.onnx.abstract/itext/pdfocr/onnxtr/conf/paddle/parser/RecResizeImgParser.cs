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
using iText.Pdfocr.Onnxtr.Conf.Paddle.Model;
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Pdfocr.Onnxtr.Util;

namespace iText.Pdfocr.Onnxtr.Conf.Paddle.Parser {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Static class with functions for parsing PaddleOCR YAML config objects into
    /// a
    /// <see cref="iText.Pdfocr.Onnxtr.Conf.Paddle.Model.RecResizeImg"/>
    /// POJO.
    /// </summary>
    internal sealed class RecResizeImgParser {
        private const String IMAGE_SHAPE_KEY = "image_shape";

        private static readonly int[] DEFAULT_IMAGE_SHAPE = new int[] { 3, 48, 320 };

        private const int EXPECTED_IMAGE_SHAPE_LENGTH = 3;

        private RecResizeImgParser() {
        }

//\cond DO_NOT_DOCUMENT
        // Static class
        /// <summary>
        /// Parses a YAML mapping from a PaddleOCR config file into a
        /// <see cref="iText.Pdfocr.Onnxtr.Conf.Paddle.Model.RecResizeImg"/>
        /// POJO.
        /// </summary>
        /// <param name="yamlObj">YAML mapping to parse</param>
        /// <param name="keyCtx">config key under which the object is located</param>
        /// <returns>the parsed object</returns>
        internal static RecResizeImg Parse(Object yamlObj, String keyCtx) {
            // Since all fields we need are optional, we also accept null
            if (yamlObj == null) {
                return new RecResizeImg(DEFAULT_IMAGE_SHAPE);
            }
            IDictionary<Object, Object> root = YamlUtil.ObjToMapping(yamlObj);
            if (root == null) {
                throw ConfigParserException.UnexpectedValueForKey(keyCtx);
            }
            int[] imageShape = ParserUtil.GetOrDefault(root, keyCtx, IMAGE_SHAPE_KEY, DEFAULT_IMAGE_SHAPE);
            if (imageShape.Length != EXPECTED_IMAGE_SHAPE_LENGTH) {
                throw ConfigParserException.UnexpectedValueForKey(keyCtx + "." + IMAGE_SHAPE_KEY);
            }
            return new RecResizeImg(imageShape);
        }
//\endcond
    }
//\endcond
}
