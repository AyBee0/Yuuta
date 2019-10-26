using AuthorityHelpers;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Types;
using static Types.EventType;
using static Types.RoleActionTypes;

namespace Commands {

    public class StaffCommands : BaseCommandModule {

        private static Random Random;

        //[Aliases("createevent")]
        //[Command("newevent")]
        //public async Task CreateNewEvent(CommandContext ctx) {
        //    if (ctx.IsStaffMember()) {
        //        if (Random == null) {
        //            Random = new Random();
        //        }
        //        var interactivity = ctx.Client.GetInteractivity();
        //        var firebaseClient = new FirebaseClient("https://the-beacon-team-battles.firebaseio.com/");
        //        var firebase = firebaseClient.Child($"Root/Guilds/{ctx.Guild.Id}/GuildEvents");
        //        var guildEvent = new GuildEvent { GuildID = ctx.Guild.Id.ToString(), EventType = DiscordEventType.DM };
        //        var enterDate = await ctx.RespondAsync($"Please enter the date of the event in mm/dd/yyyy hh:mm PM/AM format (Time in UTC).\nExample: 01/05/2019 05:60 PM");
        //        var dateMsg = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author == ctx.Message.Author
        //                && DateTime.TryParseExact(x.Content, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date));
        //        if (dateMsg.Result != null && !dateMsg.Result.Content.ToLower().Trim().Equals("cancel")) {
        //            guildEvent.Date = dateMsg.Result.Content;
        //            var enterRoles = await ctx.RespondAsync($"What role(s) would you like to notify? (Mention it, or send their names seperated by **,**)");
        //            var roleResult = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author == ctx.Message.Author
        //                                                 && (x.MentionedRoles?.Count > 0
        //                                                 || ctx.Guild.Roles?.Any(y => x.Content.Split(",").Select(z => z.Trim()).Contains(y.Value.Name)) == true));
        //            if (roleResult.Result != null && !roleResult.Result.Content.ToLower().Trim().Equals("cancel")) {
        //                Dictionary<string, UserID> userIDs = new Dictionary<string, UserID>();
        //                var members = new List<KeyValuePair<ulong, DiscordMember>>();
        //                if (roleResult.Result.MentionedRoles?.Count > 0) {
        //                    // Mentioned the roles
        //                    members = roleResult.Result.Channel.Guild.Members.Where(x => roleResult.Result.MentionedRoles.Any(y => x.Value.Roles.Contains(y))).ToList();
        //                } else {
        //                    // Named the roles
        //                    var roles = roleResult.Result.Content.Split(",").Select(s => s.Trim()).ToList();
        //                    members = roleResult.Result.Channel.Guild.Members.Where(x => roles.Any(y => x.Value.Roles.Any(z => z.Name.Equals(y)))).ToList();
        //                }
        //                members.ForEach(x => userIDs.Add(x.Key.ToString(), new UserID { Send = true }));
        //                guildEvent.UserIDs = userIDs;
        //                var enterContent = await ctx.RespondAsync($"What message would you like me to send to them at  the specified time?");
        //                var contentResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author == ctx.Message.Author);
        //                if (contentResult.Result != null && !contentResult.Result.Content.ToLower().Trim().Equals("cancel")) {
        //                    guildEvent.MessageText = contentResult.Result.Content;
        //                    var result = await firebase.PostAsync(JsonConvert.SerializeObject(guildEvent));
        //                    await ctx.Channel.DeleteMessagesAsync(new List<DiscordMessage> { enterDate, enterRoles, enterContent });
        //                    try {
        //                        var date = DateTime.ParseExact(guildEvent.Date, "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);
        //                        await ctx.RespondAsync($"Successfully created event scheduled for {date.ToString("MMMM dd, yyyy")}");
        //                    } catch (Exception e) {
        //                        throw;
        //                    }
        //                } else {
        //                    if (roleResult.TimedOut) {
        //                        await ctx.RespondAsync($"Command timed out. Please repeat the process.");
        //                    } else {
        //                        await ctx.RespondAsync($"Cancelled current operation.");
        //                    }
        //                }
        //            } else {
        //                if (roleResult.TimedOut) {
        //                    await ctx.RespondAsync($"Command timed out. Please repeat the process.");
        //                } else {
        //                    await ctx.RespondAsync($"cancelled current operation.");
        //                }
        //            }
        //        } else {
        //            await ctx.RespondAsync($"Command timed out. Please repeat the process.");
        //        }
        //    }
        //}

        [Aliases("createevent", "createnewevent")]
        [Command("newevent")]
        public async Task CreateNewEvent(CommandContext ctx) {
            if (ctx.IsStaffMember()) {
                if (Random == null) {
                    Random = new Random();
                }
                var interactivity = ctx.Client.GetInteractivity();
                var firebaseClient = new FirebaseClient("https://the-beacon-team-battles.firebaseio.com/");
                var firebase = firebaseClient.Child($"Root/Guilds/{ctx.Guild.Id}/GuildEvents");
                var guildEvent = new GuildEvent { GuildID = ctx.Guild.Id.ToString(), EventType = DiscordEventType.DM };
                var titleSend = await ctx.RespondAsync($"Send \"cancel\" at anytime to stop this process!\n\nPlease enter the event's title.");
                var titleResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id);
                if (titleResult.Result != null && !titleResult.Result.Content.Trim().ToLower().Equals("cancel")) {
                    var title = titleResult.Result.Content;
                    var descriptionSend = await ctx.RespondAsync($"What's the event's description?");
                    var descriptionResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id);
                    if (descriptionResult.Result != null && !titleResult.Result.Content.Trim().ToLower().Equals("cancel")) {
                        var description = descriptionResult.Result.Content;
                        var channelSend = await ctx.RespondAsync($"What channel(s) would you like me to announce this in? If you would like me not to announce this " +
                        $"event in any channel, please send \"none\"");
                        var channelResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id
                                                                                    && (x.MentionedChannels.Count > 0 || x.Content.Trim().ToLower().Equals("none")
                                                                                        || x.Content.Trim().ToLower().Equals("cancel")));
                        if (channelResult.Result != null && !channelResult.Result.Content.Equals("cancel")) {
                            var channel = channelResult.Result;
                            List<DiscordChannel> channels;
                            channels = channel.MentionedChannels?.ToList();
                            var dateSend = await ctx.RespondAsync($"Please enter the date of the event in UTC, in format `mm/dd/yyyy hh:mm tt` (month/day/year hour/min PM/AM).");
                            DateTime date;
                            var dateResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id
                                                                                        && (x.Content.Trim().ToLower().Equals("cancel") ||
                                                                                          DateTime.TryParseExact(x.Content, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)));
                            if (dateResult.Result != null && !dateResult.Result.Content.ToLower().Trim().Equals("cancel")) {
                                date = DateTime.ParseExact(dateResult.Result.Content, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                                guildEvent.Date = date.Subtract(TimeSpan.FromMinutes(15)).ToString("MM/dd/yyyy hh:mm tt");
                                var rolesSend = await ctx.RespondAsync($"What role(s) would you like to notify? (Mention it, or send their names seperated by **,**)");
                                var rolesResult = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author == ctx.Message.Author
                                                                     && (x.MentionedRoles?.Count > 0
                                                                     || ctx.Guild.Roles?.Any(y => x.Content.Split(",").Select(z => z.Trim()).Contains(y.Value.Name)) == true));
                                if (rolesResult.Result != null && !rolesResult.Result.Content.ToLower().Trim().Equals("cancel")) {
                                    Dictionary<string, UserID> userIDs = new Dictionary<string, UserID>();
                                    var members = new List<KeyValuePair<ulong, DiscordMember>>();
                                    if (rolesResult.Result.MentionedRoles?.Count > 0) {
                                        // Mentioned the roles
                                        members = rolesResult.Result.Channel.Guild.Members.Where(x => rolesResult.Result.MentionedRoles.Any(y => x.Value.Roles.Contains(y))).ToList();
                                    } else {
                                        // Named the roles
                                        var roles = rolesResult.Result.Content.Split(",").Select(s => s.Trim()).ToList();
                                        members = rolesResult.Result.Channel.Guild.Members.Where(x => roles.Any(y => x.Value.Roles.Any(z => z.Name.Equals(y)))).ToList();
                                    }
                                    members.ForEach(x => userIDs.Add(x.Key.ToString(), new UserID { Send = true }));
                                    guildEvent.UserIDs = userIDs;
                                    var contentSend = await ctx.RespondAsync($"Finally, what text should I DM 15 minutes before specified time? (Send \"none\" if you don't want to)");
                                    var contentResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author == ctx.Message.Author);
                                    if (contentResult.Result != null && !contentResult.Result.Content.ToLower().Trim().Equals("cancel")) {
                                        guildEvent.MessageText = contentResult.Result.Content;
                                        await ctx.RespondAsync($"Success! JSON Result: ```json\n{JsonConvert.SerializeObject(guildEvent)}");
                                        var builder = new DiscordEmbedBuilder {
                                            Author = new DiscordEmbedBuilder.EmbedAuthor { Name = ctx.Message.Author.Username, IconUrl = ctx.Message.Author.AvatarUrl },
                                            Color = new DiscordColor("#EFCEB6"),
                                            Title = title,
                                            Description = description,
                                            ThumbnailUrl = "http://images6.fanpop.com/image/photos/36100000/Togashi-Yuuta-image-togashi-yuuta-36129216-1280-720.jpg",
                                        };
                                        builder.AddField("Date", date.ToString("MM/dd/yyyy hh:mm tt"));
                                        string mentions = "";
                                        rolesResult.Result.MentionedRoles.ToList().ForEach(x => mentions = $"{mentions}{x.Mention} ");
                                        channels.ForEach(async x => await x.SendMessageAsync(content: mentions, embed: builder.Build()));
                                        if (!contentResult.Result.Content.Trim().ToLower().Equals("none")) {
                                            await firebase.PostAsync(JsonConvert.SerializeObject(guildEvent));
                                        }
                                        await ctx.Channel.DeleteMessagesAsync(new List<DiscordMessage> { titleSend, descriptionSend, channelSend, dateSend, contentSend, rolesSend });
                                    } else {
                                        if (contentResult.TimedOut) { await ctx.RespondAsync($"Timed out, please repeat the process again."); } else { await ctx.RespondAsync($"Cancelled operation."); }
                                        await ctx.Channel.DeleteMessagesAsync(new List<DiscordMessage> { titleSend, descriptionSend, channelSend, dateSend, rolesSend, contentSend });
                                    }
                                } else {
                                    if (rolesResult.TimedOut) { await ctx.RespondAsync($"Timed out, please repeat the process again."); } else { await ctx.RespondAsync($"Cancelled operation."); }
                                    await ctx.Channel.DeleteMessagesAsync(new List<DiscordMessage> { titleSend, descriptionSend, channelSend, dateSend, rolesSend });
                                }
                            } else {
                                if (dateResult.TimedOut) { await ctx.RespondAsync($"Timed out, please repeat the process again."); } else { await ctx.RespondAsync($"Cancelled operation."); }
                                await ctx.Channel.DeleteMessagesAsync(new List<DiscordMessage> { titleSend, descriptionSend, channelSend, dateSend });
                            }
                        } else {
                            if (channelResult.TimedOut) { await ctx.RespondAsync($"Timed out, please repeat the process again."); } else { await ctx.RespondAsync($"Cancelled operation."); }
                            await ctx.Channel.DeleteMessagesAsync(new List<DiscordMessage> { titleSend, descriptionSend, channelSend });
                        }
                    } else {
                        if (descriptionResult.TimedOut) { await ctx.RespondAsync($"Timed out, please repeat the process again."); } else { await ctx.RespondAsync($"Cancelled operation."); }
                        await ctx.Channel.DeleteMessagesAsync(new List<DiscordMessage> { titleSend, descriptionSend });
                    }
                } else {
                    if (titleResult.TimedOut) { await ctx.RespondAsync($"Timed out, please repeat the process again."); } else { await ctx.RespondAsync($"Cancelled operation."); }
                    await titleSend.DeleteAsync();
                }
            }
        }
    }
}