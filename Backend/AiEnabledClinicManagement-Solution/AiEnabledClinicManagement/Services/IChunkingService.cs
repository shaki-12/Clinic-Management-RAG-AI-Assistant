namespace AiEnabledClinicManagement.Services
{
    public interface IChunkingService
    {
        List<string> CreateSemanticChunks(string text);

    }
}
