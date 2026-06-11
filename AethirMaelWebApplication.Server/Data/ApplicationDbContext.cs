using Microsoft.EntityFrameworkCore;
using AethirMaelWebApplication.Server.Models;

namespace AethirMaelWebApplication.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>().ToTable("Medici");
            modelBuilder.Entity<Patient>().ToTable("Pacienti");
            modelBuilder.Entity<Specialization>().ToTable("Specializari");
            modelBuilder.Entity<User>().ToTable("Utilizatori");
            modelBuilder.Entity<Appointment>().ToTable("Programari");
            modelBuilder.Entity<MedicalRecord>().ToTable("FiseMedicale");

            // Reguli de stergere 

            // Stergem doctor, stergem programarile 
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Stergerea unui doctor NU sterge fisele medicale
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(r => r.Doctor)
                .WithMany()
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Stergem pacient, stergem programarile
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Stergem pacient, stergem fisele medicale
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(r => r.Patient)
                .WithMany() 
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Patient>().HasIndex(p => p.CNP).IsUnique();
            modelBuilder.Entity<Patient>().HasIndex(p => p.Email).IsUnique();
            modelBuilder.Entity<Doctor>().HasIndex(d => d.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        }
    }
}