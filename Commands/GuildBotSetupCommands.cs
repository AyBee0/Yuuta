using AuthorityHelpers;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FirebaseHelper;
using InteractivityHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Types;
using Types.DatabaseObjects.DiscordObjects;
using static FirebaseHelper.YuutaFirebaseClient;
using static InteractivityHelpers.InteractivityEventTracker;

namespace Commands
{

    [Group("setup")]
    public class GuildBotSetupCommands : BaseCommandModule {

        private readonly ulong[] BotAdmins = new ulong[] { 247386254499381250 };

        //private Random Random;
        private YuutaFirebaseClient FirebaseClient;

        [Description("Sets up the detention system")]
        [Aliases("detain")]
        [Command("detention")]
        public async Task SetupDetain(CommandContext ctx) {
            if (ctx.IsStaffMember()) {
                await ctx.TriggerTypingAsync();
                var detentionObj = new Detention();
                var tracker = new InteractivityEventTracker(ctx);
                var guildRoles = ctx.Guild.Roles.Values;
                await tracker.AskInteractivityAsync($"*To cancel at any time, please respond with \"cancel\"*\n\n Oki, I'll walk you through setting up the detention system." +
                    $"\n\n**As a start, what channel would you like to be set as the detention channel?**");
                var interactivity = ctx.Client.GetInteractivity();
                var channelSent = await interactivity.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Author.Id == ctx.Member.Id && (x.MentionedChannels?.Count > 0
                                                                            || x.Content.ToLower().Trim() == "cancel"));
                tracker.Update(channelSent);
                if (tracker.Status == InteractivityStatus.OK) {
                    detentionObj.DetentionChannel = channelSent.Result.MentionedChannels[0].Id.ToString();
                    await tracker.AskInteractivityAsync($":white_check_mark: Oki.\nWhat roles should I give the user when they're detained? Seperate them by commas please, and feel free to ping them or just insert the role names.");
                    var detRolesSent = await interactivity.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Author.Id == ctx.Member.Id
                                            && x.Content.Replace("@", "").Split(",").Select(y => y.Trim().ToLower()).ToList().Any(z => guildRoles.Select(w => w.Name.ToLower()).Contains(z)));
                    tracker.Update(detRolesSent);
                    if (tracker.Status == InteractivityStatus.OK) {
                        detentionObj.DetentionRoles = guildRoles
                            .Where(guildRole => detRolesSent.Result.Content.Replace("@", "").Split(",").Select(x => x.Trim().ToLower()).Any(givenRole => guildRole.Name.ToLower() == givenRole))
                            .Select(role => role.Id)
                            .ToList();
                        await tracker.AskInteractivityAsync($":white_check_mark: Oki.\nWhat roles should I take revoke the user from when they're detained? Seperate them by commas please, and feel free to ping them or just insert the role names.");
                        var revokeRolesSent = await interactivity.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Author.Id == ctx.Member.Id
                                            && (x.Content.Replace("@", "").Split(",").Select(y => y.Trim().ToLower()).ToList().Any(z => guildRoles.Select(w => w.Name.ToLower()).Contains(z))
                                                || x.Content.ToLower().Trim() == "cancel"));
                        tracker.Update(revokeRolesSent);
                        if (tracker.Status == InteractivityStatus.OK) {
                            detentionObj.RolesToRemove = guildRoles
                            .Where(guildRole => revokeRolesSent.Result.Content.Replace("@", "").Split(",").Select(x => x.Trim().ToLower()).Any(givenRole => guildRole.Name.ToLower() == givenRole))
                            .Select(role => role.Id)
                            .ToList();
                            tracker.SetFinished();
                        }
                    }
                }
                switch (tracker.Status) {
                    case InteractivityStatus.Cancelled:
                        await tracker.AskInteractivityAsync($":white_check_mark: Oki, cancelled.");
                        break;
                    case InteractivityStatus.TimedOut:
                        await tracker.AskInteractivityAsync($":x: Oki, cancelled.");
                        break;
                    case InteractivityStatus.OK:
                    case InteractivityStatus.Finished:
                        await tracker.AskInteractivityAsync($":white_check_mark: Oki, successfully setup the detention system. Gib pats.", false);
                        FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                        await FirebaseClient.Child($"Guilds").Child(ctx.Guild.Id).Child("Info").Child("Detention").SetValueAsync(detentionObj);
                        break;
                    default:
                        break;
                }
                await Task.Delay(5000);
                await tracker.DeleteMessagesAsync();
            }
        }

        [RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        [Description("Sets up staff. Only admins can execute this command.")]
        [Command("staff")]
        public async Task SetupStaff(CommandContext ctx) {
            await ctx.RespondAsync(ctx.Guild.EveryoneRole.Id.ToString());
            var tracker = new InteractivityEventTracker(ctx);
            var interactivity = ctx.Client.GetInteractivity();
            var guildRoles = ctx.Guild.Roles.Values;
            await tracker.AskInteractivityAsync($"*Send \"cancel\" to cancel at anytime.*\n\n Staff roles have the ability to create events, setup detention, etc.\n" +
                $"Please send the staff role names below, seperated by commas. You may mention the roles. Please just seperate the roles by commas I ain't spending" +
                $" the entire day trying to parse your laziness.");
            var staffRolesSent = await interactivity.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Author.Id == ctx.Member.Id
                                            && (x.Content.Replace("@", "").Split(",").Select(y => y = y == "everyone" ? "@everyone" : y.Trim().ToLower()).ToList().Any(z => guildRoles.Select(w => w.Name.ToLower()).Contains(z))
                                                || x.Content.ToLower().Trim() == "cancel"));
            tracker.Update(staffRolesSent);
            switch (tracker.Status) {
                case InteractivityStatus.Cancelled:
                    await tracker.AskInteractivityAsync(":white_check_mark: Cancelled ongoing operation.");
                    break;
                case InteractivityStatus.TimedOut:
                    await tracker.AskInteractivityAsync(":x: Timed out. Please try again");
                    break;
                case InteractivityStatus.Finished:
                case InteractivityStatus.OK:
                    var staffRoles = guildRoles
                    .Where(guildRole => staffRolesSent.Result.Content.Replace("@", "").Split(",").Select(x => x.Trim().ToLower()).Any(givenRole => guildRole.Name.ToLower().Replace("@everyone","everyone") == givenRole))
                    .Select(role => role.Id)
                    .ToList();
                    tracker.SetFinished();
                    FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                    await FirebaseClient
                        .Child("Guilds")
                        .Child(ctx.Guild.Id)
                        .Child("Info")
                        .Child("Authority")
                        .Child("StaffRoles")
                        .SetValueAsync(string.Join("|", staffRoles));
                    await tracker.AskInteractivityAsync($":white_check_mark: Successfully added staff roles.", false);
                    break;
                default:
                    break;
            }
            await Task.Delay(5000);
            await tracker.DeleteMessagesAsync();
        }

        [Description("Sets up bot channels.")]
        [Command("channels")]
        public async Task SetupChannels(CommandContext ctx) {
            if (ctx.IsStaffMember()) {
                var tracker = new InteractivityEventTracker(ctx);
                var interactivity = ctx.Client.GetInteractivity();
                var guildRoles = ctx.Guild.Roles.Values;
                await tracker.AskInteractivityAsync($"*Send \"cancel\" at anytime to cancel the ongoing operation.\n\n" +
                    $"In what channels should I limit non-staff members to use my commands to? Mention them below. *E.g: #bot-commands*");
                var botChannelsSent = await interactivity.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id
                                                                                && (x.MentionedChannels.Count > 0 || x.Content.ToLower().Trim() == "cancel"));
                tracker.Update(botChannelsSent);
                if (tracker.Status == InteractivityStatus.OK) {
                    var botChannels = botChannelsSent.Result.MentionedChannels.Select(x => x.Id).ToList();
                    await tracker.AskInteractivityAsync($"What roles should I exclude in this limitation? Send \"none\" to apply this for all non-staff members.");
                    var exclusionRolesSent = await interactivity.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Author.Id == ctx.Member.Id
                                            && (x.Content.Replace("@", "").Split(",").Select(y => y.Trim().ToLower()).ToList().Any(z => guildRoles.Select(w => w.Name.ToLower()).Contains(z))
                                            || x.Content.Trim().ToLower() == "none" || x.Content.ToLower().Trim() == "cancel"));
                    tracker.Update(exclusionRolesSent);
                    if (tracker.Status == InteractivityStatus.OK) {
                        var exclusionRoles = exclusionRolesSent.Result.Content.ToLower().Trim() == "none" ? new List<ulong>()
                            : guildRoles
                            .Where(guildRole => exclusionRolesSent.Result.Content.Replace("@", "").Split(",").Select(x => x.Trim().ToLower()).Any(givenRole => guildRole.Name.ToLower() == givenRole))
                            .Select(role => role.Id)
                            .ToList();
                        FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                        var authorityObj = Database?.Guilds[ctx.Guild.Id.ToString()]?.Info?.Authority ?? new Authority();
                        authorityObj.GlobalBotChannels = botChannels;
                        authorityObj.GlobalBotRoleOverrides = exclusionRoles;
                        Console.Write(authorityObj.GlobalBotChannels);
                        Console.Write(authorityObj.GlobalBotRoleOverrides);
                        await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id).Child("Info").Child("Authority").SetValueAsync(authorityObj);
                        tracker.SetFinished();
                    }
                }
                switch (tracker.Status) {
                    case InteractivityStatus.Cancelled:
                        await tracker.AskInteractivityAsync($":white_check_mark: Cancelled ongoing operations.");
                        break;
                    case InteractivityStatus.TimedOut:
                        await tracker.AskInteractivityAsync($":x: Timed out out. Please try again.");
                        break;
                    case InteractivityStatus.OK:
                    case InteractivityStatus.Finished:
                        await tracker.AskInteractivityAsync($":white_check_mark: Successfully setup bot channels and overrides.", false);
                        break;
                    default:
                        break;
                }
                await Task.Delay(5000);
                await tracker.DeleteMessagesAsync();
            }
        }

        [Description("Sets up the welcome system.")]
        [Command("welcome")]
        public async Task SetupWelcome(CommandContext ctx) {
            if (ctx.IsStaffMember()) {
                var welcome = Database?.Guilds[ctx.Guild.Id.ToString()]?.Info?.Welcome;
                welcome = welcome ?? new Welcome();
                welcome.Enabled = true;
                var tracker = new InteractivityEventTracker(ctx);
                var interactivity = ctx.Client.GetInteractivity();
                await tracker.AskInteractivityAsync($"*Send cancel at anytime to cancel ongoing operation.*\n\n" +
                    $"Hey bowss, pls mention the channel you're going to use for welcomes. E.g: #welcome-channel.");
                var sentWelcomeChannel = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id
                                                            && (x.MentionedChannels?.Count > 0 || x.Content.Trim().ToLower() == "cancel"));
                tracker.Update(sentWelcomeChannel);
                if (tracker.Status == InteractivityStatus.OK) {
                    welcome.Channel = sentWelcomeChannel.Result.MentionedChannels[0].Id.ToString();
                    await tracker.AskInteractivityAsync("What would you like your welcome message to be? Type {SERVER} for that to be replaced with your server name, " +
                        "{MEMBER} for that to be replaced with the member's name, and {MENTION} for that to be replaced with the user's mention.");
                    var messageTextResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id);
                    tracker.Update(sentWelcomeChannel);
                    if (tracker.Status == InteractivityStatus.OK) {
                        welcome.Message = messageTextResult.Result.Content;
                    }
                }
                switch (tracker.Status) {
                    case InteractivityStatus.Cancelled:
                        await tracker.AskInteractivityAsync($":white_check_mark: Cancelled ongoing operaiton.");
                        break;
                    case InteractivityStatus.TimedOut:
                        await tracker.AskInteractivityAsync($":x: Timed out. Please try again.");
                        break;
                    case InteractivityStatus.Finished:
                    case InteractivityStatus.OK:
                        FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                        await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id).Child("Info").Child("Welcome").SetValueAsync(welcome);
                        await tracker.AskInteractivityAsync($":white_check_mark: Successfully set and enabled welcome message." +
                            $"\nHere's an example of what will be sent when someone joins the server. Placeholder being you, {ctx.Message.Author.Username}.\n" +
                            $"{welcome.Message.Replace("{MENTION}", ctx.Member.Mention).Replace("{MEMBER}", ctx.Member.Nickname).Replace("{SERVER}", ctx.Guild.Name)}\n\n\n" +
                            $"*Hint:* To toggle welcome messages, send `~welcome disable` or `~welcome enable`", false);
                        break;
                    default:
                        break;
                }
                await Task.Delay(TimeSpan.FromSeconds(5));
                await tracker.DeleteMessagesAsync();
            }
        }

        [Description("Enables/Disables welcome messages")]
        [Command("welcome")]
        public async Task SetupWelcome(CommandContext ctx, [Description("Enable or Disable welcome messages")] string enableOrDisable) {
            if (ctx.IsStaffMember()) {
                await ctx.TriggerTypingAsync();
                await ctx.Message.DeleteAsync();
                var mEnableOrDisable = enableOrDisable.ToLower().Trim();
                if (mEnableOrDisable == "enable" || mEnableOrDisable == "disable") {
                    FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                    await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id).Child("Info").Child("Welcome").Child("Enabled").SetValueAsync(mEnableOrDisable == "enable");
                    await ctx.RespondAsync($":white_check_mark: Successfully {(mEnableOrDisable == "enable" ? "enabled" : "disabled")} welcome messages.\n\n");
                }
            }
        }

        [Description("Sets up the leave system.")]
        [Command("leave")]
        public async Task SetupLeave(CommandContext ctx) {
            if (ctx.IsStaffMember()) {
                var leave = Database?.Guilds[ctx.Guild.Id.ToString()]?.Info?.Leave;
                leave = leave ?? new Leave();
                leave.Enabled = true;
                var tracker = new InteractivityEventTracker(ctx);
                var interactivity = ctx.Client.GetInteractivity();
                await tracker.AskInteractivityAsync($"*Send cancel at anytime to cancel ongoing operation.*\n\n" +
                    $"Hey bowss, pls mention the channel you're going to use for leaves. E.g: #leave-channel.");
                var sentLeaveChannel = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id
                                                            && (x.MentionedChannels?.Count > 0 || x.Content.Trim().ToLower() == "cancel"));
                tracker.Update(sentLeaveChannel);   
                if (tracker.Status == InteractivityStatus.OK) {
                    leave.Channel = sentLeaveChannel.Result.MentionedChannels[0].Id.ToString();
                    await tracker.AskInteractivityAsync("What would you like your leave message to be? Type {SERVER} for that to be replaced with your server name, " +
                        "{MEMBER} for that to be replaced with the member's name, and {MENTION} for that to be replaced with the user's mention.");
                    var messageTextResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id);
                    tracker.Update(sentLeaveChannel);
                    if (tracker.Status == InteractivityStatus.OK) {
                        leave.Message = messageTextResult.Result.Content;
                    }
                }
                switch (tracker.Status) {
                    case InteractivityStatus.Cancelled:
                        await tracker.AskInteractivityAsync($":white_check_mark: Cancelled ongoing operaiton.");
                        break;
                    case InteractivityStatus.TimedOut:
                        await tracker.AskInteractivityAsync($":x: Timed out. Please try again.");
                        break;
                    case InteractivityStatus.Finished:
                    case InteractivityStatus.OK:
                        FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                        await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id).Child("Info").Child("Leave").SetValueAsync(leave);
                        await tracker.AskInteractivityAsync($":white_check_mark: Successfully set and enabled leave message." +
                            $"\nHere's an example of what will be sent when someone joins the server. Placeholder being you, {ctx.Message.Author.Username}.\n" +
                            $"{leave.Message.Replace("{MENTION}", ctx.Member.Mention).Replace("{MEMBER}", ctx.Member.Nickname).Replace("{SERVER}", ctx.Guild.Name)}\n\n\n" +
                            $"*Hint:* To toggle leave messages, send `~leave disable` or `~leave enable`", false);
                        break;
                    default:
                        break;
                }
                await Task.Delay(TimeSpan.FromSeconds(5));
                await tracker.DeleteMessagesAsync();
            }
        }

        [Description("Enables/Disables member leave messages")]
        [Command("leave")]
        public async Task SetupLeave(CommandContext ctx, [Description("Enable or Disable leave messages")] string enableOrDisable) {
            if (ctx.IsStaffMember()) {
                await ctx.TriggerTypingAsync();
                await ctx.Message.DeleteAsync();
                var mEnableOrDisable = enableOrDisable.ToLower().Trim();
                if (mEnableOrDisable == "enable" || mEnableOrDisable == "disable") {
                    FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                    await FirebaseClient.Child("Guilds").Child(ctx.Guild.Id).Child("Info").Child("Leave").Child("Enabled").SetValueAsync(mEnableOrDisable == "enable");
                    await ctx.RespondAsync($":white_check_mark: Successfully {(mEnableOrDisable == "enable" ? "enabled" : "disabled")} leave messages.\n\n");
                }
            }
        }

        [Hidden]
        [Aliases("status")]
        [Command("setstatus")]
        public async Task SetStatus(CommandContext ctx) {
            if (BotAdmins.Any(x => x == ctx.Message.Author.Id)) {
                var tracker = new InteractivityEventTracker(ctx);
                var interactivity = ctx.Client.GetInteractivity();
                var availableStatuses = Database?.BotSettings?.BotStatuses.ToList();
                if (availableStatuses == null || availableStatuses.Count <= 0) {
                    await ctx.RespondAsync($":x: No statuses are available. Please add a status using `~setup addstatus~`");
                    return;
                }
                var stringBuilder = new StringBuilder();
                for (int i = 0; i < availableStatuses.Count; i++) {
                    var activity = availableStatuses[i].Activity;
                    stringBuilder.Append($"`{i + 1}.` **{activity.ActivityType.ToString()}** {activity.Name}\n");
                }
                await tracker.AskInteractivityAsync($"*Send \"cancel\" at any time to cancel any ongoing operation.*\n\nAvailable status are:\n\n" +
                    $"{stringBuilder.ToString().Trim()}\n\nPlease choose a number from 1 to {availableStatuses.Count}");
                int result = -1;
                var chosenStatusSent = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id
                                                            && ((int.TryParse(x.Content.Trim(), out result) && (int.Parse(x.Content.Trim()) >= 1 && int.Parse(x.Content.Trim()) <= availableStatuses.Count))
                                                            || x.Content.ToLower().Equals("cancel")));
                tracker.Update(chosenStatusSent);
                switch (tracker.Status) {
                    case InteractivityStatus.Cancelled:
                        await tracker.AskInteractivityAsync($":white_check_mark: Cancelled ongoing operation.");
                        break;
                    case InteractivityStatus.TimedOut:
                        await tracker.AskInteractivityAsync($":x: Hey bowsssss, timed out. Please try again.");
                        break;
                    case InteractivityStatus.Finished:
                    case InteractivityStatus.OK:
                        FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                        availableStatuses.Select(x => x.Current = false);
                        availableStatuses[result - 1].Current = true;
                        await FirebaseClient.Child("BotSettings").Child("BotStatuses").SetValueAsync(availableStatuses);
                        await ctx.RespondAsync($":white_check_mark: Successfully changed current status.");
                        break;
                    default:
                        break;
                }
                await Task.Delay(5000);
                await tracker.DeleteMessagesAsync();
            }
        }

        [Hidden]
        [Command("addstatus")]
        public async Task AddStatus(CommandContext ctx) {
            if (BotAdmins.Any(x => x == ctx.Message.Author.Id)) {
                var tracker = new InteractivityEventTracker(ctx);
                var interactivity = ctx.Client.GetInteractivity();
                var discordStatus = new DiscordStatus { Activity = DiscordStatus.DefaultDiscordActivity() };
                var availableTypes = new List<ActivityType> {
                    ActivityType.Playing,
                    ActivityType.Streaming,
                    ActivityType.ListeningTo,
                    ActivityType.Watching
                };
                var sb = new StringBuilder();
                for (int i = 0; i < availableTypes.Count; i++) {
                    try {
                        sb.Append($"`{i + 1}`. {Enum.GetName(typeof(ActivityType), availableTypes[i])}\n");
                    } catch (Exception e) {
                        Console.Write(e.StackTrace);
                        throw;
                    }
                }
                var activityTypeNumberedList = sb.ToString().Trim();
                var chosenActivityTypeResult = 0;
                await tracker.AskInteractivityAsync($"*Send \"cancel\" anytime to cancel the ongoing operation*.\n\n" +
                    $"Please choose from 1-{availableTypes.Count}:\n\n{activityTypeNumberedList}");
                var chosenActivityTypeSent = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id
                                                            && ((int.TryParse(x.Content.Trim(), out chosenActivityTypeResult) && (int.Parse(x.Content.Trim()) >= 1 && int.Parse(x.Content.Trim()) <= availableTypes.Count))
                                                            || x.Content.ToLower().Equals("cancel")));
                tracker.Update(chosenActivityTypeSent);
                if (tracker.Status == InteractivityStatus.OK) {
                    discordStatus.Activity.ActivityType = (ActivityType)(chosenActivityTypeResult-1);
                    await tracker.AskInteractivityAsync($"Oki, what would you like the status text to be?");
                    var statusTextSent = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id);
                    tracker.Update(statusTextSent);
                    if (tracker.Status == InteractivityStatus.OK) {
                        discordStatus.Activity.Name = statusTextSent.Result.Content;
                        await tracker.AskInteractivityAsync($"Finally, would you like to set this as the current status message? Reply with \"yes\" or \"no\"");
                        var setAsCurrentSent = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Message.Author.Id
                                                                                        && (x.Content.Trim().ToLower() == "yes" || x.Content.Trim().ToLower() == "no"));
                        tracker.Update(setAsCurrentSent);
                        if (tracker.Status == InteractivityStatus.OK) {
                            discordStatus.Current = setAsCurrentSent.Result.Content.Trim().ToLower().Equals("yes");
                        }
                    }
                }
                switch (tracker.Status) {
                    case InteractivityStatus.Cancelled:
                        await tracker.AskInteractivityAsync($":white_check_mark: Cancelled ongoing operation.");
                        break;
                    case InteractivityStatus.TimedOut:
                        await tracker.AskInteractivityAsync($":x: Timed out. Please try again.");
                        break;
                    case InteractivityStatus.Finished:
                    case InteractivityStatus.OK:
                        FirebaseClient = FirebaseClient ?? new YuutaFirebaseClient();
                        var currentList = Database?.BotSettings?.BotStatuses?.ToList();
                        currentList = currentList ?? new List<DiscordStatus>();
                        currentList.ForEach(x => x.Current = false);
                        currentList.Add(discordStatus);
                        await FirebaseClient.Child("BotSettings").Child("BotStatuses").SetValueAsync(currentList);
                        await tracker.AskInteractivityAsync($":white_check_mark: Successfully added status.", false);
                        break;
                    default:
                        break;
                }
                await Task.Delay(TimeSpan.FromSeconds(5));
                await tracker.DeleteMessagesAsync();
            }
        }

    }
}
