namespace AiEnabledClinicManagement.Models
{
    public class ChunkSearchResult
    {
        public string Text { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public double Score { get; set; }
    }
}
