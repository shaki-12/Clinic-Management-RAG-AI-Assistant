using AiEnabledClinicManagement.Models;

namespace AiEnabledClinicManagement.Services
{
    public interface IRagOrchestrationService
    {
        // Runs the full RAG pipeline for a single question:
        // embed question -> similarity search -> top-K chunks -> RAG prompt
        // -> LLM -> grounded answer + sources.
        Task<AskResponse> AskAsync(string question);
    }
}
