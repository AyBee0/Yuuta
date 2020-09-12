using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InteractivityHelpers.Entities
{
    public class Interaction<T> where T : ResultEntity, new()
    {
        public string AskMessage { get; private set; }
        public Func<DiscordMessage, bool> Condition { get; private set; }
        public Parser Parser { get; private set; }
        #region Config
        /// <summary>
        /// Make sure the member and channel are the same, defaults to true
        /// </summary>
        public bool ExecuteMemberAndChannelCheck { get; set; } = true;
        /// <summary>
        /// Override the timeout set by the original D#+ interactivity
        /// </summary>
        public TimeSpan? TimeOutOverride { get; set; }
        /// <summary>
        /// Send a message indicating how to cancel. Defaults to false
        /// </summary>
        public bool AppendCancelMessage { get; set; } = false;
        /// <summary>
        /// Accept "none" as a response. Defaults to false.
        /// </summary>
        public bool AcceptNone { get; set; }
        /// <summary>
        /// Override the "none" response keyword.
        /// </summary>
        public string NoneKeyword { get; set; } = "none";
        /// <summary>
        /// Delete after the process is done. Defaults to true.
        /// </summary>
        public bool QueueForDeletion { get; set; } = true;

        /// <summary>
        /// Append the none message automatically if AcceptNone is set to true. Defaults to true.
        /// </summary>
        public bool AppendNoneMessage { get; set; } = true;

        /// <summary>
        /// Message that gets sent indicating that this interaction can be skipped.
        /// </summary>
        public string NoneMessage { get; set; } = "\n*Send \"{0}\" to skip this.*";
        #endregion/ 

        internal Expression<Func<T>> PropertyMap { get; }

        public Interaction(string askMessage, Parser parser, Expression<Func<T>> property, Func<DiscordMessage, bool> condition = null)
        {
            this.AskMessage = askMessage;
            this.Parser = parser;
            this.PropertyMap = property;
            this.Condition = condition;
            string.Format(NoneMessage, NoneKeyword);
        }
    }
}