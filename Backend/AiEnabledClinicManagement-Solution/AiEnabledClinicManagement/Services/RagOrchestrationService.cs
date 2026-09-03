using AiEnabledClinicManagement.Models;

namespace AiEnabledClinicManagement.Services
{
    public class RagOrchestrationService : IRagOrchestrationService
    {
        // Fixed grounded response used whenever no approved policy chunk is
        // relevant enough to answer the question - the LLM is never called
        // in this case, so it cannot invent an answer.
        private const string OutOfPolicyAnswer =
            "The approved clinical policies do not contain information to answer this question.";

        // Fields
        private readonly IKnowledgeBaseService _knowledgeBase;
        private readonly IChatCompletionService _chatCompletion;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RagOrchestrationService> _logger;

        // DI
        public RagOrchestrationService(
            IKnowledgeBaseService knowledgeBase,
            IChatCompletionService chatCompletion,
            IConfiguration configuration,
            ILogger<RagOrchestrationService> logger)
        {
            _knowledgeBase = knowledgeBase;
            _chatCompletion = chatCompletion;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AskResponse> AskAsync(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                throw new ArgumentException("Question cannot be empty.", nameof(question));
            }

            int topK = _configuration.GetValue<int>("KnowledgeBase:DefaultTopK", 3);
            double minimumRelevanceScore = _configuration.GetValue<double>("KnowledgeBase:MinimumRelevanceScore", 0.50);

            // 1. Question Embedding + 2. Similarity Search + 3. Top-K Retrieved Chunks
            // (this reuses the already-built Knowledge Base - no embeddings are recreated here)
            List<ChunkSearchResult> retrievedChunks = await _knowledgeBase.SearchAsync(question, topK);

            // 4. Keep only chunks that are actually relevant to the question
            List<ChunkSearchResult> relevantChunks = retrievedChunks
                .Where(chunk => chunk.Score >= minimumRelevanceScore)
                .ToList();

            // Nothing relevant retrieved - return a grounded refusal without calling the LLM
            if (relevantChunks.Count == 0)
            {
                _logger.LogInformation("No relevant policy chunks found for question: {Question}", question);

                return new AskResponse
                {
                    Question = question,
                    Answer = OutOfPolicyAnswer,
                    Sources = new List<string>(),
                    IsGrounded = false
                };
            }

            // 5. RAG Context: build it from the relevant chunks only
            string context = string.Join(
                Environment.NewLine,
                relevantChunks.Select(chunk => $"- ({chunk.Source}) {chunk.Text}"));

            // 6. RAG Prompt
            string systemPrompt = BuildGroundingSystemPrompt();

            string userPrompt =
                $"""
                Approved Policy Context:
                {context}

                Question:
                {question}
                """;

            // 7. LLM
            string answer = await _chatCompletion.GetCompletionAsync(systemPrompt, userPrompt);

            // 8. Source(s): distinct documents the relevant chunks came from
            List<string> sources = relevantChunks
                .Select(chunk => chunk.Source)
                .Distinct()
                .ToList();

            // Grounded Answer + Source(s)
            return new AskResponse
            {
                Question = question,
                Answer = answer,
                Sources = sources,
                IsGrounded = true
            };
        }

        private static string BuildGroundingSystemPrompt()
        {
            return """
                You are a clinical policy assistant for a healthcare clinic.

                Answer the user's question using ONLY the information contained in
                the "Approved Policy Context" section of the user's message.

                Rules you must always follow:
                - Use only the supplied policy context. Do not use outside knowledge.
                - Do not invent, guess, or assume any policy information that is not
                  explicitly stated in the supplied context.
                - Do not provide medical advice, diagnoses, treatment recommendations,
                  or medication suggestions of any kind.
                - If the supplied context does not contain the information needed to
                  answer the question, respond with exactly:
                  "The approved clinical policies do not contain information to answer this question."
                - Keep answers short, clear, and written in plain text suitable for a
                  patient-facing application. Do not use Markdown formatting.
                """;
        }
    }
}
