using AiEnabledClinicManagement.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace AiEnabledClinicManagement.Services
{
    public class OpenAIChatCompletionService : IChatCompletionService
    {
        // Fields
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        // DI
        public OpenAIChatCompletionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetCompletionAsync(string systemPrompt, string userPrompt)
        {
            // 1. Read configuration
            string apiKey = _configuration["OpenAI:ApiKey"]!;
            string endpoint = _configuration["OpenAI:ChatEndpoint"]!;
            string model = _configuration["OpenAI:ChatModel"]!;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "OpenAI:ApiKey is not configured. Set it in appsettings.Development.json.");
            }

            // 2. Configure authorization
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            // 3. Create the chat completion request
            ChatRequest request = new ChatRequest
            {
                Model = model,
                Messages = new List<Message>
                {
                    new Message { Role = "system", Content = systemPrompt },
                    new Message { Role = "user", Content = userPrompt }
                }
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
                    $"OpenAI Chat Completion API Error. HTTP Status: {(int)response.StatusCode} " +
                    $"Reason: {response.ReasonPhrase} Details: {jsonResponse}");
            }

            // 9. JSON -> C# object
            ChatResponse? chatResponse = JsonSerializer.Deserialize<ChatResponse>(jsonResponse);

            // 10. Validate response
            if (chatResponse == null || chatResponse.Choices == null || chatResponse.Choices.Count == 0)
            {
                throw new InvalidOperationException("No response received from the AI model.");
            }

            // 11. Return the assistant's answer
            return chatResponse.Choices[0].Message.Content;
        }
    }
}