using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models.GuildModels
{
    public class GuildMacro
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GuildMacroId { get; set; }
        [Required]
        public bool DeleteAfterSend { get; set; }
        [Required]
        public string Response { get; set; }
        public List<Attachment> Attachments { get; set; }

        public int GuildId { get; set; }
        public Guild Guild { get; set; }
    }

    public class Attachment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttachmentId { get; set; }
        [Required]
        public string AttachmentUrl { get; set; }

        public int GuildMacroId { get; set; }
        public GuildMacro GuildMacro { get; set; }
    }

}
