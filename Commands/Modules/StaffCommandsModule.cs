﻿using Commands.YuutaTasks;
using DiscordMan.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using InteractivityHelpers;
using Serilog;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Commands.Modules
{
    [StaffOnly]
    public class StaffCommandsModule : YuutaCommandModule
    {
        [Command]
        [Description("Create a new reaction message.")]
        public async Task NewReactionMessage(CommandContext ctx)
        {

        }

        [Interaction]
        [Command]
        [Description("Create a new event")]
        public async Task NewEvent(CommandContext ctx)
        {
            try
            {
                await Tasks.NewEventAsync(ctx);
            }
            catch (Exception e)
            {
                await ctx.RespondAsync($":x: Sorry, something went wrong.");
                Log.Logger.Fatal(e, "Exception occured in the NewEvent task.");
            }
        }

    }
}
