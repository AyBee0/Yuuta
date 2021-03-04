﻿using DataAccessLayer;
using DataAccessLayer.DataAccess;
using DataAccessLayer.DataAccess.Layers;
using DataAccessLayer.Models.ChannelModels;
using DataAccessLayer.Models.GuildModels;
using DataAccessLayer.Models.RoleModels;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordMan
{
    public class DiscordGuildMan
    {
        public Guild Guild { get; private set; }
        public List<Role> StaffRoles { get; private set; } = new List<Role>();
        public List<Role> GlobalCommandRoles { get; private set; } = new List<Role>();
        public List<Role> GlobalMacroRoles { get; private set; } = new List<Role>();
        public List<Role> NormalRoles { get; private set; } = new List<Role>();
        public List<Channel> BotChannels { get; set; }

        public DiscordGuildMan(DiscordGuild dGuild)
        {
            this.Guild = GuildDAL.GetByDObject(dGuild);
            this.StaffRoles = this.Guild.Roles.Where(r => r.RoleType == RoleType.Staff).ToList();
            this.GlobalCommandRoles = this.Guild.Roles.Where(r => r.RoleType == RoleType.GlobalCommands).ToList();
            this.GlobalMacroRoles = this.Guild.Roles.Where(r => r.RoleType == RoleType.GlobalMacros).ToList();
            this.NormalRoles = this.Guild.Roles.Where(r => r.RoleType == RoleType.Normal).ToList();
            this.BotChannels = this.Guild.Channels.Where(c => c.ChannelType == ChannelType.BotChannel).ToList();
        }

        public DiscordGuildMan(DiscordMember member) : this(member.Guild) { }
        public DiscordGuildMan(DiscordChannel channel) : this(channel.Guild) { }

    }
}