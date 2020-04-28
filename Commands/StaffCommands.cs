using AuthorityHelpers;
using DiscordExtensions;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity;
using FirebaseHelper;
using InteractivityHelpers;
using RestSharp;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Types;
using Types.DatabaseObjects;
using static DiscordExtensions.EmojiUtils;
using static FirebaseHelper.YuutaFirebaseClient;
using static InteractivityHelpers.InteractivityEventTracker;
using static PathUtils.PathUtils;
using static System.IO.Path;
using static Types.EventType;

namespace Commands
{
    public class StaffCommands : BaseCommandModule
    {

        private Random Random;
        private YuutaFirebaseClient FirebaseClient;

        [Description("Bot will walk you through the process of creating a new server event.")]
        [Aliases("createevent", "createnewevent")]
        [Command("newevent")]
        public async Task CreateNewEvent(CommandContext ctx)
        {
            if (!ctx.IsStaffMember() && !ctx.Member.Roles.Any(x => x.Id == 609714765723336715 || x.Id == 631591805904748555))
            {
                return;
            }
            var tracker = new InteractivityEventTracker(ctx, TimeSpan.FromMinutes(5));
            FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
            var embedBuilder = new DiscordEmbedBuilder
            {
                Color = new Optional<DiscordColor>(new DiscordColor("#EFCEB6")),
                Author = new DiscordEmbedBuilder.EmbedAuthor { Name = ctx.Message.Author.Username, IconUrl = ctx.Message.Author.AvatarUrl },
                ThumbnailUrl = "http://images6.fanpop.com/image/photos/36100000/Togashi-Yuuta-image-togashi-yuuta-36129216-1280-720.jpg"
            };
            var askTitle = await tracker.AskAndWaitForResponseAsync("**What's the title of the event?**", sendCancelNotice: true);
            DiscordChannel channel = null;
            string mentions = null;
            string reminderMessage = null;
            var guildEvent = new GuildEvent
            {
                GuildID = ctx.Guild.Id.ToString(),
                EventType = DiscordEventType.DM,
                ReactionEventMessage = new Dictionary<string, EventMessage>()
            };
            if (tracker.Status == InteractivityStatus.OK)
            {
                embedBuilder.Title = askTitle.Result.Content;
                var askDescription = await tracker.AskAndWaitForResponseAsync("**What's this event's description?**");
                if (tracker.Status == InteractivityStatus.OK)
                {
                    embedBuilder.Description = askDescription.Result.Content;
                    var guildRoles = ctx.Guild.Roles.Values.Select(guildRole => guildRole.Name.Trim().ToLower()).ToList();
                    var askPing =
                        await tracker
                            .AskAndWaitForResponseAsync(
                                $"**What role(s) would you like me to ping?**\n" +
                                    $"Mention them (or just send their names) below, and seperate the roles by commas. Send \"everyone\" to mention `@everyone`.\n\n" +
                                        $"*To not ping anyone, send \"none\"."
                                , tracker.InteractivityConditions.ValidRoleCondition);
                    if (tracker.Status == InteractivityStatus.OK)
                    {
                        try
                        {
                            var parsedRoles = askPing.ParseSentRoles();
                            mentions = string.Join(" ", parsedRoles.Select(x => x.Mention));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.StackTrace);
                            throw;
                        }
                        var askDmMessage = await tracker.AskAndWaitForResponseAsync($"**If you'd like to DM users who'd like to participate in this event 15 minutes before the event starts**," +
                            $" what would you want to DM them? Send \"none\" if you don't want to.",
                            acceptNone: true);
                        if (tracker.Status == InteractivityStatus.OK)
                        {
                            reminderMessage = askDmMessage.Result.Content.ToLower().Trim() == "none" ? null : askDmMessage.Result.Content;
                            guildEvent.MessageText = reminderMessage;
                            var askDate = await tracker.AskAndWaitForResponseAsync($"**What's the date of the event?** Send it below in the format, __**IN UTC. USE GOOGLE TO CONVERT**__:\n" +
                                $"`2019/1/25 5:00PM`. (`1` is the month, `25` is the day.)\n\n" +
                                $"> You can also just `mm/dd/yyyy` but I'm willing to bet every single kidney cell I have left in my body someone's going to `dd/mm/yyyy`"
                                , tracker.InteractivityConditions.DateCondition);
                            if (tracker.Status == InteractivityStatus.OK)
                            {
                                var date = DateTime.Parse(askDate.Result.Content).ToUniversalTime();
                                guildEvent.Date = date.AddMinutes(-15).ToString("o");
                                embedBuilder.AddField("Date", date.ToString("dddd, dd MMMM yyyy hh:mm tt UTC"));
                                // ITS ALMO.ST
                                var askItsAlmost = await tracker.AskAndWaitForResponseAsync($"Create a countdown timer at this link: <https://itsalmo.st> and send the URL here." +
                                    $"\n*You can skip this by saying `none` but please don't unless necessary.*", acceptNone: true);
                                if (tracker.Status == InteractivityStatus.OK)
                                {
                                    var itsalmosturl = askItsAlmost.Result.Content;
                                    if (itsalmosturl.Trim().ToLower() != "none")
                                    {
                                        embedBuilder.AddField("Countdown Link", itsalmosturl);
                                        embedBuilder.Author.Url = itsalmosturl;
                                    }
                                    var askImage = await tracker.AskAndWaitForResponseAsync("**Would you like to have an image/thumbnail for the event?**" +
                                    " If so, please send it as an attachment (NOT the url). Otherwise, send \"none\"", tracker.InteractivityConditions.ImageCondition, acceptNone: true);
                                    if (tracker.Status == InteractivityStatus.OK)
                                    {
                                        if (askImage.Result.Attachments?.Count > 0)
                                        {
                                            embedBuilder.ImageUrl = askImage.Result.Attachments[0].Url;
                                        }
                                        while (true)
                                        {
                                            List<Permissions> insufficientPermissions = new List<Permissions>(); ;
                                            var askChannel = await tracker.AskAndWaitForResponseAsync($"**Finally, what channel should I announce this in?**",
                                            tracker.InteractivityConditions.ChannelCondition);
                                            if (tracker.Status == InteractivityStatus.OK)
                                            {
                                                var givenChannel = askChannel.Result.MentionedChannels[0];
                                                if (givenChannel.PermissionsFor(ctx.Guild.CurrentMember)
                                                    .HasPermissions(out insufficientPermissions, Permissions.ReadMessageHistory, Permissions.SendMessages, Permissions.AddReactions))
                                                {
                                                    channel = givenChannel;
                                                    tracker.SetFinished();
                                                    break;
                                                }
                                                else
                                                {
                                                    await tracker.SendMessageResponse($"I have an insufficient permission(s) in that channel: " +
                                                        $"`{string.Join(", ", insufficientPermissions.Select(x => x.ToPermissionString()))}` Please try again.");
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        //var askChannel = await tracker.AskAndWaitForResponseAsync($"**Finally, what channel should I announce this in?**",
                                        //tracker.InteractivityConditions.ChannelCondition);
                                        //if (tracker.Status == InteractivityStatus.OK) {
                                        //    channel = askChannel.Result.MentionedChannels[0];
                                        //    tracker.SetFinished();
                                        //}
                                    }
                                }
                            }
                        }
                    }
                }
            }
            switch (tracker.Status)
            {
                case InteractivityStatus.Cancelled:
                    await tracker.SendMessageResponse(":white_check_mark: Cancelled successfully.");
                    break;
                case InteractivityStatus.TimedOut:
                    await tracker.SendMessageResponse(":x: Timed out. Please try again.");
                    break;
                case InteractivityStatus.Finished:
                case InteractivityStatus.OK:
                    var eventMessage = await channel.SendMessageAsync(mentions + $" To be reminded 15 minutes before this event, please click" +
                        $"on the :white_check_mark:!", embed: embedBuilder.Build());
                    await eventMessage.CreateReactionAsync(ParseDiscordEmoji(ctx.Client, ":white_check_mark:"));
                    guildEvent.ReactionEventMessage.Add(eventMessage.Id.ToString(), new EventMessage
                    {
                        ChannelId = eventMessage.ChannelId.ToString(),
                        EmojiID = 0.ToString(),
                        EmojiName = "✅",
                        ReminderMessage = reminderMessage,
                        MessageId = eventMessage.Id.ToString()
                    });
                    await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id.ToString()).Child("GuildEvents").PushValueAsync(guildEvent);
                    await tracker.SendMessageResponse(":white_check_mark: Successfully created a new event.", false);
                    break;
                default:
                    break;
            }
            await tracker.DeleteMessagesAsync();
        }

        [Description("Creates a new message where when a user clicks on a certain raction, it gives him a certain role. I will walk you through the process.")]
        [Aliases("newreactionmessage", "newreactionmsg", "createreactionmessage", "newrxnmsg", "reactionmessage")]
        [Command("newreactionrolemessage")]
        public async Task CreateNewReactionRoleMessage(CommandContext ctx)
        {
            var interactivityTime = TimeSpan.FromMinutes(2);
            List<DiscordMessage> sentMessages = new List<DiscordMessage>();
            var askChannel = await ctx.RespondAsync($"*Send \"cancel\" anytime to cancel this operation.*\n\n" +
                $"**Please mention the channel where you'd like this message to be.**");
            sentMessages.Add(askChannel);
            var interactivity = ctx.Client.GetInteractivity();
            var channelSent = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id && x.MentionedChannels.Count == 1, interactivityTime);
            if (channelSent.Result != null && !channelSent.Result.Content.Trim().ToLower().Equals("cancel"))
            {
                sentMessages.Add(channelSent.Result);
                var channel = channelSent.Result.MentionedChannels[0];
                var askCategory = await ctx.RespondAsync($"**What would be the category of this message? (i.e \"Game Roles\")**");
                sentMessages.Add(askCategory);
                var categorySent = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id);
                if (categorySent.Result != null && !categorySent.Result.Content.Trim().ToLower().Equals("cancel"))
                {
                    var askReactionAndRoles = await ctx.RespondAsync($"Ok so here's the kinda hard part. I tried to make this easy ok hecc off\n\n**Enter the reaction and role name(s)/ping(s) following the example below:**" +
                    $"```yaml\n:emoji: - Role - Description of this role\n\n:pepega: - @Pepega - Gives the Pepega Role\n\n:overwatch: - Overwatch - Gives the Overwatch Role (Yes btw you don't have to ping the role cool right)\n\n:rainbowsixseige: - Rainbow Six Seige, @Rainbow Six Seige Comp - Gives both Rainbow Six Seige and Rainbow Six Seige Comp roles. Seperate roles by commas. Please.```");
                    sentMessages.Add(askReactionAndRoles);
                    var reactionAndRolesSent = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id && x.ChannelId == ctx.Channel.Id, interactivityTime);
                    if (reactionAndRolesSent.Result != null && !reactionAndRolesSent.Result.Content.Trim().ToLower().Equals("cancel"))
                    {
                        sentMessages.Add(reactionAndRolesSent.Result);
                        var result = reactionAndRolesSent.Result.Content;
                        result = Regex.Replace(result, @"(\r\n)+", "\r\n\r\n", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                        var inputs = new List<string>(result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                        var abUser = await ctx.Client.GetUserAsync(247386254499381250);
                        var embedBuilder = new DiscordEmbedBuilder
                        {
                            Title = categorySent.Result.Content,
                            Description = "**Clicking on a reaction (emoji at the bottom of __this message__) will give you that emoji's role. For a list of what emoji gives what role, refer below:**",
                            Author = new DiscordEmbedBuilder.EmbedAuthor { Name = "Yuutabot - Developed by Ab", IconUrl = abUser.AvatarUrl },
                            Color = new DiscordColor("#EFCEB6"),
                        };
                        Dictionary<string, ReactionEmoji> emojiAndRoles = new Dictionary<string, ReactionEmoji>();
                        var index = 0;
                        foreach (var input in inputs)
                        {
                            var splitInput = input.Split(" - ").Select(x => x.Trim()).ToList();
                            var splitRoles = splitInput[1].Split(",").Select(x => x.Trim()).ToList();
                            DiscordEmoji emoji;
                            emoji = ParseDiscordEmoji(ctx.Client, splitInput[0]);
                            var roles = ctx.Guild.Roles.Values.Where(role => splitRoles.Contains(role.Mention) | splitRoles.Contains(role.Name)).ToList();
                            if (emoji == null || roles == null || roles.Count < 1 || string.IsNullOrWhiteSpace(splitInput[2]))
                            {
                                await ctx.RespondAsync($"Error in your formatting at line {index + 1}.");
                                continue;
                            }
                            embedBuilder.AddField($"{emoji.GetEmbedFriendlyEmojiName()} - {string.Join(",", roles.Select(x => x.Name))}", splitInput[2], true);
                            emojiAndRoles.Add(Guid.NewGuid().ToString(), new ReactionEmoji
                            {
                                EmojiName = emoji.RequiresColons ? $"{emoji.GetDiscordName()}" : emoji.GetDiscordName(),
                                Emoji = emoji,
                                Description = splitInput[2].Trim(),
                                RoleIds = roles.Select(x => x.Id).ToList()
                            });
                            index++;
                        }
                        var message = await channel.SendMessageAsync(embed: embedBuilder.Build());
                        if (message.Embeds?.Any() == true)
                        {
                            embedBuilder.Author = new DiscordEmbedBuilder.EmbedAuthor { Name = $"Yuutabot - Developed by Ab | {message.Id}", IconUrl = abUser.AvatarUrl };
                            await message.ModifyAsync(embed: embedBuilder.Build());
                        }
                        FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                        var reactionMessage = new ReactionMessage { ChannelId = channel.Id.ToString(), Emojis = emojiAndRoles };
                        await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id).Child("ReactionMessages").Child(message.Id).UpdateValueAsync(reactionMessage);
                        foreach (var reactionEmoji in emojiAndRoles)
                        {
                            try
                            {
                                await message.CreateReactionAsync(reactionEmoji.Value.Emoji);
                                await Task.Delay(700);
                            }
                            catch (Exception)
                            {
                                await ctx.RespondAsync($"Welp\nOne of your emojis are invalid.; couldn't create a reaction from it. Do I have access to this emoji? I have to be in a server where this emoji exists btw.");
                            }
                        }
                    }
                    else
                    {
                        if (reactionAndRolesSent.TimedOut) { await ctx.RespondAsync($"Timed out. Please try again."); } else { await ctx.RespondAsync($"Cancelled the operation."); }
                    }
                }
                else
                {
                    if (categorySent.TimedOut) { await ctx.RespondAsync($"Timed out. Please try again."); } else { await ctx.RespondAsync($"Cancelled the operation."); }
                }
            }
            else
            {
                if (channelSent.TimedOut) { await ctx.RespondAsync($"Timed out. Please try again."); } else { await ctx.RespondAsync($"Cancelled the operation."); }
            }
            //I KNOW IT'S AWFUL I JUST HAVE NO SHAME AND AM LAZY SEND YOUR COMPLAINTS TO THE TRASH CAN OR A PULL REQUEST
        }

        [Description("Creates a new server macro that deletes the command when sent. First argument is the command (without prefix) and everything after that will be the response. Attachments will work.")]
        [Aliases("createmacro", "creaccmacc")]
        [Command("newmacro")]
        public async Task CreateNewServerMacro(CommandContext ctx, [Description("macro invoker without prefix")] string macro, [Description("Macro response")] [RemainingText] string response)
        {
            await NewMacro(ctx, macro, response, true);
        }

        [Description("Creates a new server macro that doesn't delete the command when sent. First argument is the command (without prefix) and everything after that will be the response. Attachments will work.")]
        [Aliases("createmacronodelete", "newsticker")]
        [Command("newmacronodelete")]
        public async Task CreateNewServerMacroNoDelete(CommandContext ctx, [Description("macro invoker without prefix")] string macro, [Description("Macro response")] [RemainingText] string response)
        {
            await Task.Run(() => NewMacro(ctx, macro, response, false));
        }

        private async Task NewMacro(CommandContext ctx, string macro, string response, bool deleteCommand)
        {
            if (ctx.IsStaffMember())
            {
                FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                //var firebase = FirebaseClient.Child($"Root/Guilds/{ctx.Guild.Id}/GuildMacros");
                var guildMacro = new GuildMacro { Macro = Guild.MacroPrefix + macro, MessageResponse = response, DeleteCommand = deleteCommand };
                guildMacro.Attachments = new Dictionary<string, Types.DiscordAttachment>();
                var messageAttachments = ctx.Message.Attachments;
                if (messageAttachments.Count > 0)
                {
                    //var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    //messageAttachments.ToList().ForEach(x => guildMacro.Attachments.Add(new string(Enumerable.Repeat(chars, 12).Select(s => s[Random.Next(chars.Length)]).ToArray()),
                    //    new Types.DiscordAttachment { AttachmentURL = x.Url }));
                    foreach (var attachment in messageAttachments)
                    {
                        var attachmentID = Guid.NewGuid().ToString();
                        string path = ConvertToSystemPath($"{Environment.CurrentDirectory}/Guilds/{ctx.Guild.Id}/Macros/{attachmentID}");
                        Directory.CreateDirectory(path);
                        guildMacro.Attachments.Add(Guid.NewGuid().ToString(), new Types.DiscordAttachment { AttachmentURL = attachment.Url });
                    }
                }
                await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id).Child("GuildMacros").PushValueAsync(guildMacro);
                await ctx.RespondAsync($":white_check_mark: Created new guild macro {macro}!");
            }
        }

        [Aliases("delmacro")]
        [Command("deletemacro")]
        public async Task DelMacro(CommandContext ctx, [Description("Macro to delete.")] string macroname)
        {
            macroname = macroname[0] == '.' ? macroname : '.' + macroname;
            if (Database?.Guilds?.GetValueOrDefault(ctx.Guild.Id.ToString())?.GuildMacros?.Any(macro => macro.Value.Macro == macroname) == true)
            {
                FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                var keys = Database?.Guilds?.GetValueOrDefault(ctx.Guild.Id.ToString())?.GuildMacros?.Where(macro => macro.Value.Macro == macroname).ToList();
                var newGuildMacros = Database.Guilds[ctx.Guild.Id.ToString()].GuildMacros;
                keys.ForEach(x => newGuildMacros.Remove(x.Key));
                await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id).Child("GuildMacros").SetValueAsync(newGuildMacros);
                await ctx.RespondAsync($":white_check_mark: Successfully deleted macro `{macroname}");
            }
            else
            {
                await ctx.RespondAsync($":x: Couldn't find macro `{macroname}`");
            }
        }

        [Description("Readds all emojis to the specified reaction message you created using ~newreactionmessage. The ID is specified at the top of the reaction message.")]
        [Command("readdreactions")]
        public async Task ReaddReactions(CommandContext ctx, string reactionMessageId)
        {
            //Before you ask me why I made the messageId parameter a string and not a ulong, it's because I want to tell the user he's doing something wrong,
            //instead of just ignoring the invalid command. If an invalid ulong is passed, DSharp+ doesn't execute this command at all.
            if (ctx.IsStaffMember())
            {
                if (ulong.TryParse(reactionMessageId.Trim(), out ulong messageId) && Database.Guilds[ctx.Guild.Id.ToString()].ReactionMessages.ContainsKey(reactionMessageId.ToString()))
                {
                    var reactionMessageObj = Database.Guilds[ctx.Guild.Id.ToString()].ReactionMessages[reactionMessageId.ToString()];
                    var reactionMessage = await ctx.Guild.Channels[ulong.Parse(reactionMessageObj.ChannelId)].GetMessageAsync(messageId);
                    await ctx.RespondAsync($":white_check_mark: Will do but ratelimits hurt so this'll take a while; have a covfefe.");
                    foreach (var emojiKeyPair in reactionMessageObj.Emojis)
                    {
                        DiscordEmoji emoji = ParseDiscordEmoji(ctx.Client, emojiKeyPair.Value.EmojiName);
                        await reactionMessage.CreateReactionAsync(emoji);
                        await Task.Delay(3000);
                    }
                }
                else
                {
                    string path = $"{Environment.CurrentDirectory + PathSplitter}Files{PathSplitter}messageidinfo.png";
                    using (FileStream fs = File.OpenRead(path))
                    {
                        await ctx.RespondWithFileAsync(fs, $":x: Message of ID {reactionMessageId} is not a reaction message ID. Are you getting it correctly, boi? Look below. Very ez. I tri.");
                    }
                }
            }
        }

        [RequirePermissions(Permissions.ManageMessages)]
        [Description("Clears a given amount of messages.")]
        [Command("clear")]
        public async Task Clear(CommandContext ctx, [Description("The number of messages to delete")] int numberOfMessages, [Description("(Optional) Whose messages to delete.")] DiscordUser userToDelete = null)
        {
            if (numberOfMessages > 100)
            {
                await ctx.RespondAsync($":x: Boi, I can't do more than 100; alright? 100, take it or leave it-or do it again.");
            }
            else
            {
                await ctx.Message.DeleteAsync();
                IReadOnlyList<DiscordMessage> messages = new List<DiscordMessage>();
                if (userToDelete != null)
                {
                    var messagesUnspecified = await ctx.Channel.GetMessagesAsync(500);
                    messages = messagesUnspecified.Where(x => x.Author.Id == userToDelete.Id).Take(numberOfMessages).ToList();
                }
                else
                {
                    messages = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message.Id, numberOfMessages);
                }
                await ctx.Channel.DeleteMessagesAsync(messages, $"~clear command by {ctx.Member.DisplayName}");
                await ctx.RespondAsync($":white_check_mark: Deleted {messages.Count} messages, now praise me. __Unless you wanted me to delete messages before like" +
                    $" two weeks ago, because in that case, API-san says no.__");
            }
        }

        [RequirePermissions(Permissions.KickMembers)]
        [Description("Kick a member. You can optionally pass a reason.")]
        [Command("kick")]
        public async Task Kick(CommandContext ctx, [Description("Member to kick")] DiscordMember memberToKick, [RemainingText] [Description("(Optional) Reason of kick")] string reason = null)
        {
            await ctx.TriggerTypingAsync();
            Random = Random ?? new Random();
            string response = $"You've been kicked from {ctx.Guild.Name}. Reason being:\n```diff\n- {reason ?? ("Not specified")}\n```";
            if (Database.Commands?.ContainsKey(ctx.Command.Name) == true)
            {
                var command = Database.Commands[ctx.Command.Name];
                response = command.Response ?? response;
                if (command.Attachments?.Count > 0)
                {
                    var attachment = command.Attachments.ElementAt(Random.Next(0, command.Attachments.Count));
                    string path = Environment.CurrentDirectory + (IsLinux ? $"/cachedimages/{GetFileName(attachment.AttachmentURL)}" : $"\\cachedimages\\{GetFileName(attachment.AttachmentURL)}");
                    if (!File.Exists(path))
                    {
                        var client = new RestClient(attachment.AttachmentURL);
                        var req = new RestRequest(Method.GET);
                        Directory.CreateDirectory(Environment.CurrentDirectory + (IsLinux ? $"/cachedimages" : $"\\cachedimages"));
                        client.DownloadData(req).SaveAs(path);
                    }
                    using (FileStream fs = File.OpenRead(path))
                    {
                        await memberToKick.SendFileAsync(fs, response);
                    }
                }
            }
            await memberToKick.RemoveAsync(reason);
            await ctx.RespondAsync($":white_check_mark: Sucessfully kicked {ctx.Member.DisplayName}");
        }

        [Description("Ban a member. You can optionally pass a reason, or, instead of pinging the user to ban, send his ID.")]
        [RequirePermissions(Permissions.BanMembers)]
        [Command("ban")]
        public async Task Ban(CommandContext ctx, [Description("User to ban")] DiscordUser userToBan, [Description("(Optional) Reason of kick")] [RemainingText] string reason = null)
        {
            await ctx.TriggerTypingAsync();
            Random = Random ?? new Random();
            string response = $"Uh-oh, you've been banned from {ctx.Guild.Name}. Reason being:\n```diff\n- {reason ?? ("Not specified")}\n```";
            var guildMembers = ctx.Guild.Members;
            if (Database?.Commands?.ContainsKey(ctx.Command.Name) == true && guildMembers.ContainsKey(userToBan.Id))
            {
                var member = await ctx.Guild.GetMemberAsync(userToBan.Id);
                var command = Database.Commands[ctx.Command.Name];
                response = command.Response ?? response;
                if (command.Attachments?.Count > 0)
                {
                    var attachment = command.Attachments.ElementAt(Random.Next(0, command.Attachments.Count));
                    string path = Environment.CurrentDirectory + (IsLinux ? $"/cachedimages/{GetFileName(attachment.AttachmentURL)}" : $"\\cachedimages\\{GetFileName(attachment.AttachmentURL)}");
                    if (!File.Exists(path))
                    {
                        var client = new RestClient(attachment.AttachmentURL);
                        var req = new RestRequest(Method.GET);
                        Directory.CreateDirectory(Environment.CurrentDirectory + (IsLinux ? $"/cachedimages" : $"\\cachedimages"));
                        client.DownloadData(req).SaveAs(path);
                    }
                    using (FileStream fs = File.OpenRead(path))
                    {
                        await member.SendFileAsync(fs, response);
                    }
                }
            }
            await ctx.Guild.BanMemberAsync(userToBan.Id, reason: reason);
            await ctx.RespondAsync($":white_check_mark: Sucessfully kicked {ctx.Member.DisplayName}");
        }

        [Description("Temporarily bans a member for a given amount of days. You may pass an ID of a user instead of a ping, and, optionally, a reason.")]
        [RequirePermissions(Permissions.BanMembers)]
        [Command("tempban")]
        public async Task TempBan(CommandContext ctx, [Description("User to temporarily ban.")] DiscordUser userToBan, [Description("Ban duration in days")] int durationInDays, [Description("(Optional) Temp ban reason.")] [RemainingText] string reason = null)
        {
            await ctx.TriggerTypingAsync();
            Random = Random ?? new Random();
            string response = $"Uh-oh, you've been banned from {ctx.Guild.Name} for {durationInDays} days. Reason being:\n```diff\n- {reason ?? ("Not specified")}\n```";
            var guildMembers = ctx.Guild.Members;
            if (Database?.Commands?.ContainsKey(ctx.Command.Name) == true && guildMembers.ContainsKey(userToBan.Id))
            {
                var member = await ctx.Guild.GetMemberAsync(userToBan.Id);
                var command = Database.Commands[ctx.Command.Name];
                response = command.Response ?? response;
                if (command.Attachments?.Count > 0)
                {
                    var attachment = command.Attachments.ElementAt(Random.Next(0, command.Attachments.Count));
                    string path = Environment.CurrentDirectory + (IsLinux ? $"/cachedimages/{GetFileName(attachment.AttachmentURL)}" : $"\\cachedimages\\{GetFileName(attachment.AttachmentURL)}");
                    if (!File.Exists(path))
                    {
                        var client = new RestClient(attachment.AttachmentURL);
                        var req = new RestRequest(Method.GET);
                        Directory.CreateDirectory(Environment.CurrentDirectory + (IsLinux ? $"/cachedimages" : $"\\cachedimages"));
                        client.DownloadData(req).SaveAs(path);
                    }
                    using (FileStream fs = File.OpenRead(path))
                    {
                        await member.SendFileAsync(fs, response);
                    }
                }
            }
            await ctx.Guild.BanMemberAsync(userToBan.Id, durationInDays, reason);
            await ctx.RespondAsync($":white_check_mark: Sucessfully kicked {ctx.Member.DisplayName}");
        }

        [Description("Ban user by User ID")]
        [RequirePermissions(Permissions.BanMembers)]
        [Command("ban")]
        public async Task Ban(CommandContext ctx, [Description("ID of the user to ban.")] ulong userID, [Description("(Optional) Reason of ban")] [RemainingText] string reason = null)
        {
            await ctx.TriggerTypingAsync();
            Random = Random ?? new Random();
            string response = $"Uh-oh, you've been kicked from {ctx.Guild.Name}. Reason being:\n```diff\n- {reason ?? ("Not specified")}\n```";
            var guildMembers = ctx.Guild.Members;
            if (Database?.Commands?.ContainsKey(ctx.Command.Name) == true && guildMembers.ContainsKey(userID))
            {
                var member = await ctx.Guild.GetMemberAsync(userID);
                var command = Database.Commands[ctx.Command.Name];
                response = command.Response ?? response;
                if (command.Attachments?.Count > 0)
                {
                    var attachment = command.Attachments.ElementAt(Random.Next(0, command.Attachments.Count));
                    string path = Environment.CurrentDirectory + (IsLinux ? $"/cachedimages/{GetFileName(attachment.AttachmentURL)}" : $"\\cachedimages\\{GetFileName(attachment.AttachmentURL)}");
                    if (!File.Exists(path))
                    {
                        var client = new RestClient(attachment.AttachmentURL);
                        var req = new RestRequest(Method.GET);
                        Directory.CreateDirectory(Environment.CurrentDirectory + (IsLinux ? $"/cachedimages" : $"\\cachedimages"));
                        client.DownloadData(req).SaveAs(path);
                    }
                    using (FileStream fs = File.OpenRead(path))
                    {
                        await member.SendFileAsync(fs, response);
                    }
                }
            }
            await ctx.Guild.BanMemberAsync(userID, reason: reason);
            await ctx.RespondAsync($":white_check_mark: Sucessfully kicked {ctx.Member.DisplayName}");
        }

        [Description("Temporarily ban user by User ID")]
        [RequirePermissions(Permissions.BanMembers)]
        [Command("tempban")]
        public async Task TempBan(CommandContext ctx, [Description("ID of the user to temporarily ban.")] ulong userID, [Description("Ban duration in days")] int durationInDays, [Description("(Optional) Reason of ban")] [RemainingText] string reason = null)
        {
            await ctx.TriggerTypingAsync();
            Random = Random ?? new Random();
            string response = $"Uh-oh, you've been kicked from {ctx.Guild.Name} for {durationInDays} days. Reason being:\n```diff\n- {reason ?? ("Not specified")}\n```";
            var guildMembers = ctx.Guild.Members;
            if (Database?.Commands?.ContainsKey(ctx.Command.Name) == true && guildMembers.ContainsKey(userID))
            {
                var member = await ctx.Guild.GetMemberAsync(userID);
                var command = Database.Commands[ctx.Command.Name];
                response = command.Response ?? response;
                if (command.Attachments?.Count > 0)
                {
                    var attachment = command.Attachments.ElementAt(Random.Next(0, command.Attachments.Count));
                    string path = Environment.CurrentDirectory + (IsLinux ? $"/cachedimages/{GetFileName(attachment.AttachmentURL)}" : $"\\cachedimages\\{GetFileName(attachment.AttachmentURL)}");
                    if (!File.Exists(path))
                    {
                        var client = new RestClient(attachment.AttachmentURL);
                        var req = new RestRequest(Method.GET);
                        Directory.CreateDirectory(Environment.CurrentDirectory + (IsLinux ? $"/cachedimages" : $"\\cachedimages"));
                        client.DownloadData(req).SaveAs(path);
                    }
                    using (FileStream fs = File.OpenRead(path))
                    {
                        await member.SendFileAsync(fs, response);
                    }
                }
            }
            await ctx.Guild.BanMemberAsync(userID, durationInDays, reason);
            await ctx.RespondAsync($":white_check_mark: Sucessfully kicked {ctx.Member.DisplayName}");
        }

        [Priority(0)]
        [RequirePermissions(Permissions.ManageRoles)]
        [Description("Detains a user. Set up the detain role(s) using ~setup detain")]
        [Command("detain")]
        public async Task Detain(CommandContext ctx, [Description("The member to detain")] DiscordMember memberToDetain, [RemainingText] [Description("The reason to detain")] string reason = null)
        {
            if (ctx.CanTakeAction(memberToDetain))
            {
                //if (Regex.IsMatch(reason.Split(" ")[0].Trim(), @"[+-]?([0-9]*[.])?[0-9]+")) {
                //    var duration = double.Parse(reason.Split(" ")[0].Trim());
                //    await Detain(ctx, memberToDetain, duration, string.Join(" ", reason.Split(" ").Skip(1)));
                //    return;
                //}
                await ctx.TriggerTypingAsync();
                var guildDetention = Database?.Guilds[ctx.Guild.Id.ToString()]?.Info?.Detention;
                var detentionRoles = guildDetention?.DetentionRoles?.Select(x => ctx.Guild.GetRole(x)).ToList();
                if (detentionRoles == null || detentionRoles.Count <= 0 || ulong.Parse(guildDetention?.DetentionChannel) == 0)
                {
                    await ctx.RespondAsync($":x: You haven't setup the detention role. Please do so using `{ctx.Prefix}setup detention`");
                    return;
                }
                var detentionChannel = ctx.Guild.GetChannel(ulong.Parse(guildDetention.DetentionChannel));
                var rolesToRemove = guildDetention?.RolesToRemove?.Select(x => ctx.Guild.GetRole(x)).ToList();
                foreach (var detentionRole in detentionRoles)
                {
                    await memberToDetain.GrantRoleAsync(detentionRole);
                }
                if (rolesToRemove != null)
                {
                    foreach (var roleToRemove in rolesToRemove)
                    {
                        await memberToDetain.RevokeRoleAsync(roleToRemove);
                    }
                }
                await ctx.RespondAsync($":white_check_mark: Successfully detained {memberToDetain.DisplayName}.");
                await detentionChannel.SendMessageAsync($"{memberToDetain.Mention}, you've been detained by {ctx.Member.Mention} for the following reason:\n```diff\n- {reason}\n```");
            }
        }

        [Priority(1)]
        [Description("Temporarily detains a user. Will automatically undetain after a specific time. Set up the detain role(s) using ~setup detain")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Command("detain")]
        public async Task Detain(CommandContext ctx, [Description("The member to detain")] DiscordMember memberToDetain, [Description("Detention duration in hours")] double detainHours, [RemainingText] [Description("The reason to detain")] string reason = "No reason specified.")
        {
            if (ctx.CanTakeAction(memberToDetain))
            {
                await ctx.TriggerTypingAsync();
                var guildDetention = Database?.Guilds[ctx.Guild.Id.ToString()]?.Info?.Detention;
                var detentionRoles = guildDetention?.DetentionRoles?.Select(x => ctx.Guild.GetRole(x)).ToList();
                if (detentionRoles == null || detentionRoles.Count <= 0 || ulong.Parse(guildDetention.DetentionChannel) == 0)
                {
                    await ctx.RespondAsync($"You havent setup the detention role. Please do so using `~setup detain`");
                    return;
                }
                var detentionChannel = ctx.Guild.GetChannel(ulong.Parse(guildDetention.DetentionChannel));
                var rolesToRemove = guildDetention?.RolesToRemove?.Select(x => ctx.Guild.GetRole(x)).ToList();
                foreach (var detentionRole in detentionRoles)
                {
                    await memberToDetain.GrantRoleAsync(detentionRole);
                }
                if (rolesToRemove != null)
                {
                    foreach (var roleToRemove in rolesToRemove)
                    {
                        await memberToDetain.RevokeRoleAsync(roleToRemove);
                    }
                }
                if (!Database.Guilds.ContainsKey(ctx.Guild.Id.ToString()))
                {
                    Database.Guilds.Add(ctx.Guild.Id.ToString(), new Guild { });
                }
                Dictionary<string, RoleEvent> roleEvents = new Dictionary<string, RoleEvent>();
                detentionRoles.ForEach(x => roleEvents.Add(x.Id.ToString(), new RoleEvent { RoleActionType = RoleActionTypes.RoleActionType.Revoke }));
                rolesToRemove.ForEach(x => roleEvents.Add(x.Id.ToString(), new RoleEvent { RoleActionType = RoleActionTypes.RoleActionType.Give }));
                var roleRemoveAndAddEvent = new GuildEvent
                {
                    Date = DateTime.Now.ToUniversalTime().AddHours(detainHours).ToString("s", CultureInfo.InvariantCulture),
                    EventType = EventType.DiscordEventType.RoleAction,
                    GuildID = ctx.Guild.Id.ToString(),
                    UserIds = new Dictionary<string, DiscordUserID> { { memberToDetain.Id.ToString(), new DiscordUserID { Send = true } } },
                    Roles = roleEvents,
                };
                FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id.ToString()).Child("GuildEvents").Child($"{memberToDetain.Id.ToString()}detention").SetValueAsync(roleRemoveAndAddEvent);
                await ctx.RespondAsync($":white_check_mark: Successfully detained {memberToDetain.DisplayName}.");
                await detentionChannel.SendMessageAsync($"{memberToDetain.Mention}, you've been detained by {ctx.Member.Mention} for the following reason:\n```diff\n- {reason}\n```" +
                    $"\nYou'll be undetained automatically in {detainHours} hours.");
            }
        }

        [RequirePermissions(Permissions.ManageRoles)]
        [Description("Undetains a user; this also works if you temporarily detained them.")]
        [Command("undetain")]
        public async Task Undetain(CommandContext ctx, DiscordMember member)
        {
            if (ctx.CanTakeAction(member))
            {
                await ctx.TriggerTypingAsync();
                var detentionRoles = Database.Guilds[ctx.Guild.Id.ToString()]?.Info?.Detention?.DetentionRoles.Select(x => ctx.Guild.GetRole(x)).ToList();
                var rolesToGiveBack = Database.Guilds[ctx.Guild.Id.ToString()]?.Info?.Detention?.RolesToRemove.Select(x => ctx.Guild.GetRole(x)).ToList();
                if (detentionRoles == null || detentionRoles.Count <= 0)
                {
                    await ctx.RespondAsync($":x: You havent setup the detention role. Please do so using `~setup detain`");
                    return;
                }
                if (!member.Roles.Any(x => detentionRoles.Contains(x)))
                {
                    await ctx.RespondAsync($":x: This user isn't detained.");
                    return;
                }
                else
                {
                    var filteredDetentionRoles = detentionRoles.Where(x => member.Roles.Contains(x)).ToList();
                    foreach (var detentionRole in filteredDetentionRoles)
                    {
                        await member.RevokeRoleAsync(detentionRole);
                    }
                    if (rolesToGiveBack != null && rolesToGiveBack.Count > 0)
                    {
                        var filteredRolesToGiveBack = rolesToGiveBack.Where(x => !member.Roles.Contains(x)).ToList();
                        foreach (var roleToGiveBack in filteredRolesToGiveBack)
                        {
                            await member.GrantRoleAsync(roleToGiveBack);
                        }
                    }
                    await ctx.RespondAsync($":white_check_mark: Successfully undetained {member.DisplayName}");
                    if (Database.Guilds[ctx.Guild.Id.ToString()]?.GuildEvents?.ContainsKey($"{member.Id.ToString()}detention") == true)
                    {
                        await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id.ToString()).Child("GuildEvents").Child($"{member.Id.ToString()}detention").DeleteValueAsync();
                    }
                }
            }
        }

        [Description("Gets a list of users in this server with a specific role, some exclusions, or just everyone.")]
        [Command("listmembers")]
        public async Task ListMembers(CommandContext ctx)
        {
            if (!ctx.IsStaffMember())
            {
                return;
            }
            var tracker = new InteractivityEventTracker(ctx);
            List<DiscordRole> rolesToSend = new List<DiscordRole> { ctx.Guild.EveryoneRole };
            List<DiscordRole> roleExclusions = new List<DiscordRole>();
            var askRoles = await
                tracker.
                    AskAndWaitForResponseAsync($"**In the following inqueries, please respond with a role by eithering `@`ing it, " +
                        $"or just sending their names; all seperated by commas.**\n" +
                        $"What roles would you like to include in this list? Send *everyone* to include all roles.",
                        tracker.InteractivityConditions.ValidRoleCondition, sendCancelNotice: true, acceptNone: true, noneOverride: "everyone");
            if (tracker.Status == InteractivityStatus.OK)
            {
                rolesToSend = askRoles.ParseSentRoles();
                var askExclusions = await
                    tracker.
                        AskAndWaitForResponseAsync($"What roles would you like to exclude from this list? Send *none* to not exclude any role.",
                            tracker.InteractivityConditions.ValidRoleCondition, acceptNone: true);
                if (tracker.Status == InteractivityStatus.OK)
                {
                    roleExclusions = askExclusions.ParseSentRoles();
                    tracker.SetFinished();
                }
            }
            switch (tracker.Status)
            {
                case InteractivityStatus.Cancelled:
                    await tracker.SendMessageResponse($":white_check_mark: Cancelled ongoing operation.");
                    break;
                case InteractivityStatus.TimedOut:
                    await tracker.SendMessageResponse($":x: Timed out. Please try again.");
                    break;
                case InteractivityStatus.OK:
                case InteractivityStatus.Finished:
                    List<DiscordMember> members = (await ctx.Guild.GetAllMembersAsync()).ToList();
                    members = members
                        .Where(member =>
                            member.Roles.Any(memberRole => rolesToSend.Contains(memberRole)) ||
                            !member.Roles.Any(memberRole => roleExclusions.Contains(memberRole)))
                        .ToList();
                    string membersList = string.Join("\n", members.Select(x => $"{x.DisplayName}#{x.Discriminator}"));
                    var listID = $"{ctx.Guild.Name}.{DateTime.Now.ToString("MM.dd.yyyy")}";
                    var file = ConvertToSystemPath($"{Environment.CurrentDirectory}/{ctx.Guild.Id}/{listID}.txt");
                    Directory.CreateDirectory(GetDirectoryName(file));
                    using (FileStream fs = File.Create(file))
                    {
                        byte[] text = new UTF8Encoding(true).GetBytes(membersList);
                        fs.Write(text, 0, text.Length);
                    }
                    using (FileStream fs = File.OpenRead(file))
                    {
                        await ctx.RespondWithFileAsync(fs, ":white_check_mark: Here's your very ugly list.");
                    }
                    File.Delete(file);
                    break;
                default:
                    break;
            }
            await tracker.DeleteMessagesAsync();
        }

        [Command("addreactionrole")]
        public async Task AddRolesToReactionMessage(CommandContext ctx, string reactionMsgId)
        {
            if (ctx.IsStaffMember())
            {
                await ctx.TriggerTypingAsync();
                var reactionMessages = Database.Guilds[ctx.Guild.Id.ToString()]?.ReactionMessages;
                var tracker = new InteractivityEventTracker(ctx);
                if (reactionMessages == null)
                {
                    await ctx.RespondAsync($":x: This server doesn't have any reaction messages.");
                    return;
                }
                try
                {
                    var reactionMessageObj = reactionMessages.First(reactionMsg => reactionMsg.Key == reactionMsgId);
                    var reactionMessage = await ctx.Guild
                                .GetChannel(ulong.Parse(reactionMessageObj.Value.ChannelId))
                                .GetMessageAsync(ulong.Parse(reactionMessageObj.Key));
                    var embedBuilder = new DiscordEmbedBuilder(reactionMessage.Embeds[0]);
                    var askEmojisToAdd = await tracker.AskAndWaitForResponseAsync($"**Enter the reaction and role name(s)/ping(s) following the example below:**" +
                    $"```yaml\n:emoji: - Role - Description of this role\n\n:pepega: - @Pepega - Gives the Pepega Role\n\n" +
                    $":overwatch: - Overwatch - Gives the Overwatch Role (Yes btw you don't have to ping the role cool right)\n\n" +
                    $":rainbowsixseige: - Rainbow Six Seige, @Rainbow Six    Seige LFG - Gives both Rainbow Six Seige and Rainbow Six Seige Comp roles. Seperate roles by commas. Please.```",
                    timeOutOverride: TimeSpan.FromMinutes(4), sendCancelNotice: true);
                    if (tracker.Status == InteractivityStatus.OK)
                    {
                        var result = Regex.Replace(askEmojisToAdd.Result.Content, @"(\r\n)+", "\r\n\r\n", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                        var inputs = new List<string>(result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                        if (reactionMessage.Reactions.Count + inputs.Count > 20)
                        {
                            await ctx.RespondAsync($":x: You can only have up to 20 role-reactions in one message.");
                            return;
                        }
                        Dictionary<string, ReactionEmoji> emojiAndRoles =
                            new Dictionary<string, ReactionEmoji>();
                        var index = 0;
                        foreach (var input in inputs)
                        {
                            var splitInput = input.Split(" - ").Select(x => x.Trim()).ToList();
                            var splitRoles = splitInput[1].Split(",").Select(x => x.Trim()).ToList();
                            DiscordEmoji emoji;
                            emoji = ParseDiscordEmoji(ctx.Client, splitInput[0]);
                            var roles = ctx.Guild.Roles.Values.Where(role => splitRoles.Contains(role.Mention) | splitRoles.Contains(role.Name)).ToList();
                            if (emoji == null || roles == null || roles.Count < 1 || string.IsNullOrWhiteSpace(splitInput[2]))
                            {
                                await ctx.RespondAsync($"Error in your formatting at line {index + 1}.");
                                continue;
                            }
                            embedBuilder.AddField($"{emoji.GetEmbedFriendlyEmojiName()} - {string.Join(",", roles.Select(x => x.Name))}", splitInput[2], true);
                            emojiAndRoles.Add(Guid.NewGuid().ToString(), new ReactionEmoji
                            {
                                EmojiName = emoji.RequiresColons ? $"{emoji.GetDiscordName()}" : emoji.GetDiscordName(),
                                Emoji = emoji,
                                Description = splitInput[2].Trim(),
                                RoleIds = roles.Select(x => x.Id).ToList()
                            });
                            index++;
                        }
                        await reactionMessage.ModifyAsync(embed: embedBuilder.Build());
                        FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                        var newReactionMessageObj = new ReactionMessage { Emojis = emojiAndRoles };
                        await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id).Child("ReactionMessages").Child(reactionMessage.Id.ToString()).UpdateValueAsync(newReactionMessageObj);
                        foreach (var reactionEmoji in emojiAndRoles)
                        {
                            try
                            {
                                await reactionMessage.CreateReactionAsync(reactionEmoji.Value.Emoji);
                                await Task.Delay(700);
                            }
                            catch (Exception e)
                            {
                                await ctx.RespondAsync($"Welp\nOne of your emojis are invalid; couldn't create a reaction from it. Do I have access to this emoji? I have to be in a server where this emoji exists btw.");
                                Console.WriteLine(e.StackTrace);
                            }
                        }
                    }
                }
                catch (Exception ex) when (ex is UnauthorizedException || ex is InvalidOperationException || ex is IndexOutOfRangeException)
                {
                    await ctx.RespondAsync($":x: Couldn't find a valid reaction-role message with ID {reactionMsgId}");
                    return;
                }
                switch (tracker.Status)
                {
                    case InteractivityStatus.Cancelled:
                        await ctx.RespondAsync($":white_check_mark: Cancelled all ongoing operations.");
                        break;
                    case InteractivityStatus.TimedOut:
                        await ctx.RespondAsync($":x: Timed out. Please try again.");
                        break;
                    case InteractivityStatus.OK:
                    case InteractivityStatus.Finished:
                        await ctx.RespondAsync($":white_check_mark: Successfully addded reaction role(s)!");
                        break;
                    default:
                        break;
                }
                await tracker.DeleteMessagesAsync();
            }
        }

        [Command("deletereactionrole")]
        public async Task DeleteReactionRole(CommandContext ctx, string reactionMsgId)
        {
            if (ctx.IsStaffMember())
            {
                await ctx.TriggerTypingAsync();
                var reactionMessages = Database.Guilds[ctx.Guild.Id.ToString()]?.ReactionMessages;
                var tracker = new InteractivityEventTracker(ctx);
                if (reactionMessages == null)
                {
                    await ctx.RespondAsync($":x: This server doesn't have any reaction messages.");
                    return;
                }
                try
                {
                    var reactionMessageObj = reactionMessages.First(reactionMsg => reactionMsg.Key == reactionMsgId);
                    var reactionMessage = await ctx.Guild
                                .GetChannel(ulong.Parse(reactionMessageObj.Value.ChannelId))
                                .GetMessageAsync(ulong.Parse(reactionMessageObj.Key));
                    var embedBuilder = new DiscordEmbedBuilder(reactionMessage.Embeds[0]);
                    StringBuilder availableRoleReactions = new StringBuilder();
                    int listIndex = 1;
                    Database?.Guilds[ctx.Guild.Id.ToString()]
                        ?.ReactionMessages[reactionMessage.Id.ToString()]
                        ?.Emojis?.Values?.ToList()?.ForEach(x =>
                        {
                            availableRoleReactions.Append($"\n{listIndex}. {x.EmojiName}");
                            listIndex++;
                        });
                    var askWhichRoleReaction = await tracker.AskAndWaitForResponseAsync($"Which role-reaction would you like to delete? Available ones:" +
                        $"\n\n```{availableRoleReactions.ToString().Trim()}```", tracker.InteractivityConditions.IntegerCondition, sendCancelNotice: true);
                    var roleReactionEmojis = Database.Guilds[ctx.Guild.Id.ToString()]
                        .ReactionMessages[reactionMessage.Id.ToString()]
                        .Emojis;
                    var roleReaction = roleReactionEmojis.Values?.ToList()[int.Parse(askWhichRoleReaction.Result.Content.Trim()) - 1];
                    Dictionary<string, ReactionEmoji> emojis = new Dictionary<string, ReactionEmoji>(roleReactionEmojis);
                    emojis.Remove(roleReactionEmojis.First(x => x.Value == roleReaction).Key);
                    var newFields = embedBuilder.Fields.ToList();
                    var emoji = ParseDiscordEmoji(ctx.Client, roleReaction.EmojiName);
                    await reactionMessage.DeleteOwnReactionAsync(emoji);
                    newFields.Remove(embedBuilder.Fields.FirstOrDefault(field => field.Name.StartsWith(emoji.GetEmbedFriendlyEmojiName())));
                    newFields = newFields.OrderBy(field => field.Value).ToList();
                    embedBuilder.ClearFields();
                    newFields.ForEach(x => embedBuilder.AddField(x.Name, x.Value, x.Inline));
                    await reactionMessage.ModifyAsync(embed: embedBuilder.Build());
                    FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                    await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id.ToString()).Child("ReactionMessages").Child(reactionMessage.Id.ToString())
                        .Child("Emojis").SetValueAsync(emojis);
                }
                catch (Exception ex) when (ex is UnauthorizedException || ex is InvalidOperationException || ex is IndexOutOfRangeException)
                {
                    await ctx.RespondAsync($":x: Couldn't find a valid reaction-role message with ID {reactionMsgId}");
                    return;
                }
                switch (tracker.Status)
                {
                    case InteractivityStatus.Cancelled:
                        await ctx.RespondAsync($":white_check_mark: Cancelled all ongoing operations.");
                        break;
                    case InteractivityStatus.TimedOut:
                        await ctx.RespondAsync($":x: Timed out. Please try again.");
                        break;
                    case InteractivityStatus.OK:
                    case InteractivityStatus.Finished:
                        await ctx.RespondAsync($":white_check_mark: Successfully removed reaction role(s)!");
                        break;
                    default:
                        break;
                }
            }
        }

        [Description("Update the text under the role name in a reaction message.")]
        [Command("updatereactionroletext")]
        public async Task UpdateReactionRoleText(CommandContext ctx, 
            [Description("What channel this message is in")] DiscordChannel channel,
            [Description("The reaction message's ID")] ulong id,
            [Description("The role's name (e.g Overwatch)")] string roleName,
            [RemainingText] [Description("New text of this field")] string newText)
        {
            if (!ctx.IsStaffMember())
            {
                return;
            }
            var message = await channel.GetMessageAsync(id);
            if (message == null)
            {
                await ctx.RespondAsync($":x: Couldn't find that message.");
                return;
            }
            var embed = message.Embeds.FirstOrDefault();
            if (embed == null)
            {
                await ctx.RespondAsync($":x: Not a valid reaction-role message.");
                return;
            }
            var builder = new DiscordEmbedBuilder(embed)
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor { IconUrl = ctx.Guild.CurrentMember?.AvatarUrl, Name = embed.Author?.Name, Url = embed.Author.Url?.AbsoluteUri },
            };
            var fields = builder.Fields.ToList();
            DiscordEmbedField foundField = fields.FirstOrDefault(x => x.Name.ToLower().Split(" - ").ElementAtOrDefault(1) == roleName.ToLower());
            int index = fields.IndexOf(foundField);
            if (foundField == null)
            {
                await ctx.RespondAsync($":x: Couldn't find that role.");
                return;
            }
            fields.Remove(foundField);
            builder.ClearFields();
            foundField.Value = newText;
            fields.Insert(index, foundField);
            foreach (var field in fields)
            {
                builder.AddField(field.Name, field.Value, field.Inline);
            }
            await message.ModifyAsync(embed: builder.Build());
            await ctx.RespondAsync(":white_check_mark: Success!");
        }

        [Command("sortreactionrolemessage")]
        [Description("Sorts this message's roles alphabetically")]
        public async Task SortRoleReactionMessage(CommandContext ctx, DiscordChannel channel, ulong messageId)
        {
            if (!ctx.IsStaffMember())
            {
                return;
            }
            var message = await channel.GetMessageAsync(messageId);
            if (message == null)
            {
                await ctx.RespondAsync($":x: Couldn't find that message.");
                return;
            }
            var embed = message.Embeds.FirstOrDefault();
            if (embed == null)
            {
                await ctx.RespondAsync($":x: Not a valid reaction-role message.");
                return;
            }
            var builder = new DiscordEmbedBuilder(embed);
            var fields = builder.Fields.ToList();
            fields = fields.OrderBy(x => x.Name.Split(" - ").ElementAtOrDefault(1)).ToList();
            builder.ClearFields();
            fields.ForEach(x => builder.AddField(x.Name, x.Value, x.Inline));
            await message.ModifyAsync(embed: builder.Build());
            await ctx.RespondAsync($":white_check_mark: Success!");
        }
        //[Command("otp")]
        //public async Task OTP(CommandContext ctx)
        //{
        //    var client = new YuutaFirebaseClient();
        //    await ctx.Message.DeleteAsync();
        //    Dictionary<string, ReactionEmoji> emojis = new Dictionary<string, ReactionEmoji>();
        //    var message = await ctx.Channel.GetMessageAsync(648183644338389025);
        //    var embed = message.Embeds[0];
        //    var allroles = ctx.Guild.Roles.Values;
        //    foreach (var field in embed.Fields)
        //    {
        //        var split = field.Name.Split(" - ");
        //        var roleName = split[1].Trim().ToLower();
        //        var role = allroles.First(x => x.Name.ToLower() == roleName);
        //        try
        //        {
        //            ReactionEmoji emoji = new ReactionEmoji();
        //            emoji.EmojiName = Regex.Match(split[0].Trim(), ":(.*?):").Value;
        //            emoji.Description = field.Value;
        //            emoji.RoleIds = new List<ulong>() { role.Id };
        //            emojis.Add(Guid.NewGuid().ToString(), emoji);
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e.StackTrace);
        //            throw;
        //        }
        //    }
        //    await client.Child("Guilds").Child(ctx.Guild.Id.ToString()).Child("ReactionMessages").Child(648183644338389025).Child("Emojis").SetValueAsync(emojis);
        //    await Task.Delay(0);
        //}

    }
}