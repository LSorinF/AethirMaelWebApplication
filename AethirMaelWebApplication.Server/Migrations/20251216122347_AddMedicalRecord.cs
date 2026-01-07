using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AethirMaelWebApplication.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicalRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Doctors_DoctorId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Specializations_SpecializationId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Doctors_DoctorId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Patients_PatientId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Specializations",
                table: "Specializations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Doctors",
                table: "Doctors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Utilizatori");

            migrationBuilder.RenameTable(
                name: "Specializations",
                newName: "Specializari");

            migrationBuilder.RenameTable(
                name: "Patients",
                newName: "Pacienti");

            migrationBuilder.RenameTable(
                name: "Doctors",
                newName: "Medici");

            migrationBuilder.RenameTable(
                name: "Appointments",
                newName: "Programari");

            migrationBuilder.RenameIndex(
                name: "IX_Users_PatientId",
                table: "Utilizatori",
                newName: "IX_Utilizatori_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "Utilizatori",
                newName: "IX_Utilizatori_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Users_DoctorId",
                table: "Utilizatori",
                newName: "IX_Utilizatori_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_Email",
                table: "Pacienti",
                newName: "IX_Pacienti_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_CNP",
                table: "Pacienti",
                newName: "IX_Pacienti_CNP");

            migrationBuilder.RenameIndex(
                name: "IX_Doctors_SpecializationId",
                table: "Medici",
                newName: "IX_Medici_SpecializationId");

            migrationBuilder.RenameIndex(
                name: "IX_Doctors_Email",
                table: "Medici",
                newName: "IX_Medici_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_PatientId",
                table: "Programari",
                newName: "IX_Programari_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_DoctorId",
                table: "Programari",
                newName: "IX_Programari_DoctorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Utilizatori",
                table: "Utilizatori",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specializari",
                table: "Specializari",
                column: "SpecializationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pacienti",
                table: "Pacienti",
                column: "PatientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medici",
                table: "Medici",
                column: "DoctorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Programari",
                table: "Programari",
                column: "AppointmentId");

            migrationBuilder.CreateTable(
                name: "FiseMedicale",
                columns: table => new
                {
                    MedicalRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Symptoms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Diagnosis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Treatment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvestigationResults = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    AppointmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiseMedicale", x => x.MedicalRecordId);
                    table.ForeignKey(
                        name: "FK_FiseMedicale_Medici_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Medici",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FiseMedicale_Pacienti_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Pacienti",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FiseMedicale_Programari_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Programari",
                        principalColumn: "AppointmentId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FiseMedicale_AppointmentId",
                table: "FiseMedicale",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FiseMedicale_DoctorId",
                table: "FiseMedicale",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_FiseMedicale_PatientId",
                table: "FiseMedicale",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medici_Specializari_SpecializationId",
                table: "Medici",
                column: "SpecializationId",
                principalTable: "Specializari",
                principalColumn: "SpecializationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Programari_Medici_DoctorId",
                table: "Programari",
                column: "DoctorId",
                principalTable: "Medici",
                principalColumn: "DoctorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Programari_Pacienti_PatientId",
                table: "Programari",
                column: "PatientId",
                principalTable: "Pacienti",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizatori_Medici_DoctorId",
                table: "Utilizatori",
                column: "DoctorId",
                principalTable: "Medici",
                principalColumn: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizatori_Pacienti_PatientId",
                table: "Utilizatori",
                column: "PatientId",
                principalTable: "Pacienti",
                principalColumn: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medici_Specializari_SpecializationId",
                table: "Medici");

            migrationBuilder.DropForeignKey(
                name: "FK_Programari_Medici_DoctorId",
                table: "Programari");

            migrationBuilder.DropForeignKey(
                name: "FK_Programari_Pacienti_PatientId",
                table: "Programari");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilizatori_Medici_DoctorId",
                table: "Utilizatori");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilizatori_Pacienti_PatientId",
                table: "Utilizatori");

            migrationBuilder.DropTable(
                name: "FiseMedicale");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Utilizatori",
                table: "Utilizatori");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Specializari",
                table: "Specializari");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Programari",
                table: "Programari");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pacienti",
                table: "Pacienti");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medici",
                table: "Medici");

            migrationBuilder.RenameTable(
                name: "Utilizatori",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "Specializari",
                newName: "Specializations");

            migrationBuilder.RenameTable(
                name: "Programari",
                newName: "Appointments");

            migrationBuilder.RenameTable(
                name: "Pacienti",
                newName: "Patients");

            migrationBuilder.RenameTable(
                name: "Medici",
                newName: "Doctors");

            migrationBuilder.RenameIndex(
                name: "IX_Utilizatori_PatientId",
                table: "Users",
                newName: "IX_Users_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Utilizatori_Email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Utilizatori_DoctorId",
                table: "Users",
                newName: "IX_Users_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Programari_PatientId",
                table: "Appointments",
                newName: "IX_Appointments_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Programari_DoctorId",
                table: "Appointments",
                newName: "IX_Appointments_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Pacienti_Email",
                table: "Patients",
                newName: "IX_Patients_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Pacienti_CNP",
                table: "Patients",
                newName: "IX_Patients_CNP");

            migrationBuilder.RenameIndex(
                name: "IX_Medici_SpecializationId",
                table: "Doctors",
                newName: "IX_Doctors_SpecializationId");

            migrationBuilder.RenameIndex(
                name: "IX_Medici_Email",
                table: "Doctors",
                newName: "IX_Doctors_Email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specializations",
                table: "Specializations",
                column: "SpecializationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments",
                column: "AppointmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "PatientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Doctors",
                table: "Doctors",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Doctors_DoctorId",
                table: "Appointments",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "DoctorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Specializations_SpecializationId",
                table: "Doctors",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "SpecializationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Doctors_DoctorId",
                table: "Users",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Patients_PatientId",
                table: "Users",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId");
        }
    }
}
