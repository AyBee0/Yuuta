using DataAccessLayer.Models.GuildModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models.GuildModels
{
    [Owned]
    [DisplayColumn("GuildSettingsId")]
    public class GuildSettings
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GuildSettingId { get; set; }

        public string WelcomeChannel { get; set; }
        public string WelcomeMessage { get; set; }
        public string GoodbyeChannel { get; set; }
        public string GoodbyeMessage { get; set; }
    }
}
