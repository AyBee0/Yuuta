using DataAccessLayer;
using DataAccessLayer.Models.ChannelModels;
using DataAccessLayer.Models.GuildModels;
using DataAccessLayer.Models.RoleModels;
using DSharpPlus.Entities;
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
        public DiscordGuildAL(DiscordGuild guild)
        {
            this.Guild = GuildDAL.GetGuildByDGuild(guild);
            this.StaffRoles = this.Guild.Roles.Where(r => r.RoleType == RoleTypeEnum.Staff).ToList();
            this.GlobalCommandRoles = this.Guild.Roles.Where(r => r.RoleType == RoleTypeEnum.GlobalCommands).ToList();
            this.GlobalMacroRoles = this.Guild.Roles.Where(r => r.RoleType == RoleTypeEnum.GlobalMacros).ToList();
            this.NormalRoles = this.Guild.Roles.Where(r => r.RoleType == RoleTypeEnum.Normal).ToList();
            this.BotChannels = this.Guild.Channels.Where(c => c.ChannelType == ChannelTypeEnum.BotChannel).ToList();
        }
        public DiscordGuildAL(DiscordMember member) : this(member.Guild) { }
        public DiscordGuildAL(DiscordChannel channel) : this(channel.Guild) { }

        public async static Task ManageBotFirstRunAsync(DiscordGuild guild)
        {
            GuildDAL.AddGuildIfUnique(guild);
            var currentMember = guild.CurrentMember;
            await currentMember.ModifyAsync(x => x.Nickname = currentMember.Nickname + " | !help.");
        }
    }
}