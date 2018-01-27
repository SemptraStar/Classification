using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Utility
{
    public static class ParseUtilities
    {
        public static int? ParseToNullableInt(this string s)
        { 
            int i;

            if (int.TryParse(s, out i))
                return i;

            return null;
        }
    }
}
