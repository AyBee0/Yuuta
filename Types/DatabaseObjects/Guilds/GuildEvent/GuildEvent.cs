using System;
using System.Collections.Generic;
using System.Text;

namespace Types {

    public class RoleEvent {
        public RoleActionTypes.RoleActionType RoleActionType { get; set; }
    }

    public class GuildEvent {
        public EventType.DiscordEventType EventType { get; set; }
        public string MessageText { get; set; }
        public Dictionary<string, DiscordAttachment> Attachments { get; set; }
        public Dictionary<string, RoleEvent> Roles { get; set; }
        public Dictionary<string, DiscordUserID> UserIds { get; set; }
        public string Date { get; set; }    
        public string MessageChannel { get; set; }
        public ulong GuildID { get; set; }
        public Dictionary<string,EventMessage> ReactionEventMessage { get; set; } 
    }
}   