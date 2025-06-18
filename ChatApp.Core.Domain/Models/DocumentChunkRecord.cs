using Microsoft.Extensions.VectorData;

namespace ChatApp.Core.Domain.Models
{
    public class DocumentChunkRecord
    {
        [VectorStoreKey]
        public required Guid Key { get; init; }

        [VectorStoreData]
        public required string FileName { get; init; }

        [VectorStoreData]
        public required int ChunkIndex { get; init; }

        [VectorStoreData]
        public required string Content { get; init; }

        [VectorStoreVector(768)]
        public ReadOnlyMemory<float> ContentEmbedding { get; set; }
    }
}
