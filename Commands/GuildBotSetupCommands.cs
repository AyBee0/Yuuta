using AuthorityHelpers;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using FirebaseHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Types;
using static Commands.InteractivityEventTracker;
using static FirebaseHelper.YuutaFirebaseClient;

namespace Commands {

    [Group("setup")]
    public class GuildBotSetupCommands : BaseCommandModule {

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
                var channelSent = await interactivity.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Author.Id == ctx.Member.Id && x.MentionedChannels?.Count > 0);
                tracker.Update(channelSent);
                if (tracker.Status == InteractivityStatus.OK) {
                    detentionObj.DetentionChannel = channelSent.Result.MentionedChannels[0].Id;
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
                                            && x.Content.Replace("@", "").Split(",").Select(y => y.Trim().ToLower()).ToList().Any(z => guildRoles.Select(w => w.Name.ToLower()).Contains(z)));
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
            var tracker = new InteractivityEventTracker(ctx);
            var interactivity = ctx.Client.GetInteractivity();
            var guildRoles = ctx.Guild.Roles.Values;
            await tracker.AskInteractivityAsync($"*Send \"cancel\" to cancel at anytime.*\n\n Staff roles have the ability to create events, setup detention, etc.\n" +
                $"Please send the staff role names below, seperated by commas. You may mention the roles. Please just seperate the roles by commas I ain't spending" +
                $" the entire day trying to parse your laziness.");
            var staffRolesSent = await interactivity.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Author.Id == ctx.Member.Id
                                            && x.Content.Replace("@", "").Split(",").Select(y => y.Trim().ToLower()).ToList().Any(z => guildRoles.Select(w => w.Name.ToLower()).Contains(z)));
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
                    .Where(guildRole => staffRolesSent.Result.Content.Replace("@", "").Split(",").Select(x => x.Trim().ToLower()).Any(givenRole => guildRole.Name.ToLower() == givenRole))
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
                                                                                && x.MentionedChannels.Count > 0);
                tracker.Update(botChannelsSent);
                if (tracker.Status == InteractivityStatus.OK) {
                    var botChannels = botChannelsSent.Result.MentionedChannels.Select(x => x.Id).ToList();
                    await tracker.AskInteractivityAsync($"What roles should I exclude in this limitation? Send \"none\" to apply this for all non-staff members.");
                    var exclusionRolesSent = await interactivity.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Author.Id == ctx.Member.Id
                                            && (x.Content.Replace("@", "").Split(",").Select(y => y.Trim().ToLower()).ToList().Any(z => guildRoles.Select(w => w.Name.ToLower()).Contains(z))
                                            || x.Content.Trim().ToLower() == "none"));
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

    }
}
