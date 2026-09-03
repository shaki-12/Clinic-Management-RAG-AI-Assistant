namespace AiEnabledClinicManagement.Services
{
    public class DocumentLoaderService : IDocumentLoaderService
    {
        public async Task<string> LoadDocumentAsync(string fileName)
        {
            // Get the Documents folder path
            string filePath = Path.Combine("Documents", fileName);

            // Check whether the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file '{fileName}' was not found in the 'Documents' folder.");
            }

            // Read the complete document asynchronously
            string text = await File.ReadAllTextAsync(filePath);
            return text;
        }
    }
}
