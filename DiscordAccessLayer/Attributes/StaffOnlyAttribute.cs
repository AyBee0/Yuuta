using DSharpPlus.CommandsNext;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Xml.XPath;

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