using DiscordAccessLayer;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;

namespace Commands.Attributes
{
    public sealed class StaffOnlyAttribute : CheckBaseAttribute
    {
        public StaffOnlyAttribute() { }
        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return Task.FromResult(new DiscordMemberAL(ctx.Member).StaffMember);
        }
    }
}