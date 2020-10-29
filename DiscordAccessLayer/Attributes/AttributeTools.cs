using DiscordMan;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMan.Attributes
{
    public static class AttributeTools
    {
        public static Dictionary<string, DiscordCommandMan> CommandALs = new Dictionary<string, DiscordCommandMan>();

        public static bool CanSendInChannel(CommandContext ctx)
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
                .ExplicitlyAuthorizedRoles.Where(x => x.Key == ctx.Guild.Id);
            var explicitlyDeniedRoles = cmal.
                ExplicitlyDeniedRoles.Where(x => x.Key == ctx.Guild.Id);
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
            var chal = new DiscordChannelMan(ctx);
            // If it's a bot channel, automatic true.
            if (chal.BotChannel)
            {
                return true;
            }
            else
            {
                // Not a bot channel, check if he has global perms or is a staff member.
                var mal = new DiscordMemberMan(ctx);
                return mal.GlobalCommandPerms || mal.StaffMember;
            }
        }

    }
}
