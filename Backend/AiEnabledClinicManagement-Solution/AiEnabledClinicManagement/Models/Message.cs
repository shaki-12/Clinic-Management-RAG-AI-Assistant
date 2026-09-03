using System.Text.Json.Serialization;

namespace AiEnabledClinicManagement.Models
{
    // One message sent up to the chat completion API ("system" or "user").
    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}
