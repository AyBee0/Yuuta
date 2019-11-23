using System;
using System.Collections.Generic;
using System.Text;

namespace Types.DatabaseObjects {
    public class Guild {
        public const string MacroPrefix = ".";
        public Dictionary<string, GuildEvent> GuildEvents { get; set; }
        public Dictionary<string, GuildMacro> GuildMacros { get; set; }
        public Dictionary<string, ReactionMessage> ReactionMessages { get; set; }
        public GuildInfo Info { get; set; }
    }
}
