using AuthorityHelpers;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Commands {

    public class StaffCommands : BaseCommandModule {

        [Description("Clears a given amount of messages." +
            "Usage:\n" +
            "`~clear 100`\n" +
            "Where 100 (max) is the amount of messages to delete in this channel.\n\nYou can also pass a user, like so:\n`~clear 100 @User#12345`")]
        [Command("clear")]
        public async Task Clear(CommandContext ctx, int numberOfMessages, DiscordUser userToDelete = null) {
            if (ctx.IsStaffMember()) {
                if (numberOfMessages > 100) {
                    await ctx.RespondAsync($":x: Boi, I can't do more than 100; alright? 100, take it or leave it-or do it again.");
                } else {
                    await ctx.Message.DeleteAsync();
                    IReadOnlyList<DiscordMessage> messages = new List<DiscordMessage>();
                    if (userToDelete != null) {
                        var messagesUnspecified = await ctx.Channel.GetMessagesAsync(500);
                        messages = messagesUnspecified.Where(x => x.Author.Id == userToDelete.Id).Take(numberOfMessages).ToList();
                    } else {
                        messages = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message.Id, numberOfMessages);
                    }
                    await ctx.Channel.DeleteMessagesAsync(messages, $"~clear command by {ctx.Member.DisplayName}");
                    await ctx.RespondAsync($":white_check_mark: Deleted {messages.Count} messages, now praise me. __Unless you wanted me to delete messages before like" +
                        $" two weeks ago, because in that case, API-san says no.__");
                }
            }
        }

        //[RequireUserPermissions(DSharpPlus.Permissions.KickMembers)]
        [Description("Kick a member. You can optionally pass a reason.")]
        [Command("kick")]
        public async Task Kick(CommandContext ctx, DiscordMember memberToKick, string reason = null) {
            if (ctx.Member.Roles.Any(x => x.Permissions.HasFlag(Permissions.KickMembers))) {
                await memberToKick.RemoveAsync(reason);
            }
        }

    }
}