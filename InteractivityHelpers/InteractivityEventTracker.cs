using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Generatsuru;
using Globals;
using InteractivityHelpers.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace InteractivityHelpers
{
    public enum InteractivityStatus
    {
        Cancelled = 0,
        TimedOut = 1,
        OK = 2,
        Finished = 3,
    }

    /// <summary>
    /// THERE'S A LOT OF TRASH HERE BUT THE THINGS I NEED WORK. PLEASE PR THE USELESS SHIT OUT THANKS LOVE YOU
    /// </summary>
    internal class InteractivityEventTracker
    {
        public CommandContext Ctx { get; set; }
        public InteractivityStatus Status { get; private set; }
        public string Message { get; private set; }
        public List<DiscordMessage> MessagesToDelete { get; private set; }
        public TimeSpan DefaultTimeSpan { get; set; }
        public InteractivityExtension Interactivity { get; private set; }

        public InteractivityEventTracker(CommandContext ctx, TimeSpan? defaultTimeSpan = null)
        {
            Ctx = ctx;
            Interactivity = Ctx.Client.GetInteractivity();
            if (Interactivity == null)
            {
                throw new InvalidOperationException("Discord Client Interactivity not set.");
            }
            MessagesToDelete = new List<DiscordMessage> { ctx.Message };
            Status = InteractivityStatus.OK;
            Message = "Success";
            DefaultTimeSpan = defaultTimeSpan ?? TimeSpan.FromMinutes(3);
        }


        public InteractivityEventTracker(InteractivityResult<DiscordMessage> result, CommandContext ctx)
        {
            Ctx = ctx;
            MessagesToDelete = new List<DiscordMessage>();
            if (result.Result == null)
            {
                SetTimedOut();
            }
            else if (result.Result.Content.Trim().ToLower().Equals("cancel"))
            {
                SetCancelled();
            }
            else
            {
                SetOk();
            }
        }

        public InteractivityEventTracker Update(InteractivityResult<DiscordMessage> result)
        {
            if (result.Result == null)
            {
                return SetTimedOut();
            }
            else if (result.Result.Content?.Trim().ToLower() == "cancel")
            {
                MessagesToDelete.Add(result.Result);
                return SetCancelled();
            }
            else
            {
                MessagesToDelete.Add(result.Result);
                return SetOk();
            }
        }

        public InteractivityEventTracker Ok()
        {
            return new InteractivityEventTracker(Ctx).SetTimedOut();
        }

        public InteractivityEventTracker TimedOut()
        {
            return new InteractivityEventTracker(Ctx).SetTimedOut();
        }

        public InteractivityEventTracker Cancelled()
        {
            return new InteractivityEventTracker(Ctx).SetCancelled();
        }

        public InteractivityEventTracker Finished()
        {
            return new InteractivityEventTracker(Ctx).SetFinished();
        }

        ///// <summary>
        ///// Sends a message asking a user to interact. Deprecated.
        ///// </summary>
        ///// <param name="message">Message to ask</param>
        ///// <param name="deleteMessage">Delete the message or not</param>
        ///// <returns></returns>
        //[Obsolete("Deprecated. Use AskAndWaitForResponseAsync instead.")]
        //public async Task AskInteractivityAsync(string message, bool deleteMessage = true)
        //{
        //    await Ctx.TriggerTypingAsync();
        //    var askMessage = await Ctx.RespondAsync(message);
        //    if (deleteMessage) MessagesToDelete.Add(askMessage);
        //}

        public async Task<object> DoInteractionAsync<T>(Interaction<T> interaction) where T : ResultEntity, new()
        {
            while (true)
            {
                InteractivityResult<DiscordMessage> result = await AskAndWaitForResponseAsync
                    (interaction.AskMessage, interaction.TryParseCondition, interaction.ExecuteMemberAndChannelCheck,
                    interaction.QueueForDeletion, interaction.TimeOutOverride, interaction.AppendCancelMessage,
                    interaction.AcceptNone, interaction.NoneKeyword, interaction.AppendNoneMessage,
                    interaction.NoneMessage);
                if (Status != InteractivityStatus.OK || (interaction.AcceptNone && result.Result.Content.ToTrimmedLower() == interaction.NoneKeyword))
                {
                    return null;
                }
                //TODO Check if parse failed
                bool parseSuccess = interaction.Parser.TryParse(result, out object obj, out string message);
                if (parseSuccess)
                {
                    return obj;
                }
                await Ctx.RespondAsync($":x: {message}");
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="message">Message to ask</param>
        ///// <param name="config">Configuration</param>
        ///// <returns></returns>
        //public async Task<InteractivityResult<DiscordMessage>>
        //    DoInteractionAsync(string message, InteractionConfig config, Func<DiscordMessage, bool> condition = null)
        //{
        //    return await AskAndWaitForResponseAsync(message, condition, config.ExecuteMemberAndChannelCheck,
        //        config.QueueForDeletion, config.TimeOutOverride, config.AppendCancel, config.AcceptNone, config.NoneOverride,
        //        config.AppendNoneMessage, config.NoneMessage);
        //}

        //TODO Customizable Cancel keyword
        /// <summary>
        /// Sends a message asking a user to interact, then handles the result.
        /// </summary>
        /// <param name="message">Message to ask</param>
        /// <param name="condition">Condition where interacted message will be accepted as a result. Defaults to channel and author.</param>
        /// <param name="deleteMessage">Delete the message or not.</param>
        /// <param name="timeOutOverride">Override the default timeout time set by your D#+ config</param>
        /// <param name="checkForMemberAndChannel">Add a channel and author check to the condition.</param>
        /// <param name="sendCancelNotice">Add a message telling the user how to cancel.</param>
        /// <returns>Interactivity Result of a discord message</returns>
        private async Task<InteractivityResult<DiscordMessage>> AskAndWaitForResponseAsync(string message,
            Func<DiscordMessage, bool> condition = null, bool checkForMemberAndChannel = true,
            bool deleteMessage = true, TimeSpan? timeOutOverride = null, bool sendCancelNotice = false,
            bool acceptNone = false, string noneOverride = "none", bool appendNone = true,
            string noneMessage = "")
        {
            if (timeOutOverride == null)
            {
                timeOutOverride = DefaultTimeSpan;
            }
            try
            {
                await Ctx.TriggerTypingAsync();
                message = sendCancelNotice ? "*Send \"cancel\" at anytime to cancel this opeation.*\n\n" + message : message;
                message = acceptNone && appendNone ? message + noneMessage : message;
                if (condition == null && !checkForMemberAndChannel)
                {
                    throw new InvalidOperationException("Condition cannot be null while asked not to check for the same member and channel.");
                }
                if (condition == null)
                {
                    condition = (sentMessage) => true;
                }
                // Should check that the member and channel are the same as whom started the interactivity operation.
                bool memberAndChannelCondition(DiscordMessage sentMessage) => sentMessage.ChannelId == Ctx.Channel.Id && sentMessage.Author.Id == Ctx.Message.Author.Id;
                if (checkForMemberAndChannel)
                {
                    var oldCondition = condition;
                    condition = (sentMessage) => oldCondition(sentMessage)
                        && memberAndChannelCondition(sentMessage);
                }
                if (acceptNone)
                {
                    var oldCondition = condition;
                    condition = sentMessage =>
                        oldCondition(sentMessage) || sentMessage.Content.Trim().ToLower() == noneOverride;
                }
                const string cancelKeyWord = "cancel";
                var conditionWithoutCancel = condition;
                condition = sentMessage =>
                    Configuration.Prefixes
                    .All(x => !sentMessage.Content.ToTrimmedLower().StartsWith(x.ToTrimmedLower()))
                    && (conditionWithoutCancel(sentMessage) 
                        || sentMessage.Content.Trim().ToLower() == cancelKeyWord);
                //condition =
                //    sentMessage =>
                //        checkForMemberAndChannel ? sentMessage.ChannelId == Ctx.Channel.Id && sentMessage.Author.Id == Ctx.Message.Author.Id : true
                //        && (condition(sentMessage)
                //            || (acceptNone && sentMessage.Content.Trim().ToLower() == noneOverride)
                //            || sentMessage.Content.Trim().ToLower() == "cancel");
                var askMessage = await Ctx.RespondAsync(message);
                MessagesToDelete.Add(askMessage);
                InteractivityResult<DiscordMessage> result;
                result = await Interactivity
                   .WaitForMessageAsync(condition);
                if (result.Result != null && deleteMessage)
                {
                    MessagesToDelete.Add(result.Result);
                }
                Update(result);
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task SendMessageResponse(string message, bool deleteMessage = true)
        {
            if (deleteMessage)
            {
                var messageSent = await Ctx.RespondAsync(message);
                MessagesToDelete.Add(messageSent);
            }
        }

        public async Task DeleteMessagesAsync()
        {
            await Ctx.Channel.DeleteMessagesAsync(MessagesToDelete, "Yuutabot Interactivity");
        }

        private InteractivityEventTracker SetTimedOut()
        {
            Status = InteractivityStatus.TimedOut;
            Message = ":x: Timed out. Please try again.";
            return this;
        }

        private InteractivityEventTracker SetCancelled()
        {
            Status = InteractivityStatus.Cancelled;
            Message = ":white_check_mark: Cancelled ongoing operation successfully.";
            return this;
        }

        public InteractivityEventTracker SetFinished()
        {
            Status = InteractivityStatus.Finished;
            Message = ":white_check_mark: Successfully finished operation.";
            return this;
        }

        private InteractivityEventTracker SetOk()
        {
            Status = InteractivityStatus.OK;
            Message = ":white_check_mark: Everything fine so far.";
            return this;
        }
    }

}