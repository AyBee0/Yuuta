using DSharpPlus.CommandsNext;
using InteractivityHelpers.Entities;
using System.Threading.Tasks;

namespace Commands.YuutaTasks
{
    public static class Tasks
    {
        public static async Task<bool> NewReactionRoleMessageAsync(CommandContext ctx)
        {
            var operation = new InteractivityOperation
            {
                new Interaction("**What's the title of the event**?", Parsers.StringParser){
                    
                    },
            };
        }
    }
}