using Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Firebase.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Types;
using DiscordEvents;
using System.Threading;
using DSharpPlus.EventArgs;

namespace Yuutabot {
    class Program {

        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        static async Task MainAsync(string[] args) {
            #region Discord
            var discord = new DiscordClient(new DiscordConfiguration {
                Token = "NTYxMjg4NDM4MjMwNDE3NDM4.XQ6dFw.V-i30a9HTeCAN5cqBPrZdw6fP6M",
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug,
                HttpTimeout = Timeout.InfiniteTimeSpan
            });
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefixes = new List<string> { "tt!" } //TODO Change
            });
            commands.RegisterCommands<GlobalCommands>();
            discord.MessageCreated += MessageEvents.OnMessageCreated;
            discord.GuildMemberAdded += GuildMemberEvents.GuildMemberAdded;
            discord.GuildMemberRemoved += GuildMemberEvents.GuildMemberRemoved;
            #endregion
            #region Firebase
            Dictionary<string, Guild> guilds;
            var firebaseClient = new FirebaseClient("https://the-beacon-team-battles.firebaseio.com/");
            var child = firebaseClient.Child("Root");
            child.AsObservable<object>().Subscribe(root => {
                JObject jObject = JObject.Parse(JsonConvert.SerializeObject(root));
                guilds = jObject["Object"].ToObject<Dictionary<string, Guild>>();
                FirebaseHandler.HandleNewGuildChanges(guilds, discord);
                DiscordEvent.Guilds = guilds;
            });
            #endregion
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static void PopulateDiscordEvents(Dictionary<string,Guild> guilds) {
            
        }

    }
}
