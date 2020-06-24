using DataAccessLayer;
using DataAccessLayer.DataAccess;
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
        //private static Task Client_GuildCreated(GuildCreateEventArgs e)
        //{
        //    Console.WriteLine(e.Guild.Name);
        //    return Task.Run(async () =>
        //    {
        //        await new DiscordGuildAL(e.Guild).ManageBotFirstRunAsync();
        //    });
        //}

        private static Task Client_GuildCreated(GuildCreateEventArgs e)
        {
            return Task.Run(async () =>
            {
                DiscordGuildAL.NewGuildCreated(e.Guild);
                await Task.Yield();
            });
        }

        private static Task OnClientReady(ReadyEventArgs e)
        {
            return Task.Run(async () =>
            {
                foreach (var guild in e.Client.Guilds.Values)
                {
                    DiscordGuildAL.AddInitialGuildsIfUnique(e.Client.Guilds.Values.ToList());
                }
                await Task.Yield();
            });
        }
    }   
}
