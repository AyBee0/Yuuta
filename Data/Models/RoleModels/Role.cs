using DataAccessLayer.Models.GuildModels;
using DSharpPlus.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models.RoleModels
{

    public enum RoleType
    {
        Normal,
        GlobalCommands,
        GlobalMacros,
        Staff
    }

    public class Role
    {

        [Required]
        public virtual RoleType RoleType { get; set; } 

        [Required]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public ulong RoleDid { get; set; }

        public int GuildId { get; set; }
        public Guild Guild { get; set; }

        internal Role() { }

        public Role(DiscordRole role)
        {
            this.Title = role.Name;
            this.RoleDid = role.Id;
        }

    }
}
