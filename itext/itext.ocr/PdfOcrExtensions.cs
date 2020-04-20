using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using iText.IO.Util;
using System.Reflection;
using iText.Ocr;


public static class PdfOcrExtensions
{
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

}
