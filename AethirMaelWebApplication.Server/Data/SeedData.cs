using AethirMaelWebApplication.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AethirMaelWebApplication.Server.Data
{
    public static class SeedData
    {
        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        public static async Task EnsurePopulated(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await context.Database.MigrateAsync();

                if (!await context.Specializations.AnyAsync())
                {
                    var specializations = new List<Specialization>
                    {
                        new Specialization { Name = "Cardiology" },
                        new Specialization { Name = "Dermatology" },
                        new Specialization { Name = "Neurology" },
                        new Specialization { Name = "Pediatrics" },
                        new Specialization { Name = "Oncology" }
                    };
                    await context.Specializations.AddRangeAsync(specializations);
                    await context.SaveChangesAsync();
                }

                //DOCTORI & ADMIN
                if (!await context.Doctors.AnyAsync())
                {
                    var cardiology = await context.Specializations.FirstOrDefaultAsync(s => s.Name == "Cardiology");
                    var neurology = await context.Specializations.FirstOrDefaultAsync(s => s.Name == "Neurology");

                    var doctors = new List<Doctor>
                    {
                        new Doctor
                        {
                            FirstName = "John",
                            LastName = "Doe",
                            Email = "johndoe@clinic.com",
                            Phone = "0700111222",
                            SpecializationId = cardiology!.SpecializationId
                        },
                        new Doctor
                        {
                            FirstName = "Jane",
                            LastName = "Smith",
                            Email = "janesmith@clinic.com",
                            Phone = "0700333444",
                            SpecializationId = neurology!.SpecializationId
                        }
                    };
                    await context.Doctors.AddRangeAsync(doctors);
                    await context.SaveChangesAsync();

                    var adminHash = HashPassword("adminpass");
                    var doctor1 = doctors.FirstOrDefault(d => d.Email == "johndoe@clinic.com");

                    var users = new List<User>
                    {
                        new User { Email = "admin@hospital.com", PasswordHash = adminHash, Role = "Admin" }, 
                        new User { Email = doctor1!.Email, PasswordHash = HashPassword("doctorpass"), Role = "Doctor", DoctorId = doctor1.DoctorId }
                    };
                    await context.Users.AddRangeAsync(users);
                    await context.SaveChangesAsync();
                }


                if (!await context.Patients.AnyAsync())
                {
                    var Patient = new Patient
                    {
                        FirstName = "Mike",
                        LastName = "Wazowski",
                        CNP = "1234567890123",
                        DateOfBirth = new DateTime(1985, 5, 15),
                        Email = "mike@test.com",
                        Phone = "0700555666"
                    };
                    await context.Patients.AddAsync(Patient);
                    await context.SaveChangesAsync();

                    var patientUser = new User
                    {
                        Email = Patient.Email,
                        PasswordHash = HashPassword("Patientpass"),
                        Role = "Patient",
                        PatientId = Patient.PatientId
                    };
                    await context.Users.AddAsync(patientUser);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}