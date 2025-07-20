using ChatApp.Core.Domain.Models;
using Microsoft.SemanticKernel.Data;

namespace ChatApp.Core.Application.Mappers
{
    public class DocumentChunkRecordTextSearchStringMapper : ITextSearchStringMapper
    {
        public string MapFromResultToString(object result)
        {
            if (result is DocumentChunkRecord documentChunk)
                return documentChunk.Text;

            throw new ArgumentException("Invalid result type.");
        }
    }
}
