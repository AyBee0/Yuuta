using DataAccessLayer;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Threading.Tasks;

namespace YuutaBot.Events
{
    public static partial class DiscordEvents
    {
        private static Task Client_MessageSent(DiscordClient sender, MessageCreateEventArgs e)
        {
            return Task.Run(async () =>
            {
                await Task.Yield();
            });
        }
    }
}
