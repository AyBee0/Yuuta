using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using InteractivityHelpers.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
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

    internal class InteractivityEventTracker
    {
        public CommandContext Ctx { get; set; }
        public InteractivityStatus Status { get; private set; }
        public string Message { get; private set; }
        public List<DiscordMessage> MessagesToDelete { get; private set; }
        public TimeSpan DefaultTimeSpan { get; set; }
        public InteractivityExtension Interactivity
        {
            get {
                return Ctx.Client.GetInteractivity();
            }
        }

        public InteractivityEventTracker(CommandContext ctx, TimeSpan? defaultTimeSpan = null)
        {
            Ctx = ctx;
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
            else if (result.Result.Content.Trim().ToLower().Equals("cancel"))
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
            var result = await AskAndWaitForResponseAsync
                (interaction.AskMessage, interaction.Condition, interaction.ExecuteMemberAndChannelCheck,
                interaction.QueueForDeletion, interaction.TimeOutOverride, interaction.AppendCancelMessage,
                interaction.AcceptNone, interaction.NoneKeyword, interaction.AppendNoneMessage,
                interaction.NoneMessage);
            if (Status != InteractivityStatus.OK)
            {
                return null;
            }
            var parser = interaction.Parser;
            return interaction.Parser.Exec.Invoke(result.Result);
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
                condition =
                    sentMessage =>
                        checkForMemberAndChannel ? sentMessage.ChannelId == Ctx.Channel.Id && sentMessage.Author.Id == Ctx.Message.Author.Id : true
                        && (condition(sentMessage)
                            || (acceptNone && sentMessage.Content.Trim().ToLower() == noneOverride)
                            || sentMessage.Content.Trim().ToLower() == "cancel");
                var askMessage = await Ctx.RespondAsync(message);
                MessagesToDelete.Add(askMessage);
                var result = await Interactivity.WaitForMessageAsync(condition, timeOutOverride);
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