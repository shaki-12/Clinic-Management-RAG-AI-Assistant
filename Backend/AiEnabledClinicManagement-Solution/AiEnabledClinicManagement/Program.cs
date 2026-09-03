using AiEnabledClinicManagement.Services;

namespace AiEnabledClinicManagement
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Simple, stateless helper services - safe to share as singletons.
            builder.Services.AddSingleton<IDocumentLoaderService, DocumentLoaderService>();
            builder.Services.AddSingleton<IChunkingService, ChunkingService>();

            // Typed HttpClient that talks to the OpenAI Embeddings endpoint.
            builder.Services.AddHttpClient<IEmbeddingService, OpenAIEmbeddingService>();

            // The Knowledge Base holds the in-memory chunk + embedding store,
            // so it must be a singleton: it is built once at startup and then
            // reused for every /search and /ask request afterwards.
            builder.Services.AddSingleton<IKnowledgeBaseService, KnowledgeBaseService>();

            // Typed HttpClient that talks to the OpenAI Chat Completions endpoint.
            builder.Services.AddHttpClient<IChatCompletionService, OpenAIChatCompletionService>();

            // Orchestrates the RAG pipeline for a single question.
            builder.Services.AddScoped<IRagOrchestrationService, RagOrchestrationService>();

            builder.Services.AddControllers();

            // Allow the Angular development server to call this API.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Enable CORS before authorization/controllers.
            app.UseCors("AllowAngular");

            app.UseAuthorization();

            app.MapControllers();

            // Build the Knowledge Base once, during application startup.
            var knowledgeBase = app.Services.GetRequiredService<IKnowledgeBaseService>();

            try
            {
                await knowledgeBase.InitializeAsync();
            }
            catch (Exception ex)
            {
                app.Logger.LogError(
                    ex,
                    "Failed to initialize the Knowledge Base at startup. " +
                    "Check that OpenAI:ApiKey is set in appsettings.Development.json.");
            }

            app.Run();
        }
    }
}