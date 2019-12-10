using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace InteractivityHelpers
{
    public class InteractivityEventTracker
    {

        public enum InteractivityStatus
        {
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
        public Conditions InteractivityConditions
        {
            get {
                return new Conditions(Ctx);
            }
        }

        public InteractivityEventTracker(CommandContext ctx)
        {
            Ctx = ctx;
            MessagesToDelete = new List<DiscordMessage> { ctx.Message };
            Status = InteractivityStatus.OK;
            Message = "Success";
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

        /// <summary>
        /// Sends a message asking a user to interact. Deprecated.
        /// </summary>
        /// <param name="message">Message to ask</param>
        /// <param name="deleteMessage">Delete the message or not</param>
        /// <returns></returns>
        [Obsolete("Deprecated. Use AskAndWaitForResponseAsync instead.")]
        public async Task AskInteractivityAsync(string message, bool deleteMessage = true)
        {
            await Ctx.TriggerTypingAsync();
            var askMessage = await Ctx.RespondAsync(message);
            if (deleteMessage) MessagesToDelete.Add(askMessage);
        }

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
        public async Task<InteractivityResult<DiscordMessage>> AskAndWaitForResponseAsync(string message, Func<DiscordMessage, bool> condition = null, bool checkForMemberAndChannel = true, bool deleteMessage = true, TimeSpan? timeOutOverride = null, bool sendCancelNotice = false, bool acceptNone = false, string noneOverride = "none")
        {
            try
            {
                await Ctx.TriggerTypingAsync();
                message = sendCancelNotice ? "*Send \"cancel\" at anytime to cancel this opeation.*\n\n" + message : message;
                if (condition == null && !checkForMemberAndChannel)
                {
                    throw new InvalidOperationException("Condition cannot be null while asked not to check for the same member and channel.");
                }
                //condition = (condition == null || checkForMemberAndChannel) ? sentMessage =>
                //    (sentMessage.ChannelId == Ctx.Channel.Id && sentMessage.Author.Id == Ctx.Message.Author.Id)
                //    && ((condition != null ? condition(sentMessage) || (acceptNone ? sentMessage.Content.Trim().ToLower() == "none" : false) : true)) : condition;
                //if (checkForMemberAndChannel)
                //{
                //    condition =
                //        sentMessage => condition(sentMessage) && sentMessage.ChannelId == Ctx.Channel.Id && sentMessage.Author.Id == Ctx.Message.Author.Id;
                //    //|| (acceptNone ? sentMessage.Content.Trim().ToLower() == "none" : false)
                //    ////&& sentMessage.ChannelId == Ctx.Channel.Id
                //    //&& sentMessage.Author.Id == Ctx.Message.Author.Id;
                //}
                condition =
                    sentMessage =>
                        checkForMemberAndChannel ? sentMessage.ChannelId == Ctx.Channel.Id && sentMessage.Author.Id == Ctx.Message.Author.Id : true
                        && (condition(sentMessage)
                            || (acceptNone ? sentMessage.Content.Trim().ToLower() == noneOverride : false)
                            || sentMessage.Content.Trim().ToLower() == "cancel");
                var askMessage = await Ctx.RespondAsync(message);
                MessagesToDelete.Add(askMessage);
                var result = await Interactivity.WaitForMessageAsync(condition, TimeSpan.FromMinutes(1));
                if (result.Result != null)
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


        /// <summary>
        /// Check whether a discord message has a same channel and user as the command context executing person and place.
        /// </summary>
        /// <param name="discordMessage">The discord message</param>
        /// <returns></returns>
        public bool SameChannelAndUser(DiscordMessage discordMessage)
        {
            return discordMessage.ChannelId == Ctx.Channel.Id && discordMessage.Author.Id == Ctx.Message.Author.Id;
        }

        public class Conditions
        {
            public CommandContext Ctx { get; set; }

            public Conditions(CommandContext ctx)
            {
                Ctx = ctx;
            }

            public Func<DiscordMessage, bool> DateCondition
            {
                get {
                    return x =>
                    DateTime.TryParse(x.Content, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);
                }
            }
            public Func<DiscordMessage, bool> ChannelCondition
            {
                get {
                    return x =>
                    x.MentionedChannels?.Count > 0;
                }
            }

            public Func<DiscordMessage, bool> ImageCondition
            {
                get {
                    return x => x.Attachments?.Count == 1;
                }
            }

            public Func<DiscordMessage, bool> ValidRoleCondition
            {
                get {
                    var guildRoles = Ctx.Guild.Roles.Values.Select(x => x.Name.Trim().ToLower());
                    Console.Write(guildRoles);
                    return x => x.Content
                        .Replace("@", "")
                        .Split(",")
                        .Select(y => y.ToLower().Trim())
                        .Any(y => y == "everyone" || guildRoles.Contains(y));
                }
            }

        }

    }

}