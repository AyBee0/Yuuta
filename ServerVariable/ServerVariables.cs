using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using ServerVars;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerVariable {
    public class ServerVariables {

        readonly CommandContext CTX;

        public const ulong TheBeaconId = 310279910264406017;
        public const ulong NoobsId = 450117929384542249;
        public const ulong TestId = 453487691216977922;

        public const ulong TheBeaconGameRoleReactMessageId = 620260875789664260;
        public const ulong TheBeaconOtherRoleReactMessageId = 620260879526789131;
        public const ulong TheBeaconPlatformMessageId = 620260877211271178;
        public const ulong TheBeaconRoleChannelId = 602599352556322828;

        //TEMP
        public const ulong TheBeaconTempReactMessageId = 607964037405343756;

        public static readonly List<ulong> TheBeaconMacroRoles = new List<ulong> {
            ActiveMemberRole,
            VetMemberRole,
            DonatorRole
        };

        public static readonly List<ulong> FilteredGuilds = new List<ulong> {
                        //310279910264406017,
                        453487691216977922
        };

        public static readonly List<ulong> TheBeaconBotChannels = new List<ulong> {
            397340837501075457
        };

        public const ulong ActiveMemberRole = 359708482217181185;
        public const ulong VetMemberRole = 346717093128830978;
        public const ulong DonatorRole = 351411448402149386;


        public ServerVariables(CommandContext _ctx) {
            CTX = _ctx;
            //ServerStaffRoles = new Dictionary<long, List<DiscordRole>>();
            //List<DiscordRole> beaconStaffIds = new List<DiscordRole>();
        }

        public bool IsStaffMember() {
            switch (CTX.Guild.Id) {
                case TheBeaconId: //The Beacon
                    foreach (var id in new ServerRoleIds().TheBeaconStaffIds) {
                        var role = CTX.Guild.GetRole(id);
                        if (CTX.Member.Roles.Contains(role)) {
                            return true;
                        }
                    }
                    return false;
                case NoobsId:
                    foreach (var id in new ServerRoleIds().NoobsStaffIds) {
                        var role = CTX.Guild.GetRole(id);
                        if (CTX.Member.Roles.Contains(role)) {
                            return true;
                        }
                    }
                    return false;
                case TestId:
                    foreach (var id in new ServerRoleIds().TestStaffIds) {
                        var role = CTX.Guild.GetRole(id);
                        if (CTX.Member.Roles.Contains(role)) {
                            return true;
                        }
                    }
                    return false;
                default:
                    return false;
            }
        }

        public static bool IsStaffMember(DiscordMember member) {
            switch (member.Guild.Id) {
                case TheBeaconId: //The Beacon
                    foreach (var id in new ServerRoleIds().TheBeaconStaffIds) {
                        var role = member.Guild.GetRole(id);
                        if (member.Roles.Contains(role)) {
                            return true;
                        }
                    }
                    return false;
                case NoobsId:
                    foreach (var id in new ServerRoleIds().NoobsStaffIds) {
                        var role = member.Guild.GetRole(id);
                        if (member.Roles.Contains(role)) {
                            return true;
                        }
                    }
                    return false;
                case TestId:
                    foreach (var id in new ServerRoleIds().TestStaffIds) {
                        var role = member.Guild.GetRole(id);
                        if (member.Roles.Contains(role)) {
                            return true;
                        }
                    }
                    return false;
                default:
                    return false;
            }
        }

        public List<DiscordRole> GetServerAdminRoles() {
            switch (CTX.Guild.Id) {
                case TheBeaconId:
                    return new List<DiscordRole> {
                        CTX.Guild.GetRole(445138815087017997)
                    };
                case NoobsId:
                    return new List<DiscordRole> {
                        CTX.Guild.GetRole(530092112079749120)
                    };
                case TestId:
                    return new List<DiscordRole> {
                        CTX.Guild.GetRole(602229969351082036)
                    };
                default:
                    return null;
            }
        }

        public DiscordRole GetVacationRole() {
            switch (CTX.Guild.Id) {
                case TheBeaconId:
                    return CTX.Guild.GetRole(369952220369649664);
                case TestId:
                    return CTX.Guild.GetRole(602232450835415041);
                default:
                    return null;
            }
        }

        public bool CanSendInChannel() {
            if (CTX.Guild.Id == TheBeaconId) {
                if (TheBeaconBotChannels.Contains(CTX.Channel.Id)) {
                    return true;
                } else if (IsStaffMember()) {
                    return true;
                } else {
                    var userRoles = CTX.Member.Roles;
                    foreach (var roleId in TheBeaconMacroRoles) {
                        var role = CTX.Guild.GetRole(roleId);
                        if (userRoles.Contains(role)) {
                            return true;
                        }
                    }
                    return false;
                }
            } else {
                return true;
            }
        }

        public static bool CanSendInChannel(DiscordMember member, ulong channelId) {
            if (member.Guild.Id == TheBeaconId) {
                if (TheBeaconBotChannels.Contains(channelId)) {
                    return true;
                } else if (IsStaffMember(member)) {
                    return true;
                } else {
                    var userRoles = member.Roles;
                    foreach (var roleId in TheBeaconMacroRoles) {
                        var role = member.Guild.GetRole(roleId);
                        if (userRoles.Contains(role)) {
                            return true;
                        }
                    }
                    return false;
                }
            } else {
                return true;
            }
        }

        public DiscordChannel GetDetentionChannel() {
            switch (CTX.Guild.Id) {
                case TheBeaconId:
                    return CTX.Guild.GetChannel(597481358784200722);
                default:
                    return null;
            }
        }

    }
}