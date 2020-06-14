using DataAccessLayer.Models.GuildModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.ChannelModels
{
    public class Channel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChannelId { get; set; }
        [Required]
        public long ChannelDid { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public ChannelType ChannelType { get; set; }
        [Required]
        public int GuildId { get; set; }
        [Required]
        public Guild Guild { get; set; }
        protected Channel() { }
    }
}
