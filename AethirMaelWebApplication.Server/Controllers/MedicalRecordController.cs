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

        // POST: Creare
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult> Create(CreateMedicalRecordDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _service.CreateRecordAsync(dto, userId);

            if (result != "Success") return BadRequest(result);
            return Ok(new { message = "Fișa medicală a fost salvată." });
        }

        // GET: Istoricul Meu 
        [HttpGet("my-history")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<List<object>>> GetMyHistory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return Ok(await _service.GetPatientHistoryAsync(userId));
        }

        // GET: Istoric Pacient Specific 
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<List<object>>> GetPatientHistory(int patientId)
        {
            return Ok(await _service.GetHistoryByPatientIdAsync(patientId));
        }

        // PUT: Actualizare
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
 
        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor,Medic")]
        public async Task<ActionResult> Delete(int id)
        {
            // Extragem ID-ul utilizatorului logat din token-ul JWT
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);


            var result = await _service.DeleteRecordAsync(id, userId);

            if (result != "Success")
            {
                return BadRequest(new { message = result });
            }

            return Ok(new { message = "Fișa a fost ștearsă cu succes." });
        }
    }
}