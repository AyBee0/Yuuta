using System;
using System.Collections.Generic;
using System.Text;

namespace Types {
    public class GuildMacro {
        public Dictionary<string, DiscordAttachment> Attachments { get; set; }
        public string MessageResponse { get; set; }
        public string Macro { get; set; }
        public bool DeleteCommand { get; set; }
    }
}
