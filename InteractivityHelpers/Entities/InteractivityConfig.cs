using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace InteractivityHelpers.Entities
{
    public class InteractionConfig
    {
        /// <summary>
        /// Make sure the member and channel are the same, defaults to true
        /// </summary>
        public bool ExecuteMemberAndChannelCheck { get; set; } = true;
        /// <summary>
        /// Override the timeout set by the original D#+ interactivity
        /// </summary>
        public TimeSpan? TimeOutOverride { get; set; }
        /// <summary>
        /// Send a message indicating how to cancel.
        /// </summary>
        public bool AppendCancel { get; set; }
        /// <summary>
        /// Accept "none" as a response.
        /// </summary>
        public bool AcceptNone { get; set; }
        /// <summary>
        /// Override the "none" response.
        /// </summary>
        public string NoneOverride { get; set; } = "none";
        /// <summary>
        /// Delete after the process is done.
        /// </summary>
        public bool QueueForDeletion { get; set; } = true;

        /// <summary>
        /// Append the none message automatically if AcceptNone is set to true.
        /// </summary>
        public bool AppendNoneMessage { get; set; } = true;

        public string NoneMessage { get; set; } = "\n*Send \"none\" to skip this.*";
    }
}
