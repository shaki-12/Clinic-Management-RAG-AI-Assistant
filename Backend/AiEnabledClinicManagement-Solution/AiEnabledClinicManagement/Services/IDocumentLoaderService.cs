namespace AiEnabledClinicManagement.Services
{
    public interface IDocumentLoaderService
    {
        Task<string> LoadDocumentAsync(string fileName);
    }
}
