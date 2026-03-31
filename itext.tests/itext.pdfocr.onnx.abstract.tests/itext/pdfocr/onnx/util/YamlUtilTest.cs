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
using System.Linq;
using iText.Commons.Utils;
using iText.Test;

namespace iText.Pdfocr.Onnx.Util {
    [NUnit.Framework.Category("UnitTest")]
    public class YamlUtilTest : ExtendedITextTest {
        private static readonly String TEST_DIRECTORY = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/pdfocr/onnx/util/YamlUtilTest/";

        private static readonly String TYPES_TEST_FILE = TEST_DIRECTORY + "types.yml";

        [NUnit.Framework.Test]
        public virtual void DeserializeFromStreamTest() {
            Object yaml;
            using (Stream @is = iText.Commons.Utils.FileUtil.GetInputStreamForFile(System.IO.Path.Combine(TYPES_TEST_FILE
                ))) {
                yaml = YamlUtil.DeserializeFromStream(@is);
            }
            NUnit.Framework.Assert.IsInstanceOf(typeof(IDictionary), yaml);
            IDictionary<Object, Object> root = (IDictionary<Object, Object>)yaml;
            // String
            NUnit.Framework.Assert.AreEqual("Hello, YAML", root.Get("string_plain"));
            NUnit.Framework.Assert.AreEqual("Hello, YAML with quotes", root.Get("string_quoted"));
            NUnit.Framework.Assert.AreEqual("This is a multi-line\n" + "literal block scalar.\n" + "Preserves line breaks.\n"
                , root.Get("string_multiline"));
            NUnit.Framework.Assert.AreEqual("This is a folded block scalar that folds newlines into spaces.\n", root.Get
                ("string_folded"));
            // Null
            NUnit.Framework.Assert.IsTrue(root.ContainsKey("null_implicit"));
            NUnit.Framework.Assert.IsNull(root.Get("null_implicit"));
            NUnit.Framework.Assert.IsTrue(root.ContainsKey("null_shortcut"));
            NUnit.Framework.Assert.IsNull(root.Get("null_shortcut"));
            // Boolean, should remain a string, if no explicit tag
            NUnit.Framework.Assert.AreEqual("true", root.Get("boolean_true"));
            NUnit.Framework.Assert.AreEqual("false", root.Get("boolean_false"));
            NUnit.Framework.Assert.AreEqual(true, root.Get("boolean_explicit_true"));
            NUnit.Framework.Assert.AreEqual(false, root.Get("boolean_explicit_false"));
            // Int, should remain a string, if no explicit tag
            NUnit.Framework.Assert.AreEqual("42", root.Get("int_decimal"));
            NUnit.Framework.Assert.AreEqual("0o52", root.Get("int_octal"));
            NUnit.Framework.Assert.AreEqual("0x2A", root.Get("int_hexadecimal"));
            NUnit.Framework.Assert.AreEqual(42, root.Get("int_explicit"));
            // Float, should remain a string, if no explicit tag
            NUnit.Framework.Assert.AreEqual("3.14159", root.Get("float_plain"));
            NUnit.Framework.Assert.AreEqual("1.2e+3", root.Get("float_exponent"));
            NUnit.Framework.Assert.AreEqual("-.inf", root.Get("float_negative_inf"));
            NUnit.Framework.Assert.AreEqual(".inf", root.Get("float_positive_inf"));
            NUnit.Framework.Assert.AreEqual(".nan", root.Get("float_nan"));
            NUnit.Framework.Assert.AreEqual(3.0, root.Get("float_explicit"));
            // Sequence
            AssertObjectCollectionEquals(JavaUtil.ArraysAsList((Object)"red", "green", "blue"), root.Get("sequence_inline"
                ));
            AssertObjectCollectionEquals(JavaUtil.ArraysAsList((Object)"apple", "banana", "cherry"), root.Get("sequence_block"
                ));
 {
                // Mapping (inline)
                Object obj = root.Get("mapping_inline");
                NUnit.Framework.Assert.IsInstanceOf(typeof(IDictionary), obj);
                IDictionary<Object, Object> mapping = (IDictionary<Object, Object>)obj;
                NUnit.Framework.Assert.AreEqual(2, mapping.Count);
                NUnit.Framework.Assert.AreEqual("Alice", mapping.Get("name"));
                NUnit.Framework.Assert.AreEqual("30", mapping.Get("age"));
            }
 {
                // Mapping (block)
                Object obj = root.Get("mapping_block");
                NUnit.Framework.Assert.IsInstanceOf(typeof(IDictionary), obj);
                IDictionary<Object, Object> mapping = (IDictionary<Object, Object>)obj;
                NUnit.Framework.Assert.AreEqual(3, mapping.Count);
                NUnit.Framework.Assert.AreEqual("Bob", mapping.Get("name"));
                NUnit.Framework.Assert.AreEqual("25", mapping.Get("age"));
                NUnit.Framework.Assert.AreEqual("true", mapping.Get("active"));
            }
        }

        [NUnit.Framework.Test]
        public virtual void ObjToMappingTest() {
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToMapping("not map"));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToMapping(null));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToMapping(true));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToMapping(3));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToMapping(3.14));
            IDictionary<Object, Object> map = new Dictionary<Object, Object>();
            NUnit.Framework.Assert.AreEqual(map, YamlUtil.ObjToMapping(map));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToMapping(new List<Object>()));
        }

        [NUnit.Framework.Test]
        public virtual void MappingElementsTest() {
            IDictionary<int, List<String>> map = new Dictionary<int, List<String>>();
            List<String> array = new List<String>(JavaUtil.ArraysAsList("one", "two", "three"));
            map.Put(1, array);
            map.Put(2, new List<String>());
            IDictionary<Object, Object> newMap = YamlUtil.ObjToMapping(map);
            NUnit.Framework.Assert.AreEqual(2, newMap.Count);
            NUnit.Framework.Assert.IsTrue(map.ContainsKey(1));
            NUnit.Framework.Assert.AreEqual(array, newMap.Get(1));
            NUnit.Framework.Assert.IsTrue(map.ContainsKey(2));
            NUnit.Framework.Assert.AreEqual(new List<Object>(), newMap.Get(2));
        }

        [NUnit.Framework.Test]
        public virtual void ObjToSequenceTest() {
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToSequence("not seq"));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToSequence(null));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToSequence(true));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToSequence(3));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToSequence(3.14));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToSequence(new Dictionary<Object, Object>()));
            IList<Object> seq = new List<Object>();
            NUnit.Framework.Assert.AreEqual(seq, YamlUtil.ObjToSequence(seq));
        }

        [NUnit.Framework.Test]
        public virtual void SequenceElementsTest() {
            IList<int> seq = new List<int>(JavaUtil.ArraysAsList(1, 2, 3));
            ICollection<Object> newSeq = YamlUtil.ObjToSequence(seq);
            NUnit.Framework.Assert.IsTrue(newSeq.Contains(1));
            NUnit.Framework.Assert.IsTrue(newSeq.Contains(2));
            NUnit.Framework.Assert.IsTrue(newSeq.Contains(3));
        }

        [NUnit.Framework.Test]
        public virtual void ObjToStringTest() {
            String str = "3.14";
            NUnit.Framework.Assert.AreEqual(str, YamlUtil.ObjToString(str));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToString(null));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToString(true));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToString(3));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToString(3.14));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToString(new Dictionary<Object, Object>()));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToString(new List<Object>()));
        }

        [NUnit.Framework.Test]
        public virtual void ObjToBoolTest() {
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToBool("not bool"));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToBool(null));
            NUnit.Framework.Assert.AreEqual(true, YamlUtil.ObjToBool(true));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToBool(3));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToBool(3.14));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToBool(new Dictionary<Object, Object>()));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToBool(new List<Object>()));
            // Implicit case
            NUnit.Framework.Assert.AreEqual(true, YamlUtil.ObjToBool("true"));
            NUnit.Framework.Assert.AreEqual(false, YamlUtil.ObjToBool("false"));
        }

        [NUnit.Framework.Test]
        public virtual void ObjToIntTest() {
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToInt("not int"));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToInt(null));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToInt(true));
            int? i = 3;
            NUnit.Framework.Assert.AreEqual(i, YamlUtil.ObjToInt(i));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToInt(3.14));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToInt(new Dictionary<Object, Object>()));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToInt(new List<Object>()));
            // Implicit case
            NUnit.Framework.Assert.AreEqual(-42, YamlUtil.ObjToInt("-42"));
            NUnit.Framework.Assert.AreEqual(42, YamlUtil.ObjToInt("42"));
        }

        [NUnit.Framework.Test]
        public virtual void ObjToFloatTest() {
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToFloat("not float"));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToFloat(null));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToFloat(true));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToFloat(3));
            double? f = 3.14;
            NUnit.Framework.Assert.AreEqual(f, YamlUtil.ObjToFloat(f));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToFloat(new Dictionary<Object, Object>()));
            NUnit.Framework.Assert.IsNull(YamlUtil.ObjToFloat(new List<Object>()));
            // Implicit case
            NUnit.Framework.Assert.AreEqual(-3.14, YamlUtil.ObjToFloat("-3.14"));
            NUnit.Framework.Assert.AreEqual(3.14, YamlUtil.ObjToFloat("3.14"));
            NUnit.Framework.Assert.AreEqual(-1200.0, YamlUtil.ObjToFloat("-1.2e+3"));
            NUnit.Framework.Assert.AreEqual(1200.0, YamlUtil.ObjToFloat("1.2e+3"));
            NUnit.Framework.Assert.AreEqual(-42.0, YamlUtil.ObjToFloat("-42"));
            NUnit.Framework.Assert.AreEqual(42.0, YamlUtil.ObjToFloat("42"));
            NUnit.Framework.Assert.AreEqual(double.NegativeInfinity, YamlUtil.ObjToFloat("-.inf"));
            NUnit.Framework.Assert.AreEqual(double.PositiveInfinity, YamlUtil.ObjToFloat(".inf"));
            NUnit.Framework.Assert.IsTrue(double.IsNaN((double)YamlUtil.ObjToFloat(".nan")));
        }

        private static void AssertObjectCollectionEquals(ICollection<Object> expected, Object actual) {
            NUnit.Framework.Assert.IsInstanceOf(typeof(ICollection), actual);
            NUnit.Framework.Assert.AreEqual(expected.ToArray(), ((ICollection<Object>)actual).ToArray());
        }
    }
}
