using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore.Internal;
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

        public static Parser DateTimeParser { get; } = new Parser((message) =>
        {
            bool s = DateTime.TryParse(message.Content, out DateTime result);
            return s ? result : throw new ParseException("Couldn't parse that as a date.");
        });

        public static Parser URLParser { get; } = new Parser((message) =>
        {
            bool result = Uri.TryCreate(message.Content, UriKind.Absolute, out Uri? u)
                && (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps);
            if (!result)
            {
                throw new ParseException("Couldn't parse that as a URL");
            }
            return u.AbsoluteUri;
        });

        public static Parser FutureDateTimeParser { get; } = new Parser(msg =>
        {
            bool s = DateTime.TryParse(msg.Content, out DateTime result);
            if (s && result > DateTime.Now)
            {
                return result;
            }
            else if (s && result <= DateTime.Now)
            {
                throw new ParseException("Date needs to be in the future");
            }
            throw new ParseException("Couldn't parse that as a date");
        });

        public static Parser IntegerParser { get; } = new Parser(message =>
        {
            bool s = int.TryParse(message.Content, out int result);
            return s ? result : throw new ParseException("Couldn't that as integer.");
        });

        public static Parser RolesParser { get; } = new Parser(message => ParseSentRoles(message));

        public static Parser ChannelsParser { get; } = new Parser(message => message.MentionedChannels.ToList());
        public static Parser ChannelParser { get; } = new Parser(msg => GetFirst(msg.MentionedChannels,
            "Couldn't parse any channels from your message"));

        public static Parser AttachmentsParser { get; } = new Parser(message => message.Attachments.ToList());

        public static Parser AttachmentParser { get; } = new Parser(message =>
            GetFirst(message.Attachments, "Couldn't parse that as an attachment."));

        public static Parser StringParser { get; } = new Parser(message => message.Content);

        private static List<DiscordRole> ParseSentRoles(DiscordMessage message)
        {
            var guildRoleNames = message.Channel.Guild.Roles.Values.Select(x => x.Name.Trim().ToLower()).ToList();
            var guildRoles = message.Channel.Guild.Roles.Values;
            string content = Regex.Replace(message.Content, @"<@&[0-9]*>\s*,?", "").Trim().Replace(",,", "").Replace(", ,", "");
            foreach (var mention in message.MentionedRoles)
            {
                content += $",{mention.Name}";
            }
            var r = content
                .Replace("@", "")
                .Split(",")
                .Select(x => x.Trim().ToLower() == "everyone" ? "@everyone" : x.Trim().ToLower())
                .Where(x => guildRoleNames.Contains(x))
                .Select(x => guildRoles.First(y => y.Name.Trim().ToLower() == x))
                .ToList();
            if (r.Count < 1)
            {
                throw new ParseException("Couldn't find any roles in your message.");
            }
            return r;
        }

        private static T GetFirst<T>(IEnumerable<T> col, string errorMsg)
        {
            var r = col.FirstOrDefault();
            if (r == null)
            {
                throw new ParseException(errorMsg);
            }
            return r;
        }

    }
}
