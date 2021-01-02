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
            client.MessageCreated += (s, e) => Client_MessageSent(s,e);
            client.GuildDownloadCompleted += (s, e) => Client_GuildDownloadCompleted(s,e);
            client.GuildCreated += (s, e) => Client_GuildCreated(s, e);
        }
    }
}