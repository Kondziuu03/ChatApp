using ChatApp.Core.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace ChatApp.Core.Domain.Interfaces.Services
{
    public interface IPdfProcessingService
    {
        List<DocumentChunk> ProcessPdf(IFormFile file);
    }
}
