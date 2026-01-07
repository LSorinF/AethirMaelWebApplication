using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AethirMaelWebApplication.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddDateModifiedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "FiseMedicale",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "FiseMedicale");
        }
    }
}
