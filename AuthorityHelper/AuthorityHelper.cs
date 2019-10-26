using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Types;

namespace AuthorityHelpers {

    public class AuthorityHelper {

        public static Dictionary<string, Guild> Guilds { get; set; }

        private readonly DiscordMember Member;
        private readonly DiscordChannel Channel;
        private readonly IReadOnlyList<DiscordRole> MemberRoles;
        private readonly DiscordMessage Message;
        private readonly string MessageContent;
        private readonly DiscordGuild Guild;
        private readonly Guild GuildObject;

        public AuthorityHelper(CommandContext ctx) {
            Member = ctx.Member;
            Channel = ctx.Channel;
            MemberRoles = ctx.Member.Roles.ToList();
            MessageContent = ctx.Message.Content;
            Message = ctx.Message;
            Guild = ctx.Guild;
            GuildObject = Guilds[Guild.Id.ToString()];
        }

        public AuthorityHelper(DiscordMember member, DiscordChannel channel, IReadOnlyList<DiscordRole> memberRoles, DiscordMessage message, string messageContent, DiscordGuild guild, Guild guildObject) {
            Member = member;
            Channel = channel;
            MemberRoles = memberRoles;
            Message = message;
            MessageContent = messageContent;
            Guild = guild;
            GuildObject = guildObject;
        }

        public bool IsStaffMember
        {
            get {
                return GuildObject.Info.Authority.StaffRoles?.Any(x => MemberRoles?.Select(y => y.Id).Any(z => z == x) == true) == true;
            }
        }

        public bool CanSendInChannel
        {
            get {
                return GuildObject.Info.Authority.GlobalBotChannels?.Any(x => x == Channel.Id) == true || GuildObject.Info.Authority.GlobalBotRoleOverrides?.Any(x => MemberRoles?.Select(y => y.Id).Any(z => z == x) == true) == true;
            }
        }

    }

}
