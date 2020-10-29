using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;

namespace InteractivityHelpers.Entities
{
    public class Parser
    {
        private readonly Func<DiscordMessage, object> func;

        public Parser(Func<DiscordMessage, object> exec)
        {
            func = exec;
        }

        public bool TryParse(InteractivityResult<DiscordMessage> toParse, out object result, out string message)
        {
            try
            {
                result = func(toParse.Result);
                message = null;
                return true;
            }
            catch (ParseException e)
            {
                result = null;
                message = e.Message;
                return false;
            }
        }

    }
}

