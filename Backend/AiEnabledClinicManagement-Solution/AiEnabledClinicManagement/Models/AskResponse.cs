namespace AiEnabledClinicManagement.Models
{
    // Response body for POST /api/AI/ask
    public class AskResponse
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;

        // Distinct policy document names the answer was grounded in.
        // Empty when the approved policies did not contain the answer.
        public List<string> Sources { get; set; } = new();

        // True when at least one sufficiently relevant policy chunk was
        // found and used to produce the answer.
        public bool IsGrounded { get; set; }
    }
}
