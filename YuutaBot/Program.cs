using AuthorityHelpers;
using Commands;
using DiscordEvents;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Firebase.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Types;
using DSharpPlus.Interactivity;
using DSharpPlus.EventArgs;

namespace Yuutabot {
    class Program {

        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();
        private static DiscordClient Discord;

        static async Task MainAsync(string[] args) {
            #region Discord
            Discord = new DiscordClient(new DiscordConfiguration {
                Token = "NTYxMjg4NDM4MjMwNDE3NDM4.XQ6dFw.V-i30a9HTeCAN5cqBPrZdw6fP6M",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug,
                HttpTimeout = Timeout.InfiniteTimeSpan
            });
            var commands = Discord.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefixes = new List<string> { "tt!" } //TODO Change
            });
            commands.RegisterCommands<GlobalCommands>();
            commands.RegisterCommands<StaffCommands>();
            Discord.UseInteractivity(new InteractivityConfiguration { });
            Discord.MessageCreated += GuildMessageCreateAndEditEvents.OnMessageCreated;
            Discord.GuildMemberAdded += GuildMemberEvents.GuildMemberAdded;
            Discord.GuildMemberRemoved += GuildMemberEvents.GuildMemberRemoved;
            Discord.MessageReactionAdded += GuildReactionEvents.MessageReactionAdded;
            Discord.MessageReactionRemoved += GuildReactionEvents.MessageReactionRemoved;
            Discord.Ready += OnDiscordReady;
            #endregion
            await Discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private async static Task OnDiscordReady(ReadyEventArgs e) {
            await Task.Run(() => {
                Dictionary<string, Guild> guilds;
                var firebaseClient = new FirebaseClient("https://the-beacon-team-battles.firebaseio.com/");
                var child = firebaseClient.Child("Root");
                child.AsObservable<object>().Subscribe(root => {
                    JObject jObject = JObject.Parse(JsonConvert.SerializeObject(root));
                    guilds = jObject["Object"].ToObject<Dictionary<string, Guild>>();
                    FirebaseHandler.HandleNewGuildChanges(guilds, Discord);
                    DiscordEvent.Guilds = guilds;
                    AuthorityHelper.Guilds = guilds;
                });
            });
        }
    }
}
