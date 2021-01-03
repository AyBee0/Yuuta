using Data.Models.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.Events
{
    public class DirectMessageEvent : Event
    {

        protected DirectMessageEvent() : base(default, default)
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
