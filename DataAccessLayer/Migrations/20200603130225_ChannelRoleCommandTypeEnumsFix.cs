using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class ChannelRoleCommandTypeEnumsFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChannelTypes",
                keyColumn: "ChannelTypeId",
                keyValue: 0);

            migrationBuilder.DeleteData(
                table: "ChannelTypes",
                keyColumn: "ChannelTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CommandTypes",
                keyColumn: "CommandTypeId",
                keyValue: 0);

            migrationBuilder.DeleteData(
                table: "CommandTypes",
                keyColumn: "CommandTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CommandTypes",
                keyColumn: "CommandTypeId",
                keyValue: 2);
        }
    }
}
