using DataAccessLayer.Models.GuildModels;
using DSharpPlus.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models.ChannelModels
{
    public class Channel
    {
        public Channel()
        {

        }
        public Channel(DiscordChannel discordObj) 
        {
            this.ChannelDid = discordObj.Id;
            this.Title = discordObj.Name;
            this.ChannelType = ChannelType.Normal;
        }

        [Key]
        public ulong ChannelDid { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public ChannelType ChannelType { get; set; } = ChannelType.Normal;
        [Required]
        public int GuildId { get; set; }
        [Required]
        public Guild Guild { get; set; }
    }
}
