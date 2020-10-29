using DataAccessLayer.Models.GuildModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models.CommandModels
{
    public class CommandRestrictionOverload 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RestrictionOverloadId { get; set; }
        [Required]
        public bool Authorize { get; set; }
        [Required]
        public ulong RoleDid { get; set; }
        [Required]
        public ulong GuildDid { get; set; }

        public int CommandId { get; set; }
        public YuutaCommand Command { get; set; }

    }
}
