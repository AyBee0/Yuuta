using DSharpPlus.CommandsNext;
using System.Threading.Tasks;

namespace DiscordMan.Attributes
{
    public sealed class StaffOnlyAttribute : RestrictedAttribute
    {
        public StaffOnlyAttribute() { }
        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return Task.Run(() =>
            {
                var man = new DiscordMemberMan(ctx.Member);
                return AttributeTools.CanSendInChannel(ctx) && (man.StaffMember || man.IsAdmin(ctx.Channel)); 
            });
        }
    }
}