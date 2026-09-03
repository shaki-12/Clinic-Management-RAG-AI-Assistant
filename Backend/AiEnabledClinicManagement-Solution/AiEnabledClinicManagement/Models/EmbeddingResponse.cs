using System.Text.Json.Serialization;

namespace AiEnabledClinicManagement.Models
{
    public class EmbeddingResponse
    {
        [JsonPropertyName("data")]
        public List<EmbeddingData> Data { get; set; } = new();

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;
    }
}
