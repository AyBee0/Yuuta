using DataAccessLayer.Models.RoleModels;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace DiscordAccessLayer
{
    public class DiscordMemberAL
    {
        public DiscordMember Member { get; private set; }
        public DiscordGuildAL DGuildAL { get; private set; }
        public bool StaffMember { get; private set; }
        public bool GlobalCommandPerms { get; private set; }

        public DiscordMemberAL(DiscordMember member)
        {
            this.Member = member;
            this.DGuildAL = new DiscordGuildAL(member);
            this.StaffMember = IsStaffMember();
            this.GlobalCommandPerms = HasGlobalCommandPerms();
        }

        private bool IsStaffMember()
        {
            return Member.Roles
                .Select(role => role.Id)
                .Cast<long>()
                .Intersect(DGuildAL.StaffRoles.Select(staffRole => staffRole.RoleDid))
                .Any();
        }

        private bool HasGlobalCommandPerms()
        {
            return Member.Roles
                .Select(role => role.Id)
                .Cast<long>()
                .Intersect(DGuildAL.GlobalCommandRoles.Select(globalCommandRole => globalCommandRole.RoleDid))
                .Any();
        }

        public bool CanSendInChannel(DiscordChannel channel)
        {
            return GlobalCommandPerms || new DiscordChannelAL(channel).BotChannel || StaffMember;
        }

    }
}