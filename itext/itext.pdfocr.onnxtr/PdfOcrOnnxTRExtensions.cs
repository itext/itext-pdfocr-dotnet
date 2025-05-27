/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
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
using System.Reflection;
using System.Text;
//\cond DO_NOT_DOCUMENT
internal static class PdfOcrOnnxTRExtensions
{
    public static String Name(this Encoding e)
    {
        return e.WebName.ToUpperInvariant();
    }

    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key)
    {
        TValue value = default(TValue);
        if (key != null)
        {
            col.TryGetValue(key, out value);
        }

        return value;
    }

    public static TValue Put<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key, TValue value)
    {
        TValue oldVal = col.Get(key);
        col[key] = value;
        return oldVal;
    }

    public static byte[] GetBytes(this String str)
    {
        return System.Text.Encoding.UTF8.GetBytes(str);
    }

    public static Assembly GetAssembly(this Type type)
    {
        return type.Assembly;
    }

    public static Attribute GetCustomAttribute(this Assembly assembly, Type attributeType)
    {
        object[] customAttributes = assembly.GetCustomAttributes(attributeType, false);
        if (customAttributes.Length > 0 && customAttributes[0] is Attribute)
        {
            return customAttributes[0] as Attribute;
        }
        else
        {
            return null;
        }
    }

}
//\endcond