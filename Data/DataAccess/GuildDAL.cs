using DataAccessLayer.Models;
using DataAccessLayer.Models.GuildModels;
using DSharpPlus.Entities;
using System;
using System.Linq;

namespace DataAccessLayer.DataAccess.Layers
{
    public static class GuildDAL
    {
        public static Guild GetByDObject(DiscordGuild guild, bool createIfNew = true)
        {
            using var db = new YuutaDbContext();
            var found = db.Guilds.SingleOrDefault(x => x.GuildDid == guild.Id);
            if (found == null && createIfNew)
            {
                found = new Guild(guild);
                db.Guilds.Add(found);
            }
            return found;
        }

        public static void AddGuildIfUnique(DiscordGuild dGuild)
        {
            using var db = new YuutaDbContext();
            if (!Exists(dGuild, out Guild _, db))
            {
                db.Add(new Guild(dGuild));
            }
        }

        private static bool Exists(DiscordGuild dguild, out Guild guild, YuutaDbContext dbContext)
        {
            guild = dbContext.Guilds.SingleOrDefault(x => x.GuildDid == dguild.Id);
            return guild != null;
        }

    }
}
