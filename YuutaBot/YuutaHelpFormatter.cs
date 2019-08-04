using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuutaBot {

    public sealed class YuutaHelpFormatter : BaseHelpFormatter {

        private DiscordEmbedBuilder Content { get; }

        public YuutaHelpFormatter(CommandContext ctx) : base(ctx) {
            this.Content = new DiscordEmbedBuilder {
                Color = new DiscordColor("#EFCEB6"),
                ThumbnailUrl = "https://i.pinimg.com/236x/a4/9c/a3/a49ca31e338b3fab0659e3e3fa92517f--pictures-manga.jpg",
            };
            this.Content.WithAuthor("Yuuta Bot - Developed by Ab", null, "https://i.imgur.com/YKDFzsB.png");
        }
        //public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands) {
        //    if (this.Content.Length == 0)
        //        this.Content.Append("Displaying all available commands.\n\n");
        //    else {
        //        this.Content.Append("Subcommands:\n");
        //    }
        //    if (subcommands?.Any() == true) {
        //        var ml = subcommands.Max(xc => xc.Name.Length);
        //        var sb = new StringBuilder();
        //        foreach (var xc in subcommands)
        //            sb.Append(xc.Name.PadRight(ml, ' '))
        //                .Append("  ")
        //                .Append(string.IsNullOrWhiteSpace(xc.Description) ? "" : xc.Description).Append("\n");
        //        this.Content.Append(sb.ToString());
        //    }

        //    return this;
        //}

        ////public override CommandHelpMessage Build()
        ////    => new CommandHelpMessage($"```less\n{this.Content.ToString().Trim()}\n```");

        //public override CommandHelpMessage Build()
        //    => new
        public override BaseHelpFormatter WithCommand(Command command) {
            Content.WithTitle(command.Name);
            Content.WithDescription(command.Description ?? "No description provided.");
            if (command.Aliases?.Any() == true)
                Content.AddField("Aliases", string.Join(", ", command.Aliases));

            if (command.Overloads?.Any() == true) {
                foreach (var overload in command.Overloads) {
                    var sb = new StringBuilder();
                    foreach (var arg in overload.Arguments)
                        sb.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name).Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? "] " : "> ");
                    if (sb.Length >= 1) {
                        sb.Length--;
                    }
                    sb.Append("\n\n");
                    var a = sb.ToString();
                    foreach (var arg in overload.Arguments) {
                        sb.Append("**").Append(arg.Name).Append(" (").Append(this.CommandsNext.GetUserFriendlyTypeName(arg.Type)).Append("): ").Append("**").Append(arg.Description ?? "No description provided.").Append("\n");
                    }
                    Content.AddField($"Parameters for {command.Name}:", sb.ToString(), true);
                }
            }

            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands) {
            if (Content.Fields?.Any() == false) {
                this.Content.WithDescription("Displaying all available commands.");
            } else {
                this.Content.WithDescription("Displaying subcommands");
            }
            if (subcommands?.Any() == true) {
                foreach (var xc in subcommands) {
                    this.Content.AddField(xc.Name, string.IsNullOrWhiteSpace(xc.Description) ? "No Description Provided" : xc.Description,true);
                }
            }
            return this;
        }

        public override CommandHelpMessage Build()
            => new CommandHelpMessage("",Content.Build());
    }
}
