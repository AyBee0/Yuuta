using Commands.Modules;
using DiscordMan;
using DiscordMan.Attributes;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Generatsuru;
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
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Info,
            });
#if !DEBUG
                        discord.UseCommandsNext(new CommandsNextConfiguration()
                        {
                            StringPrefixes = new string[] { "!", "~", "-", },
                        });
#else
            discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { "tt!" },
                EnableMentionPrefix = false
            });
#endif
            discord.GetCommandsNext().RegisterCommands<MemberCommandsModule>();
            Dictionary<string, DiscordCommandMan> commands = discord.GetCommandsNext().RegisteredCommands.ToDictionary(x => x.Key, x => new DiscordCommandMan(x.Value));
            RestrictedAttribute.CommandALs.AddRange(commands);
            #endregion
            await discord.ConnectAsync();
            DiscordEvents.SetupSubscriptions(discord);
            await Task.Delay(-1);
        }
    }
}