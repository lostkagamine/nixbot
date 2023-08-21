using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NixBot.Migrations
{
    /// <inheritdoc />
    public partial class LastTriggerMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "LastTriggerMessage",
                table: "Messages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTriggerMessage",
                table: "Messages");
        }
    }
}
