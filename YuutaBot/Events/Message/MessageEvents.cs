using DataAccessLayer;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Threading.Tasks;

namespace YuutaBot.Events
{
    public static partial class DiscordEvents
    {
        private static async Task Client_MessageSent(DiscordClient sender, MessageCreateEventArgs e)
        {
            await Task.Yield();
        }
    }
}
