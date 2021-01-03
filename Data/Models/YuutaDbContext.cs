using DataAccessLayer.Models.ChannelModels;
using DataAccessLayer.Models.CommandModels;
using DataAccessLayer.Models.Events;
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
        public DbSet<CommandRestrictionOverload> CommandRestrictionOverloads { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Channel> Channels { get; set; }

        #region Events
        public DbSet<DirectMessageEvent> DirectMessageEvents { get; set; }
        public DbSet<RoleEvent> RoleEvents { get; set; }
        public DbSet<GuildMessageEvent> GuildMessageEvents { get; set; }
        public DbSet<ReactionLinkedEvent> ReactionLinkedEvents { get; set; }
        public DbSet<EventUser> User { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(dbPath);
            options.EnableSensitiveDataLogging(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<YuutaCommand>()
            //    .Property(c => c.CommandType).HasConversion<int>();
            //modelBuilder.Entity<Role>()
            //    .Property(c => c.RoleType).HasConversion<int>();
            //modelBuilder.Entity<Channel>()
            //    .Property(c => c.ChannelType).HasConversion<int>();
        }
    }
}
