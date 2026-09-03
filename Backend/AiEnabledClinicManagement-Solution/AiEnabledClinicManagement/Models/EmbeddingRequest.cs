using System.Text.Json.Serialization;

namespace AiEnabledClinicManagement.Models
{
    public class EmbeddingRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("input")]
        public string Input { get; set; } = string.Empty;
    }
}
