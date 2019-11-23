using System;
using System.Collections.Generic;
using System.Text;

namespace Types.DatabaseObjects {

    public class Command {
        public string Response { get; set; }
        public List<DiscordAttachment> Attachments { get; set; }
    }

}
