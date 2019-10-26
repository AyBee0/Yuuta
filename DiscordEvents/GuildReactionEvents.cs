using DSharpPlus.EventArgs;
using System;
using System.Threading.Tasks;

namespace DiscordEvents {

    public class GuildReactionEvents : DiscordEvent {

        public static async Task MessageReactionAdded(MessageReactionAddEventArgs e) {
            if (e.User.IsBot) {
                return;
            }
            var guild = Guilds[e.Channel.GuildId.ToString()];
            var reactionMessages = guild.ReactionMessages;
            if (reactionMessages.ContainsKey(e.Message.Id.ToString())) {
                var message = reactionMessages[e.Message.Id.ToString()];
                var emojis = message.Emojis;
                if (emojis.ContainsKey(e.Emoji.Id.ToString())) {
                    var emoji = emojis[e.Emoji.Id.ToString()];
                    var role = e.Channel.Guild.GetRole(emoji.RoleID);
                    if (role != null) {
                        var member = await e.Channel.Guild.GetMemberAsync(e.User.Id);
                        await member.GrantRoleAsync(role);  
                        var confirmMessage = await e.Channel.SendMessageAsync($"{e.User.Username}, I have granted you the {emoji.RoleName} role!");
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        await confirmMessage.DeleteAsync();
                    }   
                } else {
                    return;
                }
            }
        }

        public static async Task MessageReactionRemoved(MessageReactionRemoveEventArgs e) {
            if (e.User.IsBot) {
                return;
            }
            var guild = Guilds[e.Channel.GuildId.ToString()];
            var reactionMessages = guild.ReactionMessages;
            if (reactionMessages.ContainsKey(e.Message.Id.ToString())) {
                var message = reactionMessages[e.Message.Id.ToString()];
                var emojis = message.Emojis;
                if (emojis.ContainsKey(e.Emoji.Id.ToString())) {
                    var emoji = emojis[e.Emoji.Id.ToString()];
                    var role = e.Channel.Guild.GetRole(emoji.RoleID);
                    if (role != null) {
                        var member = await e.Channel.Guild.GetMemberAsync(e.User.Id);
                        await member.RevokeRoleAsync(role);
                        var confirmMessage = await e.Channel.SendMessageAsync($"{e.User.Username}, I have revoked your {emoji.RoleName} role!");
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        await confirmMessage.DeleteAsync();
                    }
                } else {
                    return;
                }
            }
        }

    }

}
