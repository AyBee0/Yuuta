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
using System.Text;

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
        private readonly static string[] FilteredWords = { "abby" };
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
            discord.MessageUpdated += OnMessageUpdated;
            FirebaseClient = new FirebaseClient("https://the-beacon-team-battles.firebaseio.com/");
            Child = FirebaseClient.Child("Commands");
            await discord.ConnectAsync();
            Random = new Random();
            await Task.Delay(-1);
        }

        private static async Task OnMessageUpdated(MessageUpdateEventArgs e) {
            if (e.Message == null) {
                return;
            }
            if (ServerVariables.FilteredGuilds.Contains(e.Guild.Id)) {
                #region Word Censoring
                if (Regex.IsMatch(e.Message.Content.ToLower(), @"\b" + "abby" + @"\b")) {
                    await e.Channel.TriggerTypingAsync();
                    await e.Message.RespondAsync($"Ab.");
                    await e.Message.DeleteAsync("Ab ffs");
                } else {
                    foreach (var filteredWord in FilteredWords) {
                        if (Regex.IsMatch(e.Message.Content.ToLower(), @"\b" + filteredWord.ToLower() + @"\b")) {
                            await e.Message.DeleteAsync("Offensive word filter");
                            var member = await e.Guild.GetMemberAsync(e.Message.Author.Id);
                            await member.SendMessageAsync($"Your message contains the filtered word `{filteredWord}` and has thus been deleted.");
                            break;
                        }
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
                //REALM ROYALE
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.RealmRoyale]);
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
                //FIGHTERZ
                await gameMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Games.FighterZ]);
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
                //LFG CONSOLE
                await otherMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Other.LFG_CONSOLE]);
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
            string content = e.Message.Content;
            if (ServerVariables.FilteredGuilds.Contains(e.Guild.Id)) {
                #region Word Censoring
                if (Regex.IsMatch(content.ToLower(), @"\b" + "abby" + @"\b")) {
                    await e.Channel.TriggerTypingAsync();
                    await e.Message.RespondAsync($"Ab.");
                    await e.Message.DeleteAsync("Ab ffs");
                } else {
                    foreach (var filteredWord in FilteredWords) {
                        if (Regex.IsMatch(content.ToLower(), @"\b" + filteredWord.ToLower() + @"\b")) {
                            await e.Message.DeleteAsync("Offensive word filter");
                            var member = await e.Guild.GetMemberAsync(e.Message.Author.Id);
                            await member.SendMessageAsync($"Your message contains the filtered word `{filteredWord}` and has thus been deleted.");
                            break;
                        }
                    }
                }
                #endregion
                #region Bot prevention
                if (Regex.IsMatch(content, @"([a - zA - Z0 - 9] +://)?([a-zA-Z0-9_]+:[a-zA-Z0-9_]+@)?([a-zA-Z0-9.-]+\\.[A-Za-z]{2,4})(:[0-9]+)?(/.*)?") | e.Message.Attachments.Count < 1) {
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
            if (content.ToLower().Contains("play despacito") | (content.ToLower().Contains("alexa") & content.ToLower().Contains("despacito"))) {
                var member = await e.Guild.GetMemberAsync(e.Author.Id);
                await e.Message.RespondAsync("not alexa wont play");
            } else if (content.ToLower().Equals("creeper")) {
                await e.Message.RespondAsync("1. No.\n2. No.\n3. Especially: __**No**__.");
            } else if (content.ToLower().Contains("chuunibyou") || content.ToLower().Contains("chunibyou")) {
                var chance = Random.Next(0, 3);
                if (chance == 0) {
                    await e.Message.RespondAsync("Chuunibyou is love chuunibyou is life.");
                }
            } else if (e.Message.Author.Id == 252810598721519616 && content.ToLower().Contains("anime")) {
                var chance = Random.Next(0, 3);
                if (chance == 0) {
                    await e.Message.RespondAsync("Don't worry warrior, I'll say the line for you.\nAnime? Banime :100: :ok_hand: :laughing:");
                }
            } else if (content.ToLower().Replace(" ", "").Equals("🅰🅱") | content.ToLower().Replace(" ", "").Equals("🆎")) {
                var chance = Random.Next(0, 2);
                if (chance == 0) {
                    await e.Message.RespondAsync($"Ab!");
                }
            } else if ((e.Author.Id == 389990162295291905 || e.Author.Id == 296360459710234624) && content.ToLower().Contains("jojo")) {
                await e.Channel.SendMessageAsync($"wAtCh jOjO rEeeEee");
            } else if (content.ToLower().Replace("'","").Contains("im")) {
                Random = Random ?? new Random();
                var chance = Random.Next(1,9);
                if (chance == 0) {
                    var wordAfterIm = Regex.Match(content.ToLower().Replace("'", ""), @"(?<=\bim\s)(\w+)").ToString();
                    if (!string.IsNullOrWhiteSpace(wordAfterIm)) {
                        await e.Message.RespondAsync($"Hi {wordAfterIm}, I'm Yuuta.");
                    }   
                }
            }
        }

        private async static Task Discord_Ready(ReadyEventArgs e) {
            await e.Client.UpdateStatusAsync(new DiscordActivity("Do I overrate Chuunibyou? No. Stick your tongue into a power outlet.", ActivityType.Playing));

        }

        private async static Task OnReactionAdded(MessageReactionAddEventArgs e) {
            if (e.User.IsBot) {
                return;
            }
            var messageId = e.Message.Id;
            if (messageId == ServerVariables.TheBeaconGameRoleReactMessageId | messageId == ServerVariables.TheBeaconOtherRoleReactMessageId | messageId == ServerVariables.TheBeaconPlatformMessageId) {
                var member = await e.Channel.Guild.GetMemberAsync(e.User.Id);
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
                        //await recepient.SendMessageAsync($"I have granted you the {role.RoleName} role in `{e.Channel.Guild.Name}`!");
                        var confirmMessage = await e.Channel.SendMessageAsync($"{e.User.Username}, I have granted you the {role.RoleName} role in `{e.Channel.Guild.Name}`!");
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        await confirmMessage.DeleteAsync();
                    }
                }
            }
        }

        private async static Task OnReactionRemoved(MessageReactionRemoveEventArgs e) {
            if (e.User.IsBot) {
                return;
            }
            var messageId = e.Message.Id;
            if (messageId == ServerVariables.TheBeaconGameRoleReactMessageId | messageId == ServerVariables.TheBeaconOtherRoleReactMessageId | messageId == ServerVariables.TheBeaconPlatformMessageId) {
                var roles = GameRole.ParseRole(e.Emoji.Id);
                if (roles == null || roles.Count < 1)
                    return;
                var role = roles[0];
                var reactionRole = e.Channel.Guild.GetRole(role.RoleId);
                var recepient = await e.Channel.Guild.GetMemberAsync(e.User.Id);
                await recepient.RevokeRoleAsync(reactionRole);
                var confirmMessage = await e.Channel.SendMessageAsync($"{e.User.Username}, I have revoked your {role.RoleName} role in `{e.Channel.Guild.Name}`!");
                await Task.Delay(TimeSpan.FromSeconds(5));
                await confirmMessage.DeleteAsync();
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
                } catch (Exception) {
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