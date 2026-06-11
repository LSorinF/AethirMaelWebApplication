using AethirMaelWebApplication.Server.DTOs;
using AethirMaelWebApplication.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AethirMaelWebApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] 
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<ActionResult> GetAllUsers()
        {
            return Ok(await _adminService.GetAllUsersAsync());
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var result = await _adminService.DeleteUserAsync(id);

            if (result != "Success")
            {
                return BadRequest(result);
            }

            return Ok(new { message = "Utilizator și datele asociate șterse cu succes." });
        }

        [HttpGet("stats")]
        public async Task<ActionResult<AdminDashboardStatsDto>> GetStats()
        {
            return Ok(await _adminService.GetDashboardStatsAsync());
        }
    }
}