using Generatsuru;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.CommandModels
{
    public enum CommandTypeEnum
    {
        Normal, ForcedGlobal, StaffOnly
    }

    public class CommandType
    {
        private CommandType(CommandTypeEnum @enum)
        {
            CommandTypeId = (int)@enum;
            Name = @enum.ToString();
            Description = @enum.GetEnumDescription();
        }

        protected CommandType() { } //For EF

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CommandTypeId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public static implicit operator CommandType(CommandTypeEnum @enum) => new CommandType(@enum);
        public static implicit operator CommandTypeEnum(CommandType commandType) => (CommandTypeEnum)commandType.CommandTypeId;
    }
}