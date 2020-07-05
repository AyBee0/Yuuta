using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Linq;

namespace DiscordMan
{
    public class DiscordChannelMan
    {
        public DiscordChannel Channel { get; set; }
        public bool BotChannel { get; set; }
        public DiscordChannelMan(DiscordChannel channel)
        {
            this.Channel = channel;
            this.BotChannel = IsBotChannel();
        }
        public DiscordChannelMan(CommandContext ctx) : this(ctx.Channel) {}

        private bool IsBotChannel()
        {
            var dGuildAL = new DiscordGuildMan(Channel);
            var botChannelIds = dGuildAL.BotChannels
                .Select(Channel => Channel.ChannelDid);
            // It's a bot Channel
            return botChannelIds.Any(bChannelId => bChannelId == (long)Channel.Id);
        }

    }
}
