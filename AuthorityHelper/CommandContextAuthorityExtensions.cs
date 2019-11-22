using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Linq;

namespace AuthorityHelpers {
    public static class AuthorityExtensions {

        public static bool IsStaffMember(this CommandContext ctx) {
            return new AuthorityHelper(ctx).IsStaffMember;
        }

        public static bool CanSendInChannel(this CommandContext ctx) {
            return new AuthorityHelper(ctx).CanSendInChannel;
        }

        public static bool CanTakeAction(this CommandContext ctx, DiscordMember actionAgainst) {
            return ctx.Member.IsRankedHigher(actionAgainst);
        }

        public static int CompareTo(this DiscordRole role, DiscordRole role2) {
            return role.Position.CompareTo(role2.Position);
        }

        public static int CompareHierarchy(this DiscordMember member, DiscordMember member2) {
            return member.Roles.Max(x => x.Position).CompareTo(member2.Roles.Max(x => x.Position));
        }

        public static bool IsRankedHigher(this DiscordMember member, DiscordMember member2) {
            return member.CompareHierarchy(member2) > 0;
        }

    }
}
