using System;
using System.Collections.Generic;
using System.Text;

namespace Types.GuildCommands {

    public class GuildCommand {
        public string Response { get; set; }
        public List<Attachment> Attachments { get; set; }
    }

}
