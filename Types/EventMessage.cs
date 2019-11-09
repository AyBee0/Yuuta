using System;
using System.Collections.Generic;
using System.Text;

namespace Types {
    public class EventMessage {
        public ulong EmojiID { get; set; }
        public ulong MessageId { get; set; }
        public ulong ChannelId { get; set; }
        public string ReminderMessage { get; set; }
        public string EmojiName { get; set; }
    }
}
