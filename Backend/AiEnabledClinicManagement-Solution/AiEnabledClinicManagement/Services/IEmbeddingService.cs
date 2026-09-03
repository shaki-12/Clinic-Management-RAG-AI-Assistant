namespace AiEnabledClinicManagement.Services
{
    public interface IEmbeddingService
    {
        Task<float[]> GetEmbeddingAsync(string text);
 
    }
}
