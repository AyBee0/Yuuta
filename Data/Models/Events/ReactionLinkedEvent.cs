using Data.Models.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.Events
{
    [Keyless]
    public class ReactionLinkedEvent : IEvent
    {
        protected ReactionLinkedEvent()
        {
        }

        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public string EmojiName { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public EventType EventType { get; private set; }

        public ReactionLinkedEvent(EventType eventType,
            int eventId,
            ulong guildId,
            ulong channelId,
            ulong messageId)
        {
            this.EventType = EventType;
            this.GuildId = guildId;
            this.ChannelId = channelId;
            this.MessageId = messageId;
            this.EventId = eventId;
        }

        public ReactionLinkedEvent(EventType eventType, ulong channelId, ulong messageId, DirectMessageEvent dmEvent)
            : this(eventType, dmEvent.EventId, dmEvent.Guild.GuildId, channelId, messageId)
        {
        }

    }
}
