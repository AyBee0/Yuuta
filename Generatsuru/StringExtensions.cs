using System;
using System.Collections.Generic;
using System.Text;

namespace Generatsuru
{
    public static class StringExtensions
    {
        public static string ToTrimmedLower(this string s)
        {
            return s.ToLower().Trim();
        }
    }
}
