using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.Events
{
    public class ReactionLinkedEvent : DirectMessageEvent
    {
        protected ReactionLinkedEvent()
        {
        }

        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }

        public ReactionLinkedEvent(DateTime eventDate, ulong guildId, string text, ulong channelId, ulong messageId, List<EventUser> initialUsers = null) : base(eventDate, guildId, text, initialUsers)
        {
            this.ChannelId = channelId;
            this.MessageId = messageId;
        }
    }
}
