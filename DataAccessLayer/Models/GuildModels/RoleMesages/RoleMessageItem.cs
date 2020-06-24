using DataAccessLayer.Models.RoleModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.GuildModels.RoleMessages
{
    public class RoleMessageItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleMessageItemId { get; set; }

        [Required]
        public string EmojiName { get; set; }
        [Required]
        public List<Role> RolesToAdd { get; set; }
        [Required]
        public string Description { get; set; }

        public int RoleMessageId { get; set; }
        public RoleMessage RoleMessage { get; set; }
    }
}
