using Commands.ResultEntities;
using Data.Models.Events;
using DataAccessLayer.Models;
using DataAccessLayer.Models.Events;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Globals;
using InteractivityHelpers;
using InteractivityHelpers.Entities;
using Newtonsoft.Json;
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
            //NewEventResult result = await GetEventInfoAsync(ctx);
            NewEventResult result = new()
            {
                Title = "New Event",
                Description = "This is a new event",
                Countdown = "https://itsalmo.st",
                EventDate = DateTime.Now.AddDays(2),
                ReminderMessage = "Testing",
                ResultChannel = ctx.Channel,
            };
            var success = await HandleResult(ctx, result);
            if (!success)
            {
                return false;
            }
            using var db = new YuutaDbContext();
            DiscordMessage msg = await SendEmbed(ctx, result);
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    DirectMessageEvent dmEvent = new(result.EventDate, ctx.Guild.Id, result.ReminderMessage);
                    db.DirectMessageEvents.Add(dmEvent);
                    await db.SaveChangesAsync();
                    ReactionLinkedEvent reactionLinkedEvent = new(EventType.DirectMessageEvent,
                        msg.ChannelId, msg.Id, dmEvent);
                    db.ReactionLinkedEvents.Add(reactionLinkedEvent);
                    await db.SaveChangesAsync();
                }
                catch (Exception w)
                {
                    await msg.DeleteAsync();
                    await result.ResultChannel.SendMessageAsync(":x: An event was supposed to be announced here, but an unknown error has occured." +
                        "If you've been pinged, you can ignore this until the issue is resolved.");
                    await ctx.RespondAsync(":x: An unhandled exception occured. The event message has been deleted.");
                    await transaction.RollbackAsync();
                }
            }
            return true;
        }

        private static async Task<DiscordMessage> SendEmbed(CommandContext ctx, NewEventResult result)
        {
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
                ImageUrl = result.Thumbnail?.Url,
                Color = new DiscordColor(Constants.EmbedColor),
                Footer = new DiscordEmbedBuilder.EmbedFooter() { Text = "Id: {{LOADING...}}" }
            };
            embedBuilder.AddField("Event Date", result.EventDate.ToString("dddd, dd MMMM yyyy UTC/GMT"), true);
            if (result.RelatedChannels != null && result.RelatedChannels.Count != 0)
            {
                embedBuilder.AddField("Related Channel(s)", string.Join(" | ", result.RelatedChannels.Select(x => x.Mention)));
            }
            if (result.EventPlatform != null)
            {
                embedBuilder.AddField("Event Platform", result.EventPlatform);
            }
            if (result.EventRegion != null)
            {
                embedBuilder.AddField("Event Region", result.EventRegion);
            }
            System.Collections.Generic.List<string> mentions = result.Mentions?.Select(x => x.Mention).ToList();
            string mentionText;
            if (mentions == null)
            {
                mentionText = "";
            }
            else
            {
                mentionText = string.Join("", mentions);
            }
            return await result.ResultChannel
                .SendMessageAsync("To be reminded of this event 15 minutes prior to the specified date, " +
                "please click on the :white_check_mark: at the bottom of this message." +
                mentionText,
                embed: embedBuilder
            );
        }

        private static async Task<NewEventResult> GetEventInfoAsync(CommandContext ctx)
        {
            InteractivityOperation<NewEventResult> operation = new InteractivityOperation<NewEventResult>(TimeSpan.FromMinutes(3))
            {
                new Interaction<NewEventResult>
                ("**What's the title of the event?**", Parsers.StringParser, x => x.Title)
                {
                    AppendCancelMessage = true
                },

                new Interaction<NewEventResult>
                ("**What's the description of the event?**", Parsers.StringParser, (x) => x.Description),

                new Interaction<NewEventResult>
                ("**What role(s) should be pinged in the announcement message?**", Parsers.RolesParser,
                (x) => x.Mentions)
                {
                    AcceptNone = true
                },

                new Interaction<NewEventResult>("**What's the date of this event? Send it in `mm/dd/yyyy 0:00PM/AM` format.** " +
                "E.g:\n`" +
                $"{DateTime.Now.AddDays(7):MM/dd/yyyy h:m tt}`", Parsers.FutureDateTimeParser,
                (x) => x.EventDate),

                new Interaction<NewEventResult>
                ("**If there is/are any channel(s) related to this event, such as a sign up channel, please #mention them.**",
                Parsers.ChannelsParser, (x) => x.RelatedChannels)
                {
                    AcceptNone = true
                },

                new Interaction<NewEventResult>("**What message would you like for me to send 15 minutes before the event for those who choose to be reminded?**",
                Parsers.StringParser, (x) => x.ReminderMessage)
                {
                    AcceptNone = true,
                },

                new Interaction<NewEventResult>("**Is this event region specific? such as a North America server game gathering? Send the region if so.**",
                Parsers.StringParser, x => x.EventRegion)
                {
                    AcceptNone = true, NoneKeyword = "none"
                },

                new Interaction<NewEventResult>("**Is this event platform specific, such as a PS4 game gathering? Send the platform if so.**",
                Parsers.StringParser, x => x.EventPlatform)
                {
                    AcceptNone = true, NoneKeyword = "none"
                },

                new Interaction<NewEventResult>("**Please create a countdown timer at a site like https://itsalmo.st and send the URL of the countdown here.",
                Parsers.URLParser, x => x.Countdown),

                new Interaction<NewEventResult>("**Would you like a thumbnail for the event? This will be displayed in the announcement message.**" +
                " If so, do attach it; don't send its URL as that won't be parsed.",
                Parsers.AttachmentParser, x => x.Thumbnail){
                    AcceptNone = true, NoneKeyword = "none"
                },

                new Interaction<NewEventResult>("**Finally, in which #channel would you like me to announce this event?**",
                Parsers.ChannelParser, x => x.ResultChannel)
            };
            OperationResult<NewEventResult> returned = await operation.ExecuteAsync(ctx);
            return returned.Result;
        }
        private static async Task<bool> HandleResult(CommandContext ctx, NewEventResult result)
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