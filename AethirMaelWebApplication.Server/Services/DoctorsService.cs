using AethirMaelWebApplication.Server.Data;
using AethirMaelWebApplication.Server.DTOs;
using AethirMaelWebApplication.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AethirMaelWebApplication.Server.Services
{
    public class DoctorService
    {
        private readonly ApplicationDbContext _context;

        public DoctorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateDoctorAsync(CreateDoctorDto dto)
        {
            // 1. Verificam daca emailul exista deja
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return "Acest email este deja folosit.";

            // 2. Cream entitatea DOCTOR
            var doctor = new Doctor
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                SpecializationId = dto.SpecializationId
            };

            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync(); // Salvam pentru a genera DoctorId

            // 3. Cream entitatea USER (Login)
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password), // Hash-uim parola
                Role = "Doctor", // Setam rolul corect!
                DoctorId = doctor.DoctorId // Legam de doctorul creat mai sus
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return "Success";
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        public async Task<string> DeleteDoctorAsync(int id)
        {
            // 1. Verificam daca exista
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return "Medicul nu a fost găsit.";

            // 2. Verificam daca are programari (nu stergem istoricul medical!)
            bool hasAppointments = await _context.Appointments.AnyAsync(a => a.DoctorId == id);
            if (hasAppointments)
            {
                return "Nu se poate șterge acest medic deoarece are programări asociate. Anulează programările mai întâi.";
            }

            // 3. Gasim userul asociat pentru a-l sterge si pe el
            var user = await _context.Users.FirstOrDefaultAsync(u => u.DoctorId == id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            // 4. Stergem medicul
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return "Success";
        }

        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return await _context.Doctors
                                 .Include(d => d.Specialization) // Vital pentru Frontend!
                                 .ToListAsync();
        }

        public async Task<List<Specialization>> GetAllSpecializationsAsync()
        {
            return await _context.Specializations.ToListAsync();
        }

        public async Task<Doctor?> GetDoctorByIdAsync(int id)
        {
            return await _context.Doctors
                                 .Include(d => d.Specialization)
                                 .FirstOrDefaultAsync(d => d.DoctorId == id);
        }
    }
}