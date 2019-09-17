using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Commands {
    class Guild {

        public class Command {
            public string CommandTrigger { get; set; }
            public string ImageURL { get; set; }
            public string Response { get; set; }
            public bool RestrictedPerms { get; set; }
            public bool StaffOnly { get; set; }
        }

        public class Commands {
            public Command CustomCommand { get; set; }
        }

        public class Leave {
            public string Channel { get; set; }
            public bool Enabled { get; set; }
            public string Message { get; set; }
        }

        public class Welcome {
            public string Channel { get; set; }
            public bool Enabled { get; set; }
            public string Message { get; set; }
        }

        public class RootObject {
            public Commands Commands { get; set; }
            public string InviteLink { get; set; }
            public Leave Leave { get; set; }
            public string Name { get; set; }
            public string ServerDescription { get; set; }
            public Welcome Welcome { get; set; }
        }

    }
}
