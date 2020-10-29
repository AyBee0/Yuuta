using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals
{
    public static class Configuration
    {
#if !DEBUG
        public static string[] Prefixes { get; } = new string[] { "!", "~", "-", };
        public static bool EnableMentions { get; } = true;
#else
        public static string[] Prefixes { get; } = new string[] { "tt!" };
        public static bool EnableMentions { get; } = false;
#endif
    }
}
