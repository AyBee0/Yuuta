using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using InteractivityHelpers;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InteractivityHelpers.InteractivityEventTracker;

namespace Commands.YuutaTasks
{
    public partial class InteractivityMan
    {
        private readonly InteractivityEventTracker tracker;
        private readonly CommandContext ctx;
        public InteractivityMan(CommandContext ctx, TimeSpan? timeout = null)
        {
            this.ctx = ctx;
            tracker = new InteractivityEventTracker(ctx, timeout);
        }

        public async Task CreateNewReactionMessageAsync()
        {
            string title;
            string description;
            List<DiscordRole> rolesToPing;
            List<DiscordChannel> eventChannels;
            string platform;
            string region;
            int reminderMinutes;
            string reminderMessage;
            DateTime date;
            string countdownTimer;
            DiscordAttachment thumbnail;
            while (true)
            {
                title = await AskForTextAsync(new InteractionConfig() { AppendCancel = true },
               "**What is the title of the event?**");
                if (tracker.Status != InteractivityStatus.OK)
                {
                    break;
                }
                description = await
                    AskForTextAsync(new InteractionConfig(), "**What's the description of the event?**");
                if (tracker.Status != InteractivityStatus.OK)
                {
                    break;
                }
                date = await
                    AskForDateTimeAsync(new InteractionConfig(),
                    "**When is this event? Send the date in the format of `mm/dd/yyyy 00:00 AM/PM`.**");
                if (tracker.Status != InteractivityStatus.OK)
                {
                    break;
                }
                countdownTimer = await
                    AskForLinkAsync(new InteractionConfig({ AcceptNone = true },))
                rolesToPing = await
                    AskForRolesAsync(new InteractionConfig() { AcceptNone = true },
                    "**What roles would you like to mention (ping) for the event?");
                if (tracker.Status != InteractivityStatus.OK)
                {
                    break;
                }
                eventChannels = await
                    AskForChannelsAsync(new InteractionConfig() { AcceptNone = true },
                    "**If there are any channels related to this event, such as a sign up channel, #mention them.**");
                if (tracker.Status != InteractivityStatus.OK)
                {
                    break;
                }
                platform = await AskForTextAsync(new InteractionConfig() { AcceptNone = true },
                    "**If this event is platform-specific, such as `PS4`, `XBOX`, `Switch`, or `PC`," +
                    " send the platform below.");
                if (tracker.Status != InteractivityStatus.OK)
                {
                    break;
                }
                region = await AskForTextAsync(new InteractionConfig() { AcceptNone = true },
                    "**If this event is region-specific, such as `EU`, `NA`, `OCE`, or `AS`, send the region below.");
                if (tracker.Status != InteractivityStatus.OK)
                {
                    break;
                }
                reminderMessage = await AskForTextAsync(new InteractionConfig() { AcceptNone = true },
                    "**If you would like to remind those who choose the option to be reminded, what would the DM message be?**");
                if (tracker.Status != InteractivityStatus.OK)
                {
                    break;
                }
                if (reminderMessage.ToLower().Trim() != "none")
                {
                    reminderMinutes = await AskForIntAsync(new InteractionConfig(),
                        "How many minutes before the event would you like to remind participants of it?");
                }

            }
        }
    }
}