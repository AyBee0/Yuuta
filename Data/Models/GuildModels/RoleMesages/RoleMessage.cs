using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.GuildModels.RoleMessages
{
    public class RoleMessage
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleMessageId { get; set; }
        public List<RoleMessageItem > RoleMessageItems { get; set; } = new List<RoleMessageItem>();

        [Required]
        public long ChannelDid { get; set; }
        [Required]
        public long MessageDid { get; set; }

        public int GuildId { get; set; }
        public Guild Guild { get; set; }
    }
}
