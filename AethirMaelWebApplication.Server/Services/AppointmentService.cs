using AethirMaelWebApplication.Server.Data;
using AethirMaelWebApplication.Server.DTOs;
using AethirMaelWebApplication.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AethirMaelWebApplication.Server.Services
{
    public class AppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- 1. Programările Doctorului ---
        public async Task<List<object>> GetDoctorAppointmentsAsync(int userId)
        {
            var user = await _context.Users.Include(u => u.Doctor).FirstOrDefaultAsync(u => u.UserId == userId);

            if (user?.DoctorId == null) return new List<object>();

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == user.DoctorId)
                .OrderBy(a => a.AppointmentDate)
                .Select(a => new
                {
                    // Folosim nume explicite pentru a evita confuzia JSON
                    appointmentId = a.AppointmentId,
                    appointmentDate = a.AppointmentDate,
                    patientName = a.Patient.LastName + " " + a.Patient.FirstName,
                    patientId = a.Patient.PatientId,
                    status = a.Status,
                    patientNotes = a.PatientNotes
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

        public async Task<List<string>> GetBusySlotsAsync(int doctorId, DateTime date)
        {
            // Căutăm programările pentru doctor in ziua respectiva
            var busySlots = await _context.Appointments
                .Where(a => a.DoctorId == doctorId
                         && a.AppointmentDate.Date == date.Date // Comparăm doar data, ignorăm ora
                         && a.Status != "Anulată") // Ignorăm programările anulate
                .Select(a => a.AppointmentDate)
                .ToListAsync();

            // Facem formatarea în memorie pentru siguranță
            return busySlots.Select(d => d.ToString("HH:mm", CultureInfo.InvariantCulture)).ToList();
        }

        // --- 3. Creare Programare ---
        public async Task<string> CreateAppointmentAsync(CreateAppointmentDto dto, int userId)
        {
            var user = await _context.Users.Include(u => u.Patient).FirstOrDefaultAsync(u => u.UserId == userId);

            if (user?.PatientId == null) return "Eroare profil pacient.";

            // Validare Trecut
            if (dto.AppointmentDate < DateTime.Now) return "Nu poți face o programare în trecut.";

            // Validare Weekend
            if (dto.AppointmentDate.DayOfWeek == DayOfWeek.Saturday ||
                dto.AppointmentDate.DayOfWeek == DayOfWeek.Sunday)
            {
                return "Clinica este închisă în weekend.";
            }

            // Validare Medic Ocupat
            var isBusy = await _context.Appointments.AnyAsync(a =>
               a.DoctorId == dto.DoctorId &&
               a.AppointmentDate == dto.AppointmentDate &&
               a.Status != "Anulată" 
           );

            if (isBusy) return "Medicul are deja o altă programare la această oră.";

            var appointment = new Appointment
            {
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

        // --- 4. Programările Mele (Pacient) ---
        public async Task<List<object>> GetMyAppointmentsAsync(int userId)
        {
            // 1. Găsim ID-ul pacientului asociat userului logat
            var patientId = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.PatientId)
                .FirstOrDefaultAsync();

            if (patientId == null) return new List<object>();

            // 2. Aducem programările
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new
                {
                    // Nume proprietăți camelCase pentru a fi sigur ca Frontend-ul le vede
                    appointmentId = a.AppointmentId,
                    appointmentDate = a.AppointmentDate,
                    doctorName = "Dr. " + a.Doctor.LastName + " " + a.Doctor.FirstName,
                    status = a.Status,
                    patientNotes = a.PatientNotes
                })
                .ToListAsync();

            return appointments.Cast<object>().ToList();
        }
    }
}