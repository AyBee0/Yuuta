using Data.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.Events
{
    public enum RoleEventType
    {
        Deny,
        Grant,
    }
    public class RoleEvent : Event
    {
        public RoleEvent(DateTime eventDate, ulong guildId) : base(eventDate, guildId)
        {
        }

        public ulong User { get; set; }
        public ulong RoleId { get; set; }
        public RoleEventType RoleEventType { get; set; }

        public override EventType EventType => EventType.RoleEvent;
    }
}
