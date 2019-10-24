using System;
using System.Collections.Generic;
using System.Text;

namespace Types {

    public class ServerInfo {
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

}
