﻿using Data.Models.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.Events
{
    public class EventUser
    {
        [Key]
        public int EventUserId { get; set; }
        public ulong EventUserDid { get; set; }
        
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
