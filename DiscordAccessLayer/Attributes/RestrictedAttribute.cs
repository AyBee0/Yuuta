using DataAccessLayer.DataAccess;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordAccessLayer.Attributes
{
    public class RestrictedAttribute : CheckBaseAttribute
    {

        public static Dictionary<string, DiscordCommandAL> CommandALs = new Dictionary<string, DiscordCommandAL>();

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return Task.FromResult(CanSendInChannel(ctx));
        }

        private bool CanSendInChannel(CommandContext ctx)
        {
            if (ctx.Member.IsAdmin(ctx.Channel))
            {
                return true;
            }
            var cmal = CommandALs[ctx.Command.Name];
            //var cmal = CommandALs.GetValueOrDefault(ctx.Command.Name) ?? new DiscordCommandAL(ctx);
            var roles = ctx.Member.Roles.Zip(ctx.Member.Roles.Select(x => x.Id));
            int highestAuth = -1;
            int highestDeny = -1;
            var explicitlyAuthorizedRoles = cmal
                .ExplicitlyAuthorizedRoles.Where(x => x.Key == (long)ctx.Guild.Id);
            var explicitlyDeniedRoles = cmal.
                ExplicitlyDeniedRoles.Where(x => x.Key == (long)ctx.Guild.Id);
            foreach (var authorizedOverload in explicitlyAuthorizedRoles)
            {
                (DiscordRole First, ulong Second) overload = roles.SingleOrDefault(x => x.Second == (ulong)authorizedOverload.Value);
                if (overload == default) { continue; }
                int position = overload.First.Position;
                if (highestAuth < position)
                {
                    highestAuth = overload.First.Position;
                }
            }
            foreach (var unauthorizedOverload in explicitlyDeniedRoles)
            {
                (DiscordRole First, ulong Second) overload = roles.SingleOrDefault(x => x.Second == (ulong)unauthorizedOverload.Value);
                if (overload == default) { continue; }
                int position = overload.First.Position;
                if (highestDeny < position)
                {
                    highestDeny = overload.First.Position;
                }
            }
            if (highestAuth > highestDeny)
            {
                // Highest auth is greater than the highest deny
                return true;
            }
            else if (highestAuth < highestDeny)
            {
                // Highest deny is greater than the highest auth 
                return false;
            }
            // At this point, no overloads have been set.
            var chal = new DiscordChannelAL(ctx);
            // If it's a bot channel, automatic true.
            if (chal.BotChannel)
            {
                return true;
            }
            else
            {
                // Not a bot channel, check if he has global perms or is a staff member.
                var mal = new DiscordMemberAL(ctx);
                return mal.GlobalCommandPerms || mal.StaffMember;
            }
        }

    }
}
