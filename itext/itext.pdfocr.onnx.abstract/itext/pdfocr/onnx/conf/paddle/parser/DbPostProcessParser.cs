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
using iText.Pdfocr.Onnx.Util;

namespace iText.Pdfocr.Onnx.Conf.Paddle.Parser {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Static class with functions for parsing PaddleOCR YAML config objects into
    /// a
    /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.DbPostProcess"/>
    /// POJO.
    /// </summary>
    internal sealed class DbPostProcessParser {
        private const String NAME_KEY = "name";

        private const String THRESH_KEY = "thresh";

        private const String BOX_THRESH_KEY = "box_thresh";

        private const String UNCLIP_RATIO_KEY = "unclip_ratio";

        private const String MAX_CANDIDATES_KEY = "max_candidates";

        private const String USE_DILATION_KEY = "use_dilation";

        private const String SCORE_MODE_KEY = "score_mode";

        private const String BOX_TYPE_KEY = "box_type";

        private const float DEFAULT_THRESH = 0.3F;

        private const float DEFAULT_BOX_THRESH = 0.6F;

        private const float DEFAULT_UNCLIP_RATIO = 1.5F;

        private const int DEFAULT_MAX_CANDIDATES = 1000;

        private const bool DEFAULT_USE_DILATION = false;

        private static readonly ScoreMode DEFAULT_SCORE_MODE = ScoreMode.FAST;

        private static readonly BoxType DEFAULT_BOX_TYPE = BoxType.QUAD;

        private DbPostProcessParser() {
        }

//\cond DO_NOT_DOCUMENT
        // Static class
        /// <summary>
        /// Parses a YAML mapping from a PaddleOCR config file into a
        /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.DbPostProcess"/>
        /// POJO.
        /// </summary>
        /// <param name="yamlObj">YAML mapping to parse</param>
        /// <param name="keyCtx">config key under which the object is located</param>
        /// <returns>the parsed object</returns>
        internal static DbPostProcess Parse(IDictionary<Object, Object> yamlObj, String keyCtx) {
            System.Diagnostics.Debug.Assert(DbPostProcess.NAME.Equals(YamlUtil.ObjToString(yamlObj.Get(NAME_KEY))));
            return new DbPostProcess(ParserUtil.GetOrDefault(yamlObj, keyCtx, THRESH_KEY, DEFAULT_THRESH), ParserUtil.
                GetOrDefault(yamlObj, keyCtx, BOX_THRESH_KEY, DEFAULT_BOX_THRESH), ParserUtil.GetOrDefault(yamlObj, keyCtx
                , UNCLIP_RATIO_KEY, DEFAULT_UNCLIP_RATIO), ParserUtil.GetOrDefault(yamlObj, keyCtx, MAX_CANDIDATES_KEY
                , DEFAULT_MAX_CANDIDATES), ParserUtil.GetOrDefault(yamlObj, keyCtx, USE_DILATION_KEY, DEFAULT_USE_DILATION
                ), ParserUtil.GetOrDefault(yamlObj, keyCtx, SCORE_MODE_KEY, DEFAULT_SCORE_MODE), ParserUtil.GetOrDefault
                (yamlObj, keyCtx, BOX_TYPE_KEY, DEFAULT_BOX_TYPE));
        }
//\endcond
    }
//\endcond
}
