using Newtonsoft.Json;
using System.Collections.Generic;

namespace Types {

    public class Emoji {
        [JsonProperty("EmojiID")]
        public ulong Id { get; set; }
        public ulong RoleID { get; set; }
        public string RoleName { get; set; }
    }

    public class ReactionMessage {
        public Dictionary<string,Emoji> Emojis { get; set; }
    }

}
