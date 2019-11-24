using DSharpPlus.EventArgs;
using System;
using System.Linq;
using System.Threading.Tasks;
using static FirebaseHelper.YuutaFirebaseClient;

namespace DiscordEvents {

    public class GuildReactionEvents {

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

        public static async Task MessageReactionAdded(MessageReactionAddEventArgs e) {
            if (e.User.IsBot) {
                return;
            }
            var guild = Database.Guilds[e.Guild.Id.ToString()];
            var reactionMessages = guild.ReactionMessages;
            if (reactionMessages.ContainsKey(e.Message.Id.ToString())) {
                var reactionMessage = reactionMessages[e.Message.Id.ToString()];
                await e.Message.DeleteReactionAsync(e.Emoji, e.User);
                foreach (var reactionEmoji in reactionMessage.Emojis.Values) {
                    if (reactionEmoji.EmojiName == e.Emoji.GetDiscordName()) {
                        var member = await e.Guild.GetMemberAsync(e.User.Id);
                        var roles = reactionEmoji.RoleIds.Select(x => e.Guild.GetRole(x)).ToList();
                        roles.ForEach(async x => {
                            if (!member.Roles.Contains(x)) {
                                await member.GrantRoleAsync(x);
                            } else {
                                await member.RevokeRoleAsync(x);
                            }
                        });
                        var message = await e.Channel.SendMessageAsync($"{e.User.Username}, I have granted/revoked your `{string.Join(",", roles.Select(x => x.Name))}` role(s)!");
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        await message.DeleteAsync();
                        break;
                    }
                }
            }
        }

        //public static async Task MessageReactionRemoved(MessageReactionRemoveEventArgs e) {
        //    if (e.User.IsBot) {
        //        return;
        //    }
        //    var guild = Database.Guilds[e.Guild.Id.ToString()];
        //    var reactionMessages = guild.ReactionMessages;
        //    if (reactionMessages.ContainsKey(e.Message.Id.ToString())) {
        //        var reactionMessage = reactionMessages[e.Message.Id.ToString()];
        //        foreach (var reactionEmoji in reactionMessage.Emojis.Values) {
        //            if (reactionEmoji.EmojiName == e.Emoji.GetDiscordName()) {
        //                var member = await e.Guild.GetMemberAsync(e.User.Id);
        //                var roles = reactionEmoji.RoleIds.Select(x => e.Guild.GetRole(x)).ToList();
        //                roles.ForEach(async x => await member.RevokeRoleAsync(x));
        //                var message = await e.Channel.SendMessageAsync($"{e.User.Username}, I have revoked your `{string.Join(",", roles.Select(x => x.Name))}` role(s)!");
        //                await Task.Delay(TimeSpan.FromSeconds(5));
        //                await message.DeleteAsync();
        //                break;
        //            }
        //        }
        //    }
        //}

    }

}
