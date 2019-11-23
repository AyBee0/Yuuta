using DSharpPlus;
using DSharpPlus.Entities;
using FirebaseHelper;
using Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Types;
using Types.DatabaseObjects.DiscordObjects;

namespace Yuutabot {
    public static class FirebaseHandler {


        //NameValueCollection props = new NameValueCollection {
        //                        { "quartz.serializer.type", "binary" }
        //                    };
        //StdSchedulerFactory factory = new StdSchedulerFactory(props);
        //IScheduler sched = factory.GetScheduler().GetAwaiter().GetResult();
        private static NameValueCollection Props;
        private static StdSchedulerFactory Factory;
        private static IScheduler Sched;
        private static Random Random;
        private static YuutaFirebaseClient FirebaseClient;

        public static async Task HandleNewGuildChanges(YuutaBot root, DiscordClient client) {
            try {
                #region Bot Settings Handling
                var botSettings = root?.BotSettings;
                var statuses = botSettings?.BotStatuses?.Where(x => x.Current).ToList();
                Random = Random ?? new Random();
                if (statuses == null || statuses.Count <= 0) {
                    statuses = new List<DiscordStatus> { new DiscordStatus { Activity = DiscordStatus.DefaultDiscordActivity(), Current = true } };
                    statuses[Random.Next(0, statuses.Count)].Current = true;
                    FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                    await FirebaseClient.Child("BotSettings").Child("BotStatuses").SetValueAsync(statuses);
                }
                var randStatus = statuses[Random.Next(0, statuses.Count)];
                //await client.UpdateStatusAsync(randStatus.Activity); TODO - ENABLE
                #endregion
                #region Guild Event Handling
                var guilds = root.Guilds;
                if (Props == null) {
                    Props = new NameValueCollection {
                    { "quartz.serializer.type", "binary" }
                };
                }
                if (Factory == null) {
                    Factory = new StdSchedulerFactory(Props);
                }
                if (Sched == null) {
                    Sched = await Factory.GetScheduler();
                }
                await Sched.Clear();
                if (guilds == null) {
                    Console.WriteLine("No guilds to iterate over.");
                }
                foreach (var guildKeyValuePair in guilds) {
                    var guildID = ulong.Parse(guildKeyValuePair.Key);
                    var guildInformation = guildKeyValuePair.Value;
                    var guild = await client.GetGuildAsync(guildID);
                    if (guildInformation.GuildEvents != null) {
                        foreach (var guildEventKeyValuePair in guildInformation.GuildEvents) {
                            var guildEventID = guildEventKeyValuePair.Key;
                            var guildEvent = guildEventKeyValuePair.Value;
                            DateTime eventTime = DateTime.Parse(guildEvent.Date, null);
                            var timeUntil = eventTime.Subtract(DateTime.UtcNow);
                            var timeUntilString = $"{timeUntil.Hours}h {timeUntil.Minutes}m";
                            if (eventTime.CompareTo(DateTime.UtcNow) >= 0) {
                                IJobDetail job = JobBuilder.Create<EventJob>()
                                    .WithIdentity(guildEventID, guildID.ToString())
                                    .Build();
                                job.JobDataMap["discordClient"] = client;
                                job.JobDataMap["guildEvent"] = guildEvent;
                                var trigger = (ISimpleTrigger)TriggerBuilder.Create()
                                    .WithIdentity(guildEventID + "_TRIGGER", "guildevents")
                                    .StartAt(eventTime.ToLocalTime())
                                    .ForJob(guildEventID, guildID.ToString())
                                    .Build();
                                await Sched.Start();
                                await Sched.ScheduleJob(job, trigger);
                            }
                        }
                    }
                }
                Console.WriteLine($"Guilds have been populated.");
                #endregion
            } catch (Exception ex) {
                Console.Write(ex.StackTrace);
            }
        }

    }
}
