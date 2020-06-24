using DataAccessLayer;
using DSharpPlus.EventArgs;
using System.Threading.Tasks;

namespace YuutaBot.Events
{
    public static partial class DiscordEvents
    {
        private static Task OnMessageSent(MessageCreateEventArgs e)
        {
            return Task.Run(async () =>
            {
                await Task.Yield();
            });
        }
    }
}
