using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class ReactionLinkedEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Commands",
                columns: table => new
                {
                    CommandId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommandName = table.Column<string>(type: "TEXT", nullable: false),
                    CommandType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commands", x => x.CommandId);
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    GuildDid = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.GuildDid);
                });

            migrationBuilder.CreateTable(
                name: "CommandRestrictionOverloads",
                columns: table => new
                {
                    RestrictionOverloadId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Authorize = table.Column<bool>(type: "INTEGER", nullable: false),
                    RoleDid = table.Column<ulong>(type: "INTEGER", nullable: false),
                    GuildDid = table.Column<ulong>(type: "INTEGER", nullable: false),
                    CommandId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandRestrictionOverloads", x => x.RestrictionOverloadId);
                    table.ForeignKey(
                        name: "FK_CommandRestrictionOverloads_Commands_CommandId",
                        column: x => x.CommandId,
                        principalTable: "Commands",
                        principalColumn: "CommandId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    ChannelDid = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    ChannelType = table.Column<int>(type: "INTEGER", nullable: false),
                    GuildId = table.Column<int>(type: "INTEGER", nullable: false),
                    GuildDid = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.ChannelDid);
                    table.ForeignKey(
                        name: "FK_Channels_Guilds_GuildDid",
                        column: x => x.GuildDid,
                        principalTable: "Guilds",
                        principalColumn: "GuildDid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: true),
                    ChannelId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    MessageId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    ChannelToSend = table.Column<ulong>(type: "INTEGER", nullable: true),
                    GuildMessageEvent_Text = table.Column<string>(type: "TEXT", nullable: true),
                    User = table.Column<ulong>(type: "INTEGER", nullable: true),
                    RoleId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    RoleEventType = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Event_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildDid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildMacros",
                columns: table => new
                {
                    GuildMacroId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeleteAfterSend = table.Column<bool>(type: "INTEGER", nullable: false),
                    Response = table.Column<string>(type: "TEXT", nullable: false),
                    GuildId = table.Column<int>(type: "INTEGER", nullable: false),
                    GuildDid = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildMacros", x => x.GuildMacroId);
                    table.ForeignKey(
                        name: "FK_GuildMacros_Guilds_GuildDid",
                        column: x => x.GuildDid,
                        principalTable: "Guilds",
                        principalColumn: "GuildDid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildSettings",
                columns: table => new
                {
                    GuildSettingId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WelcomeChannel = table.Column<string>(type: "TEXT", nullable: true),
                    WelcomeMessage = table.Column<string>(type: "TEXT", nullable: true),
                    GoodbyeChannel = table.Column<string>(type: "TEXT", nullable: true),
                    GoodbyeMessage = table.Column<string>(type: "TEXT", nullable: true),
                    GuildDid = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildSettings", x => x.GuildSettingId);
                    table.ForeignKey(
                        name: "FK_GuildSettings_Guilds_GuildDid",
                        column: x => x.GuildDid,
                        principalTable: "Guilds",
                        principalColumn: "GuildDid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleMessages",
                columns: table => new
                {
                    RoleMessageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelDid = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MessageDid = table.Column<ulong>(type: "INTEGER", nullable: false),
                    GuildId = table.Column<int>(type: "INTEGER", nullable: false),
                    GuildDid = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMessages", x => x.RoleMessageId);
                    table.ForeignKey(
                        name: "FK_RoleMessages_Guilds_GuildDid",
                        column: x => x.GuildDid,
                        principalTable: "Guilds",
                        principalColumn: "GuildDid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventUser",
                columns: table => new
                {
                    EventUserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventUserDid = table.Column<ulong>(type: "INTEGER", nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    DirectMessageEventEventId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventUser", x => x.EventUserId);
                    table.ForeignKey(
                        name: "FK_EventUser_Event_DirectMessageEventEventId",
                        column: x => x.DirectMessageEventEventId,
                        principalTable: "Event",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventUser_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AttachmentUrl = table.Column<string>(type: "TEXT", nullable: false),
                    GuildMacroId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.AttachmentId);
                    table.ForeignKey(
                        name: "FK_Attachment_GuildMacros_GuildMacroId",
                        column: x => x.GuildMacroId,
                        principalTable: "GuildMacros",
                        principalColumn: "GuildMacroId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleMessageItems",
                columns: table => new
                {
                    RoleMessageItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmojiName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    RoleMessageId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMessageItems", x => x.RoleMessageItemId);
                    table.ForeignKey(
                        name: "FK_RoleMessageItems_RoleMessages_RoleMessageId",
                        column: x => x.RoleMessageId,
                        principalTable: "RoleMessages",
                        principalColumn: "RoleMessageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleDid = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleType = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    GuildId = table.Column<int>(type: "INTEGER", nullable: false),
                    GuildDid = table.Column<ulong>(type: "INTEGER", nullable: true),
                    RoleMessageItemId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleDid);
                    table.ForeignKey(
                        name: "FK_Roles_Guilds_GuildDid",
                        column: x => x.GuildDid,
                        principalTable: "Guilds",
                        principalColumn: "GuildDid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Roles_RoleMessageItems_RoleMessageItemId",
                        column: x => x.RoleMessageItemId,
                        principalTable: "RoleMessageItems",
                        principalColumn: "RoleMessageItemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_GuildMacroId",
                table: "Attachment",
                column: "GuildMacroId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_GuildDid",
                table: "Channels",
                column: "GuildDid");

            migrationBuilder.CreateIndex(
                name: "IX_CommandRestrictionOverloads_CommandId",
                table: "CommandRestrictionOverloads",
                column: "CommandId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_GuildId",
                table: "Event",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_EventUser_DirectMessageEventEventId",
                table: "EventUser",
                column: "DirectMessageEventEventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventUser_EventId",
                table: "EventUser",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildMacros_GuildDid",
                table: "GuildMacros",
                column: "GuildDid");

            migrationBuilder.CreateIndex(
                name: "IX_GuildSettings_GuildDid",
                table: "GuildSettings",
                column: "GuildDid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleMessageItems_RoleMessageId",
                table: "RoleMessageItems",
                column: "RoleMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMessages_GuildDid",
                table: "RoleMessages",
                column: "GuildDid");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_GuildDid",
                table: "Roles",
                column: "GuildDid");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleMessageItemId",
                table: "Roles",
                column: "RoleMessageItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "CommandRestrictionOverloads");

            migrationBuilder.DropTable(
                name: "EventUser");

            migrationBuilder.DropTable(
                name: "GuildSettings");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "GuildMacros");

            migrationBuilder.DropTable(
                name: "Commands");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "RoleMessageItems");

            migrationBuilder.DropTable(
                name: "RoleMessages");

            migrationBuilder.DropTable(
                name: "Guilds");
        }
    }
}
