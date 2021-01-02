using DataAccessLayer.Models.GuildModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.Events
{
    public enum EventType
    {
        DirectMessageEvent,
        RoleEvent,
        GuildMessageEvent
    }
    public abstract class Event
    {
        [Key]
        public int EventId { get; set; }

        public DateTime EventDate { get; set; }

        public Guild Guild { get; set; }
        public ulong GuildId { get; set; }

        public Event(DateTime eventDate, ulong guildId)
        {
            EventDate = eventDate;
            GuildId = guildId;
        }

        public Event()
        {

        }

        public abstract EventType EventType { get; }

    }
}

