namespace AiEnabledClinicManagement.Services
{
    public interface IChatCompletionService
    {
        // Calls the LLM with a system prompt (grounding rules) and a user
        // prompt (RAG context + question) and returns the generated answer.
        Task<string> GetCompletionAsync(string systemPrompt, string userPrompt);
    }
}
