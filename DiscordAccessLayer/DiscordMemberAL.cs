using DataAccessLayer.Models.RoleModels;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;

namespace DiscordAccessLayer
{
    public class DiscordMemberAL
    {
        public DiscordMember Member { get; private set; }
        public DiscordGuildAL GuildAL { get; private set; }
        public bool StaffMember { get; private set; }
        public bool GlobalCommandPerms { get; private set; }

        public DiscordMemberAL(DiscordMember member)
        {
            this.Member = member;
            this.GuildAL = new DiscordGuildAL(member);
            this.StaffMember = IsStaffMember();
            this.GlobalCommandPerms = HasGlobalCommandPerms();
        }

        public DiscordMemberAL(CommandContext ctx) : this(ctx.Member) 
        { }

        private bool IsStaffMember()
        {
            return Member.Roles
                .Select(role => role.Id)
                .Cast<long>()
                .Intersect(GuildAL.StaffRoles.Select(staffRole => staffRole.RoleDid))
                .Any();
        }

        private bool HasGlobalCommandPerms()
        {
            return Member.Roles
                .Select(role => role.Id)
                .Cast<long>()
                .Intersect(GuildAL.GlobalCommandRoles.Select(globalCommandRole => globalCommandRole.RoleDid))
                .Any();
        }

    }
}