using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using ServerVariable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Interactivity;
using System.Linq;
using Firebase.Database;
using Firebase.Database.Query;

namespace Commands {
    public class GlobalCommands : BaseCommandModule {

        private static Random random;
        string[] Fortunes = { "It is certain.", "It is decidedly so.", "Without a doubt.", "Yes - definitely.", "You may rely on it.", "As I see it ,  yes.", "Most likely.", "Outlook good.", "Yes.", "Signs point to yes.", "Reply hazy ,  try again.", "Ask again later.", "Better not tell you now.", "Cannot predict now.", "Concentrate and ask again.", "Don't count on it.", "My reply is no.", "My sources say no.", "Outlook not so good.", "Very doubtful." };

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
        public async Task Pat(CommandContext ctx, [Description("@User to pat")]DiscordUser user, [Description("(Optional) Pat message.")] [RemainingText] string content = "") {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                if (random == null) {
                    random = new Random();
                }
                // wrap it into an embed
                int index = random.Next(1, 8);
                string path = Environment.CurrentDirectory + (IsLinux ? $"/Pats/{index}.gif" : $"\\Pats\\{index}.gif");
                using (FileStream fs = File.OpenRead(path)) {
                    await ctx.Message.DeleteAsync();
                    await ctx.RespondWithFileAsync(fs, $"*pats* {user.Mention} {content}");
                }
            }
        }

        [Description("Hug someone.")]
        [Command("hug")]
        public async Task Hug(CommandContext ctx, [Description("@User to hug")]DiscordUser user, [Description("(Optional) Hug message.")] [RemainingText] string content = "") {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                if (random == null) {
                    random = new Random();
                }
                // wrap it into an embed
                int index = random.Next(1, 8);
                string path = Environment.CurrentDirectory + (IsLinux ? $"/Hugs/{index}.gif" : $"\\Hugs\\{index}.gif");
                using (FileStream fs = File.OpenRead(path)) {
                    await ctx.Message.DeleteAsync();
                    await ctx.RespondWithFileAsync(fs, $"*hugs* {user.Mention} {content}");
                }
            }
        }


        [Description("Abuse someone.")]
        [Aliases("punch", "hit", "yeet")]
        [Command("abuse")]
        public async Task Abuse(CommandContext ctx, [Description("@User to abuse")]DiscordUser user, [Description("(Optional) Abuse message.")] [RemainingText] string content = "") {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                if (random == null) {
                    random = new Random();
                }
                // wrap it into an embed
                int index = random.Next(1, 10);
                //string path = Environment.CurrentDirectory + $"\\other\\hits\\{index}.gif";
                string path = Environment.CurrentDirectory + (IsLinux ? $"/other/hits/{index}.gif" : $"\\other\\hits\\{index}.gif");
                FileStream fs = File.OpenRead(path);
                await ctx.Message.DeleteAsync();
                await ctx.RespondWithFileAsync(fs, $"*physically abuses* {user.Mention} {content}");
            }
        }

        [Description("Dance.")]
        [Command("dance")]
        public async Task Dance(CommandContext ctx, [Description("(Optional) Dance message.")] [RemainingText] string content = "") {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                string path = Environment.CurrentDirectory + (IsLinux ? "/other/dance.gif" : "\\other\\dance.gif");
                Console.WriteLine(path);
                FileStream fs = File.OpenRead(path);
                await ctx.Message.DeleteAsync();
                await ctx.RespondWithFileAsync(fs, content);
            }
        }

        [Description("Get a user's profile picture")]
        [Command("pfp")]
        public async Task PFP(CommandContext ctx, [Description("Profle Picture To Retrieve")] DiscordMember member) {
            await ctx.TriggerTypingAsync();
            member = member ?? ctx.Member;
            await ctx.Message.DeleteAsync();
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (!serverVariables.CanSendInChannel()) {
                return;
            }
            await ctx.RespondAsync(member.AvatarUrl);
        }

        [Description("Say.")]
        [Command("say")]
        public async Task Say(CommandContext ctx, [Description("wut say")] [RemainingText] string text) {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (!serverVariables.CanSendInChannel()) {
                return;
            }
            await ctx.Message.DeleteAsync();
            string vowels = "aeiou";
            string newText = text;
            int index = 0;
            foreach (char letter in text) {
                if ((char.ToLower(letter).Equals('n') | (char.ToLower(letter).Equals('m'))) & (index + 1 < text.Length && vowels.Contains(text[index + 1]))) {
                    newText = newText.Insert(index + 1, "y");
                }
                if (char.ToLower(letter).Equals('p') & (index + 1 < text.Length && vowels.Contains(text[index + 1]))) {
                    newText = newText.Insert(index + 1, "w");
                }
                index++;
            }
            newText = newText.Replace("r", "w").Replace("R", "W").Replace("v", "f").Replace("V", "F").Replace("l", "w").Replace("L", "W") + " owo";
            Regex.Replace(newText, "th", "d", RegexOptions.IgnoreCase);
            await ctx.RespondAsync(newText);
        }

        [Hidden]
        [Description("reallysay")]
        [Command("reallysay")]
        public async Task ReallySay(CommandContext ctx, [Description("What to say")] [RemainingText] string text) {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (!serverVariables.CanSendInChannel()) {
                return;
            }
            await ctx.Message.DeleteAsync();
            await ctx.RespondAsync(text);
        }

        [Description("Roll a dice")]
        [Command("roll")]
        public async Task Roll(CommandContext ctx, [Description("Minimum Roll")] int min = 0, [Description("Maximum Roll")] int max = 0) {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (!serverVariables.CanSendInChannel()) {
                return;
            }
            if (min != 0 & max == 0) {
                await ctx.Message.DeleteAsync();
                await ctx.RespondAsync("If you provide a `min`, please provide a `max`.\nE.g: `~roll 1 50`");
                return;
            } else if ((min != 0 & max != 0) && max <= min) {
                await ctx.Message.DeleteAsync();
                await ctx.RespondAsync("Minimum value cannot be greater than or equal to the maximum value\nE.g: `~roll 1 50`");
                return;
            }
            if (random == null) {
                random = new Random();
            }
            var value = random.Next(min == 0 ? 1 : min, max == 0 ? 12 : max);
            await ctx.RespondAsync($"{ctx.Member.Mention} rolled {value}");
        }

        [Command("8ball")]
        [Description("Let the 8-Ball decide your decisions for you.")]
        public async Task Ball8(CommandContext ctx, [Description("Fortunte to tell")] [RemainingText] string text) {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (!serverVariables.CanSendInChannel()) {
                return;
            }
            await ctx.Message.DeleteAsync();
            if (random == null) {
                random = new Random();
            }
            var index = random.Next(0, Fortunes.Length);
            await ctx.RespondAsync($"{ctx.Member.Mention}`\n{text}`\n\n**{Fortunes[index]}**");
        }

        [Command("choose")]
        [Description("Randomly choose between items.")]
        public async Task Choose(CommandContext ctx, [Description("Your items to randomly choose from. Seperate them with |")] [RemainingText] string itemsUnseperated) {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (!serverVariables.CanSendInChannel()) {
                return;
            }
            var items = itemsUnseperated.Split("|");
            if (random == null) {
                random = new Random();
            }
            var size = items.Length;
            var index = random.Next(0, items.Length);
            await ctx.RespondAsync($"I choose `{items[index]}`!");
        }

        [Aliases("coin")]
        [Command("flip")]
        [Description("Flip a coin.")]
        public async Task Flip(CommandContext ctx) {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                random = random ?? new Random();
                var selection = (random.Next(0, 2) == 0) ? "Heads" : "Tails";
                await ctx.RespondAsync($"I flipped.... `{selection}`!");
            }
        }

        [Aliases("lazygoogle")]
        [Command("lmgtfy")]
        [Description("LMGTFY for people who are too lazy to google and would rather bother you with their question.")]
        public async Task Lmgtfy(CommandContext ctx, [Description("Question to LMGTFY")] [RemainingText] string query) {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                await ctx.RespondAsync($"Easy, here you go: <https://lmgtfy.com/?q={query.Replace(" ", "+")}>");
            }
        }

        [Description("Get the roles of a user, or yourself")]
        [Command("roles")]
        public async Task GetRoles(CommandContext ctx, [Description("User to display roles for. Leave empty to return your own.")] DiscordMember member = null) {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                member = member ?? ctx.Member;
                StringBuilder builder = new StringBuilder($"{member.Nickname}'s roles are: ");
                foreach (var role in member.Roles) {
                    if (!role.Name.StartsWith("━")) {
                        builder.Append($"`{role.Name}` ");
                    }
                }
                await ctx.RespondAsync(builder.ToString());
            }
        }

        [Aliases("facts")]
        [Description("Facts!")]
        [Command("fact")]
        public async Task Fact(CommandContext ctx, [Description("What type of fact? To see available types, do `~fact help`. Leave empty for general")] string type = null) {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.CanSendInChannel()) {
                await ctx.TriggerTypingAsync();
                type = type ?? "General";
                random = random ?? new Random();
                var factsClass = new Facts();
                string[] factArray = null;
                try {
                    factArray = (string[])factsClass.GetType().GetField(ToUpperFirstLetter(type)).GetValue(factsClass);
                } catch (Exception) {
                    Console.WriteLine($"Couldn't find fact array with name {type}");
                }
                if (factArray != null) {
                    await ctx.RespondAsync(factArray[random.Next(0, factArray.Length)]);
                } else {
                    StringBuilder builder = new StringBuilder("Our available facts are: `");
                    var i = 0;
                    foreach (var item in factsClass.GetType().GetFields()) {
                        if (i == factsClass.GetType().GetFields().Length - 1) {
                            builder.Append($"{item.Name}`");
                        } else {
                            builder.Append($"{item.Name}, ");
                        }
                        i++;
                    }
                    await ctx.RespondAsync(builder.ToString());
                }
            }
        }

        [Command("podcast")]
        public async Task Podcast(CommandContext ctx) {
            if (ctx.Guild.Id == ServerVariables.TheBeaconId) {
                ServerVariables variables = new ServerVariables(ctx);
                if (variables.CanSendInChannel()) {
                    await ctx.Message.DeleteAsync();
                    var firebaseClient = new FirebaseClient("https://the-beacon-team-battles.firebaseio.com/");
                    var LatestPodcast = await firebaseClient.Child("info").Child($"{ctx.Guild.Id}").Child("LatestPodcast").OnceSingleAsync<string>();
                    if (string.IsNullOrWhiteSpace(LatestPodcast)) {
                        await ctx.RespondAsync($"This server doesn't have a podcast.");

                    } else {
                        await ctx.RespondAsync($"Latest Podcast: -> {LatestPodcast}\nThe Beacon's Youtube: -> <https://www.youtube.com/channel/UCFW1hIgpFxsfzM2GxMyIaiw>");
                    }
                }
            }
        }

        //[Command("giball")]
        //public async Task GibAll(CommandContext ctx) {
        //    var members = await ctx.Guild.GetAllMembersAsync();
        //    var gamesRole = ctx.Guild.GetRole(416905411321397249);
        //    var specialsRole = ctx.Guild.GetRole(416906284932005889);
        //    foreach (var member in members) {
        //        if (!member.Roles.Contains(gamesRole)) {
        //            await member.GrantRoleAsync(gamesRole);
        //        }
        //        if (!member.Roles.Contains(specialsRole)) {
        //            await member.GrantRoleAsync(specialsRole);
        //        }
        //    }
        //}

        [Command("info")]
        public async Task Info(CommandContext ctx) {
            ServerVariables variables = new ServerVariables(ctx);
            if (variables.CanSendInChannel()) {
                await ctx.Message.DeleteAsync();
                await ctx.TriggerTypingAsync();
                var builder = new DiscordEmbedBuilder {
                    Title = "I am Yuuta Bot",
                    Description = "I am a bot programmed by Ab#8582. It's pronounced Ay - Bee. Not ahb. Stop pronouncing it ahb. Thanks.\n\n **For a list of commands, do `~help`**",
                    Color = new DiscordColor("#EFCEB6"),
                    ThumbnailUrl = "https://i.pinimg.com/236x/a4/9c/a3/a49ca31e338b3fab0659e3e3fa92517f--pictures-manga.jpg",
                };
                builder.WithAuthor("Yuuta Bot - Developed by Ab", null, "https://i.imgur.com/YKDFzsB.png");
                await ctx.RespondAsync(embed: builder.Build());
            }
        }

        [Command("serverinfo")]
        public async Task ServerInfo(CommandContext ctx) {
            ServerVariables variables = new ServerVariables(ctx);
            if (variables.CanSendInChannel()) {
                await ctx.Message.DeleteAsync();
                await ctx.TriggerTypingAsync();
                var firebaseClient = new FirebaseClient("https://the-beacon-team-battles.firebaseio.com/");
                var description = await firebaseClient.Child("info").Child($"{ctx.Guild.Id}").Child("ServerDescription").OnceSingleAsync<string>() ?? "No description provided";
                var inviteLink = await firebaseClient.Child("info").Child($"{ctx.Guild.Id}").Child("InviteLink").OnceSingleAsync<string>() ?? "No invite link provided.";
                var guild = ctx.Guild;
                var builder = new DiscordEmbedBuilder {
                    Title = guild.Name,
                    Description = description,
                    Color = new DiscordColor("#EFCEB6"),
                    ThumbnailUrl = guild.IconUrl,
                };
                builder.WithAuthor("Yuuta Bot - Developed by Ab", null, "https://i.imgur.com/YKDFzsB.png");
                builder.AddField("Server Name", guild.Name);
                builder.AddField("Server Created at", guild.JoinedAt.ToString("dd-MMM-yyyy"), true);
                builder.AddField("Server Owned by", guild.Owner.Mention, true);
                builder.AddField("Member Count", guild.MemberCount.ToString(), true);
                builder.AddField("You Joined At", ctx.Member.JoinedAt.ToString("dd-MMM-yyyy"), true);
                builder.AddField("Voice Region", guild.VoiceRegion.Name, true);
                //var strnBuilder = new StringBuilder("Roles: ");
                //var index = 0;
                //foreach (var role in guild.Roles.Values) {
                //    if (index + 1 == guild.Roles.Values.Count()) {
                //        strnBuilder.Append($"`{role.Name}`");
                //    } else {
                //        strnBuilder.Append($"`{role.Name}`, ");
                //    }
                //    index++;
                //}
                builder.AddField("Roles", guild.Roles.Count.ToString(),true);
                builder.AddField("Invite Link", inviteLink);
                await ctx.RespondAsync(embed: builder.Build());
            }
        }

        static bool IsLinux
        {
            get {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        static string ToUpperFirstLetter(string source) {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            // return the array made of the new char array
            return new string(letters);
        }
    }
}