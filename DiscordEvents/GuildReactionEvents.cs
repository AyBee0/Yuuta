using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static FirebaseHelper.YuutaFirebaseClient;

namespace DiscordEvents
{

    public class GuildReactionEvents
    {

        //public static async Task MessageReactionAdded(MessageReactionAddEventArgs e) {
        //    if (e.User.IsBot) {
        //        return;   
        //    }
        //    var guild = .YuutaBot.Guilds[e.Channel.GuildId.ToString()];
        //    var reactionMessages = guild.ReactionMessages;
        //    if (reactionMessages.ContainsKey(e.Message.Id.ToString())) {
        //        var message = reactionMessages[e.Message.Id.ToString()];
        //        var emojis = message.Emojis;
        //        if (emojis.ContainsKey(e.Emoji.Id.ToString())) {
        //            var emoji = emojis[e.Emoji.Id.ToString()];
        //            //var role = e.Channel.Guild.GetRole(emoji.RoleID);
        //            var roles = e.Channel.Guild.Roles.Values.Where(x => emoji.RoleIds.Contains(x.Id));
        //            foreach (var role in roles) {
        //                if (role != null) {
        //                    var member = await e.Channel.Guild.GetMemberAsync(e.User.Id);
        //                    await member.GrantRoleAsync(role);
        //                }
        //            }
        //            var confirmMessage = await e.Channel.SendMessageAsync($"{e.User.Username}, I have granted you the {roles.Select(x => x.Name)} role(s)!");
        //            await Task.Delay(TimeSpan.FromSeconds(5));
        //            await confirmMessage.DeleteAsync();
        //        } else {
        //            return;
        //        }
        //    }
        //}

        //public static async Task MessageReactionRemoved(MessageReactionRemoveEventArgs e) {
        //    if (e.User.IsBot) {
        //        return;
        //    }
        //    var guild = .YuutaBot.Guilds[e.Channel.GuildId.ToString()];
        //    var reactionMessages = guild.ReactionMessages;
        //    if (reactionMessages.ContainsKey(e.Message.Id.ToString())) {
        //        var message = reactionMessages[e.Message.Id.ToString()];
        //        var emojis = message.Emojis;
        //        if (emojis.ContainsKey(e.Emoji.Id.ToString())) {
        //            var emoji = emojis[e.Emoji.Id.ToString()];
        //            //var role = e.Channel.Guild.GetRole(emoji.RoleID);
        //            var roles = e.Channel.Guild.Roles.Values.Where(x => emoji.RoleIds.Contains(x.Id));
        //            foreach (var role in roles) {
        //                if (role != null) {
        //                    var member = await e.Channel.Guild.GetMemberAsync(e.User.Id);
        //                    await member.RevokeRoleAsync(role);
        //                }
        //            }
        //            var confirmMessage = await e.Channel.SendMessageAsync($"{e.User.Username}, I have revoked your {roles.Select(x => x.Name)} role(s)!");
        //            await Task.Delay(TimeSpan.FromSeconds(5));
        //            await confirmMessage.DeleteAsync();
        //        } else {
        //            return;
        //        }
        //    }
        //}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static async Task MessageReactionChange
            (DiscordMember member,
            DiscordGuild dGuild,
            DiscordMessage message,
            DiscordEmoji emoji,
            DiscordChannel channel,
            bool added)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (member.IsBot)
            {
                return;
            }
            // Get the guild
            var guild = Database.Guilds[dGuild.Id.ToString()];
            // Get the reaction messages
            Dictionary<string, Types.ReactionMessage> reactionMessages = guild.ReactionMessages;
            // If it's a valid message
            if (reactionMessages.ContainsKey(message.Id.ToString()))
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                HandleRoleReaction(message.Id.ToString(), emoji, dGuild, member, channel, reactionMessages, added);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        private static async Task HandleRoleReaction(
            string messageId,
            DiscordEmoji emoji,
            DiscordGuild guild,
            DiscordMember user,
            DiscordChannel channel,
            Dictionary<string, Types.ReactionMessage> reactionMessages,
            bool added)
        {
            var reactionMessage = reactionMessages[messageId];
            //await e.Message.DeleteReactionAsync(e.Emoji, e.User);
            foreach (var reactionEmoji in reactionMessage.Emojis.Values)
            {
                if (reactionEmoji.EmojiName == emoji.GetDiscordName())
                {
                    var member = await guild.GetMemberAsync(user.Id);
                    var roles = reactionEmoji.RoleIds.Select(x => guild.GetRole(x)).ToList();
                    DiscordMessage message = null;
                    foreach (var role in roles)
                    {
                        if (added)
                        {
                            await member.GrantRoleAsync(role);
                            message = await channel.SendMessageAsync($"{user.Username}, I have granted your `{string.Join(",", roles.Select(x => x.Name))}` role(s)!");
                        }
                        else
                        {
                            await member.RevokeRoleAsync(role);
                            message = await channel.SendMessageAsync($"{user.Username}, I have revoked your `{string.Join(",", roles.Select(x => x.Name))}` role(s)!");
                        }
                    }
                    if (message != null)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        await message.DeleteAsync();
                    }
                    break;
                }
            }
        }
    }

}
