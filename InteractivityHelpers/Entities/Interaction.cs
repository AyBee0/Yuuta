using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InteractivityHelpers.Entities
{
    public class Interaction
    {
        public string AskMessage { get; set; }
        public InteractionConfig Config { get; set; } = new InteractionConfig();
        public Func<DiscordMessage, bool> Condition { get; set; }
        public Parser Parser { get; set; }

        public Interaction(string askMessage, Parser parser, Func<DiscordMessage,bool> condition = null)
        {
            this.AskMessage = askMessage;
            this.Parser = parser;
            this.Condition = condition;
        }
    }
}