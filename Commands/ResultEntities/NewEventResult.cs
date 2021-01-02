using DSharpPlus.Entities;
using InteractivityHelpers.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.ResultEntities
{
    public class NewEventResult : ResultEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<DiscordRole> Mentions { get; set; }
        public DateTime EventDate { get; set; }
        public List<DiscordChannel> RelatedChannels { get; set; }
        public string ReminderMessage { get; set; }
        public string EventRegion { get; set; }
        public string EventPlatform { get; set; }
        public string Countdown { get; set; }
        public DiscordAttachment Thumbnail { get; set; }
        public DiscordChannel ResultChannel { get; set; }

    }
}
