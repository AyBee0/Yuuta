using DSharpPlus.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Types {

    public class ReactionEmoji {
        [JsonProperty("EmojiName")]
        public string EmojiName { get; set; }
        [JsonProperty("RoleIds")]
        private string RoleIdsNotList { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonIgnore]
        public DiscordEmoji Emoji { get; set; }
        [JsonIgnore]
        public IEnumerable<ulong> RoleIds
        {
            get {
                return RoleIdsNotList?.Split("|").ToList().Select(x => x.Trim()).Select(ulong.Parse).ToList();
            }
            set {
                RoleIdsNotList = string.Join("|", value.ToArray());
            }
        }
    }

    public class ReactionMessage {
        public Dictionary<string,ReactionEmoji> Emojis { get; set; }
        public string ChannelId { get; set; }
    }

}
