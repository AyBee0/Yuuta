using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Events {

    public class Attachment {
        public string AttachmentURL { get; set; }
    }

    public class UserID {
        public bool Send { get; set; }
    }

    public class GuildEvent {
        public int EventType { get; set; }
        public string MessageText { get; set; }
        public List<Attachment> Attachments { get; set; }
        public Dictionary<string, UserID> UserIDs;
    }

    public class Guild {
        public Dictionary<string,GuildEvent> GuildEvents { get; set; }
    }   

    public class Root {
        public Dictionary<string, Guild> Guilds { get; set; }
    }

}
