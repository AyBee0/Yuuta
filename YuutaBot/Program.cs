﻿using Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json.Linq;
using RoleAssignment;
using ServerVariable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace YuutaBot {
    class Program {

        static DiscordClient discord;
        static CommandsNextExtension commands;
        static readonly bool RunReactionAdd = false;
        static DiscordRole NeutralRole;
        static DiscordRole AbRole;
        static DiscordRole BargotRole;
        static ChildQuery Child;
        static bool RecordingStarted;

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
                HttpTimeout = System.Threading.Timeout.InfiniteTimeSpan //TODO - DELETE ----------------------------------------------------------------------------------------------
            });
            commands = discord.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefixes = new[] { "tt!", "yu!" }
            });
            commands.SetHelpFormatter<YuutaHelpFormatter>();
            commands.RegisterCommands<GlobalCommands>();
            commands.RegisterCommands<ModeratorCommands>();
            discord.Ready += Discord_Ready;
            discord.MessageReactionAdded += OnReactionAdded;
            discord.MessageReactionRemoved += OnReactionRemoved;
            discord.MessageCreated += OnMessageCreated;
            discord.GuildAvailable += OnGuildAvailable;
            var TheBeaconGuild = await discord.GetGuildAsync(ServerVariables.TheBeaconId);
            NeutralRole = TheBeaconGuild.GetRole(607205082525597706);
            AbRole = TheBeaconGuild.GetRole(607203125883043843);
            BargotRole = TheBeaconGuild.GetRole(607204919971151882);
            var FirebaseClient = new FirebaseClient("https://the-beacon-team-battles.firebaseio.com/");
            await Task.Run(() => {
                CheckIfRecordingStarted(FirebaseClient);
            });
            Console.WriteLine($"RECORDING STARTED: {RecordingStarted}");
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async void CheckIfRecordingStarted(FirebaseClient firebaseClient) {
            while (true) {
                Console.WriteLine("Checking to see if recording should be initialized...");
                RecordingStarted = await firebaseClient.Child("info").Child("RecordingStarted").OnceSingleAsync<bool>();
                Console.WriteLine($"Recording Initialized: {RecordingStarted}");
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        private async static Task OnGuildAvailable(GuildCreateEventArgs e) {
            if (!RunReactionAdd) {
                return;
            }
            var roleChannel = await e.Client.GetChannelAsync(ServerVariables.TheBeaconRoleChannelId);
            var harmonyGuild = await e.Client.GetGuildAsync(453487691216977922); // harmony guild
            var gameMessage = await roleChannel.GetMessageAsync(ServerVariables.TheBeaconGameRoleReactMessageId);

            var tempChannel = await e.Client.GetChannelAsync(607900655264333824);
            var tempMessage = await tempChannel.GetMessageAsync(ServerVariables.TheBeaconTempReactMessageId);
            //var gameMessage2 = await roleChannel.GetMessageAsync(ServerVariables.TheBeaconGameRoleReactMessageId2);
            var otherMessage = await roleChannel.GetMessageAsync(ServerVariables.TheBeaconOtherRoleReactMessageId);
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
            #endregion
            #region Temp
            await tempMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Temp.Ab]);
            await tempMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Temp.Bargot]);
            await tempMessage.CreateReactionAsync(harmonyGuild.Emojis[RoleVariables.TheBeacon.Emojis.Temp.Neutral]);
            #endregion
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
            if (RecordingStarted && e.Guild.Id == ServerVariables.TheBeaconId) {
                var member = await e.Guild.GetMemberAsync(e.Author.Id);
                if (member.Roles.Contains(AbRole)) {
                    var value = await Child.Child("Ab").OnceSingleAsync<int>();
                    //await AbChild.PatchAsync($"\"Team Ab\": {++value}");
                    var jsonObject = new JObject {
                        ["Ab"] = ++value
                    };
                    await Child.PatchAsync(jsonObject);
                } else if (member.Roles.Contains(NeutralRole)) {
                    var value = await Child.Child("Neutral").OnceSingleAsync<int>();
                    //await NeutralChild.PatchAsync($"\"Team Neutral\": {++value}");
                    var jsonObject = new JObject {
                        ["Neutral"] = ++value
                    };
                    await Child.PatchAsync(jsonObject);
                } else if (member.Roles.Contains(BargotRole)) {
                    var value = await Child.Child("Bargot").OnceSingleAsync<int>();
                    //await BargotChild.PatchAsync($"\"Team Bargot\": {++value}");
                    var jsonObject = new JObject {
                        ["Bargot"] = ++value
                    };
                    await Child.PatchAsync(jsonObject);
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
            if (messageId == ServerVariables.TheBeaconGameRoleReactMessageId | messageId == ServerVariables.TheBeaconOtherRoleReactMessageId | messageId == ServerVariables.TheBeaconTempReactMessageId) {
                var member = await e.Channel.Guild.GetMemberAsync(e.User.Id);
                if (e.Message.Id == ServerVariables.TheBeaconTempReactMessageId & member.Roles.Any(x => x.Id == 607203125883043843 | x.Id == 607204919971151882 | x.Id == 607205082525597706)) {
                    await e.Message.DeleteReactionAsync(e.Emoji, e.User);
                    await member.SendMessageAsync("You've already chosen a side! Stay loyal! By rule #10 of the UN's Great War agreement, once you choose a team, you're stuck with it. Failing to do so is a war crime.");
                    return;
                }
                var role = GameRole.ParseRole(e.Emoji.Id);
                if (role == null)
                    return;
                var reactionRole = e.Channel.Guild.GetRole(role.RoleId);
                var recepient = await e.Channel.Guild.GetMemberAsync(e.User.Id);
                if (e.Message.Id == ServerVariables.TheBeaconTempReactMessageId) {
                    await e.Message.DeleteReactionAsync(e.Emoji, e.User);
                }
                await recepient.GrantRoleAsync(reactionRole);
                await recepient.SendMessageAsync($"I have granted you the {role.RoleName} role in `{e.Channel.Guild.Name}`!");
            }
        }

        private async static Task OnReactionRemoved(MessageReactionRemoveEventArgs e) {
            if (e.User.IsBot) {
                return;
            }
            var messageId = e.Message.Id;
            if (messageId == ServerVariables.TheBeaconGameRoleReactMessageId | messageId == ServerVariables.TheBeaconOtherRoleReactMessageId) {
                var role = GameRole.ParseRole(e.Emoji.Id);
                if (role == null)
                    return;
                var reactionRole = e.Channel.Guild.GetRole(role.RoleId);
                var recepient = await e.Channel.Guild.GetMemberAsync(e.User.Id);
                if (recepient.Roles.Contains(reactionRole)) {
                    await recepient.RevokeRoleAsync(reactionRole);
                    await recepient.SendMessageAsync($"I have revoked your {role.RoleName} role in `{e.Channel.Guild.Name}`!");
                }
            }
        }


    }

}