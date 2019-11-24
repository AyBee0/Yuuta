using AuthorityHelpers;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tenor;

namespace Commands {

    public class GlobalCommands : BaseCommandModule {

        private TenorClient TenorClient;
        private Random Random;
        private readonly string[] EightBallAnswers =
            { "It is certain.", "It is decidedly so.", "Without a doubt.", "Yes - definitely.", "You may rely on it.", "As I see it, yes.",
            "Most likely.", "Outlook good.", "Yes.", "Signs point to yes.", "Reply hazy, try again.", "Ask again later.", "Better not tell you now.",
            "Cannot predict now.", "Concentrate and ask again.", "Don't count on it.", "My reply is no.", "My sources say no.", "Outlook not so good.",
            "Very doubtful." };
        private const ulong AbId = 247386254499381250;

        [Command("ping")]
        [Description("Ping the bot")]
        public async Task Ping(CommandContext ctx) {
            // This extension was written by me, it checks the database to see whether or not the user can use the bot in the specific channel.
            // This is useful for when people when to limit commands to a #bot-commands channel for example.
            // For more info, read CommandContextAuthorityExtensions.cs under AuthorityHelper.
            if (ctx.CanSendInChannel()) {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var message = await ctx.RespondAsync($"Pong!\nResponse time: CALCULATING...");
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                await message.ModifyAsync($"Pong!\nResponse time: {elapsedMs}ms");
            }
        }

        [Command("pat")]
        [Description("Pat someone. ")]
        public async Task Pat(CommandContext ctx, [RemainingText] [Description("Pat message.")] string patMessage) {
            if (ctx.CanSendInChannel()) {
                await ctx.Message.DeleteAsync();
                await ctx.TriggerTypingAsync();
                TenorClient = TenorClient ?? new TenorClient();
                var embedBuilder = new DiscordEmbedBuilder() {
                    Author = new DiscordEmbedBuilder.EmbedAuthor { IconUrl = ctx.Message.Author.AvatarUrl, Name = ctx.Message.Author.Username },
                    Color = new Optional<DiscordColor>(new DiscordColor("#EFCEB6")),
                    ImageUrl = TenorClient.GetRandomGif("anime pat"),
                };
                await ctx.RespondAsync($"*Pats {patMessage}*.", embed: embedBuilder);
            }
        }

        [Command("hug")]
        [Description("Hug someone. ")]
        public async Task Hug(CommandContext ctx, [RemainingText] [Description("Hug message. Mention the user you want to hug in here.")] string hugMessage) {
            if (ctx.CanSendInChannel()) {
                await ctx.Message.DeleteAsync();
                await ctx.TriggerTypingAsync();
                TenorClient = TenorClient ?? new TenorClient();
                var embedBuilder = new DiscordEmbedBuilder() {
                    Author = new DiscordEmbedBuilder.EmbedAuthor { IconUrl = ctx.Message.Author.AvatarUrl, Name = ctx.Message.Author.Username },
                    Color = new Optional<DiscordColor>(new DiscordColor("#EFCEB6")),
                    ImageUrl = TenorClient.GetRandomGif("anime hug"),
                };
                await ctx.RespondAsync($"*Hugs {hugMessage}*.", embed: embedBuilder);
            }
        }

        [Aliases("hit", "beat")]
        [Command("abuse")]
        [Description("Abuse someone.")]
        public async Task Abuse(CommandContext ctx, [RemainingText] [Description("Abuse message. Mention the user you want to abuse in here.")] string abuseMessage) {
            if (ctx.CanSendInChannel()) {
                await ctx.Message.DeleteAsync();
                await ctx.TriggerTypingAsync();
                TenorClient = TenorClient ?? new TenorClient();
                var embedBuilder = new DiscordEmbedBuilder() {
                    Author = new DiscordEmbedBuilder.EmbedAuthor { IconUrl = ctx.Message.Author.AvatarUrl, Name = ctx.Message.Author.Username },
                    Color = new Optional<DiscordColor>(new DiscordColor("#EFCEB6")),
                    ImageUrl = TenorClient.GetRandomGif("anime hit"),
                };
                await ctx.RespondAsync($"*Physically abuses {abuseMessage}*.", embed: embedBuilder);
            }
        }

        [Command("flip")]
        [Description("Flips a coin")]
        public async Task CoinFlip(CommandContext ctx) {
            if (ctx.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                Random = Random ?? new Random();
                await ctx.RespondAsync($"The coin has landed on `{(Random.Next() % 2 == 0 ? "Heads" : "Tails")}`!");
            }
        }

        [Command("8ball")]
        [Description("Asks the 8ball a question requiring truth.")]
        public async Task EightBall(CommandContext ctx, [Description("Question for the 8ball")] [RemainingText] string question) {
            if (ctx.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                Random = Random ?? new Random();
                await ctx.RespondAsync($"{ctx.Message.Author.Mention} {EightBallAnswers[Random.Next(0, EightBallAnswers.Length)]}");
            }
        }

        [Command("choose")]
        [Description("Chooses between a set of given choices. Choices are seperated by a |")]
        public async Task Choose(CommandContext ctx, [RemainingText] [Description("Choices seperated by a |")] string allChoices) {
            if (ctx.CanSendInChannel()) {
                var choices = allChoices.Split("|").Select(x => x.Trim()).ToArray();
                Random = Random ?? new Random();
                await ctx.RespondAsync($"I choose `{choices[Random.Next(0, choices.Length)]}`!");
            }
        }

        [Command("dance")]
        [Description("Dance.")]
        public async Task Dance(CommandContext ctx) {
            if (ctx.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                TenorClient = TenorClient ?? new TenorClient();
                var embedBuilder = new DiscordEmbedBuilder() {
                    Author = new DiscordEmbedBuilder.EmbedAuthor { IconUrl = ctx.Message.Author.AvatarUrl, Name = ctx.Message.Author.Username },
                    Color = new Optional<DiscordColor>(new DiscordColor("#EFCEB6")),
                    ImageUrl = TenorClient.GetRandomGif("anime dance"),
                };
                await ctx.RespondAsync(embed: embedBuilder);
            }
        }

        [Aliases("lazysearch")]
        [Command("lmgtfy")]
        [Description("For people who would rather bother you with their question that look it up themselves.")]
        public async Task Lmgtfy(CommandContext ctx, [RemainingText] [Description("Query to \"Give an answer to\".")] string query) {
            if (ctx.CanSendInChannel()) {
                await ctx.RespondAsync($"I looked for an answer and found this: <https://lmgtfy.com/?q={query.Replace(" ","%20")}>");
            }
        }

        [Aliases("lmgtfy+")]
        [Command("lmgtfyextra")]
        [Description("For people who would rather bother you with their question that look it up themselves. Gives a deeper explanation of the internet.")]
        public async Task LmgtfyExtra(CommandContext ctx, [RemainingText] [Description("Query to \"Give an answer to\".")] string query) {
            if (ctx.CanSendInChannel()) {
                await ctx.RespondAsync($"I looked for an answer and found this: <https://lmgtfy.com/?q={query.Replace(" ","%20")}&iie=1>");
            }
        }

        [Aliases("profilepicture", "getpfp", "getprofilepicture")]
        [Command("pfp")]
        [Description("Gets a user's pfp")]
        public async Task Pfp(CommandContext ctx, [Description("User to get their profile picture. If not passed, I'll get yours.")] DiscordUser user = null) {
            if (ctx.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                user = user ?? ctx.Message.Author;
                var embedBuilder = new DiscordEmbedBuilder() {
                    Color = new Optional<DiscordColor>(new DiscordColor("#EFCEB6")),
                    ImageUrl = user.AvatarUrl,
                };
                await ctx.RespondAsync(embed: embedBuilder);
            }
        }

        [Aliases("botinfo")]
        [Command("info")]
        [Description("Get's the bot's info!")]
        public async Task BotInfo(CommandContext ctx) {
            if (ctx.CanSendInChannel()) {
                var ab = await ctx.Client.GetUserAsync(AbId);
                await ctx.TriggerTypingAsync();
                var embedBuilder = new DiscordEmbedBuilder() {
                    Author = new DiscordEmbedBuilder.EmbedAuthor { IconUrl = ab.AvatarUrl, Name = "Yuutabot - Developed by Ab" },
                    Color = new Optional<DiscordColor>(new DiscordColor("#EFCEB6")),
                    Title = "I am Yuuta Bot - A bot developed  by Ab!",
                    Description = "I am a bot programmed by Ab#8582. It's pronounced Ay - Bee. Not ahb. Stop pronouncing it ahb. Thanks."
                };
                await ctx.RespondAsync(embed: embedBuilder);
            }
        }

        [Aliases("guildinfo")]
        [Command("serverinfo")]
        [Description("Gets the server's info.")]
        public async Task GetServerInfo(CommandContext ctx) {
            if (ctx.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                var owner = ctx.Guild.Owner;
                var embedBuilder = new DiscordEmbedBuilder() {
                    Author = new DiscordEmbedBuilder.EmbedAuthor { IconUrl = owner.AvatarUrl, Name = ctx.Guild.Name },
                    Color = new Optional<DiscordColor>(new DiscordColor("#EFCEB6")),
                    Title = ctx.Guild.Name,
                };
                var guild = ctx.Guild;
                embedBuilder.AddField("Server Create Date", ctx.Guild.CreationTimestamp.ToUniversalTime().ToString("ddd, dd MMM yyy HH:mm:ss UTC"),true);
                embedBuilder.AddField("Server Age", $"{Years(DateTime.UtcNow,ctx.Guild.CreationTimestamp.UtcDateTime)} Years",true);
                embedBuilder.AddField("Server Created By", owner.Mention, true);
                embedBuilder.AddField("Server Member Count", guild.MemberCount.ToString(), true);
                //embedBuilder.AddField("Server Roles", string.Join(", ", guild.Roles.Values.Select(x => $"`{x.Name}`")), true);
                await ctx.RespondAsync(embed: embedBuilder);
            }
        }

        [Command("say")]
        [Description("Make the bot say something.")]
        public async Task Say(CommandContext ctx, [RemainingText][Description("What to say")] string text) {
            if (ctx.CanSendInChannel()) {
                await ctx.Message.DeleteAsync();
                await ctx.TriggerTypingAsync();
                await ctx.RespondAsync($"{text.Replace("r", "w").Replace("l", "w").Replace("ww", "w").Replace("ov", "uv").Replace("er", "w")}");
            }
        }

        [Command("reallysay")]
        [Description("Make the bot say something. For real.")]
        public async Task ReallySay(CommandContext ctx, [RemainingText] [Description("What to say")] string text) {
            if (ctx.CanSendInChannel()) {
                await ctx.Message.DeleteAsync();
                await ctx.TriggerTypingAsync();
                await ctx.RespondAsync(text);
            }
        }

        [Command("roll")]
        [Description("Roll a dice.")]
        public async Task Roll(CommandContext ctx, [Description("Minimum. Default to 1")] int minimum = 1, [Description("Maximum. Defaults to 12.")] int maximum = 12) {
            if (ctx.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                Random = Random ?? new Random();
                await ctx.RespondAsync($"I rolled {Random.Next(minimum, maximum + 1)}");
            }
        }

        private static int   Years(DateTime start, DateTime end) {
            return Math.Abs((end.Year - start.Year - 1) +
                (((end.Month > start.Month) ||
                ((end.Month == start.Month) && (end.Day >= start.Day))) ? 1 : 0));
        }

    }

}
