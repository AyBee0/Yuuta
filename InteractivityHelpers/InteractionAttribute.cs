using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractivityHelpers
{
    public class InteractionAttribute : CheckBaseAttribute
    {
        internal static List<(ulong User, ulong Channel)> UserInteractiveChannels { get; set; }
            = new List<(ulong, ulong)>();

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return Task.Run(() =>
            {
                Log.Information
                ("Checking for already existing user interactions for:\n" +
                $"User: {ctx.User.Username}#{ctx.User.Discriminator} ID {ctx.User.Id}\n" +
                $"Channel: {ctx.Channel.Name} ID {ctx.Channel.Id}\n" +
                $"Guild: {ctx.Guild.Name} ID {ctx.Guild.Id}");
                var tuple = (ctx.User.Id, ctx.Channel.Id);
                if (!UserInteractiveChannels.Contains(tuple))
                {
                    Log.Information("User does not have interaction in this channel.");
                    UserInteractiveChannels.Add(tuple);
                    return true;
                }
                else
                {
                    Log.Warning("User already has interaction in this channel. Ignoring...");
                    return false;
                }
            });
        }
    }
}
