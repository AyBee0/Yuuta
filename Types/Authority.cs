using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Types {

    public class Authority {

        [JsonProperty("GlobalBotChannels")]
        private string GlobalBotChannelsUnsplit { get; set; }

        [JsonIgnore]
        public IReadOnlyList<ulong> GlobalBotChannels
        {
            get {
                return GlobalBotChannelsUnsplit.Split(",").ToList().Select(ulong.Parse).ToList();
            }
        }

        [JsonProperty("GlobalBotRoleOverrides")]
        private string GlobalBotRoleOverridesUnsplit { get; set; }

        [JsonIgnore]
        public IReadOnlyList<ulong> GlobalBotRoleOverrides
        {
            get {
                return GlobalBotRoleOverridesUnsplit.Split(",").ToList().Select(ulong.Parse).ToList();
            }
        }

        [JsonProperty("StaffRoles")]
        private string StaffRolesUnsplitMethod { get; set; }

        [JsonIgnore]
        public IReadOnlyList<ulong> StaffRoles
        {
            get {
                if (StaffRolesUnsplitMethod == null) {
                    Console.WriteLine("Staffrolesunsplit is null!");
                }
                return StaffRolesUnsplitMethod?.Split(",").ToList().Select(ulong.Parse).ToList();
            }
        }

    }

}
