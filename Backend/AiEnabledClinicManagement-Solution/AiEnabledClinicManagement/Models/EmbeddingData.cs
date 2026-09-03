using System.Text.Json.Serialization;

namespace AiEnabledClinicManagement.Models
{
    public class EmbeddingData
    {
        [JsonPropertyName("embedding")]
        public float[] Embedding { get; set; } = [];

        [JsonPropertyName("index")]
        public int Index { get; set; }
    }
}
