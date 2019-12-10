using DSharpPlus;
using System.Collections.Generic;
using System.Linq;

namespace DiscordExtensions
{
    public static class DiscordExtensions
    {

        public static bool HasPermissions(this Permissions givenPermissions, out List<Permissions> missingPermissions, params Permissions[] permissions)
        {
            missingPermissions = permissions.Where(permission => !givenPermissions.HasPermission(permission)).ToList();
            return permissions.All(permission => givenPermissions.HasPermission(permission));
        }

    }
}
