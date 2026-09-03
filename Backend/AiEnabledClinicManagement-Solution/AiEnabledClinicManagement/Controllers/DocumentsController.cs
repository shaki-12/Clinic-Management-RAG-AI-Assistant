using AiEnabledClinicManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AiEnabledClinicManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        // Fields
        private readonly IDocumentLoaderService _documentLoader;
        private readonly IChunkingService _chunkingService;
        private readonly IKnowledgeBaseService _knowledgeBase;

        // Dependency Injection
        public DocumentsController(
            IDocumentLoaderService documentLoaderService,
            IChunkingService chunkingService,
            IKnowledgeBaseService knowledgeBaseService)
        {
            _documentLoader = documentLoaderService;
            _chunkingService = chunkingService;
            _knowledgeBase = knowledgeBaseService;
        }

        // GET: api/Documents/chunks/{fileName}
        // Loads a policy document straight off disk and chunks it, so the
        // chunking logic can be verified independently of embeddings.
        [HttpGet("chunks/{fileName}")]
        public async Task<IActionResult> GetChunks(string fileName)
        {
            try
            {
                // Load the document
                string text = await _documentLoader.LoadDocumentAsync(fileName);

                // Create semantic chunks
                List<string> chunks = _chunkingService.CreateSemanticChunks(text);

                // Return the chunks as a JSON response
                return Ok(new
                {
                    document = fileName,
                    totalChunks = chunks.Count,
                    chunks
                });
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        // GET: api/Documents/search?question=...&topK=3
        // Embeds the question and retrieves the topK most similar chunks
        // from the already-initialized, in-memory Knowledge Base.
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string question, [FromQuery] int topK = 3)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(question))
            {
                return BadRequest(new { message = "Question cannot be empty." });
            }

            try
            {
                var results = await _knowledgeBase.SearchAsync(question, topK);

                return Ok(new
                {
                    question,
                    topK,
                    results = results.Select(r => new
                    {
                        text = r.Text,
                        source = r.Source,
                        score = Math.Round(r.Score, 4)
                    })
                });
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
