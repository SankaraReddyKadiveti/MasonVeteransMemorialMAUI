using System;
using System.Linq;

namespace MasonVeteransMemorial.BusinessServices.CommonUtils
{
    public class Utils
    {
        public Utils()
        {
        }

        public static int GetIndexInAlphabet(char value)
        {
            // Uses the uppercase character unicode code point. 'A' = U+0042 = 65, 'Z' = U+005A = 90
            char upper = char.ToUpper(value);
            if (upper < 'A' || upper > 'Z')
            {
                throw new ArgumentOutOfRangeException(nameof(value), "This method only accepts standard Latin characters.");
            }

            return (int)upper - (int)'A';
        }

    }
}
