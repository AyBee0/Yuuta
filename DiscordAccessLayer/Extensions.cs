using DSharpPlus;
using DSharpPlus.Entities;
using System.Linq;

namespace DiscordAccessLayer
{
    public static class Extensions
    {

        public static bool IsAdmin(this DiscordMember member, DiscordChannel channel)
        {
            return member.PermissionsIn(channel).HasPermission(Permissions.Administrator);
        }

    }
}
