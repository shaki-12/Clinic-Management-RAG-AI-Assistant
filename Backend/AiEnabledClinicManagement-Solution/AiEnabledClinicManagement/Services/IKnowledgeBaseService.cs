using AiEnabledClinicManagement.Models;

namespace AiEnabledClinicManagement.Services
{
    public interface IKnowledgeBaseService
    {
        // True once the policy documents have been loaded, chunked,
        // embedded, and stored in memory.
        bool IsInitialized { get; }

        // Total number of chunks currently stored in the Knowledge Base.
        int TotalChunks { get; }

        // Loads, chunks, and embeds all approved policy documents.
        // Safe to call more than once - the embeddings are only generated
        // the first time this actually runs.
        Task InitializeAsync();

        // Embeds the question and returns the topK most similar
        // policy chunks using cosine similarity against the stored embeddings.
        Task<List<ChunkSearchResult>> SearchAsync(string question, int topK);
    }
}
