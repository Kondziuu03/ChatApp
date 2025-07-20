using ChatApp.Core.Domain.Models;

namespace ChatApp.Core.Domain.Interfaces.Services
{
    public interface IRagService
    {
        Task StoreDocumentChunksAsync(List<DocumentChunk> chunks);
        Task<string> QueryAsync(string request);
    }
}
