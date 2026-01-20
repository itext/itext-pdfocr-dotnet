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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace iText.Pdfocr.Onnxtr.Util {
    /// <summary>Functions for working with YAML documents.</summary>
    public sealed class YamlUtil {
        private YamlUtil() { 
            // Static class
        }

        /// <summary>Deserializes a content stream, which contains a single YAML document.</summary>
        /// <remarks>
        /// Deserializes a content stream, which contains a single YAML document.
        /// <para />
        /// This method returns a regular object. To get access to concrete types
        /// in the document, use the
        /// <c>objTo*</c>
        /// family of functions from this
        /// class.
        /// </remarks>
        /// <param name="content">YAML document content stream to parse</param>
        /// <returns>the parsed object</returns>
        public static Object DeserializeFromStream(Stream content) {
            /*
            * We will be using our custom schema here. This means, that scalar
            * values, which do not have an explicit tag attached, will either be
            * nulls or strings. By default, it is using a JSON schema, which
            * guesses types in implicit cases. This is not what we really want,
            * and we will waste performance on all the guessing, so, instead, we
            * will just rely on objTo* methods. This will also make porting a bit
            * more predictable.
            */
            IDeserializer deserializer = new DeserializerBuilder().Build();

            using (StreamReader reader = new StreamReader(content)) {
                return deserializer.Deserialize(reader);
            }
        }

        /// <summary>Casts a parsed YAML object to a map/dictionary, if it is a mapping.</summary>
        /// <remarks>
        /// Casts a parsed YAML object to a map/dictionary, if it is a mapping.
        /// Otherwise, returns
        /// <see langword="null"/>.
        /// </remarks>
        /// <param name="obj">parsed YAML object to cast</param>
        /// <returns>
        /// mapping or
        /// <see langword="null"/>
        /// </returns>
        public static IDictionary<Object, Object> ObjToMapping(Object obj) {
            if (obj is IDictionary) {
                return (IDictionary<Object, Object>)obj;
            }
            return null;
        }

        /// <summary>Casts a parsed YAML object to a collection, if it is a sequence.</summary>
        /// <remarks>
        /// Casts a parsed YAML object to a collection, if it is a sequence.
        /// Otherwise, returns
        /// <see langword="null"/>.
        /// </remarks>
        /// <param name="obj">parsed YAML object to cast</param>
        /// <returns>
        /// sequence or
        /// <see langword="null"/>
        /// </returns>
        public static ICollection<Object> ObjToSequence(Object obj) {
            if (obj is ICollection && !(obj is IDictionary)) {
                ICollection<Object> collection = new List<Object>();
                foreach (Object item in (ICollection)obj) {
                    collection.Add(item);
                }
                return collection;
            }
            return null;
        }

        /// <summary>Casts a parsed YAML object to a string, if it is a string.</summary>
        /// <remarks>
        /// Casts a parsed YAML object to a string, if it is a string.
        /// Otherwise, returns
        /// <see langword="null"/>.
        /// <para />
        /// If the object in the YAML document had an explicit type tag, which is
        /// not str, then this method won't do any conversions and will just return
        /// <see langword="null"/>.
        /// </remarks>
        /// <param name="obj">parsed YAML object to cast</param>
        /// <returns>
        /// string or
        /// <see langword="null"/>
        /// </returns>
        public static String ObjToString(Object obj) {
            // With our schema, this should be enough. In case it is explicitly
            // not a string, then we will do no conversions
            if (obj is String) {
                return (String)obj;
            }
            return null;
        }

        /// <summary>Casts a parsed YAML object to a boolean, if it is a bool.</summary>
        /// <remarks>
        /// Casts a parsed YAML object to a boolean, if it is a bool.
        /// Otherwise, returns
        /// <see langword="null"/>.
        /// <para />
        /// If the object in the YAML document had an explicit type tag, which is
        /// not bool, then this method won't do any conversions and will just
        /// return
        /// <see langword="null"/>.
        /// </remarks>
        /// <param name="obj">parsed YAML object to cast</param>
        /// <returns>
        /// boolean or
        /// <see langword="null"/>
        /// </returns>
        public static bool? ObjToBool(Object obj) {
            // In case there was an explicit tag, it will already be Boolean
            if (obj is bool?) {
                return (bool?)obj;
            }
            // In the implicit case we will try to parse the string
            if ("false".Equals(obj)) {
                return false;
            }
            if ("true".Equals(obj)) {
                return true;
            }
            return null;
        }

        /// <summary>Casts a parsed YAML object to an int32, if it is an int.</summary>
        /// <remarks>
        /// Casts a parsed YAML object to an int32, if it is an int.
        /// Otherwise, returns
        /// <see langword="null"/>.
        /// <para />
        /// If the object in the YAML document had an explicit type tag, which is
        /// not int, then this method won't do any conversions and will just return
        /// <see langword="null"/>.
        /// <para />
        /// If the value does not fit into int32, then this method will also return
        /// <see langword="null"/>.
        /// </remarks>
        /// <param name="obj">parsed YAML object to cast</param>
        /// <returns>
        /// int32 or
        /// <see langword="null"/>
        /// </returns>
        public static int? ObjToInt(Object obj) {
            // We will only work with int32 here, if it only fit into int64 or
            // bigint, then we will assume "failure"
            if (obj is int?) {
                return (int?)obj;
            }
            // In the implicit case we will try to parse the string
            if (obj is String) {
                try {
                    return Convert.ToInt32((String)obj, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception) {
                }
            }
            // Empty
            return null;
        }

        /// <summary>Casts a parsed YAML object to a double, if it is a float.</summary>
        /// <remarks>
        /// Casts a parsed YAML object to a double, if it is a float.
        /// Otherwise, returns
        /// <see langword="null"/>.
        /// <para />
        /// If the object in the YAML document had an explicit type tag, which is
        /// not float, then this method won't do any conversions and will just
        /// return
        /// <see langword="null"/>.
        /// <para />
        /// If the value does not fit into int32, then this method will also return
        /// <see langword="null"/>.
        /// </remarks>
        /// <param name="obj">parsed YAML object to cast</param>
        /// <returns>
        /// double or
        /// <see langword="null"/>
        /// </returns>
        public static double? ObjToFloat(Object obj) {
            // Explicit tag case
            if (obj is double?) {
                return (double?)obj;
            }
            // In the implicit case we will try to parse the string
            if (obj is String) {
                String s = (String)obj;
                switch (s) {
                    case "-.inf": {
                        return double.NegativeInfinity;
                    }

                    case ".inf": {
                        return double.PositiveInfinity;
                    }

                    case ".nan": {
                        return double.NaN;
                    }

                    default: {
                        try {
                            return Double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (Exception) {
                        }
                        break;
                    }
                }
            }
            // Empty
            return null;
        }
    }
}
