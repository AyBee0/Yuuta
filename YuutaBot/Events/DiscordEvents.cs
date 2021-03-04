using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Threading.Tasks;

namespace YuutaBot.Events
{
    public static partial class DiscordEvents
    {
        public static void SetupSubscriptions(DiscordClient client)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            client.MessageCreated += async (s, e) => Task.Run(() => Client_MessageSent(s, e));
            client.GuildDownloadCompleted += async (s, e) => Task.Run(() => Client_GuildDownloadCompleted(s, e));
            client.GuildCreated += async (s, e) => Task.Run(() => Client_GuildCreated(s, e));
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}