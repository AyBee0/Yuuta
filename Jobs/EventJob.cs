using Data.Models.Events;
using DataAccessLayer.Models;
using DataAccessLayer.Models.Events;
using DSharpPlus;
using DSharpPlus.Entities;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobs
{

    public class EventJob : IJob
    {

        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            DiscordClient client = (DiscordClient)dataMap["discordClient"];
            IEvent guildEvent = (IEvent)dataMap["guildEvent"];
            DiscordGuild guild = await client.GetGuildAsync(guildEvent.GuildId);
            if (guildEvent is DirectMessageEvent dmEvent)
            {
                await CommitDirectMessageEventInstance(guild, dmEvent);
            }
            else if (guildEvent is RoleEvent rEvent)
            {
                await CommitRoleEventInstance(guild, rEvent);
            }
            else if (guildEvent is GuildMessageEvent gmEvent)
            {
                await CommitGuildMessageEventInstance(guild, gmEvent);
            }
            else if (guildEvent is ReactionLinkedEvent rlEvent)
            {
                await CommitReactionLinkedEventInstance(client, guild, rlEvent);
            }
        }

        private static async Task CommitReactionLinkedEventInstance(DiscordClient client, DiscordGuild guild, ReactionLinkedEvent rlEvent)
        {
            var channel = guild.GetChannel(rlEvent.ChannelId);
            var message = await channel.GetMessageAsync(rlEvent.MessageId);
            var reactions = await message.GetReactionsAsync(DiscordEmoji.FromName(client, rlEvent.EmojiName));
            using var db = new YuutaDbContext();
            foreach (DiscordUser reaction in reactions)
            {
                DiscordMember member = await guild.GetMemberAsync(reaction.Id);
                if (member == null)
                {
                    continue;
                }
                switch (rlEvent.EventType)
                {
                    case EventType.DirectMessageEvent:
                        DirectMessageEvent dmEvent = await db.DirectMessageEvents.FindAsync(rlEvent.EventId);
                        await CommitDirectMessageEventInstance(guild, dmEvent);
                        break;
                    case EventType.RoleEvent:
                        RoleEvent rEvent = await db.RoleEvents.FindAsync(rlEvent.EventId);
                        await CommitRoleEventInstance(guild, rEvent);
                        break;
                    case EventType.GuildMessageEvent:
                        GuildMessageEvent gmEvent = await db.GuildMessageEvents.FindAsync(rlEvent.EventId);
                        await CommitGuildMessageEventInstance(guild, gmEvent);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private static async Task CommitGuildMessageEventInstance(DiscordGuild guild, GuildMessageEvent gmEvent)
        {
            var channel = guild.GetChannel(gmEvent.ChannelToSend);
            await channel.SendMessageAsync(gmEvent.Text);
        }

        private static async Task CommitRoleEventInstance(DiscordGuild guild, RoleEvent rEvent)
        {
            var member = await guild.GetMemberAsync(rEvent.User);
            var role = guild.GetRole(rEvent.RoleId);
            switch (rEvent.RoleEventType)
            {
                case RoleEventType.Deny:
                    await member.RevokeRoleAsync(role);
                    break;
                case RoleEventType.Grant:
                    await member.GrantRoleAsync(role);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static async Task CommitDirectMessageEventInstance(DiscordGuild guild, DirectMessageEvent dmEvent)
        {
            foreach (EventUser eventUser in dmEvent.UserToSend)
            {
                DiscordMember user = await guild.GetMemberAsync(eventUser.EventUserId);
                await user.SendMessageAsync(dmEvent.Text);
            }
        }
    }

}
