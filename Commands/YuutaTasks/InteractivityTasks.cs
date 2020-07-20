using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using InteractivityHelpers;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InteractivityHelpers.InteractivityEventTracker;

namespace Commands.YuutaTasks
{
    public partial class InteractivityMan
    {
        /// <summary>
        /// Ask for tagged channels
        /// </summary>
        /// <param name="askMessage">The message to be asked</param>
        /// <param name="config">Configuration for this interaction</param>
        /// <returns></returns>
        private async Task<List<DiscordChannel>> AskForChannelsAsync
            (InteractionConfig config,
            string askMessage)
        {
            var result =
                await tracker.DoInteractionAsync(askMessage,
                config, Conditions.ChannelCondition);
            if (tracker.Status != InteractivityStatus.OK)
            {
                return null;
            }
            return result.Result.MentionedChannels.ToList();
        }

        private async Task<string> AskForTextAsync(InteractionConfig config, string askMessage)
        {
            var result =
                await tracker.DoInteractionAsync(askMessage, config);
            if (tracker.Status != InteractivityStatus.OK)
            {
                return null;
            }
            return result.Result.Content;
        }

        /// <summary>
        /// Ask for roles seperated by a comma (,). You can either tag (@) or just send the role names. 
        /// </summary>
        /// <param name="askMessage">The message to be asked</param>
        /// <param name="config">Configuration for this interaction</param>
        /// <returns></returns>
        private async Task<List<DiscordRole>> AskForRolesAsync(InteractionConfig config, string askMessage)
        {
            var result =
                await tracker.DoInteractionAsync(askMessage,
                config, Conditions.RoleCondition);
            return result.ParseSentRoles();
        }

        private async Task<DateTime> AskForDateTimeAsync(InteractionConfig config, string askMessage)
        {
            var result =
                await tracker.DoInteractionAsync(askMessage, config, Conditions.DateCondition);
            if (tracker.Status != InteractivityStatus.OK)
            {
                return default;
            }
            return DateTime.Parse(result.Result.Content);
        }

        private async Task<List<DiscordAttachment>> AskForAttachmentsAsync(InteractionConfig config, string askMessage)
        {
            var result =
                await tracker.DoInteractionAsync(askMessage, config, Conditions.AttachmentCondition);
            if (tracker.Status != InteractivityStatus.OK)
            {
                return null;
            }
            return result.Result.Attachments.ToList();
        }

        private async Task<string> AskForLinkAsync(InteractionConfig config, string askMessage)
        {
            var result =
                await tracker.DoInteractionAsync(askMessage, config, Conditions.LinkCondition);
            if (tracker.Status != InteractivityStatus.OK)
            {
                return null;
            }
            return result.Result.Content;
        }

        private async Task<int> AskForIntAsync(InteractionConfig config, string askMessage)
        {
            var result =
                await tracker.DoInteractionAsync(askMessage, config, Conditions.IntegerCondition);
            if (tracker.Status != InteractivityStatus.OK)
            {
                return 0;
            }
            return int.Parse(result.Result.Content);
        }

    }
}