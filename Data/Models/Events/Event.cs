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

    public interface IEvent
    {
        public int EventId { get; set; }
        public ulong GuildId { get; set; }
        EventType EventType { get; }
    }

    public abstract class Event : IEvent
    {
        [Key]
        public int EventId { get; set; }

        public DateTime EventDate { get; set; }

        public ulong GuildId { get; set; }

        public Event(DateTime eventDate, ulong guildId)
        {
            EventDate = eventDate;
            GuildId = guildId;
        }

        public abstract EventType EventType { get; }
    }
}

