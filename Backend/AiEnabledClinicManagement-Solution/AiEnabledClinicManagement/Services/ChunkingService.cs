namespace AiEnabledClinicManagement.Services
{
    public class ChunkingService : IChunkingService
    {
        public List<string> CreateSemanticChunks(string text)
        {
            // Split the policy document into individual lines. Each policy
            // document is written with one complete policy statement per
            // line, so splitting on line breaks gives meaningful,
            // self-contained chunks rather than arbitrary character slices.
            var chunks = text.Split(
                new[] { "\r\n", "\n" },
                StringSplitOptions.RemoveEmptyEntries);

            // Clean each chunk and drop anything empty/whitespace-only
            return chunks
                .Select(chunk => chunk.Trim())
                .Where(chunk => !string.IsNullOrWhiteSpace(chunk))
                .ToList();
        }
    }
}
