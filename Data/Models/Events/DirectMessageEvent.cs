using Data.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.Events
{
    public class DirectMessageEvent : Event
    {
        public ulong UserToSend { get; set; }
        public string Text { get; set; }

        public override EventType EventType => EventType.DirectMessageEvent;
    }
}
