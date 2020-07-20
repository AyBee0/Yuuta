using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InteractivityHelpers.Entities
{
    public interface IParser { }
    public class Parser<T>
    {
        public Func<DiscordMessage, T> Exec { get; set; }

        public Parser(Func<DiscordMessage, T> exec)
        {
            Exec = exec;
        }
    }

}
