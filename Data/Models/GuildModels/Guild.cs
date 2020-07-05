using DataAccessLayer.Models.ChannelModels;
using DataAccessLayer.Models.CommandModels;
using DataAccessLayer.Models.GuildModels.RoleMessages;
using DataAccessLayer.Models.RoleModels;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.GuildModels
{
    public class Guild
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GuildId { get; set; }

        [Required]
        public long GuildDid { get; set; }

        [Required]
        public string GuildName { get; set; }
        [Required]
        public DateTime GuildBotJoined { get; } = DateTime.Now;
        public GuildSettings GuildSettings { get; set; } = new GuildSettings();

        public List<Role> Roles { get; set; } = new List<Role>();
        public List<GuildMacro> GuildMacros { get; set; } = new List<GuildMacro>();
        public List<Channel> Channels { get; set; } = new List<Channel>();
        public List<RoleMessage> RoleMessages { get; set; } = new List<RoleMessage>();

        #region Constructors
        protected Guild()
        {
        }
        public Guild(DiscordGuild guild)
        {
            this.GuildDid = (long)guild.Id;
            this.GuildName = guild.Name;
            this.Roles.AddRange(guild.Roles.Values.Select(x => new Role(x)));
        }

        public Guild(string guildName, ulong guildDid)
        {
            this.GuildName = guildName;
            this.GuildDid = (long)guildDid;
        }
        #endregion

    }
}