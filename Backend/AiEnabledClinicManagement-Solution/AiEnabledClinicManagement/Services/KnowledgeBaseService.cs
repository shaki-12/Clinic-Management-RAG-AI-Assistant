using AiEnabledClinicManagement.Models;

namespace AiEnabledClinicManagement.Services
{

    public class KnowledgeBaseService : IKnowledgeBaseService
    {
        // Names of the approved policy documents that make up the Knowledge Base.
        private static readonly string[] PolicyFileNames =
        {
            "AppointmentPolicy.txt",
            "HealthInsurancePolicy.txt"
        };

        // Fields
        private readonly IDocumentLoaderService _documentLoader;
        private readonly IChunkingService _chunkingService;
        private readonly IEmbeddingService _embeddingService;
        private readonly ILogger<KnowledgeBaseService> _logger;

        // In-memory Knowledge Base: every chunk from every approved policy
        // document, together with its embedding vector. Populated once by
        // InitializeAsync() and then only ever read from.
        private readonly List<EmbeddingDocument> _knowledgeBase = new();

        // Guards InitializeAsync so the embedding calls only ever run once,
        // even if multiple requests happen to race against it.
        private readonly SemaphoreSlim _initLock = new(1, 1);

        public bool IsInitialized { get; private set; }

        public int TotalChunks => _knowledgeBase.Count;

        // DI
        public KnowledgeBaseService(
            IDocumentLoaderService documentLoader,
            IChunkingService chunkingService,
            IEmbeddingService embeddingService,
            ILogger<KnowledgeBaseService> logger)
        {
            _documentLoader = documentLoader;
            _chunkingService = chunkingService;
            _embeddingService = embeddingService;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            // Already built - do not recreate the embeddings.
            if (IsInitialized)
            {
                return;
            }

            await _initLock.WaitAsync();
            try
            {
                // Re-check after acquiring the lock in case another caller
                // already finished initialization while we were waiting.
                if (IsInitialized)
                {
                    return;
                }

                _logger.LogInformation("Initializing Knowledge Base...");

                foreach (string fileName in PolicyFileNames)
                {
                    // 1. Load the raw document text
                    string text = await _documentLoader.LoadDocumentAsync(fileName);

                    // 2. Split it into meaningful policy chunks
                    List<string> chunks = _chunkingService.CreateSemanticChunks(text);

                    // 3. Embed every chunk once and store it in memory
                    foreach (string chunk in chunks)
                    {
                        float[] embedding = await _embeddingService.GetEmbeddingAsync(chunk);

                        _knowledgeBase.Add(new EmbeddingDocument
                        {
                            Id = Guid.NewGuid().ToString(),
                            Text = chunk,
                            Source = fileName,
                            Embedding = embedding
                        });
                    }

                    _logger.LogInformation("Loaded {ChunkCount} chunks from {FileName}", chunks.Count, fileName);
                }

                IsInitialized = true;
                _logger.LogInformation("Knowledge Base ready with {TotalChunks} chunks in total.", _knowledgeBase.Count);
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<List<ChunkSearchResult>> SearchAsync(string question, int topK)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                throw new ArgumentException("Question cannot be empty.", nameof(question));
            }

            // Defensive: make sure the Knowledge Base has been built. In
            // normal operation this already happened once at application
            // startup, so this returns immediately without doing any work.
            if (!IsInitialized)
            {
                await InitializeAsync();
            }

            if (topK <= 0)
            {
                topK = 3;
            }

            // Embed the incoming question - this is the only embedding call
            // made per question. The policy chunk embeddings are reused as-is.
            float[] queryEmbedding = await _embeddingService.GetEmbeddingAsync(question);

            // Score every stored chunk against the question using cosine similarity
            var scored = _knowledgeBase
                .Select(doc => new ChunkSearchResult
                {
                    Text = doc.Text,
                    Source = doc.Source,
                    Score = CosineSimilarity(queryEmbedding, doc.Embedding)
                })
                .OrderByDescending(result => result.Score)
                .Take(Math.Min(topK, _knowledgeBase.Count))
                .ToList();

            return scored;
        }

        // Cosine similarity = (A . B) / (||A|| * ||B||)
        // Returns a value between -1 and 1 - closer to 1 means more similar.
        private static double CosineSimilarity(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length || vectorA.Length == 0)
            {
                return 0d;
            }

            double dotProduct = 0d;
            double magnitudeA = 0d;
            double magnitudeB = 0d;

            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                magnitudeA += vectorA[i] * vectorA[i];
                magnitudeB += vectorB[i] * vectorB[i];
            }

            if (magnitudeA == 0d || magnitudeB == 0d)
            {
                return 0d;
            }

            return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
        }
    }
}
