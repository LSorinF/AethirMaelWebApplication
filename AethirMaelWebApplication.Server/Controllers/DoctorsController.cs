using AethirMaelWebApplication.Server.DTOs;
using AethirMaelWebApplication.Server.Models;
using AethirMaelWebApplication.Server.Services; 
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult> CreateDoctor(CreateDoctorDto dto)
        {
            var result = await _doctorService.CreateDoctorAsync(dto);

            if (result != "Success")
            {
                return BadRequest(result);
            }

            return Ok(new { message = "Medic adăugat cu succes!" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult> DeleteDoctor(int id)
        {
            var result = await _doctorService.DeleteDoctorAsync(id);

            if (result != "Success")
            {
                return BadRequest(result); 
            }

            return Ok(new { message = "Medic șters cu succes!" });
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