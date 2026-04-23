using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AethirMaelWebApplication.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminStatsPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Pacienti",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Pacienti");
        }
    }
}
