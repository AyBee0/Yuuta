using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Types.DatabaseObjects;
using static FirebaseHelper.YuutaFirebaseClient;

namespace AuthorityHelpers {

    public class AuthorityHelper {

        private DiscordMember Member { get; set; }
        private DiscordChannel Channel { get; set; }
        private DiscordMessage Message { get; set; }
        private DiscordGuild DiscordGuild { get; set; }

        private IReadOnlyList<DiscordRole> MemberRoles
        {
            get {
                return Member.Roles.ToList();
            }
        }
        private Guild GuildObject
        {
            get {
                if (Database == null) {
                    throw new ArgumentNullException("Database cannot be null");
                }
                return Database.Guilds.GetValueOrDefault(DiscordGuild.Id.ToString());
            }
        }

        public AuthorityHelper(CommandContext ctx) {
            Member = ctx.Member;
            Channel = ctx.Channel;
            Message = ctx.Message;
            DiscordGuild = ctx.Guild;
        }

        //public AuthorityHelper(DiscordMember member, DiscordChannel channel, IReadOnlyList<DiscordRole> memberRoles, DiscordMessage message, string messageContent, DiscordGuild guild, Guild guildObject) {
        //    Member = member;
        //    Channel = channel;
        //    MemberRoles = memberRoles;
        //    Message = message;
        //    MessageContent = messageContent;
        //    Guild = guild;
        //    GuildObject = guildObject;
        //}

        public bool IsStaffMember
        {
            get {
                try {
                    return GuildObject?.Info?.Authority?.StaffRoles?.Any(x => MemberRoles?.Select(y => y.Id).Any(z => z == x) == true || x == DiscordGuild.EveryoneRole.Id) == true;
                }
                catch (Exception e) {
                    Console.WriteLine(e.StackTrace);
                    throw e;
                }
            }
        }

        public static bool CheckIfStaffMember(DiscordMember member, DiscordGuild guild) {
            var staffRoles = Database?.Guilds?.GetValueOrDefault(guild.Id.ToString())?.Info?.Authority?.StaffRoles.ToList();
            var memberRoles = member.Roles.Select(x => x.Id);
            return memberRoles.Any(x => staffRoles?.Contains(x) == true);
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
                var staffRoles = Database?.Guilds?.GetValueOrDefault(DiscordGuild.Id.ToString())?.Info?.Authority?.StaffRoles?.ToList();
                var globalBotChannels = Database?.Guilds?.GetValueOrDefault(DiscordGuild.Id.ToString())?.Info?.Authority?.GlobalBotChannels?.ToList();
                var globalBotOverrides = Database?.Guilds?.GetValueOrDefault(DiscordGuild.Id.ToString())?.Info?.Authority?.GlobalBotRoleOverrides?.ToList();
                var memberRoles = Member.Roles.Select(x => x.Id).ToList();
                return (globalBotChannels?.Contains(Channel.Id) == null | true) || IsStaffMember || memberRoles.Any(x => globalBotOverrides?.Contains(x) == true)
                    ? true
                    : false;
            }
        }

        public static bool MemberCanSendInChannel(DiscordMember member, DiscordChannel discordChannel, ulong guildId) {
            var globalBotChannels = Database?.Guilds?.GetValueOrDefault(guildId.ToString())?.Info?.Authority?.GlobalBotChannels.ToList();
            var globalBotOverrides = Database?.Guilds?.GetValueOrDefault(guildId.ToString())?.Info?.Authority?.GlobalBotRoleOverrides.ToList();
            var memberRoles = member.Roles.Select(x => x.Id).ToList();
            return globalBotChannels?.Contains(discordChannel.Id) == null || true || CheckIfStaffMember(member,discordChannel.Guild) || memberRoles.Any(x => globalBotOverrides?.Contains(x) == true)
                ? true
                : false;
        }

    }

}
