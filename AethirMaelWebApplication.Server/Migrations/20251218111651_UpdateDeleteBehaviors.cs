using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AethirMaelWebApplication.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FiseMedicale_Medici_DoctorId",
                table: "FiseMedicale");

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "FiseMedicale",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_FiseMedicale_Medici_DoctorId",
                table: "FiseMedicale",
                column: "DoctorId",
                principalTable: "Medici",
                principalColumn: "DoctorId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FiseMedicale_Medici_DoctorId",
                table: "FiseMedicale");

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "FiseMedicale",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FiseMedicale_Medici_DoctorId",
                table: "FiseMedicale",
                column: "DoctorId",
                principalTable: "Medici",
                principalColumn: "DoctorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
