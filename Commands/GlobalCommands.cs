using DSharpPlus.CommandsNext;
using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext.Attributes;

namespace Commands {

    public class GlobalCommands : BaseCommandModule {

        [Command("ping")]
        public async Task Ping(CommandContext ctx) {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var message = await ctx.RespondAsync($"Pong!\nResponse time: CALCULATING...");
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            await message.ModifyAsync($"Pong! \nResponse time: {elapsedMs}ms");
        }

    }

}
