using DataAccessLayer.Models.GuildModels;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int GuildId { get; set; }

        public abstract EventType EventType { get; }

    }
}