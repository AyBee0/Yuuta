using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Firebase.Database;
using System;
using System.Threading.Tasks;

namespace Commands {

    public class StaffCommands : BaseCommandModule {

        [Command("say")]
        public async Task Say(CommandContext ctx, [RemainingText] string text) {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync(text);
        }

    }
}