using Commands.ResultEntities;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Globals;
using InteractivityHelpers;
using InteractivityHelpers.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Commands.YuutaTasks
{
    public static partial class Tasks
    {
        public static async Task<bool> NewEventAsync(CommandContext ctx)
        {
            EventResult result = await GetEventInfoAsync(ctx);
            var success = HandleResult(ctx, result);
            if (result == null)
            {
                return false;
            }
            var embedBuilder = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    IconUrl = ctx.Client.CurrentUser.AvatarUrl,
                    Name = $"Yuuta - Developed By Ab#8582",
                    Url = @"https://www.youtube.com/watch?v=dQw4w9WgXcQ"
                },
                Title = result.Title,
                Description = result.Description,
                ImageUrl = result.Thumbnail.Url,
                Color = new DiscordColor(Constants.EmbedColor),
                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Id: {{LOADING...}}" }
            };
            embedBuilder.AddField("Event Date", result.EventDate.ToString("dddd, dd MMMM yyyy UTC/GMT"), true);
            embedBuilder.AddField("Related Channel(s)", string.Join(" | ", result.RelatedChannels.Select(x => x.Mention)));
            if (result.EventPlatform != null)
            {
                embedBuilder.AddField("Event Platform", result.EventPlatform);
            }
            if (result.EventRegion != null)
            {
                embedBuilder.AddField("Event Region", result.EventRegion);
            }
            await result.ResultChannel
                .SendMessageAsync("To be reminded of this event 15 minutes prior to the specified date, " +
                "please click on the :white_check_mark: at the bottom of this message." +
                string.Join("", result.Mentions.Select(x => x.Mention).ToList()),
                embed: embedBuilder
            );
            return true;
        }

        private static async Task<EventResult> GetEventInfoAsync(CommandContext ctx)
        {
            InteractivityOperation<EventResult> operation = new InteractivityOperation<EventResult>(TimeSpan.FromMinutes(3))
            {
                new Interaction<EventResult>
                ("**What's the title of the event?**", Parsers.StringParser, (x) => x.Title)
                {
                    AppendCancelMessage = true
                },

                new Interaction<EventResult>
                ("**What's the description of the event?**", Parsers.StringParser, (x) => x.Description),

                new Interaction<EventResult>
                ("**What role(s) should be pinged in the announcement message?**", Parsers.RolesParser,
                (x) => x.Mentions,
                Conditions.RolesCondition)
                {
                    AcceptNone = true
                },

                new Interaction<EventResult>("**What's the date of this event? Send it in `mm/dd/yyyy 0:00PM/AM` format. " +
                "E.g:\n`" +
                $"{DateTime.Now.AddDays(7):MM/dd/yyyy h:m tt}`", Parsers.DateTimeParser,
                (x) => x.EventDate, Conditions.DateTimeCondition),

                new Interaction<EventResult>
                ("**If there is/are any channel(s) related to this event, such as a sign up channel, please #mention them.?**",
                Parsers.StringParser, (x) => x.RelatedChannels)
                {
                    AcceptNone = true
                },

                new Interaction<EventResult>("**What message would you like to be sent 15 minutes before the event for those who choose to be reminded?",
                Parsers.StringParser, (x) => x.ReminderMessage)
                {
                    AcceptNone = true,
                },

                new Interaction<EventResult>("**Is this event region specific? such as a North America server game gathering? Send the region if so.**",
                Parsers.StringParser, x => x.EventRegion)
                {
                    AcceptNone = true, NoneKeyword = "no"
                },

                new Interaction<EventResult>("**Is this event platform specific, such as a PS4 game gathering? Send the platform if so.**",
                Parsers.StringParser, x => x.EventPlatform)
                {
                    AcceptNone = true, NoneKeyword = "no"
                },

                new Interaction<EventResult>("**Please create a countdown timer at a site like https://itsalmo.st and send the URL of the countdown here.",
                Parsers.StringParser, x => x.Countdown),

                new Interaction<EventResult>("**Would you like a thumbnail for the event? This will be displayed in the announcement message." +
                " If so, do attach it; don't send its URL as that won't be parsed.",
                Parsers.AttachmentsParser, x => x.Thumbnail){
                    AcceptNone = true, NoneKeyword = "no"
                },

                new Interaction<EventResult>("**Finally, in which #channel would you like me to announce this event?**",
                Parsers.ChannelsParser, x => x.ResultChannel)
            };
            OperationResult<EventResult> returned = await operation.ExecuteAsync(ctx);
            return returned.Result;
        }
        private static async Task<bool> HandleResult(CommandContext ctx, EventResult result)
        {
            switch (result.Status)
            {
                case InteractivityStatus.Cancelled:
                    await ctx.RespondAsync($":exclamation: Cancelled successfully.");
                    return false;
                case InteractivityStatus.TimedOut:
                    await ctx.RespondAsync($":x: Operation timed out. Please try again.");
                    return false;
                case InteractivityStatus.OK:
                case InteractivityStatus.Finished:
                    await ctx.RespondAsync(":white_check_mark: Success!");
                    return true;
                default:
                    throw new NotImplementedException("Interactivity status not implemented");
            }
        }

    }
}