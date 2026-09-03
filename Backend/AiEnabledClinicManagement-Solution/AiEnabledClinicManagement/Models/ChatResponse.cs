using System.Text.Json.Serialization;

namespace AiEnabledClinicManagement.Models
{
    // Top-level response returned by POST https://api.openai.com/v1/chat/completions
    public class ChatResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; } = new();
    }
}
