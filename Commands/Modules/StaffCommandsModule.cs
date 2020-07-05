using DiscordMan.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace Commands.Modules
{
    public class StaffCommandsModule : YuutaCommandModule
    {
        [StaffOnly]
        [Command]
        [Description("Create a new reaction message.")]
        public async Task NewReactionMessage(CommandContext ctx)
        {

        }
    }
}
