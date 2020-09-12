using Commands.YuutaTasks;
using DiscordMan.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Commands.Modules
{
    [StaffOnly]
    public class StaffCommandsModule : YuutaCommandModule
    {
        [Command]
        [Description("Create a new reaction message.")]
        public async Task NewReactionMessage(CommandContext ctx)
        {

        }

        [Command]
        [Description("Create a new event")]
        public async Task NewEvent(CommandContext ctx)
        {
            try
            {
                await Tasks.NewEventAsync(ctx);
            }
            catch (Exception)
            {
                await ctx.RespondAsync($":x: Sorry, something went wrong.");
                throw;
            }
        }

    }
}
