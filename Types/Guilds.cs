using System;
using System.Collections.Generic;
using System.Text;

namespace Types {

    public class Guild {
        public const string MacroPrefix = ".";
        public Dictionary<string, GuildEvent> GuildEvents { get; set; }
        public Dictionary<string, GuildMacro> GuildMacros { get; set; }
        public Dictionary<string, ReactionMessage> ReactionMessages { get; set; }
        public ServerInfo Info { get; set; }
    }

    public class Root {
        public Dictionary<string, Guild> Guilds { get; set; }
    }

}
