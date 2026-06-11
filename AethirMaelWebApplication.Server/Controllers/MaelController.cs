using AethirMaelWebApplication.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AethirMaelWebApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Doar utilizatorii logati pot vorbi cu Mael
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
                var response = await _maelService.AskMaelAsync(request.Message, User);

                return Ok(new { response = response });
            }
            catch (Exception ex)
            {
                return Ok(new { response = $"🤖 Eroare Controller: {ex.Message}" });
            }
        }
    }
}