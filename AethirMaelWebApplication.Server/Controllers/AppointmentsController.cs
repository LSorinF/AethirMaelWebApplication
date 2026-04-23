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
    public class AppointmentsController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentsController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("my-appointments")]
        public async Task<ActionResult> GetMyAppointments()
        {
            // ... (Codul vechi ramane la fel)
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return Ok(await _appointmentService.GetMyAppointmentsAsync(userId));
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateAppointmentDto dto)
        {
            // ... (Codul vechi ramane la fel)
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _appointmentService.CreateAppointmentAsync(dto, userId);
            return result == "Success" ? Ok(new { message = "Trimis!" }) : BadRequest(result);
        }

        // --- ENDPOINT-URI NOI PENTRU DOCTORI ---

        [HttpGet("doctor-appointments")]
        [Authorize(Roles = "Doctor")] // Doar doctorii au voie aici!
        public async Task<ActionResult> GetDoctorAppointments()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _appointmentService.GetDoctorAppointmentsAsync(userId);
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Doctor")] // Doar doctorii pot schimba statusul
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var success = await _appointmentService.UpdateStatusAsync(id, dto.Status);
            if (!success) return NotFound();
            return Ok();
        }

        [HttpGet("busy-slots")]
        public async Task<ActionResult<List<string>>> GetBusySlots([FromQuery] int doctorId, [FromQuery] DateTime date)
        {
            // Exemplu apel: /api/Appointments/busy-slots?doctorId=5&date=2025-12-20
            var slots = await _appointmentService.GetBusySlotsAsync(doctorId, date);
            return Ok(slots);
        }
    }
}