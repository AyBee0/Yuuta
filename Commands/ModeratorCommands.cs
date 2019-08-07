using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ServerVariable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Commands {

    [Description("A group of staff commands. To see these commands, do ~help staff")]
    [Hidden]
    public class ModeratorCommands : BaseCommandModule {

        private ulong[] detentionAllowedChannels = { 419937457182867457, 597481358784200722, 396233755137671170, 396313821184131074 };

        [Description("[Staff Only] Clears x amount of messages.")]
        [Aliases("delete", "delet")]
        [Command("clear")]
        public async Task Clear(CommandContext ctx, [Description("Number of messages to delete.")] int clearCount, [Description("(Optional) User whose messages are to be deleted. If not specified,.")] DiscordUser user = null) {
            ServerVariables variables = new ServerVariables(ctx);
            if (!variables.IsStaffMember()) {
                await ctx.Channel.SendMessageAsync("You're unegligible to execute that command.");
            }
            await ctx.Message.DeleteAsync();
            if (user == null) {
                List<DiscordMessage> messagesToDelete = new List<DiscordMessage>();
                var messages = await ctx.Channel.GetMessagesAsync(clearCount);
                foreach (var message in messages) {
                    messagesToDelete.Add(message);
                }
                await ctx.Channel.DeleteMessagesAsync(messagesToDelete);
                DiscordMessage confirmationMessage = await ctx.Channel.SendMessageAsync($"Deleted {clearCount} messages, please note that due to Discord limitations I can't delete messages older than 2 weeks.");
                await Task.Run(() => {
                    Thread.Sleep(5000);
                    confirmationMessage.DeleteAsync();
                });
            } else {
                List<DiscordMessage> messagesToDelete = new List<DiscordMessage>();
                var messages = await ctx.Channel.GetMessagesAsync(50);
                var userMessageCount = 0;
                foreach (var message in messages) {
                    if (userMessageCount > clearCount) {
                        break;
                    }
                    if (message.Author == user) {
                        ++userMessageCount;
                        messagesToDelete.Add(message);
                    }
                }
                await ctx.Channel.DeleteMessagesAsync(messagesToDelete);
                DiscordMessage confirmationMessage = await ctx.Channel.SendMessageAsync($"Deleted {clearCount} messages, please note that due to Discord limitations I can't delete messages older than 2 weeks.");
                await Task.Run(() => {
                    Thread.Sleep(5000);
                    confirmationMessage.DeleteAsync();
                });
            }
        }

        [Description("[Staff Only] Hit someone with the ban hammer.")]
        [Command("ban")]
        public async Task Ban(CommandContext ctx, [Description("Who ban")] DiscordMember user, [Description("(Optional, empty=infinite) Ban duration")] int numberOfDaysToBan = 0, [Description("Why ban")] [RemainingText] string reason = "None provided") {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (!serverVariables.IsStaffMember()) {
                await ctx.Channel.SendMessageAsync("Did you really think that would work? I mean, really? You really thought my programmar is that dumb? You really think he'd let a mere non staff member ban someone?");
                return;
            }
            if (ServerVariables.TheBeaconId == ctx.Guild.Id) {
                await ctx.Channel.SendMessageAsync("That command is disabled on this server. Please ban manually.");
                return;
            }
            string username = user.DisplayName;
            await ctx.Guild.BanMemberAsync(user, numberOfDaysToBan, reason);
            int random = new Random().Next(1, 8);
            using (FileStream fs = File.OpenRead(Environment.CurrentDirectory + $"\\other\\hits\\{random}.gif")) {
                await ctx.Message.DeleteAsync();
                await ctx.RespondWithFileAsync(fs, $"Thou who dare desecrate this land of the rising sun! I, Yuuta, lay waste with the vanhammer, and expel thy vast defilement!\n\nBANS {username}!");
            }
        }

        [Description("[Staff Only] Kick someone.")]
        [Command("kick")]
        public async Task Kick(CommandContext ctx, [Description("Who kick")] DiscordMember user, [Description("Why kick")] string reason = "None provided") {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (!serverVariables.IsStaffMember()) {
                await ctx.Channel.SendMessageAsync("Did you really think that would work? I mean, really? You really thought my programmar is that dumb? You really think he'd let a mere non staff member ban someone?");
                return;
            }
            if (ServerVariables.TheBeaconId == ctx.Guild.Id) {
                await ctx.Channel.SendMessageAsync("That command is disabled on this server. Please ban manually.");
                return;
            }
            string username = user.DisplayName;
            await user.RemoveAsync(reason);
            int random = new Random().Next(1, 3);
            using (FileStream fs = File.OpenRead(Environment.CurrentDirectory + $"\\other\\hits\\{random}.gif")) {
                await ctx.Message.DeleteAsync();
                await ctx.RespondWithFileAsync(fs, $"Thou who dare desecrate this land of the rising sun! I, Yuuta, lay waste with the vanhammer, and expel thy vast defilement!\n\nBANS {username}!");
            }
        }

        [Description("[Staff Only] Go on vacation.")]
        [Command("vacation")]
        public async Task Vacation(CommandContext ctx, [Description("Optional - What Member to go on vacation, default is author of command.")] DiscordMember member, [RemainingText] string reason) {
            await ctx.TriggerTypingAsync();
            if (member == null) {
                member = ctx.Member;
            }
            var variables = new ServerVariables(ctx);
            if (!variables.IsStaffMember()) {
                await ctx.Channel.SendMessageAsync("Vacation? Do you work here? Welcome to Lowe's!");
                return;
            }
            var role = variables.GetVacationRole();
            if (role == null) {
                await ctx.Channel.SendMessageAsync("This server does not have a vacation system.");
            } else {
                await member.GrantRoleAsync(role, reason);
                //using (FileStream fs = File.OpenRead(Environment.CurrentDirectory + $"\\pats\\{1}.gif")) {
                //await ctx.Message.DeleteAsync();
                //await ctx.RespondWithFileAsync(fs, $"Thou who dare desecrate this land of the rising sun! I, Yuuta, lay waste with the vanhammer, and expel thy vast defilement!\n\nBANS!");
                //}
                using (FileStream fs = File.OpenRead(Environment.CurrentDirectory + $"\\other\\dance.gif")) {
                    await ctx.Message.DeleteAsync();
                    await ctx.RespondWithFileAsync(fs, $"{member.Mention} has gone on vacation!");
                    await member.SendFileAsync(fs, $"Enjoy your vacation! Admins have been informed that you went on vacation, reason being: `{reason}`.");
                }
                var adminRoles = variables.GetServerAdminRoles();
                foreach (var adminRole in adminRoles) {
                    var admins = ctx.Guild.Members.Values.Where(x => x.Roles.Contains(adminRole));
                    foreach (var admin in admins) {
                        await admin.SendMessageAsync($"{member.DisplayName} has gone on vacation, reason being: `{reason}`");
                    }
                }
            }
        }

        [Command("otp")]
        public async Task OTP(CommandContext ctx) {
            if (ctx.Member.Id != 247386254499381250) {
                return;
            }
            var embedBuilder = new DiscordEmbedBuilder {
                Color = new DiscordColor("#EFCEB6"),
                Title = "How do I get roles and why do I need them?",
                Description = "Upon obtaining a role, you will gain access to that role's respective channels. You will also be pinged for any events related to the game/role." +
                "\nFor example, upon clicking on the Overwatch icon, you'll immediately gain access to the <#368503704581701632> channel, which you'll be free to discuss all things Overwatch related. This applies to" +
                " many other roles aswell.\n\n",
                ThumbnailUrl = "https://i.pinimg.com/236x/a4/9c/a3/a49ca31e338b3fab0659e3e3fa92517f--pictures-manga.jpg",
            };
            embedBuilder.WithAuthor("GAME ROLES!");
            embedBuilder.AddField("__**How do I get myself a role**__?", "Below are our available roles. Get the role by clicking on the emoji reaction respective to the role at the very end of this message." +
                "\n\n**__GAME ROLES__**");
            #region Games
            //other channels
            embedBuilder.AddField("For games without a channel", "For games without a channel/role, do go to <#346644433967644672>");
            //overwatch
            embedBuilder.AddField("Overwatch: <:overwatch:602570373615190016>", "<#368503704581701632>", true);
            //Rainbow
            embedBuilder.AddField("Rainbow 6: <:rainbow:602570339813163026>", "<#368504087635034112>", true);
            //Apex
            embedBuilder.AddField("Apex Legends: <:apex:602570214072385536>", "<#548125790789828633>", true);
            //CSGO
            embedBuilder.AddField("CS:GO: <:csgo:602573024452935680>", "<#346644433967644672>", true);
            //Dauntless
            embedBuilder.AddField("Dauntless: <:dauntless:602569854259560458>", "<#584510375794442250>", true);
            //Destiny
            embedBuilder.AddField("Destiny: <:destiny:602571885657718822>", "<#346644433967644672>", true);
            //Fortnite
            embedBuilder.AddField("Fortnite: <:fortnite:602570231306518528>", "<#346644433967644672>", true);
            //League of Legends
            embedBuilder.AddField("League of Legends: <:lol:602570255646326784>", "<#346644433967644672>", true);
            //Mario Kart
            embedBuilder.AddField("Mario Kart: <:mariokart:602572110132674570>", "<#550488213509373972>", true);
            //Magic
            embedBuilder.AddField("MTG: <:mtg:602570317587677184>", "<#552511898176716831>", true);
            //PUBG
            embedBuilder.AddField("PUBG: <:pubg:602570335514132558>", "<#346644433967644672>", true);
            //Roller Champions
            embedBuilder.AddField("Roller Champions: <:rollerchampions:602571869438083093>", "<#346644433967644672>", true);
            //SMM2
            embedBuilder.AddField("SMM2: <:smm2:602571872630079565>", "<#550488213509373972>", true);
            //Splatoon
            embedBuilder.AddField("Splatoon: <:splatoon:602570433811972106>", "<#550488213509373972>", true);
            //Warframe
            embedBuilder.AddField("Warframe: <:warframe:602570402035924993>", "<#346644433967644672>.", true);
            //Paladins
            embedBuilder.AddField("Paladins: <:paladins:607216511513526291>", "<#346644433967644672>.", true);
            //Super Smash Bros
            embedBuilder.AddField("Smash Bros: <:smash:602570362261209088>", "<#606420207426207757>", true);
            //Minecraft
            embedBuilder.AddField("Minecraft: <:minecraft:602572142667759616>", $"<#607209457910415385>. We have a server! The IP is: thebeaconfficial.nitrous.it \nWe are currently running 1.14, as of right now we have no plugin. If you would like to join the server Please DM <@!214562659977134080> your Minecraft username so he can add you to the whitelist.", true);
            #endregion
            //await ctx.RespondAsync("", false, embedBuilder.Build());
            var gameMessage = await ctx.Channel.GetMessageAsync(607254115512483860);
            await gameMessage.ModifyAsync("", embedBuilder.Build());
            //var embedBuilder2 = new DiscordEmbedBuilder {
            //    Color = new DiscordColor("#EFCEB6"),
            //    Title = "How do I get roles and why do I need them?",
            //    Description = "Upon obtaining one of the roles below, you will get pinged depending on the role." +
            //    "\nFor example, upon clicking on the Event icon, you will get pinged for general events. As another example, if you have the Movie Night role, you will get pinged on our next Movie Night.",
            //    ThumbnailUrl = "https://i.pinimg.com/236x/a4/9c/a3/a49ca31e338b3fab0659e3e3fa92517f--pictures-manga.jpg",
            //};
            //#region Fields
            //embedBuilder2.WithAuthor("OTHER ROLES!");
            //embedBuilder2.AddField("__**How do I get myself a role**__?", "Below are our available roles. Get the role by clicking on the emoji reaction respective to the role at the very end of this message." +
            //    "\n\n**__EVENT ROLES__**");
            //embedBuilder2.AddField("@Event <a:events:605751912473690116>", "Get pinged for general server events.\n\n**__REGION AND LFG ROLES__**", true);
            //embedBuilder2.AddField("@Movie Night <:popcorn_blob:605757092627873804>", "Get pinged for Movie Nights!", true);
            //embedBuilder2.AddField("__Region Roles__ <:eu:605868737123450885> - <:na:605868984004378634> - <:oce:605869308731588755> - <:as:605869133237846072>", "Let people know what region you play/are in.\n");
            //embedBuilder2.AddField("__LFG Roles__ <:lfgeu:605735395455402004> - <:lfgas:605735840827834393> - <:lfgoce:605735621595758622> - <:lfgna:605735102722342933>", "Each region has its own LFG role. For example, if you get the <:lfgeu:605735395455402004> role, and someone does <@&442095956960346113> in <#368503704581701632> (if you have the Overwatch role), you will get pinged.\n\n**__OTHER__**");
            //embedBuilder2.AddField("Memer Role <:memes:605862790489440286>", "Gives access to <#311715143526645760>.");
            //#endregion
            //await ctx.RespondAsync("", false, embedBuilder2.Build());
        }

        [Command("otp2")]
        public async Task OP2(CommandContext ctx) {
            if (ctx.Member.Id != 247386254499381250) {
                return;
            }
            await ctx.Message.DeleteAsync();
            var message = await ctx.Channel.GetMessageAsync(607964037405343756);
            var embedBuilder = new DiscordEmbedBuilder {
                Color = new DiscordColor("#EFCEB6"),
                Title = "Choose Your Team!",
                Description = "A war has broken out in The Beacon between Bargot and Ab. Whose side will you join? Team Ab <:ab:607949913770164235>, or Team Bargot <:bargot:607949915376582658>? Or maybe you'd like to fight along the resistance to end the war, with Team Neutral <:neutral:607950400280199189>?\n\n" +
                "**__In any case, to join a side, click the reaction for the team you want to join. For example, if you click on the <:ab:607949913770164235> button at the very end of this message, you will join Team Ab.\nHere are the corresponding reactions:__**",
                ThumbnailUrl = "https://i.pinimg.com/236x/a4/9c/a3/a49ca31e338b3fab0659e3e3fa92517f--pictures-manga.jpg"
            };
            embedBuilder.WithAuthor("Bot by Ab", null, "https://cdn.discordapp.com/avatars/247386254499381250/c7b7ee45d5ad21046d6dacbcb80e1147.png");
            embedBuilder.AddField("Team Ab <:ab:607949913770164235>", "Join us and we will do whatever it takes to acheive salvation. Our religion consists of thighs, thigh highs, Shawarma, Hummus, and Skittles. We obey the all mighty Thanos, as he is our ideology. RIP Thanos 2019 we will forever pray to our god.\nSalvation will come.",true);
            embedBuilder.AddField("Team Bargot <:bargot:607949915376582658>", "Team Bargot is the team of greatness and peak performance. Join us to achieve your dreams as well as ultimate dankness", true);
            embedBuilder.AddField("Team Neutral <:neutral:607950400280199189>", "Team Neutral supports fairness for others and aim not to fight. Although that's only lawful neutral");
            embedBuilder.WithFooter("Look between me and you Ab is my creator so like join his side he has hummus. ...---...");
            await message.ModifyAsync("",embedBuilder.Build());
        }
        //[Command("otp2")]
        //public async Task OTP2(CommandContext ctx) {
        //    if (ctx.Member.Id != 247386254499381250) {
        //        return;
        //    }
        //    var embedBuilder = new DiscordEmbedBuilder {
        //        Color = new DiscordColor("#EFCEB6"),
        //        Title = "Choose Your Team?",
        //        Description = "Choose on of your teams below",
        //        ThumbnailUrl = "https://i.pinimg.com/236x/a4/9c/a3/a49ca31e338b3fab0659e3e3fa92517f--pictures-manga.jpg",
        //    };

        //}

        [Description("[Staff Only] Make user an active member.")]
        [Aliases("activemember", "giveactive")]
        [Command("makeactive")]
        public async Task MakeActive(CommandContext ctx, DiscordMember member) {
            var serverVariables = new ServerVariables(ctx);
            if (ctx.Guild.Id == ServerVariables.TheBeaconId && serverVariables.IsStaffMember()) {
                var activeRole = ctx.Guild.GetRole(ServerVariables.ActiveMemberRole);
                await member.GrantRoleAsync(activeRole);
            }
        }

        [Description("[Staff Only] Make user a veteran member.")]
        [Aliases("veteran", "vet", "givevet", "giveveteran")]
        [Command("makevet")]
        public async Task MakeVet(CommandContext ctx, DiscordMember member) {
            var serverVariables = new ServerVariables(ctx);
            if (ctx.Guild.Id == ServerVariables.TheBeaconId && serverVariables.IsStaffMember()) {
                var vetRole = ctx.Guild.GetRole(ServerVariables.VetMemberRole);
                await member.GrantRoleAsync(vetRole);
            }
        }

        [Aliases("detention","mute")]
        [Description("[Staff Only] Detain someone")]
        [Command("detain")]
        public async Task Detain(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null) {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.IsStaffMember()) {
                if (ctx.Guild.Id == ServerVariables.TheBeaconId) {
                    var detentionRole = ctx.Guild.GetRole(597797180778217474);
                    var memberRole = ctx.Guild.GetRole(310281719808917515);
                    var socialiteRole = ctx.Guild.GetRole(345319685052694528);
                    if (member.Roles.Contains(memberRole)) {
                        await member.GrantRoleAsync(ctx.Guild.GetRole(608222312923136012));
                    } else if (member.Roles.Contains(socialiteRole)) {
                        await member.GrantRoleAsync(ctx.Guild.GetRole(608222408574500867));
                    }
                    await member.RevokeRoleAsync(memberRole);
                    await member.RevokeRoleAsync(socialiteRole);
                    await member.GrantRoleAsync(detentionRole, reason);
                }
                if (serverVariables.GetDetentionChannel() != null) {
                    var reasonMessage = reason ?? "No reason specified";
                    await serverVariables.GetDetentionChannel().SendMessageAsync($"{member.Mention}, you have been detained by {ctx.Member.Mention} for the following reason:\n ```diff\n- {reasonMessage}\n```");
                }
            }
        }

        [Aliases("undetention", "unmute")]
        [Description("[Staff Only] Undetain someone")]
        [Command("undetain")]
        public async Task UnDetain(CommandContext ctx, DiscordMember member) {
            ServerVariables serverVariables = new ServerVariables(ctx);
            if (serverVariables.IsStaffMember()) {
                if (ctx.Guild.Id == ServerVariables.TheBeaconId) {
                    var detentionRole = ctx.Guild.GetRole(597797180778217474);
                    var memberRole = ctx.Guild.GetRole(310281719808917515);
                    var socialiteRole = ctx.Guild.GetRole(345319685052694528);
                    var detainedMemberRole = ctx.Guild.GetRole(608222312923136012);
                    var detainedSocialiteRole = ctx.Guild.GetRole(608222408574500867);
                    if (member.Roles.Contains(detainedMemberRole)) {
                        await member.RevokeRoleAsync(detainedMemberRole);
                        await member.GrantRoleAsync(memberRole);
                    } else if (member.Roles.Contains(detainedSocialiteRole)) {
                        await member.RevokeRoleAsync(detainedSocialiteRole);
                        await member.GrantRoleAsync(socialiteRole);
                    } else {
                        await member.RevokeRoleAsync(detainedSocialiteRole);
                        await member.RevokeRoleAsync(detainedMemberRole);
                        await member.GrantRoleAsync(memberRole);
                    }
                    await member.RevokeRoleAsync(detentionRole);
                }
            }
        }

    }
}