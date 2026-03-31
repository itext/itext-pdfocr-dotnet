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
    /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.NormalizeImage"/>
    /// POJO.
    /// </summary>
    internal sealed class NormalizeImageParser {
        private const String MEAN_KEY = "mean";

        private const String STD_KEY = "std";

        private static readonly float[] DEFAULT_MEAN = new float[] { 0.485F, 0.456F, 0.406F };

        private static readonly float[] DEFAULT_STD = new float[] { 0.229F, 0.224F, 0.225F };

        private NormalizeImageParser() {
        }

//\cond DO_NOT_DOCUMENT
        // Static class
        /// <summary>
        /// Parses a YAML mapping from a PaddleOCR config file into a
        /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.NormalizeImage"/>
        /// POJO.
        /// </summary>
        /// <param name="yamlObj">YAML mapping to parse</param>
        /// <param name="keyCtx">config key under which the object is located</param>
        /// <returns>the parsed object</returns>
        internal static NormalizeImage Parse(Object yamlObj, String keyCtx) {
            // Since all fields we need are optional, we also accept null
            if (yamlObj == null) {
                return new NormalizeImage(DEFAULT_MEAN, DEFAULT_STD);
            }
            IDictionary<Object, Object> root = YamlUtil.ObjToMapping(yamlObj);
            if (root == null) {
                throw ConfigParserException.UnexpectedValueForKey(keyCtx);
            }
            return new NormalizeImage(ParserUtil.GetOrDefault(root, keyCtx, MEAN_KEY, DEFAULT_MEAN), ParserUtil.GetOrDefault
                (root, keyCtx, STD_KEY, DEFAULT_STD));
        }
//\endcond
    }
//\endcond
}
