using DSharpPlus;
using Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Types;

namespace Yuutabot {
    public static class FirebaseHandler {


        //NameValueCollection props = new NameValueCollection {
        //                        { "quartz.serializer.type", "binary" }
        //                    };
        //StdSchedulerFactory factory = new StdSchedulerFactory(props);
        //IScheduler sched = factory.GetScheduler().GetAwaiter().GetResult();
        static NameValueCollection Props;
        static StdSchedulerFactory Factory;
        static IScheduler Sched;

        public static void HandleNewGuildChanges(Dictionary<string, Guild> guilds, DiscordClient client) {
            if (Props == null) {
                Props = new NameValueCollection {
                    { "quartz.serializer.type", "binary" }
                };
            }
            if (Factory == null) {
                Factory = new StdSchedulerFactory(Props);
            }
            if (Sched == null) {
                Sched = Factory.GetScheduler().GetAwaiter().GetResult();
            }
            Sched.Clear();
            if (guilds == null) {
                Console.WriteLine("No guilds to iterate over.");
            }
            foreach (var guildKeyValuePair in guilds) {
                var guildID = ulong.Parse(guildKeyValuePair.Key);
                var guildInformation = guildKeyValuePair.Value;
                var guild = client.GetGuildAsync(guildID).GetAwaiter().GetResult();
                foreach (var guildEventKeyValuePair in guildInformation.GuildEvents) {
                    var guildEventID = guildEventKeyValuePair.Key;
                    var guildEvent = guildEventKeyValuePair.Value;
                    DateTime guildEventTime = DateTime.ParseExact(guildEvent.Date, "MM/dd/yyyy hh:mm tt", null);
                    var timeUntil = guildEventTime.Subtract(DateTime.Now);
                    var a = timeUntil.Seconds;
                    if (guildEventTime.CompareTo(DateTime.Now) >= 0) {
                        IJobDetail job = JobBuilder.Create<EventJob>()
                            .WithIdentity(guildEventID, guildID.ToString())
                            .Build();
                        job.JobDataMap["discordClient"] = client;
                        job.JobDataMap["guildEvent"] = guildEvent;
                        var trigger = (ISimpleTrigger)TriggerBuilder.Create()
                            .WithIdentity(guildEventID + "_TRIGGER", "guildevents")
                            .StartAt(guildEventTime.ToUniversalTime())
                            .ForJob(guildEventID, guildID.ToString())
                            .Build();
                        Sched.Start();
                        Sched.ScheduleJob(job, trigger);
                    }
                }
            }
            Console.WriteLine($"Guilds have been populated.");
        }

    }
}
