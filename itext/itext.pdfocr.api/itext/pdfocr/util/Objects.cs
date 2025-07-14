using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iText.Pdfocr.Util
{
    public class Objects
    {
        public static T RequireNonNull<T> (T obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException(nameof(obj));
            }
            return obj;
        }
    }
}
