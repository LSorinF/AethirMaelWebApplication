using AethirMaelWebApplication.Server.DTOs;
using AethirMaelWebApplication.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AethirMaelWebApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized("Email sau parolă invalidă.");
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            if (result = false)
            {
                // Returnam 400 Bad Request cu mesajul specific (
                return BadRequest(result);
            }

            return Ok(new { message = "Înregistrare reușită! Te poți loga acum." });
        }
    }
}