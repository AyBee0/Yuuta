using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerVars {
    public class ServerRoleIds {

        public List<ulong> TheBeaconStaffIds { get; }
        public List<ulong> NoobsStaffIds { get; }
        public List<ulong> TestStaffIds { get; }

        public List<ulong> TheBeaconDetentionIds { get; }

        public ServerRoleIds() {
            #region The Beacon
            TheBeaconStaffIds = new List<ulong> {
                346643751235747851
            };
            #endregion
            #region noobs
            NoobsStaffIds = new List<ulong> {
                564683492953423875
            };
            #endregion
            #region Test
            TestStaffIds = new List<ulong> {
                602229969351082036
            };
            #endregion

        }

    }
}
