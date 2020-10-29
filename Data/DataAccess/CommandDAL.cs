using DataAccessLayer.Models;
using DataAccessLayer.Models.CommandModels;
using DSharpPlus.CommandsNext;
using System.Linq;

namespace DataAccessLayer.DataAccess.Layers
{
    public class CommandDAL
    {
        public static YuutaCommand GetByDObject(Command dCommand, bool createIfUnique)
        {
            using var db = new YuutaDbContext();
            var found = db.Commands.SingleOrDefault(x => x.CommandName == dCommand.Name);
            if (found == null && createIfUnique)
            {
                found = new YuutaCommand(dCommand);
                db.Add(found);
            }
            return found;
        }
    }
}
