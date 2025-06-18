using ChatApp.Core.Domain.Interfaces.Services;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;

namespace ChatApp.Core.Application.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
        private readonly Kernel _kernel;

        public EmbeddingService(Kernel kernel)
        {
            _kernel = kernel;
            _embeddingGenerator = _kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var embedding = await _embeddingGenerator.GenerateVectorAsync(text);
            return embedding.ToArray();
        }
    }
}
