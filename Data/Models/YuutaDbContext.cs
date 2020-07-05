using DataAccessLayer.Models.ChannelModels;
using DataAccessLayer.Models.CommandModels;
using DataAccessLayer.Models.GuildModels;
using DataAccessLayer.Models.GuildModels.RoleMessages;
using DataAccessLayer.Models.RoleModels;
using Generatsuru;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DataAccessLayer.Models
{
    public class YuutaDbContext : DbContext
    {
        private readonly string dbPath = $"Data Source={Environment.GetEnvironmentVariable("YuutaDbPath")}";

        public DbSet<Guild> Guilds { get; set; }
        public DbSet<GuildSettings> GuildSettings { get; set; }
        public DbSet<GuildMacro> GuildMacros { get; set; }

        public DbSet<RoleMessage> RoleMessages { get; set; }
        public DbSet<RoleMessageItem> RoleMessageItems { get; set; }

        public DbSet<YuutaCommand> Commands { get; set; }
        public DbSet<CommandType> CommandTypes { get; set; }
        public DbSet<CommandRestrictionOverload> CommandRestrictionOverloads { get; set; }

        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleType> RoleTypes { get; set; }

        public DbSet<ChannelType> ChannelTypes { get; set; }
        public DbSet<Channel> Channels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(dbPath);
            options.EnableSensitiveDataLogging(true);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.SeedEnumValues<RoleType, RoleTypeEnum>(e => e);
            builder.SeedEnumValues<ChannelType, ChannelTypeEnum>(e => e);
            builder.SeedEnumValues<CommandType, CommandTypeEnum>(e => e);
        }

    }
}
