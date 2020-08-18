using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractivityHelpers.Entities
{
    public static class Conditions
    {
        public static Func<DiscordMessage, bool> DateTimeCondition =>
            x => DateTime.TryParse(x.Content, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _);

        public static Func<DiscordMessage, bool> ChannelCondition =>
            x => x.MentionedChannels.Count > 0;

        public static Func<DiscordMessage, bool> AttachmentCondition =>
            x => x.Attachments.Count > 0;

        public static Func<DiscordMessage, bool> RolesCondition =>
        (message) => InteractivityTools.ParseSentRoles(message)?.Count > 0;

        public static Func<DiscordMessage, bool> IntegerCondition =>
            x => int.TryParse(x.Content.Trim(), out int result);

        public static Func<DiscordMessage, bool> LinkCondition =>
            x => Uri.IsWellFormedUriString(x.Content.Trim(), UriKind.RelativeOrAbsolute);

    }
}
