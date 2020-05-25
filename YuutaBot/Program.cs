using DSharpPlus;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace YuutaBot
{
    class Program
    {
        static void Main(string[] args) => MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        static async Task MainAsync(string[] args)
        {
            _ = args;
            var discord = new DiscordClient(new DiscordConfiguration
            {
                Token = Environment.GetEnvironmentVariable("yuutatoken"),
                TokenType = TokenType.Bot
            });
            var user = await discord.GetCurrentApplicationAsync();
            Console.WriteLine($"{user.Name} version {Assembly.GetExecutingAssembly().GetName().Version}");
            //discord.
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}