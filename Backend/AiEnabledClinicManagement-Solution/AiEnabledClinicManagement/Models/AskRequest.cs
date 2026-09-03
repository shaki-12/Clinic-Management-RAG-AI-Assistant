namespace AiEnabledClinicManagement.Models
{
    // Request body for POST /api/AI/ask
    public class AskRequest
    {
        public string Question { get; set; } = string.Empty;
    }
}
