using System.Text.Json.Serialization;

namespace AiEnabledClinicManagement.Models
{
    // The assistant's reply message, as returned inside a Choice.
    public class AssistantMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}
