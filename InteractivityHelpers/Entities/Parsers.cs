using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InteractivityHelpers.Entities
{
    public static class Parsers
    {
        //public static Func<DiscordMessage, DateTime> DateTimeParser =
        //    (message) => DateTime.Parse(message.Content);

        //public static Func<DiscordMessage, int> IntegerParser =
        //    (message) => int.Parse(message.Content);

        //public static Func<DiscordMessage, List<DiscordRole>> RolesParser =
        //    (message) => ParseSentRoles(message);

        //public static Func<DiscordMessage, List<DiscordChannel>> ChannelsParser =
        //    (message) => message.MentionedChannels.ToList();

        //public static Func<DiscordMessage, List<DiscordAttachment>> AttachmentsParser =
        //    (message) => message.Attachments.ToList();

        public static Parser DateTimeParser { get; }
            = new Parser(message => DateTime.TryParse(message.Content, out DateTime result) ? result : (DateTime?)null);

        public static Parser IntegerParser { get; }
            = new Parser(message => int.TryParse(message.Content, out int result) ? result : (int?)null);

        public static Parser RolesParser { get; }
            = new Parser(message => ParseSentRoles(message));

        public static Parser ChannelsParser { get; }
            = new Parser(message => message.MentionedChannels.ToList());

        public static Parser AttachmentsParser { get; }
            = new Parser(message => message.Attachments.ToList());

        public static Parser StringParser { get; }
            = new Parser(message => message.Content);

        private static List<DiscordRole> ParseSentRoles(DiscordMessage message)
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
