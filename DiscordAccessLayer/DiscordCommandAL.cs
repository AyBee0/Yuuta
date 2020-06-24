using DataAccessLayer.Models.CommandModels;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Models;
using DataAccessLayer.DataAccess;
using Generatsuru;

namespace DiscordAccessLayer
{
    public class DiscordCommandAL
    {
        /// <summary>
        /// GuildDid, RoleDid KVP
        /// </summary>
        public Dictionary<long, long> ExplicitlyDeniedRoles { get; private set; } = new Dictionary<long, long>();
        /// <summary>
        /// GuildDid, RoleDid KVP
        /// </summary>
        public Dictionary<long, long> ExplicitlyAuthorizedRoles { get; private set; } = new Dictionary<long, long>();
        public YuutaCommand Command { get; private set; }
        public Command DCommand { get; private set; }

        public DiscordCommandAL(Command dCommand)
        {
            DCommand = dCommand;
            using (var commandDAL = new CommandDAL())
            {
                Command = commandDAL.GetCommand(dCommand, true);
            }
            ExplicitlyAuthorizedRoles.AddRange(
                Command.RestrictionOverloads
                .Where(x => x.Authorize)
                .Select(x => new KeyValuePair<long, long>(x.GuildDid, x.RoleDid))
                .ToDictionary(pair => pair.Key, pair => pair.Value)
            );
            ExplicitlyDeniedRoles.AddRange(
                Command.RestrictionOverloads
                .Where(x => !x.Authorize)
                .Select(x => new KeyValuePair<long, long>(x.GuildDid, x.RoleDid))
                .ToDictionary(pair => pair.Key, pair => pair.Value)
            );
        }
        public DiscordCommandAL(CommandContext ctx) : this(ctx.Command) { }

    }
}