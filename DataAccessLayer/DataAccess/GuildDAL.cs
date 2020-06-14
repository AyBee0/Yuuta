using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using DataAccessLayer.Models;
using DataAccessLayer.Models.GuildModels;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DataAccessLayer
{
    public class GuildDAL
    {
        public static void AddGuildIfUnique(DiscordGuild dGuild)
        {
            using var db = new YuutaDbContext();
            if (!GuildExists(dGuild, out Guild guild))
            {
                guild = new Guild(dGuild);
                db.Add(guild);
            }
        }
        private static bool GuildExists(DiscordGuild dGuild, out Guild guild)
        {
            guild = GetGuildByDGuild(dGuild);
            return guild != null;
        }

        /// <summary>
        /// Gets the guild by discord ID.
        /// </summary>
        /// <param name="did">Guild Discord ID</param>
        /// <returns>Null if not found, Guild otherwise.</returns>
        public static Guild GetGuildByDGuild(DiscordGuild dGuild, bool createIfNew = true)
        {
            using var db = new YuutaDbContext();
            return GetGuildByDGuild(dGuild, db, createIfNew);
        }
        private static Guild GetGuildByDGuild(DiscordGuild dGuild, YuutaDbContext db, bool createIfNew = true)
        {
            var found = db.Guilds.SingleOrDefault(x => x.GuildDid == (long)dGuild.Id);
            if (found == null && createIfNew)
            {
                found = new Guild(dGuild);
            }
            return found;
        }
    }
}
