using System;
using System.Collections.Generic;
using System.Text;
using Types.GuildCommands;

namespace Types {

    public class Guild {
        public const string MacroPrefix = ".";
        public Dictionary<string, GuildEvent> GuildEvents { get; set; }
        public Dictionary<string, GuildMacro> GuildMacros { get; set; }
        public Dictionary<string, ReactionMessage> ReactionMessages { get; set; }
        public GuildInfo Info { get; set; }
    }

    //In case you're asking what in the literal hell I'm doing: FirebaseDatabase.net is retarded, basically. Don't use firebase unless you're developing a mobile
    //app, people ;-;
    public class YuutaBot {
        public Dictionary<string, GuildCommand> Commands { get; set; }
        public Dictionary<string, Guild> Guilds { get; set; }
    }

    public class Root {
        public YuutaBot YuutaBot { get; set; }
    }

}
