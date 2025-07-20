using ChatApp.Core.Domain.Interfaces.Services;
using ChatApp.Core.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

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

        public async Task<string> QueryAsync(string message)
        {
            string promptTemplate = """
                {{#with (SearchPlugin-GetTextSearchResults query)}}
                    The following information was retrieved based on the query "{{query}}":
                    {{#each this}}
                    Name: {{Name}}
                    Value: {{Value}}
                    Link: {{Link}}
                    -----------------
                    {{/each}}
                {{/with}}

                Now, using the information above and your own knowledge:

                - If the information provided is sufficient, write a helpful and accurate answer to the question: "{{query}}"
                - If the answer cannot be determined from the search data or your own knowledge, reply: "I'm sorry, but I couldn't find enough information to answer that question."

                Return a concise, clear response.
                """;

            KernelArguments arguments = new() { { "query", message } };

            HandlebarsPromptTemplateFactory promptTemplateFactory = new();

            var result = await _kernel.InvokePromptAsync(
                promptTemplate,
                arguments,
                templateFormat: HandlebarsPromptTemplateFactory.HandlebarsTemplateFormat,
                promptTemplateFactory: promptTemplateFactory
            );

            return result.GetValue<string>() ?? string.Empty;
        }

        public async Task StoreDocumentChunksAsync(List<DocumentChunk> chunks)
        {
            try
            {
                var collection = _vectorStore.GetCollection<Guid, DocumentChunkRecord>(CollectionName);
                await collection.EnsureCollectionExistsAsync();

                var records = new List<DocumentChunkRecord>();
                int failedCount = 0;

                foreach (var chunk in chunks)
                {
                    try
                    {
                        records.Add(new DocumentChunkRecord
                        {
                            Key = Guid.NewGuid(),
                            FileName = chunk.FileName,
                            ChunkIndex = chunk.ChunkIndex,
                            Text = chunk.Text,
                            Embedding = await _embeddingService.GenerateEmbeddingAsync(chunk.Text)
                        });
                    }
                    catch (Exception ex)
                    {
                        failedCount++;
                        _logger.LogWarning(ex, "Failed to generate embedding for chunk: {ChunkIndex} from file: {FileName}", chunk.ChunkIndex, chunk.FileName);
                    }
                }

                _logger.LogInformation($"Generated embeddings for {records.Count} chunks, {failedCount} failed");
                await collection.UpsertAsync(records);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store document chunks");
                throw;
            }
        }
    }
}
