using Generatsuru;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.ChannelModels
{
    public enum ChannelTypeEnum
    {
        Normal, BotChannel
    }
    public class ChannelType
    {
        private ChannelType(ChannelTypeEnum @enum)
        {
            ChannelTypeId = (int)@enum;
            Name = @enum.ToString();
            Description = @enum.GetEnumDescription();
        }

        protected ChannelType() { } //For EF

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ChannelTypeId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public static implicit operator ChannelType(ChannelTypeEnum @enum) => new ChannelType(@enum);
        public static implicit operator ChannelTypeEnum(ChannelType channelType) => (ChannelTypeEnum)channelType.ChannelTypeId;
    }
}
