using AethirMaelWebApplication.Server.Data;
using AethirMaelWebApplication.Server.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AethirMaelWebApplication.Server.Services
{
    public class AdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly DoctorService _doctorService; 

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
                return await _doctorService.DeleteDoctorAsync(user.Doctor.DoctorId);
            }

            // CAZ 2: Este PACIENT
            if (user.Patient != null)
            {
                // Stergem entitatea Pacient. 
                _context.Patients.Remove(user.Patient);
                await _context.SaveChangesAsync();
                return "Success";
            }

            // CAZ 3: Cont fara detalii 
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return "Success";
        }

        public async Task<AdminDashboardStatsDto> GetDashboardStatsAsync()
        {
            var stats = new AdminDashboardStatsDto();

            // Contoare Generale
            stats.TotalPatients = await _context.Patients.CountAsync();
            stats.TotalDoctors = await _context.Doctors.CountAsync();
            stats.TotalAppointments = await _context.Appointments.CountAsync();

            // Programari pe 6 luni
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);

            var appointmentsData = await _context.Appointments
                .Where(a => a.AppointmentDate >= sixMonthsAgo)
                .GroupBy(a => new { a.AppointmentDate.Year, a.AppointmentDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            stats.AppointmentsPerMonth = appointmentsData.Select(x => new MonthlyStatDto
            {
                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(x.Month) + " " + x.Year,
                Count = x.Count
            }).ToList();

            // Pacienti noi
            var patientsData = await _context.Patients
                .Where(p => p.DateCreated >= sixMonthsAgo)
                .GroupBy(p => new { p.DateCreated.Year, p.DateCreated.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            stats.PatientsPerMonth = patientsData.Select(x => new MonthlyStatDto
            {
                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(x.Month) + " " + x.Year,
                Count = x.Count
            }).ToList();

            return stats;
        }
    }
}