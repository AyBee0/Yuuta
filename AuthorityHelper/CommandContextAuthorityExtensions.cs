using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthorityHelpers {
    public static class CommandContextAuthorityExtensions {

        public static bool IsStaffMember(this CommandContext ctx) {
            return new AuthorityHelper(ctx).IsStaffMember;
        }

        public static bool CanSendInChannel(this CommandContext ctx) {
            return new AuthorityHelper(ctx).CanSendInChannel;
        }

    }
}
