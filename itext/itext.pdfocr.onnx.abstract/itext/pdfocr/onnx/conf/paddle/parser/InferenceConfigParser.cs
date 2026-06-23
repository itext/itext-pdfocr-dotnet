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
using System.IO;
using iText.Commons.Internal.Runtime;
using iText.Pdfocr.Onnx.Conf.Paddle.Model;
using iText.Pdfocr.Onnx.Exceptions;
using iText.Pdfocr.Onnx.Util;

namespace iText.Pdfocr.Onnx.Conf.Paddle.Parser {
    /// <summary>
    /// Static class with functions for parsing PaddleOCR YAML config files into
    /// a
    /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.InferenceConfig"/>
    /// POJO.
    /// </summary>
    public sealed class InferenceConfigParser {
        private const String PRE_PROCESS_KEY = "PreProcess";

        private const String POST_PROCESS_KEY = "PostProcess";

        private InferenceConfigParser() {
        }

        // Static class
        /// <summary>
        /// Parses a PaddleOCR config file into a
        /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.InferenceConfig"/>
        /// POJO.
        /// </summary>
        /// <param name="content">stream with the config file contents</param>
        /// <returns>the parsed object</returns>
        public static InferenceConfig Parse(Stream content) {
            return Parse(YamlUtil.DeserializeFromStream(content));
        }

        /// <summary>
        /// Parses a YAML object from a PaddleOCR config file into a
        /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.InferenceConfig"/>
        /// POJO.
        /// </summary>
        /// <param name="yamlObj">YAML object to parse</param>
        /// <returns>the parsed object</returns>
        private static InferenceConfig Parse(Object yamlObj) {
            IDictionary<Object, Object> root = YamlUtil.ObjToMapping(yamlObj);
            if (root == null) {
                throw ConfigParserException.UnexpectedValueForKey("<root>");
            }
            return new InferenceConfig(PreProcessParser.Parse(root.Get(PRE_PROCESS_KEY), PRE_PROCESS_KEY), PostProcessParser
                .Parse(root.Get(POST_PROCESS_KEY), POST_PROCESS_KEY));
        }
    }
}
