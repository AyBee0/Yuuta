using DSharpPlus;
using Firebase.Database;
using Newtonsoft.Json;
using Quartz;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Types;

namespace Jobs {

    public class EventJob : IJob {

        public async Task Execute(IJobExecutionContext context) {
            try {
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                DiscordClient client = dataMap["discordClient"] as DiscordClient;
                var guildEvent = dataMap["guildEvent"] as GuildEvent;
                var guild = await client.GetGuildAsync(ulong.Parse(guildEvent.GuildID));
                switch (guildEvent.EventType) {
                    case EventType.DiscordEventType.DM:
                        var userIDs = guildEvent.UserIDs;
                        foreach (var keyValuePair in userIDs) {
                            var userID = ulong.Parse(keyValuePair.Key);
                            var send = keyValuePair.Value.Send;
                            if (send) {
                                var member = await guild.GetMemberAsync(userID);
                                await member.SendMessageAsync(guildEvent.MessageText);
                            }
                        }
                        break;
                    case EventType.DiscordEventType.Message:
                        var channelID = ulong.Parse(guildEvent.MessageChannel);
                        var channel = guild.GetChannel(channelID);
                        await channel.SendMessageAsync(guildEvent.MessageText);
                        break;
                    case EventType.DiscordEventType.GiveRole:
                        var roles = guildEvent.Roles;
                        foreach (var roleEvent in roles) {
                            var roleID = ulong.Parse(roleEvent.Key);
                            var role = guild.GetRole(roleID);
                            var roleActionType = roleEvent.Value.RoleActionType;
                            switch (roleActionType) {
                                case RoleActionTypes.RoleActionType.Give:
                                    foreach (var user in guildEvent.UserIDs) {
                                        var userID = ulong.Parse(user.Key);
                                        var member = await guild.GetMemberAsync(userID);
                                        await member.GrantRoleAsync(role, $"Event scheduled by YuutaBot");
                                    }
                                    break;
                                case RoleActionTypes.RoleActionType.Revoke:
                                    foreach (var user in guildEvent.UserIDs) {
                                        var userID = ulong.Parse(user.Key);
                                        var member = await guild.GetMemberAsync(userID);
                                        await member.RevokeRoleAsync(role, $"Event scheduled by YuutaBot");
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
                await DeleteDiscordEvent(guildEvent.GuildID, context.JobDetail.Key.Name);
            } catch (Exception e) {
                await Console.Out.WriteLineAsync(e.Message);
                throw;
            }
        }

        public static async Task DeleteDiscordEvent(string guildID, string guildEventID) {
            var firebaseClient = new FirebaseClient("https://the-beacon-team-battles.firebaseio.com/");
            await firebaseClient.Child($"Root/Guilds/{guildID}/GuildEvents/{guildEventID}").DeleteAsync();
        }

    }

}
