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
        public IEnumerable<ulong> GlobalBotChannels
        {
            get {
                return GlobalBotChannelsUnsplit?.Split("|").ToList().Select(x => ulong.Parse(x.Trim())).ToList();
            }
            set {
                GlobalBotChannelsUnsplit = string.Join("|", value);
            }
        }

        [JsonProperty("GlobalBotRoleOverrides")]
        private string GlobalBotRoleOverridesUnsplit { get; set; }

        [JsonIgnore]
        public IEnumerable<ulong> GlobalBotRoleOverrides
        {
            get {
                try {
                    return GlobalBotRoleOverridesUnsplit?.Split("|").Select(x => ulong.Parse(x.Trim())).ToList();
                } catch (Exception e) {
                    Console.WriteLine(e.StackTrace);
                    throw;
                }
            }
            set {
                GlobalBotRoleOverridesUnsplit = string.Join("|", value);
            }
        }

        [JsonProperty("StaffRoles")]
        private string StaffRolesUnsplit { get; set; }

        [JsonIgnore]
        public IEnumerable<ulong> StaffRoles
        {
            get {
                if (StaffRolesUnsplit == null) {
                    Console.WriteLine("Staffrolesunsplit is null!");
                }
                return StaffRolesUnsplit?.Split("|").Select(x => ulong.Parse(x.Trim())).ToList();
            }
            set {
                StaffRolesUnsplit = string.Join("|", value);
            }
        }

    }

}
