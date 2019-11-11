using DSharpPlus.CommandsNext;
using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext.Attributes;
using AuthorityHelpers;

namespace Commands {

    public class GlobalCommands : BaseCommandModule {

        [Command("ping")]
        [Description("Ping the bot to check its availability in the current channel")]
        public async Task Ping(CommandContext ctx) {
            // This extension was written by me, it checks the database to see whether or not the user can use the bot in the specific channel.
            // This is useful for when people when to limit commands to a #bot-commands channel for example.
            // For more info, read CommandContextAuthorityExtensions.cs under AuthorityHelper.
            if (ctx.CanSendInChannel()) {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var message = await ctx.RespondAsync($"Pong!\nResponse time: CALCULATING...");
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                await message.ModifyAsync($"Pong! \nResponse time: {elapsedMs}ms");
            }
        }

    }

}
