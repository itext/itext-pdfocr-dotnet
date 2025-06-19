using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iText.Pdfocr.Util
{
    public class Objects
    {
        public static T RequireNonNull<T> (T obj, string message = "Object cannot be null")
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), message);
            }
            return obj;
        }
    }
}
