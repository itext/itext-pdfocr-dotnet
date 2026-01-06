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
using System.Reflection;
using System.Text;
//\cond DO_NOT_DOCUMENT
internal static class PdfOcrOnnxTRExtensions
{
    public static String Name(this Encoding e)
    {
        return e.WebName.ToUpperInvariant();
    }
    
    /// <summary>Returns the number of Unicode code points in the specified text range of this String.</summary>
    /// <remarks>
    /// Returns the number of Unicode code points in the specified text range of this String.
    /// The text range begins at the specified beginIndex and extends to the char at index endIndex - 1.
    /// Thus the length (in chars) of the text range is endIndex-beginIndex.
    /// Unpaired surrogates within the text range count as one code point each.
    /// </remarks>
    /// <param name="str">str to search codepoints for.</param>
    /// <param name="beginIndex">– the index to the first char of the text range.</param>
    /// <param name="endIndex">– the index after the last char of the text range.</param>
    /// <returns>the number of Unicode code points in the specified text range</returns>
    public static int CodePointCount(this String str, int beginIndex, int endIndex) {
        if (str == null)
            throw new ArgumentNullException(nameof(str));
        if (beginIndex < 0 || endIndex > str.Length || beginIndex > endIndex)
            throw new ArgumentOutOfRangeException();

        int count = 0;
        for (int i = beginIndex; i < endIndex; i++)
        {
            if (char.IsHighSurrogate(str[i]))
            {
                if (i + 1 < endIndex && char.IsLowSurrogate(str[i + 1]))
                {
                    i++; 
                }
            }
            count++;
        }
        return count;
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

    public static StringBuilder JAppend(this StringBuilder sb, String str, int begin, int end) {
        return sb.Append(str, begin, end - begin);
    }

    public static String JSubstring(this String str, int beginIndex, int endIndex) {
        return str.Substring(beginIndex, endIndex - beginIndex);
    }
    
    public static String Substring(this StringBuilder collector, int beginIndex) {
        return collector.ToString().Substring(beginIndex);
    }
    
    public static IronSoftware.Drawing.AnyBitmap GetSubimage(this IronSoftware.Drawing.AnyBitmap image, 
        int x, int y, int width, int height) {
        return image.Clone(new IronSoftware.Drawing.Rectangle(x, y, width, height));
    }
    
    public static void ForEachRemaining<E>(this IEnumerator<E> iterator, IList<E> list) {
        while (iterator.MoveNext()) {
            list.Add(iterator.Current);
        }
    }
    
    public static void Close<T,R>(this iText.Pdfocr.Onnxtr.IPredictor<T,R> predictor) {
        predictor.Dispose();
    }

    public static bool IsEmpty<T>(this IList<T> list) {
        return list.Count == 0;
    }
}
//\endcond
