using Data.Models.Events;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Models.Events
{
    public class DirectMessageEvent : Event
    {
        public DirectMessageEvent() : base(default, default)
        {
            
        }

        public DirectMessageEvent(DateTime eventDate, ulong guildId, string text) : base(eventDate, guildId)
        {
            this.Text = text;
        }

        public List<EventUser> UserToSend { get; set; } = new List<EventUser>();
        public string Text { get; set; }

        public override EventType EventType => EventType.DirectMessageEvent;
    }
}
