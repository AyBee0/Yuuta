using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Commands {
    public class InteractivityEventTracker {

        public enum InteractivityStatus {
            Cancelled = 0,
            TimedOut = 1,
            OK = 2,
            Finished = 3,
        }

        public CommandContext Ctx { get; set; }
        public InteractivityStatus Status { get; private set; }
        public string Message { get; private set; }
        public List<DiscordMessage> MessagesToDelete { get; private set; }
        public InteractivityExtension Interactivity
        {
            get {
                return Ctx.Client.GetInteractivity();
            }
        }

        public InteractivityEventTracker(CommandContext ctx) {
            Ctx = ctx;
            MessagesToDelete = new List<DiscordMessage> { ctx.Message };
            Status = InteractivityStatus.OK;
            Message = "Success";
        }


        public InteractivityEventTracker(InteractivityResult<DiscordMessage> result, CommandContext ctx) {
            Ctx = ctx;
            MessagesToDelete = new List<DiscordMessage>();
            if (result.Result == null) {
                SetTimedOut();
            } else if (result.Result.Content.Trim().ToLower().Equals("cancel")) {
                SetCancelled();
            } else {
                SetOk();
            }
        }

        public InteractivityEventTracker Update(InteractivityResult<DiscordMessage> result) {
            if (result.Result == null) {
                return SetTimedOut();
            } else if (result.Result.Content.Trim().ToLower().Equals("cancel")) {
                MessagesToDelete.Add(result.Result);
                return SetCancelled();
            } else {
                MessagesToDelete.Add(result.Result);
                return SetOk();
            }
        }

        public InteractivityEventTracker Ok() {
            return new InteractivityEventTracker(Ctx).SetTimedOut();
        }

        public InteractivityEventTracker TimedOut() {
            return new InteractivityEventTracker(Ctx).SetTimedOut();
        }

        public InteractivityEventTracker Cancelled() {
            return new InteractivityEventTracker(Ctx).SetCancelled();
        }

        public InteractivityEventTracker Finished() {
            return new InteractivityEventTracker(Ctx).SetFinished();
        }

        /// <summary>
        /// Sends a message asking a user to interact. Deprecated.
        /// </summary>
        /// <param name="message">Message to ask</param>
        /// <param name="deleteMessage">Delete the message or not</param>
        /// <returns></returns>
        [Obsolete("Obsolete overload.")]
        public async Task AskInteractivityAsync(string message, bool deleteMessage = true) {
            await Ctx.TriggerTypingAsync();
            var askMessage = await Ctx.RespondAsync(message);
            if (deleteMessage) MessagesToDelete.Add(askMessage);
        }

        /// <summary>
        /// Sends a message asking a user to interact, then handles the result.
        /// </summary>
        /// <param name="message">Message to ask</param>
        /// <param name="condition">Condition where interacted message will be accepted as a result.</param>
        /// <param name="deleteMessage">Delete the message or not.</param>
        /// <param name="timeOutOverride">Override the default timeout time set by your D#+ config</param>
        /// <returns>Interactivity Result of a discord message</returns>
        public async Task<InteractivityResult<DiscordMessage>> AskInteractivityAsync(string message, Func<DiscordMessage, bool> condition, bool checkForMemberAndChannel = true, bool deleteMessage = true, TimeSpan? timeOutOverride = null) {
            await Ctx.TriggerTypingAsync();
            var askMessage = await Ctx.RespondAsync(message);
            MessagesToDelete.Add(askMessage);
            var result = await Interactivity.WaitForMessageAsync(condition, timeOutOverride);
            if (result.Result != null) {
                MessagesToDelete.Add(result.Result);
            }
            Update(result);
            return result;
        }

        public async Task DeleteMessagesAsync() {
            await Ctx.Channel.DeleteMessagesAsync(MessagesToDelete, "Yuutabot Interactivity");
        }

        private InteractivityEventTracker SetTimedOut() {
            Status = InteractivityStatus.TimedOut;
            Message = ":x: Timed out. Please try again.";
            return this;
        }

        private InteractivityEventTracker SetCancelled() {
            Status = InteractivityStatus.Cancelled;
            Message = ":white_check_mark: Cancelled ongoing operation successfully.";
            return this;
        }

        public InteractivityEventTracker SetFinished() {
            Status = InteractivityStatus.Finished;
            Message = ":white_check_mark: Successfully finished operation.";
            return this;
        }

        private InteractivityEventTracker SetOk() {
            Status = InteractivityStatus.OK;
            Message = ":white_check_mark: Everything fine so far.";
            return this;
        }

        /// <summary>
        /// Check whether a discord message has a same channel and user as the command context executing person and place.
        /// </summary>
        /// <param name="discordMessage">The discord message</param>
        /// <returns></returns>
        public bool SameChannelAndUser(DiscordMessage discordMessage) {
            return discordMessage.ChannelId == Ctx.Channel.Id && discordMessage.Author.Id == Ctx.Message.Author.Id;
        }

    }

}