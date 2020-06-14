using Commands;
using DataAccessLayer;
using DataAccessLayer.Models;
using DataAccessLayer.Models.GuildModels;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using System;
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
            discord.GetCommandsNext().RegisterCommands<GlobalCommandsModule>();
            #endregion
            DiscordEvents.SetupSubscriptions(discord);
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}