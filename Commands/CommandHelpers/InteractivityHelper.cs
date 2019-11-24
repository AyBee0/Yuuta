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

        public async Task AskInteractivityAsync(string message, bool deleteMessage = true) {
            await Ctx.TriggerTypingAsync();
            var askMessage = await Ctx.RespondAsync(message);
            if (deleteMessage) MessagesToDelete.Add(askMessage);
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

    }

}