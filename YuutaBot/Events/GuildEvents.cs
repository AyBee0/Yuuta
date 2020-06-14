using DataAccessLayer;
using DiscordAccessLayer;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuutaBot.Events
{
    // GUILD EVENTS
    public static partial class DiscordEvents
    {
        private static Task Client_GuildCreated(GuildCreateEventArgs e)
        {
            return Task.Run(async () =>
            {
                await DiscordGuildAL.ManageBotFirstRunAsync(e.Guild);
                await Task.Yield();
            });
        }
    }
}
