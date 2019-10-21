using System;
using System.Collections.Generic;
using System.Text;

namespace Types {

    public class Attachment {
        public string AttachmentURL { get; set; }
    }

    public class UserID {
        public bool Send { get; set; }
    }

    public class RoleEvent {
        public RoleActionTypes.RoleActionType RoleActionType { get; set; }
    }

    public class GuildEvent {
        public EventType.DiscordEventType EventType { get; set; }
        public string MessageText { get; set; }
        public Dictionary<string, Attachment> Attachments { get; set; }
        public Dictionary<string, RoleEvent> Roles { get; set; }
        public Dictionary<string, UserID> UserIDs { get; set; }
        public string Date { get; set; }
        public string MessageChannel { get; set; }
        public string GuildID { get; set; }
    }

    public class Guild {
        public Dictionary<string, GuildEvent> GuildEvents { get; set; }
        public Dictionary<string, GuildMacro> GuildMacros { get; set; }
        public Info Info { get; set; }
    }

    public class GuildMacro {
        public Dictionary<string,Attachment> Attachments { get; set; }
        public string MessageResponse { get; set; }
        public string Macro { get; set; }
        public bool DeleteCommand { get; set; }
    }

    public class Info {
        public string InviteLink { get; set; }
        public string Name { get; set; }
        public string ServerDescription { get; set; }
        public Welcome Welcome { get; set; }
        public Leave Leave { get; set; }
    }

    public class Welcome {
        public ulong Channel { get; set; }
        public bool Enabled { get; set; }
        public string Message { get; set; }
    }

    public class Leave {
        public ulong Channel { get; set; }
        public bool Enabled { get; set; }
        public string Message { get; set; }
    }

    public class Root {
        public Dictionary<string, Guild> Guilds { get; set; }
    }

}
