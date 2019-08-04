using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using ServerVariable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Commands {
    public class GlobalCommands : BaseCommandModule {

        private static Random random;

        [Description("Ping the bot.\n")]
        [Command("ping")]
        public async Task Ping(CommandContext ctx) {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.CanSendInChannel()) {

                await ctx.TriggerTypingAsync();
                await ctx.Channel.SendMessageAsync("Pong!");
            }
        }

        [Description("Pat someone.")]
        [Command("pat")]
        public async Task Pat(CommandContext ctx, [Description("@User to pat")]DiscordUser user, [Description("(Optional) Pat message.")] string content = "") {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                if (random == null) {
                    random = new Random();
                }
                // wrap it into an embed
                int index = random.Next(1, 8);
                using (FileStream fs = File.OpenRead(Environment.CurrentDirectory + $"\\Pats\\{index}.gif")) {
                    await ctx.Message.DeleteAsync();
                    await ctx.RespondWithFileAsync(fs, $"*pats* {user.Mention} {content}");
                }
            }
        }

        [Description("Hug someone.")]
        [Command("hug")]
        public async Task Hug(CommandContext ctx, [Description("@User to hug")]DiscordUser user, [Description("(Optional) Hug message.")] string content = "") {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                if (random == null) {
                    random = new Random();
                }
                // wrap it into an embed
                int index = random.Next(1, 8);
                using (FileStream fs = File.OpenRead(Environment.CurrentDirectory + $"\\Hugs\\{index}.gif")) {
                    await ctx.Message.DeleteAsync();
                    await ctx.RespondWithFileAsync(fs, $"*hugs* {user.Mention} {content}");
                }
            }
        }


        [Description("Abuse someone.")]
        [Aliases("punch","hit","yeet")]
        [Command("abuse")]
        public async Task Abuse(CommandContext ctx, [Description("@User to abuse")]DiscordUser user, [Description("(Optional) Abuse message.")] string content = "") {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                if (random == null) {
                    random = new Random();
                }
                // wrap it into an embed
                int index = random.Next(1, 10);
                string path = Environment.CurrentDirectory + $"\\other\\hits\\{index}.gif";
                FileStream fs = File.OpenRead(path);
                await ctx.Message.DeleteAsync();
                await ctx.RespondWithFileAsync(fs, $"*physically abuses* {user.Mention} {content}");
            }
        }

        [Description("Dance.")]
        [Command("dance")]
        public async Task Dance(CommandContext ctx, [Description("(Optional) Dance message.")] string content = "") {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                string path = Environment.CurrentDirectory + $"\\other\\dance.gif";
                FileStream fs = File.OpenRead(path);
                await ctx.Message.DeleteAsync();
                await ctx.RespondWithFileAsync(fs, content);
            }
        }

    }
}