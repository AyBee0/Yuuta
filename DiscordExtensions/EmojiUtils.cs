using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DiscordExtensions {

    public static class EmojiUtils {

        public static DiscordEmoji ParseDiscordEmoji(DiscordClient client, string toParse) {
            DiscordEmoji emoji;
            try {
                emoji = DiscordEmoji.FromName(client, toParse);
            } catch (Exception) {
                var emojiNameMatch = Regex.Match(toParse, @":([^:]*):");
                if (emojiNameMatch.Success) {
                    emoji = DiscordEmoji.FromName(client, emojiNameMatch.Value);
                } else {
                    emoji = DiscordEmoji.FromUnicode(client, toParse);
                }
            }
            return emoji;
        }

        public static string GetEmbedFriendlyEmojiName(this DiscordEmoji emoji) {
            if (emoji.RequiresColons) {
                return $"<:{emoji.Name}:{emoji.Id}>";
            } else {
                return emoji.Name;
            }
        }

    }

}
