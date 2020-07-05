using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class GuildSettingsId5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChannelTypes",
                columns: table => new
                {
                    ChannelTypeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelTypes", x => x.ChannelTypeId);
                });

            migrationBuilder.CreateTable(
                name: "CommandTypes",
                columns: table => new
                {
                    CommandTypeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandTypes", x => x.CommandTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    GuildId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildDid = table.Column<long>(nullable: false),
                    GuildName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "RoleTypes",
                columns: table => new
                {
                    RoleTypeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleTypes", x => x.RoleTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Commands",
                columns: table => new
                {
                    CommandId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommandTrigger = table.Column<string>(nullable: false),
                    CommandTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commands", x => x.CommandId);
                    table.ForeignKey(
                        name: "FK_Commands_CommandTypes_CommandTypeId",
                        column: x => x.CommandTypeId,
                        principalTable: "CommandTypes",
                        principalColumn: "CommandTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    ChannelId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelDid = table.Column<long>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    ChannelTypeId = table.Column<int>(nullable: false),
                    GuildId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.ChannelId);
                    table.ForeignKey(
                        name: "FK_Channels_ChannelTypes_ChannelTypeId",
                        column: x => x.ChannelTypeId,
                        principalTable: "ChannelTypes",
                        principalColumn: "ChannelTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Channels_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildMacros",
                columns: table => new
                {
                    GuildMacroId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeleteAfterSend = table.Column<bool>(nullable: false),
                    Response = table.Column<string>(nullable: false),
                    GuildId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildMacros", x => x.GuildMacroId);
                    table.ForeignKey(
                        name: "FK_GuildMacros_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildSettings",
                columns: table => new
                {
                    GuildSettingId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WelcomeChannel = table.Column<string>(nullable: true),
                    WelcomeMessage = table.Column<string>(nullable: true),
                    GoodbyeChannel = table.Column<string>(nullable: true),
                    GoodbyeMessage = table.Column<string>(nullable: true),
                    GuildId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildSettings", x => x.GuildSettingId);
                    table.ForeignKey(
                        name: "FK_GuildSettings_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleMessages",
                columns: table => new
                {
                    RoleMessageId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChannelDid = table.Column<long>(nullable: false),
                    MessageDid = table.Column<long>(nullable: false),
                    GuildId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMessages", x => x.RoleMessageId);
                    table.ForeignKey(
                        name: "FK_RoleMessages_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommandRestrictionOverloads",
                columns: table => new
                {
                    RestrictionOverloadId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Authorize = table.Column<bool>(nullable: false),
                    RoleDid = table.Column<long>(nullable: false),
                    GuildDid = table.Column<long>(nullable: false),
                    CommandId = table.Column<int>(nullable: false)
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
                name: "Attachment",
                columns: table => new
                {
                    AttachmentId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AttachmentUrl = table.Column<string>(nullable: false),
                    GuildMacroId = table.Column<int>(nullable: false)
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
                    RoleMessageItemId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmojiName = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    RoleMessageId = table.Column<int>(nullable: false)
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
                    RoleId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleTypeId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    RoleDid = table.Column<long>(nullable: false),
                    GuildId = table.Column<int>(nullable: false),
                    RoleMessageItemId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_Roles_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "GuildId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Roles_RoleMessageItems_RoleMessageItemId",
                        column: x => x.RoleMessageItemId,
                        principalTable: "RoleMessageItems",
                        principalColumn: "RoleMessageItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Roles_RoleTypes_RoleTypeId",
                        column: x => x.RoleTypeId,
                        principalTable: "RoleTypes",
                        principalColumn: "RoleTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ChannelTypes",
                columns: new[] { "ChannelTypeId", "Description", "Name" },
                values: new object[] { 0, "Normal", "Normal" });

            migrationBuilder.InsertData(
                table: "ChannelTypes",
                columns: new[] { "ChannelTypeId", "Description", "Name" },
                values: new object[] { 1, "BotChannel", "BotChannel" });

            migrationBuilder.InsertData(
                table: "CommandTypes",
                columns: new[] { "CommandTypeId", "Description", "Name" },
                values: new object[] { 0, "Normal", "Normal" });

            migrationBuilder.InsertData(
                table: "CommandTypes",
                columns: new[] { "CommandTypeId", "Description", "Name" },
                values: new object[] { 1, "ForcedGlobal", "ForcedGlobal" });

            migrationBuilder.InsertData(
                table: "CommandTypes",
                columns: new[] { "CommandTypeId", "Description", "Name" },
                values: new object[] { 2, "StaffOnly", "StaffOnly" });

            migrationBuilder.InsertData(
                table: "RoleTypes",
                columns: new[] { "RoleTypeId", "Description", "Name" },
                values: new object[] { 0, "Normal", "Normal" });

            migrationBuilder.InsertData(
                table: "RoleTypes",
                columns: new[] { "RoleTypeId", "Description", "Name" },
                values: new object[] { 1, "GlobalCommands", "GlobalCommands" });

            migrationBuilder.InsertData(
                table: "RoleTypes",
                columns: new[] { "RoleTypeId", "Description", "Name" },
                values: new object[] { 2, "GlobalMacros", "GlobalMacros" });

            migrationBuilder.InsertData(
                table: "RoleTypes",
                columns: new[] { "RoleTypeId", "Description", "Name" },
                values: new object[] { 3, "Staff", "Staff" });

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_GuildMacroId",
                table: "Attachment",
                column: "GuildMacroId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_ChannelTypeId",
                table: "Channels",
                column: "ChannelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_GuildId",
                table: "Channels",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_CommandRestrictionOverloads_CommandId",
                table: "CommandRestrictionOverloads",
                column: "CommandId");

            migrationBuilder.CreateIndex(
                name: "IX_Commands_CommandTypeId",
                table: "Commands",
                column: "CommandTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildMacros_GuildId",
                table: "GuildMacros",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildSettings_GuildId",
                table: "GuildSettings",
                column: "GuildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleMessageItems_RoleMessageId",
                table: "RoleMessageItems",
                column: "RoleMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMessages_GuildId",
                table: "RoleMessages",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_GuildId",
                table: "Roles",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleMessageItemId",
                table: "Roles",
                column: "RoleMessageItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleTypeId",
                table: "Roles",
                column: "RoleTypeId");
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
                name: "GuildSettings");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "GuildMacros");

            migrationBuilder.DropTable(
                name: "ChannelTypes");

            migrationBuilder.DropTable(
                name: "Commands");

            migrationBuilder.DropTable(
                name: "RoleMessageItems");

            migrationBuilder.DropTable(
                name: "RoleTypes");

            migrationBuilder.DropTable(
                name: "CommandTypes");

            migrationBuilder.DropTable(
                name: "RoleMessages");

            migrationBuilder.DropTable(
                name: "Guilds");
        }
    }
}
