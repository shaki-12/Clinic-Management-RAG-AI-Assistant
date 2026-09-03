using System.Text.Json.Serialization;

namespace AiEnabledClinicManagement.Models
{
    public class Choice
    {
        [JsonPropertyName("message")]
        public AssistantMessage Message { get; set; } = new();
    }
}
