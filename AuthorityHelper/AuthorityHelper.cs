using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using Types.DatabaseObjects;
using static FirebaseHelper.YuutaFirebaseClient;

namespace AuthorityHelpers {

    public class AuthorityHelper {

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
            GuildObject = Database.Guilds.ContainsKey(Guild.Id.ToString()) ? Database.Guilds[Guild.Id.ToString()] : null;
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
                return GuildObject?.Info?.Authority?.StaffRoles?.Any(x => MemberRoles?.Select(y => y.Id).Any(z => z == x) == true || x == Guild.EveryoneRole.Id) == true;
            }
        }

        public bool CanSendInChannel
        {
            get {
                //Probably a way simpler way to do this but im sleepy heck off. No, ?. won't work  because I need it to return true if it's null.
                //return (GuildObject == null || GuildObject.Info == null || GuildObject.Info.Authority == null || GuildObject.Info.Authority.GlobalBotChannels == null) ? true:
                //    GuildObject.Info.Authority.GlobalBotChannels.Any(x => x == Channel.Id) == true || GuildObject.Info.Authority.GlobalBotRoleOverrides.Any(x => MemberRoles?.Select(y => y.Id).Any(z => z == x) == true) == true;
                //return GuildObject == null || GuildObject.Info == null || GuildObject.Info.Authority == null || GuildObject.Info.Authority.GlobalBotChannels == null ? true:
                //    GuildObject.Info.Authority.GlobalBotChannels.Any(x => x == Channel.Id) == true ||
                //    GuildObject?.Info?.Authority?.GlobalBotRoleOverrides?.Any(x => MemberRoles?.Select(y => y.Id).Any(z => z == x) == true) == true;
                return GuildObject?.Info?.Authority?.GlobalBotChannels == null
                    ? true
                    : GuildObject?.Info?.Authority?.StaffRoles?.Any(x => MemberRoles?.Select(y => y.Id).Any(z => z == x) == true) == true ||
                    GuildObject.Info.Authority.GlobalBotChannels.Any(x => x == Channel.Id) == true ||
                    GuildObject?.Info?.Authority?.GlobalBotRoleOverrides?.Any(x => MemberRoles?.Select(y => y.Id).Any(z => z == x) == true) == true;
            }
        }

    }

}
