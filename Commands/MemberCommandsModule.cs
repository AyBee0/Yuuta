using DiscordAccessLayer.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Commands
{
    [Restricted]
    public class MemberCommandsModule : YuutaCommandModule
    {
        [Command]
        public async Task Ping(CommandContext ctx)
        {
            var sw = Stopwatch.StartNew();
            var msg = await ctx.RespondAsync("Pong!\n\nResponse Time: *loading...*");
            sw.Stop();
            await msg.ModifyAsync($"Pong!\n\nResponse Time: {sw.ElapsedMilliseconds}ms");
        }
    }
}