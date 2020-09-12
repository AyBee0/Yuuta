using DataAccessLayer.Models.GuildModels;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataAccess.Layers
{
    public class GuidDAL : IBridge<Guild, DiscordGuild>
    {
        public void AddEntity(DiscordGuild obj, bool singleCheck = true)
        {
            
        }

        public bool Exists(DiscordGuild obj, out Guild found)
        {
            throw new NotImplementedException();
        }

        public Guild GetByDid(ulong id)
        {
            throw new NotImplementedException();
        }

        public Guild GetByDObject(DiscordGuild obj, bool createIfNew = true)
        {
            throw new NotImplementedException();
        }
    }
}
