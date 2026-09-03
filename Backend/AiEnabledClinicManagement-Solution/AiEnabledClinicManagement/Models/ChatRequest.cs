using System.Text.Json.Serialization;

namespace AiEnabledClinicManagement.Models
{
    // Request body sent to POST https://api.openai.com/v1/chat/completions
    public class ChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; } = new();
    }
}
