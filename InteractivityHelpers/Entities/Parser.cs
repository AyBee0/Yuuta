using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InteractivityHelpers.Entities
{
    public class Parser
    {
        public Func<DiscordMessage, dynamic> Exec { get; set; }

        public Parser(Func<DiscordMessage, dynamic> exec)
        {
            Exec = exec;
        }
    }

}
