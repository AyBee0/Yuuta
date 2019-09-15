using Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json.Linq;
using RestSharp;
using RoleAssignment;
using ServerVariable;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Interactivity;

namespace YuutaBot {
    class Program {

        static DiscordClient discord;
        static CommandsNextExtension commands;
        static bool RunReactionAdd = true;
        static ChildQuery Child;
        static FirebaseClient FirebaseClient;
        static Random Random;
        //static Dictionary<ulong, List<CustomCommand>> GuildCommands;

        #region langauge
        private readonly static string[] FilteredWords = { "retard", "nigga", "nigger", "faggot" };
        #endregion

        public static void Main(string[] args) => MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();

        static async Task MainAsync(string[] args) {
            Console.WriteLine("Yuutabot Version 0.8");
            discord = new DiscordClient(new DiscordConfiguration {
                Token = "NTYxMjg4NDM4MjMwNDE3NDM4.XQ6dFw.V-i30a9HTeCAN5cqBPrZdw6fP6M",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug,
                HttpTimeout = Timeout.InfiniteTimeSpan //TODO - DELETE ----------------------------------------------------------------------------------------------
            });
            if (IsLinux) {
                RunReactionAdd = true;
                commands = discord.UseCommandsNext(new CommandsNextConfiguration {
                    StringPrefixes = new[] { "~", "yu!", "-" }
                });
            } else {
                RunReactionAdd = false;
                commands = discord.UseCommandsNext(new CommandsNextConfiguration {
                    StringPrefixes = new[] { "tt!" },
                    IgnoreExtraArguments = true,
                    CaseSensitive = false,
                    EnableDefaultHelp = true
                });
            }
            var interactivity = discord.UseInteractivity(new InteractivityConfiguration());
            commands.SetHelpFormatter<YuutaHelpFormatter>();
            commands.RegisterCommands<GlobalCommands>();
            commands.RegisterCommands<ModeratorCommands>();
            discord.Ready += Discord_Ready;
            discord.MessageReactionAdded += OnReactionAdded;
            discord.MessageReactionRemoved += OnReactionRemoved;
            discord.MessageCreated += OnMessageCreated;
            discord.GuildAvailable += OnGuildAvailable;
            discord.MessageDeleted += OnMessageDeleted;
            discord.GuildMemberAdded += OnMemberAdded;
            FirebaseClient = new FirebaseClient("https://the-beacon-team-battles.firebaseio.com/");
            Child = FirebaseClient.Child("Commands");
            await discord.ConnectAsync();
            Random = new Random();
            await Task.Delay(-1);
        }

        private static async Task OnMemberAdded(GuildMemberAddEventArgs e) {
            var guildId = e.Guild.Id;
            var welcomeEnabled = await FirebaseClient.Child("Info").Child(guildId.ToString()).Child("Welcome").Child("Enabled").OnceSingleAsync<bool?>();
            if (welcomeEnabled.HasValue && welcomeEnabled.Value) {
                var welcomeChannelId = await FirebaseClient.Child("Info").Child(guildId.ToString()).Child("Welcome").Child("Channel").OnceSingleAsync<long>();
                var welcomeChannel = e.Guild.GetChannel((ulong)welcomeChannelId);
                var welcomeMessage = await FirebaseClient.Child("Info").Child(guildId.ToString()).Child("Welcome").Child("Message").OnceSingleAsync<string>();
                welcomeMessage = welcomeMessage.Replace("{MENTION}", e.Member.Mention).Replace("{SERVER}", e.Guild.Name).Replace("{MEMBER}", e.Member.DisplayName);
                await welcomeChannel.SendMessageAsync(welcomeMessage);
            } else {
                return;
            }
        }

        //private static async void CheckIfRecordingIsEnabled(FirebaseClient firebaseClient) {
        //    while (true) {
        //        try {
        //            Console.WriteLine("Checking to see if recording should be initialized...");
        //            RecordingStarted = await firebaseClient.Child("Info").Child("310279910264406017").Child("RecordingStarted").OnceSingleAsync<bool>();
        //            Console.WriteLine($"Recording Initialized: {RecordingStarted}");
        //            Thread.Sleep(TimeSpan.FromSeconds(30));
        //        } catch (Exception e) {
        //            Console.WriteLine(e.StackTrace);
        //            continue;
        //        }
        //    }
        //}

        private async static Task OnGuildAvailable(GuildCreateEventArgs e) {
            if (RunReactionAdd) {
                var roleChannel = await e.Client.GetChannelAsync(ServerVariables.TheBeaconRoleChannelId);
                var harmonyGuild = await e.Client.GetGuildAsync(453487691216977922); // harmony guild
                var gameMessage = await roleChannel.GetMessageAsync(ServerVariables.TheBeaconGameRoleReactMessageId);
                //var gameMessage2 = await roleChannel.GetMessageAsync(ServerVariables.TheBeaconGameRoleReactMessageId2);
                var otherMessage = await roleChannel.GetMessageAsync(ServerVariables.TheBeaconOtherRoleReactMessageId);
                var platformsMessage = await roleChannel.GetMessageAsync(ServerVariables.TheBeaconPlatformMessageId);
                if (gameMessage == null | otherMessage == null) {
                    return;
                }
                #region Game Roles
                //APEX
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.Apex]);
                //CSGO
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.CSGO]);
                //DAUNTLESS
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.Dauntless]);
                //DESTINY
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.Destiny]);
                //FORTNITE
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.Fortnite]);
                //LEAGUE
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.League]);
                //MTG
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.Magic]);
                //MARIO KART
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.MarioKart]);
                //MINECRAFT
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.Minecraft]);
                //OVERWATCH
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.Overwatch]);
                //PUBG
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.Pubg]);
                //RAINBOW
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.Rainbow]);
                //ROLLER
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.RollerChampions]);
                //SMASH
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.Smash]);
                //SMM2
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.SMM2]);
                //SPLATOON
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.Splatoon]);
                //WARFRAME
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.Warframe]);
                //PALADINS
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.Paladins]);
                #endregion
                #region Platform
                //PC
                await platformsMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Platforms.PC]);
                //XBOX
                await platformsMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Platforms.Xbox]);
                //PS4
                await platformsMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Platforms.PS4]);
                #endregion
                #region Other Roles
                //EVENT
                await otherMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Other.Event]);
                //MOVIE NIGHT
                await otherMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Other.Movie_Night]);
                //EU
                await otherMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Other.EU]);
                //NA
                await otherMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Other.NA]);
                //AS
                await otherMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Other.AS]);
                //OCE
                await otherMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Other.OCE]);
                //LFG EU
                await otherMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Other.LFG_EU]);
                //LFG NA
                await otherMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Other.LFG_NA]);
                //LFG AS
                await otherMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Other.LFG_AS]);
                //LFG OCE
                await otherMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Other.LFG_OCE]);
                //MEMES
                await otherMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Other.Memes]);
                //FREE GAME
                await otherMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Other.Free_Game]);
                #endregion
            }

        }

        private async static Task OnMessageCreated(MessageCreateEventArgs e) {
            if (e.Guild == null) {
                return;
            }
            if (ServerVariables.FilteredGuilds.Contains(e.Guild.Id)) {
                #region Word Censoring
                foreach (var filteredWord in FilteredWords) {
                    if (Regex.IsMatch(e.Message.Content, @"\b" + filteredWord + @"\b")) {
                        await e.Message.DeleteAsync("Offensive word filter");
                        var member = await e.Guild.GetMemberAsync(e.Message.Author.Id);
                        await member.SendMessageAsync($"Your message contains the filtered word `{filteredWord}` and has thus been deleted.");
                        break;
                    }
                }
                #endregion
                #region Bot prevention
                if (Regex.IsMatch(e.Message.Content, @"([a - zA - Z0 - 9] +://)?([a-zA-Z0-9_]+:[a-zA-Z0-9_]+@)?([a-zA-Z0-9.-]+\\.[A-Za-z]{2,4})(:[0-9]+)?(/.*)?") | e.Message.Attachments.Count < 1) {
                    var member = await e.Guild.GetMemberAsync(e.Author.Id);
                    var joinDate = member.JoinedAt.ToUniversalTime();
                    var currentDate = DateTime.UtcNow;
                    var difference = (currentDate - joinDate).TotalMinutes;
                    if (difference < 10) {
                        await e.Message.DeleteAsync("You must be in the server for 10 minutes before sending an image or url");
                        await member.SendMessageAsync("You must be in the server for 10 minutes before sending an image or url.");
                    }
                }
                #endregion
            }
            //if (RecordingStarted && e.Guild.Id == ServerVariables.TheBeaconId) {
            //    var member = await e.Guild.GetMemberAsync(e.Author.Id);
            //    if (member.Roles.Contains(AbRole)) {
            //        var value = await Child.Child("Ab").OnceSingleAsync<int>();
            //        //await AbChild.PatchAsync($"\"Team Ab\": {++value}");
            //        var jsonObject = new JObject {
            //            ["Ab"] = ++value
            //        };
            //        await Child.PatchAsync(jsonObject);
            //    } else if (member.Roles.Contains(NeutralRole)) {
            //        var value = await Child.Child("Neutral").OnceSingleAsync<int>();
            //        //await NeutralChild.PatchAsync($"\"Team Neutral\": {++value}");
            //        var jsonObject = new JObject {
            //            ["Neutral"] = ++value
            //        };
            //        await Child.PatchAsync(jsonObject);
            //    } else if (member.Roles.Contains(BargotRole)) {
            //        var value = await Child.Child("Bargot").OnceSingleAsync<int>();
            //        //await BargotChild.PatchAsync($"\"Team Bargot\": {++value}");
            //        var jsonObject = new JObject {
            //            ["Bargot"] = ++value
            //        };
            //        await Child.PatchAsync(jsonObject);
            //    }
            //}
            if (e.Message.Content.ToLower().Contains("play despacito") | (e.Message.Content.ToLower().Contains("alexa") & e.Message.Content.ToLower().Contains("despacito"))) {
                var member = await e.Guild.GetMemberAsync(e.Author.Id);
                if (ServerVariables.CanSendInChannel(member, e.Channel.Id)) {
                    await e.Message.RespondAsync("1- I'm not goddamn Alexa. Can you stop acting like I am? It's Yuuta. Get it right ffs. Do I have your social security number? Is a Lizard watching you through my eyes right now? Christ.\n2- no.");
                }
            } else if (e.Message.Content.ToLower().Equals("creeper")) {
                await e.Message.RespondAsync("1. No.\n2. No.\n3. Especially: __**No**__.");
            } else if (e.Message.Content.ToLower().Contains("chuunibyou") || e.Message.Content.ToLower().Contains("chunibyou")) {
                var chance = Random.Next(0, 3);
                if (chance == 0) {
                    await e.Message.RespondAsync("Chuunibyou is love chuunibyou is life.");
                }
            } else if (e.Message.Author.Id == 252810598721519616 && e.Message.Content.ToLower().Contains("anime")) {
                var chance = Random.Next(0, 3);
                if (chance == 0) {
                    await e.Message.RespondAsync("Don't worry warrior, I'll say the line for you.\nAnime? Banime :100: :ok_hand: :laughing:");
                }
            } else if (e.Message.Content.ToLower().Replace(" ", "").Equals("🅰🅱")) {
                var chance = Random.Next(0, 2);
                if (chance == 0) {
                    await e.Message.RespondAsync($"Ab!");
                }
            }

        }

        private async static Task Discord_Ready(ReadyEventArgs e) {
            await e.Client.UpdateStatusAsync(new DiscordActivity("Do I overrate Chuunibyou? No. Stick your tongue into a power outlet.", ActivityType.Playing));

        }

        static ulong tempId = 621778306534080521;

        private async static Task OnReactionAdded(MessageReactionAddEventArgs e) {
            if (e.User.IsBot) {
                return;
            }
            var messageId = e.Message.Id;
            if (messageId == ServerVariables.TheBeaconGameRoleReactMessageId | messageId == ServerVariables.TheBeaconOtherRoleReactMessageId | messageId == ServerVariables.TheBeaconPlatformMessageId | messageId == tempId) {
                var member = await e.Channel.Guild.GetMemberAsync(e.User.Id);
                if (e.Message.Id == ServerVariables.TheBeaconTempReactMessageId & member.Roles.Any(x => x.Id == 607203125883043843 | x.Id == 607204919971151882 | x.Id == 607205082525597706)) {
                    await e.Message.DeleteReactionAsync(e.Emoji, e.User);
                    await member.SendMessageAsync("You've already chosen a side! Stay loyal! By rule #10 of the UN's Great War agreement, once you choose a team, you're stuck with it. Failing to do so is a war crime.");
                    return;
                }
                var roles = GameRole.ParseRole(e.Emoji.Id);
                if (roles == null)
                    return;
                foreach (var role in roles) {
                    var reactionRole = e.Channel.Guild.GetRole(role.RoleId);
                    var recepient = await e.Channel.Guild.GetMemberAsync(e.User.Id);
                    if (e.Message.Id == ServerVariables.TheBeaconTempReactMessageId) {
                        await e.Message.DeleteReactionAsync(e.Emoji, e.User);
                    }
                    if (!recepient.Roles.Contains(reactionRole)) {
                        await recepient.GrantRoleAsync(reactionRole);
                        await recepient.SendMessageAsync($"I have granted you the {role.RoleName} role in `{e.Channel.Guild.Name}`!");
                    }
                }
            }
        }

        private async static Task OnReactionRemoved(MessageReactionRemoveEventArgs e) {
            if (e.User.IsBot) {
                return;
            }
            var messageId = e.Message.Id;
            if (messageId == ServerVariables.TheBeaconGameRoleReactMessageId | messageId == ServerVariables.TheBeaconOtherRoleReactMessageId | messageId == ServerVariables.TheBeaconPlatformMessageId | messageId == tempId) {
                var roles = GameRole.ParseRole(e.Emoji.Id);
                if (roles == null || roles.Count < 1)
                    return;
                var role = roles[0];
                var reactionRole = e.Channel.Guild.GetRole(role.RoleId);
                var recepient = await e.Channel.Guild.GetMemberAsync(e.User.Id);
                await recepient.RevokeRoleAsync(reactionRole);
                await recepient.SendMessageAsync($"I have revoked your {role.RoleName} role in `{e.Channel.Guild.Name}`!");
                //foreach (var role in roles) {
                //    if (index >= 1) {
                //        if (recepient.Roles.Contains(reactionRole)) {
                //            var interactivity = e.Client.GetInteractivity();
                //            var res = await recepient.SendMessageAsync($"Would you also like to have your {role.RoleName} role revoked?");
                //            var yesEmoji = DiscordEmoji.FromName(e.Client,":white_check_mark:");
                //            var noEmoji = DiscordEmoji.FromName(e.Client,":x:");
                //            await res.CreateReactionAsync(yesEmoji);
                //            await res.CreateReactionAsync(noEmoji);
                //            var em = await res.WaitForReactionAsync(recepient, TimeSpan.FromSeconds(20));
                //            if (em.Result != null) {
                //                if (em.Result.Emoji.Equals(yesEmoji)) {
                //                    await recepient.RevokeRoleAsync(recepient.Guild.GetRole(role.RoleId), "User request");
                //                    await recepient.SendMessageAsync($"Alright, I have revoked your {role.RoleName} role.");
                //                }
                //                else {
                //                    await recepient.SendMessageAsync($"Alright, I won't revoke your {role.RoleName} role.");
                //                }
                //            }
                //            else {
                //                await recepient.SendMessageAsync("k");
                //            }
                //        }
                //    }
                //    else if (recepient.Roles.Contains(reactionRole)) {
                //        await recepient.RevokeRoleAsync(reactionRole);
                //        await recepient.SendMessageAsync($"I have revoked your {role.RoleName} role in `{e.Channel.Guild.Name}`!");
                //    }
                //    index++;
                //}
            }
        }

        private async static Task OnMessageDeleted(MessageDeleteEventArgs e) {
            if (e.Guild.Id == ServerVariables.TheBeaconId) {
                try {
                    var embedBuilder = new DiscordEmbedBuilder();
                    var channel = e.Guild.GetChannel(381830470423412750);
                    embedBuilder.WithAuthor(e.Message.Author.Username, null, e.Message.Author.AvatarUrl);
                    //embedBuilder.WithTitle($"Message sent by {e.Message.Author.Mention} deleted in {e.Channel.Mention}");
                    //embedBuilder.WithDescription($"Message: {e.Message.Content}");
                    embedBuilder.WithDescription($"**Message sent by {e.Message.Author.Mention} deleted in {e.Channel.Mention}**");
                    embedBuilder.AddField("Message", e.Message.Content);
                    await channel.SendMessageAsync("", false, embedBuilder.Build());
                } catch (Exception a) {

                    Console.WriteLine("Exception on delete event");
                }
            }
        }

        private static bool IsLinux
        {
            get {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

    }

}