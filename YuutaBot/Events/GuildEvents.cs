using DataAccessLayer.DataAccess.Layers;
using DiscordMan;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Serilog;
using Serilog.Core;
using System;
using System.Linq;
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

        private static async Task Client_GuildCreated(DiscordClient client, GuildCreateEventArgs e)
        {
            Log.Information($"Guild of ID {e.Guild.Id} created.");
            GuildDAL.AddGuildIfUnique(e.Guild);
            await Task.Yield();
        }

        //private static Task OnClientReady(ReadyEventArgs e)
        //{
        //    return Task.Run(async () =>
        //    {
        //        foreach (var kvp in e.Client.Guilds)
        //        {
        //            var guild = await e.Client.GetGuildAsync(310279910264406017);
        //            if (guild.Name == null)
        //            {
        //                continue;
        //            }
        //            DiscordGuildAL.AddInitialGuildsIfUnique(e.Client.Guilds.Values.ToList());
        //        }
        //        await Task.Yield();
        //    });
        //}

        private static async Task Client_GuildDownloadCompleted(DiscordClient client, GuildDownloadCompletedEventArgs e)
        {
            GuildDAL.AddGuildsIfUnique(client.Guilds.Values.ToList());
            await Task.Yield();
        }
    }
}