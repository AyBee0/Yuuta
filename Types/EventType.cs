using System;
using System.Collections.Generic;
using System.Text;

namespace Types {

    public class EventType {

        public enum DiscordEventType {
            DM = 0,
            Message = 1,
            RoleAction = 2,
            RevokeRole = 3,
        }

    }

}
