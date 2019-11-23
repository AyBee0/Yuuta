using DSharpPlus.EventArgs;
using System.Threading.Tasks;
using static FirebaseHelper.YuutaFirebaseClient;

namespace DiscordEvents {
    public class GuildMemberEvents{

        public static async Task GuildMemberAdded(GuildMemberAddEventArgs e) {
            var guildID = e.Guild.Id;
            var member = e.Member;
            var guildInfo = Database.Guilds[guildID.ToString()].Info;
            var guildWelcomeInfo = guildInfo.Welcome;
            if (guildWelcomeInfo == null) {
                return;
            }
            if (guildWelcomeInfo.Enabled) {
                var message = guildWelcomeInfo.Message.Replace("{MENTION}", member.Mention).Replace("{MEMBER}", member.Nickname).Replace("{SERVER}", guildInfo.Name);
                var guild = await e.Client.GetGuildAsync(guildID);
                var channel = guild.GetChannel(guildWelcomeInfo.Channel);
                await channel.SendMessageAsync(message);
            }
        }

        public static async Task GuildMemberRemoved(GuildMemberRemoveEventArgs e) {
            var guilds = Database.Guilds;
            var guildID = e.Guild.Id;
            var member = e.Member;
            var guildInfo = guilds[guildID.ToString()].Info;
            var guildLeaveInfo = guildInfo.Leave;
            if (guildLeaveInfo == null) {
                return;
            }
            if (guildLeaveInfo.Enabled) {
                var message = guildLeaveInfo.Message.Replace("{MENTION}", member.Mention).Replace("{MEMBER}", member.Nickname).Replace("{SERVER}", guildInfo.Name);
                var guild = await e.Client.GetGuildAsync(guildID);
                var channel = guild.GetChannel(guildLeaveInfo.Channel);
                await channel.SendMessageAsync(message);
            }
        }

    }
}
