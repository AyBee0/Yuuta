using Generatsuru;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.RoleModels
{
    public enum RoleTypeEnum
    {
        Normal,
        GlobalCommands,
        GlobalMacros,
        Staff
    }

    public class RoleType
    {
        private RoleType(RoleTypeEnum @enum)
        {
            RoleTypeId = (int)@enum;
            Name = @enum.ToString();
            Description = @enum.GetEnumDescription();
        }

        protected RoleType() { } //For EF

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RoleTypeId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public static implicit operator RoleType(RoleTypeEnum @enum) => new RoleType(@enum);
        public static implicit operator RoleTypeEnum(RoleType roleType) => (RoleTypeEnum)roleType.RoleTypeId;
    }
}
