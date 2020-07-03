using DataAccessLayer.Models;
using DataAccessLayer.Models.GuildModels;
using DSharpPlus.Entities;
using System;
using System.Linq;

namespace DataAccessLayer.DataAccess
{
    public class GuildDAL : DAL
    {
        public void AddGuildIfUnique(DiscordGuild dGuild)
        {
            if (!GuildExists(dGuild, out Guild guild))
            {
                Console.WriteLine("Guild doesn't exist");
                guild = new Guild(dGuild);
                Database.Add(guild);
            }
            Console.WriteLine(guild.GuildName);
        }
        private bool GuildExists(DiscordGuild dGuild, out Guild guild)
        {
            guild = GetGuildByDGuild(dGuild);
            return guild != null;
        }

        /// <summary>
        /// Gets the guild by discord ID.
        /// </summary>
        /// <param name="did">Guild Discord-provided ID</param>
        /// <returns>Null if not found, Guild otherwise.</returns>
        public Guild GetGuildByDGuild(DiscordGuild dGuild, bool createIfNew = true)
        {
            return GetGuildByDGuild(dGuild, Database, createIfNew);
        }
        private Guild GetGuildByDGuild(DiscordGuild dGuild, YuutaDbContext db, bool createIfNew = true)
        {
            var found = db.Guilds.SingleOrDefault(x => (ulong) x.GuildDid == dGuild.Id);
            if (found == null && createIfNew)
            {
                found = new Guild(dGuild);
            }
            return found;
        }

        public Guild GetGuildByDid(ulong id)
        {
            var found = Database.Guilds.AsEnumerable().Where(x => x.GuildDid == (long) id);
            return found.ElementAtOrDefault(0);
        }

    }
}
