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
        public Dictionary<ulong, ulong> ExplicitlyDeniedRoles { get; private set; } = new Dictionary<ulong, ulong>();
        /// <summary>
        /// GuildDid, RoleDid KVP
        /// </summary>
        public Dictionary<ulong, ulong> ExplicitlyAuthorizedRoles { get; private set; } = new Dictionary<ulong, ulong>();
        public YuutaCommand Command { get; private set; }
        public Command DCommand { get; private set; }

        public DiscordCommandMan(Command dCommand)
        {
            DCommand = dCommand;
            Command = CommandDAL.GetByDObject(dCommand, true);
            ExplicitlyAuthorizedRoles.AddRange(
                Command.RestrictionOverloads
                .Where(x => x.Authorize)
                .Select(x => new KeyValuePair<ulong, ulong>(x.GuildDid, x.RoleDid))
                .ToDictionary(pair => pair.Key, pair => pair.Value)
            );
            ExplicitlyDeniedRoles.AddRange(
                Command.RestrictionOverloads
                .Where(x => !x.Authorize)
                .Select(x => new KeyValuePair<ulong, ulong>(x.GuildDid, x.RoleDid))
                .ToDictionary(pair => pair.Key, pair => pair.Value)
            );
        }
        public DiscordCommandMan(CommandContext ctx) : this(ctx.Command) { }

    }
}