using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InteractivityHelpers
{
    public static class InteractivityParser
    {

        public static List<DiscordRole> ParseSentRoles(this InteractivityResult<DiscordMessage> interactivityResult)
        {
             if (interactivityResult.Result == null)
            {
                return null;
            }
            var result = interactivityResult.Result;
            var guildRoleNames = result.Channel.Guild.Roles.Values.Select(x => x.Name.Trim().ToLower()).ToList();
            var guildRoles = result.Channel.Guild.Roles.Values;
            //var matches = Regex.Matches(result.Content, @"<@&[0-9]*>");
            //foreach (var match in matches)
            //{

            //}
            string content = Regex.Replace(result.Content, @"<@&[0-9]*>\s*,?", "").Trim().Replace(",,","").Replace(", ,","");
            foreach (var mention in result.MentionedRoles)
            {
                content = content + $",{mention.Name}";
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
