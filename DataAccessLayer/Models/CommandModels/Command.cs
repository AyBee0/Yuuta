using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.CommandModels
{
    public class Command
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommandId { get; set; }
        [Required]
        public string CommandTrigger { get; set; }

        [Required]
        public CommandType CommandType { get; set; }
        [Required]
        public CommandTypeEnum CommandTypeEnum { get; set; } = CommandTypeEnum.Normal;

        public List<CommandRestrictionOverload> RestrictionOverloads { get; set; }
    }
}
