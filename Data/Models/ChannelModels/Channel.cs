using DataAccessLayer.Models.GuildModels;
using DSharpPlus.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models.ChannelModels
{
    public class Channel : Entity<DiscordChannel>
    {
        public Channel(DiscordChannel discordObj) : base(discordObj)
        {
            this.ChannelDid = (long) discordObj.Id;
            this.Title = discordObj.Name;
            this.ChannelType = ChannelTypeEnum.Normal;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChannelId { get; set; }
        [Required]
        public long ChannelDid { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public ChannelType ChannelType { get; set; } = ChannelTypeEnum.Normal;
        [Required]
        public int GuildId { get; set; }
        [Required]
        public Guild Guild { get; set; }
    }
}
