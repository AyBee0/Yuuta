using Commands;
using DiscordEvents;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using FirebaseHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Types;

namespace Yuutabot {
    class Program {

        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();
        private static DiscordClient Discord;
        private static YuutaFirebaseClient FirebaseClient;

        static async Task MainAsync(string[] args) {
            #region Discord
            Discord = new DiscordClient(new DiscordConfiguration {
                Token = "NTYxMjg4NDM4MjMwNDE3NDM4.XQ6dFw.V-i30a9HTeCAN5cqBPrZdw6fP6M",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug,
            });
            CommandsNextExtension commands;
#if DEBUG
            commands = Discord.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefixes = new List<string> { "tt!" }
            });
#else
            commands = Discord.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefixes = new List<string> { "~", "-", "yu!", "yuuta.", "" }
            });
#endif
            commands.RegisterCommands<GlobalCommands>();
            commands.RegisterCommands<StaffCommands>();
            commands.RegisterCommands<GuildBotSetupCommands>();
            Discord.UseInteractivity(new InteractivityConfiguration { });
            Discord.MessageCreated += GuildMessageCreateAndEditEvents.OnMessageCreated;
            //Discord.GuildMemberAdded += GuildMemberEvents.GuildMemberAdded;
            //Discord.GuildMemberRemoved += GuildMemberEvents.GuildMemberRemoved;
            Discord.MessageReactionAdded += GuildReactionEvents.MessageReactionAdded;
            //Discord.MessageReactionRemoved += GuildReactionEvents.MessageReactionRemoved;
            Discord.Ready += OnDiscordReady;
            #endregion
            await Discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task OnDiscordReady(ReadyEventArgs e) {
            return Task.Run(() => {
                //This library sucks. Yes, unnecessary code. None of this makes any sense. Yes, I want to die. 
                YuutaBot databaseObject;
                FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient(false);
                FirebaseClient.CurrentQuery.AsObservable<YuutaBot>().Subscribe(async root => {
                    try {
                        JObject jObject = JObject.Parse(JsonConvert.SerializeObject(root));
                        databaseObject = root.Object;
                        await FirebaseHandler.HandleNewGuildChanges(databaseObject, Discord);
                        YuutaFirebaseClient.Database = databaseObject;
                        //YuutaFirebaseClient.Guilds = guilds;
                    } catch (Exception ex) {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            });
        }
    }
}
