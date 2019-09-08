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
            Content = new DiscordEmbedBuilder {
                Color = new DiscordColor("#EFCEB6"),
                ThumbnailUrl = "https://i.pinimg.com/236x/a4/9c/a3/a49ca31e338b3fab0659e3e3fa92517f--pictures-manga.jpg",
            };
            Content.WithAuthor("Yuuta Bot - Developed by Ab", null, "https://i.imgur.com/YKDFzsB.png");
        }
        //public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands) {
        //    if (Content.Length == 0)
        //        Content.Append("Displaying all available commands.\n\n");
        //    else {
        //        Content.Append("Subcommands:\n");
        //    }
        //    if (subcommands?.Any() == true) {
        //        var ml = subcommands.Max(xc => xc.Name.Length);
        //        var sb = new StringBuilder();
        //        foreach (var xc in subcommands)
        //            sb.Append(xc.Name.PadRight(ml, ' '))
        //                .Append("  ")
        //                .Append(string.IsNullOrWhiteSpace(xc.Description) ? "" : xc.Description).Append("\n");
        //        Content.Append(sb.ToString());
        //    }

        //    return this;
        //}

        ////public override CommandHelpMessage Build()
        ////    => new CommandHelpMessage($"```less\n{Content.ToString().Trim()}\n```");

        //public override CommandHelpMessage Build()
        //    => new
        public override BaseHelpFormatter WithCommand(Command command) {
            Content.WithTitle(command.Name);
            Content.WithDescription(command.Description ?? "No description provided.");
            //if (command.Aliases?.Any() == true)
            //    Content.AddField("Aliases", string.Join(", ", command.Aliases));

            //if (command.Overloads?.Any() == true) {
            //    foreach (var overload in command.Overloads) {
            //        var sb = new StringBuilder();
            //        foreach (var arg in overload.Arguments)
            //            sb.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name).Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? "] " : "> ");
            //        if (sb.Length >= 1) {
            //            sb.Length--;
            //        }
            //        sb.Append("\n\n");
            //        var a = sb.ToString();
            //        foreach (var arg in overload.Arguments) {
            //            sb.Append("**").Append(arg.Name).Append(" (").Append(this.CommandsNext.GetUserFriendlyTypeName(arg.Type)).Append("): ").Append("**").Append(arg.Description ?? "No description provided.").Append("\n");
            //        }
            //        Content.AddField($"Parameters for {command.Name}:", sb.ToString(), true);
            //    }
            //}
            if (command.Aliases?.Any() == true) {
                Content.AddField("Aliases", string.Join(",", command.Aliases));
            }
            if (command.Overloads?.Any() == true) {
                foreach (var overload in command.Overloads) {
                    var sb = new StringBuilder();
                    //foreach (var arg in overload.Arguments) {
                    //    sb.Append(arg.IsOptional || arg.IsCatchAll ? "[Optional] " : "").Append(arg.Name).Append(arg.IsCatchAll ? "..." : "");
                    //    sb.Append("\n");
                    //}
                    //if (sb.Length >= 1) {
                    //    sb.Length--;
                    //}
                    //sb.Append("\n\n");
                    var a = sb.ToString();
                    foreach (var arg in overload.Arguments) {
                        sb.Append("**").Append(arg.Name).Append(arg.IsOptional ? " (optional)**: " : "**: ").Append(arg.Description ?? "No Description Provided");
                        sb.Append("\n");
                    }
                    Content.AddField($"Arguments for the {command.Name} command:", sb.ToString(), true);
                    sb.Clear();
                    sb.Append($"`~{command.Name} ");
                    foreach (var arg in overload.Arguments) {
                        sb.Append(ExampleType(arg.Type) + " ");
                    }
                    if (sb.Length >= 1)
                        sb.Length--;
                    sb.Append("`");
                    Content.AddField("Example", sb.ToString());
                }
            }
            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands) {
            if (Content.Fields?.Any() == false) {
                Content.WithDescription("Displaying all available commands.");
            } else {
                Content.WithDescription("Displaying subcommands");
            }
            if (subcommands?.Any() == true) {
                foreach (var xc in subcommands) {
                    Content.AddField(xc.Name, string.IsNullOrWhiteSpace(xc.Description) ? "No Description Provided" : xc.Description, true);
                }
            }
            return this;
        }

        public override CommandHelpMessage Build()
            => new CommandHelpMessage("", Content.Build());

        private string ExampleType(Type type) {
            if (type == typeof(string)) {
                return "hello world";
            } else if (type == typeof(DiscordMember) || type == typeof(DiscordUser)) {
                return "@BargotGay#1234";
            } else if (type == typeof(int) || type == typeof(long)) {
                return "10";
            } else if (type == typeof(float) || type == typeof(double)) {
                return "10.5";
            } else if (type == typeof(DiscordChannel)) {
                return "#general";
            } else {
                return type.Name;
            }
        }

    }
}
