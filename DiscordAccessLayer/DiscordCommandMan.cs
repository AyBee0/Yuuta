using DataAccessLayer.DataAccess.Layers;
using DataAccessLayer.Models.CommandModels;
using DSharpPlus.CommandsNext;
using Generatsuru;
using System.Collections.Generic;
using System.Linq;

namespace DiscordMan
{
    public class DiscordCommandMan
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

        public DiscordCommandMan(Command dCommand)
        {
            DCommand = dCommand;
            var commandDAL = new CommandDAL();
            Command = commandDAL.GetByDObject(dCommand, true);
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
        public DiscordCommandMan(CommandContext ctx) : this(ctx.Command) { }

    }
}