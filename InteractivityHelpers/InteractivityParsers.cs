using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return result.Content
                .Replace("@", "")
                .Split(",")
                .Select(x => x.Trim().ToLower() == "everyone" ? "@everyone" : x.Trim().ToLower())
                .Where(x => guildRoleNames.Contains(x))
                .Select(x => guildRoles.First(y => y.Name.Trim().ToLower() == x))
                .ToList();
        }

    }
}
