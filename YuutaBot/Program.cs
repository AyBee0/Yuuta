using Commands.Modules;
using DiscordMan;
using DiscordMan.Attributes;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Generatsuru;
using Globals;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YuutaBot.Events;

namespace YuutaBot
{
    class Program
    {
        static void Main(string[] args) => MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        static async Task MainAsync(string[] args)
        {
            _ = args;
            Console.WriteLine($"Bot version {Assembly.GetExecutingAssembly().GetName().Version}");
            #region Discord Setup
            var discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Environment.GetEnvironmentVariable("yuutatoken"),
                MinimumLogLevel = LogLevel.Debug,
            });
            discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = Configuration.Prefixes,
                EnableMentionPrefix = Configuration.EnableMentions
            });
            discord.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(3)
            });
            discord.GetCommandsNext().RegisterCommands<MemberCommandsModule>();
            discord.GetCommandsNext().RegisterCommands<StaffCommandsModule>();
            Dictionary<string, DiscordCommandMan> commands = discord.GetCommandsNext().RegisteredCommands.ToDictionary(x => x.Key, x => new DiscordCommandMan(x.Value));
            AttributeTools.CommandALs.AddRange(commands);
            #endregion
            await discord.ConnectAsync();
            Console.WriteLine("Connected successfully.");
            DiscordEvents.SetupSubscriptions(discord);
            await Task.Delay(-1);
        }
    }
}