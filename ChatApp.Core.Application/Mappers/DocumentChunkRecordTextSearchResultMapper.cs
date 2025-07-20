using ChatApp.Core.Domain.Models;
using Microsoft.SemanticKernel.Data;

namespace ChatApp.Core.Application.Mappers
{
    public class DocumentChunkRecordTextSearchResultMapper : ITextSearchResultMapper
    {
        public TextSearchResult MapFromResultToTextSearchResult(object result)
        {
            if (result is DocumentChunkRecord documentChunk)
                return new TextSearchResult(value: documentChunk.Text) { Name = documentChunk.Key.ToString(), Link = $"{documentChunk.FileName}: {documentChunk.ChunkIndex}"};

            throw new ArgumentException("Invalid result type.");
        }
    }
}
