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
            client.MessageCreated += OnMessageSent;
            client.Ready += OnClientReady;
            client.GuildCreated += Client_GuildCreated;
        }
    }
}