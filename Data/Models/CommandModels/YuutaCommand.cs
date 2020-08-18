using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.CommandModels
{
    public class YuutaCommand
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommandId { get; set; }
        [Required]
        public string CommandName { get; set; }

        [Required]
        public CommandType CommandType { get; set; }

        public List<CommandRestrictionOverload> RestrictionOverloads { get; set; } = new List<CommandRestrictionOverload>();
        protected YuutaCommand()
        {

        }
        public YuutaCommand(DSharpPlus.CommandsNext.Command command)
        {
            CommandName = command.Name;
        }
    }
}
