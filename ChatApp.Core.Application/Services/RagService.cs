using ChatApp.Core.Domain.Interfaces.Services;
using ChatApp.Core.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;

namespace ChatApp.Core.Application.Services
{
    public class RagService : IRagService
    {
        private const string CollectionName = "documents";

        private readonly ILogger<RagService> _logger;
        private readonly Kernel _kernel;
        private readonly IEmbeddingService _embeddingService;
        private readonly VectorStore _vectorStore;

        public RagService(ILogger<RagService> logger, Kernel kernel, IEmbeddingService embeddingService)
        {
            _logger = logger;
            _kernel = kernel;
            _embeddingService = embeddingService;
            _vectorStore = _kernel.GetRequiredService<VectorStore>();
        }

        public async Task<QueryResponse> QueryAsync(QueryRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task StoreDocumentChunksAsync(List<DocumentChunk> chunks)
        {
            try
            {
                var collection = _vectorStore.GetCollection<Guid, DocumentChunkRecord>(CollectionName);
                await collection.EnsureCollectionExistsAsync();

                foreach (var chunk in chunks)
                {
                    Console.WriteLine($"Generating embedding for chunk: {chunk.FileName}.{chunk.ChunkIndex}");

                    var result = new DocumentChunkRecord
                    {
                        Key = Guid.NewGuid(),
                        FileName = chunk.FileName,
                        ChunkIndex = chunk.ChunkIndex,
                        Content = chunk.Content,
                        ContentEmbedding = await _embeddingService.GenerateEmbeddingAsync(chunk.Content)
                    };

                    Console.WriteLine($"Upserting chunk: {chunk.FileName}.{chunk.ChunkIndex}");
                    await collection.UpsertAsync(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store document chunks");
                throw;
            }
        }
    }
}
