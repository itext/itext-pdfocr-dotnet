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
using iText.Commons.Utils;
using iText.Pdfocr.Onnxtr.Conf.Paddle.Model;
using iText.Pdfocr.Onnxtr.Exceptions;
using iText.Pdfocr.Onnxtr.Util;

namespace iText.Pdfocr.Onnxtr.Conf.Paddle.Parser {
//\cond DO_NOT_DOCUMENT
    /// <summary>
    /// Static class with functions for parsing PaddleOCR YAML config objects into
    /// a
    /// <see cref="iText.Pdfocr.Onnxtr.Conf.Paddle.Model.PreProcess"/>
    /// POJO.
    /// </summary>
    internal sealed class PreProcessParser {
        private const String TRANSFORM_OPS_KEY = "transform_ops";

        private static readonly ICollection<String> IGNORED_TRANSFORM_OPS = new HashSet<String>(JavaUtil.ArraysAsList
            ("DetLabelEncode", "KeepKeys", "MultiLabelEncode", "ToCHWImage"));

        private PreProcessParser() {
        }

//\cond DO_NOT_DOCUMENT
        // Static class
        /// <summary>
        /// Parses a YAML mapping from a PaddleOCR config file into a
        /// <see cref="iText.Pdfocr.Onnxtr.Conf.Paddle.Model.PreProcess"/>
        /// POJO.
        /// </summary>
        /// <param name="yamlObj">YAML mapping to parse</param>
        /// <param name="keyCtx">config key under which the object is located</param>
        /// <returns>the parsed object</returns>
        internal static PreProcess Parse(Object yamlObj, String keyCtx) {
            IDictionary<Object, Object> root = YamlUtil.ObjToMapping(yamlObj);
            if (root == null) {
                throw ConfigParserException.UnexpectedValueForKey(keyCtx);
            }
            ICollection<Object> keys = new HashSet<Object>(root.Keys);
            keys.Remove(TRANSFORM_OPS_KEY);
            if (!keys.IsEmpty()) {
                String key = keys.Iterator().Current.ToString();
                throw ConfigParserException.UnexpectedKey(keyCtx + "." + key);
            }
            ICollection<Object> transformOps = YamlUtil.ObjToSequence(root.Get(TRANSFORM_OPS_KEY));
            if (transformOps == null) {
                throw ConfigParserException.UnexpectedValueForKey(keyCtx + "." + TRANSFORM_OPS_KEY);
            }
            IList<TransformOp> parsedOps = new List<TransformOp>(transformOps.Count);
            int idx = 0;
            for (IEnumerator<Object> it = transformOps.GetEnumerator(); it.MoveNext(); ++idx) {
                String entryKeyCtx = keyCtx + "." + TRANSFORM_OPS_KEY + "." + idx;
                IDictionary<Object, Object> op = YamlUtil.ObjToMapping(it.Current);
                if (op == null || op.Count != 1) {
                    throw ConfigParserException.UnexpectedValueForKey(entryKeyCtx);
                }
                KeyValuePair<Object, Object> opEntry = op.Iterator().Current;
                String opKey = YamlUtil.ObjToString(opEntry.Key);
                if (DecodeImage.WRAPPING_KEY.Equals(opKey)) {
                    parsedOps.Add(DecodeImageParser.Parse(opEntry.Value, entryKeyCtx + "." + DecodeImage.WRAPPING_KEY));
                }
                else {
                    if (DetResizeForTest.WRAPPING_KEY.Equals(opKey)) {
                        parsedOps.Add(DetResizeForTestParser.Parse(opEntry.Value, entryKeyCtx + "." + DetResizeForTest.WRAPPING_KEY
                            ));
                    }
                    else {
                        if (NormalizeImage.WRAPPING_KEY.Equals(opKey)) {
                            parsedOps.Add(NormalizeImageParser.Parse(opEntry.Value, entryKeyCtx + "." + NormalizeImage.WRAPPING_KEY));
                        }
                        else {
                            if (RecResizeImg.WRAPPING_KEY.Equals(opKey)) {
                                parsedOps.Add(RecResizeImgParser.Parse(opEntry.Value, entryKeyCtx + "." + RecResizeImg.WRAPPING_KEY));
                            }
                            else {
                                if (opKey == null || !IGNORED_TRANSFORM_OPS.Contains(opKey)) {
                                    throw ConfigParserException.UnexpectedValueForKey(entryKeyCtx);
                                }
                            }
                        }
                    }
                }
            }
            // Else it is an ignored op and we do nothing
            return new PreProcess(parsedOps.ToArray(new TransformOp[0]));
        }
//\endcond
    }
//\endcond
}
