using AiEnabledClinicManagement.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace AiEnabledClinicManagement.Services
{
    public class OpenAIEmbeddingService : IEmbeddingService
    {
        // Fields
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        // DI
        public OpenAIEmbeddingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            // 1. Read configuration
            string apiKey = _configuration["OpenAI:ApiKey"]!;
            string endpoint = _configuration["OpenAI:EmbeddingEndpoint"]!;
            string model = _configuration["OpenAI:EmbeddingModel"]!;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "OpenAI:ApiKey is not configured. Set it in appsettings.Development.json.");
            }

            // 2. Configure authorization
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            // 3. Create the embedding request
            EmbeddingRequest request = new EmbeddingRequest
            {
                Model = model,
                Input = text
            };

            // 4. Convert C# object -> JSON
            string jsonRequest = JsonSerializer.Serialize(request);

            // 5. Create HTTP request body
            StringContent httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // 6. Send request to OpenAI
            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, httpContent);

            // 7. Read response
            string jsonResponse = await response.Content.ReadAsStringAsync();

            // 8. Handle API errors
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"OpenAI Embedding API Error. HTTP Status: {(int)response.StatusCode} " +
                    $"Reason: {response.ReasonPhrase} Details: {jsonResponse}");
            }

            // 9. JSON -> C# object
            EmbeddingResponse? embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(jsonResponse);

            // 10. Validate response
            if (embeddingResponse == null || embeddingResponse.Data == null || embeddingResponse.Data.Count == 0)
            {
                throw new InvalidOperationException("No embedding data received from OpenAI.");
            }

            // 11. Return the embedding vector
            return embeddingResponse.Data[0].Embedding;
        }
    }
}
