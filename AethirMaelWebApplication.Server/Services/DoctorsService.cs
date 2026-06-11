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

        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return await _context.Doctors.Include(d => d.Specialization).ToListAsync();
        }

        public async Task<List<Specialization>> GetAllSpecializationsAsync()
        {
            return await _context.Specializations.ToListAsync();
        }

        public async Task<Doctor?> GetDoctorByIdAsync(int id)
        {
            return await _context.Doctors.Include(d => d.Specialization).FirstOrDefaultAsync(d => d.DoctorId == id);
        }

        public async Task<string> CreateDoctorAsync(CreateDoctorDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email)) return "Acest email este deja folosit.";

            var doctor = new Doctor
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                SpecializationId = dto.SpecializationId
            };

            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Role = "Doctor",
                DoctorId = doctor.DoctorId 
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return "Success";
        }

        public async Task<string> DeleteDoctorAsync(int id)
        {
            // Verificam daca exista medicul
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return "Medicul nu a fost găsit.";

            // Gasim ID-urile programarilor
            var doctorAppointmentIds = await _context.Appointments
                .Where(a => a.DoctorId == id)
                .Select(a => a.AppointmentId)
                .ToListAsync();

            // Gasim fisele medicale
            var recordsToUnlink = await _context.MedicalRecords
                .Where(r => r.AppointmentId.HasValue && doctorAppointmentIds.Contains(r.AppointmentId.Value))
                .ToListAsync();

            foreach (var record in recordsToUnlink)
            {
                record.AppointmentId = null;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.DoctorId == id);

            if (user != null)
            {
                _context.Users.Remove(user);
            }

            // Stergem medicul
            _context.Doctors.Remove(doctor);

            try
            {
                await _context.SaveChangesAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                return $"Eroare la ștergere: {ex.InnerException?.Message ?? ex.Message}";
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}