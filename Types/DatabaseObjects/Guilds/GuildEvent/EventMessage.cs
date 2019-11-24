using System;
using System.Collections.Generic;
using System.Text;

namespace Types {
    public class EventMessage {
        public string EmojiID { get; set; }
        public string MessageId { get; set; }
        public string ChannelId { get; set; }
        public string ReminderMessage { get; set; }
        public string EmojiName { get; set; }
    }
}
