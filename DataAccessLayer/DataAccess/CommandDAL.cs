using DataAccessLayer.Models.CommandModels;
using DSharpPlus.CommandsNext;
using System.Linq;

namespace DataAccessLayer.DataAccess
{
    public class CommandDAL : DAL
    {
        public void AddCommandIfNew(Command dCommand)
        {
            if (!CommandExists(dCommand, out YuutaCommand _))
            {
                AddCommand(dCommand);
            }
        }

        public YuutaCommand AddCommand(Command dCommand)
        {
            var command = new YuutaCommand(dCommand);
            Database.Add(command);
            return command;
        }

        public bool CommandExists(Command dCommand, out YuutaCommand command)
        {
            command = GetCommandByName(dCommand.Name);
            return command != null;
        }

        /// <summary>
        /// Get database-stored command by its name.
        /// </summary>
        /// <param name="name">Command name</param>
        /// <returns>Database-stored Command type. Null if not found.</returns>
        public YuutaCommand GetCommandByName(string name)
        {
            return null;
        }

        /// <summary>
        /// Get database-stored command by the DSharpPlus command type.
        /// </summary>
        /// <param name="dCommand">DSharpPlus command type</param>
        /// <param name="createIfNew">True: If the command is not found, create it, returns null if false.</param>
        /// <returns></returns>
        public YuutaCommand GetCommand(Command dCommand, bool createIfNew)
        {
            var command = GetCommandByName(dCommand.Name);
            if (createIfNew && command == null)
            {
                return AddCommand(dCommand);
            }
            else
            {
                return command;
            }
        }
    }
}
