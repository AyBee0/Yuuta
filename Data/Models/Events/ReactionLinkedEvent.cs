using Data.Models.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.Events
{
    public class ReactionLinkedEvent : IEvent
    {
        protected ReactionLinkedEvent()
        {
        }

        [Key]
        public int ReactionLinkedEventId { get; set; }

        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public string EmojiName { get; set; }

        public int EventId { get; set; }

        public EventType EventType { get; private set; }

        public ReactionLinkedEvent(EventType eventType,
            int eventId,
            ulong guildId,
            ulong channelId,
            ulong messageId)
        {
            this.EventType = eventType;
            this.GuildId = guildId;
            this.ChannelId = channelId;
            this.MessageId = messageId;
            this.EventId = eventId;
        }

        public ReactionLinkedEvent(EventType eventType, ulong channelId, ulong messageId, DirectMessageEvent dmEvent)
            : this(eventType, dmEvent.EventId, dmEvent.GuildId, channelId, messageId)
        {
        }

    }
}
