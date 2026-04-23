using AethirMaelWebApplication.Server.DTOs;
using AethirMaelWebApplication.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AethirMaelWebApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicalRecordsController : ControllerBase
    {
        private readonly MedicalRecordService _service;

        public MedicalRecordsController(MedicalRecordService service)
        {
            _service = service;
        }

        // 1. POST: Creare
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> Create(CreateMedicalRecordDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _service.CreateRecordAsync(dto, userId);

            if (result != "Success") return BadRequest(result);
            return Ok(new { message = "Fișa medicală a fost salvată." });
        }

        // 2. GET: Istoricul Meu (Pacient)
        [HttpGet("my-history")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<List<object>>> GetMyHistory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return Ok(await _service.GetPatientHistoryAsync(userId));
        }

        // 3. GET: Istoric Pacient Specific (Doctor)
        [HttpGet("patient/{paientId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<List<object>>> GetPatientHistory(int patientId)
        {
            return Ok(await _service.GetHistoryByPatientIdAsync(patientId));
        }

        // 4. PUT: Actualizare
        [HttpPut]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> Update([FromBody] UpdateMedicalRecordDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _service.UpdateRecordAsync(dto, userId);

            if (result != "Success")
            {
                return BadRequest(result);
            }

            return Ok(new { message = "Fișa medicală a fost actualizată." });
        }

        // 5. DELETE: Ștergere (Aici era problema, lipsea!)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _service.DeleteRecordAsync(id);

            if (!success)
            {
                return NotFound("Fișa nu a fost găsită.");
            }

            return Ok(new { message = "Fișa a fost ștearsă." });
        }
    }
}