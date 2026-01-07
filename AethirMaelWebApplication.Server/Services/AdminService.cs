using AethirMaelWebApplication.Server.Data;
using AethirMaelWebApplication.Server.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AethirMaelWebApplication.Server.Services
{
    public class AdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly DoctorService _doctorService; // Refolosim logica de stergere medic

        public AdminService(ApplicationDbContext context, DoctorService doctorService)
        {
            _context = context;
            _doctorService = doctorService;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            // Selectam toti userii si facem Join cu tabelele de detalii
            return await _context.Users
                .Include(u => u.Patient)
                .Include(u => u.Doctor)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Email = u.Email,
                    Role = u.Role,
                    // Determinam numele in functie de rol
                    FullName = u.Patient != null ? u.Patient.LastName + " " + u.Patient.FirstName :
                               u.Doctor != null ? "Dr. " + u.Doctor.LastName + " " + u.Doctor.FirstName :
                               "Administrator"
                })
                .ToListAsync();
        }

        public async Task<string> DeleteUserAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Patient)
                .Include(u => u.Doctor)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return "Utilizatorul nu există.";
            if (user.Role == "Admin") return "Nu poți șterge un cont de Administrator.";

            // CAZ 1: Este DOCTOR
            if (user.Doctor != null)
            {
                // Apelam logica speciala din DoctorService (care face unlink la fise)
                // Nota: User-ul se sterge automat in acea metoda
                return await _doctorService.DeleteDoctorAsync(user.Doctor.DoctorId);
            }

            // CAZ 2: Este PACIENT
            if (user.Patient != null)
            {
                // Stergem entitatea Pacient. 
                // DB Cascade va sterge automat: User-ul, Programarile si Fisele Medicale.
                _context.Patients.Remove(user.Patient);
                await _context.SaveChangesAsync();
                return "Success";
            }

            // CAZ 3: User orfan (fara profil)
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return "Success";
        }
    }
}