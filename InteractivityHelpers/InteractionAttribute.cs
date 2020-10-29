using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
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
                var tuple = (ctx.User.Id, ctx.Channel.Id);
                if (!UserInteractiveChannels.Contains(tuple))
                {
                    UserInteractiveChannels.Add(tuple);
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }
    }
}
