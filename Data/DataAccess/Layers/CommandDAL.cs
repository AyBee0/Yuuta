using DataAccessLayer.Models;
using DataAccessLayer.Models.CommandModels;
using DSharpPlus.CommandsNext;
using System;
using System.Linq;

namespace DataAccessLayer.DataAccess.Layers
{
    public class CommandDAL : IBridge<YuutaCommand, Command>
    {
        public void AddEntity(Command obj, bool singleCheck = true)
        {
            using var db = new YuutaDbContext();
            if (db.Commands.SingleOrDefault(x => x.CommandName == obj.Name) != null)
            {
                return;
            }
            db.Commands.Add(new YuutaCommand(obj));
            db.SaveChanges();
        }

        public bool Exists(Command obj, out YuutaCommand found)
        {
            found = GetByDObject(obj, false);
            return found != null;
        }

        public YuutaCommand GetByDid(ulong id)
        {
            throw new InvalidOperationException("Discord commands don't have IDs");
        }

        public YuutaCommand GetByDObject(Command obj, bool createIfNew = false)
        {
            using var db = new YuutaDbContext();
            YuutaCommand found = db.Commands.SingleOrDefault(x => x.CommandName == obj.Name);
            if (found == null && createIfNew)
            {
                found = new YuutaCommand(obj);
                db.Commands.Add(found);
            }
            return found;
        }
    }
}
