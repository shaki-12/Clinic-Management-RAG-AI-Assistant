using AiEnabledClinicManagement.Models;
using AiEnabledClinicManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AiEnabledClinicManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        // Fields
        private readonly IRagOrchestrationService _ragOrchestrationService;

        // DI
        public AIController(IRagOrchestrationService ragOrchestrationService)
        {
            _ragOrchestrationService = ragOrchestrationService;
        }

        // POST: api/AI/ask
        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskRequest request)
        {
            // Validate input
            if (request == null || string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest(new { message = "Question cannot be empty." });
            }

            try
            {
                AskResponse response = await _ragOrchestrationService.AskAsync(request.Question);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}
