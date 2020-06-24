using DataAccessLayer;
using DataAccessLayer.DataAccess;
using DataAccessLayer.Models.ChannelModels;
using DataAccessLayer.Models.GuildModels;
using DataAccessLayer.Models.RoleModels;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordAccessLayer
{
    public class DiscordGuildAL
    {
        public Guild Guild { get; private set; }
        public List<Role> StaffRoles { get; private set; } = new List<Role>();
        public List<Role> GlobalCommandRoles { get; private set; } = new List<Role>();
        public List<Role> GlobalMacroRoles { get; private set; } = new List<Role>();
        public List<Role> NormalRoles { get; private set; } = new List<Role>();
        public List<Channel> BotChannels { get; set; }

        public DiscordGuildAL(DiscordGuild dGuild)
        {
            using (var guildDAL = new GuildDAL())
            {
                this.Guild = guildDAL.GetGuildByDGuild(dGuild);
            }
            this.StaffRoles = this.Guild.Roles.Where(r => r.RoleType == RoleTypeEnum.Staff).ToList();
            this.GlobalCommandRoles = this.Guild.Roles.Where(r => r.RoleType == RoleTypeEnum.GlobalCommands).ToList();
            this.GlobalMacroRoles = this.Guild.Roles.Where(r => r.RoleType == RoleTypeEnum.GlobalMacros).ToList();
            this.NormalRoles = this.Guild.Roles.Where(r => r.RoleType == RoleTypeEnum.Normal).ToList();
            this.BotChannels = this.Guild.Channels.Where(c => c.ChannelType == ChannelTypeEnum.BotChannel).ToList();
        }
        public DiscordGuildAL(DiscordMember member) : this(member.Guild) { }
        public DiscordGuildAL(DiscordChannel channel) : this(channel.Guild) { }

        //public async Task ManageBotFirstRunAsync()
        //{
        //    using (var guildDAL = new GuildDAL())
        //    {
        //        guildDAL.AddGuildIfUnique(dGuild);
        //    }
        //    var currentMember = dGuild.CurrentMember;
        //    await currentMember.ModifyAsync(x => x.Nickname = currentMember.Nickname + " | !help.");
        //}

        public static void NewGuildCreated(DiscordGuild dGuild)
        {
            using var dal = new GuildDAL();
            dal.AddGuildIfUnique(dGuild);
        }

        public static void AddInitialGuildsIfUnique(List<DiscordGuild> guilds)
        {
            foreach (var dGuild in guilds)
            {
                using (var guildDAL = new GuildDAL())
                {
                    guildDAL.AddGuildIfUnique(dGuild);
                }
                var currentMember = dGuild.CurrentMember;
            }
        }

    }
}