using AethirMaelWebApplication.Server.Data;
using AethirMaelWebApplication.Server.Models;
using Microsoft.EntityFrameworkCore;

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