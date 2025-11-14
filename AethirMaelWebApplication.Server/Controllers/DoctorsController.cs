using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using AethirMaelWebApplication.Server.Data;
using AethirMaelWebApplication.Server.Models;



namespace AethirMaelWebApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        public readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Doctor>>> GetDoctors()
        {
            // Interogarea bazei de date si includerea Specializarii (JOIN implicit)
            // pentru a putea afisa si numele specializarii in frontend
            try
            {
                var response = await _context.Doctors
                                .ToListAsync();
                return response;
            }
            catch (Exception e)
            {
                var exception = e;
                return null;
            }
           
        }

        [HttpGet("Specializations")]
        public async Task<ActionResult<IEnumerable<Specialization>>> GetSpecializations()
        {
            return await _context.Specializations.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors
                                       .Include(d => d.Specialization)
                                       .FirstOrDefaultAsync(d => d.DoctorId == id);

            if (doctor == null)
            {
                return NotFound();
            }

            return doctor;
        }
    }
}
