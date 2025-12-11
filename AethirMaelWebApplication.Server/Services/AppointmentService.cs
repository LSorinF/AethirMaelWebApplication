using AethirMaelWebApplication.Server.Data;
using AethirMaelWebApplication.Server.DTOs;
using AethirMaelWebApplication.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AethirMaelWebApplication.Server.Services
{
    public class AppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. Programările Doctorului (Folosim DOCTOR / DoctorId) ---
        public async Task<List<object>> GetDoctorAppointmentsAsync(int userId)
        {
            // Folosim Doctor
            var user = await _context.Users.Include(u => u.Doctor).FirstOrDefaultAsync(u => u.UserId == userId);

            // Verificam DoctorId
            if (user?.DoctorId == null) return new List<object>();

            // Pasul 1: Aducem datele din DB (Async)
            var appointments = await _context.Appointments
                .Include(a => a.Patient) // Folosim Patient
                .Where(a => a.DoctorId == user.DoctorId) // Folosim DoctorId
                .OrderBy(a => a.AppointmentDate)
                .Select(a => new
                {
                    a.AppointmentId,
                    a.AppointmentDate,
                    // Folosim Patient pentru nume
                    PatientName = a.Patient.LastName + " " + a.Patient.FirstName,
                    a.Status,
                    a.PatientNotes
                })
                .ToListAsync();

            return appointments.Cast<object>().ToList();
        }

        // --- 2. Actualizare Status ---
        public async Task<bool> UpdateStatusAsync(int appointmentId, string newStatus)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null) return false;

            appointment.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        // --- 3. Creare Programare ---
        public async Task<string> CreateAppointmentAsync(CreateAppointmentDto dto, int userId)
        {
            // Folosim Patient
            var user = await _context.Users.Include(u => u.Patient).FirstOrDefaultAsync(u => u.UserId == userId);

            // Folosim PatientId
            if (user?.PatientId == null) return "Eroare profil Patient.";

            if (dto.AppointmentDate.DayOfWeek == DayOfWeek.Saturday ||
                dto.AppointmentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return "Clinica este închisă în weekend.";
            }

            // Folosim DoctorId
            var isBusy = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == dto.DoctorId &&
                a.AppointmentDate == dto.AppointmentDate
            );

            if (isBusy) return "Medicul are deja o altă programare la această oră.";

            var appointment = new Appointment
            {
                // Setam DoctorId si PatientId
                DoctorId = dto.DoctorId,
                PatientId = user.PatientId.Value,
                AppointmentDate = dto.AppointmentDate,
                PatientNotes = dto.PatientNotes,
                Status = "Solicitata"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return "Success";
        }

        // --- 4. Programările Mele (Patient) ---
        public async Task<List<object>> GetMyAppointmentsAsync(int userId)
        {
            // Extragem PatientId
            var PatientId = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.PatientId)
                .FirstOrDefaultAsync();

            if (PatientId == null) return new List<object>();

            var appointments = await _context.Appointments
                .Include(a => a.Doctor) // Folosim Doctor
                .Where(a => a.PatientId == PatientId) // Folosim PatientId
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new
                {
                    a.AppointmentId,
                    a.AppointmentDate,
                    // Folosim Doctor pentru nume
                    DoctorName = "Dr. " + a.Doctor.LastName + " " + a.Doctor.FirstName,
                    a.Status,
                    a.PatientNotes
                })
                .ToListAsync();

            return appointments.Cast<object>().ToList();
        }
    }
}