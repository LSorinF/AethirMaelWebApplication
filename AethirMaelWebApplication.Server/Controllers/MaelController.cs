using AethirMaelWebApplication.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AethirMaelWebApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Doar utilizatorii logați pot vorbi cu Maël
    public class MaelController : ControllerBase
    {
        private readonly MaelAiService _maelService;

        public MaelController(MaelAiService maelService)
        {
            _maelService = maelService;
        }

        public class ChatRequest
        {
            public string Message { get; set; }
        }

        [HttpPost("ask")]
        public async Task<ActionResult> AskMael([FromBody] ChatRequest request)
        {
            try
            {
                // Apelează metoda din MaelAiService.cs
                var response = await _maelService.AskMaelAsync(request.Message, User);

                // Returnează rezultatul într-un obiect JSON cu proprietatea "response"
                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                return Ok(new { response = $"🤖 Eroare Controller: {ex.Message}" });
            }
        }
    }
}