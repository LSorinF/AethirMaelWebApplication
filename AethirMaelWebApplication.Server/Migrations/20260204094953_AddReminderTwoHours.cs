using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AethirMaelWebApplication.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderTwoHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReminderTwoHoursSent",
                table: "Programari",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderTwoHoursSent",
                table: "Programari");
        }
    }
}
