using System;
using System.Collections.Generic;
using System.Text;

internal static class PdfOcrTesseract4Extensions
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
}
