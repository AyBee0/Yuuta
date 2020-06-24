using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Linq;

namespace DiscordAccessLayer
{
    public class DiscordChannelAL
    {
        public DiscordChannel Channel { get; set; }
        public bool BotChannel { get; set; }
        public DiscordChannelAL(DiscordChannel channel)
        {
            this.Channel = channel;
            this.BotChannel = IsBotChannel();
        }
        public DiscordChannelAL(CommandContext ctx) : this(ctx.Channel) {}

        private bool IsBotChannel()
        {
            var dGuildAL = new DiscordGuildAL(Channel);
            var botChannelIds = dGuildAL.BotChannels
                .Select(Channel => Channel.ChannelDid);
            // It's a bot Channel
            return botChannelIds.Any(bChannelId => bChannelId == (long)Channel.Id);
        }

    }
}
