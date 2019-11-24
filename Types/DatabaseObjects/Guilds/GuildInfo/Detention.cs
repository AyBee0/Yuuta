using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Types {
    public class Detention {

        [JsonProperty("DetentionRoles")]
        private string DetentionRolesUnsplit { get; set; }

        public string DetentionChannel { get; set; }

        [JsonIgnore]
        public IEnumerable<ulong> DetentionRoles
        {
            get {
                return DetentionRolesUnsplit.Split("|").Select(ulong.Parse).ToList();
            }
            set {
                DetentionRolesUnsplit = string.Join("|", value);
            }
        }

        [JsonProperty("RolesToRemove")]
        private string RolesToRemoveUnsplit { get; set; }

        [JsonIgnore]
        public IEnumerable<ulong> RolesToRemove
        {
            get {
                return RolesToRemoveUnsplit.Split("|").Select(ulong.Parse).ToList();
            }
            set {
                RolesToRemoveUnsplit = string.Join("|", value);
            }
        }
    }
}
