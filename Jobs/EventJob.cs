using DSharpPlus;
using Firebase.Database;
using Newtonsoft.Json;
using Quartz;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Types;
using System.Linq;
using System.Collections.Generic;
using DSharpPlus.Entities;

namespace Jobs {

    public class EventJob : IJob {

        public async Task Execute(IJobExecutionContext context) {
            try {
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                DiscordClient client = dataMap["discordClient"] as DiscordClient;
                var guildEvent = dataMap["guildEvent"] as GuildEvent;
                var guild = await client.GetGuildAsync(ulong.Parse(guildEvent.GuildID));
                if (guildEvent.EventMessages != null) {
                    var guildEventMessages = guildEvent.EventMessages.ToList();
                    foreach (var guildEventMessageItem in guildEventMessages) {
                        var guildEventMessage = guildEventMessageItem.Value;
                        guildEvent.UserIds = guildEvent.UserIds ?? (guildEvent.UserIds = new Dictionary<string, UserID>());
                        var channel = await client.GetChannelAsync(guildEventMessage.ChannelId);
                        var message = await channel.GetMessageAsync(guildEventMessage.MessageId);
                        //message.Reactions.ToList().ForEach(reactionEmoji => {
                        //    if ((reactionEmoji.Emoji.Id != 0 && reactionEmoji.Emoji.Id == guildEventMessage.EmojiID) || (reactionEmoji.Emoji.Name == guildEventMessage.EmojiName)) {
                        //        await message.GetReactionsAsync
                        //    }
                        //});
                        IReadOnlyList<DiscordUser> reactors;
                        try {
                            reactors = await message.GetReactionsAsync(DiscordEmoji.FromName(client, guildEventMessage.EmojiName));
                        } catch (ArgumentOutOfRangeException) {
                            try {
                                reactors = await message.GetReactionsAsync(DiscordEmoji.FromUnicode(client, guildEventMessage.EmojiName));
                            } catch (Exception) {
                                throw;
                            }
                        }
                        reactors.ToList().ForEach(x => {
                            guildEvent.UserIds.Add(x.Id.ToString(), new UserID { Send = true });
                        });
                        await message.DeleteAsync();
                    }
                }
                switch (guildEvent.EventType) {
                    case EventType.DiscordEventType.DM:
                        var userIDs = guildEvent.UserIds;
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
                                    foreach (var user in guildEvent.UserIds) {
                                        var userID = ulong.Parse(user.Key);
                                        var member = await guild.GetMemberAsync(userID);
                                        await member.GrantRoleAsync(role, $"Event scheduled by YuutaBot");
                                    }
                                    break;
                                case RoleActionTypes.RoleActionType.Revoke:
                                    foreach (var user in guildEvent.UserIds) {
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
            string child = $"Root/Guilds/{guildID}/GuildEvents/{guildEventID}";
            await firebaseClient.Child(child).DeleteAsync();
        }

    }

}
