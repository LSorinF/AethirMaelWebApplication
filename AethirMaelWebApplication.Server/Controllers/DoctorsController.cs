using AethirMaelWebApplication.Server.Models;
using AethirMaelWebApplication.Server.Services; // Importam Serviciile
using Microsoft.AspNetCore.Mvc;

namespace AethirMaelWebApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly DoctorService _doctorService;
        
        public DoctorsController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Doctor>>> GetDoctors()
        {
            try
            {
                var doctors = await _doctorService.GetAllDoctorsAsync();
                return Ok(doctors);
            }
            catch (Exception)
            {
                // Putem loga eroarea aici
                return StatusCode(500, "A aparut o eroare interna.");
            }
        }

        [HttpGet("Specializations")]
        public async Task<ActionResult<IEnumerable<Specialization>>> GetSpecializations()
        {
            var specializations = await _doctorService.GetAllSpecializationsAsync();
            return Ok(specializations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);

            if (doctor == null)
            {
                return NotFound();
            }

            return Ok(doctor);
        }
    }
}