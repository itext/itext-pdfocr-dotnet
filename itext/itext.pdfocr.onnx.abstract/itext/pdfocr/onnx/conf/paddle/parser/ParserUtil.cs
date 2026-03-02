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
    /// Internal static class with extracted functions related to PaddleOCR
    /// configuration file parsing.
    /// </summary>
    internal sealed class ParserUtil {
        private ParserUtil() {
        }

//\cond DO_NOT_DOCUMENT
        // Static class
        /// <summary>Tries to cast a YAML sequence object to an array of strings.</summary>
        /// <remarks>
        /// Tries to cast a YAML sequence object to an array of strings. Throws
        /// a
        /// <see cref="iText.Pdfocr.Onnx.Exceptions.ConfigParserException"/>
        /// exception on error.
        /// </remarks>
        /// <param name="obj">YAML sequence object to cast</param>
        /// <param name="keyCtx">config key under which the object is located</param>
        /// <returns>resulting array of strings</returns>
        internal static String[] CastToStringArray(ICollection<Object> obj, String keyCtx) {
            if (obj == null) {
                throw ConfigParserException.UnexpectedValueForKey(keyCtx);
            }
            String[] casted = new String[obj.Count];
            int idx = 0;
            for (IEnumerator<Object> it = obj.GetEnumerator(); it.MoveNext(); ++idx) {
                String elem = YamlUtil.ObjToString(it.Current);
                if (elem == null) {
                    throw ConfigParserException.UnexpectedValueForKey(keyCtx + "." + idx);
                }
                casted[idx] = elem;
            }
            return casted;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Tries to cast a YAML sequence object to an array of integers.</summary>
        /// <remarks>
        /// Tries to cast a YAML sequence object to an array of integers. Throws
        /// a
        /// <see cref="iText.Pdfocr.Onnx.Exceptions.ConfigParserException"/>
        /// exception on error.
        /// </remarks>
        /// <param name="obj">YAML sequence object to cast</param>
        /// <param name="keyCtx">config key under which the object is located</param>
        /// <returns>resulting array of integers</returns>
        internal static int[] CastToIntArray(ICollection<Object> obj, String keyCtx) {
            if (obj == null) {
                throw ConfigParserException.UnexpectedValueForKey(keyCtx);
            }
            int[] casted = new int[obj.Count];
            int idx = 0;
            for (IEnumerator<Object> it = obj.GetEnumerator(); it.MoveNext(); ++idx) {
                int? elem = YamlUtil.ObjToInt(it.Current);
                if (elem == null) {
                    throw ConfigParserException.UnexpectedValueForKey(keyCtx + "." + idx);
                }
                casted[idx] = (int)elem;
            }
            return casted;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>Tries to cast a YAML sequence object to an array of floats.</summary>
        /// <remarks>
        /// Tries to cast a YAML sequence object to an array of floats. Throws
        /// a
        /// <see cref="iText.Pdfocr.Onnx.Exceptions.ConfigParserException"/>
        /// exception on error.
        /// </remarks>
        /// <param name="obj">YAML sequence object to cast</param>
        /// <param name="keyCtx">config key under which the object is located</param>
        /// <returns>resulting array of floats</returns>
        internal static float[] CastToFloatArray(ICollection<Object> obj, String keyCtx) {
            if (obj == null) {
                throw ConfigParserException.UnexpectedValueForKey(keyCtx);
            }
            float[] casted = new float[obj.Count];
            int idx = 0;
            for (IEnumerator<Object> it = obj.GetEnumerator(); it.MoveNext(); ++idx) {
                double? elem = YamlUtil.ObjToFloat(it.Current);
                if (elem == null) {
                    throw ConfigParserException.UnexpectedValueForKey(keyCtx + "." + idx);
                }
                casted[idx] = (float)elem;
            }
            return casted;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Tries to get a float value from a YAML mapping object under the
        /// specified key.
        /// </summary>
        /// <remarks>
        /// Tries to get a float value from a YAML mapping object under the
        /// specified key. If key is not present, returns
        /// <paramref name="defaultValue"/>.
        /// Throws a
        /// <see cref="iText.Pdfocr.Onnx.Exceptions.ConfigParserException"/>
        /// exception on error.
        /// </remarks>
        /// <param name="obj">YAML mapping object to get the value from</param>
        /// <param name="keyCtx">config key under which the mapping object is located</param>
        /// <param name="key">key to use for value extraction</param>
        /// <param name="defaultValue">value to return if value is missing</param>
        /// <returns>
        /// value from the mapping object, if present, or
        /// <paramref name="defaultValue"/>
        /// </returns>
        internal static float GetOrDefault(IDictionary<Object, Object> obj, String keyCtx, String key, float defaultValue
            ) {
            Object value = obj.Get(key);
            if (value == null && !obj.ContainsKey(key)) {
                return defaultValue;
            }
            double? casted = YamlUtil.ObjToFloat(value);
            if (casted == null) {
                throw ConfigParserException.UnexpectedValueForKey(keyCtx + "." + key);
            }
            return (float)casted;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Tries to get a float array value from a YAML mapping object under the
        /// specified key.
        /// </summary>
        /// <remarks>
        /// Tries to get a float array value from a YAML mapping object under the
        /// specified key. If key is not present, returns
        /// <paramref name="defaultValue"/>.
        /// Throws a
        /// <see cref="iText.Pdfocr.Onnx.Exceptions.ConfigParserException"/>
        /// exception on error.
        /// </remarks>
        /// <param name="obj">YAML mapping object to get the value from</param>
        /// <param name="keyCtx">config key under which the mapping object is located</param>
        /// <param name="key">key to use for value extraction</param>
        /// <param name="defaultValue">value to return if value is missing</param>
        /// <returns>
        /// value from the mapping object, if present, or
        /// <paramref name="defaultValue"/>
        /// </returns>
        internal static float[] GetOrDefault(IDictionary<Object, Object> obj, String keyCtx, String key, float[] defaultValue
            ) {
            Object value = obj.Get(key);
            if (value == null && !obj.ContainsKey(key)) {
                return defaultValue;
            }
            return CastToFloatArray(YamlUtil.ObjToSequence(value), keyCtx + "." + key);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Tries to get an integer value from a YAML mapping object under the
        /// specified key.
        /// </summary>
        /// <remarks>
        /// Tries to get an integer value from a YAML mapping object under the
        /// specified key. If key is not present, returns
        /// <paramref name="defaultValue"/>.
        /// Throws a
        /// <see cref="iText.Pdfocr.Onnx.Exceptions.ConfigParserException"/>
        /// exception on error.
        /// </remarks>
        /// <param name="obj">YAML mapping object to get the value from</param>
        /// <param name="keyCtx">config key under which the mapping object is located</param>
        /// <param name="key">key to use for value extraction</param>
        /// <param name="defaultValue">value to return if value is missing</param>
        /// <returns>
        /// value from the mapping object, if present, or
        /// <paramref name="defaultValue"/>
        /// </returns>
        internal static int GetOrDefault(IDictionary<Object, Object> obj, String keyCtx, String key, int defaultValue
            ) {
            Object value = obj.Get(key);
            if (value == null && !obj.ContainsKey(key)) {
                return defaultValue;
            }
            int? casted = YamlUtil.ObjToInt(value);
            if (casted == null) {
                throw ConfigParserException.UnexpectedValueForKey(keyCtx + "." + key);
            }
            return (int)casted;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Tries to get an integer array value from a YAML mapping object under
        /// the specified key.
        /// </summary>
        /// <remarks>
        /// Tries to get an integer array value from a YAML mapping object under
        /// the specified key. If key is not present, returns
        /// <paramref name="defaultValue"/>.
        /// Throws a
        /// <see cref="iText.Pdfocr.Onnx.Exceptions.ConfigParserException"/>
        /// exception on error.
        /// </remarks>
        /// <param name="obj">YAML mapping object to get the value from</param>
        /// <param name="keyCtx">config key under which the mapping object is located</param>
        /// <param name="key">key to use for value extraction</param>
        /// <param name="defaultValue">value to return if value is missing</param>
        /// <returns>
        /// value from the mapping object, if present, or
        /// <paramref name="defaultValue"/>
        /// </returns>
        internal static int[] GetOrDefault(IDictionary<Object, Object> obj, String keyCtx, String key, int[] defaultValue
            ) {
            Object value = obj.Get(key);
            if (value == null && !obj.ContainsKey(key)) {
                return defaultValue;
            }
            return CastToIntArray(YamlUtil.ObjToSequence(value), keyCtx + "." + key);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Tries to get a boolean value from a YAML mapping object under the
        /// specified key.
        /// </summary>
        /// <remarks>
        /// Tries to get a boolean value from a YAML mapping object under the
        /// specified key. If key is not present, returns
        /// <paramref name="defaultValue"/>.
        /// Throws a
        /// <see cref="iText.Pdfocr.Onnx.Exceptions.ConfigParserException"/>
        /// exception on error.
        /// </remarks>
        /// <param name="obj">YAML mapping object to get the value from</param>
        /// <param name="keyCtx">config key under which the mapping object is located</param>
        /// <param name="key">key to use for value extraction</param>
        /// <param name="defaultValue">value to return if value is missing</param>
        /// <returns>
        /// value from the mapping object, if present, or
        /// <paramref name="defaultValue"/>
        /// </returns>
        internal static bool GetOrDefault(IDictionary<Object, Object> obj, String keyCtx, String key, bool defaultValue
            ) {
            Object value = obj.Get(key);
            if (value == null && !obj.ContainsKey(key)) {
                return defaultValue;
            }
            bool? casted = YamlUtil.ObjToBool(value);
            if (casted == null) {
                throw ConfigParserException.UnexpectedValueForKey(keyCtx + "." + key);
            }
            return (bool)casted;
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Tries to get a
        /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.ScoreMode"/>
        /// value from a YAML mapping object
        /// under the specified key.
        /// </summary>
        /// <remarks>
        /// Tries to get a
        /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.ScoreMode"/>
        /// value from a YAML mapping object
        /// under the specified key. If key is not present, returns
        /// <paramref name="defaultValue"/>
        /// . Throws a
        /// <see cref="iText.Pdfocr.Onnx.Exceptions.ConfigParserException"/>
        /// exception
        /// on error.
        /// </remarks>
        /// <param name="obj">YAML mapping object to get the value from</param>
        /// <param name="keyCtx">config key under which the mapping object is located</param>
        /// <param name="key">key to use for value extraction</param>
        /// <param name="defaultValue">value to return if value is missing</param>
        /// <returns>
        /// value from the mapping object, if present, or
        /// <paramref name="defaultValue"/>
        /// </returns>
        internal static ScoreMode GetOrDefault(IDictionary<Object, Object> obj, String keyCtx, String key, ScoreMode
             defaultValue) {
            Object value = obj.Get(key);
            if (value == null && !obj.ContainsKey(key)) {
                return defaultValue;
            }
            String casted = YamlUtil.ObjToString(value);
            if ("fast".Equals(casted)) {
                return ScoreMode.FAST;
            }
            if ("slow".Equals(casted)) {
                return ScoreMode.SLOW;
            }
            throw ConfigParserException.UnexpectedValueForKey(keyCtx + "." + key);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Tries to get a
        /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.BoxType"/>
        /// value from a YAML mapping object under
        /// the specified key.
        /// </summary>
        /// <remarks>
        /// Tries to get a
        /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.BoxType"/>
        /// value from a YAML mapping object under
        /// the specified key. If key is not present, returns
        /// <paramref name="defaultValue"/>.
        /// Throws a
        /// <see cref="iText.Pdfocr.Onnx.Exceptions.ConfigParserException"/>
        /// exception on error.
        /// </remarks>
        /// <param name="obj">YAML mapping object to get the value from</param>
        /// <param name="keyCtx">config key under which the mapping object is located</param>
        /// <param name="key">key to use for value extraction</param>
        /// <param name="defaultValue">value to return if value is missing</param>
        /// <returns>
        /// value from the mapping object, if present, or
        /// <paramref name="defaultValue"/>
        /// </returns>
        internal static BoxType GetOrDefault(IDictionary<Object, Object> obj, String keyCtx, String key, BoxType defaultValue
            ) {
            Object value = obj.Get(key);
            if (value == null && !obj.ContainsKey(key)) {
                return defaultValue;
            }
            String casted = YamlUtil.ObjToString(value);
            if ("quad".Equals(casted)) {
                return BoxType.QUAD;
            }
            if ("poly".Equals(casted)) {
                return BoxType.POLY;
            }
            throw ConfigParserException.UnexpectedValueForKey(keyCtx + "." + key);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        /// <summary>
        /// Tries to get an
        /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.ImgMode"/>
        /// value from a YAML mapping object under
        /// the specified key.
        /// </summary>
        /// <remarks>
        /// Tries to get an
        /// <see cref="iText.Pdfocr.Onnx.Conf.Paddle.Model.ImgMode"/>
        /// value from a YAML mapping object under
        /// the specified key. If key is not present, returns
        /// <paramref name="defaultValue"/>.
        /// Throws a
        /// <see cref="iText.Pdfocr.Onnx.Exceptions.ConfigParserException"/>
        /// exception on error.
        /// </remarks>
        /// <param name="obj">YAML mapping object to get the value from</param>
        /// <param name="keyCtx">config key under which the mapping object is located</param>
        /// <param name="key">key to use for value extraction</param>
        /// <param name="defaultValue">value to return if value is missing</param>
        /// <returns>
        /// value from the mapping object, if present, or
        /// <paramref name="defaultValue"/>
        /// </returns>
        internal static ImgMode GetOrDefault(IDictionary<Object, Object> obj, String keyCtx, String key, ImgMode defaultValue
            ) {
            Object value = obj.Get(key);
            if (value == null && !obj.ContainsKey(key)) {
                return defaultValue;
            }
            String casted = YamlUtil.ObjToString(value);
            if ("GRAY".Equals(casted)) {
                return ImgMode.GRAY;
            }
            if ("RGB".Equals(casted)) {
                return ImgMode.RGB;
            }
            if ("BGR".Equals(casted)) {
                return ImgMode.BGR;
            }
            throw ConfigParserException.UnexpectedValueForKey(keyCtx + "." + key);
        }
//\endcond
    }
//\endcond
}
