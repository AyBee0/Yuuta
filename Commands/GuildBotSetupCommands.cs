using AuthorityHelpers;
using FirebaseHelper;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Types;
using static Commands.CommandUtils;
using static Types.EventType;

namespace Commands {

    public class GuildBotSetupCommands : BaseCommandModule {

        //TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD
        //TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD
        //TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD
        //TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD
        //TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD
        //TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD
        //TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD
        //TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD
        //TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD
        //TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD
        //TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD
        //TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD - TERRIBLE CODE AHEAD
        private static Random Random;
        private static YuutaFirebaseClient FirebaseClient;

        [Description("Bot will walk you through the process of creating a new server event.")]
        [Aliases("createevent", "createnewevent")]
        [Command("newevent")]
        public async Task CreateNewEvent(CommandContext ctx) {
            if (ctx.IsStaffMember()) {
                var interactivityTimeout = TimeSpan.FromMinutes(2);
                if (Random == null) {
                    Random = new Random();
                }
                var interactivity = ctx.Client.GetInteractivity();
                FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                var guildEvent = new GuildEvent { GuildID = ctx.Guild.Id.ToString(), EventType = DiscordEventType.DM };
                var titleSend = await ctx.RespondAsync($"Send \"cancel\" at anytime to stop this process!\n\n**Please enter the event's title.**");
                var titleResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id, interactivityTimeout);
                if (titleResult.Result != null && !titleResult.Result.Content.Trim().ToLower().Equals("cancel")) {
                    var title = titleResult.Result.Content;
                    var descriptionSend = await ctx.RespondAsync($"**What's the event's description?**");
                    var descriptionResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id, interactivityTimeout);
                    if (descriptionResult.Result != null && !titleResult.Result.Content.Trim().ToLower().Equals("cancel")) {
                        var description = descriptionResult.Result.Content;
                        var channelSend = await ctx.RespondAsync($"**What channel(s) would you like me to announce this in? Mention them below (e.g: #channel1 #channel2)**");
                        var channelResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id && x.MentionedChannels.Count > 0, interactivityTimeout);
                        if (channelResult.Result != null && !channelResult.Result.Content.Equals("cancel")) {
                            var channel = channelResult.Result;
                            List<DiscordChannel> channels;
                            channels = channel.MentionedChannels?.ToList();
                            var dateSend = await ctx.RespondAsync($"**Please enter the date of the event in __UTC__, like the following: `2019-09-27 5:00PM` where `09` represents the current month.**" +
                                $"\n\n> *Hint: If you really can't enter the date in UTC, just indicate how many hours ahead/behind of UTC you are. E.g: `2019-9-27 3:00PM -2` if you're 2 hours behind UTC. Just send it in utc pls thx i didn't test this.*");
                            DateTime date;
                            var dateResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id
                                                                                        && (x.Content.Trim().ToLower().Equals("cancel") ||
                                                                                          DateTime.TryParse(x.Content, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                                                                                          , interactivityTimeout);
                            if (dateResult.Result != null && !dateResult.Result.Content.ToLower().Trim().Equals("cancel")) {
                                date = DateTime.Parse(dateResult.Result.Content).ToUniversalTime();
                                guildEvent.Date = date.Subtract(TimeSpan.FromMinutes(15)).ToString();
                                var rolesSend = await ctx.RespondAsync($"**What role(s) would you like to notify? (Mention it, or send their names seperated by a \",\", or send \"none\" if you don't want to ping any role.)**");
                                var rolesResult = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author == ctx.Message.Author
                                                                     && (x.MentionedRoles?.Count > 0
                                                                     || ctx.Guild.Roles?.Any(y => x.Content.Split(",").Select(z => z.Trim()).Contains(y.Value.Name)) == true || x.Content.Trim().ToLower().Equals("none"))
                                                                     , interactivityTimeout);
                                if (rolesResult.Result != null && !rolesResult.Result.Content.ToLower().Trim().Equals("cancel")) {
                                    var contentSend = await ctx.RespondAsync($"**Finally, what text should I DM 15 minutes before specified time if they want to participate? (Send \"none\" if you don't want to)**");
                                    var contentResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author == ctx.Message.Author, interactivityTimeout);
                                    if (contentResult.Result != null && !contentResult.Result.Content.ToLower().Trim().Equals("cancel")) {
                                        guildEvent.MessageText = contentResult.Result.Content;
                                        var builder = new DiscordEmbedBuilder {
                                            Author = new DiscordEmbedBuilder.EmbedAuthor { Name = ctx.Message.Author.Username, IconUrl = ctx.Message.Author.AvatarUrl },
                                            Color = new DiscordColor("#EFCEB6"),
                                            Title = title,
                                            Description = description,
                                            ThumbnailUrl = "http://images6.fanpop.com/image/photos/36100000/Togashi-Yuuta-image-togashi-yuuta-36129216-1280-720.jpg",
                                        };
                                        builder.AddField("Date", date.ToString());
                                        string mentions = "";
                                        rolesResult.Result.MentionedRoles.ToList().ForEach(x => mentions = $"{mentions}{x.Mention} ");
                                        var eventMessages = new Dictionary<string, EventMessage>();
                                        foreach (var channelToSendAndCheck in channels) {
                                            var message = await channelToSendAndCheck.SendMessageAsync(content: mentions, embed: builder.Build());
                                            if (!contentResult.Result.Content.Trim().ToLower().Equals("none")) {
                                                var checkmarkEmoji = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
                                                eventMessages.Add(message.Id.ToString(),
                                                    new EventMessage {
                                                        EmojiID = checkmarkEmoji.Id,
                                                        MessageId = message.Id,
                                                        ReminderMessage = contentResult.Result.Content,
                                                        ChannelId = message.ChannelId,
                                                        EmojiName = checkmarkEmoji.Name
                                                    });
                                                await message.CreateReactionAsync(checkmarkEmoji);
                                            }
                                        }
                                        if (!contentResult.Result.Content.Trim().ToLower().Equals("none")) {
                                            guildEvent.EventMessages = eventMessages;
                                            //var firebase = FirebaseClient.Child($"Root/Guilds/{ctx.Guild.Id}/GuildEvents");
                                            await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id.ToString()).Child("GuildEvents").PushValueAsync(JsonConvert.SerializeObject(guildEvent));
                                        }
                                        await ctx.Channel.DeleteMessagesAsync(new List<DiscordMessage>
                                        {
                                            titleSend, titleResult.Result, descriptionSend, descriptionResult.Result, channelSend, channelResult.Result,
                                            dateSend, dateResult.Result, contentSend, contentResult.Result, rolesSend, rolesResult.Result,
                                        });
                                    } else {
                                        if (contentResult.TimedOut) { await ctx.RespondAsync($"Timed out, please repeat the process again."); } else { await ctx.RespondAsync($"Cancelled operation."); }
                                        await ctx.Channel.DeleteMessagesAsync(new List<DiscordMessage> {
                                            titleSend, titleResult.Result, descriptionSend, descriptionResult.Result, channelSend, channelResult.Result,
                                            dateSend, dateResult.Result, contentSend, contentResult.Result });
                                    }
                                } else {
                                    if (rolesResult.TimedOut) { await ctx.RespondAsync($"Timed out, please repeat the process again."); } else { await ctx.RespondAsync($"Cancelled operation."); }
                                    await ctx.Channel.DeleteMessagesAsync(new List<DiscordMessage> { titleSend, titleResult.Result, descriptionSend, descriptionResult.Result, channelSend, channelResult.Result,
                                            dateSend, dateResult.Result, rolesSend, rolesResult.Result, });
                                }
                            } else {
                                if (dateResult.TimedOut) { await ctx.RespondAsync($"Timed out, please repeat the process again."); } else { await ctx.RespondAsync($"Cancelled operation."); }
                                await ctx.Channel.DeleteMessagesAsync(new List<DiscordMessage> { titleSend, titleResult.Result, descriptionSend, descriptionResult.Result, channelSend, channelResult.Result, dateSend, dateResult.Result });
                            }
                        } else {
                            if (channelResult.TimedOut) { await ctx.RespondAsync($"Timed out, please repeat the process again."); } else { await ctx.RespondAsync($"Cancelled operation."); }
                            await ctx.Channel.DeleteMessagesAsync(new List<DiscordMessage> {
                                titleSend, titleResult.Result, descriptionSend, descriptionResult.Result, channelSend, channelResult.Result,
                            });
                        }
                    } else {
                        if (descriptionResult.TimedOut) { await ctx.RespondAsync($"Timed out, please repeat the process again."); } else { await ctx.RespondAsync($"Cancelled operation."); }
                        await ctx.Channel.DeleteMessagesAsync(new List<DiscordMessage> {
                            titleSend, titleResult.Result, descriptionSend, descriptionResult.Result
                        });
                    }
                } else {
                    List<DiscordMessage> messagesToDelete = new List<DiscordMessage> { titleSend, titleResult.Result };
                    if (titleResult.TimedOut) {
                        await ctx.RespondAsync($"Timed out, please repeat the process again.");
                    } else { await ctx.RespondAsync($"Cancelled operation."); }
                    //await titleSend.DeleteAsync();
                    await ctx.Channel.DeleteMessagesAsync(messagesToDelete);
                }
                //I KNOW THIS IF STATEMENT ERROR HANDLING IS HORRIBLE SHUT UP IM LAZY
            }
            await ctx.Message.DeleteAsync();
        }

        [Description("Creates a new server macro that deletes the command when sent. First argument is the command (without prefix) and everything after that will be the response. Attachments will work.")]
        [Aliases("createmacro", "creaccmacc")]
        [Command("newmacro")]
        public async Task CreateNewServerMacro(CommandContext ctx, string macro, [RemainingText] string response) {
            await Task.Run(() => NewMacro(ctx, macro, response, true));
        }

        [Description("Creates a new server macro that doesn't delete the command when sent. First argument is the command (without prefix) and everything after that will be the response. Attachments will work.")]
        [Aliases("createmacronodelete", "createanewdamnmacrothatsomeonewillwastetheirtimetoexecutekillmeplsnodelete")]
        [Command("newmacronodelete")]
        public async Task CreateNewServerMacroNoDelete(CommandContext ctx, string macro, [RemainingText] string response) {
            await Task.Run(() => NewMacro(ctx, macro, response, false));
        }

        private async void NewMacro(CommandContext ctx, string macro, string response, bool deleteCommand) {
            FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
            //var firebase = FirebaseClient.Child($"Root/Guilds/{ctx.Guild.Id}/GuildMacros");
            var guildMacro = new GuildMacro { Macro = Guild.MacroPrefix + macro, MessageResponse = response, DeleteCommand = deleteCommand };
            guildMacro.Attachments = new Dictionary<string, Attachment>();
            var messageAttachments = ctx.Message.Attachments;
            if (messageAttachments.Count > 0) {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                messageAttachments.ToList().ForEach(x => guildMacro.Attachments.Add(new string(Enumerable.Repeat(chars, 12).Select(s => s[Random.Next(chars.Length)]).ToArray()),
                    new Attachment { AttachmentURL = x.Url }));
            }
            await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id).Child("GuildMacros").PushValueAsync(guildMacro);
            await ctx.RespondAsync($":white_check_mark: Created new guild macro {macro}!");
        }

        [Description("Creates a new message where when a user clicks on a certain raction, it gives him a certain role. I will walk you through the process.")]
        [Aliases("newreactionmessage", "newreactionmsg", "createreactionmessage", "newrxnmsg", "reactionmessage")]
        [Command("newreactionrolemessage")]
        public async Task CreateNewReactionRoleMessage(CommandContext ctx) {
            var interactivityTime = TimeSpan.FromMinutes(2);
            List<DiscordMessage> sentMessages = new List<DiscordMessage>();
            var askChannel = await ctx.RespondAsync($"*Send \"cancel\" anytime to cancel this operation.*\n\n" +
                $"**Please mention the channel where you'd like this message to be.**");
            sentMessages.Add(askChannel);
            var interactivity = ctx.Client.GetInteractivity();
            var channelSent = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id && x.MentionedChannels.Count == 1, interactivityTime);
            if (channelSent.Result != null && !channelSent.Result.Content.Trim().ToLower().Equals("cancel")) {
                sentMessages.Add(channelSent.Result);
                var channel = channelSent.Result.MentionedChannels[0];
                var askCategory = await ctx.RespondAsync($"**What would be the category of this message? (i.e \"Game Roles\")**");
                sentMessages.Add(askCategory);
                var categorySent = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id);
                if (categorySent.Result != null && !categorySent.Result.Content.Trim().ToLower().Equals("cancel")) {
                    var askReactionAndRoles = await ctx.RespondAsync($"Ok so here's the kinda hard part. I tried to make this easy ok hecc off\n\n**Enter the reaction and role name(s)/ping(s) following the example below:**" +
                    $"```yaml\n:emoji: - Role - Description of this role\n\n:pepega: - @Pepega - Gives the Pepega Role\n\n:overwatch: - Overwatch - Gives the Overwatch Role (Yes btw you don't have to ping the role cool right)\n\n:rainbowsixseige: - Rainbow Six Seige, @Rainbow Six Seige Comp - Gives both Rainbow Six Seige and Rainbow Six Seige Comp roles. Seperate roles by commas. Please.```	");
                    sentMessages.Add(askReactionAndRoles);
                    var reactionAndRolesSent = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id, interactivityTime);
                    if (reactionAndRolesSent.Result != null && !reactionAndRolesSent.Result.Content.Trim().ToLower().Equals("cancel")) {
                        sentMessages.Add(reactionAndRolesSent.Result);
                        var result = reactionAndRolesSent.Result.Content;
                        result = Regex.Replace(result, @"(\r\n)+", "\r\n\r\n", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                        var inputs = new List<string>(result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                        var abUser = await ctx.Client.GetUserAsync(247386254499381250);
                        var embedBuilder = new DiscordEmbedBuilder {
                            Title = categorySent.Result.Content,
                            Description = "**Clicking on a reaction (emoji at the bottom of __this message__) will give you that emoji's role. For a list of what emoji gives what role, refer below:**",
                            Author = new DiscordEmbedBuilder.EmbedAuthor { Name = "Yuutabot - Developed by Ab", IconUrl = abUser.AvatarUrl },
                            Color = new DiscordColor("#EFCEB6"),
                        };
                        Dictionary<string, ReactionEmoji> emojiAndRoles = new Dictionary<string, ReactionEmoji>();
                        var index = 0;
                        foreach (var input in inputs) {
                            var splitInput = input.Split(" - ").Select(x => x.Trim()).ToList();
                            var splitRoles = splitInput[1].Split(",").Select(x => x.Trim()).ToList();
                            DiscordEmoji emoji;
                            emoji = ParseDiscordEmoji(ctx.Client, splitInput[0]);
                            var roles = ctx.Guild.Roles.Values.Where(role => splitRoles.Contains(role.Mention) | splitRoles.Contains(role.Name)).ToList();
                            if (emoji == null || roles == null || roles.Count < 1 || string.IsNullOrWhiteSpace(splitInput[2])) {
                                await ctx.RespondAsync($"Error in your formatting at line {index + 1}.");
                                continue;
                            }
                            embedBuilder.AddField($"{emoji.GetEmbedFriendlyEmojiName()} - {string.Join(",", roles.Select(x => x.Name))}", splitInput[2], true);
                            emojiAndRoles.Add(Guid.NewGuid().ToString(), new ReactionEmoji {
                                EmojiName = emoji.RequiresColons ? $"{emoji.GetDiscordName()}" : emoji.GetDiscordName(),
                                Emoji = emoji,
                                Description = splitInput[2].Trim(),
                                RoleIds = roles.Select(x => x.Id).ToList()
                            });
                            index++;
                        }
                        var message = await channel.SendMessageAsync(embed: embedBuilder.Build());
                        if (message.Embeds?.Any() == true) {
                            embedBuilder.Author = new DiscordEmbedBuilder.EmbedAuthor { Name = $"Yuutabot - Developed by Ab | {message.Id}", IconUrl = abUser.AvatarUrl };
                            await message.ModifyAsync(embed: embedBuilder.Build());
                        }
                        FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                        await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id).Child("ReactionMessages").Child(message.Id).Child("Emojis").UpdateValueAsync(emojiAndRoles);
                        foreach (var reactionEmoji in emojiAndRoles) {
                            try {
                                await message.CreateReactionAsync(reactionEmoji.Value.Emoji);
                                await Task.Delay(700);
                            } catch (Exception) {
                                await ctx.RespondAsync($"Welp\nOne of your emojis are invalid.; couldn't create a reaction from it. Do I have access to this emoji? I have to be in a server where this emoji exists btw.");
                            }
                        }
                    } else {
                        if (reactionAndRolesSent.TimedOut) { await ctx.RespondAsync($"Timed out. Please try again."); } else { await ctx.RespondAsync($"Cancelled the operation."); }
                    }
                } else {
                    if (categorySent.TimedOut) { await ctx.RespondAsync($"Timed out. Please try again."); } else { await ctx.RespondAsync($"Cancelled the operation."); }
                }
            } else {
                if (channelSent.TimedOut) { await ctx.RespondAsync($"Timed out. Please try again."); } else { await ctx.RespondAsync($"Cancelled the operation."); }
            }
            //I KNOW THIS IF STATEMENT ERROR HANDLING IS HORRIBLE SHUT UP IM LAZY
        }
    }
}
