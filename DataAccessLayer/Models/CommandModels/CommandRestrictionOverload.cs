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
        public int RoleDid { get; set; }

        public int CommandId { get; set; }
        public Command Command { get; set; }

    }
}
