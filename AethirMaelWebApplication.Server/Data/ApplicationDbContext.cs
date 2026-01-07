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

            // --- REGULI DE ȘTERGERE (CRITIC) ---

            // 1. Ștergere Doctor -> Șterge Programări (CASCADE)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. Ștergere Doctor -> Păstrează Fișele, dar pune DoctorId pe NULL (SET NULL)
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(r => r.Doctor)
                .WithMany()
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.SetNull); // <--- Aici e schimbarea majoră

            // 3. Ștergere Pacient -> Șterge Programări (CASCADE)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // 4. Ștergere Pacient -> Șterge Fișe Medicale (CASCADE)
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(r => r.Patient)
                .WithMany() // (Asumând că nu avem colecție în Patient, e ok)
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurările unice existente...
            modelBuilder.Entity<Patient>().HasIndex(p => p.CNP).IsUnique();
            modelBuilder.Entity<Patient>().HasIndex(p => p.Email).IsUnique();
            modelBuilder.Entity<Doctor>().HasIndex(d => d.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        }
    }
}