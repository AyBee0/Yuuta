using System.Collections.Generic;
using Types.DatabaseObjects;

namespace Types {

    //In case you're asking what in the literal hell I'm doing: FirebaseDatabase.net is retarded, basically. Don't use firebase unless you're developing a mobile
    //app, people ;-;
    public class YuutaBot {
        public Dictionary<string, Command> Commands { get; set; }
        public Dictionary<string, Guild> Guilds { get; set; }
        public YuutaBotSettings BotSettings { get; set; }
    }

    public class Root {
        public YuutaBot YuutaBot { get; set; }
    }

}
