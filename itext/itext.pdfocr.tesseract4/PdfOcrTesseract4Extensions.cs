using System;
using System.Collections.Generic;
using System.Text;

public static class PdfOcrTesseract4Extensions
{
    public static String Name(this Encoding e)
    {
        return e.WebName.ToUpperInvariant();
    }
}
