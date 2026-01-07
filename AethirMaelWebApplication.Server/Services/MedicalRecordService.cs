using AethirMaelWebApplication.Server.Data;
using AethirMaelWebApplication.Server.DTOs;
using AethirMaelWebApplication.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AethirMaelWebApplication.Server.Services
{
    public class MedicalRecordService
    {
        private readonly ApplicationDbContext _context;

        public MedicalRecordService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Doctorul creează o fișă
        public async Task<string> CreateRecordAsync(CreateMedicalRecordDto dto, int userId)
        {
            var user = await _context.Users.Include(u => u.Doctor).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user?.Doctor == null) return "Utilizatorul nu este doctor.";

            var record = new MedicalRecord
            {
                PatientId = dto.PatientId,
                DoctorId = user.Doctor.DoctorId,
                Symptoms = dto.Symptoms,
                Diagnosis = dto.Diagnosis,
                Treatment = dto.Treatment,
                InvestigationResults = dto.InvestigationResults,
                AppointmentId = dto.AppointmentId,
                DateCreated = DateTime.Now
            };

            _context.MedicalRecords.Add(record);

            if (dto.AppointmentId.HasValue)
            {
                var app = await _context.Appointments.FindAsync(dto.AppointmentId.Value);
                if (app != null) app.Status = "Finalizată";
            }

            await _context.SaveChangesAsync();
            return "Success";
        }

        public async Task<string> UpdateRecordAsync(UpdateMedicalRecordDto dto, int userId)
        {
            // 1. Identificăm doctorul logat
            var user = await _context.Users.Include(u => u.Doctor).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user?.Doctor == null) return "Utilizatorul nu este doctor.";

            // 2. Găsim fișa medicală
            var record = await _context.MedicalRecords.FindAsync(dto.MedicalRecordId);
            if (record == null) return "Fișa medicală nu a fost găsită.";

            // 3. SECURITATE: Verificăm dacă fișa aparține acestui doctor
            if (record.DoctorId != user.Doctor.DoctorId)
            {
                return "Nu aveți dreptul să modificați fișa creată de alt medic.";
            }

            // 4. Actualizăm datele
            record.Symptoms = dto.Symptoms;
            record.Diagnosis = dto.Diagnosis;
            record.Treatment = dto.Treatment;
            record.InvestigationResults = dto.InvestigationResults;
            record.DateModified = DateTime.Now;

            await _context.SaveChangesAsync();
            return "Success";
        }

        public async Task<List<object>> GetPatientHistoryAsync(int userId)
        {
            var patientId = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.PatientId)
                .FirstOrDefaultAsync();

            if (patientId == null) return new List<object>();

            var records = await _context.MedicalRecords
                .Include(r => r.Doctor).ThenInclude(d => d.Specialization)
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.DateCreated)
                .Select(r => new
                {
                    medicalRecordId = r.MedicalRecordId,
                    dateCreated = r.DateCreated,
                    // MODIFICARE: Verificăm dacă Doctor este null
                    doctorName = r.Doctor != null
                        ? "Dr. " + r.Doctor.LastName + " " + r.Doctor.FirstName
                        : "Medic Șters / Necunoscut",
                    specialization = r.Doctor != null
                        ? r.Doctor.Specialization.Name
                        : "N/A",
                    symptoms = r.Symptoms,
                    diagnosis = r.Diagnosis,
                    treatment = r.Treatment,
                    investigationResults = r.InvestigationResults
                })
                .ToListAsync();

            return records.Cast<object>().ToList();
        }

        public async Task<List<object>> GetHistoryByPatientIdAsync(int patientId)
        {
            var records = await _context.MedicalRecords
               .Include(r => r.Doctor).ThenInclude(d => d.Specialization)
               .Where(r => r.PatientId == patientId)
               .OrderByDescending(r => r.DateCreated)
               .Select(r => new
               {
                   medicalRecordId = r.MedicalRecordId,
                   dateCreated = r.DateCreated,
                   // MODIFICARE: Verificăm dacă Doctor este null
                   doctorName = r.Doctor != null
                       ? "Dr. " + r.Doctor.LastName + " " + r.Doctor.FirstName
                       : "Medic Șters / Necunoscut",
                   specialization = r.Doctor != null
                       ? r.Doctor.Specialization.Name
                       : "N/A",
                   symptoms = r.Symptoms,
                   diagnosis = r.Diagnosis,
                   treatment = r.Treatment,
                   investigationResults = r.InvestigationResults
               })
               .ToListAsync();

            return records.Cast<object>().ToList();
        }

        public async Task<bool> DeleteRecordAsync(int recordId)
        {
            var record = await _context.MedicalRecords.FindAsync(recordId);
            if (record == null) return false;

            _context.MedicalRecords.Remove(record);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}