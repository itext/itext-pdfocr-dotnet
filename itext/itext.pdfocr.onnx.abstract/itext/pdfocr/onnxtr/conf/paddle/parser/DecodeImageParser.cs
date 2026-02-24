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
    /// <see cref="iText.Pdfocr.Onnxtr.Conf.Paddle.Model.DecodeImage"/>
    /// POJO.
    /// </summary>
    internal sealed class DecodeImageParser {
        private const String CHANNEL_FIRST_KEY = "channel_first";

        private const String IMG_MODE_KEY = "img_mode";

        private const bool DEFAULT_CHANNEL_FIRST = false;

        private static readonly ImgMode DEFAULT_IMG_MODE = ImgMode.BGR;

        private DecodeImageParser() {
        }

//\cond DO_NOT_DOCUMENT
        // Static class
        /// <summary>
        /// Parses a YAML mapping from a PaddleOCR config file into a
        /// <see cref="iText.Pdfocr.Onnxtr.Conf.Paddle.Model.DecodeImage"/>
        /// POJO.
        /// </summary>
        /// <param name="yamlObj">YAML mapping to parse</param>
        /// <param name="keyCtx">config key under which the object is located</param>
        /// <returns>the parsed object</returns>
        internal static DecodeImage Parse(Object yamlObj, String keyCtx) {
            // Since all fields we need are optional, we also accept null
            if (yamlObj == null) {
                return new DecodeImage(DEFAULT_CHANNEL_FIRST, DEFAULT_IMG_MODE);
            }
            IDictionary<Object, Object> root = YamlUtil.ObjToMapping(yamlObj);
            if (root == null) {
                throw ConfigParserException.UnexpectedValueForKey(keyCtx);
            }
            return new DecodeImage(ParserUtil.GetOrDefault(root, keyCtx, CHANNEL_FIRST_KEY, DEFAULT_CHANNEL_FIRST), ParserUtil
                .GetOrDefault(root, keyCtx, IMG_MODE_KEY, DEFAULT_IMG_MODE));
        }
//\endcond
    }
//\endcond
}
