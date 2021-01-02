using Data.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.Events
{
    public class GuildMessageEvent : Event
    {
        public GuildMessageEvent(DateTime eventDate, ulong guildId) : base(eventDate, guildId)
        {
        }

        public ulong ChannelToSend { get; set; }
        public string Text { get; set; }

        public override EventType EventType => EventType.GuildMessageEvent;
    }
}
