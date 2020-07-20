using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InteractivityHelpers
{
    public static class InteractivityTools
    {
        public static List<DiscordRole> ParseSentRoles(DiscordMessage message)
        {
            var guildRoleNames = message.Channel.Guild.Roles.Values.Select(x => x.Name.Trim().ToLower()).ToList();
            var guildRoles = message.Channel.Guild.Roles.Values;
            string content = Regex.Replace(message.Content, @"<@&[0-9]*>\s*,?", "").Trim().Replace(",,", "").Replace(", ,", "");
            foreach (var mention in message.MentionedRoles)
            {
                content += $",{mention.Name}";
            }
            return content
                .Replace("@", "")
                .Split(",")
                .Select(x => x.Trim().ToLower() == "everyone" ? "@everyone" : x.Trim().ToLower())
                .Where(x => guildRoleNames.Contains(x))
                .Select(x => guildRoles.First(y => y.Name.Trim().ToLower() == x))
                .ToList();
        }

    }
}
